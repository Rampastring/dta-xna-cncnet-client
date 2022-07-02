using ClientGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class SizePositionParam
    {
        private const double SnapDifference = 1.0;

        public double Value { get; set; }
        public double TargetValue { get; set; }

        /// <summary>
        /// Determines how many distance units this parameter changes by each second.
        /// </summary>
        public double Rate { get; set; } = 100.0;

        public bool IsReady => Value == TargetValue;

        public void Update(GameTime gameTime)
        {
            if (Value != TargetValue)
            {
                double diff = TargetValue - Value;
                Value += Rate * gameTime.ElapsedGameTime.TotalSeconds * (diff > 0 ? 1.0 : -1.0);

                if (diff > 0 && Value >= TargetValue)
                    Value = TargetValue;
                else if (diff < 0 && Value <= TargetValue)
                    Value = TargetValue;
            }
        }

        public void Snap()
        {
            Value = TargetValue;
        }

        public void SnapToValue(double value)
        {
            Value = value;
            TargetValue = value;
        }
    }

    public class StoryImage : XNAControl
    {
        public StoryImage(WindowManager windowManager, int id) : base(windowManager)
        {
            InputEnabled = false;
            ID = id;
            Width = UIDesignConstants.CUTSCENE_DESIGN_RES_X;
            Height = UIDesignConstants.CUTSCENE_DESIGN_RES_Y;
        }

        public event EventHandler SizingReady;

        public int ID { get; }

        private Texture2D _texture;
        public Texture2D Texture 
        {
            get => _texture;
            set
            {
                _texture = value;
                ImageWidth.TargetValue = _texture.Width;
                ImageWidth.Value = _texture.Width;
                ImageHeight.TargetValue = _texture.Height;
                ImageHeight.Value = _texture.Height;
                ImageX.TargetValue = Width / 2 - Texture.Width / 2;
                ImageX.Value = ImageX.TargetValue;
                ImageY.TargetValue = Height / 2 - Texture.Height / 2;
                ImageY.Value = ImageY.TargetValue;
            }
        }

        public bool DrawBorders { get; set; }

        public Color BorderColor { get; set; } = Color.LimeGreen;

        public SizePositionParam ImageX = new SizePositionParam();
        public SizePositionParam ImageY = new SizePositionParam();
        public SizePositionParam ImageWidth = new SizePositionParam();
        public SizePositionParam ImageHeight = new SizePositionParam();

        private bool IsMovementReady() => ImageX.IsReady && ImageY.IsReady && ImageWidth.IsReady && ImageHeight.IsReady;

        private bool IsTransparencyReady() => (Alpha >= 1.0f && AlphaRate >= 0.0f) || (Alpha <= 0.0f && AlphaRate <= 0.0f);

        public bool IsReady
        {
            get => IsMovementReady() && IsTransparencyReady();
        }

        public float AlphaRate = 1.0f;

        public override void Update(GameTime gameTime)
        {
            if (!IsMovementReady())
            {
                ImageX.Update(gameTime);
                ImageY.Update(gameTime);
                ImageWidth.Update(gameTime);
                ImageHeight.Update(gameTime);

                if (IsMovementReady())
                {
                    SizingReady?.Invoke(this, EventArgs.Empty);
                }
            }

            Alpha += (float)(AlphaRate * gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        public void Snap()
        {
            if (IsReady)
                return;

            if (AlphaRate > 0.0f && Alpha < 1.0f)
                Alpha = 1.0f;
            else if (AlphaRate < 0.0f && Alpha > 0.0f)
                Alpha = 0.0f;

            ImageX.Snap();
            ImageY.Snap();
            ImageWidth.Snap();
            ImageHeight.Snap();
        }

        public override void Draw(GameTime gameTime)
        {
            if (Texture != null)
            {
                Vector2 scale = new Vector2((float)(ImageWidth.Value / Texture.Width), (float)(ImageHeight.Value / Texture.Height));
                DrawTexture(Texture, new Vector2((float)ImageX.Value, (float)ImageY.Value), 0f, Vector2.Zero, scale, Color.White * Alpha);
            }

            DrawChildren(gameTime);

            if (DrawBorders)
            {
                Rectangle rect = new Rectangle((int)ImageX.Value, (int)ImageY.Value, (int)ImageWidth.Value, (int)ImageHeight.Value);
                DrawRectangle(rect, BorderColor, 1);
            }
        }
    }
}
