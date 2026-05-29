using ClientCore;
using Rampastring.Tools;
using System;
using System.IO;
using OpenMcdf;

namespace DTAClient.Domain
{
    public enum TSEngineGameType
    {
        GAME_NORMAL,     // Not multiplayer, (campaign)
        GAME_MODEM,      // modem game
        GAME_NULL_MODEM, // NULL-modem
        GAME_IPX,        // IPX Network game
        GAME_INTERNET,   // Internet H2H
        GAME_SKIRMISH,   // 1 plr vs. AI's
        GAME_WDT,        // World domination tour game
    }

    /// <summary>
    /// A single-player saved game.
    /// </summary>
    public class SavedGame
    {
        public SavedGame(string filePath)
        {
            FilePath = filePath;
        }

        // public GameSessionInfo SessionInfo { get; private set; }
        public string FilePath { get; private set; }
        public string FileName => Path.GetFileName(FilePath);
        public string GUIName { get; private set; }
        public string PlayerHouseName { get; private set; }
        public DateTime LastModified { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }

        public TSEngineGameType GameType { get; private set; }

        public int PlaythroughID { get; private set; } = -1;
        public string MissionInternalName { get; private set; }
        public int PlayerSide { get; private set; }
        public DifficultyRank ClientDifficulty { get; private set; }
        public int[] GlobalFlags { get; private set; }
        public bool IsCheatSession { get; private set; }
        public string BonusName { get; private set; }

        /// <summary>
        /// Get the saved game's name from a .sav file.
        /// </summary>
        private void ParseCompoundFileInfo(Stream file)
        {
            var cf = new CompoundFile(file);

            GUIName = GetStringFromCompoundFile(cf, "Scenario Description");
            PlayerHouseName = GetStringFromCompoundFile(cf, "Player House");
            GameType = (TSEngineGameType)GetIntFromCompoundFile(cf, "GameType");

            try
            {
                ElapsedTime = TimeSpan.FromSeconds(GetIntFromCompoundFile(cf, "Elapsed Time") / 60);
                MissionInternalName = GetStringFromCompoundFile(cf, "Mission Internal Name");
                PlaythroughID = GetIntFromCompoundFile(cf, "Playthrough ID");
                PlayerSide = GetIntFromCompoundFile(cf, "Player Side");
                ClientDifficulty = (DifficultyRank)GetIntFromCompoundFile(cf, "Client Difficulty");
                GlobalFlags = GetIntArrayFromCompoundFile(cf, "Global Flags");
                IsCheatSession = GetBoolFromCompoundFile(cf, "Cheat Session");
                BonusName = GetStringFromCompoundFile(cf, "Bonus Name");
            }
            catch { }
        }

        private string GetStringFromCompoundFile(CompoundFile cf, string streamName)
        {
            byte[] bytes = cf.RootStorage.GetStream(streamName).GetData();
            string str = System.Text.Encoding.Unicode.GetString(bytes);
            str = str.TrimEnd('\0');
            return str;
        }

        private int GetIntFromCompoundFile(CompoundFile cf, string streamName)
        {
            byte[] bytes = cf.RootStorage.GetStream(streamName).GetData();

            // Compound files typically store integers in little-endian format
            return BitConverter.ToInt32(bytes, 0);
        }

        private bool GetBoolFromCompoundFile(CompoundFile cf, string streamName)
        {
            byte[] bytes = cf.RootStorage.GetStream(streamName).GetData();

            // COM stores bool as VT_BOOL which has VARIANT_TRUE (0xFFFF) and VARIANT_FALSE (0x0000)
            // instead of a typical bool structure
            short raw = BitConverter.ToInt16(bytes, 0);
            return raw != 0;
        }

        private int[] GetIntArrayFromCompoundFile(CompoundFile cf, string streamName)
        {
            byte[] bytes = cf.RootStorage.GetStream(streamName).GetData();

            if (bytes.Length < 4)
                return Array.Empty<int>();

            int count = BitConverter.ToInt32(bytes, 0);

            int expectedSize = 4 + count * 4;

            if (bytes.Length < expectedSize)
                throw new InvalidDataException("Stream is corrupted or incomplete.");

            int[] result = new int[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = BitConverter.ToInt32(bytes, 4 + (i * 4));
            }

            return result;
        }

        /// <summary>
        /// Reads and sets the saved game's name and last modified date, and returns true if succesful.
        /// </summary>
        /// <returns>True if parsing the info was succesful, otherwise false.</returns>
        public bool ParseInfo()
        {
            try
            {
                using (Stream file = (File.Open(FilePath, FileMode.Open, FileAccess.Read)))
                {
                    ParseCompoundFileInfo(file);
                }

                LastModified = File.GetLastWriteTime(FilePath);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("An error occured while parsing saved game " + FileName + ":" +
                    ex.Message);
                return false;
            }
        }
    }
}
