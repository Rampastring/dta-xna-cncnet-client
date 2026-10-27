using System;
using System.Collections.Generic;
using System.Linq;
using ClientCore;
using DTAClient.Online;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI;

namespace DTAClient.Domain.Multiplayer.CnCNet;

/// <summary>The small DTA adapter shared by the regular and saved-game lobbies.</summary>
internal sealed class V3LobbySession : IV3NegotiationHost
{
    private readonly TunnelHandler tunnels;
    private readonly Func<List<PlayerInfo>> getPlayers;
    private readonly Func<Channel> getChannel;
    private readonly Func<bool> getIsHost;
    private readonly Action<string, Color> addNotice;
    private readonly Action<PlayerInfo> updatePing;
    private int launchRevision;
    private bool announcedComplete;
    private bool tunnelAvailable = true;

    public V3LobbySession(TunnelHandler tunnels, WindowManager windowManager,
        Func<List<PlayerInfo>> getPlayers, Func<Channel> getChannel, Func<bool> getIsHost,
        Action<string, Color> addNotice, Action<PlayerInfo> updatePing)
    {
        this.tunnels = tunnels;
        this.getPlayers = getPlayers;
        this.getChannel = getChannel;
        this.getIsHost = getIsHost;
        this.addNotice = addNotice;
        this.updatePing = updatePing;
        Negotiator = new V3TunnelNegotiationManager(this, tunnels, windowManager);
        tunnels.TunnelsRefreshed += (sender, args) => UpdatePlayers();
    }

    public V3TunnelNegotiationManager Negotiator { get; }
    public event EventHandler NegotiationStateChanged;
    public List<PlayerInfo> Players => getPlayers();
    public string ChannelName => getChannel()?.ChannelName ?? string.Empty;
    public TunnelMode TunnelMode => TunnelModeExtensions.FromTunnel(tunnels.CurrentTunnel);
    public bool IsHost => getChannel() != null && getIsHost();
    public bool UsesV3 => TunnelMode != TunnelMode.V2Legacy;

    public void UpdatePlayers()
    {
        if (getChannel() == null)
            return;

        Negotiator.RegenerateV3PlayerInfos();
        Negotiator.StartPendingNegotiations();
        if (TunnelMode == TunnelMode.V3Static)
            Negotiator.ApplyStaticTunnel(tunnels.CurrentTunnel);
        OnNegotiationStateChanged();
    }

    public void Clear()
    {
        InvalidateLaunch();
        announcedComplete = false;
        tunnelAvailable = true;
        Negotiator.ClearAll();
    }

    public void InvalidateLaunch() => launchRevision++;

    public bool ChangeTunnel(CnCNetTunnel tunnel)
    {
        if (ProgramConstants.IsInGame || Players.Any(p => p.IsInGame))
        {
            AddNotice("Wait for all players to return before changing tunnels.", Color.Yellow);
            return false;
        }

        InvalidateLaunch();
        var oldMode = TunnelMode;
        tunnels.CurrentTunnel = tunnel;
        tunnelAvailable = true;
        Negotiator.RegenerateV3PlayerInfos();
        Negotiator.ApplyModeTransition(oldMode, TunnelMode);
        UpdatePlayers();
        return true;
    }

    public void MarkTunnelUnavailable()
    {
        tunnelAvailable = false;
        InvalidateLaunch();
    }

    public bool CanAcceptStart => tunnelAvailable;

    public void ConfirmTunnel() => tunnelAvailable = true;

    public void BroadcastTunnel()
    {
        if (IsHost)
            getChannel().SendCTCPMessage($"{TunnelNegotiationCommands.ChangeTunnelServer} {TunnelHandler.GetTunnelAddress(tunnels.CurrentTunnel)}",
                QueuedMessageType.GAME_TUNNEL_MESSAGE, 12);
    }

    public void HandleReport(string sender, string data)
    {
        if (getChannel() != null && TunnelMode == TunnelMode.V3Dynamic &&
            sender != ProgramConstants.PLAYERNAME && Players.Any(p => p.Name == sender))
            Negotiator.HandleNegotiationReportMessage(sender, data);
    }

    public void Renegotiate()
    {
        if (!IsHost || TunnelMode != TunnelMode.V3Dynamic || ProgramConstants.IsInGame ||
            Players.Any(p => p.IsInGame) || Negotiator.HasActiveNegotiations)
            return;

        getChannel().SendCTCPMessage(TunnelNegotiationCommands.RenegotiateAll, QueuedMessageType.SYSTEM_MESSAGE, 10);
        RestartNegotiations();
    }

    public void HandleRenegotiate(string sender, string hostName)
    {
        if (sender == hostName && !IsHost && TunnelMode == TunnelMode.V3Dynamic &&
            !ProgramConstants.IsInGame && !Players.Any(p => p.IsInGame))
            RestartNegotiations();
    }

    private void RestartNegotiations()
    {
        InvalidateLaunch();
        Negotiator.RestartAllNegotiations();
    }

    public void BeginHostLaunch(Action launch)
    {
        if (!IsHost || !tunnelAvailable || ProgramConstants.IsInGame || Negotiator.LaunchConnectivityCheckInProgress ||
            tunnels.GameTunnelBridge?.IsRunning == true || Players.Any(p => p.IsInGame))
            return;

        UpdatePlayers();
        if (!Negotiator.AreAllNegotiationsSuccessful())
        {
            AddNotice("Player connections are not ready. Wait for negotiation or use /RENEGOTIATE (or select a dedicated tunnel).", Color.Yellow);
            return;
        }

        if (!Negotiator.TryReserveGamePort())
        {
            AddNotice("Unable to open the local game connection.", Color.Red);
            return;
        }

        int revision = launchRevision;
        var channel = getChannel();
        var roster = Players.Select(p => (p.Name, p.Ready, p.IsInGame)).ToList();
        Action verified = () =>
        {
            if (revision != launchRevision || channel != getChannel() || !IsHost || ProgramConstants.IsInGame ||
                !roster.SequenceEqual(Players.Select(p => (p.Name, p.Ready, p.IsInGame))))
                return;

            launch();
        };

        if (TunnelMode == TunnelMode.V3Dynamic && Players.Count > 1)
            Negotiator.BeginLaunchConnectivityCheck(verified);
        else
            verified();
    }

    public bool StartBridge()
    {
        if (tunnels.GameTunnelBridge?.IsRunning == true)
            return true;

        if (!Negotiator.StartGameBridge())
        {
            AddNotice("Unable to start the local game connection.", Color.Red);
            return false;
        }

        return true;
    }

    public void SendNegotiationReport(string message) => getChannel()?.SendCTCPMessage(message,
        QueuedMessageType.GAME_NEGOTIATION_MESSAGE, 10);

    public void AddNotice(string message, Color color)
    {
        if (getChannel() != null)
            addNotice(message, color);
    }

    public void OnNegotiationStateChanged()
    {
        bool complete = TunnelMode == TunnelMode.V3Dynamic && Players.Count > 1 && Negotiator.AreAllNegotiationsSuccessful();
        if (complete && !announcedComplete)
            AddNotice("All player connections are ready.", Color.LightGreen);
        announcedComplete = complete;
        NegotiationStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnLocalNegotiationStatus(PlayerInfo player, NegotiationStatus status, int ping)
    {
        player.Ping = status == NegotiationStatus.Succeeded ? ping : -1;
        updatePing(player);
    }

    public void OnRemoteNegotiationStatus(PlayerInfo player, NegotiationStatus status, int ping)
        => OnLocalNegotiationStatus(player, status, ping);

    public void OnPairPingUpdated(PlayerInfo player, int ping)
    {
        player.Ping = ping;
        updatePing(player);
        NegotiationStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnNegotiationsRestarted() => announcedComplete = false;
}
