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

        private XNALabel lblSessionTypeValue;
        private XNALabel lblMissionNameValue;
        private XNALabel lblDifficultyLevelValue;
        private XNALabel lblGlobalFlagsValue;

        private List<SavedGame> savedGames = new List<SavedGame>();

        public override void Initialize()
        {
            Name = "GameLoadingWindow";
            BackgroundTexture = AssetLoader.LoadTexture("loadmissionbg.png");

            ClientRectangle = new Rectangle(0, 0, 800, 380);
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
            btnLaunch.ClientRectangle = new Rectangle(0, 345, 110, 23);
            btnLaunch.Text = "Load";
            btnLaunch.AllowClick = false;
            btnLaunch.LeftClick += BtnLaunch_LeftClick;

            btnDelete = new XNAClientButton(WindowManager);
            btnDelete.Name = nameof(btnDelete);
            btnDelete.ClientRectangle = new Rectangle(0, btnLaunch.Y, 110, 23);
            btnDelete.Text = "Delete";
            btnDelete.AllowClick = false;
            btnDelete.LeftClick += BtnDelete_LeftClick;

            btnCancel = new XNAClientButton(WindowManager);
            btnCancel.Name = nameof(btnCancel);
            btnCancel.ClientRectangle = new Rectangle(0, btnLaunch.Y, 110, 23);
            btnCancel.Text = "Cancel";
            btnCancel.LeftClick += BtnCancel_LeftClick;

            var lblSaveInfoHeader = new XNALabel(WindowManager);
            lblSaveInfoHeader.Name = nameof(lblSaveInfoHeader);
            lblSaveInfoHeader.Y = lbSaveGameList.Y;
            lblSaveInfoHeader.X = lbSaveGameList.Right + UIDesignConstants.CONTROL_HORIZONTAL_MARGIN * 2;
            lblSaveInfoHeader.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblSaveInfoHeader.Text = "SAVE INFORMATION";
            AddChild(lblSaveInfoHeader);

            var lblSessionType = new XNALabel(WindowManager);
            lblSessionType.Name = nameof(lblSessionType);
            lblSessionType.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblSessionType.Y = lblSaveInfoHeader.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblSessionType.X = lblSaveInfoHeader.X;
            lblSessionType.Text = "Session Type:";
            AddChild(lblSessionType);

            lblSessionTypeValue = new XNALabel(WindowManager);
            lblSessionTypeValue.Name = nameof(lblSessionTypeValue);
            lblSessionTypeValue.Y = lblSessionType.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN;
            lblSessionTypeValue.X = lblSaveInfoHeader.X;
            lblSessionTypeValue.Text = " ";
            AddChild(lblSessionTypeValue);

            var lblMissionName = new XNALabel(WindowManager);
            lblMissionName.Name = nameof(lblMissionName);
            lblMissionName.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblMissionName.Y = lblSessionTypeValue.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblMissionName.X = lblSaveInfoHeader.X;
            lblMissionName.Text = "Mission Name:";
            AddChild(lblMissionName);

            lblMissionNameValue = new XNALabel(WindowManager);
            lblMissionNameValue.Name = nameof(lblMissionNameValue);
            lblMissionNameValue.Y = lblMissionName.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN;
            lblMissionNameValue.X = lblSessionTypeValue.X;
            lblMissionNameValue.Text = " ";
            AddChild(lblMissionNameValue);

            var lblDifficultyLevel = new XNALabel(WindowManager);
            lblDifficultyLevel.Name = nameof(lblDifficultyLevel);
            lblDifficultyLevel.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblDifficultyLevel.Y = lblMissionNameValue.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblDifficultyLevel.X = lblSaveInfoHeader.X;
            lblDifficultyLevel.Text = "Difficulty Level:";
            AddChild(lblDifficultyLevel);

            lblDifficultyLevelValue = new XNALabel(WindowManager);
            lblDifficultyLevelValue.Name = nameof(lblDifficultyLevelValue);
            lblDifficultyLevelValue.Y = lblDifficultyLevel.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN;
            lblDifficultyLevelValue.X = lblSaveInfoHeader.X;
            lblDifficultyLevelValue.Text = " ";
            AddChild(lblDifficultyLevelValue);

            var lblGlobalFlags = new XNALabel(WindowManager);
            lblGlobalFlags.Name = nameof(lblGlobalFlags);
            lblGlobalFlags.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblGlobalFlags.Y = lblDifficultyLevelValue.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblGlobalFlags.X = lblSaveInfoHeader.X;
            lblGlobalFlags.Text = "Preconditions:";
            AddChild(lblGlobalFlags);

            lblGlobalFlagsValue = new XNALabel(WindowManager);
            lblGlobalFlagsValue.Name = nameof(lblGlobalFlagsValue);
            lblGlobalFlagsValue.Y = lblGlobalFlags.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN;
            lblGlobalFlagsValue.X = lblSaveInfoHeader.X;
            lblGlobalFlagsValue.Text = " ";
            AddChild(lblGlobalFlagsValue);

            AddChild(lbSaveGameList);
            AddChild(btnLaunch);
            AddChild(btnDelete);
            AddChild(btnCancel);

            btnDelete.CenterOnParentHorizontally();
            btnLaunch.X = btnDelete.X - 10 - btnLaunch.Width;
            btnCancel.X = btnDelete.Right + 10;

            base.Initialize();

            ListSaves();
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSaveGameList.SelectedIndex == -1)
            {
                btnLaunch.AllowClick = false;
                btnDelete.AllowClick = false;
                ClearSaveInformation(string.Empty);
                return;
            }

            btnLaunch.AllowClick = true;
            btnDelete.AllowClick = true;

            SavedGame sg = savedGames[lbSaveGameList.SelectedIndex];

            if (sg.SessionInfo == null)
            {
                ClearSaveInformation(string.Empty);
                lblSessionTypeValue.Text = "Unknown";
                return;
            }

            switch (sg.SessionInfo.SessionType)
            {
                case GameSessionType.SKIRMISH:
                    ClearSaveInformation(string.Empty);
                    lblSessionTypeValue.Text = "Skirmish";
                    break;
                case GameSessionType.MULTIPLAYER:
                    ClearSaveInformation(string.Empty);
                    lblSessionTypeValue.Text = "Multiplayer";
                    break;
                case GameSessionType.SINGLEPLAYER:
                    var mission = CampaignHandler.Instance.Missions.Find(m => m.InternalName == sg.SessionInfo.MissionInternalName);
                    if (mission == null)
                    {
                        ClearSaveInformation("Unknown");
                    }
                    else
                    {
                        string difficultyName = ProgramConstants.DifficultyRankToName(sg.SessionInfo.Difficulty);
                        if (mission.DifficultyLabels != null)
                            difficultyName = mission.DifficultyLabels[(int)sg.SessionInfo.Difficulty - 1];

                        if (difficultyName.Length > 1)
                            difficultyName = difficultyName[0].ToString() + difficultyName.ToLower().Substring(1);

                        string globalFlagInfo;

                        if (sg.SessionInfo.GlobalFlagValues != null)
                        {
                            globalFlagInfo = string.Empty;

                            foreach (var kvp in sg.SessionInfo.GlobalFlagValues)
                            {
                                int globalFlagIndex = kvp.Key;
                                bool enabled = kvp.Value;

                                if (CampaignHandler.Instance.GlobalVariables.Count > globalFlagIndex)
                                {
                                    var globalVariable = CampaignHandler.Instance.GlobalVariables[globalFlagIndex];
                                    globalFlagInfo += globalVariable.UIName + ": " + (enabled ? "Yes" : "No");
                                }
                                else
                                {
                                    globalFlagInfo += "(Unknown variable) " + globalFlagIndex + ": " + (enabled ? "Yes" : "No");
                                }

                                globalFlagInfo += Environment.NewLine;
                            }
                        }
                        else
                        {
                            globalFlagInfo = "None";
                        }

                        lblMissionNameValue.Text = mission.GUIName;
                        lblDifficultyLevelValue.Text = difficultyName;
                        lblGlobalFlagsValue.Text = globalFlagInfo;
                    }

                    lblSessionTypeValue.Text = "Singleplayer";

                    break;
            }
        }

        private void ClearSaveInformation(string defaultText)
        {
            lblSessionTypeValue.Text = defaultText;
            lblMissionNameValue.Text = defaultText;
            lblDifficultyLevelValue.Text = defaultText;
            lblGlobalFlagsValue.Text = defaultText;
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
