using ClientCore;
using DTAClient.DXGUI.Generic.Campaign;
using Rampastring.Tools;
using System;
using System.Collections.Generic;

namespace DTAClient.Domain.Singleplayer
{
    /// <summary>
    /// A Tiberian Sun mission listed in Battle(E).ini or INI/Campaigns.ini.
    /// </summary>
    public class Mission
    {
        private const int DifficultyLabelCount = 3;
        private const int ExtendedDifficultyLabelCount = 4;

        public Mission(IniSection iniSection, bool isCampaignMission, int index)
        {
            Index = index;
            InternalName = iniSection.SectionName;
            Side = iniSection.GetIntValue(nameof(Side), 0);
            Scenario = iniSection.GetStringValue(nameof(Scenario), string.Empty);
            GUIName = iniSection.GetStringValue("Description", "Undefined mission");
            if (iniSection.KeyExists("UIName"))
                GUIName = iniSection.GetStringValue("UIName", GUIName);

            IconPath = iniSection.GetStringValue(nameof(IconPath), string.Empty);
            GUIDescription = iniSection.GetStringValue("LongDescription", string.Empty);
            Author = iniSection.GetStringValue(nameof(Author), string.Empty);
            HeaderFor = iniSection.GetStringValue(nameof(HeaderFor), string.Empty);
            PreviewImagePath = iniSection.GetStringValue(nameof(PreviewImagePath), Scenario.Replace(".map", ".png").Replace(".MAP", ".PNG"));
            LoadingScreenPath = iniSection.GetStringValue(nameof(LoadingScreenPath), string.Empty);
            RequiredAddon = iniSection.GetBooleanValue(nameof(RequiredAddon), false);
            Enabled = iniSection.GetBooleanValue(nameof(Enabled), Enabled);
            Visible = iniSection.GetBooleanValue(nameof(Visible), Visible);
            BuildOffAlly = iniSection.GetBooleanValue(nameof(BuildOffAlly), false);
            WarnOnHardWithoutMediumPlayed = iniSection.GetBooleanValue(nameof(WarnOnHardWithoutMediumPlayed), WarnOnHardWithoutMediumPlayed);
            PlayerAlwaysOnNormalDifficulty = iniSection.GetBooleanValue(nameof(PlayerAlwaysOnNormalDifficulty), false);
            MusicRecommended = iniSection.GetBooleanValue(nameof(MusicRecommended), MusicRecommended);
            AllowBonuses = iniSection.GetBooleanValue(nameof(AllowBonuses), AllowBonuses);
            HasExtendedDifficulty = iniSection.GetBooleanValue(nameof(HasExtendedDifficulty), HasExtendedDifficulty);

            if (Enum.TryParse(iniSection.GetStringValue(nameof(StartCutscene), Cutscene.None.ToString()), out Cutscene startCutscene))
                StartCutscene = startCutscene;

            if (Enum.TryParse(iniSection.GetStringValue(nameof(EndCutscene), Cutscene.None.ToString()), out Cutscene endCutscene))
                EndCutscene = endCutscene;

            if (iniSection.KeyExists("DifficultyLabels"))
            {
                DifficultyLabels = iniSection.GetListValue("DifficultyLabels", ',', s => s).ToArray();

                if (HasExtendedDifficulty && DifficultyLabels.Length != ExtendedDifficultyLabelCount)
                {
                    throw new NotSupportedException($"Invalid number of DifficultyLabels= specified for mission {InternalName}: " +
                        $"{DifficultyLabels.Length}, expected {ExtendedDifficultyLabelCount}");
                }

                if (!HasExtendedDifficulty && DifficultyLabels.Length != DifficultyLabelCount)
                {
                    throw new NotSupportedException($"Invalid number of DifficultyLabels= specified for mission {InternalName}: " +
                        $"{DifficultyLabels.Length}, expected {DifficultyLabelCount}");
                }
            }

            CampaignInternalName = iniSection.GetStringValue(nameof(CampaignInternalName), null);
            RequiresUnlocking = iniSection.GetBooleanValue(nameof(RequiresUnlocking), isCampaignMission);
            UnlockMissions = iniSection.GetStringValue(nameof(UnlockMissions), string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            UsedGlobalVariables = iniSection.GetStringValue(nameof(UsedGlobalVariables), string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            UnlockGlobalVariables = iniSection.GetStringValue(nameof(UnlockGlobalVariables), string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            InvalidGlobalCombination = iniSection.GetStringValue(nameof(InvalidGlobalCombination), string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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
            Index = index;
        }

        public int Index { get; }
        public string InternalName { get; }
        public int Side { get; }
        public string Scenario { get; }
        public string GUIName { get; }
        public string IconPath { get; }
        public string GUIDescription { get; }
        public string Author { get; }
        public string PreviewImagePath { get; }
        public string LoadingScreenPath { get; }
        public bool RequiredAddon { get; }
        public bool Enabled { get; } = true;
        public bool Visible { get; } = true;
        public bool BuildOffAlly { get; }

        public bool PlayerAlwaysOnNormalDifficulty { get; }

        public bool MusicRecommended { get; }

        public bool AllowBonuses { get; }

        public bool HasExtendedDifficulty { get; }

        public string[] DifficultyLabels { get; }

        public string HeaderFor { get; }

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

        /// <summary>
        /// Specifies an invalid global configuration. The user is unable to launch
        /// the mission if all of the globals specified in this array are enabled.
        /// </summary>
        public string[] InvalidGlobalCombination { get; private set; }


        private int DifficultyRankToIndex(DifficultyRank rank)
        {
            if (HasExtendedDifficulty)
            {
                switch (rank)
                {
                    case DifficultyRank.BRUTAL:
                        return 3;
                    case DifficultyRank.HARD:
                        return 2;
                    case DifficultyRank.NORMAL:
                        return 1;
                    case DifficultyRank.EASY:
                        return 0;
                }
            }
            else
            {
                switch (rank)
                {
                    case DifficultyRank.BRUTAL:
                        return 2;
                    case DifficultyRank.HARD:
                        return 1;
                    case DifficultyRank.NORMAL:
                        return 1;
                    case DifficultyRank.EASY:
                        return 0;
                }
            }

            return 0;
        }

        public string GetNameForDifficultyRank(DifficultyRank rank)
        {
            if (DifficultyLabels != null)
            {
                return DifficultyLabels[DifficultyRankToIndex(rank)];
            }

            if (HasExtendedDifficulty)
            {
                switch (rank)
                {
                    case DifficultyRank.BRUTAL:
                        return "Brutal";
                    case DifficultyRank.HARD:
                        return "Hard";
                    case DifficultyRank.NORMAL:
                        return "Normal";
                    case DifficultyRank.EASY:
                        return "Easy";
                }
            }
            else
            {
                switch (rank)
                {
                    case DifficultyRank.BRUTAL:
                        return "Hard";
                    case DifficultyRank.HARD:
                        return "Normal";
                    case DifficultyRank.NORMAL:
                        return "Normal";
                    case DifficultyRank.EASY:
                        return "Easy";
                }
            }

            return "Unknown";
        }

        public string GetNameForDifficultyRankStylized(DifficultyRank rank)
        {
            string difficultyName = GetNameForDifficultyRank(rank);
            if (difficultyName.Length > 1)
                difficultyName = difficultyName.Substring(0, 1).ToUpperInvariant() + difficultyName.Substring(1).ToLowerInvariant();

            return difficultyName;
        }
    }
}
