using Rampastring.Tools;

namespace DTAClient.Domain.Singleplayer
{
    public class Category
    {
        public string IniName { get; }
        public string DisplayName { get; }
        public string AltDisplayName { get; }
        public string ImagePath { get; }
        public bool IsGeneric { get; }

        public Category(string iniName, string displayName, string altDisplayName, string imagePath, bool isGeneric)
        {
            IniName = iniName;
            DisplayName = displayName;
            AltDisplayName = altDisplayName;
            ImagePath = imagePath;
            IsGeneric = isGeneric;

            Logger.Log($"Created category instance with values: DisplayName: {DisplayName}, AltDisplayName: {AltDisplayName}");
        }
    }
}
