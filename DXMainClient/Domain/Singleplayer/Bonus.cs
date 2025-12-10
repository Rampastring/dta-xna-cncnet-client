using Rampastring.Tools;
using System;

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

        public Bonus(string iniName)
        {
            ININame = iniName;
            Difficulty = new Difficulty();
        }

        public string ININame { get; }
        public string UIName { get; private set; } = "Unnamed bonus";
        public string UnlockFromMission { get; private set; }
        public string Description { get; private set; } = "No description";
        public string LoreDescription { get; private set; } = "No lore description";
        public string CampaignID { get; private set; }
        public Difficulty Difficulty { get; private set; }

        public bool Unlocked { get; set; }

        public void WriteToIni(IniFile iniFile)
        {
            Difficulty.ININame = ININame;

            Difficulty.WriteToFile(iniFile, false);

            var section = iniFile.GetSection(ININame);
            section.SetStringValue(nameof(UIName), UIName);
            section.SetStringValue(nameof(UnlockFromMission), UnlockFromMission);
            section.SetStringValue(nameof(Description), Description);
            section.SetStringValue(nameof(LoreDescription), LoreDescription);
            section.SetStringValue(nameof(CampaignID), CampaignID);
        }

        public static Bonus FromIniSection(IniSection iniSection, string iniName)
        {
            if (iniSection == null)
                throw new ArgumentException($"{nameof(Bonus)}.{nameof(FromIniSection)}: {nameof(iniSection)} cannot be null or empty!");

            if (string.IsNullOrWhiteSpace(iniName))
                throw new ArgumentException($"{nameof(Bonus)}.{nameof(FromIniSection)}: {nameof(iniName)} cannot be null or empty!");

            var bonus = new Bonus(iniName);

            bonus.UIName = iniSection.GetStringValue(nameof(UIName), bonus.UIName);
            bonus.UnlockFromMission = iniSection.GetStringValue(nameof(UnlockFromMission), bonus.UnlockFromMission);
            bonus.Description = iniSection.GetStringValue(nameof(Description), bonus.Description);
            bonus.LoreDescription = iniSection.GetStringValue(nameof(LoreDescription), bonus.LoreDescription);
            bonus.CampaignID = iniSection.GetStringValue(nameof(CampaignID), bonus.CampaignID);

            bonus.Difficulty.ReadFromIniSection(iniSection);

            return bonus;
        }
    }
}
