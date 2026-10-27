using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using ClientCore;

using DTAClient.Online;

using Microsoft.Xna.Framework;

using Rampastring.Tools;
using Rampastring.XNAUI;

namespace DTAClient.Domain.Multiplayer.CnCNet
{
    public class TunnelHandler : GameComponent
    {
        /// <summary>
        /// Determines the time between pinging the current tunnel (if it's set).
        /// Configurable via NetworkDefinitions.ini ([V3TunnelNegotiation] CurrentTunnelPingIntervalSeconds).
        /// </summary>
        private static double CURRENT_TUNNEL_PING_INTERVAL => V3TunnelConfiguration.V3CurrentTunnelPingIntervalSeconds;

        /// <summary>
        /// A reciprocal to the value which determines how frequent the full tunnel
        /// refresh would be done instead of just pinging the current tunnel (1/N of
        /// current tunnel ping refreshes would be substituted by a full list refresh).
        /// Multiply by <see cref="CURRENT_TUNNEL_PING_INTERVAL"/> to get the interval
        /// between full list refreshes.
        /// Configurable via NetworkDefinitions.ini ([V3TunnelNegotiation] CyclesPerTunnelListRefresh).
        /// </summary>
        private static uint CYCLES_PER_TUNNEL_LIST_REFRESH => V3TunnelConfiguration.V3CyclesPerTunnelListRefresh;

        // Version 4 identifies a matchmaking server, which speaks the same protocol as a version 3
        // relay but will not carry games.
        private static readonly int[] SUPPORTED_TUNNEL_VERSIONS = [2, 3, CnCNetTunnel.MATCHMAKING_VERSION];
        private static TimeSpan tunnelRefreshInterval => TimeSpan.FromSeconds(CURRENT_TUNNEL_PING_INTERVAL);

        private readonly object _refreshLock = new();
        private bool _refreshInProgress = false;
        private bool _connected;
        private readonly V3TunnelCommunicator _tunnelCommunicator;

        public TunnelHandler(WindowManager wm, CnCNetManager connectionManager) : base(wm.Game)
        {
            this.wm = wm;
            this.connectionManager = connectionManager;

            wm.Game.Components.Add(this);

            Enabled = false;

            connectionManager.Connected += ConnectionManager_Connected;
            connectionManager.Disconnected += ConnectionManager_Disconnected;
            connectionManager.ConnectionLost += ConnectionManager_ConnectionLost;

            _tunnelCommunicator = new V3TunnelCommunicator();
            KeepAliveMonitor = new V3KeepAliveMonitor(_tunnelCommunicator, wm);
            _p2pEndpointDiscovery = new P2PEndpointDiscovery(_tunnelCommunicator);
        }

        public List<CnCNetTunnel> Tunnels { get; private set; } = [];

        /// <summary>
        /// The matchmaking servers peers meet on to exchange tunnel lists before negotiating,
        /// discovered from the master list by their version and restricted to official servers.
        /// Every client sees the same set, which is what lets two peers count on meeting on one.
        /// </summary>
        public List<CnCNetTunnel> MatchmakingTunnels { get; private set; } = [];

        public CnCNetTunnel CurrentTunnel { get; set; } = null;

        public static string GetTunnelAddress(CnCNetTunnel tunnel) => tunnel == null ? "0.0.0.0:0" : $"{tunnel.Address}:{tunnel.Port}";

        public bool TryGetGameTunnel(string address, out CnCNetTunnel tunnel)
        {
            tunnel = null;
            if (address == "0.0.0.0:0")
                return true;

            string[] parts = address.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out int port))
                return false;

            tunnel = Tunnels.Find(t => t.Address == parts[0] && t.Port == port && (t.Version == 2 || t.Version == 3));
            return tunnel != null;
        }
        public V3GameTunnelBridge GameTunnelBridge;

        private readonly P2PEndpointDiscovery _p2pEndpointDiscovery;

        // Reserved local game-relay UDP port for the current lobby session.
        private UdpClient _localGameSocket;

        public event EventHandler TunnelsRefreshed;

        /// <summary>
        /// Fired after the in-game tunnel bridge has stopped (also when no bridge was
        /// running), i.e. once the negotiated routes are no longer needed for game data.
        /// </summary>
        public event Action GameBridgeStopped;
        public event EventHandler CurrentTunnelPinged;
        public event Action<string, int> TunnelPinged; //address, port

        private WindowManager wm;
        private CnCNetManager connectionManager;

        private readonly Stopwatch refreshTimer = Stopwatch.StartNew();
        private TimeSpan? lastTunnelRefreshTimestamp;
        private uint skipCount = 0;

        /// <summary>
        /// How many unanswered probes a tunnel's last good ping survives before it reads as
        /// unknown. See <see cref="CnCNetTunnel.ApplyPingResult"/>.
        /// Configurable via NetworkDefinitions.ini ([V3TunnelNegotiation] RetainedPingFailures).
        /// </summary>
        private static int PING_RETAINED_FAILURES => V3TunnelConfiguration.V3RetainedPingFailures;

        /// <summary>
        /// The keepalive subsystem for negotiated V3 paths: NAT/registration refresh, live
        /// pair pings, liveness detection and the launch-time connectivity probe. Owned and
        /// ticked by this handler; consumers subscribe to and configure it directly.
        /// </summary>
        public V3KeepAliveMonitor KeepAliveMonitor { get; }

        private void DoTunnelPinged(string address, int port)
        {
            if (TunnelPinged != null)
                wm.AddCallback(TunnelPinged, address, port);
        }

        private void DoCurrentTunnelPinged()
        {
            if (CurrentTunnelPinged != null)
                wm.AddCallback(CurrentTunnelPinged, this, EventArgs.Empty);
        }

        private void ConnectionManager_Connected(object sender, EventArgs e)
        {
            _connected = true;
            InitializeTunnelCommunicator();
            Enabled = true;
        }

        private void ConnectionManager_ConnectionLost(object sender, Online.EventArguments.ConnectionLostEventArgs e)
        {
            ConnectionManager_Disconnected(sender, EventArgs.Empty);
        }

        private void ConnectionManager_Disconnected(object sender, EventArgs e)
        {
            _connected = false;
            // IRC can disconnect while the match is still using the UDP bridge.
            Enabled = GameTunnelBridge?.IsRunning == true;
            if (!Enabled)
                _tunnelCommunicator.Shutdown();
            _p2pEndpointDiscovery.ClearCache();
        }

        private void RefreshTunnelsAsync()
        {
            lock (_refreshLock)
            {
                if (_refreshInProgress)
                    return;
                _refreshInProgress = true;
            }

            Task.Run(() =>
            {
                try
                {
                    List<CnCNetTunnel> tunnels = RefreshTunnels();
                    wm.AddCallback(new Action<List<CnCNetTunnel>>(HandleRefreshedTunnels), tunnels);
                }
                finally
                {
                    lock (_refreshLock)
                    {
                        _refreshInProgress = false;
                    }
                }
            });
        }

        private void HandleRefreshedTunnels(List<CnCNetTunnel> newTunnels)
        {
            if (newTunnels.Count == 0)
            {
                TunnelsRefreshed?.Invoke(this, EventArgs.Empty);
                return;
            }

            var existingTunnels = Tunnels.ToDictionary(t => $"{t.Address}:{t.Port}");
            var updatedTunnels = new List<CnCNetTunnel>();

            foreach (var newTunnel in newTunnels)
            {
                string key = $"{newTunnel.Address}:{newTunnel.Port}";
                if (existingTunnels.TryGetValue(key, out var existingTunnel))
                {
                    // update existing tunnels
                    existingTunnel.UpdateFrom(newTunnel);
                    updatedTunnels.Add(existingTunnel);
                }
                else
                {
                    // add new tunnels
                    updatedTunnels.Add(newTunnel);
                }
            }

            // remove old tunnels
            Tunnels = updatedTunnels;

            // Official-only, and deliberately not subject to PingUnofficialCnCNetTunnels the way
            // the relay pool is: that setting is per-user, and two peers who disagree on it could
            // hold matchmaking sets with no server in common. This also keeps a third party from
            // standing one up to observe who is negotiating with whom.
            MatchmakingTunnels = Tunnels.Where(t => t.IsMatchmaking && t.Official).ToList();

            // Lobby listeners may immediately begin negotiations with newly listed servers.
            InitializeTunnelCommunicator();
            _tunnelCommunicator.AddTunnels(Tunnels);
            TunnelsRefreshed?.Invoke(this, EventArgs.Empty);

            // Group tunnels by IP address and ping each unique address. Matchmaking servers are
            // skipped: their latency is never ranked against anything.
            var tunnelsByAddress = Tunnels
                .Where(t => !t.IsMatchmaking &&
                    (UserINISettings.Instance.PingUnofficialCnCNetTunnels || t.Official || t.Recommended))
                .GroupBy(t => t.Address)
                .ToList();

            foreach (var group in tunnelsByAddress)
            {
                _ = PingAddressAndUpdateTunnelsAsync(group.Key, group.ToList());
            }

            if (CurrentTunnel != null)
            {
                var updatedTunnel = Tunnels.Find(t => t.Address == CurrentTunnel.Address && t.Port == CurrentTunnel.Port);
                if (updatedTunnel != null)
                {
                    // don't re-ping if the tunnel still exists in list, just update the tunnel instance and
                    // fire the event handler (the tunnel was already pinged when traversing the tunnel list)
                    CurrentTunnel = updatedTunnel;
                    DoCurrentTunnelPinged();
                }
                else
                {
                    // tunnel is not in the list anymore so it's not updated with a list instance and pinged
                    PingCurrentTunnelAsync();
                }
            }

        }

        /// <summary>
        /// Pings a single IP address and updates all tunnels sharing that address with the same ping result.
        /// This prevents redundant pings for tunnels on the same IP but different ports (e.g., V2 and V3 versions).
        /// </summary>
        private Task PingAddressAndUpdateTunnelsAsync(string address, List<CnCNetTunnel> tunnelsWithSameAddress)
        {
            if (tunnelsWithSameAddress.Count == 0)
                return Task.CompletedTask;

            return Task.Run(() =>
            {
                // One probe for the whole address; each tunnel then applies it to its own
                // retention state, since they can have been unreachable for different lengths
                // of time before being grouped here.
                PingValue measuredPing = tunnelsWithSameAddress[0].MeasurePing();

                foreach (var tunnel in tunnelsWithSameAddress)
                {
                    tunnel.ApplyPingResult(measuredPing, PING_RETAINED_FAILURES);
                    DoTunnelPinged(tunnel.Address, tunnel.Port);
                }
            });
        }

        private Task PingCurrentTunnelAsync(bool checkTunnelList = false)
        {
            return Task.Run(() =>
            {
                var tunnel = CurrentTunnel;
                if (tunnel == null) return;

                PingValue measuredPing = tunnel.MeasurePing();
                tunnel.ApplyPingResult(measuredPing, PING_RETAINED_FAILURES);
                DoCurrentTunnelPinged();

                if (checkTunnelList)
                {
                    DoTunnelPinged(tunnel.Address, tunnel.Port);

                    // Update all other tunnels with the same IP address
                    var otherTunnelsWithSameAddress = Tunnels.Where(t => t.Address == tunnel.Address && t != tunnel).ToList();
                    foreach (var otherTunnel in otherTunnelsWithSameAddress)
                    {
                        otherTunnel.ApplyPingResult(measuredPing, PING_RETAINED_FAILURES);
                        DoTunnelPinged(otherTunnel.Address, otherTunnel.Port);
                    }
                }
            });
        }

        /// <summary>
        /// Downloads and parses the list of CnCNet tunnels.
        /// </summary>
        /// <returns>A list of tunnel servers.</returns>
        private List<CnCNetTunnel> RefreshTunnels()
        {
            string tunnelCacheFile = ProgramConstants.GamePath + "Client/tunnel_cache";

            List<CnCNetTunnel> returnValue = new List<CnCNetTunnel>();

            using var client = new ExtendedWebClient(10000);

            byte[] data;

            Logger.Log("Fetching tunnel server info.");

            try
            {
                data = client.DownloadData(MainClientConstants.CNCNET_TUNNEL_LIST_URL);
            }
            catch (Exception ex)
            {
                Logger.Log("Error when downloading tunnel server info: " + ex.Message);
                Logger.Log("Retrying.");
                try
                {
                    data = client.DownloadData(MainClientConstants.CNCNET_TUNNEL_LIST_URL);
                }
                catch
                {
                    if (!File.Exists(tunnelCacheFile))
                    {
                        Logger.Log("Tunnel cache file doesn't exist!");
                        return returnValue;
                    }
                    else
                    {
                        Logger.Log("Fetching tunnel server list failed. Using cached tunnel data.");
                        data = File.ReadAllBytes(tunnelCacheFile);
                    }
                }
            }

            string convertedData = Encoding.Default.GetString(data);

            string[] serverList = convertedData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string serverInfo in serverList)
            {
                try
                {
                    CnCNetTunnel tunnel = CnCNetTunnel.Parse(serverInfo);

                    if (tunnel == null)
                        continue;

                    if (tunnel.RequiresPassword)
                        continue;

                    if (!SUPPORTED_TUNNEL_VERSIONS.Contains(tunnel.Version))
                        continue;

                    if (!returnValue.Any(t => t.Address == tunnel.Address && t.Port == tunnel.Port))
                        returnValue.Add(tunnel);
                }
                catch (Exception ex)
                {
                    Logger.Log("Caught an exception when parsing a tunnel server: " + ex.Message);
                }
            }

            if (returnValue.Count > 0)
            {
                try
                {
                    if (File.Exists(tunnelCacheFile))
                        File.Delete(tunnelCacheFile);
                    if (!Directory.Exists(ProgramConstants.GamePath + "Client"))
                        Directory.CreateDirectory(ProgramConstants.GamePath + "Client");
                    File.WriteAllBytes(tunnelCacheFile, data);
                }
                catch (Exception ex)
                {
                    Logger.Log("Refreshing tunnel cache file failed! Returned error: " + ex.Message);
                }
            }

            return returnValue;
        }

        public override void Update(GameTime gameTime)
        {
            TimeSpan currentTimestamp = refreshTimer.Elapsed;
            TimeSpan elapsedSinceLastRefresh = lastTunnelRefreshTimestamp.HasValue
                ? currentTimestamp - lastTunnelRefreshTimestamp.Value
                : TimeSpan.MaxValue;

            if (elapsedSinceLastRefresh > tunnelRefreshInterval)
            {
                if (skipCount % CYCLES_PER_TUNNEL_LIST_REFRESH == 0)
                {
                    skipCount = 0;
                    RefreshTunnelsAsync();
                }
                else if (CurrentTunnel != null)
                {
                    _ = PingCurrentTunnelAsync(true);
                }

                lastTunnelRefreshTimestamp = currentTimestamp;
                skipCount++;
            }

            KeepAliveMonitor.Update(GameTunnelBridge != null && GameTunnelBridge.IsRunning);

            base.Update(gameTime);
        }

        /// <summary>
        /// The local game-relay UDP port reserved for this lobby session, or null if
        /// <see cref="ReserveLocalGamePort"/> hasn't been called (or the reservation was
        /// released) yet.
        /// </summary>
        public int? ReservedGamePort => (_localGameSocket?.Client.LocalEndPoint as IPEndPoint)?.Port;

        /// <summary>
        /// Reserves (or returns the already-reserved) local UDP port used to relay traffic
        /// between the local game process and its V3 tunnels.
        /// </summary>
        public int ReserveLocalGamePort()
        {
            if (_localGameSocket != null)
                return ReservedGamePort.Value;

            _localGameSocket = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
            _localGameSocket.Client.ReceiveTimeout = 500;
            V3TunnelCommunicator.DisableIcmpPortUnreachableExceptions(_localGameSocket.Client);

            Logger.Log($"TunnelHandler: Reserved local game port {ReservedGamePort}.");
            return ReservedGamePort.Value;
        }

        /// <summary>
        /// Releases the reserved local game port. Only call this when actually leaving the
        /// lobby — the reservation is meant to survive every game launched within one lobby
        /// session, not just one.
        /// </summary>
        public void ReleaseLocalGamePort()
        {
            if (_localGameSocket == null)
                return;

            _localGameSocket.Close();
            _localGameSocket = null;
        }

        public V3GameTunnelBridge StartGameBridge(uint localId, List<V3PlayerInfo> allPlayers)
        {
            if (GameTunnelBridge != null)
                StopGameBridge();

            if (_localGameSocket == null)
                throw new InvalidOperationException("StartGameBridge: no local game port has been reserved.");

            GameTunnelBridge = new V3GameTunnelBridge(localId, _localGameSocket, allPlayers, this);
            KeepAliveMonitor.SetTargets(localId, allPlayers.Where(p => p.Id != localId && p.Tunnel != null)
                .Select(p => (p.Id, p.Tunnel)).ToList());
            GameTunnelBridge.Start();

            return GameTunnelBridge;
        }

        public void StopGameBridge()
        {
            if (GameTunnelBridge != null)
            {
                GameTunnelBridge.Stop();
                GameTunnelBridge = null;
            }

            GameBridgeStopped?.Invoke();

            if (!_connected)
            {
                Enabled = false;
                KeepAliveMonitor.ClearTargets();
                _tunnelCommunicator.Shutdown();
                ReleaseLocalGamePort();
            }
        }

        public void InitializeTunnelCommunicator()
        {
            if (_tunnelCommunicator.IsInitialized || Tunnels.Count == 0)
                return;

            _tunnelCommunicator.Initialize(Tunnels);
        }

        /// <summary>
        /// Returns the cached STUN-discovered external endpoint for this session,
        /// or discovers it by querying official tunnel servers as STUN endpoints.
        /// Returns null if the NAT is symmetric or no STUN servers respond.
        /// </summary>
#nullable enable
        public Task<IPEndPoint?> GetOrDiscoverP2PEndpointAsync() => _p2pEndpointDiscovery.GetOrDiscoverAsync(Tunnels);
#nullable restore

        /// <summary>
        /// Returns this machine's local (LAN) endpoints offered as additional P2P candidates.
        /// </summary>
        public List<IPEndPoint> GetLocalP2PEndpoints() => _p2pEndpointDiscovery.GetLocalEndpoints();

        /// <summary>
        /// Registers a P2P peer's endpoint with the communicator so packets from
        /// that address are dispatched correctly.
        /// </summary>
        public void AddP2PTunnel(P2PTunnel tunnel, uint localId, uint remoteId) => _tunnelCommunicator.AddP2PTunnel(tunnel, localId, remoteId);

        /// <summary>
        /// Removes a player pair's P2P endpoints from the communicator's routing tables,
        /// optionally preserving the chosen path's endpoint.
        /// </summary>
#nullable enable
        public void CleanupP2PPair(uint localId, uint remoteId, IPEndPoint? keepEndpoint = null) => _tunnelCommunicator.CleanupP2PPair(localId, remoteId, keepEndpoint);
#nullable restore

        /// <summary>
        /// Clears the cached STUN result so the next P2P negotiation re-queries.
        /// Call when P2P is enabled in options or after a network change.
        /// </summary>
        public void ClearP2PEndpointCache() => _p2pEndpointDiscovery.ClearCache();

        public void RegisterV3PacketHandler(uint localId, uint remoteId, PacketHandler handler) => _tunnelCommunicator.RegisterHandler(localId, remoteId, handler);

        public void UnregisterV3PacketHandler(uint localId, uint remoteId) => _tunnelCommunicator.UnregisterHandler(localId, remoteId);

        public void SendRegistrationToTunnels(uint localId, List<CnCNetTunnel> tunnels = null) => _tunnelCommunicator.SendRegistrationToTunnels(localId, tunnels);

        public void SendPacket(CnCNetTunnel tunnel, uint senderId, uint receiverId,
            TunnelPacketType packetType, byte[] payload = null) => _tunnelCommunicator.SendPacket(tunnel, senderId, receiverId, packetType, payload);

        /// <summary>
        /// Sends an already-framed packet from a caller-owned buffer. See
        /// <see cref="V3TunnelCommunicator.SendRawPacket"/>.
        /// </summary>
#nullable enable
        public void SendRawPacket(CnCNetTunnel? tunnel, byte[] packet, int length)
            => _tunnelCommunicator.SendRawPacket(tunnel, packet, length);
#nullable restore
    }
}
