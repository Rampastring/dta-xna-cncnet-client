using System;
using System.Linq;
using ClientCore;
using ClientGUI;
using DTAClient.Domain.Multiplayer;
using DTAClient.Domain.Multiplayer.CnCNet;
using DTAClient.Online;
using Microsoft.Xna.Framework;

namespace DTAClient.DXGUI.Multiplayer.GameLobby
{
    public partial class CnCNetGameLobby
    {
        private TunnelNegotiationStatusPanel negotiationStatusPanel;
        private XNAClientButton btnNegotiationStatus;

        private void InitializeNegotiationStatusPanel()
        {
            negotiationStatusPanel = new TunnelNegotiationStatusPanel(WindowManager);
            negotiationStatusPanel.DrawOrder = 10;
            negotiationStatusPanel.UpdateOrder = 10;
            negotiationStatusPanel.RenegotiateAllRequested += (s, e) => tunnelSession.Renegotiate();
            AddChild(negotiationStatusPanel);

            btnNegotiationStatus = FindChild<XNAClientButton>(nameof(btnNegotiationStatus), true);
            if (btnNegotiationStatus == null)
            {
                btnNegotiationStatus = new XNAClientButton(WindowManager);
                btnNegotiationStatus.Name = nameof(btnNegotiationStatus);
                btnNegotiationStatus.Text = "Connections";
                btnNegotiationStatus.ClientRectangle = new Rectangle(
                    (btnLoadGame ?? btnChangeTunnel).X - UIDesignConstants.BUTTON_WIDTH_133 - UIDesignConstants.CONTROL_HORIZONTAL_MARGIN * 2,
                    btnChangeTunnel.Y, UIDesignConstants.BUTTON_WIDTH_133, UIDesignConstants.BUTTON_HEIGHT);
                AddChild(btnNegotiationStatus);
            }
            btnNegotiationStatus.LeftClick += (s, e) => ToggleNegotiationStatus(string.Empty);
            tunnelSession.NegotiationStateChanged += (s, e) => UpdateNegotiationUI();
        }

        private void UpdateNegotiationUI()
        {
            bool usesDynamicTunnels = tunnelSession.TunnelMode == TunnelMode.V3Dynamic;
            btnNegotiationStatus.AllowClick = usesDynamicTunnels;
            negotiationStatusPanel.SetIsHost(IsHost);
            if (!usesDynamicTunnels || channel == null)
            {
                negotiationStatusPanel.Disable();
                return;
            }

            if (negotiationStatusPanel.Enabled)
                negotiationStatusPanel.UpdateNegotiationStatus(Players.Select(p => p.Name).ToList(),
                    tunnelSession.Negotiator.NegotiationData, inferInProgress: true);
        }

        protected override void CopyPlayerDataToUI()
        {
            base.CopyPlayerDataToUI();
            if (negotiationStatusPanel != null)
                UpdateNegotiationUI();
        }

        private void ToggleNegotiationStatus(string args)
        {
            if (tunnelSession.TunnelMode != TunnelMode.V3Dynamic)
            {
                AddNotice("Negotiation status is only available when using dynamic tunnels.");
                return;
            }

            if (negotiationStatusPanel.Enabled)
                negotiationStatusPanel.Disable();
            else
            {
                negotiationStatusPanel.Enable();
                UpdateNegotiationUI();
            }
        }

        protected override void UpdatePlayerPingIndicator(PlayerInfo pInfo)
        {
            if (tunnelSession.TunnelMode != TunnelMode.V3Dynamic)
            {
                base.UpdatePlayerPingIndicator(pInfo);
                return;
            }

            var indicator = ddPlayerNames[pInfo.Index];
            if (pInfo.Name == ProgramConstants.PLAYERNAME)
            {
                indicator.Items[0].Texture = null;
                indicator.ToolTip.Text = string.Empty;
                return;
            }

            var data = tunnelSession.Negotiator.NegotiationData;
            var status = data.GetNegotiationStatus(ProgramConstants.PLAYERNAME, pInfo.Name);
            var ping = data.GetPing(ProgramConstants.PLAYERNAME, pInfo.Name) ?? PingValue.Unknown;
            var tier = status == NegotiationStatus.Succeeded ? PingQualityRules.GetV3Tier(ping)
                : status == NegotiationStatus.Failed ? PingQualityTier.Bad : PingQualityTier.Unknown;
            indicator.Items[0].Texture = PingTextures[PingQualityVisuals.GetTextureIndex(tier)];
            indicator.ToolTip.Text = BuildV3Tooltip(tunnelSession.Negotiator.FindPlayer(pInfo.Name), status, ping);
        }

        private string BuildV3Tooltip(V3PlayerInfo v3Info, NegotiationStatus status, PingValue ping)
        {
            if (status == NegotiationStatus.InProgress)
                return "Negotiating tunnel...";

            if (status == NegotiationStatus.Failed)
                return "Tunnel negotiation failed";

            if (v3Info?.Tunnel != null && status == NegotiationStatus.Succeeded)
            {
                string tooltip = "Ping: " + ping + "\nTunnel: " + v3Info.Tunnel.Name;
                if (v3Info.NegotiatedPacketLoss.HasValue)
                    tooltip += "\nPacket Loss: " + $"{v3Info.NegotiatedPacketLoss.Value:F1}%";

                return tooltip;
            }

            return "Waiting for tunnel negotiation";
        }

        private void HostLaunchGameV3()
        {
            tunnelSession.BeginHostLaunch(() =>
            {
                if (!Locked)
                    return;

                string payload = tunnelSession.Negotiator.GenerateV3StartPayload();
                if (!tunnelSession.StartBridge())
                    return;

                channel.SendCTCPMessage($"STARTV3 {UniqueGameID};{payload}", QueuedMessageType.SYSTEM_MESSAGE, 10);
                Players.ForEach(p => p.IsInGame = true);
                StartGame();
            });
        }

        private void NonHostLaunchGameV3(string sender, string message)
        {
            if (sender != hostName || !tunnelSession.CanAcceptStart || IsHost || ProgramConstants.IsInGame || !tunnelSession.UsesV3 ||
                tunnelHandler.GameTunnelBridge?.IsRunning == true)
                return;

            int separator = message.IndexOf(';');
            if (separator < 0 || !int.TryParse(message.Substring(0, separator), out int gameId) || gameId < 0 ||
                !tunnelSession.Negotiator.TryApplyV3StartPayload(message.Substring(separator + 1)))
            {
                AddNotice("The host sent an invalid V3 start message.", Color.Yellow);
                return;
            }

            if (!tunnelSession.StartBridge())
                return;

            UniqueGameID = gameId;
            Players.ForEach(p => p.IsInGame = true);
            StartGame();
        }
    }
}
