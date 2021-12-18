using ClientCore;
using Rampastring.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace DTAClient.Domain.Singleplayer
{
    /// <summary>
    /// Imports and exports mission rank and unlock data.
    /// </summary>
    public static class MissionRankHandler
    {
        private const string SP_SCORE_FILE = "Client/spscore.dat";
        private const string SP_SCORE_FILE_BACKUP = "Client/spscore_backup.dat";
        private const string MISSIONS_SECTION = "Missions";
        private const string GLOBAL_VARIABLES_SECTION = "GlobalVariables";
        private const int RANK_MIN = 1;
        private const int RANK_MAX = 3;
        private const int EXPECTED_GLOBAL_VARIBLE_FIELD_COUNT = 3;

        // Data format:
        // [Missions]
        // M_TTD_LOST_POSITION=0,0 ; isunlocked, rank
        // 
        // [GlobalVariables]
        // GVAR_XXX = 0,0            ; disabled-unlocked, enabled-unlocked

        // The data is written into an INI file that then gets base64-encoded 
        // to prevent players with no programming experience from messing with it.
        // We can't prevent actual programmers from unlocking missions with this method.

        public static void LoadData(List<Mission> missions, List<CampaignGlobalVariable> globalVariables)
        {
            Logger.Log("Loading single-player mission rank data.");

            string filePath = ProgramConstants.GamePath + SP_SCORE_FILE;

            if (!File.Exists(filePath))
            {
                return;
            }

            IniFile iniFile = null;

            try
            {
                string data = File.ReadAllText(filePath, Encoding.UTF8);

                if (data.Length > 0 && data.StartsWith("["))
                {
                    // We're dealing with raw INI data (before obfuscation)
                    iniFile = new IniFile(filePath);
                }
                else
                {
                    // We're dealing with base64-encoded data (unless it's just corrupted, but let's hope it's not)

                    byte[] decoded = Convert.FromBase64String(data);

                    using (var memoryStream = new MemoryStream(decoded))
                    {
                        iniFile = new IniFile(memoryStream, Encoding.UTF8);
                    }

                }
            }
            catch (FormatException ex)
            {
                Logger.Log("FAILED to load mission competion data due to FormatException: " + ex.Message);
                return;
            }

            var missionsSection = iniFile.GetSection(MISSIONS_SECTION);
            if (missionsSection != null)
            {
                foreach (var kvp in missionsSection.Keys)
                {
                    string missionName = kvp.Key;
                    string[] unlockAndRank = kvp.Value.Split(',');
                    if (unlockAndRank.Length != 2)
                    {
                        Logger.Log("Invalid mission clear data for mission " + missionName + ": " + kvp.Value);
                        continue;
                    }

                    bool isUnlocked = unlockAndRank[0] == "1";
                    int rank = Conversions.IntFromString(unlockAndRank[1], 0);

                    Mission mission = missions.Find(m => m.InternalName == missionName);
                    if (mission != null)
                    {
                        if (mission.RequiresUnlocking)
                            mission.IsUnlocked = isUnlocked;

                        if (rank >= RANK_MIN && rank <= RANK_MAX)
                            mission.Rank = (DifficultyRank)rank;
                    }
                }
            }

            var globalVariablesSection = iniFile.GetSection(GLOBAL_VARIABLES_SECTION);
            if (globalVariablesSection != null)
            {
                foreach (var kvp in globalVariablesSection.Keys)
                {
                    string globalName = kvp.Key;
                    string[] unlocks = kvp.Value.Split(',');

                    if (unlocks.Length != EXPECTED_GLOBAL_VARIBLE_FIELD_COUNT)
                    {
                        Logger.Log("Invalid global variable unlock data for global variable " + globalName + ": " + kvp.Value);
                        continue;
                    }

                    bool isDisabledOptionUnlocked = unlocks[0] == "1";
                    bool isEnabledOptionUnlocked = unlocks[1] == "1";
                    bool isEnabledThroughPreviousScenario = unlocks[2] == "1";

                    CampaignGlobalVariable globalVariable = globalVariables.Find(gvar => gvar.InternalName == globalName);
                    if (globalVariable != null)
                    {
                        globalVariable.IsDisabledUnlocked = isDisabledOptionUnlocked;
                        globalVariable.IsEnabledUnlocked = isEnabledOptionUnlocked;
                        globalVariable.EnabledThroughPreviousScenario = isEnabledThroughPreviousScenario;
                    }
                }
            }
        }

        public static void WriteData(List<Mission> missions, List<CampaignGlobalVariable> globalVariables)
        {
            Logger.Log("Writing single-player mission rank data.");

            try
            {
                if (File.Exists(ProgramConstants.GamePath + SP_SCORE_FILE))
                {
                    File.Copy(ProgramConstants.GamePath + SP_SCORE_FILE,
                        ProgramConstants.GamePath + SP_SCORE_FILE_BACKUP, true);
                }
            }
            catch (IOException ex)
            {
                Logger.Log("FAILED to refresh back-up of SP score data due to IOException: " + ex.Message);
                return;
            }

            IniFile spScoreIni = new IniFile();
            spScoreIni.Encoding = Encoding.UTF8;

            foreach (var mission in missions)
            {
                bool isUnlocked = mission.IsUnlocked;
                int rank = (int)mission.Rank;

                if ((isUnlocked && mission.RequiresUnlocking) || rank > 0)
                {
                    spScoreIni.SetStringValue(
                        MISSIONS_SECTION,
                        mission.InternalName,
                        $"{ (isUnlocked ? "1" : "0") },{ rank.ToString(CultureInfo.InvariantCulture)}");
                }
            }

            foreach (var globalVariable in globalVariables)
            {
                if (globalVariable.IsDisabledUnlocked || globalVariable.IsEnabledUnlocked)
                {
                    spScoreIni.SetStringValue(
                        GLOBAL_VARIABLES_SECTION,
                        globalVariable.InternalName,
                        $"{ (globalVariable.IsDisabledUnlocked ? "1" : "0" ) },{ (globalVariable.IsEnabledUnlocked ? "1" : "0") },{ (globalVariable.EnabledThroughPreviousScenario ? "1" : "0") }");
                }
            }

            spScoreIni.WriteIniFile(ProgramConstants.GamePath + SP_SCORE_FILE);

            string fullINIText = File.ReadAllText(ProgramConstants.GamePath + SP_SCORE_FILE, spScoreIni.Encoding);
            byte[] bytes = spScoreIni.Encoding.GetBytes(fullINIText);
            string base64 = Convert.ToBase64String(bytes);
            File.Delete(ProgramConstants.GamePath + SP_SCORE_FILE);
            File.WriteAllText(ProgramConstants.GamePath + SP_SCORE_FILE, base64, Encoding.UTF8);

            Logger.Log("Completed writing single-player mission rank data.");
        }
    }
}
