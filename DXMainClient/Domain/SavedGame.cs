using ClientCore;
using Rampastring.Tools;
using System;
using System.IO;
using OpenMcdf;

namespace DTAClient.Domain
{
    /// <summary>
    /// A single-player saved game.
    /// </summary>
    public class SavedGame
    {
        public SavedGame(string filePath, long uniqueSessionId)
        {
            FilePath = filePath;
            SessionInfo = new GameSessionInfo(GameSessionType.UNKNOWN, uniqueSessionId);
        }

        public GameSessionInfo SessionInfo { get; private set; }
        public string FilePath { get; private set; }
        public string FileName => Path.GetFileName(FilePath);
        public string GUIName { get; private set; }
        public string PlayerHouseName { get; private set; }
        public DateTime LastModified { get; private set; }
        

        /// <summary>
        /// Get the saved game's name from a .sav file.
        /// </summary>
        private void ParseCompoundFileInfo(Stream file)
        {
            var cf = new CompoundFile(file);

            GUIName = GetStringFromCompoundFile(cf, "Scenario Description");
            PlayerHouseName = GetStringFromCompoundFile(cf, "Player House");
        }

        private string GetStringFromCompoundFile(CompoundFile cf, string streamName)
        {
            byte[] bytes = cf.RootStorage.GetStream(streamName).GetData();
            string str = System.Text.Encoding.Unicode.GetString(bytes);
            str = str.TrimEnd('\0');
            return str;
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

                ParseMetadata();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("An error occured while parsing saved game " + FileName + ":" +
                    ex.Message);
                return false;
            }
        }

        private void ParseMetadata()
        {
            string metaFilePath = Path.ChangeExtension(FilePath, GameSessionManager.SavedGameMetaExtension);
            GameSessionInfo gameSessionInfo = GameSessionInfo.ParseFromFile(metaFilePath);
            if (gameSessionInfo != null)
            {
                SessionInfo = gameSessionInfo;
            }
        }
    }
}
