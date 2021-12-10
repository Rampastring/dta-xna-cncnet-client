using ClientCore;
using ClientGUI;
using DTAClient.Domain;
using DTAClient.Domain.Singleplayer;
using Microsoft.Xna.Framework;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DTAClient.DXGUI.Generic
{
    /// <summary>
    /// A window for loading saved singleplayer games.
    /// </summary>
    public class GameLoadingWindow : XNAWindow
    {
        private const string SAVED_GAMES_DIRECTORY = "Saved Games";

        public GameLoadingWindow(WindowManager windowManager, DiscordHandler discordHandler) : base(windowManager)
        {
            this.discordHandler = discordHandler;
        }

        private DiscordHandler discordHandler;

        private XNAMultiColumnListBox lbSaveGameList;
        private XNAClientButton btnLaunch;
        private XNAClientButton btnDelete;
        private XNAClientButton btnCancel;

        private List<SavedGame> savedGames = new List<SavedGame>();

        public override void Initialize()
        {
            Name = "GameLoadingWindow";
            BackgroundTexture = AssetLoader.LoadTexture("loadmissionbg.png");

            ClientRectangle = new Rectangle(0, 0, 600, 380);
            CenterOnParent();

            lbSaveGameList = new XNAMultiColumnListBox(WindowManager);
            lbSaveGameList.Name = nameof(lbSaveGameList);
            lbSaveGameList.ClientRectangle = new Rectangle(13, 13, 574, 317);
            lbSaveGameList.AddColumn("SAVED GAME NAME", 400);
            lbSaveGameList.AddColumn("DATE / TIME", 174);
            lbSaveGameList.BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 1, 1);
            lbSaveGameList.PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            lbSaveGameList.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            lbSaveGameList.AllowKeyboardInput = true;

            btnLaunch = new XNAClientButton(WindowManager);
            btnLaunch.Name = nameof(btnLaunch);
            btnLaunch.ClientRectangle = new Rectangle(125, 345, 110, 23);
            btnLaunch.Text = "Load";
            btnLaunch.AllowClick = false;
            btnLaunch.LeftClick += BtnLaunch_LeftClick;

            btnDelete = new XNAClientButton(WindowManager);
            btnDelete.Name = nameof(btnDelete);
            btnDelete.ClientRectangle = new Rectangle(btnLaunch.Right + 10, btnLaunch.Y, 110, 23);
            btnDelete.Text = "Delete";
            btnDelete.AllowClick = false;
            btnDelete.LeftClick += BtnDelete_LeftClick;

            btnCancel = new XNAClientButton(WindowManager);
            btnCancel.Name = nameof(btnCancel);
            btnCancel.ClientRectangle = new Rectangle(btnDelete.Right + 10, btnLaunch.Y, 110, 23);
            btnCancel.Text = "Cancel";
            btnCancel.LeftClick += BtnCancel_LeftClick;

            AddChild(lbSaveGameList);
            AddChild(btnLaunch);
            AddChild(btnDelete);
            AddChild(btnCancel);

            base.Initialize();

            ListSaves();
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSaveGameList.SelectedIndex == -1)
            {
                btnLaunch.AllowClick = false;
                btnDelete.AllowClick = false;
            }
            else
            {
                btnLaunch.AllowClick = true;
                btnDelete.AllowClick = true;
            }
        }

        private void BtnCancel_LeftClick(object sender, EventArgs e)
        {
            Enabled = false;
        }

        private void BtnLaunch_LeftClick(object sender, EventArgs e)
        {
            SavedGame sg = savedGames[lbSaveGameList.SelectedIndex];
            Logger.Log("Loading saved game " + sg.FilePath);

            var gameSessionInfo = new GameSessionManager(sg.SessionInfo, WindowManager.AddCallback);
            gameSessionInfo.StartSession(); // Starting the session copies the saved games for this session to the main saved games directory

            bool writeNewSpawnIni = true;

            if (sg.SessionInfo.SessionType == GameSessionType.SINGLEPLAYER)
            {
                var mission = CampaignHandler.Instance.Missions.Find(m => m.InternalName == sg.SessionInfo.MissionInternalName);

                if (mission == null)
                {
                    Logger.Log("Mission entry for " + sg.SessionInfo.MissionInternalName + " not found when loading game, " +
                        "unable to write the actual singleplayer mission file to directory for restarts.");
                }
                else
                {
                    CampaignHandler.Instance.WriteFilesForMission(mission, (int)sg.SessionInfo.Difficulty - 1, sg.SessionInfo.GlobalFlagValues);
                    writeNewSpawnIni = false;
                }
            }

            int nextAutoSaveId = -1;
            const int AutoSaveStringLength = 8;
            if (sg.FileName.StartsWith("AUTOSAVE") && sg.FileName.Length > AutoSaveStringLength)
            {
                // Parse and assign next auto-save number
                int pointIndex = sg.FileName.IndexOf('.');
                if (pointIndex > -1)
                {
                    // The spawner increments the ID by 1 before actually saving
                    nextAutoSaveId = Conversions.IntFromString(sg.FileName.Substring(AutoSaveStringLength, 1), -1);
                }
            }

            if (writeNewSpawnIni)
            {
                Logger.Log($"Writing new {ProgramConstants.SPAWNER_SETTINGS} for loading game.");

                File.Delete(ProgramConstants.GamePath + ProgramConstants.SPAWNER_SETTINGS);

                using (StreamWriter sw = new StreamWriter(ProgramConstants.GamePath + ProgramConstants.SPAWNER_SETTINGS))
                {
                    sw.WriteLine("; generated by DTA Client");
                    sw.WriteLine("[Settings]");
                    sw.WriteLine("Scenario=spawnmap.ini");
                    sw.WriteLine("SaveGameName=" + sg.FileName);
                    sw.WriteLine("LoadSaveGame=Yes");
                    sw.WriteLine("SidebarHack=" + ClientConfiguration.Instance.SidebarHack);
                    sw.WriteLine("Firestorm=No");
                    sw.WriteLine("GameSpeed=" + UserINISettings.Instance.GameSpeed);
                    if (UserINISettings.Instance.EnableSPAutoSave)
                    {
                        sw.WriteLine("AutoSaveGame=" + ClientConfiguration.Instance.SinglePlayerAutoSaveInterval);
                        sw.WriteLine("NextSPAutoSaveId=" + nextAutoSaveId);
                    }
                }

                File.Delete(ProgramConstants.GamePath + "spawnmap.ini");
                using (var sw = new StreamWriter(ProgramConstants.GamePath + "spawnmap.ini"))
                {
                    sw.WriteLine("[Map]");
                    sw.WriteLine("Size=0,0,50,50");
                    sw.WriteLine("LocalSize=0,0,50,50");
                    sw.WriteLine();
                    sw.Close();
                }
            }
            else
            {
                Logger.Log($"Appending game loading settings to {ProgramConstants.SPAWNER_SETTINGS}.");

                var spawnIni = new IniFile(ProgramConstants.GamePath + ProgramConstants.SPAWNER_SETTINGS);
                spawnIni.SetBooleanValue("Settings", "LoadSaveGame", true);
                spawnIni.SetStringValue("Settings", "SaveGameName", sg.FileName);
                if (UserINISettings.Instance.EnableSPAutoSave)
                {
                    spawnIni.SetIntValue("Settings", "AutoSaveGame", ClientConfiguration.Instance.SinglePlayerAutoSaveInterval);
                    spawnIni.SetIntValue("Settings", "NextSPAutoSaveId", nextAutoSaveId);
                }

                spawnIni.WriteIniFile();
            }

            discordHandler?.UpdatePresence(sg.GUIName, true);

            Enabled = false;
            GameProcessLogic.GameProcessExited += GameProcessExited_Callback;

            GameProcessLogic.StartGameProcess(gameSessionInfo);
        }

        private void BtnDelete_LeftClick(object sender, EventArgs e)
        {
            SavedGame sg = savedGames[lbSaveGameList.SelectedIndex];
            var msgBox = new XNAMessageBox(WindowManager, "Delete Confirmation",
                    "The following saved game will be deleted permanently:" + Environment.NewLine +
                    Environment.NewLine +
                    "Filename: " + sg.FileName + Environment.NewLine +
                    "Saved game name: " + Renderer.GetSafeString(sg.GUIName, lbSaveGameList.FontIndex) + Environment.NewLine +
                    "Date and time: " + sg.LastModified.ToString() + Environment.NewLine +
                    Environment.NewLine +
                    "Are you sure you want to proceed?", XNAMessageBoxButtons.YesNo);
            msgBox.Show();
            msgBox.YesClickedAction = DeleteMsgBox_YesClicked;
        }

        private void DeleteMsgBox_YesClicked(XNAMessageBox obj)
        {
            SavedGame sg = savedGames[lbSaveGameList.SelectedIndex];

            Logger.Log("Deleting saved game " + sg.FileName);
            File.Delete(sg.FilePath);
            File.Delete(Path.ChangeExtension(sg.FilePath, GameSessionManager.SavedGameMetaExtension));
            if (Directory.GetFiles(Path.GetDirectoryName(sg.FilePath)).Length == 0)
                Directory.Delete(Path.GetDirectoryName(sg.FilePath));

            ListSaves();
        }
        
        private void GameProcessExited_Callback()
        {
            WindowManager.AddCallback(new Action(GameProcessExited), null);
        }

        protected virtual void GameProcessExited()
        {
            GameProcessLogic.GameProcessExited -= GameProcessExited_Callback;
            discordHandler?.UpdatePresence();

            if (GameProcessLogic.GameSessionManager.SessionType == GameSessionType.SINGLEPLAYER)
            {
                CampaignHandler.Instance.PostGameExitOnSingleplayerMission(GameProcessLogic.GameSessionManager.SessionInfo);
            }
        }

        public void ListSaves()
        {
            savedGames.Clear();
            lbSaveGameList.ClearItems();
            lbSaveGameList.SelectedIndex = -1;

            if (!Directory.Exists(ProgramConstants.GamePath + SAVED_GAMES_DIRECTORY))
            {
                Logger.Log("Saved Games directory not found!");
                return;
            }

            string[] files = Directory.GetFiles(ProgramConstants.GamePath + 
                SAVED_GAMES_DIRECTORY + Path.DirectorySeparatorChar,
                "*.SAV", SearchOption.TopDirectoryOnly);

            string[] directories = Directory.GetDirectories(ProgramConstants.GamePath + SAVED_GAMES_DIRECTORY);

            foreach (string dirPath in directories)
            {
                string _dirPath = dirPath.Replace('\\', '/');
                int lastDirectorySeparatorIndex = _dirPath.LastIndexOf('/');
                string dirName = _dirPath.Substring(lastDirectorySeparatorIndex + 1);

                long.TryParse(dirName, NumberStyles.None, CultureInfo.InvariantCulture, out long uniqueSessionId);

                string[] saveNames = Directory.GetFiles(dirPath, "*.SAV");
                foreach (string file in saveNames)
                {
                    ParseSaveGame(file, uniqueSessionId);
                }
            }

            savedGames = savedGames.OrderBy(sg => sg.LastModified.Ticks).ToList();
            savedGames.Reverse();

            foreach (SavedGame sg in savedGames)
            {
                string[] item = new string[] {
                    Renderer.GetSafeString(sg.GUIName, lbSaveGameList.FontIndex),
                    sg.LastModified.ToString() };
                lbSaveGameList.AddItem(item, true);
            }
        }

        private void ParseSaveGame(string filePath, long uniqueSessionId)
        {
            SavedGame sg = new SavedGame(filePath, uniqueSessionId);
            if (sg.ParseInfo())
                savedGames.Add(sg);
        }
    }
}
