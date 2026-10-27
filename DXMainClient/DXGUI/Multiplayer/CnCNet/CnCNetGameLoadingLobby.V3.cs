using System.Linq;
using ClientCore;
using ClientGUI;
using DTAClient.Domain.Multiplayer.CnCNet;
using DTAClient.DXGUI.Multiplayer.GameLobby;
using DTAClient.Online;
using Microsoft.Xna.Framework;

namespace DTAClient.DXGUI.Multiplayer.CnCNet
{
    public partial class CnCNetGameLoadingLobby
    {
        private TunnelNegotiationStatusPanel negotiationStatusPanel;

        private void InitializeNegotiationStatusPanel()
        {
            negotiationStatusPanel = new TunnelNegotiationStatusPanel(WindowManager);
            negotiationStatusPanel.RenegotiateAllRequested += (s, e) => tunnelSession.Renegotiate();
            DarkeningPanel.AddAndInitializeWithControl(WindowManager, negotiationStatusPanel);
            tunnelSession.NegotiationStateChanged += (s, e) => UpdateNegotiationUI();
            AddChatBoxCommand(new ChatBoxCommand("NEGSTATUS",
                "Toggle the tunnel negotiation status display", false, ToggleNegotiationStatus));
            AddChatBoxCommand(new ChatBoxCommand("NS",
                "Shorthand for /NEGSTATUS", false, ToggleNegotiationStatus));
        }

        private void UpdateNegotiationUI()
        {
            negotiationStatusPanel.SetIsHost(IsHost);
            if (tunnelSession.TunnelMode != TunnelMode.V3Dynamic || channel == null)
            {
                negotiationStatusPanel.Disable();
                return;
            }

            if (negotiationStatusPanel.Enabled)
                negotiationStatusPanel.UpdateNegotiationStatus(Players.Select(p => p.Name).ToList(),
                    tunnelSession.Negotiator.NegotiationData, inferInProgress: true);
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

        private void HostStartGameV3()
        {
            tunnelSession.BeginHostLaunch(() =>
            {
                string payload = tunnelSession.Negotiator.GenerateV3StartPayload();
                if (!tunnelSession.StartBridge())
                    return;

                channel.SendCTCPMessage("STARTV3 " + payload, QueuedMessageType.SYSTEM_MESSAGE, 9);
                started = true;
                Players.ForEach(p => p.IsInGame = true);
                LoadV3Game();
            });
        }

        private void HandleStartGameV3(string sender, string payload)
        {
            if (sender != hostName || !tunnelSession.CanAcceptStart || IsHost || ProgramConstants.IsInGame || !tunnelSession.UsesV3 ||
                tunnelHandler.GameTunnelBridge?.IsRunning == true)
                return;

            if (!tunnelSession.Negotiator.TryApplyV3StartPayload(payload))
            {
                AddNotice("The host sent an invalid V3 start message.", Color.Yellow);
                return;
            }

            if (!tunnelSession.StartBridge())
                return;

            started = true;
            Players.ForEach(p => p.IsInGame = true);
            LoadV3Game();
        }

        private void LoadV3Game()
        {
            try
            {
                LoadGame();
            }
            catch
            {
                tunnelHandler.StopGameBridge();
                Players.ForEach(p => p.IsInGame = false);
                started = false;
                throw;
            }
        }
    }
}
