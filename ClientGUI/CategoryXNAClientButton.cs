using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;

namespace ClientGUI
{
    public class CategoryXNAClientButton : XNAClientButton
    {
        public CategoryXNAClientButton(WindowManager windowManager) : base(windowManager)
        {
        }

        private CategoryIconDisplay categoryIcon;

        public void InitCategoryIcon(string imagePath)
        {
            if (categoryIcon != null)
                throw new InvalidOperationException("The star display is already initialized!");

            categoryIcon = new CategoryIconDisplay(WindowManager, imagePath);
            categoryIcon.InputEnabled = false;
            AddChild(categoryIcon);
            ClientRectangleUpdated += (e, sender) => UpdateIconPosition();
            UpdateIconPosition();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override string Text
        {
            get => base.Text;
            set { base.Text = value; UpdateIconPosition(); }
        }

        private void UpdateIconPosition()
        {
            if (categoryIcon == null)
                return;

            categoryIcon.Y = (Height - categoryIcon.Height) / 2;
            categoryIcon.X = 15 - categoryIcon.Width / 2; // centers images with different widths
        }

        public override void Draw(GameTime gameTime)
        {
            TextXPosition = 30; // hack to force the text to be left-aligned rather than centered
            base.Draw(gameTime);
        }
    }

    class CategoryIconDisplay : XNAControl
    {
        public CategoryIconDisplay(WindowManager windowManager, string imagePath) : base(windowManager)
        {
            Name = nameof(CategoryIconDisplay);
            texture = AssetLoader.LoadTexture(imagePath);
            Width = texture.Width;
            Height = texture.Height;
        }

        private readonly Texture2D texture;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            DrawTexture(texture, Point.Zero, Color.White);
            base.Draw(gameTime);
        }
    }
}
