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

    /// <summary>
    /// Contains information on a game session
    /// and manages a session's saved games.
    /// </summary>
    public class GameSessionManager
    {
        private const string AutoSavesDirectoryName = "Autosaves";
        public const string SavedGamesDirectory = "Saved Games";
        public const string SavedGameMetaExtension = ".sgmeta";
        public const string LastSessionFilePath = "Client/LastGameSession";
        public const int SavedGameMetaFieldCount = 3;

        public GameSessionManager(GameSessionInfo sessionInfo, Action<Delegate, object[]> callbackAction)
        {
            SessionInfo = sessionInfo;
            this.callbackAction = callbackAction;
        }

        public GameSessionInfo SessionInfo { get; }
        public GameSessionType SessionType => SessionInfo.SessionType;
        public long UniqueId => SessionInfo.UniqueId;

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
        public static void CheckForSavesInMainSaveDirectory()
        {
            Logger.Log("Looking for orphan saves in main saved games directory.");

            if (!Directory.Exists(ProgramConstants.GamePath + SavedGamesDirectory))
                Directory.CreateDirectory(ProgramConstants.GamePath + SavedGamesDirectory);

            string[] saveFiles = Directory.GetFiles(ProgramConstants.GamePath + SavedGamesDirectory, "*.SAV", SearchOption.TopDirectoryOnly);

            GameSessionInfo previousSessionInfo = GameSessionInfo.ParseFromFile(ProgramConstants.GamePath + LastSessionFilePath);

            string unknownSaveDirPath = ProgramConstants.GamePath + SavedGamesDirectory + "/Unknown/";
            Directory.CreateDirectory(unknownSaveDirPath);
            foreach (string save in saveFiles)
            {
                string metaFilePath = Path.ChangeExtension(save, SavedGameMetaExtension);

                string destination;
                Random random;

                // Try to read metadata for the saved game.
                GameSessionInfo meta = GameSessionInfo.ParseFromFile(metaFilePath);

                // If metadata wasn't found, assume the saved game is related to the previous game session.
                if (meta == null && previousSessionInfo != null)
                {
                    Logger.Log($"Parsing metadata for save {Path.GetFileName(save)} failed. Falling back to previous session {previousSessionInfo.UniqueId}");
                    meta = previousSessionInfo;
                }

                if (meta != null && meta.SessionType != GameSessionType.UNKNOWN)
                {
                    string uniqueIdString = meta.UniqueId.ToString(CultureInfo.InvariantCulture);
                    if (Path.GetFileName(save).StartsWith("AUTOSAVE"))
                    {
                        destination = ProgramConstants.GamePath + SavedGamesDirectory + "/" + AutoSavesDirectoryName + "/" + Path.GetFileName(save);
                        Logger.Log($"Moving saved game {Path.GetFileName(save)} to auto-saves directory.");
                    }
                    else
                    {
                        destination = ProgramConstants.GamePath + SavedGamesDirectory + "/" + uniqueIdString + "/" + Path.GetFileName(save);
                        Logger.Log($"Moving saved game {Path.GetFileName(save)} to {uniqueIdString} saves directory.");
                    }

                    // Check to make sure a similarly-named file does not yet exist.
                    // If it does, then add random numbers to the file name.
                    random = new Random();
                    while (File.Exists(destination))
                    {
                        destination = Path.GetDirectoryName(destination) + "/" + Path.GetFileNameWithoutExtension(destination) + random.Next(0, 10).ToString(CultureInfo.InvariantCulture) + ".SAV";
                    }

                    try
                    {
                        Logger.Log($"Performing move for {Path.GetFileName(destination)}.");
                        Directory.CreateDirectory(Path.GetDirectoryName(destination));
                        File.Move(save, destination);

                        Logger.Log($"Writing metadata for {Path.GetFileName(destination)}.");
                        string metaFileDestination = Path.ChangeExtension(destination, SavedGameMetaExtension);
                        File.Delete(metaFileDestination);
                        meta.WriteToFile(metaFileDestination, Path.GetFileName(save));
                        File.Delete(metaFilePath);
                    }
                    catch (IOException ex)
                    {
                        Logger.Log("FAILED to move save file and/or its metadata! Returned exception message: " + ex.Message);
                    }

                    // This save is done with, move on to the next.
                    continue;
                }

                // No metadata exists for this save, and no fallback could be established from previous session info.
                // Info on the session for this saved game is irretrievably lost.
                // Move the saved game to the UNKNOWN saved games directory.
                Logger.Log("Moving saved game " + Path.GetFileName(save) + " to UNKNOWN saves directory.");
                destination = unknownSaveDirPath + Path.GetFileName(save);

                // Check to make sure a similarly-named file does not yet exist.
                // If it does, then add random numbers to the file name.
                random = new Random();
                while (File.Exists(destination))
                {
                    destination = Path.GetDirectoryName(destination) + "/" + Path.GetFileNameWithoutExtension(destination) + random.Next(0, 10).ToString(CultureInfo.InvariantCulture) + ".SAV";
                }

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destination));
                    File.Move(save, destination);
                }
                catch (IOException ex)
                {
                    Logger.Log("Failed to move UNKNOWN save file! Returned exception message: " + ex.Message);
                }
            }

            string[] mpSaveFiles = Directory.GetFiles(ProgramConstants.GamePath + SavedGamesDirectory, "*.NET", SearchOption.TopDirectoryOnly);
            string mpSaveDirPath = ProgramConstants.GamePath + MultiplayerSaveGameManager.SAVED_GAMES_MP_DIRECTORY;
            Directory.CreateDirectory(mpSaveDirPath);
            foreach (string save in mpSaveFiles)
            {
                Logger.Log("Moving saved game " + Path.GetFileName(save) + " to MULTIPLAYER saves directory.");
                string destinationPath = mpSaveDirPath + "/" + Path.GetFileName(save);
                string metaFilePath = Path.ChangeExtension(save, SavedGameMetaExtension);

                try
                {
                    if (!File.Exists(destinationPath))
                    {
                        File.Move(save, destinationPath);
                        string destinationMetaFilePath = Path.ChangeExtension(destinationPath, SavedGameMetaExtension);

                        if (File.Exists(metaFilePath))
                        {
                            Logger.Log($"The meta file for the save ({Path.GetFileName(metaFilePath)}) also exists. Moving it as well.");
                            File.Move(metaFilePath, destinationMetaFilePath);
                        }
                        else
                        {
                            Logger.Log("No metadata exists for the saved multiplayer game. Attempting to write previous session metadata next to the save file.");

                            if (previousSessionInfo.SessionType == GameSessionType.MULTIPLAYER)
                            {
                                Logger.Log("Previous session was a multiplayer session. Writing previous session metadata info for save file.");
                                previousSessionInfo.WriteToFile(destinationMetaFilePath, Path.GetFileName(destinationPath));
                            }
                            else
                            {
                                Logger.Log("The previous session was not a multiplayer session - cannot use previous session metadata for writing the save file's metadata.");
                            }
                        }
                    }
                    else
                    {
                        Logger.Log("A saved game with the same filename already exists in the MULTIPLAYER saves directory. Deleting the save and its meta file instead.");
                        File.Delete(save);
                        File.Delete(metaFilePath);
                    }
                }
                catch (IOException ex)
                {
                    Logger.Log("FAILED to move MULTIPLAYER save file and/or its metadata! Returned exception message: " + ex.Message);
                }
            }

            string spawnSGIniPath = ProgramConstants.GamePath + SavedGamesDirectory + "/" + MultiplayerSaveGameManager.SPAWN_INI_NAME;
            if (File.Exists(spawnSGIniPath))
            {
                Logger.Log($"Moving { MultiplayerSaveGameManager.SPAWN_INI_NAME } to MULTIPLAYER saves directory.");
                string destinationPath = ProgramConstants.GamePath + MultiplayerSaveGameManager.SAVED_GAMES_MP_DIRECTORY + "/" + MultiplayerSaveGameManager.SPAWN_INI_NAME;

                if (File.Exists(destinationPath))
                {
                    File.Move(spawnSGIniPath, destinationPath);
                }
                else
                {
                    Logger.Log($"{ MultiplayerSaveGameManager.SPAWN_INI_NAME } already exists in MULTIPLAYER saves directory (how??). Deleting the oprhan file in the main saved games directory instead.");
                    File.Delete(spawnSGIniPath);
                }
            }

            // If there are lone metadata files in the folder after the above processes, delete them.
            string[] metaFiles = Directory.GetFiles(ProgramConstants.GamePath + SavedGamesDirectory, "*" + SavedGameMetaExtension, SearchOption.TopDirectoryOnly);

            foreach (string path in metaFiles)
            {
                try
                {
                    File.Delete(path);
                }
                catch (IOException ex)
                {
                    Logger.Log("FAILED to delete save file metadata! Returned exception message: " + ex.Message);
                }
            }
        }

        public void StartSession(bool checkForSaves)
        {
            Logger.Log(nameof(StartSession) + ": Starting game session. Session type: " + SessionType);

            if (!Directory.Exists(ProgramConstants.GamePath + SavedGamesDirectory))
                Directory.CreateDirectory(ProgramConstants.GamePath + SavedGamesDirectory);

            try
            {
                Logger.Log(nameof(StartSession) + ": Writing last session file.");

                string lastSessionFilePath = ProgramConstants.GamePath + LastSessionFilePath;
                if (!Directory.Exists(Path.GetDirectoryName(lastSessionFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(lastSessionFilePath));

                // Delete previous session
                File.Delete(lastSessionFilePath);
                SessionInfo.WriteToFile(lastSessionFilePath, "none");
            }
            catch (IOException ex)
            {
                Logger.Log(nameof(StartSession) + ": FAILED to write last session file. Returned exception message: " + ex.Message);
            }

            gameSaved = false;

            if (checkForSaves)
            {
                // Move possible saved games of this session to the main saved games directory

                // Build a list of save files to move
                List<string> saveFiles = null;
                if (SessionType == GameSessionType.MULTIPLAYER)
                {
                    if (Directory.Exists(ProgramConstants.GamePath + MultiplayerSaveGameManager.SAVED_GAMES_MP_DIRECTORY))
                    {
                        saveFiles = Directory.GetFiles(ProgramConstants.GamePath + MultiplayerSaveGameManager.SAVED_GAMES_MP_DIRECTORY, "*.NET").ToList();
                    }
                }
                else if (SessionType == GameSessionType.UNKNOWN)
                {
                    string saveDirPath = ProgramConstants.GamePath + SavedGamesDirectory + "/Unknown";
                    if (Directory.Exists(saveDirPath))
                    {
                        saveFiles = Directory.GetFiles(saveDirPath, "*.SAV").ToList();
                    }
                }
                else
                {
                    string saveDirPath = ProgramConstants.GamePath + SavedGamesDirectory + "/" + UniqueId.ToString(CultureInfo.InvariantCulture);
                    if (Directory.Exists(saveDirPath))
                    {
                        saveFiles = Directory.GetFiles(saveDirPath, "*.SAV").ToList();
                    }
                }

                if (SessionType != GameSessionType.MULTIPLAYER)
                {
                    // Check for auto-saves

                    string autoSaveDirectoryPath = ProgramConstants.GamePath + SavedGamesDirectory + "/" + AutoSavesDirectoryName;
                    if (Directory.Exists(autoSaveDirectoryPath))
                    {
                        string[] autoSaveFiles = Directory.GetFiles(autoSaveDirectoryPath, "*.SAV");

                        if (saveFiles == null)
                            saveFiles = new List<string>();

                        saveFiles.AddRange(autoSaveFiles);
                    }
                }

                // Move the files and any potential meta files
                if (saveFiles != null && saveFiles.Count > 0)
                {
                    Logger.Log("Moving up to " + saveFiles.Count + " save files from sub-directories to main saved games directory.");

                    try
                    {
                        foreach (string savePath in saveFiles)
                        {
                            string metaFilePath = Path.ChangeExtension(savePath, SavedGameMetaExtension);
                            if (File.Exists(metaFilePath))
                            {
                                var meta = GameSessionInfo.ParseFromFile(metaFilePath);

                                if (meta == null || !meta.SessionMatches(SessionInfo))
                                    continue;

                                Logger.Log("Moving metadata file " + metaFilePath.Substring(ProgramConstants.GamePath.Length));

                                string metaFilePathDest = ProgramConstants.GamePath + SavedGamesDirectory + "/" + Path.GetFileName(metaFilePath);
                                File.Delete(metaFilePathDest);
                                File.Move(metaFilePath, metaFilePathDest);
                            }

                            Logger.Log("Moving save " + savePath.Substring(ProgramConstants.GamePath.Length));

                            string savePathDest = ProgramConstants.GamePath + SavedGamesDirectory + "/" + Path.GetFileName(savePath);
                            File.Delete(savePathDest);
                            File.Move(savePath, savePathDest);
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
            }
            
            // Only set up the file system watcher in multiplayer games
            if (SessionType == GameSessionType.MULTIPLAYER)
            {
                Logger.Log("Setting up filesystem watcher for multiplayer saved games.");

                if (fileSystemWatcher != null)
                {
                    fileSystemWatcher.Dispose();
                }

                string filter = SessionType == GameSessionType.MULTIPLAYER ? "*.NET" : "*.SAV";

                fileSystemWatcher = new FileSystemWatcher(ProgramConstants.GamePath + SavedGamesDirectory, filter);
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
            callbackAction(new Action(DoEndSession), null);
        }

        private void DoEndSession()
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
            string autosaveSubDirPath = SavedGamesDirectory + "/" + AutoSavesDirectoryName + "/";

            // Build sub-directory path depending on session type
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
                    string targetSubDir = subDirPath;

                    // Move auto-saves to common auto-save directory
                    if (Path.GetFileName(file).StartsWith("AUTOSAVE"))
                    {
                        targetSubDir = autosaveSubDirPath;
                        Directory.CreateDirectory(ProgramConstants.GamePath + autosaveSubDirPath);
                        File.Delete(ProgramConstants.GamePath + targetSubDir + Path.GetFileName(file));
                    }
                    else
                    {
                        Directory.CreateDirectory(ProgramConstants.GamePath + subDirPath);
                    }

                    File.Move(file, ProgramConstants.GamePath + targetSubDir + Path.GetFileName(file));

                    string metaFilePath = Path.ChangeExtension(file, SavedGameMetaExtension);
                    File.Delete(ProgramConstants.GamePath + targetSubDir + Path.GetFileName(metaFilePath));
                    if (File.Exists(metaFilePath))
                    {
                        File.Move(metaFilePath, ProgramConstants.GamePath + targetSubDir + Path.GetFileName(metaFilePath));
                    }
                }
            }
            catch (IOException ex)
            {
                Logger.Log("FAILED to move saved game to sub-directory: " + ex.Message);
            }

            // Create meta files for the saved games
            foreach (string file in saveFiles)
            {
                string targetSubDir = subDirPath;
                if (Path.GetFileName(file).StartsWith("AUTOSAVE"))
                {
                    targetSubDir = autosaveSubDirPath;
                    Directory.CreateDirectory(ProgramConstants.GamePath + autosaveSubDirPath);
                }

                string newPath = ProgramConstants.GamePath + targetSubDir + Path.GetFileName(file);
                Logger.Log("Creating post-session saved data metadata at " + newPath.Substring(ProgramConstants.GamePath.Length));
                CreateSavedGameMetaFile(newPath);
            }

            CheckForSavesInMainSaveDirectory();
        }

        private void CreateSavedGameMetaFile(string saveFilePath)
        {
            string metaFilePath = Path.ChangeExtension(saveFilePath, SavedGameMetaExtension);
            SessionInfo.WriteToFile(metaFilePath, Path.GetFileName(saveFilePath));
        }
    }
}
