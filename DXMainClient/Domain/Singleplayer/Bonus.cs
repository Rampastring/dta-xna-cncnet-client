namespace DTAClient.Domain.Singleplayer
{
    public class Bonus
    {
        public Bonus(string uiName, string iniName, string unlockFromMission, string description, string loreDescription, Difficulty difficulty) 
        {
            UIName = uiName;
            ININame = iniName;
            UnlockFromMission = unlockFromMission;
            Description = description;
            LoreDescription = loreDescription;
            Difficulty = difficulty;
        }

        public string UIName { get; }
        public string ININame { get; }
        public string UnlockFromMission { get; }
        public string Description { get; }
        public string LoreDescription { get; }
        public Difficulty Difficulty { get; }

        public bool Unlocked { get; set; }
    }
}
