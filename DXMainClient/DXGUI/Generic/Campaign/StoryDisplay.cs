using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.Data;

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
        private const float InitialSkipTextVisibilitySeconds = 5.0f;
        private const float InitialSkipGraphicAlpha = SkipGraphicAlphaRate * InitialSkipTextVisibilitySeconds;

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
        private bool cutsceneStarted = false;

        private bool isDebriefing;

        private float escPressMilliseconds;
        private float skipGraphicAlpha = 0.0f;
        private float skipGraphicTrailingTime = 0.0f;

        private bool isFadingOutMusic;


        public void Begin(Cutscene cutscene, bool isDebriefing)
        {
            this.isDebriefing = isDebriefing;

            alphaRate = 2.0f;
            Alpha = 0.0f;

            if (cutscene == Cutscene.None)
            {
                Finish();
                return;
            }

            if (cutscenes == null)
            {
                cutscenes = new MissionCutscenes();
                cutscenes.OnPlaySong += Cutscenes_OnPlaySong;
                cutscenes.OnStopMusic += Cutscenes_OnStopMusic;
            }

            // Set default values of ConversationDisplay
            ConversationDisplay.TextColor = Color.White;
            ClearStoryImages();
            ConversationDisplay.ConversationText = string.Empty;
            ConversationDisplay.IsCentered = false;
            cutsceneStarted = false;
            isFadingOutMusic = false;

            skipGraphicAlpha = InitialSkipGraphicAlpha;
            escPressMilliseconds = 0f;
            skipGraphicTrailingTime = 0.0f;

            Phases = cutscenes.GetPhases(cutscene, this, WindowManager);
            Phase = -1;

            Keyboard.OnKeyPressed += Keyboard_OnKeyPressed;

            Enable();
        }

        private void Cutscenes_OnPlaySong(object sender, EventArgs e)
        {
            isFadingOutMusic = false;
        }

        private void Cutscenes_OnStopMusic(object sender, EventArgs e)
        {
            isFadingOutMusic = true;
        }

        private void Keyboard_OnKeyPressed(object sender, Rampastring.XNAUI.Input.KeyPressEventArgs e)
        {
            if (e.PressedKey == Keys.Space || e.PressedKey == Keys.Enter || e.PressedKey == Keys.Z)
            {
                UserInput_ProgressCutscene();
                e.Handled = true;
            }
        }

        public void AddStoryImage(StoryImage storyImage)
        {
            storyImage.DrawOrder = storyImage.ID;
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
            cutsceneStarted = false;
            alphaRate = -2.0f;

            if (!isDebriefing)
            {
                DoFinish();
            }
            else
            {
                if (Alpha <= 0f)
                {
                    DoFinish();
                }
            }
        }

        private void DoFinish()
        {
            ClearStoryImages();
            Disable();
            Keyboard.OnKeyPressed -= Keyboard_OnKeyPressed;
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
            // without +1 it doesn't fill the entire screen due to rounding
            ConversationDisplay.Width = (Width / ConversationDisplay.Scaling) + 1;
            ConversationDisplay.Height = 65;
            ConversationDisplay.Y = Height - ConversationDisplay.ScaledHeight;
            ConversationDisplay.DrawOrder = 99999;
            ConversationDisplay.UpdateOrder = 99999;
            AddChild(ConversationDisplay);

            base.Initialize();
            Disable();
        }

        public override void Update(GameTime gameTime)
        {
            CutsceneSystemUpdate(gameTime);

            Alpha += (float)(gameTime.ElapsedGameTime.TotalSeconds * alphaRate);

            // Wait for us to be fully visible before starting the first phase
            if (!cutsceneStarted && Alpha >= 1.0f)
            {
                cutsceneStarted = true;
                NextPhase();
            }

            // Fade back to menu after debriefing
            if (alphaRate < 0f)
            {
                if (Alpha <= 0f)
                {
                    DoFinish();
                }
            }

            if (isFadingOutMusic)
            {
                const float musicFadeRate = 1.0f;
                MediaPlayer.Volume -= (float)(musicFadeRate * gameTime.ElapsedGameTime.TotalSeconds);
                if (MediaPlayer.Volume == 0f)
                    MediaPlayer.Stop();
            }

            base.Update(gameTime);
        }

        private void CutsceneSystemUpdate(GameTime gameTime)
        {
            if (cutsceneStarted)
            {
                // Handle phase transition logic
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

                bool escapeDown = Keyboard.IsKeyHeldDown(Keys.Escape);

                // Update skip graphic state. If the user is currently pressing ESC or has pressed it within the last few seconds,
                // we should display the skip hint graphic.
                // Otherwise, fade the skip hint graphic away.
                if (skipGraphicTrailingTime > 0 || escapeDown)
                {
                    skipGraphicAlpha = (float)Math.Min(1.0f, skipGraphicAlpha + SkipGraphicAlphaRate * gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    skipGraphicAlpha = (float)Math.Max(0f, skipGraphicAlpha - SkipGraphicAlphaRate * gameTime.ElapsedGameTime.TotalSeconds);
                }

                // Check if ESC has been held for long enough for the cutscene to be skipped.
                if (escapeDown)
                {
                    escPressMilliseconds += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    skipGraphicTrailingTime = InitialSkipTextVisibilitySeconds;

                    if (escPressMilliseconds > SkipTime)
                    {
                        Finish();
                    }
                }
                else
                {
                    if (skipGraphicTrailingTime > 0f)
                    {
                        skipGraphicTrailingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    escPressMilliseconds = 0.0f;
                }
            }
        }

        public override void OnLeftClick()
        {
            UserInput_ProgressCutscene();

            base.OnLeftClick();
        }

        private void UserInput_ProgressCutscene()
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
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            DrawChildren(gameTime);

            if (skipGraphicAlpha > 0f)
            {
                string text = "<ESC> hold to skip";
                const int fontIndex = 1;
                var textWidth = Renderer.GetTextDimensions(text, fontIndex).X;
                var textLocation = new Vector2(Width - textWidth - 10.0f, 6.0f);
                DrawString(text, fontIndex, textLocation + new Vector2(1, 1), Color.Black * skipGraphicAlpha);
                DrawString(text, fontIndex, new Vector2(Width - textWidth - 10.0f, 6.0f), ConversationDisplay.TextColor * skipGraphicAlpha);

                float escBarMaxWidth = textWidth;
                float escBarHeight = 20.0f;
                float escBarX = Width - escBarMaxWidth - 10;
                float escBarY = 30;

                float escBarWidth = (escPressMilliseconds / SkipTime) * escBarMaxWidth;

                if (escBarWidth > 0f)
                {
                    FillRectangle(new Rectangle((int)escBarX, (int)escBarY, (int)escBarWidth, (int)escBarHeight), ConversationDisplay.TextColor * 0.5f * skipGraphicAlpha);
                    DrawRectangle(new Rectangle((int)escBarX, (int)escBarY, (int)escBarMaxWidth, (int)escBarHeight), Color.White * skipGraphicAlpha);
                }
            }
        }
    }
}
