namespace DTAClient
{
    public static class Helpers
    {
        public static string AutogenerateChannelPassword(string channelName, string gameRoomName)
        {
            return Rampastring.Tools.Utilities.CalculateSHA1ForString(
                    channelName + gameRoomName).Substring(0, 10);
        }
    }
}
