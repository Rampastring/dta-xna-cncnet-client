using Rampastring.Tools;

namespace DTAClient.Domain.Singleplayer
{
    public class Difficulty
    {
        public string ININame { get; set; }

        public double Groundspeed { get; set; } = 0.9;
        public double Airspeed { get; set; } = 1.0;
        public double BuildTime { get; set; } = 1.0;
        public double Armor { get; set; } = 1.0;
        public double ROF { get; set; } = 0.91;
        public double Cost { get; set; } = 1.0;
        public double Firepower { get; set; } = 1.0;
        public double RepairDelay { get; set; } = .02;
        public double BuildDelay { get; set; } = .03;
        public bool BuildSlowdown { get; set; } = true;
        public bool DestroyWalls { get; set; } = true;
        public bool ContentScan { get; set; } = true;

        public void WriteToFile(IniFile iniFile, string sectionName, bool moveSectionToFirst)
        {
            var section = iniFile.GetSection(sectionName);
            if (section == null)
            {
                section = new IniSection(sectionName);
                iniFile.AddSection(section);

                if (moveSectionToFirst)
                    iniFile.MoveSectionToFirst(sectionName);
            }
            
            section.SetDoubleValue(nameof(Groundspeed), Groundspeed);
            section.SetDoubleValue(nameof(Airspeed), Airspeed);
            section.SetDoubleValue(nameof(BuildTime), BuildTime);
            section.SetDoubleValue(nameof(Armor), Armor);
            section.SetDoubleValue(nameof(ROF), ROF);
            section.SetDoubleValue(nameof(Cost), Cost);
            section.SetDoubleValue(nameof(Firepower), Firepower);
            section.SetDoubleValue(nameof(RepairDelay), RepairDelay);
            section.SetDoubleValue(nameof(BuildDelay), BuildDelay);
            section.SetBooleanValue(nameof(BuildSlowdown), BuildSlowdown);
            section.SetBooleanValue(nameof(DestroyWalls), DestroyWalls);
            section.SetBooleanValue(nameof(ContentScan), ContentScan);
        }

        public void ReadFromIniSection(IniSection iniSection)
        {
            Groundspeed = iniSection.GetDoubleValue(nameof(Groundspeed), Groundspeed);
            Airspeed = iniSection.GetDoubleValue(nameof(Airspeed), Airspeed);
            BuildTime = iniSection.GetDoubleValue(nameof(BuildTime), BuildTime);
            Armor = iniSection.GetDoubleValue(nameof(Armor), Armor);
            ROF = iniSection.GetDoubleValue(nameof(ROF), ROF);
            Cost = iniSection.GetDoubleValue(nameof(Cost), Cost);
            Firepower = iniSection.GetDoubleValue(nameof(Firepower), Firepower);
            RepairDelay = iniSection.GetDoubleValue(nameof(RepairDelay), RepairDelay);
            BuildDelay = iniSection.GetDoubleValue(nameof(BuildDelay), BuildDelay);
            BuildSlowdown = iniSection.GetBooleanValue(nameof(BuildSlowdown), BuildSlowdown);
            DestroyWalls = iniSection.GetBooleanValue(nameof(DestroyWalls), DestroyWalls);
            ContentScan = iniSection.GetBooleanValue(nameof(ContentScan), ContentScan);
        }
    }
}
