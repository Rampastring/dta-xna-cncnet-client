using ClientGUI;
using Microsoft.Xna.Framework;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;

namespace DTAClient.DXGUI.Generic.Campaign
{
    /// <summary>
    /// A control for displaying conversations.
    /// </summary>
    public class ConversationDisplay : XNAPanel
    {
        public ConversationDisplay(WindowManager windowManager) : base(windowManager)
        {
            ProgressionSound = new EnhancedSoundEffect("Story/Sounds/BRIEFING.WAV");
            ProgressionSound.RepeatPrevention = 0.08f;
            ProgressionSound.Volume = 0.10f;
            // ProgressionSound.Volume = 0.5f;
            // ProgressionSound.RepeatPrevention = 0.08f;
            DrawMode = ControlDrawMode.UNIQUE_RENDER_TARGET;
            InputEnabled = false;
        }

        /// <summary>
        /// The time in seconds it takes for each character to appear.
        /// </summary>
        public double ConversationSpeed { get; set; } = 0.02;

        public Color TextColor { get; set; } = Color.White;

        public int FontIndex { get; set; } = UIDesignConstants.BOLD_FONT_INDEX;

        public EnhancedSoundEffect ProgressionSound { get; set; }

        private bool _isCentered;
        public bool IsCentered 
        {
            get => _isCentered;
            set
            {
                _isCentered = value;

                RefreshTextXOffset();
            }
        }

        private string _originalConversationText;
        private string _conversationText;
        public string ConversationText
        {
            get => _conversationText;
            set
            {
                _originalConversationText = value;
                _conversationText = Renderer.FixText(value, FontIndex, GetUsableWidth()).Text;
                ProgressedText = string.Empty;
                displayedText = string.Empty;

                RefreshTextXOffset();
            }
        }

        private string _progressedText;
        public string ProgressedText
        {
            get => _progressedText;
            set
            {
                _progressedText = value;
                displayedTextIsOutdated = true;
            }
        }

        private int textXOffset;

        private void RefreshTextXOffset()
        {
            if (!_isCentered)
            {
                textXOffset = 0;
            }
            else
            {
                int convWidth = (int)Renderer.GetTextDimensions(_conversationText, FontIndex).X;

                textXOffset = (GetUsableWidth() - convWidth) / 2;
            }
        }

        /// <summary>
        /// Calculates the part of the control's width that is usable for the text.
        /// </summary>
        private int GetUsableWidth() => Width - (UIDesignConstants.EMPTY_SPACE_SIDES * 2);

        public void Snap()
        {
            ProgressedText = ConversationText;
        }

        public bool IsReady()
        {
            return string.IsNullOrWhiteSpace(ConversationText) || ProgressedText.Length >= ConversationText.Length;
        }

        /// <summary>
        /// Progressed text but with potential 
        /// line breaks applied so the text fits the control.
        /// </summary>
        private string displayedText;
        private bool displayedTextIsOutdated = false;
        private double currentTime;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ProgressedText.Length < ConversationText.Length)
            {
                currentTime += gameTime.ElapsedGameTime.TotalSeconds;

                if (currentTime > ConversationSpeed)
                {
                    currentTime = 0.0;
                    ProgressedText += ConversationText[ProgressedText.Length];
                    if (ProgressionSound != null)
                        ProgressionSound.Play();
                    SkipSpace();
                }
            }

            if (displayedTextIsOutdated)
            {
                //displayedText = Renderer.FixText(ProgressedText, FontIndex, Width - (UIConstants.EMPTY_SPACE_SIDES * 2)).Text;
                displayedText = ProgressedText;
                displayedTextIsOutdated = false;
            }
        }

        private void SkipSpace()
        {
            if (ProgressedText.Length < ConversationText.Length)
            {
                if (ConversationText[ProgressedText.Length] == ' ')
                    ProgressedText += " ";
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (string.IsNullOrWhiteSpace(ConversationText))
                return;

            DrawPanel();

            DrawStringWithShadow(displayedText, FontIndex,
                new Vector2(UIDesignConstants.EMPTY_SPACE_SIDES + textXOffset, UIDesignConstants.EMPTY_SPACE_TOP),
                TextColor);

            DrawChildren(gameTime);
        }
    }
}
