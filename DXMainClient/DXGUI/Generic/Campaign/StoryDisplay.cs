using ClientGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public interface IStoryDisplay
    {
        void AddStoryImage(StoryImage storyImage);

        StoryImage AddSimpleStoryImage(string texturePath, int id, float alpha = 0.0f);

        void ClearStoryImages();

        void RemoveStoryImageById(int id);

        StoryImage FindStoryImageById(int id);

        List<StoryImage> GetAllStoryImages();

        void Finish();

        ConversationDisplay ConversationDisplay { get; }
    }

    public class StoryDisplay : XNAControl, IStoryDisplay
    {
        /// <summary>
        /// Milliseconds that the user must hold ESC for before the cutscene is skipped.
        /// </summary>
        private const float SkipTime = 1500.0f;
        private const float SkipGraphicAlphaRate = 5.0f;

        public StoryDisplay(WindowManager windowManager) : base(windowManager)
        {
            DrawMode = ControlDrawMode.UNIQUE_RENDER_TARGET;
        }

        public event EventHandler Finished;

        public List<StoryImage> StoryImages = new List<StoryImage>();

        public int Phase;
        public List<Phase> Phases = new List<Phase>();
        public Phase CurrentPhase;

        public PhaseState PhaseState;

        public ConversationDisplay ConversationDisplay { get; private set; }


        private MissionCutscenes cutscenes;

        private float alphaRate = 0.0f;

        private bool isDebriefing;

        private float escPressMilliseconds;
        private float skipGraphicAlpha = 0.0f;

        public void Begin(Cutscene cutscene, bool isDebriefing)
        {
            this.isDebriefing = isDebriefing;

            if (cutscene == Cutscene.None)
            {
                Finish();
                return;
            }

            alphaRate = 0.0f;
            Alpha = 1.0f;

            if (cutscenes == null)
                cutscenes = new MissionCutscenes();

            // Set default values of ConversationDisplay
            ConversationDisplay.TextColor = Color.White;
            ClearStoryImages();
            ConversationDisplay.ConversationText = string.Empty;
            ConversationDisplay.IsCentered = false;

            Phases = cutscenes.GetPhases(cutscene, this, WindowManager);
            Phase = -1;
            NextPhase();
            Enable();
        }

        public void AddStoryImage(StoryImage storyImage)
        {
            StoryImages.Add(storyImage);
            AddChild(storyImage);
        }

        public StoryImage AddSimpleStoryImage(string texturePath, int id, float alpha = 0.0f)
        {
            var storyImage = new StoryImage(WindowManager, id);
            storyImage.Texture = AssetLoader.LoadTextureUncached(texturePath);
            storyImage.Alpha = alpha;
            AddStoryImage(storyImage);
            return storyImage;
        }

        public void ClearStoryImages()
        {
            StoryImages.ForEach(sti => sti.Kill());
            StoryImages.ForEach(sti => RemoveChild(sti));
            StoryImages.ForEach(sti => sti.Texture.Dispose());
            StoryImages.Clear();
        }

        private void RemoveStoryImage(StoryImage storyImage)
        {
            if (storyImage == null)
                return;

            storyImage.Kill();
            RemoveChild(storyImage);
            storyImage.Texture.Dispose();
            StoryImages.Remove(storyImage);
        }

        public void RemoveStoryImageById(int id)
        {
            RemoveStoryImage(FindStoryImageById(id));
        }

        public StoryImage FindStoryImageById(int id)
        {
            return StoryImages.Find(si => si.ID == id);
        }

        public List<StoryImage> GetAllStoryImages() => new List<StoryImage>(StoryImages);

        public void NextPhase()
        {
            Phase++;
            if (Phase >= Phases.Count)
            {
                Finish();
                return;
            }

            PhaseState = PhaseState.Appearing;
            CurrentPhase = Phases[Phase];
            CurrentPhase.Enter?.Invoke(this);
        }

        public void Finish()
        {
            Disable();
            Finished?.Invoke(this, EventArgs.Empty);
        }

        public override void Initialize()
        {
            Name = nameof(StoryDisplay);
            Width = WindowManager.RenderResolutionX;
            Height = WindowManager.RenderResolutionY;

            ConversationDisplay = new ConversationDisplay(WindowManager);
            ConversationDisplay.Name = nameof(ConversationDisplay);
            ConversationDisplay.BackgroundTexture = AssetLoader.CreateTexture(Color.Black * 0.5f, 2, 2);
            ConversationDisplay.PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            ConversationDisplay.Scaling = 3;
            ConversationDisplay.Width = Width / ConversationDisplay.Scaling;
            ConversationDisplay.Height = 65;
            ConversationDisplay.Y = Height - ConversationDisplay.ScaledHeight;
            ConversationDisplay.DrawOrder = 1;
            ConversationDisplay.UpdateOrder = 1;
            AddChild(ConversationDisplay);

            base.Initialize();
            Disable();
        }

        public override void Update(GameTime gameTime)
        {
            if (PhaseState == PhaseState.Appearing)
            {
                if (ConversationDisplay.IsReady() && StoryImages.TrueForAll(si => si.IsReady))
                {
                    PhaseState = PhaseState.Ready;
                    CurrentPhase.Ready?.Invoke(this);
                }
            }
            else if (PhaseState == PhaseState.Disappearing)
            {
                if (ConversationDisplay.IsReady() && StoryImages.TrueForAll(si => si.IsReady))
                {
                    PhaseState = PhaseState.Disappeared;
                    CurrentPhase.Left?.Invoke(this);
                }
            }
            else if (PhaseState == PhaseState.Disappeared)
            {
                NextPhase();
            }

            if (Keyboard.IsKeyHeldDown(Keys.Escape))
            {
                escPressMilliseconds += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                skipGraphicAlpha = (float)Math.Min(1.0f, skipGraphicAlpha + SkipGraphicAlphaRate * gameTime.ElapsedGameTime.TotalSeconds);

                if (escPressMilliseconds > SkipTime)
                {
                    Finish();
                }
            }
            else
            {
                escPressMilliseconds = 0.0f;
                skipGraphicAlpha = (float)Math.Max(0f, skipGraphicAlpha - SkipGraphicAlphaRate * gameTime.ElapsedGameTime.TotalSeconds);
            }

            Alpha += (float)(gameTime.ElapsedGameTime.TotalSeconds * alphaRate);

            base.Update(gameTime);
        }

        public override void OnLeftClick()
        {
            if (PhaseState == PhaseState.Appearing)
            {
                if (!ConversationDisplay.IsReady())
                {
                    ConversationDisplay.Snap();
                }

                foreach (StoryImage storyImage in StoryImages)
                {
                    if (!storyImage.IsReady)
                    {
                        storyImage.Snap();
                    }
                }

                PhaseState = PhaseState.Ready;
                CurrentPhase.Ready?.Invoke(this);
            }
            else if (PhaseState == PhaseState.Disappearing)
            {
                if (!ConversationDisplay.IsReady())
                {
                    ConversationDisplay.Snap();
                }

                foreach (StoryImage storyImage in StoryImages)
                {
                    if (!storyImage.IsReady)
                    {
                        storyImage.Snap();
                    }
                }

                PhaseState = PhaseState.Disappeared;
                CurrentPhase.Left?.Invoke(this);
            }
            else if (PhaseState == PhaseState.Ready)
            {
                PhaseState = PhaseState.Disappearing;
                if (isDebriefing && Phase == Phases.Count - 1)
                    alphaRate = -1.0f;

                CurrentPhase.Leave?.Invoke(this);
            }
            else if (PhaseState == PhaseState.Disappeared)
            {
                NextPhase();
            }

            base.OnLeftClick();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            DrawChildren(gameTime);

            if (skipGraphicAlpha > 0f)
            {
                string text = "<ESC> hold to skip";
                var textWidth = Renderer.GetTextDimensions(text, 0).X;
                var textLocation = new Vector2(Width - textWidth - 10.0f, 6.0f);
                DrawString(text, 0, textLocation + new Vector2(1, 1), Color.Black * skipGraphicAlpha);
                DrawString(text, 0, new Vector2(Width - textWidth - 10.0f, 6.0f), ConversationDisplay.TextColor * skipGraphicAlpha);

                float escBarMaxWidth = 100.0f;
                float escBarHeight = 20.0f;
                float escBarX = Width - escBarMaxWidth - 10;
                float escBarY = 30;

                float escBarWidth = (escPressMilliseconds / SkipTime) * escBarMaxWidth;

                FillRectangle(new Rectangle((int)escBarX, (int)escBarY, (int)escBarWidth, (int)escBarHeight), ConversationDisplay.TextColor * 0.5f * skipGraphicAlpha);
                DrawRectangle(new Rectangle((int)escBarX, (int)escBarY, (int)escBarMaxWidth, (int)escBarHeight), Color.White * skipGraphicAlpha);
            }
        }
    }
}
