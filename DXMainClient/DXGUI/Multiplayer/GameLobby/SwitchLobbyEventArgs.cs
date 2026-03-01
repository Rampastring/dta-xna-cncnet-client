using DTAClient.Domain.Multiplayer.CnCNet;
using DTAClient.Online;
using System;

namespace DTAClient.DXGUI.Multiplayer.GameLobby
{
    public class SwitchLobbyEventArgs : EventArgs
    {
        public Channel Channel { get; }
        public string HostName { get; }
        public CnCNetTunnel Tunnel { get; }
        public bool IsHost { get; }
        public int PlayerLimit { get; }
        public string CurrentPassword { get; }
        public string NewPassword { get; }
        public bool IsCustomPassword { get; }

        public SwitchLobbyEventArgs(Channel channel, string hostName,
            CnCNetTunnel tunnel, bool isHost,
            int playerLimit, string currentPassword, string newPassword, bool isCustomPassword)
        {
            Channel = channel;
            HostName = hostName;
            Tunnel = tunnel;
            IsHost = isHost;
            PlayerLimit = playerLimit;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            IsCustomPassword = isCustomPassword;
        }
    }
}
