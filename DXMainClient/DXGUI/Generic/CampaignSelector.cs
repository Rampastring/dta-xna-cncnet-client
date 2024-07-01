using ClientCore;
using ClientGUI;
using DTAClient.Domain;
using DTAClient.Domain.Singleplayer;
using DTAClient.DXGUI.Generic.Campaign;
using DTAClient.DXGUI.Multiplayer.GameLobby;
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
                AssetLoader.LoadTexture("rankHard.png"),
                AssetLoader.LoadTexture("rankBrutal.png"),
            };

            EnableScrollbar = true;
        }

        private Texture2D DifficultyRankToTexture(DifficultyRank rank)
        {
            switch (rank)
            {
                case DifficultyRank.BRUTAL:
                    return rankTextures[3];
                case DifficultyRank.HARD:
                    return rankTextures[2];
                case DifficultyRank.NORMAL:
                    return rankTextures[1];
                case DifficultyRank.EASY:
                    return rankTextures[0];
                case DifficultyRank.NONE:
                default:
                    return null;
            }
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

        public string MissionAuthor { get; set; }

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
                DrawStringWithShadow(Text, FontIndex,
                    new Vector2(TextXMargin, TextYPosition), TextColor);
            }

            if (!string.IsNullOrWhiteSpace(MissionAuthor))
            {
                var briefingTextSize = Renderer.GetTextDimensions(Text, FontIndex);
                string authorText = "Mission Author: " + MissionAuthor;
                var missionAuthorTextSize = Renderer.GetTextDimensions(authorText, FontIndex);

                // Only draw mission author text if there is space for it
                if (briefingTextSize.Y + TextYPosition < Height - missionAuthorTextSize.Y)
                {
                    const int authorTextDistanceToBottom = 3;
                    DrawStringWithShadow(authorText, FontIndex,
                        new Vector2(Width - missionAuthorTextSize.X - UIDesignConstants.EMPTY_SPACE_SIDES,
                        Height - missionAuthorTextSize.Y - authorTextDistanceToBottom), UISettings.ActiveSettings.SubtleTextColor);
                }
            }

            if (DrawBorders)
                DrawPanelBorders();

            DrawChildren(gameTime);
        }
    }

    public class CampaignSelector : INItializableWindow
    {
        private const int MAX_GLOBAL_COUNT = 4;

        public event EventHandler MusicOptions;
        public event EventHandler RefreshMusicState;

        public CampaignSelector(WindowManager windowManager, DiscordHandler discordHandler) : base(windowManager)
        {
            this.discordHandler = discordHandler;
        }

        private DiscordHandler discordHandler;

        private XNAListBox lbCampaignList;
        private XNAClientButton btnLaunch;
        private MissionDescriptionBox tbMissionDescription;
        private XNATrackbar trbDifficultySelector;
        private XNAClientButton btnBonus;

        private XNALabel lblEasy;
        private XNALabel lblNormal;
        private XNALabel lblHard;
        private XNALabel lblBrutal;

        private XNALabel lblPreconditionsHeader;
        private XNALabel[] lblGlobalVariables = new XNALabel[MAX_GLOBAL_COUNT];
        private ToolTip[] globalVariableToolTips = new ToolTip[MAX_GLOBAL_COUNT];
        private XNADropDown[] cmbGlobalVariables = new XNADropDown[MAX_GLOBAL_COUNT];

        private CheaterWindow cheaterWindow;
        private BonusSelectionWindow BonuseSelectionWindow;

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

        private bool hasMissionBeenSelected = false;

        private StoryDisplay storyDisplay;

        public override void Initialize()
        {
            Name = nameof(CampaignSelector);
            base.Initialize();

            lbCampaignList = FindChild<BattleListBox>(nameof(lbCampaignList));
            lbCampaignList.SelectedIndexChanged += LbCampaignList_SelectedIndexChanged;

            tbMissionDescription = FindChild<MissionDescriptionBox>(nameof(tbMissionDescription));

            trbDifficultySelector = FindChild<XNATrackbar>(nameof(trbDifficultySelector));
            trbDifficultySelector.ButtonTexture = AssetLoader.LoadTextureUncached("trackbarButton_difficulty.png");

            // Create controls for global variable customization
            // The indexes increase from bottom to top, meaning
            // that with 4 lines the global indexes go like this:
            // global slot #3
            // global slot #2
            // global slot #1
            // global slot #0
            for (int i = 0; i < MAX_GLOBAL_COUNT; i++)
            {
                cmbGlobalVariables[i] = FindChild<XNADropDown>($"cmbGlobalVariable{i}");
                cmbGlobalVariables[i].AddItem("Disabled");
                cmbGlobalVariables[i].AddItem("Enabled");

                lblGlobalVariables[i] = FindChild<XNALabel>($"lblGlobalVariable{i}");

                globalVariableToolTips[i] = new ToolTip(WindowManager, lblGlobalVariables[i]);
            }

            lblPreconditionsHeader = FindChild<XNALabel>(nameof(lblPreconditionsHeader));

            PreconditionUIConfig(null);

            lblEasy = FindChild<XNALabel>(nameof(lblEasy));
            lblNormal = FindChild<XNALabel>(nameof(lblNormal));
            lblHard = FindChild<XNALabel>(nameof(lblHard));
            lblBrutal = FindChild<XNALabel>(nameof(lblBrutal));

            btnLaunch = FindChild<XNAClientButton>(nameof(btnLaunch));
            btnLaunch.AllowClick = false;
            btnLaunch.LeftClick += BtnLaunch_LeftClick;

            FindChild<XNAClientButton>("btnCancel").LeftClick += BtnCancel_LeftClick;

            btnBonus = FindChild<XNAClientButton>(nameof(btnBonus));
            btnBonus.LeftClick += BtnBonus_LeftClick;

            // Center on screen
            CenterOnParent();

            trbDifficultySelector.Value = Math.Min(trbDifficultySelector.MaxValue, UserINISettings.Instance.ClientDifficulty);

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

            BonuseSelectionWindow = new BonusSelectionWindow(WindowManager);
            dp = new DarkeningPanel(WindowManager);
            dp.AddChild(BonuseSelectionWindow);
            AddChild(dp);
            dp.CenterOnParent();
            BonuseSelectionWindow.CenterOnParent();
            BonuseSelectionWindow.Disable();
            RefreshBonusButtonText();
            BonuseSelectionWindow.Bonuseselected += (s, e) => RefreshBonusButtonText();
        }

        private void BtnBonus_LeftClick(object sender, EventArgs e)
        {
            BonuseSelectionWindow.Open();
        }

        private void CampaignHandler_MissionCompleted(object sender, MissionCompletionEventArgs e)
        {
            var parentPanel = (MainMenuDarkeningPanel)Parent;
            parentPanel.Show(this);
            parentPanel.Alpha = 1.0f;

            ListBattles();

            Mission nextMission = null;

            // If another mission follows the completed mission, select the following mission.
            // Otherwise select the mission itself.

            // If we unlocked a specific mission, select it.
            if (e.PrimaryUnlockedMission != null)
            {
                nextMission = e.PrimaryUnlockedMission;
            }

            // Otherwise, build a list of all missions that follow the completed mission
            // and select one of them. Could be that the user has already previously unlocked
            // all missions that follow the completed mission, hence there were no new unlocks.
            var followList = new List<string>();
            Array.ForEach(e.Mission.UnlockMissions, unlockedMissionName => followList.Add(unlockedMissionName));
            e.Mission.ConditionalMissionUnlocks.ForEach(c => followList.Add(c.UnlockMissionName));

            if (nextMission == null)
            {
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
                        nextMission = (Mission)lbCampaignList.Items[index].Tag;
                        break;
                    }
                }
            }

            // If no following mission was unlocked, then select the entry that matches our current mission
            if (nextMission == null)
            {
                nextMission = e.Mission;
            }

            SelectMission(e.Mission);

            if (e.Mission.EndCutscene != Cutscene.None)
            {
                storyDisplay.Finished += PostDebriefing_RefreshMusicState;
                storyDisplay.Begin(e.Mission.EndCutscene, true);
            }
        }

        private void PostDebriefing_RefreshMusicState(object sender, EventArgs e)
        {
            storyDisplay.Finished -= PostDebriefing_RefreshMusicState;
            RefreshMusicState?.Invoke(this, EventArgs.Empty);
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

        private void ConfigureDifficultySelectorForMission(Mission mission)
        {
            string[] difficultyLabels = new string[] { "EASY", "NORMAL", "HARD", "BRUTAL" };

            if (mission != null && mission.DifficultyLabels != null && (!mission.RequiresUnlocking || mission.IsUnlocked))
                difficultyLabels = mission.DifficultyLabels;

            lblEasy.Text = difficultyLabels[0].ToUpperInvariant();
            lblNormal.Text = difficultyLabels[1].ToUpperInvariant();
            lblHard.Text = difficultyLabels[2].ToUpperInvariant();

            bool hasExtendedDifficulty = mission != null && mission.HasExtendedDifficulty && (!mission.RequiresUnlocking || mission.IsUnlocked);

            if (hasExtendedDifficulty)
            {
                lblBrutal.Text = difficultyLabels[3].ToUpperInvariant();
                lblBrutal.Enable();
                trbDifficultySelector.MaxValue = 3;

                int difficultyStepWidth = ((trbDifficultySelector.Width - trbDifficultySelector.ButtonTexture.Width) / 3)
                    + (trbDifficultySelector.ButtonTexture.Width / 2);

                lblHard.TextAnchor = lblNormal.TextAnchor;
                lblHard.AnchorPoint = new Vector2(trbDifficultySelector.Right - difficultyStepWidth, lblHard.AnchorPoint.Y);

                lblNormal.AnchorPoint = new Vector2(trbDifficultySelector.X + difficultyStepWidth, lblNormal.AnchorPoint.Y);

                lblEasy.TextAnchor = lblNormal.TextAnchor;
                lblEasy.AnchorPoint = new Vector2(trbDifficultySelector.X + trbDifficultySelector.ButtonTexture.Width / 2, lblEasy.AnchorPoint.Y);
                lblBrutal.TextAnchor = lblNormal.TextAnchor;
                lblBrutal.AnchorPoint = new Vector2(trbDifficultySelector.Right - (trbDifficultySelector.ButtonTexture.Width / 2), lblBrutal.AnchorPoint.Y);
            }
            else
            {
                lblBrutal.Disable();
                lblHard.TextAnchor = LabelTextAnchorInfo.LEFT;
                lblHard.AnchorPoint = new Vector2(trbDifficultySelector.Right, lblHard.AnchorPoint.Y);

                lblNormal.AnchorPoint = new Vector2(trbDifficultySelector.X + (trbDifficultySelector.Width / 2), lblNormal.AnchorPoint.Y);

                lblEasy.TextAnchor = LabelTextAnchorInfo.RIGHT;
                lblEasy.AnchorPoint = new Vector2(trbDifficultySelector.X, lblEasy.AnchorPoint.Y);

                trbDifficultySelector.MaxValue = 2;
            }

            if (!hasMissionBeenSelected)
            {
                trbDifficultySelector.Value = UserINISettings.Instance.ClientDifficulty.Value;
            }

            trbDifficultySelector.Value = Math.Min(trbDifficultySelector.MaxValue, trbDifficultySelector.Value);
        }

        private void LbCampaignList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mission = lbCampaignList.SelectedItem != null ? lbCampaignList.SelectedItem.Tag as Mission : null;

            ConfigureDifficultySelectorForMission(mission);

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

            hasMissionBeenSelected = true;

            if (mission.AllowBonuses)
            {
                btnBonus.Enable();
            }
            else
            {
                btnBonus.Disable();
            }

            tbMissionDescription.LoadMissionBackgroundTexture(mission.PreviewImagePath);

            PreconditionUIConfig(mission);

            tbMissionDescription.Text = mission.GUIDescription;
            tbMissionDescription.MissionAuthor = mission.Author;

            if (!mission.Enabled)
            {
                btnLaunch.AllowClick = false;
                return;
            }

            btnLaunch.AllowClick = true;
        }

        private void RefreshBonusButtonText()
        {
            if (BonuseSelectionWindow.SelectedBonus == null)
            {
                btnBonus.Text = "No Bonus Selected";
                return;
            }

            btnBonus.Text = "Bonus: " + BonuseSelectionWindow.SelectedBonus.UIName;
        }

        /// <summary>
        /// Sets up the UI for a non-launchable mission state.
        /// </summary>
        private void MissionList_InvalidMission()
        {
            tbMissionDescription.LoadMissionBackgroundTexture(null);
            tbMissionDescription.Text = string.Empty;
            btnLaunch.AllowClick = false;
            btnBonus.Disable();
            tbMissionDescription.MissionAuthor = null;
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
                lblGlobalVariables[i].Disable();
                cmbGlobalVariables[i].Disable();
            }

            tbMissionDescription.Height = cmbGlobalVariables[0].Bottom - tbMissionDescription.Y;


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

                    lblGlobalVariables[i].Text = global.UIName;
                    lblGlobalVariables[i].TextColor = UISettings.ActiveSettings.TextColor;
                    lblGlobalVariables[i].Enable();
                    globalVariableToolTips[i].Text = global.ToolTip;

                    bool disabledSelectable = global.IsDisabledUnlocked || global.DisableOptionFreeUnlock;
                    bool enabledSelectable = global.IsEnabledUnlocked;

                    if (disabledSelectable)
                    {
                        cmbGlobalVariables[i].Items[0].Text = global.UIDisabledOption ?? "No";
                    }
                    else
                    {
                        cmbGlobalVariables[i].Items[0].Text = "Option not unlocked";
                    }

                    if (enabledSelectable)
                    {
                        cmbGlobalVariables[i].Items[1].Text = global.UIEnabledOption ?? "Yes";
                    }
                    else
                    {
                        cmbGlobalVariables[i].Items[1].Text = "Option not unlocked";
                    }

                    cmbGlobalVariables[i].Items[0].Selectable = disabledSelectable;
                    cmbGlobalVariables[i].Items[1].Selectable = enabledSelectable;
                    cmbGlobalVariables[i].SelectedIndex = 0;
                    if (global.IsEnabledUnlocked)
                    {
                        if (!global.IsDisabledUnlocked || global.EnabledThroughPreviousScenario)
                            cmbGlobalVariables[i].SelectedIndex = 1;
                    }

                    if (global.HideIfNotEnabledUnlocked && !global.IsEnabledUnlocked)
                    {
                        cmbGlobalVariables[i].Items[0].Text = "-";
                        cmbGlobalVariables[i].Items[1].Text = "-";
                        lblGlobalVariables[i].Text = "Unknown Condition (not yet unlocked)";
                        lblGlobalVariables[i].TextColor = UISettings.ActiveSettings.DisabledItemColor;
                        globalVariableToolTips[i].Text = string.Empty;
                    }

                    cmbGlobalVariables[i].Enable();
                }

                int preconditionsHeaderY = mission.UsedGlobalVariables.Length >= MAX_GLOBAL_COUNT ? lblGlobalVariables[0].Y :
                    lblGlobalVariables[mission.UsedGlobalVariables.Length - 1].Y;
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
            int hardestDifficultyLevel = missionToLaunch.HasExtendedDifficulty ? 3 : 2;

            // Is this mission probably too hard for the user? If so, display a warning dialog.
            if (trbDifficultySelector.Value == hardestDifficultyLevel && missionToLaunch.WarnOnHardWithoutMediumPlayed && (int)missionToLaunch.Rank < (int)DifficultyRank.NORMAL)
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
                        if (cmbGlobalVariables[i].SelectedIndex > 0)
                        {
                            // Global flag values default to disabled/false by default,
                            // so we only need to add enabled flags to the dictionary
                            string flagIndex = globalVariable.Index.ToString(CultureInfo.InvariantCulture);
                            globalFlagInfo.Add(globalVariable.Index, true);
                            globalVariable.EnabledThroughPreviousScenario = true;
                        }
                        else
                        {
                            globalVariable.EnabledThroughPreviousScenario = false;
                        }
                    }
                }
            }

            Difficulty bonusDifficulty = null;
            if (missionToLaunch.AllowBonuses && BonuseSelectionWindow.SelectedBonus != null)
                bonusDifficulty = BonuseSelectionWindow.SelectedBonus.Difficulty;

            CampaignHandler.Instance.WriteFilesForMission(missionToLaunch, TrackbarValueToDiffRank(), globalFlagInfo, bonusDifficulty);
            difficultyName = missionToLaunch.GetNameForDifficultyRankStylized(TrackbarValueToDiffRank());

            UserINISettings.Instance.ClientDifficulty.Value = trbDifficultySelector.Value;
            UserINISettings.Instance.SaveSettings();

            ((MainMenuDarkeningPanel)Parent).Hide();

            storyDisplay.Finished += LaunchMission_PostStoryDisplay;
            storyDisplay.Begin(missionToLaunch.StartCutscene, false);
        }

        private DifficultyRank TrackbarValueToDiffRank()
        {
            if (missionToLaunch == null || !missionToLaunch.HasExtendedDifficulty)
            {
                switch (trbDifficultySelector.Value)
                {
                    case 0:
                        return DifficultyRank.EASY;
                    case 1:
                        return DifficultyRank.HARD;
                    case 2:
                        return DifficultyRank.BRUTAL;
                    default:
                        throw new NotImplementedException("Invalid difficulty trackbar value!");
                }
            }

            switch (trbDifficultySelector.Value) 
            {
                case 0:
                    return DifficultyRank.EASY;
                case 1:
                    return DifficultyRank.NORMAL;
                case 2:
                    return DifficultyRank.HARD;
                case 3:
                    return DifficultyRank.BRUTAL;
                default:
                    throw new NotImplementedException("Invalid difficulty trackbar value!");
            }
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
                TrackbarValueToDiffRank(),
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

                if (!mission.Visible)
                {
                    return;
                }

                if (!mission.Enabled)
                {
                    item.TextColor = UISettings.ActiveSettings.DisabledItemColor;
                }
                else if (string.IsNullOrEmpty(mission.Scenario) && string.IsNullOrWhiteSpace(mission.CampaignInternalName))
                {
                    if (!string.IsNullOrEmpty(mission.HeaderFor))
                    {
                        Mission referencedMission = CampaignHandler.Instance.Missions.Find(m => m.InternalName == mission.HeaderFor);

                        if (referencedMission != null && referencedMission.RequiresUnlocking && !referencedMission.IsUnlocked)
                        {
                            // Don't add this to the campaign list
                            return;
                        }
                    }

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
