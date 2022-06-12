using ClientCore;
using ClientGUI;
using DTAClient.Domain;
using DTAClient.Domain.Singleplayer;
using DTAClient.DXGUI.Generic.Campaign;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public event EventHandler MusicOptions;

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
            "INI/ArtE.ini",
            "INI/Base/AI.ini",
            "INI/Base/AIE.ini",
            "INI/Base/Art.ini",
            "INI/Base/Rules.ini",
            "INI/Base/Enhance.ini",
            "INI/Map Code/Difficulty Hard.ini",
            "INI/Map Code/Difficulty Medium.ini",
            "INI/Map Code/Difficulty Easy.ini"
        };

        private Mission missionToLaunch;
        private bool isCheater;
        private Dictionary<int, bool> globalFlagInfo;
        private string difficultyName;

        private StoryDisplay storyDisplay;

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
            missionCompletionNotification.DrawOrder = 9999;
            missionCompletionNotification.UpdateOrder = 9999;
            WindowManager.AddAndInitializeControl(missionCompletionNotification);
            missionCompletionNotification.Disable();

            CampaignHandler.Instance.MissionRankUpdated += CampaignHandler_MissionRankUpdated;
            CampaignHandler.Instance.MissionCompleted += CampaignHandler_MissionCompleted;

            storyDisplay = new StoryDisplay(WindowManager);
            storyDisplay.Name = nameof(storyDisplay);
            storyDisplay.DrawOrder = missionCompletionNotification.DrawOrder - 1;
            storyDisplay.UpdateOrder = missionCompletionNotification.UpdateOrder - 1;
            WindowManager.AddAndInitializeControl(storyDisplay);
        }

        private void CampaignHandler_MissionCompleted(object sender, MissionCompletionEventArgs e)
        {
            var parentPanel = (MainMenuDarkeningPanel)Parent;
            parentPanel.Show(this);
            parentPanel.Alpha = 1.0f;

            ListBattles();

            // If another mission follows the completed mission, select the following mission.
            // Otherwise select the mission itself.

            // If we unlocked a specific mission, select it.
            if (e.PrimaryUnlockedMission != null)
            {
                SelectMission(e.PrimaryUnlockedMission);
                return;
            }

            // Otherwise, build a list of all missions that follow the completed mission
            // and select one of them. Could be that the user has already previously unlocked
            // all missions that follow the completed mission, hence there were no new unlocks.
            var followList = new List<string>();
            Array.ForEach(e.Mission.UnlockMissions, unlockedMissionName => followList.Add(unlockedMissionName));
            e.Mission.ConditionalMissionUnlocks.ForEach(c => followList.Add(c.UnlockMissionName));

            if (followList.Count == 0)
            {
                // No mission follows, select the entry that matches our current mission

                SelectMission(e.Mission);
                return;
            }

            foreach (string missionName in followList)
            {
                // At least one mission follows, iterate through possibly unlocked missions until we find one

                int index = lbCampaignList.Items.FindIndex(item => 
                {
                    var mission = (Mission)item.Tag;
                    return mission.InternalName == missionName && mission.IsAvailableToPlay;
                });

                if (index > -1)
                {
                    lbCampaignList.SelectedIndex = index;
                    break;
                }
            }

            // If no following mission was unlocked, then select the entry that matches our current mission
            if (lbCampaignList.SelectedItem == null)
            {
                SelectMission(e.Mission);
            }

            storyDisplay.Begin(e.Mission.EndCutscene, true);
        }

        private void SelectMission(Mission mission)
        {
            lbCampaignList.SelectedIndex = lbCampaignList.Items.FindIndex(item => ((Mission)item.Tag) == mission);
        }

        private void CampaignHandler_MissionRankUpdated(object sender, MissionCompletionEventArgs e)
        {
            missionCompletionNotification.Show(e.Mission);
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
            lblPreconditionsHeader.Disable();
            for (int i = 0; i < MAX_GLOBAL_COUNT; i++)
            {
                globalVariableNames[i].Disable();
                globalVariableValues[i].Disable();
            }

            tbMissionDescription.Height = globalVariableValues[0].Bottom - tbMissionDescription.Y;


            if (mission != null && mission.UsedGlobalVariables.Length > 0)
            {
                lblPreconditionsHeader.Enable();

                for (int i = 0; i < mission.UsedGlobalVariables.Length && i < MAX_GLOBAL_COUNT; i++)
                {
                    CampaignGlobalVariable global = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == mission.UsedGlobalVariables[i]);

                    if (global == null)
                    {
                        Logger.Log(nameof(CampaignSelector) + ": Unable to find global: " + mission.UsedGlobalVariables[i]);
                        XNAMessageBox.Show(WindowManager, "Campaign Configuration Error",
                            "Unable to find global variable used by mission: " + mission.UsedGlobalVariables[i]);
                        return;
                    }

                    globalVariableNames[i].Text = global.UIName;
                    globalVariableNames[i].TextColor = UISettings.ActiveSettings.TextColor;
                    globalVariableNames[i].Enable();

                    bool disabledSelectable = global.IsDisabledUnlocked || global.DisableOptionFreeUnlock;
                    bool enabledSelectable = global.IsEnabledUnlocked;

                    if (disabledSelectable)
                    {
                        globalVariableValues[i].Items[0].Text = global.UIDisabledOption ?? "No";
                    }
                    else
                    {
                        globalVariableValues[i].Items[0].Text = "Option not unlocked";
                    }

                    if (enabledSelectable)
                    {
                        globalVariableValues[i].Items[1].Text = global.UIEnabledOption ?? "Yes";
                    }
                    else
                    {
                        globalVariableValues[i].Items[1].Text = "Option not unlocked";
                    }

                    globalVariableValues[i].Items[0].Selectable = disabledSelectable;
                    globalVariableValues[i].Items[1].Selectable = enabledSelectable;
                    globalVariableValues[i].SelectedIndex = 0;
                    if (global.IsEnabledUnlocked)
                    {
                        if (!global.IsDisabledUnlocked || global.EnabledThroughPreviousScenario)
                            globalVariableValues[i].SelectedIndex = 1;
                    }

                    if (global.HideIfNotEnabledUnlocked && !global.IsEnabledUnlocked)
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
                preconditionsHeaderY -= UIDesignConstants.CONTROL_VERTICAL_MARGIN * 3;
                lblPreconditionsHeader.Y = preconditionsHeaderY;
                lblPreconditionsHeader.Enable();

                tbMissionDescription.Height = lblPreconditionsHeader.Y - (UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2) - tbMissionDescription.Y;
            }
        }

        private void BtnCancel_LeftClick(object sender, EventArgs e)
        {
            Enabled = false;
        }

        private void BtnLaunch_LeftClick(object sender, EventArgs e)
        {
            var mission = lbCampaignList.SelectedItem.Tag as Mission;

            isCheater = false;
            missionToLaunch = mission;

            // Beginning of the "pre-mission-launch" prompt chain.
            // 1. cheater prompt
            // 2. difficulty warning
            // 3. in-game music prompt

            if (!ClientConfiguration.Instance.ModMode && 
                (!CUpdater.IsFileNonexistantOrOriginal(mission.Scenario) || AreFilesModified()))
            {
                // Confront the user by showing the cheater screen
                isCheater = true;
                cheaterWindow.Enable();
                return;
            }

            PostCheaterWindow();
        }

        /// <summary>
        /// Executed after the "Cheater" dialog has been resolved (iow. never displayed,
        /// or it was displayed and the user didn't care about cheating).
        /// </summary>
        private void PostCheaterWindow()
        {
            // Is this mission probably too hard for the user? If so, display a warning dialog.
            if (trbDifficultySelector.Value == 2 && missionToLaunch.WarnOnHardWithoutMediumPlayed && (int)missionToLaunch.Rank < (int)DifficultyRank.NORMAL)
            {
                var messageBox = XNAMessageBox.ShowYesNoDialog(WindowManager,
                    "Difficulty warning",
                    "This mission can be quite challenging. It's recommended to clear it on lower" + Environment.NewLine +
                    "difficulty levels first before attempting it on the hardest difficulty level." + Environment.NewLine + Environment.NewLine +
                    "Are you still feeling suicidal enough to attempt it on the hardest difficulty level?");
                messageBox.btnYes.Text = "Yes";
                messageBox.btnNo.Text = "I'll reconsider";
                messageBox.YesClickedAction = _ => PostDifficultyWarning();
                return;
            }

            PostDifficultyWarning();
        }

        private void PostDifficultyWarning()
        {
            InGameMusicNotification();
        }

        private void InGameMusicNotification()
        {
            if (missionToLaunch.MusicRecommended && UserINISettings.Instance.ScoreVolume == 0.0)
            {
                var messageBox = XNAMessageBox.ShowYesNoDialog(WindowManager,
                    "In-Game Music Recommended",
                    "This mission has dynamic music that can be a significant part of the game experience." + Environment.NewLine +
                    "You currently have in-game music turned off (volume 0 in the game settings)." + Environment.NewLine + Environment.NewLine +
                    "Would you want to go to the Options menu to enable in-game music?", 121);
                messageBox.btnYes.Text = "Yes";
                messageBox.btnNo.Text = "I don't want music";
                messageBox.YesClickedAction = _ => MusicOptions?.Invoke(this, EventArgs.Empty);
                messageBox.NoClickedAction = _ => PostMusicNotification();
                return;
            }

            PostMusicNotification();
        }

        private void PostMusicNotification()
        {
            LaunchMission();
        }

        private bool AreFilesModified()
        {
            foreach (string filePath in filesToCheck)
            {
                if (!CUpdater.IsFileNonexistantOrOriginal(filePath))
                {
                    Logger.Log("Modified file detected: " + filePath);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Called when the user wants to proceed to the mission despite having
        /// been called a cheater.
        /// </summary>
        private void CheaterWindow_YesClicked(object sender, EventArgs e)
        {
            PostCheaterWindow();
        }

        /// <summary>
        /// Starts a singleplayer mission.
        /// </summary>
        /// <param name="scenario">The internal name of the scenario.</param>
        /// <param name="requiresAddon">True if the mission is for Firestorm / Enhanced Mode.</param>
        private void LaunchMission()
        {
            bool copyMapsToSpawnmapINI = ClientConfiguration.Instance.CopyMissionsToSpawnmapINI;

            // Gather global flag information
            globalFlagInfo = new Dictionary<int, bool>();

            if (missionToLaunch.UsedGlobalVariables.Length > 0)
            {
                for (int i = 0; i < missionToLaunch.UsedGlobalVariables.Length && i < MAX_GLOBAL_COUNT; i++)
                {
                    string globalFlagName = missionToLaunch.UsedGlobalVariables[i];

                    CampaignGlobalVariable globalVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == globalFlagName);
                    if (globalVariable != null)
                    {
                        // Global flag values default to disabled/false by default,
                        // so we only need to care about enabled flags
                        if (globalVariableValues[i].SelectedIndex > 0)
                        {
                            string flagIndex = globalVariable.Index.ToString(CultureInfo.InvariantCulture);
                            globalFlagInfo.Add(globalVariable.Index, true);
                        }
                    }
                }
            }

            CampaignHandler.Instance.WriteFilesForMission(missionToLaunch, trbDifficultySelector.Value, globalFlagInfo);
            difficultyName = DifficultyNames[trbDifficultySelector.Value];

            UserINISettings.Instance.Difficulty.Value = trbDifficultySelector.Value;
            UserINISettings.Instance.SaveSettings();

            ((MainMenuDarkeningPanel)Parent).Hide();

            storyDisplay.Finished += LaunchMission_PostStoryDisplay;
            storyDisplay.Begin(missionToLaunch.StartCutscene, false);
        }

        private void LaunchMission_PostStoryDisplay(object sender, EventArgs e)
        {
            discordHandler?.UpdatePresence(missionToLaunch.GUIName, difficultyName, missionToLaunch.IconPath, true);
            GameProcessLogic.GameProcessExited += GameProcessExited_Callback;

            GameProcessLogic.StartGameProcess(new GameSessionManager(
                new GameSessionInfo(GameSessionType.SINGLEPLAYER,
                DateTime.Now.Ticks,
                missionToLaunch.InternalName,
                missionToLaunch.Side,
                (DifficultyRank)(trbDifficultySelector.Value + 1),
                globalFlagInfo,
                isCheater),
                WindowManager.AddCallback));

            storyDisplay.Finished -= LaunchMission_PostStoryDisplay;
        }

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
