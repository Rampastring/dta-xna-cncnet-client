using ClientGUI;
using DTAClient.Domain.Multiplayer;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;

namespace DTAClient.DXGUI.Multiplayer.GameLobby
{
    public class GameModeDescriptionPanel : XNAPanel
    {
        public GameModeDescriptionPanel(WindowManager windowManager) : base(windowManager)
        {
            DrawMode = ControlDrawMode.UNIQUE_RENDER_TARGET;
            InputEnabled = false;
        }

        private string _description = string.Empty;
        private string Description 
        {
            get => _description;
            set
            {
                _description = Renderer.FixText(value, 0, Width - UIDesignConstants.EMPTY_SPACE_SIDES * 2).Text;
                Height = (int)Renderer.GetTextDimensions(_description, 0).Y + UIDesignConstants.EMPTY_SPACE_TOP + UIDesignConstants.EMPTY_SPACE_BOTTOM;
            }
        }

        private GameMode _gameMode;
        public GameMode GameMode
        {
            get => _gameMode;
            set
            {
                if (_gameMode != value)
                {
                    _gameMode = value;
                    Description = _gameMode.Description;
                }
            }
        }

        public override void Initialize()
        {
            Name = nameof(GameModeDescriptionPanel);
            ClientRectangle = new Rectangle(0, 0, 300, 0);
            BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 255), 1, 1);

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            DrawStringWithShadow(Description, 0, 
                new Vector2(UIDesignConstants.EMPTY_SPACE_SIDES, UIDesignConstants.EMPTY_SPACE_TOP),
                UISettings.ActiveSettings.AltColor);
        }
    }
}
