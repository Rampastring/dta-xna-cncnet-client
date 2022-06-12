using ClientCore;
using DTAClient.DXGUI.Generic.Campaign;
using Rampastring.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DTAClient.Domain.Singleplayer
{
    /// <summary>
    /// A Tiberian Sun mission listed in Battle(E).ini or INI/Campaigns.ini.
    /// </summary>
    public class Mission
    {
        private const int DifficultyLabelCount = 3;

        public Mission(IniSection iniSection, bool isCampaignMission)
        {
            InternalName = iniSection.SectionName;
            Side = iniSection.GetIntValue(nameof(Side), 0);
            Scenario = iniSection.GetStringValue(nameof(Scenario), string.Empty);
            GUIName = iniSection.GetStringValue("Description", "Undefined mission");
            if (iniSection.KeyExists("UIName"))
                GUIName = iniSection.GetStringValue("UIName", GUIName);

            IconPath = iniSection.GetStringValue(nameof(IconPath), string.Empty);
            GUIDescription = iniSection.GetStringValue("LongDescription", string.Empty);
            PreviewImagePath = iniSection.GetStringValue(nameof(PreviewImagePath), string.Empty);
            RequiredAddon = iniSection.GetBooleanValue(nameof(RequiredAddon), false);
            Enabled = iniSection.GetBooleanValue(nameof(Enabled), true);
            BuildOffAlly = iniSection.GetBooleanValue(nameof(BuildOffAlly), false);
            WarnOnHardWithoutMediumPlayed = iniSection.GetBooleanValue(nameof(WarnOnHardWithoutMediumPlayed), WarnOnHardWithoutMediumPlayed);
            PlayerAlwaysOnNormalDifficulty = iniSection.GetBooleanValue(nameof(PlayerAlwaysOnNormalDifficulty), false);
            MusicRecommended = iniSection.GetBooleanValue(nameof(MusicRecommended), MusicRecommended);

            if (Enum.TryParse(iniSection.GetStringValue(nameof(StartCutscene), Cutscene.None.ToString()), out Cutscene startCutscene))
                StartCutscene = startCutscene;

            if (Enum.TryParse(iniSection.GetStringValue(nameof(EndCutscene), Cutscene.None.ToString()), out Cutscene endCutscene))
                EndCutscene = endCutscene;

            if (iniSection.KeyExists("DifficultyLabels"))
            {
                DifficultyLabels = iniSection.GetListValue("DifficultyLabels", ',', s => s).ToArray();

                if (DifficultyLabels.Length != DifficultyLabelCount)
                {
                    throw new NotSupportedException($"Invalid number of DifficultyLabels= specified for mission { InternalName }: " +
                        $"{DifficultyLabels.Length}, expected {DifficultyLabelCount}");
                }
            }

            CampaignInternalName = iniSection.GetStringValue(nameof(CampaignInternalName), null);
            RequiresUnlocking = iniSection.GetBooleanValue(nameof(RequiresUnlocking), isCampaignMission);
            UnlockMissions = iniSection.GetStringValue(nameof(UnlockMissions), string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            UsedGlobalVariables = iniSection.GetStringValue(nameof(UsedGlobalVariables), string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            UnlockGlobalVariables = iniSection.GetStringValue(nameof(UnlockGlobalVariables), string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Parse conditional mission unlocks
            int i = 0;
            while (true)
            {
                string conditionalMissionUnlockData = iniSection.GetStringValue("ConditionalMissionUnlock" + i, null);
                if (string.IsNullOrWhiteSpace(conditionalMissionUnlockData))
                    break;

                var conditionalMissionUnlock = ConditionalMissionUnlock.FromString(conditionalMissionUnlockData);
                if (conditionalMissionUnlock != null)
                    ConditionalMissionUnlocks.Add(conditionalMissionUnlock);

                i++;
            }

            GUIDescription = GUIDescription.Replace("@", Environment.NewLine);
        }

        public string InternalName { get; }
        public int Side { get; }
        public string Scenario { get; }
        public string GUIName { get; }
        public string IconPath { get; }
        public string GUIDescription { get; }
        public string PreviewImagePath { get; }
        public bool RequiredAddon { get; }
        public bool Enabled { get; }
        public bool BuildOffAlly { get; }

        public bool PlayerAlwaysOnNormalDifficulty { get; }

        public bool MusicRecommended { get; }

        public string[] DifficultyLabels { get; }

        /// <summary>
        /// Which cutscene should be played when this mission is started?
        /// </summary>
        public Cutscene StartCutscene { get; } = Cutscene.None;

        /// <summary>
        /// Which cutscene should be played when this mission is finished?
        /// </summary>
        public Cutscene EndCutscene { get; } = Cutscene.None;

        /// <summary>
        /// Should the player be given a warning when starting 
        /// this mission on Hard if they haven't beat the mission on Medium first?
        /// </summary>
        public bool WarnOnHardWithoutMediumPlayed { get; } = false;

        /// <summary>
        /// If not null, this is not a mission but a dummy entry for a campaign.
        /// </summary>
        public string CampaignInternalName { get; }

        /// <summary>
        /// Is this a mission that is unlocked by playing other missions?
        /// </summary>
        public bool RequiresUnlocking { get; private set; }

        /// <summary>
        /// If this is a mission that requires unlocking,
        /// has the player unlocked this mission?
        /// </summary>
        public bool IsUnlocked { get; set; }

        public bool IsAvailableToPlay => !RequiresUnlocking || IsUnlocked;

        /// <summary>
        /// Which difficulty level has the player beat this mission on, if any?
        /// </summary>
        public DifficultyRank Rank { get; set; }

        /// <summary>
        /// The internal names of missions that winning this mission unlocks
        /// directly.
        /// </summary>
        public string[] UnlockMissions { get; private set; }

        public List<ConditionalMissionUnlock> ConditionalMissionUnlocks { get; } = new List<ConditionalMissionUnlock>(0);

        /// <summary>
        /// The global variables that this mission utilizes.
        /// </summary>
        public string[] UsedGlobalVariables { get; private set; }

        /// <summary>
        /// The global variables that winning this mission unlocks.
        /// </summary>
        public string[] UnlockGlobalVariables { get; private set; }
    }
}
