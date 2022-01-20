using ClientCore;
using ClientGUI;
using DTAClient.Domain.Singleplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;

namespace DTAClient.DXGUI.Generic
{
    public class MissionCompletionNotification : XNAPanel
    {
        public const float SlideRatePerSecond = 1000.0f;
        public const float LifetimeInSeconds = 5.0f;
        public const float DisappearingRate = 0.1f;

        public MissionCompletionNotification(WindowManager windowManager) : base(windowManager)
        {
            DrawMode = ControlDrawMode.UNIQUE_RENDER_TARGET;
        }

        private XNAPanel starIconPanel;
        private XNALabel lblMissionName;
        private XNALabel lblDescription;

        private Texture2D[] rankTextures;

        private float xPos;
        private double lifetime;

        public override void Initialize()
        {
            Name = nameof(MissionCompletionNotification);
            BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 196), 2, 2);
            PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;

            rankTextures = new Texture2D[]
            {
                AssetLoader.LoadTexture("rankEasy.png"),
                AssetLoader.LoadTexture("rankEasy.png"),
                AssetLoader.LoadTexture("rankNormal.png"),
                AssetLoader.LoadTexture("rankHard.png")
            };

            Width = 300;
            Height = 60;
            Y = 200;

            starIconPanel = new XNAPanel(WindowManager);
            starIconPanel.Name = nameof(starIconPanel);
            starIconPanel.Width = 60;
            starIconPanel.X = UIDesignConstants.EMPTY_SPACE_SIDES;
            starIconPanel.Y = UIDesignConstants.EMPTY_SPACE_TOP;
            starIconPanel.Height = Height - starIconPanel.Y - UIDesignConstants.EMPTY_SPACE_BOTTOM;
            starIconPanel.Width = starIconPanel.Height;
            starIconPanel.DrawBorders = false;
            starIconPanel.PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            starIconPanel.Disable();
            AddChild(starIconPanel);

            lblMissionName = new XNALabel(WindowManager);
            lblMissionName.Name = nameof(lblMissionName);
            lblMissionName.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblMissionName.TextAnchor = LabelTextAnchorInfo.HORIZONTAL_CENTER;
            lblMissionName.AnchorPoint = new Vector2(Width / 2f, UIDesignConstants.EMPTY_SPACE_TOP);
            lblMissionName.Text = "MISSION NAME";
            AddChild(lblMissionName);

            lblDescription = new XNALabel(WindowManager);
            lblDescription.Name = nameof(lblDescription);
            lblDescription.FontIndex = UIDesignConstants.BOLD_FONT_INDEX;
            lblDescription.TextAnchor = LabelTextAnchorInfo.HORIZONTAL_CENTER;
            lblDescription.AnchorPoint = new Vector2(Width / 2f, lblMissionName.Bottom + UIDesignConstants.EMPTY_SPACE_TOP * 2);
            lblDescription.Text = "Completed on Difficulty";
            AddChild(lblDescription);

            base.Initialize();
        }

        public void Show(Mission mission)
        {
            lblMissionName.Text = mission.GUIName.ToUpper();

            string difficultyName = ProgramConstants.DifficultyRankToName(mission.Rank);
            if (mission.DifficultyLabels != null)
            {
                difficultyName = mission.DifficultyLabels[(int)mission.Rank - 1];
            }

            if (difficultyName.Length > 1)
                difficultyName = difficultyName[0].ToString() + difficultyName.ToLower().Substring(1);

            lblDescription.Text = "Completed on " + difficultyName;

            starIconPanel.BackgroundTexture = rankTextures[(int)mission.Rank];

            X = WindowManager.RenderResolutionX;
            xPos = WindowManager.RenderResolutionX;
            lifetime = 0.0;
            Alpha = 1.0f;
            AlphaRate = 0f;
            WindowManager.ReorderControls();

            Enable();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (xPos + Width > WindowManager.RenderResolutionX)
            {
                xPos -= (float)gameTime.ElapsedGameTime.TotalMilliseconds * SlideRatePerSecond / 1000.0f;
                if (WindowManager.RenderResolutionX > xPos + Width)
                {
                    xPos = WindowManager.RenderResolutionX - Width;
                }

                X = (int)xPos;
            }

            lifetime += gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime > LifetimeInSeconds)
            {
                AlphaRate = -DisappearingRate;
            }

            if (Alpha == 0f)
                Disable();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer.PushSettings(new SpriteBatchSettings(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp));
            DrawTexture(starIconPanel.BackgroundTexture, starIconPanel.ClientRectangle, Color.White);
            Renderer.PopSettings();
        }
    }
}
