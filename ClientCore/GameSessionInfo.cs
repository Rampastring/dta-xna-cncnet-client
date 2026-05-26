using Rampastring.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ClientCore
{
    public enum GameSessionType
    {
        UNKNOWN = 0,
        SINGLEPLAYER = 1,
        SKIRMISH = 2,
        MULTIPLAYER = 3,

        SESSION_TYPE_MAX = 3
    }

    /// <summary>
    /// Contains information on a unique game session.
    /// </summary>
    public class GameSessionInfo
    {
        private const int MIN_EXPECTED_FIELD_COUNT = 6;
        private const int EXPECTED_FIELD_COUNT = 9;

        private const string NO_VALUE = "none";

        public GameSessionInfo(
            GameSessionType sessionType,
            long uniqueId,
            string missionInternalName = "",
            int sideIndex = -1,
            DifficultyRank difficulty = DifficultyRank.NONE,
            Dictionary<int, bool> globalFlagValues = null,
            bool isCheatSession = false,
            string bonusName = null)
        {
            SessionType = sessionType;
            UniqueId = uniqueId;
            MissionInternalName = missionInternalName;
            SideIndex = sideIndex;
            Difficulty = difficulty;
            GlobalFlagValues = globalFlagValues;
            IsCheatSession = isCheatSession;
            BonusName = bonusName;
        }

        public GameSessionType SessionType { get; }
        public long UniqueId { get; }
        public string MissionInternalName { get; }
        public int SideIndex { get; }
        public DifficultyRank Difficulty { get; }
        public Dictionary<int, bool> GlobalFlagValues { get; }
        public bool IsCheatSession { get; }
        public string BonusName { get; }

        public bool SessionMatches(GameSessionInfo other)
        {
            return other.SessionType == SessionType && other.UniqueId == UniqueId;
        }

        public void WriteToFile(string path, string associateFileName)
        {
            string globalFlagValues = NO_VALUE;
            if (GlobalFlagValues != null && GlobalFlagValues.Count > 0)
            {
                globalFlagValues = string.Join("|", GlobalFlagValues.Select(gflag => $"{gflag.Key.ToString(CultureInfo.InvariantCulture)}:{(gflag.Value ? "1" : "0")}"));
            }

            string meta = 
                $"{ ((int)SessionType).ToString(CultureInfo.InvariantCulture) }," +
                $"{ UniqueId.ToString(CultureInfo.InvariantCulture) }," +
                $"{ associateFileName }," +
                $"{ (string.IsNullOrWhiteSpace(MissionInternalName) ? NO_VALUE : MissionInternalName) }," +
                $"{ SideIndex.ToString(CultureInfo.InvariantCulture) }," +
                $"{ ((int)Difficulty).ToString(CultureInfo.InvariantCulture) }," + 
                globalFlagValues + "," +
                (IsCheatSession ? "1" : "0") + "," +
                $"{(string.IsNullOrWhiteSpace(BonusName) ? NO_VALUE : BonusName)}";


            byte[] bytes = Encoding.UTF8.GetBytes(meta);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)~bytes[i];
            }

            try
            {
                File.Delete(path);
                File.WriteAllBytes(path, bytes);
            }
            catch (IOException ex)
            {
                Logger.Log("FAILED to write saved game meta file for " + Path.GetFileName(path) + ": " + ex.Message);
            }
        }

        public static GameSessionInfo ParseFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Logger.Log($"{nameof(GameSessionInfo)}.{nameof(ParseFromFile)}: File not found in {path}");
                return null;
            }

            byte[] data = File.ReadAllBytes(path);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)~data[i];
            }

            string dataAsString = Encoding.UTF8.GetString(data);
            string[] parts = dataAsString.Split(new[] { ',' }, StringSplitOptions.None);
            if (parts.Length < MIN_EXPECTED_FIELD_COUNT || parts.Length > EXPECTED_FIELD_COUNT)
            {
                Logger.Log($"{nameof(GameSessionInfo)}.{nameof(ParseFromFile)}:Unexpected saved game meta file format in file {path}: {dataAsString}");
                return null;
            }

            GameSessionType sessionType = GameSessionType.UNKNOWN;

            int gameSessionTypeInt = Conversions.IntFromString(parts[0], -1);
            if (gameSessionTypeInt > -1 && gameSessionTypeInt <= (int)GameSessionType.SESSION_TYPE_MAX)
                sessionType = (GameSessionType)gameSessionTypeInt;

            if (!long.TryParse(parts[1], out long uniqueId))
            {
                Logger.Log($"{nameof(GameSessionInfo)}.{nameof(ParseFromFile)}: FAILED to parse unique ID in saved game meta file {path}: {dataAsString}");
                return null;
            }

            string missionInternalName = parts[3];
            int sideIndex = Conversions.IntFromString(parts[4], -1);
            int difficultyInt = Conversions.IntFromString(parts[5], 0);

            // For backwards compatibility
            if (difficultyInt == 1)
                difficultyInt = (int)DifficultyRank.EASY;
            else if (difficultyInt == 2)
                difficultyInt = (int)DifficultyRank.HARD;
            else if (difficultyInt == 3)
                difficultyInt = (int)DifficultyRank.BRUTAL;

            DifficultyRank difficulty = (difficultyInt < 0 || difficultyInt > (int)DifficultyRank.BRUTAL) ? DifficultyRank.NONE : (DifficultyRank)difficultyInt;

            Dictionary<int, bool> globalFlagsDictionary = null;
            if (parts.Length >= 7 && parts[6] != NO_VALUE)
            {
                string[] globalFlagParts = parts[6].Split('|');
                globalFlagsDictionary = new Dictionary<int, bool>();

                foreach (string gflagInfo in globalFlagParts)
                {
                    string[] globalIndexAndState = gflagInfo.Split(':');
                    if (globalIndexAndState.Length != 2)
                    {
                        Logger.Log($"{nameof(GameSessionInfo)}.{nameof(ParseFromFile)}: FAILED to parse global flag index and state from game session info: {gflagInfo}, complete string: {parts[6]}");
                        continue;
                    }

                    int globalIndex = int.Parse(globalIndexAndState[0], CultureInfo.InvariantCulture);
                    bool globalState = globalIndexAndState[1] == "1";

                    globalFlagsDictionary.Add(globalIndex, globalState);
                }
            }

            bool isCheatSession = false;
            if (parts.Length >= 8)
            {
                isCheatSession = parts[7] == "1";
            }

            string bonusName = string.Empty;
            if (parts.Length >= 9)
            {
                bonusName = parts[8];
            }

            return new GameSessionInfo(sessionType, uniqueId, missionInternalName, sideIndex, difficulty, globalFlagsDictionary, isCheatSession, bonusName);
        }
    }
}
