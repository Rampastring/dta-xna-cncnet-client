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

            MissionRankHandler.LoadData(Missions, GlobalVariables, Bonuses);
        }

        public event EventHandler<MissionCompletionEventArgs> MissionRankUpdated;
        public event EventHandler<MissionCompletionEventArgs> MissionCompleted;

        private static CampaignHandler _instance;

        public List<Bonus> Bonuses { get; } = new List<Bonus>()
        {
            new Bonus("Armor", "Armor", "M_CR2", "Your units and buildings have 10% more HP.", "Some extra armor would've been helpful when trying to survive against those Gatling tanks.", new Difficulty() { Armor = 1.1 } ),
            new Bonus("Speed", "Speed", "M_CR3", "Your units move 12% faster.", "More speed would've allowed us to reinforce that base quicker.", new Difficulty() { Groundspeed = 1.008, Airspeed = 1.12 } ),
            new Bonus("Cost", "Cost", "M_CR4", "Your units and buildings cost 8% less.", "A more efficient unit production process would've helped with limited resources.", new Difficulty() { Cost = 0.92 } ),
            new Bonus("Build Time", "BuildTime", "M_CR5", "Your units and buildings build 10% faster.", "Every second counts when setting up a defensive line or preparing to quickly rush your opponent.", new Difficulty() { BuildTime = 0.91 } ),
            new Bonus("Rate of Fire", "RoF", "M_CR6", "Your units and defenses take 10% less time to reload after firing.", "Shooting faster is universally useful.", new Difficulty() { ROF = .819 } ),
            new Bonus("Firepower", "Firepower", "M_CR7", "Your units and buildings deal 10% more damage.", "Some extra firepower would've helped clear those islands a bit faster, giving our enemy less reaction time.", new Difficulty() { Firepower = 1.1 } ),
            new Bonus("Turtle", "Turtle", "M_CRA10", "Your units and buildings have 10% more HP and build 10% faster. Rate of fire is 10% slower.", "For most effective defending, you want to build defenses faster AND have them last longer! Rate of fire is not as important.", new Difficulty() { Armor = 1.10, BuildTime = 0.91, ROF = 1.001 } ),
            new Bonus("Infiltrator", "Infiltrator", "M_CRA12", "Your units and buildings have 10% more HP and deal 10% more damage. Everything costs 14% more.", "Training skilled infiltrators is expensive, but worth it for critical missions.", new Difficulty() { Armor = 1.10, Firepower = 1.10, Cost = 1.14 } ),
            new Bonus("Generalist", "Generalist", "M_CRA14", "All objects have 2% more HP, move 2% faster, cost 2% less, build 2% faster, reload 2% faster, and deal 2% more damage", "You were just following orders. Unimaginative, but still a learning experience.", 
                new Difficulty() { Armor = 1.02, Groundspeed = 1.02, Airspeed = 1.02, Cost = 0.98, BuildTime = 0.98, ROF = 0.8918, Firepower = 1.02 } ),
            new Bonus("Hit and Run", "HitAndRun", "M_CRB10", "Your units move 12% faster and your units and buildings deal 10% more damage. Rate of fire is 12% slower.", "More speed and more damage with a hit would've allowed some intense kiting with those Tank Destroyers.", new Difficulty() { Groundspeed = 1.008, Airspeed = 1.12, Firepower = 1.1, ROF = 1.0192 } ),
            new Bonus("Entrencher", "Entrencher", "M_CRB12", "Your units and buildings have 10% more HP and cost 10% less. Your units move 12% slower.", "Once you establish yourself on a shoreline, you don't allow yourself to get pushed back into the water.", new Difficulty() { Armor = 1.10, Cost = 0.90, Groundspeed = .792 } ),
            new Bonus("Nimble", "Nimble", "M_CRB13", "Your units are 24% faster. Your units and buildings have 10% less HP.", "More mobility would have helped with catching those quick Nod units. And due to their lightness, we can beat them even with less armor.", new Difficulty() { Armor = 0.9, Groundspeed = 1.116, Airspeed = 1.24 } ),
            new Bonus("Heavy", "Heavy", "M_CRB14", "Your units and buildings have 12% more HP and deal 15% more damage per shot, but take 5% longer to reload. Your units are 12% slower.", "Thicker armor helps when you need to push through defensive formations.", new Difficulty() { Armor = 1.12, Firepower = 1.15, ROF = 0.8645, Groundspeed = 0.792, Airspeed = 0.88 } ),
            new Bonus("Valiant", "ArmorAndSpeed", "M_CRB16", "Your units and buildings have 10% more HP and your units are 12% faster. Everything costs 10% more.", "With more armor and speed, you would have had an easier time forcing the fight on the enemy. Might have been a good tradeoff despite more expensive production lines.", new Difficulty() { Armor = 1.1, Groundspeed = 1.008, Airspeed = 1.12, Cost = 1.1, } ),
            new Bonus("Rapid Producer", "RapidProducer", "M_CRB17", "Your units and buildings build 20% faster. Everything costs 12% more.", "We found ourselves with plenty of Tiberium to harvest, enough to prioritize production speed over efficiency.", new Difficulty() { BuildTime=0.83, Cost=1.12 } ),
            new Bonus("Rusher", "Rusher", "M_CRC10", "Your units are 10% faster. Your units and buildings reload 10% faster after firing and are produced 10% faster. Your units and buildings have 12% less HP.", "Speed, firepower, and production speed all contribute to an effective rush. Optimally, the enemy won't have much to fire back with.", new Difficulty() { Armor = 0.88, BuildTime=0.91, Groundspeed = 0.99, Airspeed = 1.1, ROF = 0.827 } ),
            new Bonus("War Criminal", "WarCriminal", "M_CRC12", "Your units and buildings deal 30% more damage, but have 20% less HP.", "Most of the time, civilians aren't going to fire back. No armor needed, just maximum firepower to terrorize them as efficiently as possible.", new Difficulty() { Firepower = 1.3, Armor = 0.8 } ),
            new Bonus("Eagle", "Eagle", "M_CRC14", "Your aircraft are 30% faster.", "GDI's fast and deadly aircraft strikes are quite inspiring. Could we possibly do something similar if we tried?", new Difficulty() { Airspeed = 1.3 } ),
            new Bonus("Rapid Fire", "RapidFire", "M_CRC15", "Your units and defenses take 25% less time to reload after firing. Your units and buildings are 12% more expensive and deal 5% less damage per shot.", "Our machine guns were already effective, but it wouldn't have hurt to fire even faster to counter those GDI Grenadier waves.", new Difficulty() { ROF = 0.6825, Cost=1.12, Firepower=0.95 } ),
            new Bonus("Rapid Response", "RapidResponse", "M_CRC15", "Your units are 12% faster, and your units and buildings take 10% less time to build. Everything costs 10% more.", "We found ourselves flanked pretty often. More speed and faster production would have helped in those situations.", new Difficulty() { Groundspeed=1.008, BuildTime=0.9, Cost=1.1 } ),
            new Bonus("Destroyer", "Destroyer", "M_CRC16", "Your units take 10% less time to reload after firing and deal 10% more damage. Everything costs 10% more.", "You've established a reputation as a terrifying opponent. Let them know fear.", new Difficulty() { ROF=.819, Firepower=1.1, Cost=1.1 } ),
        };

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
#if false
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

                    var mission = new Mission(missionSection, true, 0);
                    campaign.Missions.Add(mission);

                    i++;
                }
            }
#endif
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

            for (int i = 0; i < battleKeys.Count; i++)
            {
                string battleEntry = battleKeys[i];

                string battleSection = battleIni.GetStringValue("Battles", battleEntry, "NOT FOUND");

                if (!battleIni.SectionExists(battleSection))
                    continue;

                IniSection section = battleIni.GetSection(battleSection);

                var mission = new Mission(section, false, i);

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

        private int DifficultyRankToInGameDifficultyLevel(DifficultyRank difficultyRank)
        {
            switch (difficultyRank)
            {
                case DifficultyRank.BRUTAL:
                    return 2;
                case DifficultyRank.HARD:
                case DifficultyRank.NORMAL:
                    return 1;
                case DifficultyRank.EASY:
                    return 0;
            }

            return 0;
        } 

        /// <summary>
        /// Writes the spawner settings file and map file for a specific mission.
        /// </summary>
        /// <param name="mission">The mission.</param>
        /// <param name="selectedDifficultyLevel">The difficulty level of the mission.</param>
        public void WriteFilesForMission(Mission mission, DifficultyRank selectedDifficultyLevel, Dictionary<int, bool> globalFlagInfo, Difficulty bonusDifficultySetting)
        {
            bool copyMapsToSpawnmapINI = ClientConfiguration.Instance.CopyMissionsToSpawnmapINI;

            int ingameDifficultyLevel = DifficultyRankToInGameDifficultyLevel(selectedDifficultyLevel);

            string difficultyName = mission.GetNameForDifficultyRankStylized(selectedDifficultyLevel);

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

                swriter.WriteLine("CampaignID=" + (-1) /*mission.Index, meh, we handle our own campaign progression so the game doesn't need to know*/);
                swriter.WriteLine("GameSpeed=" + UserINISettings.Instance.GameSpeed);
                swriter.WriteLine("Firestorm=" + mission.RequiredAddon);

                if (!string.IsNullOrWhiteSpace(mission.LoadingScreenPath))
                    swriter.WriteLine("CustomLoadScreen=" + LoadingScreenController.GetLoadScreenName(mission.LoadingScreenPath));
                else
                    swriter.WriteLine("CustomLoadScreen=" + LoadingScreenController.GetLoadScreenName(mission.Side));

                swriter.WriteLine("IsSinglePlayer=Yes");
                swriter.WriteLine("SidebarHack=" + ClientConfiguration.Instance.SidebarHack);
                swriter.WriteLine("Side=" + mission.Side);
                swriter.WriteLine("BuildOffAlly=" + mission.BuildOffAlly);
                swriter.WriteLine("DifficultyName=" + difficultyName);
                if (UserINISettings.Instance.EnableSPAutoSave)
                    swriter.WriteLine("AutoSaveGame=" + ClientConfiguration.Instance.SinglePlayerAutoSaveInterval);

                UserINISettings.Instance.Difficulty.Value = ingameDifficultyLevel;

                swriter.WriteLine("DifficultyModeHuman=" + (mission.PlayerAlwaysOnNormalDifficulty ? "1" : ingameDifficultyLevel.ToString(CultureInfo.InvariantCulture)));
                swriter.WriteLine("DifficultyModeComputer=" + GetComputerDifficulty(ingameDifficultyLevel));

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

            IniFile difficultyIni = new IniFile(ProgramConstants.GamePath + DifficultyIniPaths[ingameDifficultyLevel]);

            if (copyMapsToSpawnmapINI)
            {
                IniFile mapIni = new IniFile(ProgramConstants.GamePath + mission.Scenario);
                IniFile.ConsolidateIniFiles(mapIni, difficultyIni);

                if (selectedDifficultyLevel == DifficultyRank.NORMAL)
                {
                    string difficultyModifierIniPath = Path.Combine(Path.GetDirectoryName(mapIni.FileName), "NormalDifficultyModifiers", Path.GetFileNameWithoutExtension(mapIni.FileName) + ".ini");

                    if (File.Exists(difficultyModifierIniPath))
                    {
                        Logger.Log("Applying Normal-difficulty modifiers from " + difficultyModifierIniPath);
                        IniFile difficultyModifierIni = new IniFile(difficultyModifierIniPath);
                        IniFile.ConsolidateIniFiles(mapIni, difficultyModifierIni);
                    }
                    else
                    {
                        Logger.Log("Normal-difficulty modifier files not found! Looked at: " + difficultyModifierIniPath);
                    }
                }

                bonusDifficultySetting?.WriteToFile(mapIni);

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

            if (string.IsNullOrWhiteSpace(logFileName))
            {
                Logger.Log("WARNING: Could not parse log file after game end because no suitable log file was found!");
                return;
            }

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

            Logger.Log("Finding and unlocking bonuses related to " + mission.InternalName);
            var bonusesToUnlock = Bonuses.FindAll(b => b.UnlockFromMission == mission.InternalName);
            foreach (var bonus in bonusesToUnlock)
            {
                Logger.Log("Unlocking bonus " + bonus.ININame);
                bonus.Unlocked = true;
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

            MissionRankHandler.WriteData(Missions, GlobalVariables, Bonuses);
            MissionCompleted?.Invoke(this, new MissionCompletionEventArgs(mission, primaryUnlockedMission));
        }
    }
}
