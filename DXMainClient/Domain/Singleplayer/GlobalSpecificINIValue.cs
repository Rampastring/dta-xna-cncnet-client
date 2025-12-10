using System;

namespace DTAClient.Domain.Singleplayer
{
    public class GlobalSpecificINIValue
    {
        public GlobalSpecificINIValue()
        {
        }

        public GlobalSpecificINIValue(string globalVariableName, string iniSection, string iniKey, string iniValue)
        {
            GlobalVariableName = globalVariableName;
            INISection = iniSection;
            INIKey = iniKey;
            INIValue = iniValue;
        }

        public string GlobalVariableName { get; private set; }
        public string INISection { get; private set; }
        public string INIKey { get; private set; }
        public string INIValue { get; private set; }

        public static GlobalSpecificINIValue FromString(string str)
        {
            var parts = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
                throw new Exception("Invalid GlobalSpecificINIValue syntax: " + str);

            string value = parts[3];
            if (parts.Length > 4)
            {
                for (int i = 4; i < parts.Length; i++)
                {
                    value += "," + parts[i];
                }
            }

            return new GlobalSpecificINIValue(parts[0], parts[1], parts[2], value);
        }
    }
}
