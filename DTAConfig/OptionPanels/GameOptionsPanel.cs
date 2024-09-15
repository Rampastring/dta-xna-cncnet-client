using ClientCore;
using ClientCore.CnCNet5;
using ClientGUI;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;

namespace DTAConfig.OptionPanels
{
    class GameOptionsPanel : XNAOptionsPanel
    {
        private const int TEXT_BACKGROUND_COLOR_TRANSPARENT = 0;
        private const int TEXT_BACKGROUND_COLOR_BLACK = 12;
        private const int MAX_SCROLL_RATE = 6;

        public GameOptionsPanel(WindowManager windowManager, UserINISettings iniSettings, XNAControl topBar)
            : base(windowManager, iniSettings)
        {
            this.topBar = topBar;
        }

        private XNALabel lblScrollRateValue;

        private XNATrackbar trbScrollRate;
        private XNAClientCheckBox chkTargetLines;
        private XNAClientCheckBox chkScrollCoasting;
        private XNAClientCheckBox chkTooltips;
        private XNAClientCheckBox chkFilterBandBoxSelection;

        private XNAClientCheckBox chkAltToUndeploy;
        private XNAClientCheckBox chkBlackChatBackground;
        private XNAClientCheckBox chkSortDefensesAsLast;

        private XNAControl topBar;

        private XNATextBox tbPlayerName;

        private HotkeyConfigurationWindow hotkeyConfigWindow;

        public override void Initialize()
        {
            base.Initialize();

            Name = "GameOptionsPanel";
            const int CheckBoxSpacing = 13;

            var lblScrollRate = new XNALabel(WindowManager);
            lblScrollRate.Name = "lblScrollRate";
            lblScrollRate.ClientRectangle = new Rectangle(12,
                14, 0, 0);
            lblScrollRate.Text = "Scroll Rate:";

            lblScrollRateValue = new XNALabel(WindowManager);
            lblScrollRateValue.Name = "lblScrollRateValue";
            lblScrollRateValue.FontIndex = 1;
            lblScrollRateValue.Text = "3";
            lblScrollRateValue.ClientRectangle = new Rectangle(
                Width - lblScrollRateValue.Width - 12,
                lblScrollRate.Y, 0, 0);

            trbScrollRate = new XNATrackbar(WindowManager);
            trbScrollRate.Name = "trbClientVolume";
            trbScrollRate.ClientRectangle = new Rectangle(
                lblScrollRate.Right + 32,
                lblScrollRate.Y - 2,
                lblScrollRateValue.X - lblScrollRate.Right - 47,
                22);
            trbScrollRate.BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 2, 2);
            trbScrollRate.MinValue = 0;
            trbScrollRate.MaxValue = MAX_SCROLL_RATE;
            trbScrollRate.ValueChanged += TrbScrollRate_ValueChanged;

            chkScrollCoasting = new XNAClientCheckBox(WindowManager);
            chkScrollCoasting.Name = "chkScrollCoasting";
            chkScrollCoasting.ClientRectangle = new Rectangle(
                lblScrollRate.X,
                trbScrollRate.Bottom + 20, 0, 0);
            chkScrollCoasting.Text = "Scroll Coasting";
            AddChild(chkScrollCoasting);

            chkTargetLines = new XNAClientCheckBox(WindowManager);
            chkTargetLines.Name = "chkTargetLines";
            chkTargetLines.ClientRectangle = new Rectangle(
                lblScrollRate.X,
                chkScrollCoasting.Bottom + CheckBoxSpacing, 0, 0);
            chkTargetLines.Text = "Target Lines";
            AddChild(chkTargetLines);

            chkTooltips = new XNAClientCheckBox(WindowManager);
            chkTooltips.Name = "chkTooltips";
            chkTooltips.Text = "Tooltips";
            chkTooltips.ClientRectangle = new Rectangle(
                lblScrollRate.X,
                chkTargetLines.Bottom + CheckBoxSpacing, 0, 0);
            AddChild(chkTooltips);

            chkFilterBandBoxSelection = new XNAClientCheckBox(WindowManager);
            chkFilterBandBoxSelection.Name = "chkFilterBandBoxSelection";
            chkFilterBandBoxSelection.Text = "Filter Band-Box Selection";
            chkFilterBandBoxSelection.ClientRectangle = new Rectangle(
                chkScrollCoasting.Right + 50,
                chkScrollCoasting.Y, 0, 0);
            AddChild(chkFilterBandBoxSelection);

            chkAltToUndeploy = new XNAClientCheckBox(WindowManager);
            chkAltToUndeploy.Name = "chkAltToUndeploy";
            chkAltToUndeploy.ClientRectangle = new Rectangle(
                chkFilterBandBoxSelection.X,
                chkFilterBandBoxSelection.Bottom + CheckBoxSpacing, 0, 0);
            chkAltToUndeploy.Text = "Hold Alt to Undeploy";
            AddChild(chkAltToUndeploy);

            chkBlackChatBackground = new XNAClientCheckBox(WindowManager);
            chkBlackChatBackground.Name = "chkBlackChatBackground";
            chkBlackChatBackground.ClientRectangle = new Rectangle(
                chkFilterBandBoxSelection.X,
                chkAltToUndeploy.Bottom + CheckBoxSpacing, 0, 0);
            chkBlackChatBackground.Text = "Dark Chat Background";
            AddChild(chkBlackChatBackground);

            chkSortDefensesAsLast = new XNAClientCheckBox(WindowManager);
            chkSortDefensesAsLast.Name = nameof(chkSortDefensesAsLast);
            chkSortDefensesAsLast.X = chkFilterBandBoxSelection.Right + 50;
            chkSortDefensesAsLast.Y = chkScrollCoasting.Y;
            chkSortDefensesAsLast.Text = "Sort Defenses as Last";
            AddChild(chkSortDefensesAsLast);

            var lblPlayerName = new XNALabel(WindowManager);
            lblPlayerName.Name = "lblPlayerName";
            lblPlayerName.Text = "Player Name*:";
            lblPlayerName.ClientRectangle = new Rectangle(
                lblScrollRate.X,
                chkBlackChatBackground.Bottom + 50, 0, 0);

            tbPlayerName = new XNATextBox(WindowManager);
            tbPlayerName.Name = "tbPlayerName";
            tbPlayerName.MaximumTextLength = ClientConfiguration.Instance.MaxNameLength;
            tbPlayerName.ClientRectangle = new Rectangle(trbScrollRate.X,
                lblPlayerName.Y - 2, 200, 19);
            tbPlayerName.Text = ProgramConstants.PLAYERNAME;

            var lblNotice = new XNALabel(WindowManager);
            lblNotice.Name = "lblNotice";
            lblNotice.ClientRectangle = new Rectangle(lblPlayerName.X,
                lblPlayerName.Bottom + 20, 0, 0);
            lblNotice.Text = "* If you are currently connected to CnCNet, you need to log out and reconnect" +
                Environment.NewLine + "for your new name to be applied.";

            hotkeyConfigWindow = new HotkeyConfigurationWindow(WindowManager);
            DarkeningPanel.AddAndInitializeWithControl(WindowManager, hotkeyConfigWindow);
            hotkeyConfigWindow.Disable();

            var btnConfigureHotkeys = new XNAClientButton(WindowManager);
            btnConfigureHotkeys.Name = "btnConfigureHotkeys";
            btnConfigureHotkeys.ClientRectangle = new Rectangle(lblPlayerName.X, lblNotice.Bottom + 20, 160, 23);
            btnConfigureHotkeys.Text = "Configure Hotkeys";
            btnConfigureHotkeys.LeftClick += BtnConfigureHotkeys_LeftClick;

            AddChild(lblScrollRate);
            AddChild(lblScrollRateValue);
            AddChild(trbScrollRate);
            AddChild(lblPlayerName);
            AddChild(tbPlayerName);
            AddChild(lblNotice);
            AddChild(btnConfigureHotkeys);
        }

        private void BtnConfigureHotkeys_LeftClick(object sender, EventArgs e)
        {
            hotkeyConfigWindow.Enable();

            if (topBar.Enabled)
            {
                topBar.Disable();
                hotkeyConfigWindow.EnabledChanged += HotkeyConfigWindow_EnabledChanged;
            }
        }

        private void HotkeyConfigWindow_EnabledChanged(object sender, EventArgs e)
        {
            hotkeyConfigWindow.EnabledChanged -= HotkeyConfigWindow_EnabledChanged;
            topBar.Enable();
        }

        private void TrbScrollRate_ValueChanged(object sender, EventArgs e)
        {
            lblScrollRateValue.Text = trbScrollRate.Value.ToString();
        }

        public void OpenHotkeyConfigurationWindow()
        {
            BtnConfigureHotkeys_LeftClick(this, EventArgs.Empty);
            hotkeyConfigWindow.OpenSidebarHotkeys();
        }

        public override void Load()
        {
            base.Load();
            
            int scrollRate = ReverseScrollRate(IniSettings.ScrollRate);

            if (scrollRate >= trbScrollRate.MinValue && scrollRate <= trbScrollRate.MaxValue)
            {
                trbScrollRate.Value = scrollRate;
                lblScrollRateValue.Text = scrollRate.ToString();
            }

            chkScrollCoasting.Checked = !Convert.ToBoolean(IniSettings.ScrollCoasting);
            chkTargetLines.Checked = IniSettings.TargetLines;
            chkTooltips.Checked = IniSettings.Tooltips;
            chkFilterBandBoxSelection.Checked = IniSettings.FilterBandBoxSelection;
            chkAltToUndeploy.Checked = !IniSettings.MoveToUndeploy;
            chkSortDefensesAsLast.Checked = IniSettings.SortDefensesAsLast;
            chkBlackChatBackground.Checked = IniSettings.TextBackgroundColor == TEXT_BACKGROUND_COLOR_BLACK;
            tbPlayerName.Text = UserINISettings.Instance.PlayerName;
        }

        public override bool Save()
        {
            base.Save();

            IniSettings.ScrollRate.Value = ReverseScrollRate(trbScrollRate.Value);

            IniSettings.ScrollCoasting.Value = Convert.ToInt32(!chkScrollCoasting.Checked);
            IniSettings.TargetLines.Value = chkTargetLines.Checked;
            IniSettings.Tooltips.Value = chkTooltips.Checked;
            IniSettings.FilterBandBoxSelection.Value = chkFilterBandBoxSelection.Checked;
            IniSettings.MoveToUndeploy.Value = !chkAltToUndeploy.Checked;
            IniSettings.SortDefensesAsLast.Value = chkSortDefensesAsLast.Checked;

            if (chkBlackChatBackground.Checked)
                IniSettings.TextBackgroundColor.Value = TEXT_BACKGROUND_COLOR_BLACK;
            else
                IniSettings.TextBackgroundColor.Value = TEXT_BACKGROUND_COLOR_TRANSPARENT;

            string playerName = NameValidator.GetValidOfflineName(tbPlayerName.Text);

            if (playerName.Length > 0)
                IniSettings.PlayerName.Value = playerName;

            return false;
        }

        private int ReverseScrollRate(int scrollRate)
        {
            return Math.Abs(scrollRate - MAX_SCROLL_RATE);
        }
    }
}
