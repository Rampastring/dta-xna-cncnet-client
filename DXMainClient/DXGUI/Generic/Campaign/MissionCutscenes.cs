using ClientCore;
using DTAClient.Domain.Singleplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Rampastring.XNAUI;
using System;
using System.Collections.Generic;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public interface ICutsceneManager
    {
        void TryPlaySong(Song song);
        void StopMusic();
        void HideAllStoryImagesWithSound(EnhancedSoundEffect sound, float alphaRate = -1.0f);
    }

    public class MissionCutscenes : ICutsceneManager
    {
        private const float CONVERSATION_MUSIC_VOLUME_MODIFIER = 0.3f;
        private const float ENDING_ALPHA_RATE = -0.25f;
        private const float FACE_ANIM_ALPHA_RATE = 2.5f;

        public MissionCutscenes()
        {
            Assets = new CutsceneResources();
        }

        private CutsceneResources Assets { get; }

        private IStoryDisplay storyDisplay;
        private WindowManager windowManager;

        public event EventHandler OnPlaySong;
        public event EventHandler OnStopMusic;

        public void TryPlaySong(Song song)
        {
            if (song == null)
                return;

            OnPlaySong?.Invoke(this, EventArgs.Empty);
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;
        }

        public void StopMusic()
        {
            OnStopMusic?.Invoke(this, EventArgs.Empty);
        }

        public void HideAllStoryImagesWithSound(EnhancedSoundEffect sound, float alphaRate = -1.0f)
        {
            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = alphaRate);

            if (sound != null)
                sound.Play();
        }

        private CovertRevoltCutscenes CreateCovertRevoltCutsceneContainer() => new CovertRevoltCutscenes(Assets, storyDisplay, this, windowManager);
        private PowerToThePeopleCutscenes CreatePowerToThePeopleCutsceneContainer() => new PowerToThePeopleCutscenes(Assets, storyDisplay, this, windowManager);
        private ShadowExodusCutscenes CreateShadowExodusCutsceneContainer() => new ShadowExodusCutscenes(Assets, storyDisplay, this, windowManager);

        public List<Phase> GetPhases(Cutscene cutscene, IStoryDisplay storyDisplay, WindowManager windowManager)
        {
            this.storyDisplay = storyDisplay;
            this.windowManager = windowManager;

            switch (cutscene)
            {
                case Cutscene.None:
                    return null;
                case Cutscene.CR01:
                    return CreateCovertRevoltCutsceneContainer().CR01();
                case Cutscene.CR01Victory:
                    return CreateCovertRevoltCutsceneContainer().CR01Victory();
                case Cutscene.CR02:
                    return CreateCovertRevoltCutsceneContainer().CR02();
                case Cutscene.CR02Victory:
                    return CreateCovertRevoltCutsceneContainer().CR02Victory();
                case Cutscene.CR03:
                    return CreateCovertRevoltCutsceneContainer().CR03();
                case Cutscene.CR04:
                    return CreateCovertRevoltCutsceneContainer().CR04();
                case Cutscene.CR05:
                    return CreateCovertRevoltCutsceneContainer().CR05();
                case Cutscene.CR05Victory:
                    return CreateCovertRevoltCutsceneContainer().CR05Victory();
                case Cutscene.CR06:
                    return CreateCovertRevoltCutsceneContainer().CR06();
                case Cutscene.CR07:
                    return CreateCovertRevoltCutsceneContainer().CR07();
                case Cutscene.CR07Victory:
                    return CreateCovertRevoltCutsceneContainer().CR07Victory();
                case Cutscene.CR08:
                    return CreateCovertRevoltCutsceneContainer().CR08();
                case Cutscene.CR08Victory:
                    return CreateCovertRevoltCutsceneContainer().CR08Victory();

                case Cutscene.CRA09:
                    return CreateCovertRevoltCutsceneContainer().CRA09();
                case Cutscene.CRA09Victory:
                    return CreateCovertRevoltCutsceneContainer().CRA09Victory();
                case Cutscene.CRA10:
                    return CreateCovertRevoltCutsceneContainer().CRA10();
                case Cutscene.CRA11:
                    return CreateCovertRevoltCutsceneContainer().CRA11();
                case Cutscene.CRA11Victory:
                    return CreateCovertRevoltCutsceneContainer().CRA11Victory();
                case Cutscene.CRA12:
                    return CreateCovertRevoltCutsceneContainer().CRA12();
                case Cutscene.CRA12Victory:
                    return CreateCovertRevoltCutsceneContainer().CRA12Victory();
                case Cutscene.CRA13:
                    return CreateCovertRevoltCutsceneContainer().CRA13();
                case Cutscene.CRA13Victory:
                    return CreateCovertRevoltCutsceneContainer().CRA13Victory();
                case Cutscene.CRA14:
                    return CreateCovertRevoltCutsceneContainer().CRA14();
                case Cutscene.CRENDA:
                    return CreateCovertRevoltCutsceneContainer().CRA14Victory();

                case Cutscene.CRB09:
                    return CreateCovertRevoltCutsceneContainer().CRB09();
                case Cutscene.CRB09Victory:
                    return CreateCovertRevoltCutsceneContainer().CRB09Victory();
                case Cutscene.CRB10:
                    return CreateCovertRevoltCutsceneContainer().CRB10();
                case Cutscene.CRB10Victory:
                    return CreateCovertRevoltCutsceneContainer().CRB10Victory();
                case Cutscene.CRB11:
                    return CreateCovertRevoltCutsceneContainer().CRB11();
                case Cutscene.CRB11Victory:
                    return CreateCovertRevoltCutsceneContainer().CRB11Victory();
                case Cutscene.CRB12:
                    return CreateCovertRevoltCutsceneContainer().CRB12();
                case Cutscene.CRB13:
                    return CreateCovertRevoltCutsceneContainer().CRB13();
                case Cutscene.CRB13Victory:
                    return CreateCovertRevoltCutsceneContainer().CRB13Victory();
                case Cutscene.CRB14:
                    return CreateCovertRevoltCutsceneContainer().CRB14();
                case Cutscene.CRB14Victory:
                    return CreateCovertRevoltCutsceneContainer().CRB14Victory();
                case Cutscene.CRB15:
                    return CreateCovertRevoltCutsceneContainer().CRB15();
                case Cutscene.CRB16:
                    return CreateCovertRevoltCutsceneContainer().CRB16();
                case Cutscene.CRB16Victory:
                    return CreateCovertRevoltCutsceneContainer().CRB16Victory();
                case Cutscene.CRB17:
                    return CreateCovertRevoltCutsceneContainer().CRB17();
                case Cutscene.CRENDB:
                    return CreateCovertRevoltCutsceneContainer().CRB17Victory();

                case Cutscene.CRC09:
                    return CreateCovertRevoltCutsceneContainer().CRC09();
                case Cutscene.CRC10:
                    return CreateCovertRevoltCutsceneContainer().CRC10();
                case Cutscene.CRC11:
                    return CreateCovertRevoltCutsceneContainer().CRC11();
                case Cutscene.CRC12:
                    return CreateCovertRevoltCutsceneContainer().CRC12();
                case Cutscene.CRC13:
                    return CreateCovertRevoltCutsceneContainer().CRC13();
                case Cutscene.CRC14:
                    return CreateCovertRevoltCutsceneContainer().CRC14();
                case Cutscene.CRC15:
                    return CreateCovertRevoltCutsceneContainer().CRC15();
                case Cutscene.CRC15Victory:
                    return CreateCovertRevoltCutsceneContainer().CRC15Victory();
                case Cutscene.CRC16:
                    return CreateCovertRevoltCutsceneContainer().CRC16();
                case Cutscene.CRC16Victory:
                    return CreateCovertRevoltCutsceneContainer().CRC16Victory();

                case Cutscene.CREXT:
                    return CreateCovertRevoltCutsceneContainer().CRExtraMission();
                case Cutscene.CRENDEXT:
                    return CreateCovertRevoltCutsceneContainer().CRExtraVictory();

                /* PTTP */
                case Cutscene.PTTP1:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP1();
                case Cutscene.PTTP2:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP2();
                case Cutscene.PTTP3:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP3();
                case Cutscene.PTTP4:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP4();
                case Cutscene.PTTP5:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP5();
                case Cutscene.PTTP6:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP6();
                case Cutscene.PTTP7:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP7();
                case Cutscene.PTTP8:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP8();
                case Cutscene.PTTP8Victory:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP8Victory();
                case Cutscene.PTTP9:
                    return CreatePowerToThePeopleCutsceneContainer().PTTP9();
                case Cutscene.PTTPEnd:
                    return CreatePowerToThePeopleCutsceneContainer().PTTPEnd();

                /* SE */
                case Cutscene.SE01:
                    return CreateShadowExodusCutsceneContainer().SE01();
                case Cutscene.SE02:
                    return CreateShadowExodusCutsceneContainer().SE02();
                case Cutscene.SE02End:
                    return CreateShadowExodusCutsceneContainer().SE02End();
                case Cutscene.SE03:
                    return CreateShadowExodusCutsceneContainer().SE03();
                case Cutscene.SE04:
                    return CreateShadowExodusCutsceneContainer().SE04();
                case Cutscene.SE05:
                    return CreateShadowExodusCutsceneContainer().SE05();
                case Cutscene.SE05End:
                    return CreateShadowExodusCutsceneContainer().SE05End();
                case Cutscene.SE06:
                    return CreateShadowExodusCutsceneContainer().SE06();
                case Cutscene.SE07:
                    return CreateShadowExodusCutsceneContainer().SE07();
                case Cutscene.SE08:
                    return CreateShadowExodusCutsceneContainer().SE08();
                case Cutscene.SE08End:
                    return CreateShadowExodusCutsceneContainer().SE08End();
                case Cutscene.SE09:
                    return CreateShadowExodusCutsceneContainer().SE09();
                case Cutscene.SE09End:
                    return CreateShadowExodusCutsceneContainer().SE09End();
                case Cutscene.SE10:
                    return CreateShadowExodusCutsceneContainer().SE10();
                case Cutscene.SE10End:
                    return CreateShadowExodusCutsceneContainer().SE10End();
                case Cutscene.SE11:
                    return CreateShadowExodusCutsceneContainer().SE11();
                case Cutscene.SE11End:
                    return CreateShadowExodusCutsceneContainer().SE11End();
            }

            return null;
        }
    }
}
