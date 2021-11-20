using ClientCore;
using ClientGUI;
using DTAClient.Domain;
using DTAClient.Domain.Singleplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Globalization;
using System.IO;
using Updater;

namespace DTAClient.DXGUI.Generic
{
    /// <summary>
    /// A battle (as in Battle.ini entry) list box that can
    /// draw difficulty rank icons on its items.
    /// </summary>
    public class BattleListBox : XNAListBox
    {
        public BattleListBox(WindowManager windowManager) : base(windowManager)
        {
        }

        private Texture2D[] rankTextures;

        public override void Initialize()
        {
            EnableScrollbar = true;

            base.Initialize();

            rankTextures = new Texture2D[]
            {
                AssetLoader.LoadTexture("rankEasy.png"),
                AssetLoader.LoadTexture("rankNormal.png"),
                AssetLoader.LoadTexture("rankHard.png")
            };

            EnableScrollbar = true;
        }

        private Texture2D DifficultyRankToTexture(DifficultyRank rank)
        {
            if (rank == DifficultyRank.NONE)
                return null;

            return rankTextures[(int)rank - 1];
        }

        protected override void DrawListBoxItem(int index, int y)
        {
            base.DrawListBoxItem(index, y);

            var lbItem = Items[index];

            var mission = (Mission)lbItem.Tag;
            Texture2D rankTexture = DifficultyRankToTexture(mission.Rank);
            if (rankTexture != null)
            {
                DrawTexture(rankTexture,
                    new Rectangle(Width - rankTexture.Width - TextBorderDistance - ScrollBar.Width, y + (LineHeight - rankTexture.Height) / 2,
                    rankTexture.Width, rankTexture.Height), Color.White);
            }
        }
    }

    /// <summary>
    /// A text block that can draw an additional custom,
    /// darkened background texture in addition to its main background texture.
    /// </summary>
    public class MissionDescriptionBox : XNATextBlock
    {
        public MissionDescriptionBox(WindowManager windowManager) : base(windowManager)
        {
        }

        private Texture2D missionBackgroundTexture;
        private Texture2D darkeningTexture;

        public override void Initialize()
        {
            darkeningTexture = AssetLoader.CreateTexture(Color.Black, 2, 2);
            base.Initialize();
        }

        public void LoadMissionBackgroundTexture(string texturePath)
        {
            if (missionBackgroundTexture != null)
            {
                missionBackgroundTexture.Dispose();
                missionBackgroundTexture = null;
            }

            if (!string.IsNullOrWhiteSpace(texturePath))
            {
                missionBackgroundTexture = AssetLoader.LoadTextureUncached(texturePath);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawPanel();

            if (missionBackgroundTexture != null)
            {
                PanelBackgroundImageDrawMode originalDrawMode = PanelBackgroundDrawMode;

                PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.CENTERED;
                DrawBackgroundTexture(missionBackgroundTexture, new Color(40, 40, 40, 255));
                PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
                // DrawBackgroundTexture(darkeningTexture, new Color(0, 0, 0, 200));
                PanelBackgroundDrawMode = originalDrawMode;
            }

            if (!string.IsNullOrEmpty(Text))
            {
                var windowRectangle = RenderRectangle();

                DrawStringWithShadow(Text, FontIndex,
                    new Vector2(TextXMargin, TextYPosition), TextColor);
            }

            if (DrawBorders)
                DrawPanelBorders();

            DrawChildren(gameTime);
        }
    }

    public class CampaignSelector : XNAWindow
    {
        private const int MAX_GLOBAL_COUNT = 4;

        private const int DEFAULT_WIDTH = 650;
        private const int DEFAULT_HEIGHT = 600;

        private static readonly string[] DifficultyNames = new string[] { "Easy", "Medium", "Hard" };
        private static readonly string[] DifficultyNamesUIDefault = new string[] { "EASY", "NORMAL", "HARD" };

        private static readonly string[] DifficultyIniPaths = new string[]
        {
            "INI/Map Code/Difficulty Easy.ini",
            "INI/Map Code/Difficulty Medium.ini",
            "INI/Map Code/Difficulty Hard.ini"
        };

        public CampaignSelector(WindowManager windowManager, DiscordHandler discordHandler) : base(windowManager)
        {
            this.discordHandler = discordHandler;
        }

        private DiscordHandler discordHandler;

        private XNAListBox lbCampaignList;
        private XNAClientButton btnLaunch;
        private MissionDescriptionBox tbMissionDescription;
        private XNATrackbar trbDifficultySelector;

        private XNALabel lblEasy;
        private XNALabel lblNormal;
        private XNALabel lblHard;

        private XNALabel lblPreconditionsHeader;
        private XNALabel[] globalVariableNames = new XNALabel[MAX_GLOBAL_COUNT];
        private XNADropDown[] globalVariableValues = new XNADropDown[MAX_GLOBAL_COUNT];

        private CheaterWindow cheaterWindow;

        private MissionCompletionNotification missionCompletionNotification;

        private readonly string[] filesToCheck = new string[]
        {
            "INI/AI.ini",
            "INI/AIE.ini",
            "INI/Art.ini",
            "INI/ArtE.ini",
            "INI/Enhance.ini",
            "INI/Rules.ini",
            "INI/Map Code/Difficulty Hard.ini",
            "INI/Map Code/Difficulty Medium.ini",
            "INI/Map Code/Difficulty Easy.ini"
        };

        private Mission missionToLaunch;

        public override void Initialize()
        {
            BackgroundTexture = AssetLoader.LoadTexture("missionselectorbg.png");
            ClientRectangle = new Rectangle(0, 0, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            BorderColor = UISettings.ActiveSettings.PanelBorderColor;

            Name = "CampaignSelector";

            var lblSelectCampaign = new XNALabel(WindowManager);
            lblSelectCampaign.Name = "lblSelectCampaign";
            lblSelectCampaign.FontIndex = 1;
            lblSelectCampaign.ClientRectangle = new Rectangle(12, 12, 0, 0);
            lblSelectCampaign.Text = "MISSIONS:";

            lbCampaignList = new BattleListBox(WindowManager);
            lbCampaignList.Name = "lbCampaignList";
            lbCampaignList.BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 2, 2);
            lbCampaignList.PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            lbCampaignList.ClientRectangle = new Rectangle(12, 
                lblSelectCampaign.Bottom + 6, 300, 516);
            lbCampaignList.SelectedIndexChanged += LbCampaignList_SelectedIndexChanged;

            var lblMissionDescriptionHeader = new XNALabel(WindowManager);
            lblMissionDescriptionHeader.Name = "lblMissionDescriptionHeader";
            lblMissionDescriptionHeader.FontIndex = 1;
            lblMissionDescriptionHeader.ClientRectangle = new Rectangle(
                lbCampaignList.Right + 12, 
                lblSelectCampaign.Y, 0, 0);
            lblMissionDescriptionHeader.Text = "MISSION DESCRIPTION:";

            tbMissionDescription = new MissionDescriptionBox(WindowManager);
            tbMissionDescription.Name = "tbMissionDescription";
            tbMissionDescription.ClientRectangle = new Rectangle(
                lblMissionDescriptionHeader.X, 
                lblMissionDescriptionHeader.Bottom + 6,
                Width - 24 - lbCampaignList.Right, 430);
            tbMissionDescription.PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            tbMissionDescription.Alpha = 1.0f;

            tbMissionDescription.BackgroundTexture = AssetLoader.CreateTexture(AssetLoader.GetColorFromString(ClientConfiguration.Instance.AltUIBackgroundColor),
                tbMissionDescription.Width, tbMissionDescription.Height);

            var lblDifficultyLevel = new XNALabel(WindowManager);
            lblDifficultyLevel.Name = "lblDifficultyLevel";
            lblDifficultyLevel.Text = "DIFFICULTY LEVEL";
            lblDifficultyLevel.FontIndex = 1;
            Vector2 textSize = Renderer.GetTextDimensions(lblDifficultyLevel.Text, lblDifficultyLevel.FontIndex);
            lblDifficultyLevel.ClientRectangle = new Rectangle(
                tbMissionDescription.X + (tbMissionDescription.Width - (int)textSize.X) / 2,
                tbMissionDescription.Bottom + 12, (int)textSize.X, (int)textSize.Y);

            trbDifficultySelector = new XNATrackbar(WindowManager);
            trbDifficultySelector.Name = "trbDifficultySelector";
            trbDifficultySelector.ClientRectangle = new Rectangle(
                tbMissionDescription.X, lblDifficultyLevel.Bottom + 6,
                tbMissionDescription.Width, 30);
            trbDifficultySelector.MinValue = 0;
            trbDifficultySelector.MaxValue = 2;
            trbDifficultySelector.BackgroundTexture = AssetLoader.CreateTexture(
                new Color(0, 0, 0, 128), 2, 2);
            trbDifficultySelector.ButtonTexture = AssetLoader.LoadTextureUncached(
                "trackbarButton_difficulty.png");

            int y = lblDifficultyLevel.Y - UIDesignConstants.EMPTY_SPACE_BOTTOM;

            // Create controls for global variable customization
            // The indexes increase from bottom to top, meaning
            // that with 4 lines the global indexes go like this:
            // global slot #3
            // global slot #2
            // global slot #1
            // global slot #0
            for (int i = 0; i < MAX_GLOBAL_COUNT; i++)
            {
                globalVariableValues[i] = new XNADropDown(WindowManager);
                globalVariableValues[i].Name = "globalVariableValue" + i;
                globalVariableValues[i].X = trbDifficultySelector.X;
                globalVariableValues[i].Width = trbDifficultySelector.Width;
                globalVariableValues[i].AddItem("Disabled");
                globalVariableValues[i].AddItem("Enabled");
                AddChild(globalVariableValues[i]);
                globalVariableValues[i].Y = y - (UIDesignConstants.EMPTY_SPACE_BOTTOM * 2) - globalVariableValues[i].Height;
                globalVariableValues[i].Disable();

                globalVariableNames[i] = new XNALabel(WindowManager);
                globalVariableNames[i].Name = "globalVariableName" + i;
                globalVariableNames[i].Text = "Global Variable #" + i;
                globalVariableNames[i].TextAnchor = LabelTextAnchorInfo.RIGHT;
                globalVariableNames[i].AnchorPoint = new Vector2(globalVariableValues[i].X, globalVariableValues[i].Y - globalVariableNames[i].Height);
                AddChild(globalVariableNames[i]);
                globalVariableNames[i].Disable();

                y = globalVariableNames[i].Y;
            }

            lblPreconditionsHeader = new XNALabel(WindowManager);
            lblPreconditionsHeader.Name = nameof(lblPreconditionsHeader);
            lblPreconditionsHeader.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblPreconditionsHeader.TextAnchor = LabelTextAnchorInfo.HORIZONTAL_CENTER;
            lblPreconditionsHeader.AnchorPoint = new Vector2(trbDifficultySelector.ClientRectangle.Center.X,
                globalVariableNames[0].Y - UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2);
            lblPreconditionsHeader.Text = "PRECONDITIONS";
            AddChild(lblPreconditionsHeader);
            lblPreconditionsHeader.Disable();

            PreconditionUIConfig(null);

            lblEasy = new XNALabel(WindowManager);
            lblEasy.Name = "lblEasy";
            lblEasy.TextAnchor = LabelTextAnchorInfo.RIGHT;
            lblEasy.AnchorPoint = new Vector2(trbDifficultySelector.X,
                trbDifficultySelector.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN);
            lblEasy.FontIndex = 1;
            lblEasy.Text = DifficultyNamesUIDefault[0];

            lblNormal = new XNALabel(WindowManager);
            lblNormal.Name = "lblNormal";
            lblNormal.TextAnchor = LabelTextAnchorInfo.HORIZONTAL_CENTER;
            lblNormal.AnchorPoint = new Vector2(trbDifficultySelector.ClientRectangle.Center.X, lblEasy.AnchorPoint.Y);
            lblNormal.FontIndex = 1;
            lblNormal.Text = DifficultyNamesUIDefault[1];

            lblHard = new XNALabel(WindowManager);
            lblHard.Name = "lblHard";
            lblHard.TextAnchor = LabelTextAnchorInfo.LEFT;
            lblHard.AnchorPoint = new Vector2(trbDifficultySelector.Right, lblEasy.AnchorPoint.Y);
            lblHard.FontIndex = 1;
            lblHard.Text = DifficultyNamesUIDefault[2];

            btnLaunch = new XNAClientButton(WindowManager);
            btnLaunch.Name = "btnLaunch";
            btnLaunch.ClientRectangle = new Rectangle(12, Height - 35, 133, 23);
            btnLaunch.Text = "Launch";
            btnLaunch.AllowClick = false;
            btnLaunch.LeftClick += BtnLaunch_LeftClick;

            var btnCancel = new XNAClientButton(WindowManager);
            btnCancel.Name = "btnCancel";
            btnCancel.ClientRectangle = new Rectangle(Width - 145,
                btnLaunch.Y, 133, 23);
            btnCancel.Text = "Cancel";
            btnCancel.LeftClick += BtnCancel_LeftClick;

            AddChild(lblSelectCampaign);
            AddChild(lblMissionDescriptionHeader);
            AddChild(lbCampaignList);
            AddChild(tbMissionDescription);
            AddChild(lblDifficultyLevel);
            AddChild(btnLaunch);
            AddChild(btnCancel);
            AddChild(trbDifficultySelector);
            AddChild(lblEasy);
            AddChild(lblNormal);
            AddChild(lblHard);

            // Set control attributes from INI file
            base.Initialize();

            // Center on screen
            CenterOnParent();

            trbDifficultySelector.Value = UserINISettings.Instance.Difficulty;

            cheaterWindow = new CheaterWindow(WindowManager);
            DarkeningPanel dp = new DarkeningPanel(WindowManager);
            dp.AddChild(cheaterWindow);
            AddChild(dp);
            dp.CenterOnParent();
            cheaterWindow.CenterOnParent();
            cheaterWindow.YesClicked += CheaterWindow_YesClicked;
            cheaterWindow.Disable();

            EnabledChanged += CampaignSelector_EnabledChanged;

            missionCompletionNotification = new MissionCompletionNotification(WindowManager);
            WindowManager.AddAndInitializeControl(missionCompletionNotification);
            missionCompletionNotification.DrawOrder = 9999;
            missionCompletionNotification.UpdateOrder = 9999;
            missionCompletionNotification.Disable();

            CampaignHandler.Instance.MissionRankUpdated += CampaignHandler_MissionRankUpdated;
        }

        private void CampaignHandler_MissionRankUpdated(object sender, MissionCompletionEventArgs e)
        {
            missionCompletionNotification.Show(e.Mission);

            if (Enabled)
                ListBattles();
        }

        private void CampaignSelector_EnabledChanged(object sender, EventArgs e)
        {
            if (Enabled)
                ListBattles();
        }

        private void LbCampaignList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mission = lbCampaignList.SelectedItem != null ? lbCampaignList.SelectedItem.Tag as Mission : null;

            string[] difficultyLabels = DifficultyNamesUIDefault;
            if (mission != null && mission.DifficultyLabels != null && (!mission.RequiresUnlocking || mission.IsUnlocked))
                difficultyLabels = mission.DifficultyLabels;

            lblEasy.Text = difficultyLabels[0].ToUpperInvariant();
            lblNormal.Text = difficultyLabels[1].ToUpperInvariant();
            lblHard.Text = difficultyLabels[2].ToUpperInvariant();

            if (mission == null)
            {
                MissionList_InvalidMission();
                return;
            }

            if (string.IsNullOrEmpty(mission.Scenario))
            {
                MissionList_InvalidMission();
                return;
            }

            if (mission.RequiresUnlocking && !mission.IsUnlocked)
            {
                MissionList_InvalidMission();
                tbMissionDescription.Text = "You have not yet unlocked this mission.";
                return;
            }

            tbMissionDescription.LoadMissionBackgroundTexture(mission.PreviewImagePath);

            PreconditionUIConfig(mission);

            tbMissionDescription.Text = mission.GUIDescription;

            if (!mission.Enabled)
            {
                btnLaunch.AllowClick = false;
                return;
            }

            btnLaunch.AllowClick = true;
        }

        /// <summary>
        /// Sets up the UI for a non-launchable mission state.
        /// </summary>
        private void MissionList_InvalidMission()
        {
            tbMissionDescription.LoadMissionBackgroundTexture(null);
            tbMissionDescription.Text = string.Empty;
            btnLaunch.AllowClick = false;
            PreconditionUIConfig(null);
        }

        /// <summary>
        /// Configures the preconditions / globals UI for a mission.
        /// </summary>
        /// <param name="mission">The mission. Can be null.</param>
        private void PreconditionUIConfig(Mission mission)
        {
            if (mission != null && mission.UsedGlobalVariables.Length > 0)
            {
                lblPreconditionsHeader.Enable();

                for (int i = 0; i < mission.UsedGlobalVariables.Length && i < MAX_GLOBAL_COUNT; i++)
                {
                    CampaignGlobalVariable global = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == mission.UsedGlobalVariables[i]);

                    globalVariableNames[i].Text = global.UIName;
                    globalVariableNames[i].TextColor = UISettings.ActiveSettings.TextColor;
                    globalVariableNames[i].Enable();

                    if (global.IsDisabledUnlocked)
                    {
                        globalVariableValues[i].Items[0].Text = global.UIDisabledOption ?? "No";
                    }
                    else
                    {
                        globalVariableValues[i].Items[0].Text = "Option not unlocked";
                    }

                    if (global.IsEnabledUnlocked)
                    {
                        globalVariableValues[i].Items[1].Text = global.UIEnabledOption ?? "Yes";
                    }
                    else
                    {
                        globalVariableValues[i].Items[1].Text = "Option not unlocked";
                    }

                    globalVariableValues[i].Items[0].Selectable = global.IsDisabledUnlocked;
                    globalVariableValues[i].Items[1].Selectable = global.IsEnabledUnlocked;
                    globalVariableValues[i].SelectedIndex = 0;
                    if (global.IsEnabledUnlocked && !global.IsDisabledUnlocked)
                        globalVariableValues[i].SelectedIndex = 1;

                    if (global.HideIfNotEnabledUnlocked)
                    {
                        globalVariableValues[i].Items[0].Text = "-";
                        globalVariableValues[i].Items[1].Text = "-";
                        globalVariableNames[i].Text = "Unknown Condition (not yet unlocked)";
                        globalVariableNames[i].TextColor = UISettings.ActiveSettings.DisabledItemColor;
                    }

                    globalVariableValues[i].Enable();
                }

                int preconditionsHeaderY = mission.UsedGlobalVariables.Length >= MAX_GLOBAL_COUNT ? globalVariableNames[0].Y :
                    globalVariableNames[mission.UsedGlobalVariables.Length - 1].Y;
                preconditionsHeaderY -= UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
                lblPreconditionsHeader.Y = preconditionsHeaderY;
                lblPreconditionsHeader.Enable();

                tbMissionDescription.Height = lblPreconditionsHeader.Y - (UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2) - tbMissionDescription.Y;
            }
            else
            {
                lblPreconditionsHeader.Disable();
                for (int i = 0; i < MAX_GLOBAL_COUNT; i++)
                {
                    globalVariableNames[i].Disable();
                    globalVariableValues[i].Disable();
                }

                tbMissionDescription.Height = globalVariableValues[0].Bottom - tbMissionDescription.Y;
            }
        }

        private void BtnCancel_LeftClick(object sender, EventArgs e)
        {
            Enabled = false;
        }

        private void BtnLaunch_LeftClick(object sender, EventArgs e)
        {
            var mission = lbCampaignList.SelectedItem.Tag as Mission;

            if (!ClientConfiguration.Instance.ModMode && 
                (!CUpdater.IsFileNonexistantOrOriginal(mission.Scenario) || AreFilesModified()))
            {
                // Confront the user by showing the cheater screen
                missionToLaunch = mission;
                cheaterWindow.Enable();
                return;
            }

            LaunchMission(mission);
        }

        private bool AreFilesModified()
        {
            foreach (string filePath in filesToCheck)
            {
                if (!CUpdater.IsFileNonexistantOrOriginal(filePath))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the user wants to proceed to the mission despite having
        /// being called a cheater.
        /// </summary>
        private void CheaterWindow_YesClicked(object sender, EventArgs e)
        {
            LaunchMission(missionToLaunch);
        }

        /// <summary>
        /// Starts a singleplayer mission.
        /// </summary>
        /// <param name="scenario">The internal name of the scenario.</param>
        /// <param name="requiresAddon">True if the mission is for Firestorm / Enhanced Mode.</param>
        private void LaunchMission(Mission mission)
        {
            bool copyMapsToSpawnmapINI = ClientConfiguration.Instance.CopyMissionsToSpawnmapINI;

            Logger.Log("About to write spawn.ini.");
            StreamWriter swriter = new StreamWriter(ProgramConstants.GamePath + "spawn.ini");
            swriter.WriteLine("; Generated by DTA Client");
            swriter.WriteLine("[Settings]");
            if (copyMapsToSpawnmapINI)
                swriter.WriteLine("Scenario=spawnmap.ini");
            else
                swriter.WriteLine("Scenario=" + mission.Scenario);

            // No one wants to play missions on Fastest, so we'll change it to Faster
            if (UserINISettings.Instance.GameSpeed == 0)
                UserINISettings.Instance.GameSpeed.Value = 1;

            swriter.WriteLine("GameSpeed=" + UserINISettings.Instance.GameSpeed);
            swriter.WriteLine("Firestorm=" + mission.RequiredAddon);
            swriter.WriteLine("CustomLoadScreen=" + LoadingScreenController.GetLoadScreenName(mission.Side));
            swriter.WriteLine("IsSinglePlayer=Yes");
            swriter.WriteLine("SidebarHack=" + ClientConfiguration.Instance.SidebarHack);
            swriter.WriteLine("Side=" + mission.Side);
            swriter.WriteLine("BuildOffAlly=" + mission.BuildOffAlly);
            if (UserINISettings.Instance.EnableSPAutoSave)
                swriter.WriteLine("AutoSaveGame=" + ClientConfiguration.Instance.SinglePlayerAutoSaveInterval);

            UserINISettings.Instance.Difficulty.Value = trbDifficultySelector.Value;

            swriter.WriteLine("DifficultyModeHuman=" + (mission.PlayerAlwaysOnNormalDifficulty ? "1" : trbDifficultySelector.Value.ToString()));
            swriter.WriteLine("DifficultyModeComputer=" + GetComputerDifficulty());

            // Write global flag information
            if (mission.UsedGlobalVariables.Length > 0)
            {
                swriter.WriteLine();
                swriter.WriteLine("[GlobalFlags]");
                for (int i = 0; i < mission.UsedGlobalVariables.Length && i < MAX_GLOBAL_COUNT; i++)
                {
                    string globalFlagName = mission.UsedGlobalVariables[i];

                    CampaignGlobalVariable globalVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == globalFlagName);
                    if (globalVariable != null)
                    {
                        if (globalVariableValues[i].SelectedIndex > 0)
                        {
                            string flagIndex = globalVariable.Index.ToString(CultureInfo.InvariantCulture);
                            swriter.WriteLine($"GlobalFlag{ flagIndex }=yes");
                        }
                    }
                }
            }

            swriter.WriteLine();
            swriter.WriteLine();
            swriter.WriteLine();
            swriter.Close();

            IniFile difficultyIni = new IniFile(ProgramConstants.GamePath + DifficultyIniPaths[trbDifficultySelector.Value]);
            string difficultyName = DifficultyNames[trbDifficultySelector.Value];

            if (copyMapsToSpawnmapINI)
            {
                IniFile mapIni = new IniFile(ProgramConstants.GamePath + mission.Scenario);
                IniFile.ConsolidateIniFiles(mapIni, difficultyIni);

                // Force values of EndOfGame and SkipScore as our progression tracking currently relies on them
                mapIni.SetBooleanValue("Basic", "EndOfGame", true);
                mapIni.SetBooleanValue("Basic", "SkipScore", false);
                mapIni.WriteIniFile(ProgramConstants.GamePath + "spawnmap.ini");
            }

            UserINISettings.Instance.Difficulty.Value = trbDifficultySelector.Value;
            UserINISettings.Instance.SaveSettings();

            ((MainMenuDarkeningPanel)Parent).Hide();

            discordHandler?.UpdatePresence(mission.GUIName, difficultyName, mission.IconPath, true);
            GameProcessLogic.GameProcessExited += GameProcessExited_Callback;

            GameProcessLogic.StartGameProcess(new GameSessionManager(
                new GameSessionInfo(GameSessionType.SINGLEPLAYER,
                DateTime.Now.Ticks,
                mission.InternalName,
                mission.Side,
                (DifficultyRank)(trbDifficultySelector.Value + 1)),
                WindowManager.AddCallback));
        }

        private int GetComputerDifficulty() =>
            Math.Abs(trbDifficultySelector.Value - 2);

        private void GameProcessExited_Callback()
        {
            WindowManager.AddCallback(new Action(GameProcessExited), null);
        }

        protected virtual void GameProcessExited()
        {
            GameProcessLogic.GameProcessExited -= GameProcessExited_Callback;
            // Logger.Log("GameProcessExited: Updating Discord Presence.");
            discordHandler?.UpdatePresence();
            CampaignHandler.Instance.PostGameExitOnSingleplayerMission(GameProcessLogic.GameSessionManager.SessionInfo);
        }

        public void ListBattles()
        {
            lbCampaignList.Clear();

            CampaignHandler.Instance.BattleList.ForEach(mission =>
            {
                XNAListBoxItem item = new XNAListBoxItem();
                item.Tag = mission;
                item.Text = mission.GUIName;
                if (!mission.Enabled)
                {
                    item.TextColor = UISettings.ActiveSettings.DisabledItemColor;
                }
                else if (string.IsNullOrEmpty(mission.Scenario) && string.IsNullOrWhiteSpace(mission.CampaignInternalName))
                {
                    item.TextColor = AssetLoader.GetColorFromString(
                        ClientConfiguration.Instance.ListBoxHeaderColor);
                    item.IsHeader = true;
                    item.Selectable = false;
                }
                else if (mission.RequiresUnlocking && !mission.IsUnlocked)
                {
                    item.TextColor = UISettings.ActiveSettings.DisabledItemColor;
                    item.Text = "Locked Mission";
                    item.Texture = AssetLoader.LoadTexture("randomicon.png");
                }
                else
                {
                    item.TextColor = lbCampaignList.DefaultItemColor;
                }

                if (item.Texture == null && !string.IsNullOrEmpty(mission.IconPath))
                    item.Texture = AssetLoader.LoadTexture(mission.IconPath);

                lbCampaignList.AddItem(item);
            });
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
