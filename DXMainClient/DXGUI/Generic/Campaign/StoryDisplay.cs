using Microsoft.Xna.Framework;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public interface IStoryDisplay
    {
        void AddStoryImage(StoryImage storyImage);

        void AddSimpleStoryImage(string texturePath, int id, float alpha = 0.0f);

        void ClearStoryImages();

        void RemoveStoryImageById(int id);

        StoryImage FindStoryImageById(int id);

        List<StoryImage> GetAllStoryImages();

        void Finish();

        ConversationDisplay ConversationDisplay { get; }
    }

    public class StoryDisplay : XNAControl, IStoryDisplay
    {
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


        public void Begin(Cutscene cutscene)
        {
            if (cutscene == Cutscene.None)
            {
                Finish();
                return;
            }

            if (cutscenes == null)
                cutscenes = new MissionCutscenes();

            ConversationDisplay.TextColor = Color.White;
            StoryImages.ForEach(sti => sti.Kill());
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

        public void AddSimpleStoryImage(string texturePath, int id, float alpha = 0.0f)
        {
            var storyImage = new StoryImage(WindowManager, id);
            storyImage.Texture = AssetLoader.LoadTextureUncached(texturePath);
            storyImage.Alpha = alpha;
            AddStoryImage(storyImage);
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
        }
    }
}
