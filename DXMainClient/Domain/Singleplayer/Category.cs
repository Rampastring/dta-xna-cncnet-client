namespace DTAClient.Domain.Singleplayer
{
    public class Category
    {
        public string IniName { get; }
        public string DisplayName { get; }
        public string ImagePath { get; }
        public bool IsGeneric { get; }

        public Category(string iniName, string displayName, string imagePath, bool isGeneric)
        {
            IniName = iniName;
            DisplayName = displayName;
            ImagePath = imagePath;
            IsGeneric = isGeneric;
        }
    }
}
