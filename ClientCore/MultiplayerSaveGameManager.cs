using System;
using System.Collections.Generic;
using System.IO;
using Rampastring.Tools;

namespace ClientCore
{
    /// <summary>
    /// A class for handling saved multiplayer games.
    /// </summary>
    public static class MultiplayerSaveGameManager
    {
        private const string SAVED_GAMES_DIRECTORY = "Saved Games";
        public const string SPAWN_INI_NAME = "spawnSG.ini";

        public static int GetSaveGameCount()
        {
            string saveGameDirectory = GetSaveGameDirectoryPath() + "/";

            if (!AreSavedGamesAvailable())
                return 0;

            for (int i = 0; i < 1000; i++)
            {
                if (!File.Exists(saveGameDirectory + ProgramConstants.MultiplayerSaveGameFileNameFromIndex(i)))
                {
                    return i;
                }
            }

            return 1000;
        }

        public static List<string> GetSaveGameTimestamps()
        {
            int saveGameCount = GetSaveGameCount();

            List<string> timestamps = new List<string>();

            string saveGameDirectory = GetSaveGameDirectoryPath() + "/";

            for (int i = 0; i < saveGameCount; i++)
            {
                string sgPath = saveGameDirectory + ProgramConstants.MultiplayerSaveGameFileNameFromIndex(i);

                DateTime dt = File.GetLastWriteTime(sgPath);

                timestamps.Add(dt.ToString());
            }

            return timestamps;
        }

        public static bool AreSavedGamesAvailable()
        {
            if (Directory.Exists(GetSaveGameDirectoryPath()))
                return true;

            return false;
        }

        private static string GetSaveGameDirectoryPath()
        {
            return ProgramConstants.GamePath + SAVED_GAMES_DIRECTORY;
        }

        /// <summary>
        /// Initializes saved MP games for a match.
        /// </summary>
        public static bool InitSavedGames()
        {
            try
            {
                Logger.Log("Writing spawn.ini for saved game.");
                Directory.CreateDirectory(ProgramConstants.GamePath + SAVED_GAMES_DIRECTORY);
                File.Delete(ProgramConstants.GamePath + SAVED_GAMES_DIRECTORY + "/" + SPAWN_INI_NAME);
                File.Copy(ProgramConstants.GamePath + "spawn.ini", ProgramConstants.GamePath + SAVED_GAMES_DIRECTORY + "/" + SPAWN_INI_NAME, true);
            }
            catch (Exception ex)
            {
                Logger.Log("Writing spawn.ini for saved game failed! Exception message: " + ex.Message);
                return false;
            }

            return true;
        }
    }
}
