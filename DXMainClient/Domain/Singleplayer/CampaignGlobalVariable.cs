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

        /// <summary>
        /// If not null, specifies a scenario which unlocks this global's "enabled" state when the scenario has been completed.
        /// </summary>
        public string UnlockEnabledStateFromScenario { get; set; }

        /// <summary>
        /// If not null, specifies a scenario which unlocks this global's "disabled" state when the scenario has been completed.
        /// </summary>
        public string UnlockDisabledStateFromScenario { get; set; }

        /// <summary>
        /// If above 0, setting this global to the "Enabled" state forces the player's Side to the specific value.
        /// </summary>
        public int EnabledStateForcedSide { get; set; } = -1;

        /// <summary>
        /// If <see cref="EnabledStateForcedSide"/> is above 0, this defines the name of the side that the player's house should be set to belong into.
        /// </summary>
        public string EnabledStateForcedSideName { get; set; }

        public void InitFromIniSection(IniSection iniSection)
        {
            UIName = iniSection.GetStringValue(nameof(UIName), UIName);
            UIEnabledOption = iniSection.GetStringValue(nameof(UIEnabledOption), UIEnabledOption);
            UIDisabledOption = iniSection.GetStringValue(nameof(UIDisabledOption), UIDisabledOption);
            DisableOptionFreeUnlock = iniSection.GetBooleanValue(nameof(DisableOptionFreeUnlock), DisableOptionFreeUnlock);
            HideIfNotEnabledUnlocked = iniSection.GetBooleanValue(nameof(HideIfNotEnabledUnlocked), HideIfNotEnabledUnlocked);
            ToolTip = iniSection.GetStringValue(nameof(ToolTip), ToolTip);

            UnlockEnabledStateFromScenario = iniSection.GetStringValue(nameof(UnlockEnabledStateFromScenario), UnlockEnabledStateFromScenario);
            UnlockDisabledStateFromScenario = iniSection.GetStringValue(nameof(UnlockDisabledStateFromScenario), UnlockDisabledStateFromScenario);
            EnabledStateForcedSide = iniSection.GetIntValue(nameof(EnabledStateForcedSide), EnabledStateForcedSide);
            EnabledStateForcedSideName = iniSection.GetStringValue(nameof(EnabledStateForcedSideName), EnabledStateForcedSideName);
        }
    }
}
