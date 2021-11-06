using Rampastring.Tools;
using System.Collections.Generic;

namespace DTAClient.Domain.Singleplayer
{
    public class Campaign
    {
        public Campaign(string internalName)
        {
            InternalName = internalName;
        }

        public string InternalName { get; }
        public string UIName { get; private set; }

        public List<Mission> Missions { get; } = new List<Mission>();

        public void InitFromINISection(IniSection iniSection, IniFile campaignsIni)
        {
            UIName = iniSection.GetStringValue(nameof(UIName), UIName);
        }
    }
}
