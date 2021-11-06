using Rampastring.Tools;

namespace DTAClient.Domain.Singleplayer
{
    /// <summary>
    /// A global variable in a campaign.
    /// </summary>
    public class CampaignGlobalVariable
    {
        public CampaignGlobalVariable() { }

        public CampaignGlobalVariable(int index, string internalName)
        {
            Index = index;
            InternalName = internalName;
        }

        /// <summary>
        /// The index of this global variable.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The internal name (INI name) of this global variable.
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// The name of this global variable as displayed in the campaign UI.
        /// </summary>
        public string UIName { get; set; } = "";

        /// <summary>
        /// The tool tip of this global variable.
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// The label to display for this option when it's enabled. Optional.
        /// </summary>
        public string UIEnabledOption { get; set; }

        /// <summary>
        /// The label to display for this option when it's disabled. Optional.
        /// </summary>
        public string UIDisabledOption { get; set; }

        /// <summary>
        /// Whether the player is allowed to enable this global variable
        /// when starting a mission that relies on this variable.
        /// </summary>
        public bool IsEnabledUnlocked { get; set; }

        /// <summary>
        /// Whether the player is allowed to disable this global variable
        /// when starting a mission that relies on this variable.
        /// </summary>
        public bool IsDisabledUnlocked { get; set; }

        public void InitFromIniSection(IniSection iniSection)
        {
            UIName = iniSection.GetStringValue(nameof(UIName), UIName);
            UIEnabledOption = iniSection.GetStringValue(nameof(UIEnabledOption), UIEnabledOption);
            UIDisabledOption = iniSection.GetStringValue(nameof(UIDisabledOption), UIDisabledOption);
        }
    }
}
