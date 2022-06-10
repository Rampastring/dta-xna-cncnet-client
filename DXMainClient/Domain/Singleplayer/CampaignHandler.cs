using ClientCore;
using ClientCore.Statistics;
using Rampastring.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DTAClient.Domain.Singleplayer
{
    public class CampaignConfigException : Exception
    {
        public CampaignConfigException(string message) : base(message)
        {
        }
    }

    public class MissionCompletionEventArgs : EventArgs
    {
        public MissionCompletionEventArgs(Mission mission, Mission primaryUnlockedMission)
        {
            Mission = mission;
            PrimaryUnlockedMission = primaryUnlockedMission;
        }


        /// <summary>
        /// The mission that was completed.
        /// </summary>
        public Mission Mission { get; }

        /// <summary>
        /// The 'primary' mission that was unlocked by completing the mission.
        /// If set, this mission should be the next default-selected mission.
        /// </summary>
        public Mission PrimaryUnlockedMission { get; }
    }

    /// <summary>
    /// The central parser and container for campaigns and global variables.
    /// </summary>
    public class CampaignHandler
    {
        private const int GLOBAL_VARIABLE_MAX = 50;

        private CampaignHandler()
        {
            InitCampaigns();

            ReadBattleIni("INI/Battle.ini");
            ReadBattleIni("INI/" + ClientConfiguration.Instance.BattleFSFileName);

            MissionRankHandler.LoadData(Missions, GlobalVariables);
        }

        public event EventHandler<MissionCompletionEventArgs> MissionRankUpdated;
        public event EventHandler<MissionCompletionEventArgs> MissionCompleted;

        private static CampaignHandler _instance;

        public List<CampaignGlobalVariable> GlobalVariables { get; } = new List<CampaignGlobalVariable>();
        public List<Campaign> Campaigns { get; } = new List<Campaign>();

        /// <summary>
        /// A list of all missions in the entire game.
        /// </summary>
        public List<Mission> Missions = new List<Mission>();

        /// <summary>
        /// The combined mission and campaign list to be displayed in the main campaign selector menu.
        /// </summary>
        public List<Mission> BattleList = new List<Mission>();

        /// <summary>
        /// Singleton pattern. We only need one instance of this class.
        /// </summary>
        public static CampaignHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CampaignHandler();

                return _instance;
            }
        }

        private void InitCampaigns()
        {
            const string CampaignsIniPath = "INI/Campaigns.ini";
            var campaignsIni = new IniFile(ProgramConstants.GamePath + CampaignsIniPath);

            if (campaignsIni == null)
            {
                Logger.Log($"{nameof(CampaignHandler)}: {CampaignsIniPath} not found!");
                return;
            }

            ReadGlobalVariables(campaignsIni);
            ReadCampaigns(campaignsIni);
            SanityCheckCampaigns();
        }

        private void ReadGlobalVariables(IniFile campaignsIni)
        {
            const string GlobalVariablesSectionName = "GlobalVariables";
            IniSection globalVariablesSection = campaignsIni.GetSection(GlobalVariablesSectionName);
            if (globalVariablesSection == null)
            {
                Logger.Log($"{nameof(CampaignHandler)}: [{GlobalVariablesSectionName}] not found from campaign config INI!");
                return;
            }

            int i = 0;
            while (true)
            {
                string variableInternalName = globalVariablesSection.GetStringValue(i.ToString(), null);
                if (string.IsNullOrWhiteSpace(variableInternalName))
                    break;

                var globalVariable = new CampaignGlobalVariable(i, variableInternalName);
                var section = campaignsIni.GetSection(variableInternalName);

                if (section != null)
                {
                    globalVariable.InitFromIniSection(section);
                }
                else
                {
                    Logger.Log($"Section for defined global variable [{variableInternalName}] not found from campaign config INI!");
                }

                GlobalVariables.Add(globalVariable);

                i++;
            }
        }

        private void ReadCampaigns(IniFile campaignsIni)
        {
            const string CampaignsSectionName = "Campaigns";
            IniSection campaignsSection = campaignsIni.GetSection(CampaignsSectionName);
            if (campaignsSection == null)
            {
                Logger.Log($"{nameof(CampaignHandler)}: [{CampaignsSectionName}] not found from campaign config INI!");
                return;
            }

            foreach (var kvp in campaignsSection.Keys)
            {
                string campaignInternalName = kvp.Value;
                if (string.IsNullOrWhiteSpace(campaignInternalName))
                    continue;

                IniSection campaignSection = campaignsIni.GetSection(campaignInternalName);
                if (campaignSection == null)
                {
                    Logger.Log($"Section for defined campaign [{campaignInternalName}] not found from campaign config INI!");
                    continue;
                }

                var campaign = new Campaign(campaignInternalName);
                Campaigns.Add(campaign);

                int i = 0;
                while (true)
                {
                    string missionInternalName = campaignSection.GetStringValue("Mission" + i, null);
                    if (string.IsNullOrWhiteSpace(missionInternalName))
                        break;

                    IniSection missionSection = campaignsIni.GetSection(missionInternalName);
                    if (missionSection == null)
                    {
                        Logger.Log($"Section for defined campaign mission [{missionInternalName}] not found from campaign config INI!");
                        break;
                    }

                    var mission = new Mission(missionSection, true);
                    campaign.Missions.Add(mission);

                    i++;
                }
            }
        }

        /// <summary>
        /// Verifies that all mission and global variable 
        /// references point to valid missions and global variables.
        /// </summary>
        private void SanityCheckCampaigns()
        {
            Campaigns.ForEach(c => c.Missions.ForEach(m => 
            {
                if (Missions.Exists(otherMission => otherMission.InternalName == m.InternalName))
                    throw new CampaignConfigException("Mission named " + m.InternalName + " exists more than once!");

                Missions.Add(m);
            }));

            foreach (var campaign in Campaigns)
            {
                foreach (var mission in campaign.Missions)
                {
                    // I'm starting to feel a bit like a functional programmer

                    Array.ForEach(mission.UnlockMissions, VerifyMissionExists);
                    Array.ForEach(mission.UsedGlobalVariables, VerifyGlobalVariableExists);
                    Array.ForEach(mission.UnlockGlobalVariables, VerifyGlobalVariableExists);

                    mission.ConditionalMissionUnlocks.ForEach(conditionalMissionUnlock =>
                    {
                        VerifyMissionExists(conditionalMissionUnlock.UnlockMissionName);
                        conditionalMissionUnlock.PrerequisiteGlobalVariableStates.ForEach(gvc => VerifyGlobalVariableExists(gvc.GlobalVariableName));
                    });
                }
            }
        }

        private void VerifyMissionExists(string missionName)
        {
            if (!Missions.Exists(m => m.InternalName == missionName))
            {
                throw new CampaignConfigException($"Reference to singleplayer campaign mission '{missionName}' defined, " +
                    $"but the mission itself does not exist. Check the INI/Campaigns.ini file for mistakes." +
                    Environment.NewLine + Environment.NewLine +
                    $"If you are an end-user, please delete the INI/Campaigns.ini file and start the client then, or contact the game/mod authors for support.");
            }
        }

        private void VerifyGlobalVariableExists(string globalVariableName)
        {
            if (!GlobalVariables.Exists(m => m.InternalName == globalVariableName))
            {
                throw new CampaignConfigException($"Reference to global variable '{globalVariableName}' defined, " +
                    $"but the global variable itself does not exist. Check the INI/Campaigns.ini file for mistakes." +
                    Environment.NewLine + Environment.NewLine + 
                    $"If you are an end-user, please delete the INI/Campaigns.ini file and start the client then, or contact the game/mod authors for support.");
            }
        }

        public void ReadBattleIni(string path)
        {
            string battleIniPath = ProgramConstants.GamePath + path;
            if (!File.Exists(battleIniPath))
            {
                Logger.Log("File " + path + " not found. Ignoring.");
                return;
            }

            var battleIni = new IniFile(battleIniPath);

            List<string> battleKeys = battleIni.GetSectionKeys("Battles");

            if (battleKeys == null)
                return; // File exists but [Battles] doesn't

            foreach (string battleEntry in battleKeys)
            {
                string battleSection = battleIni.GetStringValue("Battles", battleEntry, "NOT FOUND");

                if (!battleIni.SectionExists(battleSection))
                    continue;

                IniSection section = battleIni.GetSection(battleSection);

                var mission = new Mission(section, false);

                BattleList.Add(mission);

                if (Missions.Exists(m => m.InternalName == mission.InternalName && !string.IsNullOrWhiteSpace(m.Scenario)))
                    throw new CampaignConfigException("Mission named " + mission.InternalName + " exists multiple times! (Maybe it exists in both " + path + " and Campaigns.ini?)");

                Missions.Add(mission);
            }
        }

        private static readonly string[] DifficultyIniPaths = new string[]
        {
            "INI/Map Code/Difficulty Easy.ini",
            "INI/Map Code/Difficulty Medium.ini",
            "INI/Map Code/Difficulty Hard.ini"
        };

        /// <summary>
        /// Writes the spawner settings file and map file for a specific mission.
        /// </summary>
        /// <param name="mission">The mission.</param>
        /// <param name="selectedDifficultyLevel">The difficulty level of the mission.</param>
        public void WriteFilesForMission(Mission mission, int selectedDifficultyLevel, Dictionary<int, bool> globalFlagInfo)
        {
            bool copyMapsToSpawnmapINI = ClientConfiguration.Instance.CopyMissionsToSpawnmapINI;

            Logger.Log("Writing spawner settings and map file for a singleplayer session.");
            File.Delete(ProgramConstants.GamePath + ProgramConstants.SPAWNER_SETTINGS);
            using (StreamWriter swriter = new StreamWriter(ProgramConstants.GamePath + ProgramConstants.SPAWNER_SETTINGS))
            {
                swriter.WriteLine("; Generated by DTA Client");
                swriter.WriteLine("[Settings]");
                if (copyMapsToSpawnmapINI)
                    swriter.WriteLine("Scenario=spawnmap.ini");
                else
                    swriter.WriteLine("Scenario=" + mission.Scenario);

                // No one wants to play on Fastest
                if (UserINISettings.Instance.GameSpeed == 0)
                    UserINISettings.Instance.GameSpeed.Value = 1;

                swriter.WriteLine("GameSpeed=" + UserINISettings.Instance.GameSpeed);
                swriter.WriteLine("Firestorm=" + mission.RequiredAddon);
                swriter.WriteLine("CustomLoadScreen=" + LoadingScreenController.GetLoadScreenName(mission.Side));
                swriter.WriteLine("IsSinglePlayer=Yes");
                swriter.WriteLine("SidebarHack=" + ClientConfiguration.Instance.SidebarHack);
                swriter.WriteLine("Side=" + mission.Side);
                swriter.WriteLine("BuildOffAlly=" + mission.BuildOffAlly);
                if (UserINISettings.Instance.EnableSPAutoSave)
                    swriter.WriteLine("AutoSaveGame=" + ClientConfiguration.Instance.SinglePlayerAutoSaveInterval);

                UserINISettings.Instance.Difficulty.Value = selectedDifficultyLevel;

                swriter.WriteLine("DifficultyModeHuman=" + (mission.PlayerAlwaysOnNormalDifficulty ? "1" : selectedDifficultyLevel.ToString()));
                swriter.WriteLine("DifficultyModeComputer=" + GetComputerDifficulty(selectedDifficultyLevel));

                swriter.WriteLine();
                swriter.WriteLine();
                swriter.WriteLine();
            }

            var spawnIni = new IniFile(ProgramConstants.GamePath + ProgramConstants.SPAWNER_SETTINGS);

            if (globalFlagInfo != null && globalFlagInfo.Count > 0)
            {
                var globalFlagsSection = new IniSection("GlobalFlags");
                spawnIni.AddSection(globalFlagsSection);

                foreach (var kvp in globalFlagInfo)
                {
                    globalFlagsSection.SetStringValue($"GlobalFlag{ kvp.Key.ToString(CultureInfo.InvariantCulture) }", kvp.Value ? "yes" : "no");
                }

                spawnIni.WriteIniFile();
            }

            IniFile difficultyIni = new IniFile(ProgramConstants.GamePath + DifficultyIniPaths[selectedDifficultyLevel]);

            if (copyMapsToSpawnmapINI)
            {
                IniFile mapIni = new IniFile(ProgramConstants.GamePath + mission.Scenario);
                IniFile.ConsolidateIniFiles(mapIni, difficultyIni);

                // Force values of EndOfGame and SkipScore as our progression tracking currently relies on them
                mapIni.SetBooleanValue("Basic", "EndOfGame", true);
                mapIni.SetBooleanValue("Basic", "SkipScore", false);
                mapIni.WriteIniFile(ProgramConstants.GamePath + "spawnmap.ini");
            }
        }

        private int GetComputerDifficulty(int selectedDifficultyLevel) =>
            Math.Abs(selectedDifficultyLevel - 2);

        /// <summary>
        /// Parses singleplayer mission completion info from the game output files.
        /// Call this when the game has exited after the user has started or loaded
        /// a singleplayer mission.
        /// </summary>
        public void PostGameExitOnSingleplayerMission(GameSessionInfo sessionInfo)
        {
            if (sessionInfo.SessionType != GameSessionType.SINGLEPLAYER)
            {
                throw new ArgumentException(nameof(CampaignHandler) + "." + nameof(PostGameExitOnSingleplayerMission) + " should only be called after playing a singleplayer mission.");
            }

            string logFileName = LogFileFinder.GetLogFilePath();

            if (!File.Exists(ProgramConstants.GamePath + logFileName))
            {
                Logger.Log("WARNING: Could not parse log file after game end because the log file was not found!");
                return;
            }

            string[] lines = File.ReadAllLines(ProgramConstants.GamePath + logFileName);
            bool scoreScreenLineFound = false;
            bool[] globalVariableStates = new bool[GLOBAL_VARIABLE_MAX];

            foreach (string line in lines)
            {
                if (line.StartsWith("ScoreScreen: Loaded "))
                    scoreScreenLineFound = true;

                // Also parse global variables from the log file
                if (line.StartsWith("Global variables: "))
                {
                    string gvarString = line.Substring(18);
                    string[] gVarValues = gvarString.Split(',');

                    for (int i = 0; i < gVarValues.Length && i < globalVariableStates.Length; i++)
                    {
                        globalVariableStates[i] = gVarValues[i] == "1";
                    }
                }
            }

            if (!scoreScreenLineFound)
            {
                Logger.Log("Relevant line not found, assuming the player did not win the mission.");
                return;
            }

            Mission mission = Missions.Find(m => m.InternalName == sessionInfo.MissionInternalName);
            if (mission == null)
            {
                Logger.Log("WARNING: Failed to set mission progression data; could not find mission " + sessionInfo.MissionInternalName);
                return;
            }

            var unlockedMissions = new List<Mission>();

            Logger.Log("Finding and unlocking missions related to " + mission.InternalName);
            foreach (string unlockMissionName in mission.UnlockMissions)
            {
                Mission otherMission = Missions.Find(m => m.InternalName == unlockMissionName);
                if (otherMission == null)
                {
                    Logger.Log("FAILED to unlock mission " + unlockMissionName + "because it was not found!");
                    continue;
                }

                if (!otherMission.IsUnlocked)
                {
                    otherMission.IsUnlocked = true;
                    unlockedMissions.Add(otherMission);
                    Logger.Log("Unlocked mission " + otherMission.InternalName);
                }
            }

            Logger.Log("Finding and unlocking conditionally unlocked missions related to " + mission.InternalName);
            foreach (var conditionalMissionUnlock in mission.ConditionalMissionUnlocks)
            {
                bool conditionsMet = true;

                foreach (var globalVariableCondition in conditionalMissionUnlock.PrerequisiteGlobalVariableStates)
                {
                    var globalVariable = GlobalVariables.Find(gv => gv.InternalName == globalVariableCondition.GlobalVariableName);

                    if (globalVariable == null)
                    {
                        Logger.Log("FAILED to check condition of global variable " + globalVariableCondition.GlobalVariableName + " because it was not found!");
                        continue;
                    }

                    if (globalVariableStates[globalVariable.Index] != globalVariableCondition.Enabled)
                    {
                        conditionsMet = false;
                        break;
                    }
                }
                
                if (conditionsMet)
                {
                    Mission otherMission = Missions.Find(m => m.InternalName == conditionalMissionUnlock.UnlockMissionName);
                    if (otherMission == null)
                    {
                        Logger.Log("FAILED to unlock conditional mission " + conditionalMissionUnlock.UnlockMissionName + " because it was not found!");
                        continue;
                    }

                    if (!otherMission.IsUnlocked)
                    {
                        otherMission.IsUnlocked = true;
                        unlockedMissions.Add(otherMission);
                        Logger.Log("Unlocked conditional mission " + mission.InternalName);
                    }
                }
            }

            Logger.Log("Finding and unlocking global variable states UNLOCKED BY " + mission.InternalName);
            foreach (var globalVariableName in mission.UnlockGlobalVariables)
            {
                var globalVariable = GlobalVariables.Find(gv => gv.InternalName == globalVariableName);

                if (globalVariable == null)
                {
                    Logger.Log("FAILED to unlock global variable " + globalVariableName + " because it was not found!");
                    continue;
                }

                if (globalVariableStates[globalVariable.Index])
                {
                    Logger.Log("Unlocked 'enabled' state of " + globalVariable.InternalName);
                    globalVariable.IsEnabledUnlocked = true;
                    globalVariable.EnabledThroughPreviousScenario = true;
                }
                else
                {
                    Logger.Log("Unlocked 'disabled' state of " + globalVariable.InternalName);
                    globalVariable.IsDisabledUnlocked = true;
                    globalVariable.EnabledThroughPreviousScenario = false;
                }
            }

            Logger.Log("Finding and setting default enabled states of global variables USED IN " + mission.InternalName);
            foreach (var globalVariableName in mission.UsedGlobalVariables)
            {
                var globalVariable = GlobalVariables.Find(gv => gv.InternalName == globalVariableName);

                if (globalVariable == null)
                {
                    Logger.Log("FAILED to set default state of global variable " + globalVariableName + " because it was not found!");
                    continue;
                }

                if (globalVariableStates[globalVariable.Index])
                {
                    Logger.Log("Set default state to 'enabled' for " + globalVariable.InternalName);
                    globalVariable.EnabledThroughPreviousScenario = true;
                }
                else
                {
                    Logger.Log("Set default state to 'disabled' for " + globalVariable.InternalName);
                    globalVariable.EnabledThroughPreviousScenario = false;
                }
            }

            Mission primaryUnlockedMission = unlockedMissions.Count > 0 ? unlockedMissions[0] : null;

            if (sessionInfo.IsCheatSession)
            {
                Logger.Log("The player finished the mission with modified files, skipping setting of completion rank.");
            }
            else if ((int)mission.Rank < (int)sessionInfo.Difficulty)
            {
                Logger.Log("Setting completion rank of " + mission.InternalName + " to " + sessionInfo.Difficulty);
                mission.Rank = sessionInfo.Difficulty;
                MissionRankUpdated?.Invoke(this, new MissionCompletionEventArgs(mission, primaryUnlockedMission));
            }

            MissionRankHandler.WriteData(Missions, GlobalVariables);
            MissionCompleted?.Invoke(this, new MissionCompletionEventArgs(mission, primaryUnlockedMission));
        }
    }
}
