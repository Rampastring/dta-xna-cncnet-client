using System;
using System.Globalization;

namespace DTAClient
{
    public static class Helpers
    {
        public static string AutogenerateChannelPassword(string channelName, string gameRoomName)
        {
            return Rampastring.Tools.Utilities.CalculateSHA1ForString(
                    channelName + gameRoomName).Substring(0, 10);
        }

        public static string TimeSpanToUserFriendlyString(TimeSpan timeSpan, bool includeSeconds)
        {
            string timeString = "";
            if (timeSpan.Days > 0)
                timeString += timeSpan.Days.ToString(CultureInfo.InvariantCulture) + " d ";

            timeString += timeSpan.Hours.ToString(CultureInfo.InvariantCulture) + " h " + timeSpan.Minutes.ToString("D2", CultureInfo.InvariantCulture) + " m";
            if (includeSeconds)
            {
                timeString += " " + timeSpan.Seconds.ToString("D2", CultureInfo.InvariantCulture) + " s";
            }

            return timeString;
        }
    }
}
