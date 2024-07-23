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
                AssetLoader.LoadTexture("rankNormal.png"),
                AssetLoader.LoadTexture("rankHard.png"),
                AssetLoader.LoadTexture("rankBrutal.png"),
            };

            Width = 300;
            Height = 70;
            Y = 200;

            starIconPanel = new XNAPanel(WindowManager);
            starIconPanel.Name = nameof(starIconPanel);
            starIconPanel.Width = 60;
            starIconPanel.X = UIDesignConstants.EMPTY_SPACE_SIDES;
            starIconPanel.Y = UIDesignConstants.EMPTY_SPACE_TOP * 2;
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
            lblDescription.AnchorPoint = new Vector2(Width / 2f, lblMissionName.Bottom + UIDesignConstants.EMPTY_SPACE_TOP * 3);
            lblDescription.Text = "Completed on Difficulty";
            AddChild(lblDescription);

            base.Initialize();
        }

        private Texture2D GetDifficultyRankTexture(Mission mission)
        {
            switch (mission.Rank)
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

        private int GetDifficultyLabelIndex(Mission mission)
        {
            if (mission.HasExtendedDifficulty)
            {
                switch (mission.Rank)
                {
                    case DifficultyRank.BRUTAL:
                        return 3;
                    case DifficultyRank.HARD:
                        return 2;
                    case DifficultyRank.NORMAL:
                        return 1;
                    case DifficultyRank.EASY:
                        return 0;
                }
            }
            else
            {
                switch (mission.Rank)
                {
                    case DifficultyRank.BRUTAL:
                        return 2;
                    case DifficultyRank.HARD:
                        return 1;
                    case DifficultyRank.NORMAL:
                    case DifficultyRank.EASY:
                        return 0;
                }
            }

            return 0;
        }

        public void Show(Mission mission)
        {
            lblMissionName.Text = mission.GUIName.ToUpper();

            string difficultyName = mission.GetNameForDifficultyRankStylized(mission.Rank);

            lblDescription.Text = "Completed on " + difficultyName;

            starIconPanel.BackgroundTexture = GetDifficultyRankTexture(mission);

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
