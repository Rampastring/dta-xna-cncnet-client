using ClientCore;
using Rampastring.Tools;
using System;
using System.IO;
using OpenMcdf;
using System.Text;

namespace DTAClient.Domain
{
    /// <summary>
    /// A single-player saved game.
    /// </summary>
    public class SavedGame
    {
        private const string SAVED_GAME_PATH = "Saved Games/";

        public SavedGame(string filePath, long uniqueSessionId)
        {
            FilePath = filePath;
            UniqueSessionId = uniqueSessionId;
        }

        public long UniqueSessionId { get; }
        public GameSessionType SessionType { get; private set; } = GameSessionType.UNKNOWN;
        public string FilePath { get; private set; }
        public string FileName => Path.GetFileName(FilePath);
        public string GUIName { get; private set; }
        public DateTime LastModified { get; private set; }

        

        /// <summary>
        /// Get the saved game's name from a .sav file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetArchiveName(Stream file)
        {
            var cf = new CompoundFile(file);
            var archiveNameBytes = cf.RootStorage.GetStream("Scenario Description").GetData();
            var archiveName = System.Text.Encoding.Unicode.GetString(archiveNameBytes);
            archiveName = archiveName.TrimEnd(new char[] { '\0' });
            return archiveName;
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
                    GUIName = GetArchiveName(file);
                }

                LastModified = File.GetLastWriteTime(ProgramConstants.GamePath + SAVED_GAME_PATH + FileName);

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
            string metaFilePath = Path.ChangeExtension(FilePath, GameSessionInfo.SavedGameMetaExtension);

            if (File.Exists(metaFilePath))
            {
                byte[] data = File.ReadAllBytes(metaFilePath);
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)~data[i];
                }

                string dataAsString = Encoding.UTF8.GetString(data);
                string[] parts = dataAsString.Split(',');
                if (parts.Length != 3)
                {
                    Logger.Log("Unexpected saved game meta file format in file " + metaFilePath);
                    int gameSessionTypeInt = Conversions.IntFromString(parts[0], -1);
                    if (gameSessionTypeInt > -1 && gameSessionTypeInt <= (int)GameSessionType.SESSION_TYPE_MAX)
                        SessionType = (GameSessionType)gameSessionTypeInt;
                }
            }
        }
    }
}
