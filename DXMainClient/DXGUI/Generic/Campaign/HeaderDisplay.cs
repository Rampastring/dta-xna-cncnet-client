using ClientGUI;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class HeaderDisplay : XNAPanel
    {
        private const float SmoothFadeAlphaRate = 0.05f;

        public HeaderDisplay(WindowManager windowManager) : base(windowManager)
        {
            DrawMode = ControlDrawMode.UNIQUE_RENDER_TARGET;
            InputEnabled = false;
        }

        public Color TextColor { get; set; } = Color.White;

        public int FontIndex { get; set; } = UIDesignConstants.REGULAR_FONT_INDEX;

        private string _headerText;
        public override string Text 
        { 
            get => _headerText;
            set 
            {
                if (string.IsNullOrEmpty(_headerText) && !string.IsNullOrEmpty(value))
                {
                    // Apply a smooth transition
                    InstantHide();
                    Show();
                }

                _headerText = value;
                RefreshTextPosition();
            }
        }

        private bool _isCentered;
        public bool IsCentered 
        {
            get => _isCentered;
            set
            {
                if (_isCentered != value)
                {
                    _isCentered = value;
                    RefreshTextPosition();
                }
            }
        }

        private int textXOffset;
        private int textYOffset;

        private void RefreshTextPosition()
        {
            if (Text == null)
                return;

            if (!IsCentered)
            {
                textXOffset = UIDesignConstants.EMPTY_SPACE_SIDES;
            }
            else
            {
                Vector2 textDimensions = Renderer.GetTextDimensions(Text, FontIndex);

                textXOffset = (Width - (int)textDimensions.X) / 2;
                textYOffset = (Height - (int)textDimensions.Y) / 2;
            }
        }

        protected override void OnClientRectangleUpdated()
        {
            RefreshTextPosition();
            base.OnClientRectangleUpdated();
        }

        /// <summary>
        /// Smoothly hides the header display.
        /// </summary>
        public void Hide()
        {
            AlphaRate = -SmoothFadeAlphaRate;
        }

        public void InstantHide()
        {
            Hide();
            Alpha = 0.0f;
        }

        /// <summary>
        /// Smoothly shows the header display (if it has valid text).
        /// </summary>
        public void Show()
        {
            AlphaRate = SmoothFadeAlphaRate;
        }

        public void InstantShow()
        {
            Show();
            Alpha = 1.0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Sets both the border and text color of the header display to the given color.
        /// </summary>
        public void SetColors(Color color)
        {
            BorderColor = color;
            TextColor = color;
        }

        public override void Draw(GameTime gameTime)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

            DrawPanel();
            DrawPanelBorders();

            DrawStringWithShadow(Text, FontIndex, new Vector2(textXOffset, textYOffset), TextColor);

            DrawChildren(gameTime);
        }
    }
}
