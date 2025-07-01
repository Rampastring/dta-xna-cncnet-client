using System;
using System.Globalization;

namespace ClientCore
{
    public static class LoadingScreenController
    {
        public static string GetLoadScreenName(int sideId)
        {
            string loadingScreenName = ProgramConstants.BASE_RESOURCE_PATH + "l" + GetResolutionLabel();
            loadingScreenName = loadingScreenName + "s" + sideId.ToString(CultureInfo.InvariantCulture);

            Random random = new Random();
            int randomInt = random.Next(1, 1 + ClientConfiguration.Instance.LoadingScreenCount);

            loadingScreenName = loadingScreenName + randomInt.ToString(CultureInfo.InvariantCulture);
            loadingScreenName = loadingScreenName + ".pcx";

            return loadingScreenName;
        }

        public static string GetLoadScreenName(string baseName)
        {
            return baseName + GetResolutionLabel() + ".pcx";
        }

        private static string GetResolutionLabel()
        {
            int resHeight = UserINISettings.Instance.ScaledScreenHeight;
            if (resHeight < 480)
                return "400";
            else if (resHeight < 600)
                return "480";
            else
                return "600";
        }
    }
}
