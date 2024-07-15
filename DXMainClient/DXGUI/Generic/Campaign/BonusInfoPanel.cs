using ClientGUI;
using DTAClient.Domain.Singleplayer;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class BonusInfoPanel : XNAPanel
    {
        public BonusInfoPanel(WindowManager windowManager) : base(windowManager)
        {
            PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.TILED;
        }

        private XNALabel lblBonusName;
        private XNALabel lblBonusEffects;
        private XNALabel lblBonusLore;
        private XNALabel lblUnlockedFrom;

        public override void Initialize()
        {
            Width = 200;

            BackgroundTexture = AssetLoader.LoadTexture("msgboxform.png");
            DrawBorders = true;

            lblBonusName = new XNALabel(WindowManager);
            lblBonusName.Name = nameof(lblBonusName);
            lblBonusName.X = UIDesignConstants.EMPTY_SPACE_SIDES;
            lblBonusName.Y = UIDesignConstants.EMPTY_SPACE_TOP;
            lblBonusName.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblBonusName.Text = "Bonus Name";
            AddChild(lblBonusName);

            lblBonusEffects = new XNALabel(WindowManager);
            lblBonusEffects.Name = nameof(lblBonusEffects);
            lblBonusEffects.X = UIDesignConstants.EMPTY_SPACE_SIDES;
            lblBonusEffects.Y = lblBonusName.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblBonusEffects.Text = "Bonus Effects";
            AddChild(lblBonusEffects);

            lblBonusLore = new XNALabel(WindowManager);
            lblBonusLore.Name = nameof(lblBonusLore);
            lblBonusLore.X = UIDesignConstants.EMPTY_SPACE_SIDES;
            lblBonusLore.Y = lblBonusName.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblBonusLore.TextColor = UISettings.ActiveSettings.SubtleTextColor;
            lblBonusLore.Text = "Bonus Lore";
            AddChild(lblBonusLore);

            lblUnlockedFrom = new XNALabel(WindowManager);
            lblUnlockedFrom.Name = nameof(lblUnlockedFrom);
            lblUnlockedFrom.X = UIDesignConstants.EMPTY_SPACE_SIDES;
            lblUnlockedFrom.Y = lblBonusName.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblUnlockedFrom.Text = "Unlocked from: X";
            AddChild(lblUnlockedFrom);

            base.Initialize();
        }

        public void SetBonus(Bonus bonus)
        {
            if (bonus == null) 
            {
                lblBonusName.Text = "No bonus";
                lblBonusEffects.Text = "No effects";
                lblBonusLore.Disable();
                lblUnlockedFrom.Disable();
                Height = lblBonusEffects.Bottom + UIDesignConstants.EMPTY_SPACE_BOTTOM;
                return;
            }

            int maxTextWidth = Width - lblBonusEffects.X - UIDesignConstants.EMPTY_SPACE_SIDES;
            lblBonusName.Text = bonus.UIName;
            lblBonusEffects.Text = Renderer.FixText(bonus.Description, lblBonusEffects.FontIndex, maxTextWidth).Text;
            lblBonusLore.Y = lblBonusEffects.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblBonusLore.Text = Renderer.FixText(bonus.LoreDescription, lblBonusLore.FontIndex, maxTextWidth).Text;
            lblBonusLore.Enable();
            lblUnlockedFrom.Y = lblBonusLore.Bottom + UIDesignConstants.CONTROL_VERTICAL_MARGIN * 2;
            lblUnlockedFrom.Enable();

            string unlockMissionName = "Unknown mission";
            var mission = CampaignHandler.Instance.Missions.Find(m => m.InternalName == bonus.UnlockFromMission);
            if (mission != null)
            {
                unlockMissionName = mission.GUIName;
            }
            
            lblUnlockedFrom.Text = "Unlocked from:" + Environment.NewLine + Renderer.FixText(unlockMissionName, lblUnlockedFrom.FontIndex, maxTextWidth).Text;
            Height = lblUnlockedFrom.Bottom + UIDesignConstants.EMPTY_SPACE_BOTTOM;
        }
    }
}
