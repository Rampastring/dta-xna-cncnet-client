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
        public int Index { get; }

        /// <summary>
        /// The internal name (INI name) of this global variable.
        /// </summary>
        public string InternalName { get; }

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

        /// <summary>
        /// Is the "disabled" option of this global variable always selectable
        /// by the player?
        /// </summary>
        public bool DisableOptionFreeUnlock { get; set; }

        /// <summary>
        /// Should this global variable be hidden in the mission selection screen 
        /// if "Enabled" status has not been unlocked for it?
        /// </summary>
        public bool HideIfNotEnabledUnlocked { get; set; }

        /// <summary>
        /// Was this global variable enabled at the end of the previous scenario
        /// that the player played? If so, it should be enabled by
        /// default when starting the next scenario that this variable is relevant to.
        /// </summary>
        public bool EnabledThroughPreviousScenario { get; set; }

        public void InitFromIniSection(IniSection iniSection)
        {
            UIName = iniSection.GetStringValue(nameof(UIName), UIName);
            UIEnabledOption = iniSection.GetStringValue(nameof(UIEnabledOption), UIEnabledOption);
            UIDisabledOption = iniSection.GetStringValue(nameof(UIDisabledOption), UIDisabledOption);
            DisableOptionFreeUnlock = iniSection.GetBooleanValue(nameof(DisableOptionFreeUnlock), DisableOptionFreeUnlock);
            HideIfNotEnabledUnlocked = iniSection.GetBooleanValue(nameof(HideIfNotEnabledUnlocked), HideIfNotEnabledUnlocked);
        }
    }
}
