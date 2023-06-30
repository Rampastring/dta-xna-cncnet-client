using ClientGUI;
using DTAClient.Domain.Singleplayer;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class BonuseselectionWindow : XNAWindow
    {
        public BonuseselectionWindow(WindowManager windowManager) : base(windowManager)
        {
        }

        public Bonus SelectedBonus { get; set; }

        public event EventHandler Bonuseselected;

        private XNAListBox lbBonusList;
        private BonusInfoPanel BonusInfoPanel;

        public override void Initialize()
        {
            Name = nameof(BonuseselectionWindow);
            Width = 300;

            BackgroundTexture = AssetLoader.LoadTexture("cncnetlobbybg.png");

            var lblHeader = new XNALabel(WindowManager);
            lblHeader.Name = nameof(lblHeader);
            lblHeader.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblHeader.Y = UIDesignConstants.EMPTY_SPACE_TOP;
            lblHeader.Text = "SELECT BONUS";
            AddChild(lblHeader);

            var lblDescription = new XNALabel(WindowManager);
            lblDescription.Name = nameof(lblDescription);
            lblDescription.Y = lblHeader.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblDescription.Text = "Bonuses are modifiers that allow you to customize your" + Environment.NewLine + 
                "tactics for the mission." + Environment.NewLine + Environment.NewLine +
                "Play Covert Revolt missions to unlock more bonuses!";
            AddChild(lblDescription);

            lbBonusList = new XNAListBox(WindowManager);
            lbBonusList.Name = nameof(lbBonusList);
            lbBonusList.X = 12;
            lbBonusList.Y = lblDescription.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN;
            lbBonusList.Width = Width - (lbBonusList.X * 2);
            lbBonusList.Height = 350;
            AddChild(lbBonusList);
            lbBonusList.SelectedIndexChanged += LbBonusList_SelectedIndexChanged;
            lbBonusList.HoveredIndexChanged += LbBonusList_HoveredIndexChanged;

            var btnSelect = new XNAClientButton(WindowManager);
            btnSelect.Name = nameof(btnSelect);
            btnSelect.Width = 110;
            btnSelect.Text = "Select";
            btnSelect.Y = lbBonusList.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            AddChild(btnSelect);
            btnSelect.LeftClick += BtnSelect_LeftClick;

            lblHeader.CenterOnParentHorizontally();
            btnSelect.CenterOnParentHorizontally();

            Height = btnSelect.Bottom + UIDesignConstants.EMPTY_SPACE_BOTTOM * 2;

            base.Initialize();

            BonusInfoPanel = new BonusInfoPanel(WindowManager);
            AddChild(BonusInfoPanel);
            BonusInfoPanel.Disable();
        }

        private void LbBonusList_HoveredIndexChanged(object sender, EventArgs e)
        {
            if (lbBonusList.HoveredItem == null)
            {
                BonusInfoPanel.Disable();
                if (BonusInfoPanel.Detached)
                {
                    BonusInfoPanel.Attach();
                }

                return;
            }

            BonusInfoPanel.SetBonus(lbBonusList.HoveredItem.Tag as Bonus);
            if (!BonusInfoPanel.Detached)
                BonusInfoPanel.Detach();

            BonusInfoPanel.X = Width;
            BonusInfoPanel.Y = GetCursorPoint().Y;
            BonusInfoPanel.Enable();
        }

        private void LbBonusList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbBonusList.SelectedItem == null)
            {
                SelectedBonus = null;
                return;
            }

            SelectedBonus = (Bonus)lbBonusList.SelectedItem.Tag;
        }

        public void Open()
        {
            Enable();

            lbBonusList.Clear();
            lbBonusList.AddItem("No Bonus");

            CampaignHandler.Instance.Bonuses.FindAll(t => t.Unlocked).ForEach(t => lbBonusList.AddItem(new XNAListBoxItem() { Text = t.UIName, Tag = t }));
        }

        private void BtnSelect_LeftClick(object sender, EventArgs e)
        {
            BonusInfoPanel.Disable();
            if (BonusInfoPanel.Detached)
            {
                BonusInfoPanel.Attach();
            }

            Bonuseselected?.Invoke(this, EventArgs.Empty);
            Disable();
        }
    }
}
