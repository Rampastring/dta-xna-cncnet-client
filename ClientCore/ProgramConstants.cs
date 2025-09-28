using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ClientCore
{
    /// <summary>
    /// Contains various static variables and constants that the client uses for operation.
    /// </summary>
    public static class ProgramConstants
    {
#if DEBUG
        public static readonly string GamePath = Application.StartupPath.Replace('\\', '/') + "/";
#else
        public static readonly string GamePath = Directory.GetParent(Application.StartupPath.TrimEnd(new char[] { '\\' })).FullName.Replace('\\', '/') + "/";
#endif

        public static string ClientUserFilesPath => GamePath + "Client/";

        public static event EventHandler PlayerNameChanged;

        public const string QRES_EXECUTABLE = "qres.dat";

        public const int AI_LEVEL_COUNT = 5;
        public const int EASY_AI_INDEX = 0;

        public const string CNCNET_PROTOCOL_REVISION = "R8";
        public const string LAN_PROTOCOL_REVISION = "RL5";
        public const int LAN_PORT = 1234;
        public const int LAN_INGAME_PORT = 1234;
        public const int LAN_LOBBY_PORT = 1232;
        public const int LAN_GAME_LOBBY_PORT = 1233;
        public const char LAN_DATA_SEPARATOR = (char)01;
        public const char LAN_MESSAGE_SEPARATOR = (char)02;
        public const string LAN_PING_COMMAND = "PING";

        public const string SPAWNMAP_INI = "spawnmap.ini";
        public const string SPAWNER_SETTINGS = "spawn.ini";
        public const string SAVED_GAME_SPAWN_INI = "Saved Games/Multiplayer/spawnSG.ini";
        public const string MAP_CACHE = "Client/MapCache.ini";

        public const int GAME_ID_MAX_LENGTH = 4;

        public static readonly Encoding LAN_ENCODING = Encoding.UTF8;

        public static string GAME_VERSION = "Undefined";
        private static string PlayerName = "No name";

        public static string PLAYERNAME
        {
            get { return PlayerName; }
            set
            {
                string oldPlayerName = PlayerName;
                PlayerName = value;
                if (oldPlayerName != PlayerName)
                    PlayerNameChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static string BASE_RESOURCE_PATH = "Resources/";
        public static string RESOURCES_DIR = BASE_RESOURCE_PATH;

        public static int LOG_LEVEL = 1;

        public static bool IsInGame { get; set; }

        public static string GetResourcePath()
        {
            return GamePath + RESOURCES_DIR;
        }

        public static string GetBaseResourcePath()
        {
            return GamePath + BASE_RESOURCE_PATH;
        }

        public const string GAME_INVITE_CTCP_COMMAND = "INVITE";
        public const string GAME_INVITATION_FAILED_CTCP_COMMAND = "INVITATION_FAILED";

        public static string GetAILevelName(int aiLevel)
        {
            switch (aiLevel)
            {
                case 5:
                    return "Ultimate AI";
                case 4:
                    return "Extreme AI";
                case 3:
                    return "Brutal AI";
                case 2:
                    return "Hard AI";
                case 1:
                    return "Medium AI";
                case 0:
                    return "Easy AI";
                default:
                    return string.Empty;
            }
        }

        public static string GetAILevelDescription(int aiLevel)
        {
            switch (aiLevel)
            {
                case 5:
                    return "Ultimate AI. Has full intelligence and aggressiveness, and massive bonus income and build speed." + Environment.NewLine + Environment.NewLine +
                        "For top-tier DTA players who want a challenge despite an imbalanced map or team setup, like 2 players vs 1 AI.";
                case 4:
                    return "Extreme AI. Has full intelligence and aggressiveness, and significant bonus income and build speed." + Environment.NewLine + Environment.NewLine +
                        "For top-tier DTA players.";
                case 3:
                    return "Brutal AI. Has full intelligence and aggressiveness, and a little bit of bonus income and build speed." + Environment.NewLine + Environment.NewLine +
                        "For experienced DTA players who seek a tough challenge.";
                case 2:
                    return "Hard AI. Has full intelligence and aggressiveness." + Environment.NewLine + Environment.NewLine +
                        "For experienced C&C players who seek a reasonable challenge.";
                case 1:
                    return "Medium AI. Has limited intelligence and aggressiveness, and a slight penalty to build speed." + Environment.NewLine + Environment.NewLine +
                        "For players with some previous C&C or RTS experience, or skilled players who want a casual match.";
                case 0:
                    return "Easy AI. Has reduced intelligence and aggressiveness, and a large penalty to build speed." + Environment.NewLine + Environment.NewLine +
                        "For new players who are new to Command & Conquer or RTS games.";
                default:
                    return string.Empty;
            }
        }

        public static string DifficultyRankToName(DifficultyRank difficultyRank, bool useExtendedDifficulty)
        {
            if (!useExtendedDifficulty)
            {
                switch (difficultyRank)
                {
                    case DifficultyRank.EASY:
                        return "Easy";
                    case DifficultyRank.NORMAL:
                        return "Normal";
                    case DifficultyRank.HARD:
                        return "Normal";
                    case DifficultyRank.BRUTAL:
                        return "Hard";
                    default:
                        return "Unknown Difficulty";
                }
            }
            else
            {
                switch (difficultyRank)
                {
                    case DifficultyRank.EASY:
                        return "Easy";
                    case DifficultyRank.NORMAL:
                        return "Normal";
                    case DifficultyRank.HARD:
                        return "Hard";
                    case DifficultyRank.BRUTAL:
                        return "Brutal";
                    default:
                        return "Unknown Difficulty";
                }
            }
        }
    }
}
