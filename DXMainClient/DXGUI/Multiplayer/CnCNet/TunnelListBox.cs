using DTAClient.Domain.Multiplayer.CnCNet;
using Microsoft.Xna.Framework;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DTAClient.DXGUI.Multiplayer.CnCNet
{
    /// <summary>A list of game tunnels, with automatic per-pair V3 selection first.</summary>
    class TunnelListBox : XNAMultiColumnListBox
    {
        public TunnelListBox(WindowManager windowManager, TunnelHandler tunnelHandler) : base(windowManager)
        {
            this.tunnelHandler = tunnelHandler;
            tunnelHandler.TunnelsRefreshed += TunnelHandler_TunnelsRefreshed;
            tunnelHandler.TunnelPinged += TunnelHandler_TunnelPinged;

            int headerHeight = (int)Renderer.GetTextDimensions("Name", HeaderFontIndex).Y;
            Width = 466;
            Height = LineHeight * 12 + headerHeight + 3;
            PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 1, 1);
            AddColumn("Name", 230);
            AddColumn("Official", 70);
            AddColumn("Ping", 76);
            AddColumn("Players", 90);
            AllowRightClickUnselect = false;
            AllowKeyboardInput = true;
            TunnelHandler_TunnelsRefreshed(this, EventArgs.Empty);
        }

        public event EventHandler ListRefreshed;
        private readonly TunnelHandler tunnelHandler;
        private List<CnCNetTunnel> displayedTunnels = new List<CnCNetTunnel>();

        // Null is the upstream representation of dynamic V3 mode.
        public CnCNetTunnel SelectedTunnel => SelectedIndex > 0 && SelectedIndex <= displayedTunnels.Count
            ? displayedTunnels[SelectedIndex - 1] : null;

        public void SelectTunnel(CnCNetTunnel tunnel)
        {
            SelectedIndex = tunnel == null ? 0 : displayedTunnels.FindIndex(t => t == tunnel) + 1;
        }

        public bool IsTunnelSelected(CnCNetTunnel tunnel) => IsValidIndexSelected() && SelectedTunnel == tunnel;

        private void TunnelHandler_TunnelsRefreshed(object sender, EventArgs e)
        {
            var selected = SelectedTunnel;
            ClearItems();
            displayedTunnels = tunnelHandler.Tunnels.Where(t => t.Version == 2 || t.Version == 3).ToList();
            AddItem(new List<string> { "Automatic (V3 per-player)", "", "", "" }, true);

            foreach (var tunnel in displayedTunnels)
            {
                AddItem(new List<string>
                {
                    $"[V{tunnel.Version}] {tunnel.Name}",
                    Conversions.BooleanToString(tunnel.Official, BooleanStringStyle.YESNO),
                    tunnel.PingInMs < 0 ? "Unknown" : tunnel.PingInMs + " ms",
                    tunnel.Clients + " / " + tunnel.MaxClients
                }, true);
            }

            SelectTunnel(selected);
            ListRefreshed?.Invoke(this, EventArgs.Empty);
        }

        private void TunnelHandler_TunnelPinged(string address, int port)
        {
            int index = displayedTunnels.FindIndex(t => t.Address == address && t.Port == port);
            if (index < 0)
                return;

            var tunnel = displayedTunnels[index];
            GetItem(2, index + 1).Text = tunnel.PingInMs < 0 ? "Unknown" : tunnel.PingInMs + " ms";
        }
    }
}
