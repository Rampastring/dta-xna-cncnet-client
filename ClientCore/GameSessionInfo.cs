using Rampastring.Tools;
using System;
using System.Globalization;
using System.IO;
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
    /// Contains information on a game session
    /// and manages a session's saved games.
    /// </summary>
    public class GameSessionInfo
    {
        public const string SavedGamesDirectory = "Saved Games";
        public const string SavedGameMetaExtension = ".sgmeta";
        public const int SavedGameMetaFieldCount = 3;

        public GameSessionInfo(GameSessionType sessionType, long uniqueId, Action<Delegate, object[]> callbackAction)
        {
            SessionType = sessionType;
            UniqueId = uniqueId;
            this.callbackAction = callbackAction;
        }

        public GameSessionType SessionType { get; }
        public long UniqueId { get; }

        /// <summary>
        /// A callback used to execute functions on the main UI thread.
        /// </summary>
        private readonly Action<Delegate, object[]> callbackAction;

        private FileSystemWatcher fileSystemWatcher;

        private readonly object locker = new object();

        private bool gameSaved = false;

        /// <summary>
        /// Clears the in-game saved games directory by moving saves
        /// to sub-directories.
        /// </summary>
        public static void ClearSavedGamesDirectory()
        {
            if (!Directory.Exists(ProgramConstants.GamePath + SavedGamesDirectory))
                return;

            string[] saveFiles = Directory.GetFiles(ProgramConstants.GamePath + SavedGamesDirectory, "*.SAV");

            string unknownSaveDirPath = ProgramConstants.GamePath + SavedGamesDirectory + "/Unknown";
            Directory.CreateDirectory(unknownSaveDirPath);
            foreach (string save in saveFiles)
            {
                // TODO parse the meta files to figure out whether the saves are truly unknown
                Logger.Log("Moving saved game " + Path.GetFileName(save) + " to UNKNOWN saves directory.");
                File.Move(save, unknownSaveDirPath + "/" + Path.GetFileName(save));
            }

            string[] mpSaveFiles = Directory.GetFiles(ProgramConstants.GamePath + SavedGamesDirectory, "*.NET");
            string mpSaveDirPath = ProgramConstants.GamePath + MultiplayerSaveGameManager.SAVED_GAMES_MP_DIRECTORY;
            Directory.CreateDirectory(mpSaveDirPath);
            foreach (string save in mpSaveFiles)
            {
                Logger.Log("Moving saved game " + Path.GetFileName(save) + " to MULTIPLAYER saves directory.");
                File.Move(save, mpSaveDirPath + "/" + Path.GetFileName(save));
            }

            string spawnSGIniPath = ProgramConstants.GamePath + SavedGamesDirectory + "/" + MultiplayerSaveGameManager.SPAWN_INI_NAME;
            if (File.Exists(spawnSGIniPath))
            {
                Logger.Log($"Moving { MultiplayerSaveGameManager.SPAWN_INI_NAME } to MULTIPLAYER saves directory.");
                File.Move(spawnSGIniPath, ProgramConstants.GamePath + MultiplayerSaveGameManager.SAVED_GAMES_MP_DIRECTORY + "/" + MultiplayerSaveGameManager.SPAWN_INI_NAME);
            }
        }

        public void StartSession()
        {
            Logger.Log("Starting game session.");

            gameSaved = false;

            // Move possible saved games of this session to the main saved games directory

            // Build a list of save files to move
            string[] saveFiles = null;
            if (SessionType == GameSessionType.MULTIPLAYER)
            {
                if (Directory.Exists(ProgramConstants.GamePath + MultiplayerSaveGameManager.SAVED_GAMES_MP_DIRECTORY))
                {
                    saveFiles = Directory.GetFiles(ProgramConstants.GamePath + MultiplayerSaveGameManager.SAVED_GAMES_MP_DIRECTORY, "*.NET");
                }
            }
            else if (SessionType == GameSessionType.UNKNOWN)
            {
                string saveDirPath = ProgramConstants.GamePath + SavedGamesDirectory + "/Unknown";
                if (Directory.Exists(saveDirPath))
                {
                    saveFiles = Directory.GetFiles(saveDirPath, "*.SAV");
                }
            }
            else
            {
                string saveDirPath = ProgramConstants.GamePath + SavedGamesDirectory + "/" + UniqueId.ToString(CultureInfo.InvariantCulture);
                if (Directory.Exists(saveDirPath))
                {
                    saveFiles = Directory.GetFiles(saveDirPath, "*.SAV");
                }
            }

            // Move the files and any potential meta files
            if (saveFiles != null)
            {
                Logger.Log("Moving " + saveFiles.Length + " save files from sub-directory to main saved games directory.");

                try
                {
                    foreach (string savePath in saveFiles)
                    {
                        File.Move(savePath, ProgramConstants.GamePath + SavedGamesDirectory + Path.GetFileName(savePath));
                        string metaFilePath = Path.ChangeExtension(savePath, SavedGameMetaExtension);
                        
                        if (File.Exists(metaFilePath))
                            File.Move(metaFilePath, ProgramConstants.GamePath + SavedGamesDirectory + Path.GetFileName(metaFilePath));
                    }
                }
                catch (IOException ex)
                {
                    Logger.Log("FAILED to move saved games of session to in-game saved game directory: " + ex.Message);
                }
            }
            else
            {
                Logger.Log("Previous saved games not detected for current session.");
            }

            // Only set up the file system watcher in multiplayer games
            if (SessionType == GameSessionType.MULTIPLAYER)
            {
                if (fileSystemWatcher != null)
                {
                    fileSystemWatcher.Dispose();
                }

                string filter = SessionType == GameSessionType.MULTIPLAYER ? "*.NET" : "*.SAV";

                fileSystemWatcher = new FileSystemWatcher(ProgramConstants.GamePath + "Saved Games", filter);
                fileSystemWatcher.Created += FileSystemWatcher_Event;
                fileSystemWatcher.Changed += FileSystemWatcher_Event;
                fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        private void FileSystemWatcher_Event(object sender, FileSystemEventArgs e)
        {
            lock (locker)
            {
                // If we are in a multiplayer session, 
                // add a callback to WindowManager to let SavedGameManager rename the save
                if (SessionType == GameSessionType.MULTIPLAYER)
                {
                    callbackAction(new Action<FileSystemEventArgs>(MultiplayerFSWEvent), new object[] { e });
                    return;
                }
            }
        }

        private void MultiplayerFSWEvent(FileSystemEventArgs e)
        {
            Logger.Log("FSW Event: " + e.FullPath);

            if (Path.GetFileName(e.FullPath) == "SAVEGAME.NET")
            {
                if (!gameSaved)
                {
                    bool success = MultiplayerSaveGameManager.InitSavedGames();

                    if (!success)
                        return;
                }

                gameSaved = true;

                MultiplayerSaveGameManager.RenameSavedGame();
            }
        }

        public void EndSession()
        {
            if (fileSystemWatcher != null)
            {
                fileSystemWatcher.EnableRaisingEvents = false;
                fileSystemWatcher.Created -= FileSystemWatcher_Event;
                fileSystemWatcher.Changed -= FileSystemWatcher_Event;
                fileSystemWatcher.Dispose();
                fileSystemWatcher = null;
            }

            if (!Directory.Exists(ProgramConstants.GamePath + SavedGamesDirectory))
            {
                Logger.Log(SavedGamesDirectory + " not found! Skipping game session end logic.");
                return;
            }

            // Move the saved games of this session into a sub-directory
            string subDirPath = SavedGamesDirectory + "/";
            if (SessionType == GameSessionType.MULTIPLAYER)
            {
                subDirPath += "Multiplayer/";
            }
            else if (SessionType == GameSessionType.UNKNOWN)
            {
                subDirPath += "Unknown/";
            }
            else
            {
                subDirPath += UniqueId.ToString(CultureInfo.InvariantCulture) + "/";
            }

            Directory.CreateDirectory(ProgramConstants.GamePath + subDirPath);

            string[] saveFiles;

            if (SessionType == GameSessionType.MULTIPLAYER)
            {
                saveFiles = Directory.GetFiles(ProgramConstants.GamePath + SavedGamesDirectory, "*.NET");
            }
            else
            {
                saveFiles = Directory.GetFiles(ProgramConstants.GamePath + SavedGamesDirectory, "*.SAV");
            }

            try
            {
                foreach (string file in saveFiles)
                {
                    File.Move(file, ProgramConstants.GamePath + subDirPath + Path.GetFileName(file));

                    string metaFilePath = Path.ChangeExtension(file, SavedGameMetaExtension);
                    if (File.Exists(metaFilePath))
                        File.Move(metaFilePath, ProgramConstants.GamePath + subDirPath + Path.GetFileName(metaFilePath));
                }
            }
            catch (IOException ex)
            {
                Logger.Log("FAILED to move saved game to sub-directory: " + ex.Message);
            }
            
            // Create meta files for the saved games
            foreach (string file in saveFiles)
            {
                string newPath = ProgramConstants.GamePath + subDirPath + Path.GetFileName(file);
                CreateSavedGameMetaFile(newPath);
            }
        }

        private void CreateSavedGameMetaFile(string saveFilePath)
        {
            string metaFilePath = Path.ChangeExtension(saveFilePath, SavedGameMetaExtension);

            if (File.Exists(metaFilePath))
                return;

            string meta = $"{ ((int)SessionType).ToString(CultureInfo.InvariantCulture) },{ UniqueId.ToString(CultureInfo.InvariantCulture) },{ Path.GetFileName(saveFilePath) }";
            byte[] bytes = Encoding.UTF8.GetBytes(meta);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)~bytes[i];
            }

            try
            {
                File.WriteAllBytes(metaFilePath, bytes);
            }
            catch (IOException ex)
            {
                Logger.Log("FAILED to write saved game meta file for " + Path.GetFileName(saveFilePath) + ": " + ex.Message);
            }
        }
    }
}
