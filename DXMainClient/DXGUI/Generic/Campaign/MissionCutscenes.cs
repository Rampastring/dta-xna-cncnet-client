using ClientCore;
using ClientGUI;
using DTAClient.Domain.Singleplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Rampastring.XNAUI;
using System;
using System.Collections.Generic;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class MissionCutscenes
    {
        private const float CONVERSATION_MUSIC_VOLUME_MODIFIER = 0.3f;
        private const float ENDING_ALPHA_RATE = -0.25f;
        private const float FACE_ANIM_ALPHA_RATE = 2.5f;

        public MissionCutscenes()
        {
            beepy2 = new EnhancedSoundEffect("Story/Sounds/BEEPY2.WAV");
            beepy3 = new EnhancedSoundEffect("Story/Sounds/BEEPY3.WAV");
            bleep9 = new EnhancedSoundEffect("Story/Sounds/BLEEP9.WAV");
            bleep11 = new EnhancedSoundEffect("Story/Sounds/BLEEP11.WAV");
            bleep12 = new EnhancedSoundEffect("Story/Sounds/BLEEP12.WAV");
            bleep17 = new EnhancedSoundEffect("Story/Sounds/BLEEP17.WAV");
            country1 = new EnhancedSoundEffect("Story/Sounds/COUNTRY1.WAV");
            country4 = new EnhancedSoundEffect("Story/Sounds/COUNTRY4.WAV");
            tone5 = new EnhancedSoundEffect("Story/Sounds/TONE5.WAV");
            toney4 = new EnhancedSoundEffect("Story/Sounds/TONEY4.WAV");
            toney7 = new EnhancedSoundEffect("Story/Sounds/TONEY7.WAV");
            mapwipe2 = new EnhancedSoundEffect("Story/Sounds/MAPWIPE2.WAV");
            mapwipe5 = new EnhancedSoundEffect("Story/Sounds/MAPWIPE5.WAV");
            newtarg1 = new EnhancedSoundEffect("Story/Sounds/NEWTARG1.WAV");
            powrdn1 = new EnhancedSoundEffect("Story/Sounds/POWRDN1.WAV");
            world2 = new EnhancedSoundEffect("Story/Sounds/WORLD2.WAV");

            ramap = AssetLoader.LoadSong("Story/Music/ramap");
            raintro = AssetLoader.LoadSong("Story/Music/raintro");
            fac2226m = AssetLoader.LoadSong("Story/Music/fac2226m");
            secondhand = AssetLoader.LoadSong("Story/Music/2nd_hand");
            terminat = AssetLoader.LoadSong("Story/Music/terminat");
            chrg226m = AssetLoader.LoadSong("Story/Music/chrg226m");
            vector1a = AssetLoader.LoadSong("Story/Music/vector1a");
            hellmarch = AssetLoader.LoadSong("Story/Music/hellmarch");
            tdmaptheme = AssetLoader.LoadSong("Story/Music/credits");
            stopthem = AssetLoader.LoadSong("Story/Music/stopthem");
            gloom = AssetLoader.LoadSong("Story/Music/gloom");
        }

        private readonly EnhancedSoundEffect beepy2;
        private readonly EnhancedSoundEffect beepy3;
        private readonly EnhancedSoundEffect bleep9;
        private readonly EnhancedSoundEffect bleep11;
        private readonly EnhancedSoundEffect bleep12;
        private readonly EnhancedSoundEffect bleep17;
        private readonly EnhancedSoundEffect country1;
        private readonly EnhancedSoundEffect country4;
        private readonly EnhancedSoundEffect tone5;
        private readonly EnhancedSoundEffect toney4;
        private readonly EnhancedSoundEffect toney7;
        private readonly EnhancedSoundEffect mapwipe2;
        private readonly EnhancedSoundEffect mapwipe5;
        private readonly EnhancedSoundEffect newtarg1;
        private readonly EnhancedSoundEffect powrdn1;
        private readonly EnhancedSoundEffect world2;

        private readonly Song ramap;
        private readonly Song raintro;
        private readonly Song fac2226m;
        private readonly Song secondhand;
        private readonly Song terminat;
        private readonly Song chrg226m;
        private readonly Song vector1a;
        private readonly Song hellmarch;
        private readonly Song tdmaptheme;
        private readonly Song stopthem;
        private readonly Song gloom;

        private const double crRAdisplayX = 4.0;
        private const double crRAdisplayY = 20.0;
        private const double crRAdisplayMiddle = 234.0;
        private const double crRAdisplayImageX = crRAdisplayX + 30.0; // the RA border thingies are 30 pixels wide at left and right
        private const double crRAdisplayImageRate = 300.0;
        private const double crRAdisplayImageWidth = 400.0;
        private const double crRAdisplayImageY = crRAdisplayY + 16.0; // the RA border thingies are 16 pixels high at the top
        private const double crTDdisplayImageY = crRAdisplayY + 19.0; // the TD bordier thingies are 19 pixels high at the top
        private const float crRAdisplayAlphaRate = 5.0f;
        private const float crTDdisplayImageDisappearAlphaRate = -2.5f;

        private IStoryDisplay storyDisplay;
        private WindowManager windowManager;

        public event EventHandler OnPlaySong;
        public event EventHandler OnStopMusic;

        private void TryPlaySong(Song song)
        {
            if (song == null)
                return;

            OnPlaySong?.Invoke(this, EventArgs.Empty);
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;
        }

        private void StopMusic()
        {
            OnStopMusic?.Invoke(this, EventArgs.Empty);
        }

        public List<Phase> GetPhases(Cutscene cutscene, IStoryDisplay storyDisplay, WindowManager windowManager)
        {
            this.storyDisplay = storyDisplay;
            this.windowManager = windowManager;

            switch (cutscene)
            {
                case Cutscene.None:
                    return null;
                case Cutscene.CR01:
                    return CR01();
                case Cutscene.CR01Victory:
                    return CR01Victory();
                case Cutscene.CR02:
                    return CR02();
                case Cutscene.CR03:
                    return CR03();
                case Cutscene.CR04:
                    return CR04();
                case Cutscene.CR05:
                    return CR05();
                case Cutscene.CR05Victory:
                    return CR05Victory();
                case Cutscene.CR06:
                    return CR06();
                case Cutscene.CR07:
                    return CR07();
                case Cutscene.CR07Victory:
                    return CR07Victory();
                case Cutscene.CR08:
                    return CR08();
                case Cutscene.CR08Victory:
                    return CR08Victory();

                case Cutscene.CRA09:
                    return CRA09();
                case Cutscene.CRA09Victory:
                    return CRA09Victory();
                case Cutscene.CRA10:
                    return CRA10();
                case Cutscene.CRA11:
                    return CRA11();
                case Cutscene.CRA11Victory:
                    return CRA11Victory();
                case Cutscene.CRA12:
                    return CRA12();
                case Cutscene.CRA12Victory:
                    return CRA12Victory();
                case Cutscene.CRA13:
                    return CRA13();
                case Cutscene.CRA13Victory:
                    return CRA13Victory();
                case Cutscene.CRA14:
                    return CRA14();
                case Cutscene.CRENDA:
                    return CRA14Victory();

                case Cutscene.CRB09:
                    return CRB09();
                case Cutscene.CRB09Victory:
                    return CRB09Victory();
                case Cutscene.CRB10:
                    return CRB10();
                case Cutscene.CRB10Victory:
                    return CRB10Victory();
                case Cutscene.CRB11:
                    return CRB11();
                case Cutscene.CRB11Victory:
                    return CRB11Victory();
                case Cutscene.CRB12:
                    return CRB12();
                case Cutscene.CRB13:
                    return CRB13();
                case Cutscene.CRB13Victory:
                    return CRB13Victory();
                case Cutscene.CRB14:
                    return CRB14();
                case Cutscene.CRB14Victory:
                    return CRB14Victory();
                case Cutscene.CRB15:
                    return CRB15();
                case Cutscene.CRB16:
                    return CRB16();
                case Cutscene.CRB16Victory:
                    return CRB16Victory();
                case Cutscene.CRB17:
                    return CRB17();
                case Cutscene.CRENDB:
                    return CRB17Victory();

                case Cutscene.CRC09:
                    return CRC09();
                case Cutscene.CRC10:
                    return CRC10();
                case Cutscene.CRC11:
                    return CRC11();
                case Cutscene.CRC12:
                    return CRC12();
                case Cutscene.CRC13:
                    return CRC13();
                case Cutscene.CRC14:
                    return CRC14();
                case Cutscene.CRC15:
                    return CRC15();
                case Cutscene.CRC15Victory:
                    return CRC15Victory();

                /* PTTP */
                case Cutscene.PTTP1:
                    return PTTP1();
                case Cutscene.PTTP2:
                    return PTTP2();
                case Cutscene.PTTP3:
                    return PTTP3();
                case Cutscene.PTTP4:
                    return PTTP4();
                case Cutscene.PTTP5:
                    return PTTP5();
                case Cutscene.PTTP6:
                    return PTTP6();
                case Cutscene.PTTP7:
                    return PTTP7();
                case Cutscene.PTTP8:
                    return PTTP8();
                case Cutscene.PTTP8Victory:
                    return PTTP8Victory();
                case Cutscene.PTTP9:
                    return PTTP9();
                case Cutscene.PTTPEnd:
                    return PTTPEnd();
            }

            return null;
        }

        private void AddRADisplay(IStoryDisplay storyDisplay, int id)
        {
            var rasides = storyDisplay.AddSimpleStoryImage("Story/rasides.png", id, 0);
            rasides.AlphaRate = crRAdisplayAlphaRate;
            rasides.ImageX.SnapToValue(crRAdisplayX);
            rasides.ImageY.SnapToValue(crRAdisplayY);
            bleep9.Play();
        }

        private void AddRADisplayImage(IStoryDisplay storyDisplay, string imagePath, int id)
        {
            var image = storyDisplay.AddSimpleStoryImage(imagePath, id, 1);
            image.ImageX.Value = crRAdisplayMiddle;
            image.ImageX.TargetValue = crRAdisplayImageX;
            image.ImageX.Rate = crRAdisplayImageRate;
            image.ImageWidth.Value = 1.0;
            image.ImageWidth.TargetValue = crRAdisplayImageWidth;
            image.ImageWidth.Rate = image.ImageX.Rate * 2.0;
            image.ImageY.SnapToValue(crRAdisplayImageY);
            bleep11.Play();
        }

        private void AddTDDisplay(IStoryDisplay storyDisplay, int id)
        {
            var tdsides = storyDisplay.AddSimpleStoryImage("Story/tdsides.png", id, 0);
            tdsides.AlphaRate = crRAdisplayAlphaRate;
            tdsides.ImageX.SnapToValue(crRAdisplayX);
            tdsides.ImageY.SnapToValue(crRAdisplayY);
            world2.Play();
        }

        private void AddTDDisplayImage(IStoryDisplay storyDisplay, string imagePath, int id)
        {
            var image = storyDisplay.AddSimpleStoryImage(imagePath, id, 0);
            image.ImageY.SnapToValue(crTDdisplayImageY);
            image.ImageX.SnapToValue(crRAdisplayImageX);
            image.AlphaRate = 5.0f;
            beepy2.Play();
        }

        private void HideAllStoryImagesWithSound(IStoryDisplay storyDisplay, EnhancedSoundEffect sound, float alphaRate = -1.0f)
        {
            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = alphaRate);

            if (sound != null)
                sound.Play();
        }

        private void AddCreditPhases(List<Phase> phases, Color color)
        {
            int lastPhaseID = phases[phases.Count - 1].ID;

            phases.Add(new Phase(lastPhaseID + 1,
                storyDisplay =>
                {
                    TryPlaySong(tdmaptheme);
                    storyDisplay.ConversationDisplay.ConversationText = "";

                    var creditsStoryImage = new StoryImage(windowManager, 1);
                    creditsStoryImage.Texture = AssetLoader.LoadTextureUncached("Story/credits.png");
                    creditsStoryImage.Color = color;
                    creditsStoryImage.ImageHeight.SnapToValue(creditsStoryImage.Texture.Height);
                    creditsStoryImage.ImageY.Value = windowManager.RenderResolutionY;
                    creditsStoryImage.ImageY.TargetValue = -creditsStoryImage.Texture.Height;
                    creditsStoryImage.ImageY.Rate = 50.0;
                    storyDisplay.AddStoryImage(creditsStoryImage);

                    var creditsStoryImage2 = new StoryImage(windowManager, 2);
                    creditsStoryImage2.Texture = AssetLoader.LoadTextureUncached("Story/creditsp2.png");
                    creditsStoryImage2.Color = color;
                    creditsStoryImage2.ImageHeight.SnapToValue(creditsStoryImage2.Texture.Height);
                    creditsStoryImage2.ImageY.Value = creditsStoryImage.ImageY.Value + creditsStoryImage.ImageHeight.Value;
                    creditsStoryImage2.ImageY.TargetValue = windowManager.RenderResolutionY - creditsStoryImage2.Texture.Height;
                    creditsStoryImage2.ImageY.Rate = 50.0;
                    storyDisplay.AddStoryImage(creditsStoryImage2);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));
        }

        private void AddRouteCFakeCreditPhases(List<Phase> phases)
        {
            int lastPhaseID = phases[phases.Count - 1].ID;

            phases.Add(new Phase(lastPhaseID + 1,
                storyDisplay =>
                {
                    TryPlaySong(tdmaptheme);
                    storyDisplay.ConversationDisplay.ConversationText = "";

                    var creditsStoryImage = new StoryImage(windowManager, 1);
                    creditsStoryImage.Texture = AssetLoader.LoadTextureUncached("Story/credits.png");
                    creditsStoryImage.Color = new Color(20, 0, 0);
                    creditsStoryImage.ImageHeight.SnapToValue(creditsStoryImage.Texture.Height);
                    creditsStoryImage.ImageY.Value = windowManager.RenderResolutionY;
                    creditsStoryImage.ImageY.TargetValue = -creditsStoryImage.Texture.Height;
                    creditsStoryImage.ImageY.Rate = 2000.0;
                    storyDisplay.AddStoryImage(creditsStoryImage);

                    var creditsStoryImage2 = new StoryImage(windowManager, 2);
                    creditsStoryImage2.Texture = AssetLoader.LoadTextureUncached("Story/creditsp2.png");
                    creditsStoryImage2.Color = new Color(20, 0, 0);
                    creditsStoryImage2.ImageHeight.SnapToValue(creditsStoryImage2.Texture.Height);
                    creditsStoryImage2.ImageY.Value = creditsStoryImage.ImageY.Value + creditsStoryImage.ImageHeight.Value;
                    creditsStoryImage2.ImageY.TargetValue = windowManager.RenderResolutionY - creditsStoryImage2.Texture.Height;
                    creditsStoryImage2.ImageY.Rate = 2000.0;
                    storyDisplay.AddStoryImage(creditsStoryImage2);
                },
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages(),
                null));

            phases.Add(new Phase(lastPhaseID + 2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Red;
                    storyDisplay.ConversationDisplay.ConversationText = "Actually...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(lastPhaseID + 3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Who is Ivanov to determine your rank by this point?";
                },
                null,
                null,
                null));

            phases.Add(new Phase(lastPhaseID + 4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Your reputation in the military far outshines his own.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(lastPhaseID + 5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The current situation can be an ending, but if you wish, you can also start a more open revolt by playing CR Route C #16.";
                },
                null,
                null,
                null));
        }

        private void AddPostRouteAHintPhases(List<Phase> phases)
        {
            int lastPhaseID = phases[phases.Count - 1].ID;

            bool bRouteUnlocked = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CRB9" && m.IsUnlocked);
            bool bRouteBeat = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CRB17" && m.Rank > DifficultyRank.NONE);
            bool cRouteUnlocked = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CRC9" && m.IsUnlocked);
            bool cRouteBeat = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CRC16" && m.Rank > DifficultyRank.NONE);
            bool extraUnlocked = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CREXT" && m.IsUnlocked);
            bool extraBeat = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CREXT" && m.Rank > DifficultyRank.NONE);

            if (!bRouteUnlocked)
            {
                phases.Add(new Phase(lastPhaseID + 1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The Government didn't really treat its enemies or its own civilians in the most humane way possible.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Can you find out how to challenge them for this? To unlock the B route?";
                    },
                    null,
                    null,
                    null));

                return;
            }

            if (!bRouteBeat)
            {
                phases.Add(new Phase(lastPhaseID + 1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Route A complete! Next you can challenge route B!";
                    },
                    null,
                    null,
                    null));

                return;
            }

            if (!cRouteUnlocked)
            {
                phases.Add(new Phase(lastPhaseID + 1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You have completed both Route A and Route B.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "There is also a third route.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Can you figure out how to reach it?";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Or, you can also just be happy with your current accomplishments.";
                    },
                    null,
                    null,
                    null));

                if (!extraBeat)
                {
                    if (extraUnlocked)
                    {
                        phases.Add(new Phase(lastPhaseID + 5,
                            storyDisplay =>
                            {
                                storyDisplay.ConversationDisplay.ConversationText = "There is also the bonus mission to play.";
                            },
                            null,
                            null,
                            null));
                    }
                    else
                    {
                        phases.Add(new Phase(lastPhaseID + 5,
                            storyDisplay =>
                            {
                                storyDisplay.ConversationDisplay.ConversationText = "There would also be the bonus mission to play. But it appears you have missed the optional objective for unlocking it.";
                            },
                            null,
                            null,
                            null));
                    }
                }

                return;
            }

            if (!cRouteBeat)
            {
                phases.Add(new Phase(lastPhaseID + 1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Routes A and B complete! Next you can challenge route C...";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.TextColor = Color.Red;
                        storyDisplay.ConversationDisplay.ConversationText = "...if you can handle the pain.";
                    },
                    null,
                    null,
                    null));

                return;
            }

            phases.Add(new Phase(lastPhaseID + 1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Congratulations on beating all routes of Covert Revolt!";
                },
                null,
                null,
                null));
        }

        private void AddPostRouteBHintPhases(List<Phase> phases)
        {
            int lastPhaseID = phases[phases.Count - 1].ID;

            bool aRouteBeat = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CRA14" && m.Rank > DifficultyRank.NONE);
            bool cRouteUnlocked = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CRC9" && m.IsUnlocked);
            bool cRouteBeat = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CRC16" && m.Rank > DifficultyRank.NONE);
            bool extraUnlocked = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CREXT" && m.IsUnlocked);
            bool extraBeat = CampaignHandler.Instance.Missions.Exists(m => m.InternalName == "M_CREXT" && m.Rank > DifficultyRank.NONE);

            if (!aRouteBeat)
            {
                phases.Add(new Phase(lastPhaseID + 1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Route B complete! Next you can challenge route A, see how the story would have played out on Ivanov's side!";
                    },
                    null,
                    null,
                    null));

                return;
            }

            if (!cRouteUnlocked)
            {
                phases.Add(new Phase(lastPhaseID + 1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You have completed both Route A and Route B.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "There is also a third route.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Can you figure out how to reach it?";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Or, you can also just be happy with your current accomplishments.";
                    },
                    null,
                    null,
                    null));

                if (!extraBeat)
                {
                    if (extraUnlocked)
                    {
                        phases.Add(new Phase(lastPhaseID + 5,
                            storyDisplay =>
                            {
                                storyDisplay.ConversationDisplay.ConversationText = "There is also the bonus mission to play.";
                            },
                            null,
                            null,
                            null));
                    }
                    else
                    {
                        phases.Add(new Phase(lastPhaseID + 5,
                            storyDisplay =>
                            {
                                storyDisplay.ConversationDisplay.ConversationText = "There would also be the bonus mission to play. But it appears you have missed the optional objective for unlocking it.";
                            },
                            null,
                            null,
                            null));
                    }
                }

                return;
            }

            if (!cRouteBeat)
            {
                phases.Add(new Phase(lastPhaseID + 1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Routes A and B complete! Next you can challenge route C...";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(lastPhaseID + 2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.TextColor = Color.Red;
                        storyDisplay.ConversationDisplay.ConversationText = "...if you can handle the pain.";
                    },
                    null,
                    null,
                    null));

                return;
            }

            phases.Add(new Phase(lastPhaseID + 1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Congratulations on beating all routes of Covert Revolt!";
                },
                null,
                null,
                null));
        }

        private List<Phase> CRC15Victory()
        {
            var phases = new List<Phase>();

            var toikkaSavedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_C_TOIKKA_SAVED");
            var nodOutpostSavedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_C_NOD_OUTPOST_PRESERVED");

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Another military failure by the Global Defense Initiative has heavily undermined their operation against Ivanov.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC15/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The loss of GDI's primary logistics hub left their forces starved for supplies and vulnerable to fast Nod attack groups.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Over the coming weeks, pressure on GDI from their funding nations increased. Finally, faced with heavy losses, GDI made the decision to pull their forces away.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC15/victorybg02.png", 2);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            if (nodOutpostSavedVariable != null && !nodOutpostSavedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(1000,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "They, however, retained a strict embargo on the country.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/victorybg03.png", 3);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(1001,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Land routes on the country's borders were guarded by forces friendly to GDI, while heavy battleships made sure that no goods could move through naval routes.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(1002,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Coupled with the enormours destruction caused by the war, this embargo has led into massive shortages on everything, from food to any sort of tools or technology.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/victorybg04.png", 4);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(1003,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "This forced Ivanov to seek out help from Nod, who could smuggle enough goods to keep the population alive and Ivanov in power.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/nod.png", 5);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(1004,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Naturally, Nod did not perform this for free, but they used Government reliance on them to obtain power.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(1005,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It is expected that soon the country will be under Nod rule, pulling strings on Ivanov from the shadows.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(1006,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/bg03.png", 10, 0f).AlphaRate = 2.5f;
                        storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        country4.Play();

                        TryPlaySong(vector1a);
                        MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                    },
                    null,
                    null,
                    null));
            }
            else
            {
                phases.Add(new Phase(2000,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "GDI had at first believed that they could keep the country under a strict embargo.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/bg04.png", 3);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(2001,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "However, the sudden sinking of a heavy GDI battleship by Nod Laser Corvettes shocked the world.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(2002,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "With their naval dominance contested, GDI could execute their embargo only partially.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(2003,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Goods were frequently smuggled in from neighbouring countries.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/victorybg05.png", 4);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(2004,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "This formed enough of a stream to keep the population fed and rebuilding efforts progressing, although slowly.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(2005,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The destruction caused by the war along with the partial GDI-enforced embargo made Ivanov's Government the clearly weaker party in their alliance with Nod.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/victorybg04.png", 5);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(2006,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Despite heavy losses, Nod likely still sees local value in the Government's military after their very efficient performance against much stronger GDI units.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(2007,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It is expected that Nod eventually wants the country's military to be integrated into the Nod chain of command, with Ivanov remaining as a civilian leader only.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(2008,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 10, 0f).AlphaRate = 2.5f;
                        storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        country4.Play();

                        TryPlaySong(vector1a);
                        MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 11, 0f);
                    country1.Play();
                },
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(10),
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You destroyed our relationship with GDI.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But eventually you also brought us victory, and a country entirely free of traitorous elements.";
                },
                null,
                null,
                null));

            if (toikkaSavedVariable != null && toikkaSavedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "So far, you have been a Commander on the same level with Toikka.";
                    },
                    null,
                    null,
                    null));
            }
            else
            {
                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "So far, you have been a Commander on the same level with others.";
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But now I have decided to officially promote you to the rank of Supreme Commander of our Armed Forces.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Congratulations.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I am not sure how long-lasting this period of peace is going to be, especially as Nod appears to be losing the First Tiberium War.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Maybe you should lend them a hand, sneak yourself to the Balkans and help them turn the tide?";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Hopefully your performance has made GDI too afraid to attack us directly again even if Nod was no longer around.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Time will tell.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "";
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            string endingTitle;
            if (nodOutpostSavedVariable != null && nodOutpostSavedVariable.EnabledThroughPreviousScenario)
            {
                endingTitle = "           ENDING IIX" + Environment.NewLine + 
                              "   The Supreme Commander";
            }
            else
            {
                endingTitle = "           ENDING VII" + Environment.NewLine +
                              " Subordinate to the Brotherhood";
            }

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.AddSimpleStoryImage("Story/CRC09/crtitle_bloodred.png", 12);
                    storyDisplay.ConversationDisplay.ConversationText = endingTitle;
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "";
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                    StopMusic();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            AddRouteCFakeCreditPhases(phases);

            return phases;
        }

        private List<Phase> CRC15()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRC15/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(ramap);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            var toikkaSavedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_C_TOIKKA_SAVED");

            if (toikkaSavedVariable != null && !toikkaSavedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(1001,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.ConversationText = "Following his demise in the GDI attack, an honorary military funeral was held for Toikka.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/bg02.png", 2, 0f);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(1002,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "According to a statement by Supreme Leader Ivanov, he had bravely sacrificed himself for his country in the fight against GDI imperialism.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(1003,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "While he was not known as a particularly skilled commander, he had a significant number of units under his command.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(1004,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Losing them has negatively impacted raw Government military strength, although it remains to be seen whether this will have an impact on the battlefield.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            var nodOutpostSavedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_C_NOD_OUTPOST_PRESERVED");

            if (nodOutpostSavedVariable != null && nodOutpostSavedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(2001,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.ConversationText = "Many experts have expected the Brotherhood of Nod to start assisting the Government military against GDI.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/bg03.png", 3, 0f);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(2002,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Based on events in a recent battle for a destroyed heavy vehicle factory complex, this appears to have now happened.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(2003,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Nod forces were fighting side-by-side with Government forces, using advanced technology, including lasers and chemicals, against GDI forces.";
                        storyDisplay.AddSimpleStoryImage("Story/CRC15/bg04.png", 4, 0f);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(2004,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The scope of their cooperation is currently unknown, but it is likely that this will make GDI's already troubled offensive further less likely to succeed.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "While GDI captured a lot of land and dealt damage to Government military facilities, they failed to decapitate Ivanov's leadership and destroy his army.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC15/bg05.png", 5, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov's forces have even been able to perform limited counterattacks, pushing GDI away from a heavy vehicle factory that they were sieging.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The failed operations have come at a significant cost for GDI in both personnel and material.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC15/bg06.png", 6, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While everyone expected GDI to achieve a quick and easy victory, these failures risk turning the offensive from a swift operation to a long and expensive conflict.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With war weariness in the general population at already high levels due to the broader First Tiberium War, the idea of getting bogged down in another conflict is not popular.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC15/bg07.png", 7, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unless GDI is able to secure a victory quickly, their leadership might be forced to re-evaluate whether continuing the offensive is worth the costs.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 8, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(fac2226m);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            if (toikkaSavedVariable != null && toikkaSavedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(3001,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.ConversationText = "Good job on saving Toikka, Commander.";
                        storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                        storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 9, 0f);
                        country1.Play();
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(3002,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Aside from him being useful to... us, most of his remaining forces were stuck in the factory area. They will be assisting you in your upcoming operation.";
                    },
                    null,
                    null,
                    null));
            }
            else
            {
                phases.Add(new Phase(3003,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.ConversationText = "I believe this is the first time you have failed to perform a task that I have assigned to you.";
                        storyDisplay.ConversationDisplay.TextColor = Color.Red;
                        storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 9, 0f);
                        country1.Play();
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(3004,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Although we have already survived for longer than I - or the entire world - expected, I hope Toikka's sacrifice will not be in vain.";
                        storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While we have mostly stopped their progress, GDI is not about to retreat.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We need to push them out.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Not entirely, that would be unrealistic - even with you in command.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Their failure in decapitating us, and their high losses, has led into many politicians and analysts criticising the GDI decision to attack us.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Now is the time to use that to our advantage and double the pain.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We cannot overpower GDI in raw strength - if they really want us dead, they *will* kill us.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But we can make them bleed so much that they figure out that destroying us is not worth the cost for them.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "When they first attacked, they captured our largest harbor and set up infrastructure for flooding in heavy military equipment.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Despite their setbacks, this infrastructure is still actively used to bring in heavy vehicles by the dozen.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If we destroy it, it will be harder for GDI to support their land forces on our territory, significantly increasing the cost of their invasion, and hopefully making them retreat.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Your objective here is simple: attack them, destroy GDI bases, and take our harbor area back.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(9).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRC14()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(fac2226m);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I have to admit.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR07/officebg01.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I am still astonished by how we survived that decapitation strike.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The situation seemed very bleak there for a moment.";
                },
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRC14/bg01.png", 3, 0f);
                    powrdn1.Play();
                },
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(3).AlphaRate = -2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "It looks like we still have some electricity problems after the last battle.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(3),
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Anyway, while we survived for now, the danger is far from over.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The GDI scum has continued their offensive in various directions.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Many of our military units have been simply eliminated, and what we have left, we have been forced to pull back.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The majority of our country is now either a gray zone or occupied by foreign powers.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Not only the Global Defense Initiative: The Brotherhood of Nod has exploited this opportunity as well.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    AddRADisplay(storyDisplay, 4);
                    storyDisplay.ConversationDisplay.ConversationText = "To keep up some coherency in our lines, I assigned Toikka to defend the area of the destroyed vehicle factory.";
                },
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CRC14/factory.png", 5);
                },
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unfortunately, he failed.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "A GDI assault forced him to fall back to the vehicle factory island and fortify himself there.";
                    AddRADisplayImage(storyDisplay, "Story/CRC14/fortifications.png", 6);
                },
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "His forces are now under siege, with GDI surrounding them and attacking them from land, air and the surrounding lake.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I need you to send your forces there and save him. Destroy GDI forces from the area and allow him to escape.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This is going to be a hard operation, but losing him would be a significant blow to us.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There are two factors that should easen your work a bit.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Firstly, now that the Communist problem is solved and we appear to have a common enemy, I contacted representatives of the Brotherhood of Nod.";
                    AddRADisplayImage(storyDisplay, "Story/CRC14/nodlogo.png", 7);
                },
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6),
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We now have a ceasefire.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Nod also has their outpost in the area, and they are also being troubled by the GDI assault.";
                    AddRADisplayImage(storyDisplay, "Story/CRC14/nodoutpost.png", 8);
                },
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(7),
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They are going to help you in combat. If you manage to help them keep their outpost operational, this could pave the way for greater cooperation in the future.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Secondly, I ordered our scientists to look into activating our nuclear warheads that we acquired from the black market afer the collapse of the USSR.";
                    AddRADisplayImage(storyDisplay, "Story/CRC14/nukesilo.png", 9);
                },
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(8),
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They have now managed to do so.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If you build a Nuclear Missile Silo, you will be able to launch nukes at the eagle.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I can't imagine anything more satisfying than that.";
                    storyDisplay.FindStoryImageById(9).AlphaRate = -2.0f;
                    bleep12.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(9),
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "May they knock the bird off the sky.";
                    storyDisplay.FindStoryImageById(4).AlphaRate = -crRAdisplayAlphaRate;
                    bleep17.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.FindStoryImageById(2).AlphaRate = -1.0f;
                    toney4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    HideAllStoryImagesWithSound(storyDisplay, bleep17);
                    TryPlaySong(raintro);
                    storyDisplay.ConversationDisplay.ConversationText = "----------------";
                },
                null,
                null,
                null));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Analyzing the situation, you are clearly Ivanov's most achieved commander.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(31,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While he instructured you to save Toikka, it is hard to say whether that will be possible due to GDI's overwhelming superiority in force.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(32,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, even if you failed, that would not change your position.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(33,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov has no one to replace you with, and you could continue military operations even without Toikka on your side.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(34,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The conclusion is: the most important objective here is to destroy GDI.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(35,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You can also attempt to save Toikka. But while it's important for Ivanov, for you that is of secondary importance, an optional objective.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC09/crtitle_bloodred.png", 10);
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRC13()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(fac2226m);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Commander!";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR07/officebg01.png", 2, 0f);
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRC13/static1.png", 3, 0.85f),
                storyDisplay =>
                {
                    powrdn1.Play();
                    storyDisplay.RemoveStoryImageById(3);
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static2.png", 4, 0.9f).AlphaRate = 0.1f;
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static3.png", 5, 1.0f);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "*** GDI has ******** a full as***** on ** from *** directions.";
                    storyDisplay.RemoveStoryImageById(1);
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static4.png", 6, 0.95f);
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(5);
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static5.png", 7, 0.95f).DrawOrder = 9999;
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRC13/static6.png", 8, 0.9f).AlphaRate = 0.05f,
                storyDisplay => storyDisplay.RemoveStoryImageById(8)));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(6);
                    powrdn1.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "I **** your forces ** the HQ ************.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static7.png", 9, 1.0f);
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(9);
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static8.png", 10, 0.8f);
                },
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static9.png", 11, 0.9f).AlphaRate = 0.08f;
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(11)));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They are ****** to ********** us for a ********* fast victory.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static1.png", 12, 1.0f);
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(12);
                },
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static10.png", 13, 1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = "Do you";
                    storyDisplay.RemoveStoryImageById(10);
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static11.png", 14, 0.75f);
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static7.png", 15, 0.8f);
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/static8.png", 16, 0.9f);
                },
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    powrdn1.Play();
                    storyDisplay.ClearStoryImages();
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/staticfull.png", 17, 1.0f);
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "TRANSMISSION INTERRUP";
                },
                storyDisplay =>
                {
                    country4.Play();
                    TryPlaySong(tdmaptheme);
                    storyDisplay.ClearStoryImages();
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/gdibg1.png", 18, 1.0f);
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "THIS TRANSMISSION HAS BEEN INTERCEPTED BY THE GLOBAL DEFENSE INITIATIVE.";
                },
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "";
                    country1.Play();
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/gdibg2.png", 19, 0f);
                },
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(18);
                    storyDisplay.ConversationDisplay.ConversationText = "SURRENDER NOW, AND YOUR LIVES WILL BE SPARED.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "OTHERWISE, YOU WILL BE BOMBED UNTIL YOU CAN NO LONGER RESIST.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ClearStoryImages();
                    storyDisplay.ConversationDisplay.ConversationText = "----------------";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Reaching Ivanov's headquarters took you a couple of hours.";
                },
                null,
                null,
                storyDisplay => country1.Play()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "All the while, GDI aircraft was striking your forces.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC13/gdiplanes.png", 20);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Despite maximizing anti-aircraft coverage, there were significant casualties.";
                },
                null,
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1)));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Upon your arrival, it seemed that the battle for the headquarters had just begun.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While GDI had seized a lot of other areas and caused damage with air strikes, their land forces were only just about to arrive.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The situation looks hopeless, but maybe you can do something to turn it around...";
                    HideAllStoryImagesWithSound(storyDisplay, toney4);
                    storyDisplay.AddSimpleStoryImage("Story/CRC09/crtitle_bloodred.png", 7);
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRC12()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "W3N - STRATEGIC ANALYSIS";
                    storyDisplay.AddSimpleStoryImage("Story/CRC12/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(raintro);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "The Brotherhood of Nod is suspected to have played a heavy role in destabilizing the country.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC12/bg02.png", 2, 0f);
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRC12/bg03.png", 3, 0f),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They were most likely hoping for a revolution that would lead into a Communist government that they could lead from the shadows, undermining GDI forces in the region.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "That plan failed due to a strong Government military response with GDI support.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While successful in thwarting the Nod-Communist advance, the GDI support turned out to be a failure in other ways, enabling the Government to perform serious war crimes.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC12/bg04.png", 4, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI attempted to alleviate this by withdrawing their support from Ivanov's regime, but it appears this move might have come too late.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Despite lack of support, Government forces have proceeded to the outskirts of Karhumäki, the most significant city of the Communist militia.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC12/bg05.png", 5, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Only a month ago this kind of progress looked almost impossible. A combined Communist-Nod force should have, at minimum, held their ground against Ivanov's forces.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It is likely that the progress is partially enabled by Nod reducing their level of support for the neo-Soviets.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There is a probable motive for this - as the previously GDI-supported Government has performed war crimes, it has become a significant stain in GDI's reputation.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC12/bg06.png", 6, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Allowing the Government to perform further war crimes on new territory will further undermine the legitimacy of GDI in the region, which allows the Brotherhood of Nod to present themselves as the morally superior side.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Additionally, even if the Government won the civil war, the unpopular leadership and international isolation would likely force the Government to seek assistance from the Brotherhood of Nod to survive in the long term.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC12/bg07.png", 7, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To conclude, regardless of which side emerges victorious, the Brotherhood is set to have significant leverage on local affairs.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Of course, staying on the sidelines during the fight also preserves the Brotherhood's troops for potential future operations.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, null, -2.5f),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 8, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(fac2226m);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 9, 0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(8),
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I have made a strategic decision.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We need to send a strong message, both to our enemies as well as our \"friends\" at the Global Defense Initiative.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Destroy the city of Karhumäki.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Obliterate all buildings, regardless of form or function, and kill everyone you come across.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Man, woman, or child - it does not matter.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "All adults living there have picked their side by now. At the very least, they do not mind living under Communist leadership, which can already be seen as treason on its own.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Everyone truly loyal to us has escaped from there a long time ago.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As for the children...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...it can be seen as tragic, but if they are old enough to be conscious and they see our forces killing their parents, they will hate us forever.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Leaving them alive would, unfortunately, only spell trouble after a decade or two, when they have grown up and could perform violent acts of their own.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Thus, it is... necessary to pre-emptively neutralize them as well.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "What is also important is that this kind of an operation will destroy the Communist leadership.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Even if they escape the battle, losing such a major centre of population will prove the incompetency of their leadership to the regular soldier, causing a collapse in the ranks of their militia.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To strengthen the symbolism of this act, I will be giving you control of our last GDI heavy equipment for this operation.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The international news agencies are going to be absolutely delighted by the sight of GDI Mammoth tanks leveling down the city - just imagine how much such a controversy increases their sales.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(31,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "And to the GDI leadership, it will signal that our course cannnot be changed through blackmailing us with sanctions, or even by ending military support.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(32,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Our final victory is at hand, Commander.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(9).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(33,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRC09/crtitle_bloodred.png", 10);
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRC11()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(fac2226m);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You have done some astonishing work.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR07/officebg01.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have reached the gates of Karhumäki.";
                    storyDisplay.RemoveStoryImageById(1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, they have heavily fortified the city.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We need to resort to some unusual equipment to break it.";
                },
                storyDisplay => AddRADisplay(storyDisplay, 3),
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have kept our Battle Rig project hidden so far, but it has come time to reveal it. It is a supermassive tank designed for exactly this situation.";
                    AddRADisplayImage(storyDisplay, "Story/CRC11/brig.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But it is too slow to move to the frontline by itself. We will need to carry it close to the city by train.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The only suitable location for preparing it near the city is controlled by the enemy.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CRC11/railway.png", 5),
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Attacking their outpost directly would be too risky.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We need to trick them into receiving our train.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Send a Spy into their command center. He will deliver a broadcast making them believe the approaching train to be friendly.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, the train will actually contain our squad of elite soldiers. Use them to capture the base.";
                    storyDisplay.FindStoryImageById(5).AlphaRate = -2.0f;
                    bleep12.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate;
                    bleep17.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "Afterwards, prepare to defend against any potential counterattacks until the Battle Rig has been prepared.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(3),
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I will be waiting for news of your success. Until then, I will spend my time planning our assault on the city.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    HideAllStoryImagesWithSound(storyDisplay, toney4);
                    storyDisplay.AddSimpleStoryImage("Story/CRC09/crtitle_bloodred.png", 7);
                },
                null));

            return phases;
        }

        private List<Phase> CRC10()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRC10/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(ramap);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "After the Global Defense Initiative withdrew their support from Ivanov's Government due to war crimes, most expected a quick Nod-Communist steamroll through the country.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC10/bg02.png", 2, 0f);
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRC10/bg03.png", 3, 0f),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With enough military pressure, the Government would then be forced to perform changes to allow GDI to reinstate support for them.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It was widely seen that the Government had little chance for victory when fighting alone.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, contrary to expectations that Ivanov would set up strong defensive lines and hope to wait out the situation, his commanders have went fully on the offense.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC10/bg04.png", 4, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This move caught his enemies unprepared.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Government forces have punched a hole to the Nod-Communist frontline, leaving companies of soldiers to a chaotic retreat.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Making matters worse for the Nod-Communist alliance is the fact that this hole was punched largely with captured neo-Soviet equipment, meaning Government vehicle losses were small.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC10/bg05.png", 5, 0f);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It would, however, be very premature to talk of Ivanov's victory in the strategic sense.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC10/bg06.png", 6, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While a significant turn of events, the tactical success has not yet developed into a major breakthrough.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Communist sources have said that they are preparing new defensive lines that will stop Government forces before they reach Karhumäki, the de facto capital city of the Communist movement.";
                    storyDisplay.AddSimpleStoryImage("Story/CRC10/bg07.png", 7, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Even if Government forces reached the city, conquering and holding the city itself would be a major challenge, especially with its mostly hostile civilian population.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, null, -2.5f),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 8, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(fac2226m);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "The enemy's long-term defensive strategy relies on a river between our forces and the city of Karhumäki.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 9, 0f);
                    country1.Play();
                },
                storyDisplay => AddRADisplay(storyDisplay, 10),
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CRC10/river.png", 11),
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If they manage to blow up the bridge and fortify the riverside, it will be impossible for us to pass this obstacle.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Luckily, the scum is now in disarray. We need to exploit this opportunity before they can get their act together.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They are preparing defensive lines, but currently these \"lines\" consist mainly of idle Mammoth tanks and infantry.";
                    AddRADisplayImage(storyDisplay, "Story/CRC10/defenses.png", 12);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(11),
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With the swamps still partially frozen, our lighter vehicles have no problem moving past these clumsy formations.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Take a force of Light Tanks and Rangers. Avoiding these defenses, flank the enemy bases southwest of the river and destroy them.";
                    AddRADisplayImage(storyDisplay, "Story/CRC10/bases.png", 13);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(12),
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Once you are successful - and I trust you will be - we will be setting up an MCV for you on the northeastern side of the river.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(13).AlphaRate = -2.0f; bleep12.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(13),
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Build a base there, and wipe out all Nod and neo-Soviet resistance.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If you succeed, our forces will be at the outskirts of Karhumäki, making it possible for us to capture it and humiliate the Communist leadership currently residing there.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Our current thrust into the enemy lines is deep, but not very wide.";
                    AddRADisplayImage(storyDisplay, "Story/CRC10/thrust.png", 14);
                },
                null,
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There is a risk of flank attacks, especially by rapidly moving Nod forces.";
                    AddRADisplayImage(storyDisplay, "Story/CRC10/flank.png", 15);
                },
                null,
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I will be assigning Toikka to cover your flanks, so you won't need to worry about them and can focus purely on pushing forward.";
                },
                null,
                storyDisplay => 
                { 
                    storyDisplay.FindStoryImageById(14).AlphaRate = -2.0f;
                    storyDisplay.FindStoryImageById(15).AlphaRate = -2.0f;

                    bleep12.Play(); 
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(14);
                    storyDisplay.RemoveStoryImageById(15);
                }));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(10).AlphaRate = -crRAdisplayAlphaRate;
                    bleep17.Play();

                    storyDisplay.ConversationDisplay.ConversationText = "I could wish you good luck, but this situation won't be resolved by luck.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(10),
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It will be resolved through skill, through mastery in the art of warfare.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I hope you will again prove more skillful than our enemies.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(9).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRC09()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(fac2226m);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "So, as you certainly already know, the Global Defense Initiative halted their support for us.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR07/officebg01.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This will make our survival difficult.";
                    storyDisplay.RemoveStoryImageById(1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Very difficult.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While... let's say I unofficially approve of the way you have punished traitors to our state, you seem to have overdone it.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You got GDI too interested in the events.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In other words, _you_ brought us into this mess.";
                },
                storyDisplay => AddRADisplay(storyDisplay, 3),
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If we look at prepared forces, it is suspected that our strength is slightly below the combined force of Nod and the Communists.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CRC09/chart1.png", 4),
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We are at a disadvantage. Especially since we are currently the side trying to attack.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, if we look at manufacturing capacity... we far outweigh the Communists, but with Nod support, and no GDI support for us, it is expected that they are significantly outproducing us.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CRC09/chart2.png", 5),
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Nod appears to have an active program for spreading Tiberium on territory controlled by them. Once they get to harvest it, this disparity will only grow in the future.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In other words, we are at a disadvantage right now. And the more time passes, the bigger our disadvantage will be.";
                    storyDisplay.FindStoryImageById(5).AlphaRate = -2.0f;
                    bleep12.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have no diplomatic options.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If we surrender to the Communists, they will simply kill us. They especially have no need for negotiations when they have the advantage.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I know that GDI does not want us to fall. However, they want us to step down. If we surrender to them, they will come, force a change of government, perform investigation, throw both of us into jail, and kick Nod out.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We are not going to let either happen. At least not without trying a solution of our own first.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "That means we only have one option: to go fully on the offense with what we have.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We will charge at Karhumäki, where the Communist leadership is hiding. Try to reach the city, assault it, and decapitate the Communist leadership.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "By classic military theory, to succeed, an attacking force needs to be three times larger than the defending force.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We do not have anywhere near such a ratio. You will have to do with less.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Capturing enemy equipment can help improve our chances.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Thus, you will begin our offensive with an attack that aims to capture a Communist base. If successful, we get to use the enemy's own equipment against them, sparing our resources.";
                    AddRADisplayImage(storyDisplay, "Story/CRC09/base.png", 6);
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This could be seen as a somewhat desperate move, but you have achieved victory from poor initial conditions before. Hopefully you'll do so again.";
                    storyDisplay.FindStoryImageById(6).AlphaRate = -2.0f;
                    bleep12.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "You'd be best to perform, considering it was you whose actions brought us into this situation.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    HideAllStoryImagesWithSound(storyDisplay, toney4);
                    storyDisplay.AddSimpleStoryImage("Story/CRC09/crtitle_bloodred.png", 7);
                },
                null));

            return phases;
        }

        private List<Phase> CRB17Victory()
        {
            var phases = new List<Phase>();

            var peaceWithCommunistsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_PEACE_WITH_COMMUNISTS");
            var cityPreservedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_CITY_PRESERVED");

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "After a heavy battle with lots of casualties on both sides, Nod forces had been defeated.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(stopthem);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You had achieved your final victory over Ivanov's regime and the Brotherhood of Nod.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            if (peaceWithCommunistsVariable != null && !peaceWithCommunistsVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg02.png", 2);
                        storyDisplay.ConversationDisplay.ConversationText = "The Communists were left facing GDI alone, which resulted in a collapse of their leadership in a timespan of mere days.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg03.png", 3);
                        storyDisplay.ConversationDisplay.ConversationText = "GDI did not even need to attack the Communist HQ at Karhumäki. Without Nod support, mild pressure was enough for the Communist rule to implode. The people accepted the governance of the approaching GDI forces.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Sheppard sent you a congratulatory message on your victory.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg04.png", 4);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This victory also further improved the international reputation of the Global Defense Initiative as the world's most significant military force.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg05.png", 5);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In the immediate aftermath, you were set to lead a GDI-driven military administration of the country.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI concluded their investigation into Ivanov's fate.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "He was confirmed dead. During your assault on his HQ, he had blown himself up in the underground facility below his base that your spy had infiltrated last winter.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg06.png", 6);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Toikka barely survived. After stabilizing at the hospital, he was trialed and sentenced to a lengthy prison sentence for his crimes against the people.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg07.png", 7);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI also finished processing Government archives.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It was revealed that Ivanov had his own chemical weapons program.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg08.png", 8);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This program was further supported by Nod after GDI turned against Ivanov.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As one event of particular significance, the chemical explosion of a train in the early phases of the conflict had been a false-flag operation by Ivanov to strengthen GDI support.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg09.png", 9);
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI falling into this trap by a murderous dictator led to some international controversy, but the twists of events happened since then kept the criticism at a relatively quiet level.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Following a brief time of military administration led by you, the United Nations ordered free elections to be held.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg10.png", 10);
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The people were not particularly fond of democracy as their previous experience of it under Ivanov had been polarizing, but most people decided to give it another chance - not like they had much of a choice.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            AddCRBEndPhases(phases);

            AddCreditPhases(phases, new Color(255, 255, 0));
            AddPostRouteBHintPhases(phases);

            return phases;
        }

        private void AddCRBEndPhases(List<Phase> phases)
        {
            var peaceWithCommunistsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_PEACE_WITH_COMMUNISTS");
            var cityPreservedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_CITY_PRESERVED");

            bool peaceWithCommunists = peaceWithCommunistsVariable != null && peaceWithCommunistsVariable.EnabledThroughPreviousScenario;
            bool cityPreserved = cityPreservedVariable != null && cityPreservedVariable.EnabledThroughPreviousScenario;

            if (peaceWithCommunists)
            {
                if (cityPreserved)
                {
                    phases.Add(new Phase(20,
                        storyDisplay =>
                        {
                            TryPlaySong(chrg226m);
                            storyDisplay.ConversationDisplay.ConversationText = "The results of the election were mixed.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(21,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "The Communist leadership had at least officially disbanded their military units and reformed themselves into a peaceful party.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg02.png", 20);
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(22,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "There was relatively even support for this new Communist party as well as liberal parties that pushed for closer ties with the West and international institutions, like GDI.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg05.png", 21);
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(23,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "After lengthy negotiations, a compromising government was formed.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg19.png", 22);
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(24,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "They balanced a Soviet-style, state-driven economic policy with some democratic reforms and closer ties to the West.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(25,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "The Communist side was somewhat suspicious of their former enemy, the Global Defense Initiative, and thus wanted the country to upkeep a small military force that was separate from GDI.";
                            storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 23);
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(26,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "The liberal side agreed to this with the condition that you, a former enemy of the Communists and current GDI Commander, was made the Commander of this new force.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(27,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "You were officially to lead a force separate from GDI, but practically, not much prevented you from keeping close ties to GDI.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(28,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "The GDI was also happy for you to be in this new role, keeping the region stable in case of future unrest.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg13.png", 24);
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(29,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg14.png", 25);
                            storyDisplay.ConversationDisplay.ConversationText = "It is unknown how the country will do in the long term. Tens of thousands had died, many cities and villages had been ruined, and Tiberium had grown and ravaged a lot of the country. The challenges were enormous.";
                        },
                        storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg15.png", 26),
                        null,
                        null));

                    phases.Add(new Phase(30,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "The processing of Tiberium might become a significant industry for the country in the future, and the cooperation of traditional enemies gives hope for a better future.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(31,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Other than that, direct economic opportunities appear somewhat slim, however.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(32,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "For now, the future is unknown, but there is some hope and promise. Definitely far more so than under Ivanov's rule, and likely also more so than there would have been under pure Communist - or practically, Nod rule.";
                        },
                        null,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "";
                            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                        },
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(33,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.IsCentered = true;
                            storyDisplay.ConversationDisplay.TextColor = Color.White;
                            storyDisplay.AddSimpleStoryImage("Story/CRB12/crtitle_yellow.png", 26);
                            storyDisplay.ConversationDisplay.ConversationText = "           ENDING VI" + Environment.NewLine + "    The Commander-in-Chief";
                        },
                        null,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "";
                            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                            StopMusic();
                        },
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
                else
                {
                    phases.Add(new Phase(20,
                        storyDisplay =>
                        {
                            TryPlaySong(chrg226m);
                            storyDisplay.ConversationDisplay.ConversationText = "The Communist leadership had at least officially disbanded their military units and reformed themselves into a peaceful party.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg02.png", 20);
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(21,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "This new Communist party ended up getting a landslide victory.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(22,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "The support for the Communists was higher than anticipated by any international institution.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(23,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Ivanov's government had been unpopular in many areas and the Communists were seen as providing a real - not fresh, but still real - alternative to the corruption that his regime had brought.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg17.png", 21);
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(24,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "But the most surprising aspect was that the Communists also gained relatively high support from areas that previously supported Ivanov.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(25,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Their votes were expected to go to either nationalist or Western-minded parties, but for some reason, anti-GDI sentiment was high, presumably due to destruction caused by the war.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg12.png", 22);
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(26,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "This directed votes to the Communists - the only political force that had really survived the extended civil war.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(27,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "It could be said that the Communists succeeded in their revolution attempt - just not by weapons, but by votes.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(28,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "As one of the first changes after coming to power, the new Communist government relieved you from the country's military.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(29,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "You were still leading the local detachment of GDI forces, but the Communists distanced you from the country's affairs as much as they could.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg13.png", 23);
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(30,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Your reputation within GDI was still good. For now, you were tasked with limiting local military buildup, and unofficially, also with preventing the Communists from giving their support to any foreign anti-GDI Communist movements.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(31,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Otherwise, the Communists proceeded to dismantle corruption structures set up by Ivanov, followed by setting up a heavily state-driven rebuilding program.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg11.png", 24);
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(32,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Tens of thousands had died, many cities and villages had been ruined, and Tiberium had grown and ravaged a lot of the country, meaning rebuilding and reinvestment was highly necessary.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg14.png", 25);
                        },
                        storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg15.png", 26),
                        null,
                        null));

                    phases.Add(new Phase(33,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "It seems unlikely that the new Government's Soviet-inspired state-driven investment plans would make the economy particularly prosper in the long run.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(34,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "But at least the country should stay more stable and peaceful than it was during Ivanov's rule.";
                        },
                        null,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "";
                            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                        },
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(35,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.IsCentered = true;
                            storyDisplay.ConversationDisplay.TextColor = Color.White;
                            storyDisplay.AddSimpleStoryImage("Story/CRB12/crtitle_yellow.png", 26);
                            storyDisplay.ConversationDisplay.ConversationText = "           ENDING IV" + Environment.NewLine + "  Successful Communist Revolt";
                        },
                        null,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "";
                            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                            StopMusic();
                        },
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
            }
            else
            {
                if (cityPreserved)
                {
                    phases.Add(new Phase(20,
                        storyDisplay =>
                        {
                            TryPlaySong(chrg226m);
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg13.png", 20);
                            storyDisplay.ConversationDisplay.ConversationText = "The results of the election showed support for closer ties with the West as well as international institutions, including GDI.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(21,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg16.png", 21);
                            storyDisplay.ConversationDisplay.ConversationText = "The competing parties were mostly unknown to the wider audience; everyone who had participated in politics prior to the extended civil war had either died or had their reputation tarnished by war crimes.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(22,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg17.png", 22);
                            storyDisplay.ConversationDisplay.ConversationText = "The winner was a liberal party that sought to dismantle corruption structures set up by Ivanov and perform democratic as well as economical reforms.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(23,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg18.png", 23);
                            storyDisplay.ConversationDisplay.ConversationText = "You were chosen to be the new republic's first Commander-in-Chief of the Defense Forces, which were heavily integrated into the GDI command structure.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(24,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg14.png", 24);
                            storyDisplay.ConversationDisplay.ConversationText = "It is unknown how the country will do in the long term. Tens of thousands had died, many cities and villages had been ruined, and Tiberium had grown and ravaged a lot of the country. The challenges were enormous.";
                        },
                        storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg15.png", 25),
                        null,
                        null));

                    phases.Add(new Phase(25,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "GDI's participation should at least help halt or even revert the Tiberium growth, converting it to money instead.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(26,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "For now, the future is unknown, but there is some hope and promise. Definitely far more so than under Ivanov's rule, and likely also more so than there would have been under Communist - or practically, Nod rule.";
                        },
                        null,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "";
                            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                        },
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(27,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.IsCentered = true;
                            storyDisplay.ConversationDisplay.TextColor = Color.White;
                            storyDisplay.AddSimpleStoryImage("Story/CRB12/crtitle_yellow.png", 26);
                            storyDisplay.ConversationDisplay.ConversationText = "           ENDING V" + Environment.NewLine + "      The GDI Commander";
                        },
                        null,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "";
                            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                            StopMusic();
                        },
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
                else
                {
                    phases.Add(new Phase(20,
                        storyDisplay =>
                        {
                            TryPlaySong(chrg226m);
                            storyDisplay.ConversationDisplay.ConversationText = "The results of the election didn't cast a clear future for the country.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(21,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "All parties were led by relatively unknown people and none managed to really win the vote.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(22,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 20);
                            storyDisplay.ConversationDisplay.ConversationText = "Nationalist anti-GDI sentiment was high among the population, especially in areas where Ivanov used to enjoy high level of support.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(23,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg11.png", 21);
                            storyDisplay.ConversationDisplay.ConversationText = "Support for Communists and other far-left parties was also high.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(24,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg12.png", 22);
                            storyDisplay.ConversationDisplay.ConversationText = "After lengthy negotiations, a new compromising government was formed.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(25,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Absolutely no one is confident in its long-term stability.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(26,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "You weren't really trusted by this new government either.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(27,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg13.png", 23);
                            storyDisplay.ConversationDisplay.ConversationText = "You were still leading the local detachment of GDI forces, but the new government, suspicious of GDI, distanced you from the country's affairs as much as they could.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(28,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "At least your future career in GDI's ranks is secured, if nothing else.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(29,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg14.png", 24);
                            storyDisplay.ConversationDisplay.ConversationText = "Aside from that, due to the uncertain election result, most people felt that all the fighting had given little improvement to their lives.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(30,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Tens of thousands had died, many cities and villages had been ruined, and Tiberium had grown and ravaged a lot of the country.";
                        },
                        storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRB17/victorybg15.png", 25),
                        null,
                        null));

                    phases.Add(new Phase(31,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Only for tensions among the people to remain high, and the extent of corruption only limited by stronger GDI and UN presence.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(32,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "The processing of Tiberium might become a significant industry for the country in the future, but other than that, future economic opportunities also appear slim.";
                        },
                        null,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "";
                            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                        },
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(33,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.IsCentered = true;
                            storyDisplay.ConversationDisplay.TextColor = Color.White;
                            storyDisplay.AddSimpleStoryImage("Story/CRB12/crtitle_yellow.png", 26);
                            storyDisplay.ConversationDisplay.ConversationText = "           ENDING III" + Environment.NewLine + "           Status Quo";
                        },
                        null,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "";
                            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                            StopMusic();
                        },
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
            }
        }

        private List<Phase> CRB17()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/gdilogo.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    newtarg1.Play();
                    TryPlaySong(chrg226m);

                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander, the time has come.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg05.png", 2, 0f);
                    beepy3.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have destroyed Ivanov's Government.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg04.png", 3, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(2)));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But Nod still has a foothold on Karelian territory, and it doesn't look like they will be leaving voluntarily.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg02.png", 4, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(3)));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have been probing their lines for some time now, but their defenses have proven strong.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg05.png", 5, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We will send another major force to push against Nod's frontline.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg03.png", 6, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Use the ensuing chaos to bring your forces over a lake, allowing you to establish a base between Nod lines.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg06.png", 7, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Build up a force and wipe all trace of Nod off the face of the earth.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg02.png", 8, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(7)));

            var peaceWithCommunistsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_PEACE_WITH_COMMUNISTS");

            if (peaceWithCommunistsVariable != null && peaceWithCommunistsVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Nod might get some help from defected remnants of Ivanov's guard, but the Commies have also shown interest in helping us finish them off.";
                        storyDisplay.AddSimpleStoryImage("Story/CRB12/bg04.png", 9, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(8)));
            }
            else
            {
                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "If you succeeed, the neo-Soviets won't have the strength to put up resistance by their own.";
                        storyDisplay.AddSimpleStoryImage("Story/CRB12/bg04.png", 9, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(8)));
            }

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Beware of Nod's new weaponry, created by combining technology from different sources - including technology stolen from us.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg03.png", 10, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(9)));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Good luck.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg07.png", 11, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(10),
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(11).AlphaRate = -2.0f; toney4.Play(); }));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB16Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov's Government suffered its final defeat.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(stopthem);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Government no longer exists in any concrete form. It is gone.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It only exists in memory, but even then, only temporarily. In the future, it is going to remain only as a small remark, a curiosity within the long and wide pages of human history.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The revolution, which was started by Nod-backed Communists, ended up finished largely by a brave ex-official from Ivanov's own government, who had become the commander of GDI's local branch.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov himself was not immediately seen anywhere in the aftermath of the battle, in any form.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg02.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Various rumors are spreading around. A popular one within locals is that he secretly escaped to Siberia to evade prosecution. GDI investigation into his fate will continue.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Most realistically, it is expected that he is dead, but there is no confirmation yet.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov's right hand, Toikka, was found unconscious and heavily injured under the rubble of the destroyed Battle Rig - a massive battle fortress that Ivanov had thrown against GDI as his last desperate move.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg03.png", 3, 0f);
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg04.png", 4, 0f),
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With this victory, GDI has full access to the archives of Ivanov's Government, at least all information that hasn't been destroyed. It is expected that this will shine further light on Government actions in the war, which has lasted over a year by now.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg05.png", 5, 0f);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As for the GDI Commander, this victory has gained them further popularity abroad.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg06.png", 6, 0f);
                },
                null,
                null,
                null));

            var peaceWithCommunistsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_PEACE_WITH_COMMUNISTS");
            var cityPreservedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_CITY_PRESERVED");

            bool peaceWithCommunists = peaceWithCommunistsVariable != null && peaceWithCommunistsVariable.EnabledThroughPreviousScenario;

            if (cityPreservedVariable != null && cityPreservedVariable.EnabledThroughPreviousScenario)
            {
                // City was mostly preserved

                phases.Add(new Phase(11,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Domestically, the reception seemed relatively good too. Some people were reluctant to accept GDI troops, but generally, there was no major protest movement on previously Ivanov-controlled territory.";
                    },
                    null,
                    storyDisplay => { if (peaceWithCommunists) HideAllStoryImagesWithSound(storyDisplay, country1); },
                    storyDisplay => { if (peaceWithCommunists) storyDisplay.ClearStoryImages(); }));

                if (peaceWithCommunists)
                {
                    phases.Add(new Phase(12,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "There were some worries related to the GDI Commander's cooperation with the Communists.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg07.png", 7, 0f);
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(13,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "However, most people are hopeful that the cooperation is only for military affairs, and GDI is not allowing a brutal Soviet-style government to emerge.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
                else
                {
                    phases.Add(new Phase(12,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "The mental state of most people appears to be waitful. There's apprehension on what will come next, meaning GDI will need to soon outline concrete steps for forming a new Government.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
            }
            else
            {
                // City took heavy damage

                phases.Add(new Phase(11,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Domestically, the reception was less enthusiastic.";
                        storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg08.png", 8, 0f);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(12,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Looking at the destruction brought to their towns and cities, most people felt that GDI was about to replace one tyrant with another, simply leaving them worse off due to all the caused destruction.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                if (peaceWithCommunists)
                {
                    phases.Add(new Phase(13,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Many seemed happier about the Communist forces - while there is strong opposition to communism, people at least respected them as part of \"theirs\" due to their shared nationality.";
                            storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg07.png", 7, 0f);
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
            }

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Sheppard was too busy to visit you physically. But he relayed you a congratulatory message on your victory.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg09.png", 9, 0f);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This victory is not completely set in stone yet, though. There is still one job left for you.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Nod is still controlling a piece of your land. It is high time to kick them out.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/victorybg10.png", 10, 0f);
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB16()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "W3N - CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(tdmaptheme);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            var peaceWithCommunistsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_PEACE_WITH_COMMUNISTS");

            if (peaceWithCommunistsVariable != null && peaceWithCommunistsVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.ConversationText = "In the past few weeks, GDI has managed to put further pressure on Ivanov, especially as the ceasefire with the Communists has freed up GDI resources.";
                        storyDisplay.AddSimpleStoryImage("Story/CRB16/bg02.png", 2, 0f);
                    },
                    null,
                    null,
                    null));
            }
            else
            {
                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.ConversationText = "In the past few weeks, GDI has managed to put further pressure on Ivanov.";
                        storyDisplay.AddSimpleStoryImage("Story/CRB16/bg02.png", 2, 0f);
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "His control of the country's territory is restricted to a single enclave, with his most loyal forces remaining on the defense.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/bg03.png", 3, 0f);
                    storyDisplay.ConversationDisplay.ConversationText = "This has raised speculation on the GDI approach to finishing the conflict.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Optimally, GDI would simply lay siege, hold ground and let Ivanov's Government collapse due to lack of supplies, giving GDI an easy victory with few casualties.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));


            if (peaceWithCommunistsVariable != null && peaceWithCommunistsVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/CRB16/bg04.png", 4, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "However, the threat posed by the Brotherhood of Nod might make this option unviable.";
                    },
                    null,
                    null,
                    null));
            }
            else
            {
                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/CRB16/bg04.png", 4, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "However, the threat posed by the Brotherhood of Nod and their neo-Soviet puppets might make this option unviable.";
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov's forces are keeping a significant number of GDI units occupied, preventing them from being used in the fight against Nod.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB16/bg05.png", 5, 0f);
                    storyDisplay.ConversationDisplay.ConversationText = "In the event of a successful attack by Nod, Ivanov's remaining forces could exploit the opportunity and free themselves from the siege.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In the worst case, this could lead into GDI units getting sandwiched between Nod and Government forces, which could result in heavy casualties.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Thus, it is looking more likely that GDI's local ex-Government Commander will be assigned to perform a direct assault on Ivanov's last stronghold.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -2.5f);
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/gdilogo.png", 6, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    newtarg1.Play();
                    TryPlaySong(chrg226m);

                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "It has come time to finish what you started.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg05.png", 7, 0f);
                    beepy3.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov has strong defense, but we have identified a vulnerable position. A riverbed.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg03.png", 8, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                    AddTDDisplay(storyDisplay, 50);
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(7)));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Sneak in some troops there using an amphibious transport and take out the enemy's AA guns on the shore.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg04.png", 10, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                    AddTDDisplayImage(storyDisplay, "Story/CRB16/riverbed.png", 51);
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(8)));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "That will allow us to reinforce the position with aerial reinforcements.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg06.png", 11, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(10)));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Proceed and take out an ammo cache, which will pave the way for a successful direct ground offensive.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg04.png", 12, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                    AddTDDisplayImage(storyDisplay, "Story/CRB16/ammocache.png", 52);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(51),
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(11);
                    storyDisplay.FindStoryImageById(52).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    beepy2.Play();
                }));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Good luck.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg07.png", 13, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(12);
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    bleep17.Play();
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(52); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(13).AlphaRate = -2.0f; toney4.Play(); }));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB15()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "W3N - CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRB15/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(chrg226m);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Journalist Greg Burdette, of WWN, recently visited the Karelian warzone.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB15/bg02.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "He spread word of GDI war crimes in their assault on the shoreline, with GDI supposedly having intentionally destroyed civilian buildings in the attack.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "W3N also visited the area. Despite damages from the battle, the civilian buildings had been left relatively unscathed.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB15/bg03.png", 3, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Interviewed locals also told W3N that they do not remember seeing WWN or Greg Burdette in the area.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This, along with the legacy of the fabricated Bialystok scandal, make it doubtful whether Greg Burdette ever actually visited the conflict zone.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB15/bg04.png", 4, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Regardless, his reports have went viral on several smaller-scale news outlets, making a number of people worldwide believe this anti-GDI conspiracy theory.";
                },
                null,
                storyDisplay => { HideAllStoryImagesWithSound(storyDisplay, country1); StopMusic(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    world2.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "---------";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "* * * FETCHING BRIEFING ITEM * * *";
                    TryPlaySong(tdmaptheme);
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    country4.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "";
                    storyDisplay.AddSimpleStoryImage("Story/CRB15/bg05.png", 5, 0f).AlphaRate = 2.0f;
                },
                null,
                storyDisplay => bleep11.Play(),
                null));

            const float briefingTextAlphaRate = 1.0f;

            phases.Add(new Phase(11,
                null,
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRB15/bg06.png", 6, 0f).AlphaRate = briefingTextAlphaRate,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/crtitle_yellow.png", 9, 1f).DrawOrder = 4;
                    storyDisplay.FindStoryImageById(6).AlphaRate = -briefingTextAlphaRate;
                    bleep11.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            var sovietCommandoFoundVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_SOVIET_COMMANDO_FOUND");
            if (sovietCommandoFoundVariable != null && sovietCommandoFoundVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(12,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/CRB15/bg08.png", 8, 0f).AlphaRate = briefingTextAlphaRate;
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(8).AlphaRate = -briefingTextAlphaRate;
                        storyDisplay.FindStoryImageById(5).AlphaRate = -0.8f;
                        toney4.Play();
                    },
                    null));
            }
            else
            {
                phases.Add(new Phase(12,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/CRB15/bg07.png", 7, 0f).AlphaRate = briefingTextAlphaRate;
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(7).AlphaRate = -briefingTextAlphaRate;
                        storyDisplay.FindStoryImageById(5).AlphaRate = -0.8f;
                        toney4.Play();
                    },
                    null));
            }

            return phases;
        }

        private List<Phase> CRB14Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The GDI has gained another victory.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/victorybg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(tdmaptheme);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Their forces broke through the heavily fortified frontlines and arrived near Karhumäki, putting the only remaining Communist-controlled major settlement at risk.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Reportedly there is significant demotivation, confusion and even panic in the neo-Soviet leadership over this development.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/victorybg02.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It is possible that their command will implode even without GDI military action on the city.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Brotherhood of Nod has reacted to this by strengthening their own forces and reducing support for the fledgling Communists.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/victorybg03.png", 3, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Supposedly some senior officers in the Brotherhood are seeing further military help to the Communists as a waste of resources, considering their recent poor results.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            var sovietCommandoFoundVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_B_SOVIET_COMMANDO_FOUND");
            if (sovietCommandoFoundVariable != null && sovietCommandoFoundVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        TryPlaySong(chrg226m);
                        storyDisplay.ConversationDisplay.TextColor = Color.Gold;
                        storyDisplay.AddSimpleStoryImage("Story/CRB14/victorybg04.png", 4);
                        storyDisplay.ConversationDisplay.ConversationText = "The neo-Soviet Commando that switched to your side, Yarvelja, appears to be popular among the Communist forces.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "He says his main motivation for fighting in the civil war was to topple Ivanov's corrupt government.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "As you share this goal, he has figured he - and many other Communists - have no reason to keep fighting against you.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(10,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "He has proposed he could use his influence to negotiate a ceasefire between the GDI and the neo-Soviets.";
                        storyDisplay.AddSimpleStoryImage("Story/CRB14/victorybg05.png", 5);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(11,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "After the war is over, this ceasefire could develop into negotiations of governance with the United Nations.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(12,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "He only asks that should you accept this offer, you will protect the Communists from potential retribution by Nod.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(13,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Whether you accept the offer or dismiss it is up to you.";
                    },
                    null,
                    null,
                    null));
            }

            return phases;
        }

        private List<Phase> CRB14()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(chrg226m);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "GDI accepting the rebellious Commander into their ranks has majorly shifted the battlefield.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/bg02.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "At first, it brought GDI into a tricky crossfire situation, with them having to fight both the Government as well as the combined Nod-Soviet forces.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, the GDI appears to be successful in their attempts to stabilize the situation.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "According to an anonymous Nod official, they are now considering providing ammunition and intel to the Government forces.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/bg03.png", 3, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They want to stay loyal to their neo-Soviet allies and would make sure the Government keeps losing any engagement with the Soviets.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But they could potentially cause GDI further casualties by selectively strengthening the Government when they are facing GDI forces.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Nod is also generally increasing their commitment to the battle since they are now fighting directly against GDI, seen as the highest priority in the Brotherhood.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/bg04.png", 4, 0f);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It is suspected that the GDI will next try to assault Karhumäki, bringing pressure on the neo-Soviet---";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/bg05.png", 5, 0f);
                },
                storyDisplay =>
                {
                    StopMusic();
                    powrdn1.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "* * * TRANSMISSION INTERRUPTED * * *";
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/bg06.png", 6, 1f);
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.RemoveStoryImageById(5);
                },
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, null),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    tone5.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "---------";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    tone5.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "---------";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 7, 0f).AlphaRate = 2.5f;
                    newtarg1.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "* * * TRANSMISSION FOUND * * *";
                    TryPlaySong(secondhand);
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/officebg01.png", 8, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "How easily we could have achieved our shared victory... destroyed the remnants of the neo-Soviets, kicked Nod out of the country... we would have peace already.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You could have lived peacefully as our top military official, I could even have given you a big retirement bonus, a nice house on the shore of Lake Ladoga...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But instead, you decided to turn our glorious country into a giant field of graves for thousands of GDI soldiers.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I am worried we will run out of space for burying them all.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I know our forces can't reach you directly at the GDI HQ.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Maybe the leadership of the Brotherhood will accept some more bribes in exchange for sending some of their best assassins after you.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Anyway, Commander.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Know that you are now my biggest enemy.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I do not care about the neo-Soviets. They are filth, but you are far worse.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have prepared some new weapons.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB14/bg07.png", 9, 0f).AlphaRate = 10.0f;
                    storyDisplay.ConversationDisplay.ConversationText = "I will personally make sure your career in the GDI will end prematurely with the blood of countless Western boys.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Red;
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/crtitle_yellow.png", 11, 1f).DrawOrder = 7;
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(8),
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(9).AlphaRate = -2.0f; toney4.Play(); }));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB13Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Your operation successfully halted the offensive of the Nod-Communist alliance.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB13/victorybg01.png", 1);
                    toney7.Play();
                    TryPlaySong(tdmaptheme);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, the current state of affairs is not good enough.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The enemy is able to gather more forces and launch a renewed offensive in the future.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB13/victorybg02.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To prevent that, it has come time to launch a counterattack.";
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB13()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/gdilogo.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    newtarg1.Play();
                    TryPlaySong(chrg226m);

                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Good job on establishing the beachhead.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg04.png", 2, 0f);
                    beepy3.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But there is something else to urgently resolve.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg02.png", 3, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(2)));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Nod has taken advantage of the collapse of the Government military and is pushing forward with full force.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg06.png", 4, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(3)));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Some remnants of the Government military are still fighting back, but others have surrendered and even joined the enemy.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg03.png", 5, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If we don't stop them quickly, it will be much harder to drive them back once they get to settle down with fortifications.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg05.png", 6, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Take a force and push back the Nod offense.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg02.png", 7, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If you proceed fast enough, you might reach elements of the Government military that are still fighting against Nod.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg03.png", 8, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(7)));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They can still have meaningful equipment and production capabilities that could help us.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg06.png", 9, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(8)));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Good luck.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg07.png", 10, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(9),
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(10).AlphaRate = -2.0f; toney4.Play(); }));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB12()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/gdilogo.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    newtarg1.Play();

                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                storyDisplay => { HideAllStoryImagesWithSound(storyDisplay, beepy3); TryPlaySong(chrg226m); },
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Are you picking this up?";
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...Good.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg02.png", 2, 0f);
                    beepy3.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Welcome to the ranks of the GDI.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg03.png", 3, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                storyDisplay => storyDisplay.FindStoryImageById(1).AlphaRate = 1.0f, // make GDI logo visible behind Sheppard again
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(2)));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We are still dealing with the aftermath of the First Tiberium War in the Balkans and Africa, and didn't really need another front at this time.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg04.png", 4, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(3)));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But we cannot allow your country to fall under control of Nod loyalists.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg05.png", 5, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We will provide you the tools to beat all of your adversaries, whether it's that slimy bastard Ivanov, or Nod, or the Communists.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg06.png", 6, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But first, you need to resolve the chaotic situation on the ground.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg02.png", 7, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We currently have no safe route to supply your forces, or even our own forces in the area.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg03.png", 8, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(7)));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The best option for creating such a route is conquering a beachhead held by the Government.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg04.png", 9, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                    AddTDDisplay(storyDisplay, 50);
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(8)));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "According to intel, Ivanov has constructed heavy defenses on the shore.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg05.png", 10, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                    AddTDDisplayImage(storyDisplay, "Story/CRB12/defenses.png", 51);
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(9)));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If you can figure out a way to knock out the power from their anti-air guns, our air support can open up a route.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg06.png", 11, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                    AddTDDisplayImage(storyDisplay, "Story/CRB12/power.png", 52);
                },
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(51),
                storyDisplay => storyDisplay.RemoveStoryImageById(10)));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If you succeed, we'll provide you some ships and a limited landing force.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg03.png", 12, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(52).AlphaRate = crTDdisplayImageDisappearAlphaRate; beepy2.Play(); },
                storyDisplay => { storyDisplay.RemoveStoryImageById(11); storyDisplay.RemoveStoryImageById(52); }));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Use them to build up a base and clear out all remaining resistance.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg06.png", 13, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                storyDisplay => { storyDisplay.RemoveStoryImageById(12); storyDisplay.RemoveStoryImageById(50); }));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Good luck, Commander.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg07.png", 14, 0f).AlphaRate = FACE_ANIM_ALPHA_RATE;
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(13),
                storyDisplay => { storyDisplay.FindStoryImageById(14).AlphaRate = -2.0f; toney4.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(14)));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, beepy3),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(17,
                storyDisplay => { storyDisplay.ConversationDisplay.ConversationText = "----------------"; TryPlaySong(tdmaptheme); },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Planning this operation, you figured that the best chance of success would come with a surprise attack.";
                },
                null,
                storyDisplay => beepy3.Play(),
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Send in a Commando. There is an agent in the Government ranks who can supply him with weapons stashed in a pre-arranged location.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg08.png", 20, 0f);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, beepy3),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Fetch the weapons, figure out a way to reach the Government power facility that powers the AA guns, and C4 it to dust.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/bg09.png", 21, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "Afterwards, the GDI reinforcements will have to be enough for whatever the illegitimate Government throws at you.",
                null,
                storyDisplay =>
                {
                    HideAllStoryImagesWithSound(storyDisplay, toney4);
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "------------------";
                    storyDisplay.AddSimpleStoryImage("Story/CRB12/crtitle_yellow.png", 11, 1f);
                },
                null));

            return phases;
        }

        private List<Phase> CRB11Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "While you had been busy commanding forces on the battlefield, GDI lead Sheppard had authorized the use of force against the Government.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/victorybg01.png", 1);
                    toney7.Play();
                    TryPlaySong(tdmaptheme);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Soon afterwards, he held a session with the press where he justified his decision.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov's Government had been repressing its own civilians and was guilty of war crimes in the conflict. GDI couldn't back his actions anymore.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI also reached out to you for cooperation.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg03.png", 1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "After some initial talks, you began planning your next joint operation for toppling the Government.";
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB11()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.ConversationDisplay.ConversationText = "After escaping the immediate combat zone, you managed to gather a significant force.";
                    toney7.Play();
                    TryPlaySong(chrg226m);
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg01.png", 0);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The lines between your forces and the Government's forces are getting clear.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unfortunately, most industrial facilities fell under Government control.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg02.png", 1);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI is yet to take a clear stance on your uprising.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg03.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They seem to take their time with evaluating their options.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Not supporting either side would lead into Communist, practically Nod, control of the country, which is a no-go for them.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg04.png", 3);
                },
                null,
                storyDisplay => storyDisplay.FindStoryImageById(3).AlphaRate = -1.5f,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(3);
                    storyDisplay.ConversationDisplay.ConversationText = "Supporting Ivanov would force GDI to commit more forces into the area, and with Ivanov's public reputation down the drain, it would be messy from the standpoint of GDI's reputation.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg05.png", 4);
                },
                null,
                storyDisplay => storyDisplay.FindStoryImageById(4).AlphaRate = -1.5f,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(4);
                    storyDisplay.ConversationDisplay.ConversationText = "Supporting you, on the other hand, would force GDI to commit even more forces, but without the reputation issue.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Whatever happens, you have no time to wait for GDI's decision.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The current control of industry allows Ivanov and Toikka to produce equipment much faster than you. If this conflict goes on for longer, you are set up to lose unless this changes.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg02.png", 5);
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The easiest way to fix the situation is to pay a revisit to and re-conquer a nearby industrial area.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This is further made easier by the leadership of a nearby Government outpost declaring loyalty to you.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg06.png", 5);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Rendezvous your forces with the outpost and take control of it. The outpost has no vehicle production capacity, but it allows you to train and arm infantry.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Afterwards, assault a nearby Government base that contains a War Factory. Capture it and use it to build an MCV, which will finally allow you to set up a proper base.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg07.png", 6);
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Proceed to build a strong force and eliminate all Government bases.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Make sure to also capture Oil Refineries in the area for stable income. The area has otherwise limited resources to harvest.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB11/bg08.png", 7);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "------------------";
                    HideAllStoryImagesWithSound(storyDisplay, toney4);
                    storyDisplay.AddSimpleStoryImage("Story/CR08/crtitle.png", 11);
                },
                null));

            return phases;
        }

        private List<Phase> CRB10Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/victorybg01.png", 1);
                    toney7.Play();
                    TryPlaySong(ramap);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Following the announcement of the uprising, Ivanov immediately dismissed the Commander as a traitor, and held a speech where he said he is certain that GDI will not believe their \"lies\".";
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg02.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "He started disarming the Commander's forces and transferring elements of them under the second-in-charge, Toikka. Many units loyal to the Commander, however, resisted.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/victorybg03.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "The chain of command quickly collapsed across the front, as there was no clear line of contact between units loyal to the rebellion and units loyal to Ivanov.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Instead, there were small unit groups facing each other all over the place, while also trying to hold off against the Communists and Nod, who started taking immediate advantage of the situation.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/victorybg04.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "In many bases, total chaos ensued, with brothers-in-arms, previously on the same side, now divided. At first, many refused to conduct hostilities.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "\"What, I'm supposed to shoot my buddies!? We were just fighting the Commies together a week ago!\" one interviewed Government-sided soldier shouted in disbelief.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/victorybg05.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "Ultimately, orders were orders, and at some point, there was no choice.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The different groups of units started organizing themselves, and control lines are slowly forming between forces of the Government and forces of the rebellion's commander.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg04.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "The Global Defense Initiative has taken no immediate stance to the situation.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They have only signaled both sides to limit the violence - a mandatory non-statement, as they could never say the opposite.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The situation does, however, certainly have a negative effect on GDI, as the weakening of the Government military in this internal conflict will give an upper hand to the Nod-backed Communists.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Time will tell how the GDI will respond. The Commander's claims of serious war crimes appear legitimate at least based on initial analysis, putting pressure on the GDI to side against Ivanov's Government.";
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB10()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.ConversationDisplay.ConversationText = "Consider all the elements that you must take into account for a successful uprising.";
                    toney7.Play();
                    TryPlaySong(raintro);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "First off, you need to have support from most of the military. They need to be loyal to you.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg01.png", 1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This is possibly covered well enough, considering your reputation as a skilled commander.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Secondly, you need to convince the civilians that you'd be a better bet than the current leadership, or they might resist you.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg02.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Due to your claims of Government war crimes, you cannot crush any resisting civilians - it'd hurt your credibility if you did the very thing you accuse the other side of.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To convince the civilians, you need to control the media. Most people only believe what the media tells them to believe, after all.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg03.png", 3);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Thirdly, because the war effort relies heavily on GDI support, you need to convince GDI that you're a better choice than Ivanov.";
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg04.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As an organization mostly headed by the democratic West, GDI can be pressured through the media.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg03.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "Thus, controlling the media is the key to improving your chances.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg05.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "Alongside performing further military operations to further improve your reputation as a commander, of course.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    TryPlaySong(chrg226m);
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "------------------";
                },
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "Two months later...",
                storyDisplay =>
                {
                    HideAllStoryImagesWithSound(storyDisplay, country1);
                    storyDisplay.ConversationDisplay.ConversationText = "------------------";
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg06.png", 6);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.ConversationDisplay.ConversationText = "Making preparations for the eventual clash against Ivanov took a while.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The battles against the neo-Soviets continued, and you made significant progress in winning the civil war.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The war is not over yet, however. But your preparations are ready, and you can wait no more.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg07.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "You have prepared an info package to be leaked into the international media, with communication excerpts that prove both the atrocities and that they happened by Ivanov's indirect order.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI will also hear this through the media, and the publicity will pressure them to help you in your uprising.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg08.png", 8);
                    storyDisplay.ConversationDisplay.ConversationText = "If successful, you are also going to seek a personal connection to the GDI leadership - particularly Supreme Commander Mark Jamison Sheppard.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg09.png", 9);
                    storyDisplay.ConversationDisplay.ConversationText = "Using military communication systems under your control, you are also going to spread the message to the military and civilian population of the country.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It is difficult to predict how many in the military will join your side, and how many of the civilians will trust you.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But this is the best shot you will ever have.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "------------------";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    TryPlaySong(fac2226m);
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg10.png", 10);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.ConversationDisplay.ConversationText = "You set your plans into motion.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The results were mixed. Many officers in the military praised you and declared loyalty to you.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, many others remained loyal to Ivanov.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB10/bg11.png", 11);
                    storyDisplay.ConversationDisplay.ConversationText = "You personally soon found yourself escaping from one smaller outpost that was loyal to you, but which was overtaken by forces loyal to Ivanov.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You drove away from the gates with your Mobile HQ, with some loyal units following you, and others guarding your back, giving their lives for you to survive...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You need to reach friendly territory ASAP.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "------------------";
                    HideAllStoryImagesWithSound(storyDisplay, toney4);
                    storyDisplay.AddSimpleStoryImage("Story/CR08/crtitle.png", 11);
                },
                null));

            return phases;
        }

        private List<Phase> CRB09Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.ConversationDisplay.ConversationText = "The communication data you gathered was vast.";
                    toney7.Play();
                    TryPlaySong(raintro);
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/victorybg01.png", 1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There was no quick way to go through it all. Dozens and dozens of pages of letters and reports...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But finally, after spending many sleepless nights on them, the situation seemed clear.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The highest military leadership had been told to neutralize any civilians that could potentially side with the Communist movement.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Especially civilians who seemed unlikely to change their mind to cooperate with Ivanov's Government were to be targeted.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg01.png", 2);
                    storyDisplay.ConversationDisplay.ConversationText = "Only Ivanov himself wields the power for giving this kind of order.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/victorybg03.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "Toikka had not executed this order very efficiently - which is not surprising from him.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/victorybg04.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "But what he had done had been to give his subordinates total freedom in looting and destroying the homes of any potential dissidents.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg06.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "Thus, while individual soldiers are involved, Ivanov and his Government can be seen as responsible for the slaughters.";
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRB09/victorybg05.png", 6),
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Your next steps will have to be prepared carefully.";
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRB09()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "----------------";
                    toney7.Play();
                    TryPlaySong(tdmaptheme);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg00.png", 0);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                    storyDisplay.ConversationDisplay.ConversationText = "Investigation into the committed atrocities was not going to be very straightforward.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Who could have information on it, and where - and how - could it be obtained?";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg01.png", 1);
                    storyDisplay.ConversationDisplay.ConversationText = "Even talking about it to the wrong person could be dangerous for you in case Ivanov truly is behind the massacres.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg02.png", 2);
                    storyDisplay.ConversationDisplay.ConversationText = "While you have been pondering this, the battles have kept raging on.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Government forces have made surprisingly little progress even after conquering the heavy vehicle factory.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg03.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "After some intelligence work, the main reason for this appears to be Nod's \"Ezekiel's Wheel\" tanks - stealth tanks that perform surgical strikes on bases, harvesters and vulnerable supplies.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg04.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "Another reason is surprisingly fast Tiberium growth on neo-Soviet territory, giving them more Tiberium to harvest, which the enemy can then sell for money or use for building equipment.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg05.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "And a third reason is that the Communists seem to get significant extra manpower in the form of mercenaries from other ex-Soviet countries. With them also controlling the eastern state border, you are unable to stop the flow.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg06.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "You were often discussing these three factors with Ivanov at the Government military HQ, which, together with your most loyal contacts, allowed you to do some careful research into internal Government messaging.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg07.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "There is an archive of communications - both physical and digital mail, and reports - between many high-ranking Government officials that is kept on a server located in a Government Tech Center.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Tech Center itself is in a large Government base that is situated far from the frontline.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Infiltrating it will hopefully answer your questions.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CRB09/bg08.png", 8);
                    storyDisplay.ConversationDisplay.ConversationText = "Getting there yourself could raise suspicions, so it is safer to send a Spy.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There is little room for failure. You need the Spy to avoid patrols, infiltrate the Tech Center, fetch the documents from the archive, and escape.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This is your best chance at looking into the mystery. If the Spy gets caught, security levels will surely be raised within the whole Government-controlled territory.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "------------------";
                    HideAllStoryImagesWithSound(storyDisplay, toney4);
                    storyDisplay.AddSimpleStoryImage("Story/CR08/crtitle.png", 9);
                },
                null));

            return phases;
        }

        private List<Phase> CRA14Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "After a heavy battle with lots of casualties on both sides, the Government forces finally destroyed the Nod base, pushing Nod out from the country.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Government, supported by GDI, achieved their final victory over the neo-Soviets and the Brotherhood of Nod.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Communist leadership was nowhere to be seen.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/victorybg02.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "A source claiming to be within the Government, however, leaked information that they were executed on the spot after the battle.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Government promised to start rebuilding efforts immediately.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/victorybg03.png", 3);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As part of it, a program was launched to cleanse the recaptured territory of Tiberium.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/victorybg04.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Progress, however, was slower than expected.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While some people were suffering from the spread of Tiberium and poisoning of their neighbourhoods, others figured that Tiberium harvesting would make a good source of income for the poor country - and, with it, also for themselves.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI pressure, however, made sure that despite the slow pace of the cleanup, there still was continuous progress.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            var coCommandersForcesSavedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_A_CO_COMMANDER_FORCES_SAVED");
            if (coCommandersForcesSavedVariable != null && coCommandersForcesSavedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "There was a victory parade held in the capital of the country.";
                        storyDisplay.AddSimpleStoryImage("Story/CRA14/ending1.png", 5);
                        TryPlaySong(secondhand);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(10,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Despite that the Government had two Commanders in the war, only one was present in the ceremony.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(11,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Toikka was watching his soldiers march, while the other Commander had been dismissed for \"fatal tactical mistakes\" soon after the victory.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(12,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The reason is not clear, but some international observers suspect that Ivanov saw the other Commander's great popularity within the soldiers as a risk for his leadership.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(13,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Many lower-level officers report being shocked at the decision, but most likely, it will pass.";
                        storyDisplay.AddSimpleStoryImage("Story/CRA14/ending2.png", 6);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(14,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "After a heavy civil war, the people would not support any officers who attempted to start another one.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(15,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "In this system of corruption, no good deed goes unpunished.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "";
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(16,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.TextColor = Color.White;
                        storyDisplay.AddSimpleStoryImage("Story/CRA14/endingtitle.png", 7);
                        storyDisplay.ConversationDisplay.ConversationText = "  ENDING II" + Environment.NewLine + "Dismissed";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "";
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                        StopMusic();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }
            else
            {
                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        TryPlaySong(hellmarch);
                        storyDisplay.AddSimpleStoryImage("Story/CRA14/ending1.png", 5);
                        storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                        storyDisplay.ConversationDisplay.ConversationText = "You stood at the victory parade next to Toikka.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(10,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Both of you Commanders were awarded for your service in liberating the country.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(11,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Thousands of people were marching to tone to celebrate your success...";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(12,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "To celebrate the deaths of thousands of enemies...";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(13,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It's a great feeling, isn't it?";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "";
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(14,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.TextColor = Color.White;
                        storyDisplay.AddSimpleStoryImage("Story/CRA14/endingtitle.png", 6);
                        storyDisplay.ConversationDisplay.ConversationText = "           ENDING I" + Environment.NewLine + "Commander of the Government";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "";
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                        StopMusic();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            AddCreditPhases(phases, new Color(20, 255, 157));
            AddPostRouteAHintPhases(phases);

            return phases;
        }

        private List<Phase> CRA14()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "ON GOVERNMENT MILITARY LEADERSHIP";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(chrg226m);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "After independence from the USSR, the country had practically no military.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/bg02.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The old military had been destroyed by the Allies, its power structure was no longer deemed relevant, and new defense forces had to be built from scratch.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The process was chaotic in many ways. Partially the force was seen as purposeless and harmful due to the recent Allied victory. The Allies didn't want the parts of the ex-USSR to have a strong military.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/bg03.png", 3);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Some officers had a Soviet legacy which they upheld, while others were trying to learn new doctrines by analyzing how the Allies had won the war.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "These parts were often clashing with each other, and the military was crippled by corruption.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov's rise to power did not solve this issue. Eventually, he appointed Toikka as Commander of the defense forces.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/bg04.png", 4);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Toikka was not seen as particularly skilled, being a fairly generic officer in the military's ranks.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/bg05.png", 5);
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "He had mostly stayed neutral in the doctrinal clash between the eastern and western styles of waging war, which means he was widely accepted - considered \"good enough\", but not optimal, by both sides.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "His greatest benefit was his absolute loyalty. Ivanov gave him his own, personally beneficial position, which Toikka was happy with. He did not aspire to challenge Ivanov for leadership of the country.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/bg06.png", 6);
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Instead, he used brutal methods to crush any armed opposition, keeping his superior pleased with him.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "When the civil war broke out, Toikka's leadership proved too inefficient to quash it. He couldn't divide his attention among multiple fronts.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/bg07.png", 7);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As threat of actual revolution and change of power seemed significant, Ivanov decided to assign another Commander to handle roughly half of the battles.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The new Commander quickly started performing better than Toikka, creating a competitive environment between the Commanders within the Government's military.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA14/bg08.png", 8);
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It remains to be seen whether this \"rising star\" will replace Toikka, or whether they will survive in this system built around not stepping on anyone's toes.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 9, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(terminat);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Our final victory is at hand.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/officebg01.png", 10, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Only a single Nod base remains on our territory.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "According to our intelligence data, the leaders of the Communist scum are also situated there.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The cowards had supposedly tried to escape farther away to Nod-controlled or gray territory outside of our country.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, high-ranking Nod officers are disappointed with their performance and are forcing the Commies to fight to their bitter end.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "And that bitter end is something we'll be glad to give them.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Go in there and destroy the Nod base. As for the so-called leadership of the Soviets... a quick, dirty, secret field tribunal will do.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Oh, how I'd love to hang or maybe just have those bastards shot publicly... but GDI would disapprove.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Speaking of GDI, they have warned us about Nod potentially having new toys, developed from the technology they have taken from us, the GDI, and the Soviets.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I trust you can deal with whatever they throw at you.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "See you at the victory parade, Commander!";
                },
                null,
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Wait... one more thing...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We still haven't found those Construction Yards that Nod stole from us and the GDI.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If you happen spot any Nod buildings of special interest, be sure to capture them. Maybe their archives will contain some information.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(10).AlphaRate = -2.0f; toney4.Play(); }));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRA13Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With the border back in Government control, the situation is now clear.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA13/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Only one area with enemy forces remains. It is the main base that Nod has set up on the territory of the Government.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Our next operation, a direct assault there, will end this conflict once and for all.";
                },
                null,
                null,
                null));

            var coCommandersForcesSavedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_A_CO_COMMANDER_FORCES_SAVED");
            if (coCommandersForcesSavedVariable != null && coCommandersForcesSavedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        TryPlaySong(secondhand);
                        storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                        storyDisplay.AddSimpleStoryImage("Story/CRA13/victorybg02.png", 2);
                        storyDisplay.ConversationDisplay.ConversationText = "Toikka has been avoiding you after your victory.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(1)));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Usually he has congratulated some time after your successes, but this time, you haven't received a word from him.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "While saving his troops was clearly the right choice tactically and morally, he seems unhappy with his spreading reputation of being less competent than you.";
                    },
                    null,
                    null,
                    null));
            }

            return phases;
        }

        private List<Phase> CRA13()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRA13/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(chrg226m);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "The joint Government-GDI covert operation on Karhumäki took the Communists by surprise.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA13/bg02.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With their main support hub fallen, their frontlines quickly collapsed.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Many soldiers routed and hastily abandoned their positions, even leaving behind their equipment. Others surrendered upon hearing the news.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA13/bg03.png", 3);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Communist and Nod forces together only have two bases left in the Government's territory.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA13/bg04.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The end of the war is in sight.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 5, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(terminat);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Well, that went perfectly according to plan.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/officebg01.png", 6, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Now we just need to push the Commies out from the border, and then deal with the last Nod base.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 7)));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The enemy is receiving mercenaries, Russian Communists sympathetic with our enemy, through the border area.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CRA13/borderriver.png", 8);
                    storyDisplay.ConversationDisplay.ConversationText = "They have been training and giving weapons to this scum and then throwing them against us.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Take a force, attack the border sector, and destroy the bridges and barracks on the border river.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Toikka's forces are also there, although they've stumbled into resistance.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(8).AlphaRate = -2.0f; bleep12.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(8),
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Let's see if you will do any better.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(7).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(6).AlphaRate = -2.0f; toney4.Play(); }));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRA12Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With the victory at Karhumäki, the neo-Soviets' frontline collapsed.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA12/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "A military parade was held at the city by the Government. Officially, the local populace was said to be excited about the liberation, but there were also many thinly veiled signs of disapproval.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The star of the show was the Government's new Commander, who had achieved yet another great victory.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Among the ranks of Government military officers, they are widely seen as the most competent Commander in the Government's ranks.";
                },
                null,
                null,
                null));

            var nodCityBaseDestroyedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ROUTE_A_NOD_CITY_BASE_DESTROYED");
            if (nodCityBaseDestroyedVariable != null && !nodCityBaseDestroyedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Mixed GDI and Government forces spent some time pushing Nod out of the city. Reportedly, Nod forces only offered light resistance, and soon retreated to the countryside.";
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The only area of the republic that remains under Communist control is the border region, from where companies of Russian mercenaries are still arriving to support the Communist side.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There is also a significant Nod base to the north of the city.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov, head of the republic, assures that the fighting will continue until the whole country has been liberated.";
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRA12()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 0, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(vector1a);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "What an amazing victory you've brought to us. The Commies are really stretching themselves at the frontline after losing so much equipment.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/officebg01.png", 1, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They have left their main city underdefended. This has given us a golden opportunity to strike and capture it, without needing to level it in heavy urban combat.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Even though that could also be a sight to see, especially if anyone else has intentions to challenge us in the future...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Anyway.\r\n\r\nWe have carefully devised a plan for a multi-stage operation for taking the town. It is complex, but realistic.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 2)));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CRA12/shoreline.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "Take control of a group of special forces that we will bring in at night by helicopter.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There is a contact, a bus driver who is loyal to us. He is waiting for your forces and he has offered his services to us. Board his bus and he will take the troops near a civilian radar in the center of the city.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CRA12/radar.png", 4),
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(3)));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Capture the radar, which will allow us to push our own propaganda on the civilians and confuse the enemy's communications.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Once done, we will send in another batch of special forces. There is a railway that we want access into, but it is defended by the enemy.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CRA12/railway.png", 5),
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Assault the railway defenses with your new batch of special forces and destroy the SAM sites. GDI will follow with A-10s to take out their Tesla Coils.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "That will allow us to deliver a batch of vehicles to the city center by train.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CRA12/trains.png", 6),
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Proceed to take control of the town. Destroy the enemy's command centers and capture the City Council building.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CRA12/citycouncil.png", 7),
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I will be preparing the parade for our victory.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(7).AlphaRate = -2.0f; bleep12.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(7),
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I trust you to make sure my work does not go to waste.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(1).AlphaRate = -2.0f; toney4.Play(); }));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRA11Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Government forces, aided by a large shipment of GDI vehicles, have achieved yet another victory.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA11/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, Nod and the neo-Soviets had brought up surprisingly many reinforcements to the area, which had made the operation more difficult than anticipated.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Upon strategic inspection at the Government HQ, it appears that the Communists had likely transferred forces away from their most significant urban area at Karhumäki.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Its surrounding countryside still holds significant enemy forces, but this likely opens up a possibility for a successful flank or a sneak attack on the town.";
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRA11()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "DAILY SUN TIMES - CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRA11/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(chrg226m);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Just some time prior, in the winter, it looked like the war was coming to a standstill.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA11/bg02.png", 2);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Yet, thanks to GDI sensor technology, tactical successes and successful pressure on Communist Tiberium harvesting operations from the Government side, the power balance has shifted to the Government's favor.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA11/bg03.png", 3);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Brotherhood of Nod is reportedly starting to consider the war a lost cause and is reducing support for the neo-Soviets.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA11/bg04.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "An anonymous Nod senior officer has revealed that the main aim of their operation is no longer to allow the Communist side to win, but to drain as much resources from GDI as possible.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Meanwhile, the GDI has kept up its support for the Government.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA11/bg05.png", 5);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In latest talks between GDI head Sheppard and President Ivanov, GDI assured they will keep sending weaponry and forces to help the Government until military victory.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It appears likely the Communists will be defeated, ensuring a period of peace and stability once Nod has suffered the expected global defeat in Europe and Africa.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 6, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(terminat);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Greetings.";
                    storyDisplay.ConversationDisplay.TextColor = Color.LightBlue;
                    storyDisplay.AddSimpleStoryImage("Story/CRA11/bg06.png", 7, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov thinks we have an opportunity to assault the enemy's main city soon, but first we need to take more of the surrounding countryside. At least as a distraction if nothing else.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The GDI has given us a fresh batch of vehicles. Hum-vees, Medium tanks... all the good stuff a commander could ever wish for.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There is an area where the Soviets have fortified both sides of a river. With these GDI weapon supplies, you can launch a direct attack on both sides of the river at once.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Take out the enemy's outposts and proceed to take the territory over with MCVs.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Afterwards, build your bases, expand to new resource fields, construct forces... you know what to do. Continue eastwards and drive over the Commies.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Goodbye.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(7).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRA10()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 0, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(vector1a);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Toikka has been making progress on the frontline again with the help of the Sensor Arrays that have prevented Nod flanking attacks.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/officebg01.png", 1, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Now we just need to push the enemy, destroy them and finish the job.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 2)));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CRA10/map.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "A small city, Karhumäki, has become the primary support hub of the Communist scum. Aside from that, they mostly control some surrounding countryside.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CRA10/arrows.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "They are still also receiving major support from Russian neo-communists who have taken inspiration and are crossing our border, joining the enemy as mercenaries.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI is shifting resources to counter them to make sure there won't be a larger Communist uprising in Russia, so we might have to rely mostly on our own resources in these final fights.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Considering your latest mission, the Communist scum have allowed Nod to poison the countryside - the very land they say they are \"liberating\" - with Tiberium to fund their war effort.";
                    AddRADisplayImage(storyDisplay, "Story/CRA10/tiberium.png", 5);
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To speed up our victory, I need you to set up a new base on a flank behind the frontline to disturb the enemy's Tiberium harvesting operations.";
                    AddRADisplayImage(storyDisplay, "Story/CRA10/shore.png", 6);
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(3);
                    storyDisplay.RemoveStoryImageById(4);
                    storyDisplay.RemoveStoryImageById(5);
                },
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Build defenses and hold the position against the enemy's counterattack. That is all that is required - it will open up another front and give us a great position to launch further operations from in the future.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(6).AlphaRate = -2.0f; bleep12.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(6),
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The enemy is not expecting this move, which should give you some time for setting up your base before they detect your presence. Use the time to establish a good foothold on the shore.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I'll soon be off at negotiations with GDI for some days. I'll be awaiting news of your victory.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(1).AlphaRate = -2.0f; toney4.Play(); }));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRA09Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Thanks to your work, the sensor arrays escaped the area and are now usable in future operations.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, with the earlier capture of a GDI base and now a Government base, Nod could acquire significant technology from them.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It remains to be seen how this will affect the rest of the war.";
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CRA09()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "DAILY SUN TIMES - CONFLICT NEWS";
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(chrg226m);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "The combined Government and GDI forces had great success at the battle for the vehicle factory a few weeks ago.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/bg02.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, while it was expected to be a strategic success, the impact has remained mostly tactical and final victory for the Allies still seems uncertain.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The main reason appears to be Nod's \"Ezekiel's Wheel\" tanks - stealth tanks that perform surgical strikes on bases, harvesters and vulnerable supplies, and then disappear before the Government military can catch them.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/bg03.png", 3);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This leaves Allied frontline units starved for supplies and reinforcements, making defending against them easy and greatly increasing casualties for the Government and GDI.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/bg04.png", 4);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In the meanwhile, Nod and the Communist rebels appear to be working on ramping up production on their remaining territory.";
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRA09/bg05.png", 5),
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Satellite comparison images taken from the area have revealed a rapid increase of Tiberium growth ever since the start of the war. This development has been noted before, but the reason for it has not been known.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/bg06.png", 6);
                    mapwipe5.Play();
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CRA09/bg07.png", 7),
                storyDisplay => { storyDisplay.RemoveStoryImageById(5); storyDisplay.RemoveStoryImageById(6); },
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1)));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, lately fully-grown areas seem to have been harvested for resources.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/bg08.png", 8);
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Hence, the Government forces might be pressured against time to make advances before Nod manages to fully rebuild the supply line with Tiberium-based resources.";
                },
                null,
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1)));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Anonymous GDI officers have also told the press that the neo-Soviets seem to have surprisingly high amounts of manpower.";
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/bg09.png", 8);
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With their low population base in the country, it was expected that they'd have a shortage of soldiers by now.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However there is none of that in sight. It is suspected that the neo-Communists are being aided by significant numbers of mercenaries from other ex-Soviet countries.";
                },
                null,
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1)));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This trifecta of Stealth tanks, increased Tiberium harvesting and foreign reinforcements threatens to turn the tide of the war.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 10, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(vector1a);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "As you've surely heard from the news, these Kane's wheels are humiliating our forces.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CRA09/officebg01.png", 11, 0f);
                    country1.Play();
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 12)));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Thankfully GDI has given us a solution. Their Mobile Sensor Arrays can be escorted with other forces and deployed anywhere on the battlefield, and they can sense the Stealth tanks from afar.";
                    AddRADisplayImage(storyDisplay, "Story/CRA09/sensorarrays.png", 13);
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They've so far gifted an initial batch of three of these vehicles to us, and we're transferring them to the frontline. If we can demonstrate effective use of them, we might get more of them in the future.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We need your forces to escort these three vehicles.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(13).AlphaRate = -2.0f; bleep12.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(13)));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Make sure they get to our base safely.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(12).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                storyDisplay => { storyDisplay.FindStoryImageById(11).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CR08Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The fresh star of the Government's military leadership has once again brought victory.";
                    storyDisplay.AddSimpleStoryImage("Story/CR08/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The neo-Communists and the Brotherhood have been driven away from the damaged factory, and it shall never again strengthen the enemy's ranks.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This victory sets the stage for further weakening of the enemy and the eventual final victory over them.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            var genocideVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_VILLAGE_DESTROYED");
            if (genocideVariable != null && genocideVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Meanwhile, the Global Defense Initiative has made progress in its research into the allegations of war crimes performed by the Government.";
                        storyDisplay.AddSimpleStoryImage("Story/CR08/victorybg03.png", 2);
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "GDI has determined there to be truth to the claims. As a result, they have decided to halt their military support for the Government until the Government takes serious steps to reverse their course.";
                        storyDisplay.AddSimpleStoryImage("Story/CR08/victorybg04.png", 3);
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, toney4),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/CR08/crtitle.png", 3);
                        storyDisplay.ConversationDisplay.TextColor = Color.White;
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "You have unlocked CR Route C.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, null),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Be warned that the lack of GDI support is going make your life very difficult.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Unless you are up for a serious challenge, you might want to reconsider some of your previous choices.";
                    },
                    null,
                    null,
                    null));
            }
            else
            {
                var massGravesFoundVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_MASS_GRAVES_FOUND");
                if (massGravesFoundVariable != null && massGravesFoundVariable.EnabledThroughPreviousScenario)
                {
                    phases.Add(new Phase(3,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.IsCentered = false;
                            TryPlaySong(secondhand);
                            storyDisplay.AddSimpleStoryImage("Story/CR08/victorybg02.png", 2);
                            storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                            storyDisplay.ConversationDisplay.ConversationText = "The civilian bodies that your units discovered match with the accusations that the civilians you saved earlier made towards the Government.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(4,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "It is unknown whether the Supreme Leader himself had ordered these attacks, or whether it was someone down on the chain of command.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(5,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "You could ask Ivanov about it, but there's a chance that'd go rather wrong...";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(6,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                            storyDisplay.AddSimpleStoryImage("Story/CR08/bg04.png", 3);
                            storyDisplay.ConversationDisplay.ConversationText = "... civilian bodies, huh?";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(7,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "What's wrong with that? They collaborated with the enemy, worked on the factory controlled by the Communist scum.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(8,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Have you grown a sweet spot for the enemy, Commander?";
                        },
                        null,
                        storyDisplay => storyDisplay.FindStoryImageById(3).AlphaRate = -1.0f,
                        storyDisplay => storyDisplay.RemoveStoryImageById(3)));


                    phases.Add(new Phase(9,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                            storyDisplay.ConversationDisplay.ConversationText = "...";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(10,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "If you want to pursue this, it might be better to do some investigation on your own first.";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, toney4),
                        storyDisplay => storyDisplay.ClearStoryImages()));

                    phases.Add(new Phase(11,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CR08/crtitle.png", 3);
                            storyDisplay.ConversationDisplay.TextColor = Color.White;
                            storyDisplay.ConversationDisplay.IsCentered = true;
                            storyDisplay.ConversationDisplay.ConversationText = "[ Proceed to CR Route A to continue fighting loyally on the Government side. ]" + Environment.NewLine +
                                "[ Alternatively, proceed to CR Route B to investigate the murders of the civilians. ]";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, null),
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
                else
                {
                    phases.Add(new Phase(3,
                        storyDisplay =>
                        {
                            storyDisplay.AddSimpleStoryImage("Story/CR08/crtitle.png", 3);
                            storyDisplay.ConversationDisplay.TextColor = Color.White;
                            storyDisplay.ConversationDisplay.IsCentered = true;
                            storyDisplay.ConversationDisplay.ConversationText = "You have unlocked CR Route A!";
                        },
                        null,
                        storyDisplay => HideAllStoryImagesWithSound(storyDisplay, null),
                        storyDisplay => storyDisplay.ClearStoryImages()));
                }
            }

            return phases;
        }

        private List<Phase> CR08()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "POST-SOVIET HISTORY";
                    storyDisplay.AddSimpleStoryImage("Story/CR08/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(ramap);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "When the Soviet Union ceased to exist, many countries and territories previously dominated by it and its predecessor, the Russian Empire, gained independence.";
                    storyDisplay.AddSimpleStoryImage("Story/CR08/bg02.png", 2);
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Allies wanted the country to be weak enough that Russia could never launch a full-scale invasion of Europe again.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The death of Stalin, liquidation of Soviet war criminals and the emergence of various smaller states required new, previously mostly unknown people to step up and take lead of the countries.";
                    storyDisplay.AddSimpleStoryImage("Story/CR08/bg03.png", 3);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ivanov, born in 1963, achieved this through posing as a modern post-Soviet leader aspiring liberating the society and healthy relations with the West and the newly-independent Eastern European countries.";
                    storyDisplay.AddSimpleStoryImage("Story/CR08/bg04.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, over time, his economic policy has shown to lack substance, the corruption of the Soviet system has not gone away, and his popularity has waned.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But any serious opposition showing up has quickly got tangled in corruption scandals. Certain international observers have speculated some of the charges to be forged, but there has been little pressure for Ivanov to change course.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI has been happy with him keeping a hard line on neo-Communism and Russian influence, and due to him having no serious competition, he has been able to - at least officially - win all elections so far.";
                    storyDisplay.AddSimpleStoryImage("Story/CR08/bg05.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 6, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(terminat);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Greetings!";
                    storyDisplay.ConversationDisplay.TextColor = Color.LightBlue;
                    storyDisplay.AddSimpleStoryImage("Story/CR08/bg06.png", 7, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "My name is Toikka." + Environment.NewLine + Environment.NewLine + "Commander Toikka.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Supreme Commander himself was too busy, so he told me to convey this assignment to you.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We've been storming this giant vehicle production factory with GDI for almost a month here now.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It was a painful loss earlier in the war, and the Noddies have fortified it so that not even GDI air power has managed to blow the place up.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They are pumping out absolutely crazy amounts of Mammoths at us! Casualties are heavy on both sides, but at this rate, we're going to lose it.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We need you to grab control of the Enforcer, and since the local waterways are now frozen, use the cover of the night to bring the Enforcer over the ice onto a bridgehead that the enemy needs for supplying the factory.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Deploy the Enforcer there and prevent the commies from getting supplies across to stop the crushing flow of heavy tanks.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Afterwards, you'll get an MCV so you can build a base, gather forces, destroy the Commies and Noddy scum and save the day.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Sounds like just the perfect job for you, isn't that right?";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "See you at victory celebration at the HQ!" + Environment.NewLine + Environment.NewLine + "Looking forward to drinking with you once this is all over.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(7).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CR07Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Communist forces were caught by surprise. They were unprepared for a quick attack on the archipelago and lost it to a relatively small Government and GDI force.";
                    storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The archipelago had worked as a troop and equipment transportation route, and when frozen, would've allowed tanks to drive through the area and attack the Government shoreline.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The information gathered from the command center revealed that industry under Communist and Nod controlled territory is producing surprisingly large numbers of military equipment.";
                    storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg02.png", 1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "For that, the industry must have access to more raw materials than has earlier been possible.";
                    storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg02.png", 1);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "An anonymous source from GDI intelligence has suspected that Nod is providing them with cheap Tiberium-extracted minerals.";
                    storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg03.png", 1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To achieve a lower price for the minerals, a deal with Nod is suspected where Nod gives the tiberium-extracted minerals at a discount to enterprises that build military equipment for them or the Communists.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It is necessary for the Government and GDI to strike the manufacturing sites or cut the source of Tiberium minerals if they want to reduce the number of enemy vehicles.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));


            var genocideVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ALL_CHURCHES_DESTROYED");
            if (genocideVariable != null && genocideVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        TryPlaySong(chrg226m);
                        storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg05.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "Separately from military matters, there are reports that Government forces have brutally destroyed civilian villages loyal to the Communists.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Ivanov has denied the claims, saying that only military forces are targeted. Civilians, especially collaborators near Communist military assets, might suffer from limited collateral damage, however.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "According to an anonymous source, GDI has launched an internal investigation into the matter.";
                    },
                    null,
                    null,
                    null));    
            }
            else
            {
                var civiliansEvacuatedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_CIVILIANS_EVACUATED");
                if (civiliansEvacuatedVariable != null && civiliansEvacuatedVariable.EnabledThroughPreviousScenario)
                {
                    phases.Add(new Phase(6,
                        storyDisplay =>
                        {
                            TryPlaySong(secondhand);
                            storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg04.png", 1);
                            storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                            storyDisplay.ConversationDisplay.ConversationText = "A sergeant under your command has brought you a message from the civilians that you evacuated.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(7,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "They claim that Government forces under other commanders' command have committed attacks on civilian villages, killing and torturing entire villages if they've suspected collaboration with the enemy.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(8,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "According to them, this has also eroded public support for the Government and GDI, and increased support for the Communists and Nod. Many people who were loyal to the Government have defected due to this.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(9,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "One of the affected villages is supposedly located near a vehicle manufacturing plant that the Government forces and GDI have been assaulting recently.";
                        },
                        null,
                        null,
                        null));

                    phases.Add(new Phase(10,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "If your forces participate in combat there in the future, it could give you a chance to check whether these claims hold truth.";
                        },
                        null,
                        null,
                        null));
                }
            }

            return phases;
        }

        private List<Phase> CR07()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(secondhand);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Well. We didn't expect the Commies to have nukes.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR07/officebg01.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We don't know where they got them from, whether they're of Nod stock or smuggled from ex-USSR stock.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We could respond with our own, but as much as I'd love to, the joy wouldn't be worth losing GDI's support. And a few tactical nukes won't change the tide of the war for the scum. It might even increase GDI support for us.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The nukes, however, are not the only surprising aspect. We've made calculations about the Commies' equipment and manpower and most of their forces should be either fertilizing the ground or severely crippled already.";
                },
                storyDisplay => AddRADisplay(storyDisplay, 3),
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CR07/charts.png", 4),
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But that is not what we are seeing on the frontlines.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They still have an adequate level of tanks, with no shortage of equipment in sight.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CR07/tanks.png", 5),
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With the shoreline in our control and the water not completely frozen yet, we are now able to launch a limited naval assault on the archipelago they control.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Instead of a direct assault, however, we want to infiltrate their command and figure out where they are getting their resources from.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CR07/commandcenter.png", 6),
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We know that this archipelago has been a significant delivery route of both vehicles and manpower for them, and with our quick progress they possibly haven't been able to eradicate all traces of it yet.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But there's no further time to waste. When the night falls, infiltrate their forward command center with a Spy. Our advisors will analyze any data that you can get out, and will give you further orders depending on the outcome.";
                    storyDisplay.FindStoryImageById(6).AlphaRate = -2.0f;
                    bleep12.Play();
                },
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CR06()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "CONFLICT NEWS AND OPINION";
                    storyDisplay.AddSimpleStoryImage("Story/CR06/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(ramap);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            const int factionLogoMargin = 30;

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "With the earlier chemical attack on the Government forces, a tactical bait and subsequent destruction of a Global Defense Initiative base, Nod entered the civil war with a strong show.";
                    storyDisplay.AddSimpleStoryImage("Story/CR06/bg02.png", 2, 0f);
                },
                null,
                storyDisplay =>
                {
                    var nodLogoImage = new StoryImage(windowManager, 3);
                    nodLogoImage.Alpha = 0.0f;
                    nodLogoImage.Texture = AssetLoader.LoadTextureUncached("Story/CR06/nod.png");
                    nodLogoImage.ImageX.SnapToValue(UIDesignConstants.CUTSCENE_DESIGN_RES_X - nodLogoImage.Texture.Width - factionLogoMargin);
                    nodLogoImage.ImageY.SnapToValue(UIDesignConstants.CUTSCENE_DESIGN_RES_Y - nodLogoImage.Texture.Height - factionLogoMargin);
                    storyDisplay.AddStoryImage(nodLogoImage);
                    bleep11.Play();
                },
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As a response, GDI has massively increased support for the Government and is bringing significantly more forces to the theater.";
                },
                null,
                storyDisplay =>
                {
                    var gdiLogoImage = new StoryImage(windowManager, 4);
                    gdiLogoImage.Alpha = 0.0f;
                    gdiLogoImage.Texture = AssetLoader.LoadTextureUncached("Story/CR06/gdi.png");
                    gdiLogoImage.ImageX.SnapToValue(factionLogoMargin);
                    gdiLogoImage.ImageY.SnapToValue(factionLogoMargin);
                    storyDisplay.AddStoryImage(gdiLogoImage);
                    bleep11.Play();
                },
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With these steps, the war is seen to have evolved from a civil war to another theater of the full-blown war between GDI and Nod, which was thought to be nearing its end.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Numerically, most forces in the area are still from the local factions, but GDI and Nod are far more resourceful.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The local population has expressed fear of the escalation and the destruction it will bring.";
                    storyDisplay.AddSimpleStoryImage("Story/CR06/bg03.png", 5, 0f);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Government has taken full benefit of Nod involvement in their propaganda, portraying the Communist side as foreign Nod forces who are fighting to give control of the country to a foreign power.";
                    storyDisplay.AddSimpleStoryImage("Story/CR06/soviet.png", 6, 0f);
                },
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(6).AlphaRate = -1.0f;
                    storyDisplay.AddSimpleStoryImage("Story/CR06/nod.png", 7, 0f);
                    mapwipe5.Play();
                },
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Likewise, the Communist side is claiming that the Government is surrendering the country's independence to the Western powers who are the biggest backers of GDI.";
                    storyDisplay.AddSimpleStoryImage("Story/CR06/logo.png", 8, 0f);
                },
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(8).AlphaRate = -1.0f;
                    storyDisplay.AddSimpleStoryImage("Story/CR06/gdi.png", 9, 0f);
                    mapwipe5.Play();
                },
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Currently there is no end in sight to the war, and the sides appear to be relatively well matched. Both suffered heavy casualties in the battles for the GDI base area.";
                    storyDisplay.AddSimpleStoryImage("Story/CR06/bg04.png", 10, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Theoretically the Nod and Communist side should be weaker, but practically they have proven to be much better resourced than expected. GDI and Government officials are reportedly confused how this is possible.";
                    storyDisplay.AddSimpleStoryImage("Story/CR06/bg04.png", 10, 0f);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(6);
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -2.5f);
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(secondhand);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "We were discussing the appearance of enemy naval forces with GDI, and found out that our enemy has expanded into some of our islands.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR06/officebg01.png", 2, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We'd love to go after them, but as-is it'd be too risky. We need to liberate more of the shoreline first.";
                },
                null,
                storyDisplay => AddRADisplay(storyDisplay, 3),
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CR06/base.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "We have a base in the region, but we are unable to reach it. We suspect that it is under enemy assault and their communications are damaged, but the base has not completely fallen yet.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CR06/route.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "Our scouts are also reporting that we still have one safe supply route to the base.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Reinforce the base through the safe route, bring the base communications back online and destroy enemy forces to the east of our base. Only with the enemy gone from the shore, can we prepare a landing force against the islands.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                storyDisplay => { storyDisplay.FindStoryImageById(5).AlphaRate = -2.0f; bleep12.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unlike in the previous operation, we don't have many resources to spare this time. GDI has lost muscle too, as has the enemy.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The area itself also has limited resources to harvest. I suggest you attack the enemy aggressively and take them out quickly, before you run out of ore and tiberium to collect. But you surely know this too.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I'll be awaiting news of your victory.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CR05Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Prior to the latest operation, there was widespread ridicule directed at the Government military and GDI.";
                    storyDisplay.AddSimpleStoryImage("Story/CR05/victorybg01.png", 1);
                    country4.Play();
                    TryPlaySong(raintro);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Poster and other activity by Communist partisans had greatly increased, mocking the sudden loss of the GDI and Government shoreline bases.";
                    storyDisplay.AddSimpleStoryImage("Story/CR05/victorybg02.png", 1);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, now it has all quieted down due to the unexpectedly successful Government counterattack, supported by the full fury of the Eagle. The new Government Commander is also gaining widespread popularity among their troops.";
                    storyDisplay.AddSimpleStoryImage("Story/CR05/victorybg01.png", 1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Only a rumor exists that Nod had captured major GDI technology during the operation. GDI has refused to comment on this rumor, citing operation security as the reason for the silence.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            var globalVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ALL_CHURCHES_DESTROYED");
            if (globalVariable != null && globalVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "There is also another claim that the Government forces have heavily attacked civilians, including destroying churches and other village buildings and organizing punishmental killings while dealing with Nod collaborators.";
                        storyDisplay.AddSimpleStoryImage("Story/CR05/victorybg03.png", 1);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The Government has denied these claims, stating that they only deal with collaborators and even those are tried in court. No regular civilian needs to be afraid of their forces.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            return phases;
        }

        private List<Phase> CR05()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(secondhand);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Despite your victory, it looks like allocating our forces to the supply route was a big mistake.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 2, 0f);
                },
                null,
                storyDisplay => AddRADisplay(storyDisplay, 3),
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There is a Tiberium-infested river basin where GDI had a Tiberium research and containment facility, which they were transforming into a base to counter Nod.";
                },
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CR05/tibriver.png", 4),
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "At the height of their incompetence, GDI didn't pay enough attention that Nod had noticed their efforts and was preparing a strike.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CR05/nodbase.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "With our primary force elsewhere, Nod captured the GDI base and, together with the Soviet scum, they are now consolidating their control over the area.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It's a real meat grinder there, with us and GDI as well as the enemies constantly throwing in reinforcements. We still have an outpost there, but it's only a matter of time before it is gone.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The primary commander of our Armed Forces, Toikka, is saying that the situation is desperate and we should just fall back, but I'm not giving up so easily.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We are already getting ridiculed in pro-Soviet posters and messages in the networks, giving up here would make it far worse.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Thinking of the sudden progress of our enemy, I bet some civilians in the area were cooperating with Nod on intel regarding the GDI base. We should give them a fitting punishment once the operation is over...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CR05/bridge.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "Anyway, for the mission itself, take a force and use them to reclaim control over a critical bridge. If you succeed, our HQ will send you an MCV and more reinforcements.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                storyDisplay => { storyDisplay.FindStoryImageById(6).AlphaRate = -2.0f; bleep12.Play(); },
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If only our other commanders were even half as competent as you, this war would already be over.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CR04()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/CR04/bg01.png", 1, 0f);
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "CONFLICT NEWS AND EQUIPMENT ANALYSIS";
                    toney7.Play();
                    TryPlaySong(ramap);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "After the dissolution of the Soviet Union, the Government retained a significant inventory of Soviet-era military equipment.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.AddSimpleStoryImage("Story/CR04/bg02.png", 1, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Allies partially disarmed them and forced the Government to downscale their military industry. As a result, the Government had to rely on buying or loaning equipment from the Allies when rebuilding their military.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Some of the Soviet equipment was still widely used, but was in the process of being phased out. That process was slowed down by military personnel used to the Soviet overwhelming raw power tactics, of whom many defected to the Resistance.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Allies made sure that their own military was superior to that of the Soviet successor states.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The First Tiberium War only made this discrepancy more significant, as the Allied Nations needed their advanced equipment for fighting the Brotherhood of Nod.";
                    storyDisplay.AddSimpleStoryImage("Story/CR04/bg03.png", 1, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Due to this, the Government has had to rely on equipment that is outdated by modern standards.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To answer modern challenges, a large upgrade program was started to modernize the old Allied equipment to match the latest GDI and Nod designs.";
                    storyDisplay.AddSimpleStoryImage("Story/CR04/bg04.png", 1, 0f);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Rumors suggest this program also includes re-designs of the most powerful Allied vehicles. One of them could be the \"Enforcer\" battle fortress unit, although none have been seen in action yet.";
                    storyDisplay.AddSimpleStoryImage("Story/CR04/bg05.png", 1, 0f);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While the program continues, the Communist militia has also been upgrading their Soviet-era vehicles with modern capabilities.";
                    storyDisplay.AddSimpleStoryImage("Story/CR04/bg06.png", 1, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It is unknown where and how they get the resources for upgrades.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Limited numbers of chemical weapon attacks by the resistance have also been seen, most notably a chemical bomb attack on a Government military train.";
                    storyDisplay.AddSimpleStoryImage("Story/CR04/bg07.png", 1, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI has expressed worry about the escalating levels of violence that the modernized equipment will bring, and strongly condemned the use of chemical weapons.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -2.5f);
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(secondhand);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "What the world doesn't yet know is that the Brotherhood of Nod has been helping the Reds modernize.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The chemical attack on our MCV train and the chemical weapons found at the oil industry were a godsend. They actually got GDI to strengthen their support for us. Seems our enemy miscalculated when they teamed up with Nod.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We've managed to learn of a set of weapons that Nod is transferring to the Communist scum.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI is reluctant to send in direct forces yet, but at least they've suggested using their air power instead. They say they need a spotter on the ground to signal when their planes should strike.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We've assigned a Commando to the task. Send him to a good spot and he can signal the airstrike.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have an outpost in the area and our Enforcer prototype is also nearby. You won't be needing them during this operation, however.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "With the force of the GDI behind us, we'll be victorious in no time.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CR03()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "NEWS AND OPINION";
                    storyDisplay.AddSimpleStoryImage("Story/CR03/bg01.png", 1, 0f);
                    toney7.Play();
                    TryPlaySong(ramap);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "The recent joint covert operation between the GDI and the Government to eliminate a Communist base was successful.";
                    storyDisplay.AddSimpleStoryImage("Story/CR03/bg02.png", 2, 0f);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Afterwards the Government was making quick progress. It seemed that the neo-Soviets were on their last legs and would fall within weeks.";
                    storyDisplay.AddSimpleStoryImage("Story/CR03/bg03.png", 3, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But then something happened. The Communists suddenly attacked new areas with much more power than before and gained significant territory, including multiple oil refinement facilities.";
                    storyDisplay.AddSimpleStoryImage("Story/CR03/bg04.png", 4, 0f);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Government has blamed this turn on foreign assistance to the Communists.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The neo-Soviets themselves have said that brutal actions taken by the government have led to an increased number of volunteer troops and financial donations on their side, and the Government has managed their forces laughably poorly.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The sudden shift in the situation was recently discussed by experts on live TV. While discussing Government military failures, the transmission was suddenly cut, with the broadcasting company pointing to technical issues as the reason.";
                    storyDisplay.AddSimpleStoryImage("Story/CR03/bg05.png", 5, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "International analysts suspect political involvement, though no proof exists.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Rumors say that there's significant disappointment towards the GDI among the Government.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The GDI thinks that they shouldn't intervene heavily in a civil war, which is internal affairs of the country.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Government instead stresses that should the nation fall to the neo-Soviets, the whole region of Eastern Europe would be threatened and the Allied nations might eventually face a new World War.";
                    storyDisplay.AddSimpleStoryImage("Story/CR03/bg06.png", 6, 0f);
                },
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/CR03/bg07.png", 7, 0f),
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(6);
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -2.5f);
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(secondhand);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "As you hopefully already know, the enemy has suddenly strengthened their forces and is in the process of conquering an industrial area.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 2, 0f);
                    country1.Play();
                },
                storyDisplay => AddRADisplay(storyDisplay, 3),
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/CR03/oilrefineries.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "They've captured the area's oil refineries, but we still have a slight foothold with a base. The Communist scum, however, are also attempting to siege the base and the situation is deteriorating rapidly.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I want you to reinforce the base and take that industrial area back. We cannot let it fall.";
                    AddRADisplayImage(storyDisplay, "Story/CR03/base.png", 5);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                storyDisplay => { storyDisplay.FindStoryImageById(5).AlphaRate = -2.0f; bleep12.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The GDI isn't taking us seriously enough. They haven't yet realized that we're fighting not only for ourselves, but also for them. I bet the Brotherhood of Nod would like to form an alliance with a renewed Soviet Union.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Hopefully they'll change their minds soon enough. Until then, we have to do with our own forces and the limited support we get from them.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You have proven to be a great asset so far. I hope you do not disappoint here either.";
                },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CR02()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 1, 0f).AlphaRate = 2.5f;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    country4.Play();

                    TryPlaySong(secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Your first operation was so successful that I'm going to brief you directly from now on.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR02/bg02.png", 2, 0f);
                    country1.Play();
                },
                storyDisplay =>
                {
                    var ivanovtext = storyDisplay.AddSimpleStoryImage("Story/CR02/ivanovtext.png", 999, 0f);
                    ivanovtext.AlphaRate = 3.0f;
                    ivanovtext.ImageX.SnapToValue(930f);
                    ivanovtext.ImageY.SnapToValue(300f);
                },
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "To offset the loss of one military base, the insurgents are attempting to set up another one.",
                storyDisplay => storyDisplay.FindStoryImageById(999).AlphaRate = -3.0f,
                storyDisplay => storyDisplay.RemoveStoryImageById(999),
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Global Defense Initiative has finally displayed some signs of usefulness. They've provided us with information and suggested a plan.";
                    AddRADisplay(storyDisplay, 3);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They say they can use airplanes to bomb the enemy's Construction Yard, if we first take out their anti-air defenses.";
                    AddRADisplayImage(storyDisplay, "Story/CR02/conyard.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI also provided us some Orca aircraft. You'll get our best Commando under your command. Use him and the Orcas to destroy the SAM sites, and the GDI best do the rest.";
                    AddRADisplayImage(storyDisplay, "Story/CR02/orca.png", 5);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have an informant in the local church who might be able to provide fresh knowledge of enemy positions to your Commando.";
                    AddRADisplayImage(storyDisplay, "Story/CR02/church.png", 6);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                storyDisplay => { storyDisplay.FindStoryImageById(6).AlphaRate = -2.0f; bleep12.Play(); },
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(8,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "We will win this fight. Wherever they settle, we'll find them and crush them like ants.",
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "I'll be waiting for the results.",
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; toney4.Play(); },
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                },
                null,
                null,
                null));

            return phases;
        }

        private List<Phase> CR01Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The destruction of the Soviet base has halted the Communist advance and allowed us to re-capture the area, while the radar station is now broadcasting our own message. Well done, Commander.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/victorybg.png", 1);
                    country4.Play();
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            var globalVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_CHURCH_ONE_DESTROYED");
            if (globalVariable != null && globalVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(0,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The church in the nearby village was destroyed during the operation. The Government denies all responsibility, blaming the occurence on the Communist militia.";
                        storyDisplay.AddSimpleStoryImage("Story/CR01/churchbg.png", 2);
                    },
                    null,
                    null,
                    null));
            }

            return phases;
        }

        private List<Phase> CR01()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(raintro);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "After the fall of the Soviet Union in the end of the Second World War, most of the Soviet republics gained independence and were militarily integrated into the newly-formed Global Defense Initiative.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg01.png", 1);
                    mapwipe2.Play();
                },
                storyDisplay =>
                {
                    var mousehint = new StoryImage(windowManager, 9001);
                    mousehint.Texture = AssetLoader.LoadTextureUncached("Story/CR01/mousehint.png");
                    mousehint.Alpha = 0f;
                    mousehint.AlphaRate = 2.0f;
                    mousehint.ImageX.SnapToValue(UIDesignConstants.CUTSCENE_DESIGN_RES_X - mousehint.Texture.Width - 100);
                    storyDisplay.AddStoryImage(mousehint);
                },
                null,
                null));

            phases.Add(new Phase(2,
               storyDisplay =>
               {
                   storyDisplay.FindStoryImageById(9001).AlphaRate = -1.0f;
                   storyDisplay.AddSimpleStoryImage("Story/CR01/bg02.png", 2);
                   mapwipe5.Play();
               },
               storyDisplay => storyDisplay.RemoveStoryImageById(1),
               storyDisplay => storyDisplay.RemoveStoryImageById(9001),
               null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "For some areas this transition was successful and led to a promising future...";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "... while for others, it led to stagnation where the damages of the war were just barely being repaired.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg03.png", 3);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Allied Nations, which became the newly-formed GDI, kept an eye on, but did not micromanage the countries. The ex-Soviet republics were mostly self-governing, with support from GDI.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg02.png", 1);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI publicly advocates liberal democracy, but practically the republics have been free to decide their own structure of government.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Decades went by, but not all people lost their nostalgia for the Soviet lifestyle.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg04.png", 4);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "One of the republics in particular felt that they were coldly forgotten about after the dissolution of the Soviet empire, which left the area in an unstable state of partial political vacuum.";
                    storyDisplay.AddSimpleStoryImage("Story/coatofarms.png", 5);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This was worsened by the global economic crisis brought by the First Tiberium War. Exploiting the unrest generated by the crisis, a faction of Communist radicals took up arms and wanted to overthrow the government.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg06.png", 6);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The government response was to quench the unrest with their own forces. However, the Communist radicals' propaganda machine had grown too influential.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg07.png", 7);
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They were accountable for the bloodshed, but their leaflets and posts on message boards managed to shift the blame towards the government for many people.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "After various smaller conflicts with mixed results, some of the republic's military generals defected to the Communist side and recommissioned abandoned Soviet bases, using them to produce military equipment.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg09.png", 9);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This is an unacceptable escalation in the situation. As the government military organization was ill-equipped to deal with this force, the government has assigned a new Commander to handle the situation.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    TryPlaySong(fac2226m);

                    storyDisplay.ConversationDisplay.ConversationText = "This is your first assignment.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                },
                null,
                null,
                storyDisplay => { storyDisplay.AddSimpleStoryImage("Story/CR01/bg10.png", 10, 1.0f); }));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Take a small force and destroy the recommissioned Soviet base." + Environment.NewLine + Environment.NewLine + "While at it, re-take a local radar station that the neo-Communists are using to agitate our own population against us.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }



        #region PTTP

        private List<Phase> PTTP1()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
               storyDisplay =>
               {
                   TryPlaySong(secondhand);
                   MediaPlayer.IsRepeating = true;
                   MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                   storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                   storyDisplay.ConversationDisplay.IsCentered = true;
                   storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide1.png", 1);
                   country4.Play();
               },
               null,
               null,
               null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide2.png", 2);
                    storyDisplay.ConversationDisplay.ConversationText = "In today's news: what some are labeling as a \"Tiberium War\" continues to engulf the African continent.";
                    mapwipe5.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                 storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The latest victim: the Republic of Serunda, which has seen Nod forces cross the border only just last month.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide3.png", 3);
                },
                null, null, null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Serundan government is in a state of complete disarray, with the majority of decision-makers having fled the country.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Global Defense Initiative peacekeepers have stationed themselves at key positions across the nation and taken over the functions of the former Serundan government.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide4.png", 3);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI General Mark Jamison Sheppard has stated publicly that the decision was made \"to prevent civil disorder.\"";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide5.png", 4);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Serunda, of course, is no stranger to conflict.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide6.png", 5);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The quasi-anarchist Serundan Liberation Front, or SLF, fought a guerilla war against the government for decades.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide7.png", 6);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Their charismatic leader, Mthunzi Gumede, led SLF resistance right up until his passing just two years ago, with his daughter, Ulwazi, succeeding him as head of the group.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide8.png", 7);
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There are whispers among those in the Serundan countryside that she's a kinder, gentler leader, and that liberation may yet come.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But it's safe to say that in the face of this massive military and ideological conflict between GDI and Nod, whatever relevance the SLF may once have had in Serunda has long since deteriorated to insignificance.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide9.png", 9);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide1.png", 1);
                    toney7.Play();
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide11.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide10.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Are you receiving this message? ...Good. Our equipment barely works sometimes.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(15,
                 storyDisplay =>
                 {
                     storyDisplay.ConversationDisplay.ConversationText = "I understand you're new here - I am Ulwazi Gumede. Welcome to the Serundan Liberation Front.";
                 },
                 null,
                 null,
                 null));

            phases.Add(new Phase(16,
                 storyDisplay =>
                 {
                     storyDisplay.ConversationDisplay.ConversationText = "Let me be frank. Our cause is just, but we have little support among the people.";
                 },
                 null,
                 null,
                 storyDisplay => AddRADisplay(storyDisplay, 3)));

            phases.Add(new Phase(17,
                 storyDisplay =>
                 {
                     AddRADisplayImage(storyDisplay, "Story/PTTP1/small1.png", 4);
                     storyDisplay.ConversationDisplay.ConversationText = "To make matters worse, this \"Brotherhood of Nod\" has entered our country and co-opted our message of resistance to Western imperialism.";
                 },
                 null,
                 null,
                 null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Those who are not swayed by the messaging of these charlatans are intimidated by the political violence they inflict on those brave enough to oppose them.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP1/small2.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "There is a man that I know the Brotherhood is already planning to assassinate - Mayardit.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "A brave leader of his people, he has already spoken out against the occupiers, and put a target on his back.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This is our chance to send a message to the average Serundan.  I need you to step in with the few troops we have and protect Mayardit from Nod's assassins.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(5).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); }));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Go quickly.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP1/slide11.png", 1);
                    toney7.Play();

                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        private List<Phase> PTTP2()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP2/slide1.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP2/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "You've done well - Mayardit is an important figure to the people locally, and he has many significant connections.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The fact that he has decided to join our cause is auspicious.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I'm off to meet with the Liberation Council to discuss high-level strategy.  Speak with Mayardit in the meantime for your orders - I think he wishes to thank you personally.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<REROUTING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP2/slide1.png", 1);
                    newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP2/slide3.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "Ah, my friend! I am very grateful to you for your efforts in protecting myself and my village.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I understand the urge that caused the scorpion to sting, but bless you for crushing it, eh?";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If only the eagle was so easily deterred. When the government collapsed, I had hopes that many imprisoned fighters and activists would be released.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 3)));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But when the eagle stepped in to take the reins, not only did they not release these people, but they moved them to a new prison deep in the desert.";
                    AddRADisplayImage(storyDisplay, "Story/PTTP2/small1.png", 4);
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Enough is enough.  You must free these people, my friend.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(4).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); }));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "And you must do it before the scorpion breaks into the prison.  They will free the worst of the criminals and hang the rest.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP2/slide1.png", 1);
                    toney7.Play();
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        private List<Phase> PTTP3()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP3/slide1.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP3/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Your actions are bearing fruit. Word spreads through the villages and the towns that the SLF is back, and that this time around it is truly acting in defence of the people.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, the leaders of the two biggest tribes in Serunda, the Hadarah and the Kwandari, have thrown their backing behind GDI and Nod, respectively.  In their eyes, we are still insignificant.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP3/slide3.png", 3);
                    bleep11.Play();
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "My friend; Ulwazi. Things become worse.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 4)));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP3/small1.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "The historic villages of Ondari and Darshad are now between the frontlines of the eagle and the scorpion. They will be little but smoking rubble in a matter of days.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(3);
                    bleep12.Play();
                }));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "That is troubling indeed.  Although... we may be able to turn this situation to our advantage.";

                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If we were to step in and push both front lines back, we could buy enough time to evacuate the villages.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP3/slide3.png", 3);
                    bleep11.Play();
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "Those villages are important to both the Hadarah and the Kwandari. They will be indebted to us if we save Ondari and Darshad from armageddon.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ah, now this is becoming a plan worthy of a warrior!";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(3);
                    bleep12.Play();
                }));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Agreed.  It's decided then.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(4).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(5).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); }));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Comrade, mobilize your forces immediately and prepare to engage both GDI and Nod forces. The villages of Ondari and Darshad must be protected at all costs.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP3/slide1.png", 1);
                    toney7.Play();

                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        private List<Phase> PTTP4()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;
                
                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide1.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "During the Second Great War, the Soviet Union supplied funds and conventional weapons to anti-colonial movements across Africa and Asia.";
                    mapwipe5.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The SLF, initially founded as a response to British colonization of Serunda, took full advantage of the Soviet supplies on offer and fought the British for years.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide3.png", 3);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "At war's end, despite the collapse of the USSR, European powers found little relief in their beleaguered colonies, and abandoned them either by choice or at the head of a gun.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide4.png", 3);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In Serunda, there was a peaceful transition of power.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide11.png", 3);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "A new government was formed, comprised of pro-western Serundan intellectuals and SLF leadership, the latter having been granted amnesty by the outbound British forces.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It was to last only a matter of weeks.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide5.png", 5);
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Serundan military quickly overthrew the civilian government, turning the new Republic of Serunda into an autocratic dictatorship and forcing the SLF back into a campaign of guerilla resistance.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The fighting reignited and intensified as both sides ruthlessly attempted to eradicate their opposition, committing numerous atrocities across Serunda.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide6.png", 7);
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To the outside world, the word Serunda quickly became associated with the phrase \"humanitarian disaster.\"";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Decades of strife and suffering passed before the Serundan military came out on top.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide7.png", 9);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Beaten and discredited, the SLF fled to mountain hideouts and nursed their wounds as an increasingly corrupt Serundan government began to steal even from its own military.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Republic of Serunda was in dire straits, ripe for yet another terrible upheaval as it entered a new era - the Tiberian era.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide8.png", 9);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide1.png", 1);
                    toney7.Play();
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide9.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide10.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Hello.  I am aware you do not know who I am.  You may call me Jamal.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(17,
                 storyDisplay =>
                 {
                     storyDisplay.ConversationDisplay.ConversationText = "I only recently came into your service on account of being freed from a GDI prison.  I understand I have you to thank for that.";
                 },
                 null,
                 null,
                 null));

            phases.Add(new Phase(18,
                 storyDisplay =>
                 {
                     storyDisplay.ConversationDisplay.ConversationText = "There are many others who are increasingly thankful - with your latest victory, the Serundan people are united behind us like never before.";
                 },
                 null,
                 null,
                 null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While Ulwazi debates how to take advantage of our new popularity with the Liberation Council, I am to give you orders.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I used to be quite high up in the military - you know, before my opinions landed me in a tiny cell.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 3)));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP4/small1.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "The fact of the matter is that the SLF needs equipment, and our best chance of obtaining that equipment is to purchase it from sources in the former USSR.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, the SLF also lacks funds.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP4/small2.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "So, you will get these funds. You will harvest Tiberium from the Nkoma valley, where GDI and Nod are currently thrashing it out, and then you will retreat.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP4/small3.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "You will prioritize eliminating Nod's SAM Sites.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP4/small4.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "The local A-10 colonel is corrupt, and will gladly go after the Brotherhood if he can grab all the glory without risking his planes.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP4/small5.png", 8);
                    storyDisplay.ConversationDisplay.ConversationText = "You will NOT attack the major GDI airbase there - it is too high profile a target. Frankly, I think our past engagements with GDI have already flown us too close to the sun.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(7); storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(8).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); }));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Go now, and godspeed.";

                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP4/slide9.png", 1);
                    toney7.Play();

                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        private List<Phase> PTTP5()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP5/slide1.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP5/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Good work.  Ulwazi is still busy.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "She's now trying to reach out to contacts old and new in the former USSR.  Hopefully someone over there is not totally disillusioned.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We have a chance in the meantime to bolster our offer.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 3)));

            phases.Add(new Phase(4,
               storyDisplay =>
               {
                   AddRADisplayImage(storyDisplay, "Story/PTTP5/small1.png", 4);
                   storyDisplay.ConversationDisplay.ConversationText = "GDI is moving a nuclear detonator through our country in an effort to take it off the continent.";
               },
               null,
               null,
               null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Our intelligence sources, more reliable than ever since the battle at Ondari and Darshad, inform us that the Brotherhood is interested in stealing this detonator.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP5/small2.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "I will be clear - we have no use for the detonator itself.  It would only bring heat on us politically, and potentially alienate us from the people.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP5/small3.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "However, a nuclear detonator is something we could definitely sell to whoever we end up bartering with in the former USSR.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP5/small4.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "We know little of Nod's specific heist plans for the detonator, other than that they will launch the operation from a nearby base, and try to exfiltrate it out of the area through a route unknown to us.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP5/small5.png", 8);
                    storyDisplay.ConversationDisplay.ConversationText = "You will lead a small team to capture one of their communications facilities.  Once inside, you should have a better idea of how to hijack their efforts.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(7)));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP5/small6.png", 9);
                    storyDisplay.ConversationDisplay.ConversationText = "Do NOT inflict any harm on GDI personnel... or let GDI discover you are in the area.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(8)));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They will see us as a much larger threat if they think we are interested in nuclear weapons.  Let them think the Brotherhood has the detonator.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(9).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); }));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Go now, and godspeed.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP5/slide1.png", 1);
                    toney7.Play();
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        private List<Phase> PTTP6()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide1.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "In today's news: the situation in the Republic of Serunda has undergone a number of surprising new developments.";
                    mapwipe5.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The SLF, previously seen as a relic of a bygone era, has launched a series of small but undeniably successful operations across the country.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide3.png", 3);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Both GDI and Nod forces have found themselves surprised by the effect the SLF is having on their activities.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide4.png", 3);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "After all, the \"Tiberium War\" remains largely a bipolar conflict across the rest of the continent.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide5.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "Nod forces in particular have been repeatedly humiliated by the SLF's new leadership, finding assassination, prisoner rescue and even general offensives disrupted by SLF operations.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "A Nod push through the Nkoma valley recently stalled out due to SLF forces having completely stripped the area clean of the Tiberium needed to continue the offensive.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide6.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "It's even been rumored that Nod's commander on the Serundan front has been replaced for incompetence - which, as we understand Nod's practices, usually means execution.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide7.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "Western intelligence analysts estimate that the number of SLF soldiers may have increased by as much as tenfold since they began this latest set of actions.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide8.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "While the SLF has engaged GDI forces before, it has largely steered clear of attacking major facilities.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide9.png", 9);
                    storyDisplay.ConversationDisplay.ConversationText = "This may not stop GDI from taking a heavier hand with the rebel group, as General Mark Jamison Sheppard remarked today at a press conference that there is \"growing concern\" among the GDI leadership at what's happening in the Republic of Serunda.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide1.png", 1);
                    toney7.Play();
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide10.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide11.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Comrade - I have good news and bad news.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                storyDisplay => AddRADisplay(storyDisplay, 3)));

            phases.Add(new Phase(15,
                 storyDisplay =>
                 {
                     AddRADisplayImage(storyDisplay, "Story/PTTP6/small1.png", 4);
                     storyDisplay.ConversationDisplay.ConversationText = "The good news is that I successfully made contact with interested parties in the former Soviet Union.";
                 },
                 null,
                 null,
                 null));

            phases.Add(new Phase(16,
                 storyDisplay =>
                 {
                     AddRADisplayImage(storyDisplay, "Story/PTTP6/small2.png", 5);
                     storyDisplay.ConversationDisplay.ConversationText = "We have agreed on a deal - the funds and the detonator for valuable Soviet military equipment and the schematics to produce it.";
                 },
                 null,
                 null,
                 storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP6/small3.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "It is to arrive in two shipments - the first shipment made landfall a few days ago, and we have already begun making use of the technology.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    bleep11.Play();
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide13.png", 7);
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "The bad news is that the logistics center which handles these operations has fallen under a sustained attack.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(3);
                    storyDisplay.RemoveStoryImageById(5);
                    storyDisplay.RemoveStoryImageById(6);
                }));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    bleep11.Play();
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide12.png", 8);
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "And of course, it is not just the scorpion, but also the eagle who besieges us!  Were your plans not supposed to prevent this very outcome?";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(8);
                }));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    bleep11.Play();
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Well, within a certain -";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    bleep11.Play();
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide12.png", 8);
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "Or were you just trying to prevent too many of the eagle's men from getting torn apart?";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(8);
                }));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    bleep11.Play();
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "I'm not sure I -";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    bleep11.Play();
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide12.png", 8);
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "There are voices on the Liberation Council saying that -";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(7).AlphaRate = -crRAdisplayAlphaRate;
                    storyDisplay.FindStoryImageById(8).AlphaRate = -crRAdisplayAlphaRate;

                    //storyDisplay.RemoveStoryImageById(7);
                    //storyDisplay.RemoveStoryImageById(8);
                    bleep17.Play();
                }));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide11.png", 2);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Enough! The fact of the matter is that we may not even receive the second shipment at all if the logistics facility falls before it arrives.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Comrade, move immediately to save the situation.  You must succeed - our resistance is on the verge of transforming into a true war for liberation.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP6/slide10.png", 1);
                    toney7.Play();
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        private List<Phase> PTTP7()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide1.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "The SLF's leadership structure has remained consistent for decades. In theory, if not in practice. ";
                    mapwipe5.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In theory, the SLF is organized according to anarcho-syndicalist principles, and governed by a Liberation Council of a dozen different individuals.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide3.png", 3);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "These individuals must come to a majority consensus on all major matters concerning the revolt, as well as governance of liberated areas.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Decision on lesser matters is handed by smaller sub-councils, sometimes affiliated with military units, sometimes affiliated with towns or villages.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide4.png", 3);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Individuals from the sub-councils are invited to observe or participate at Liberation Council meetings in a non-voting capacity on an ad-hoc basis.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide5.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "In practice, the SLF for most of its lifespan has operated under the authoritarian rule of Mthunzi Gumede.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Liberation Council was primarily composed of yes-men and patsies, existing to rubber-stamp Mthunzi's decisions.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Decisions on the ground were driven strictly from the top, rather than as initiatives from below.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide6.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "Only recently has this begun to change, according to the latest reports from anonymous sources.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Under the guidance of Mthunzi's daughter, Ulwazi Gumede, the SLF is beginning to operate in accordance with its ideals, and it has decentralized much of its authority to the Liberation Council and to the sub-councils.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide7.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "However, it remains to be seen if this is a viable approach for the governance of a revolutionary army.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The same reports indicate that SLF leadership is plagued by a great deal of infighting after they barely repellled a combined GDI and Nod offensive on one of their facilities.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide1.png", 1);
                    toney7.Play();
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide8.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide9.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "Ah, my friend!  It is so good that you managed to save our facility.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Without you, we would have lost everything, and we would have no new weapons to fight the imperialists with.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It is excellent to have these weapons - no longer must we skulk in the shadows like Jamal wishes us to.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I have brought before the Liberation Council a motion that it is time to finally take the fight to the eagle.  They approved it -- narrowly and with a \"stipulation\".";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 3)));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP7/small1.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "You see, the Liberation Council is mobilizing most of our forces for a grand, surprise offensive.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                 storyDisplay =>
                 {
                     storyDisplay.ConversationDisplay.ConversationText = "The stipulation therefore is that you will only have a small contingent of our troops available.";
                 },
                 null,
                 null,
                 null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP7/small2.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "With these troops, I must ask you to punish the eagle.  Destroy their holdings, raze their structures, kill their troops.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(4);
                    storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate;
                    storyDisplay.FindStoryImageById(5).AlphaRate = -crRAdisplayAlphaRate;
                    bleep17.Play();
                }));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This will be a campaign of terror, not of conquest. They must know that we are not to be trifled with, and that we do not suffer their offenses lightly.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP7/slide8.png", 1);
                    toney7.Play();
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        private List<Phase> PTTP8()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP8/slide1.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP8/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "We must speak quickly.  I don't know what Mayardit was thinking with that last operation.";
                    country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "He was either trying to set you up to fail in order to discredit you, or he was actually just stupid enough to think that was a good plan.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(storyDisplay, 3)));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP8/small1.png", 4);
                    storyDisplay.ConversationDisplay.ConversationText = "Regardless, what is clear to me is that GDI's Mammoth Tanks could still throw a wrench in the offensive being planned by the Liberation Council.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {

                    storyDisplay.ConversationDisplay.ConversationText = "The fact of the matter is that we don't have equipment that can match their capabilities.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP8/small2.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "However, what I can tell you is that GDI's concentrated all of their Mammoth production in Serunda at a single facility.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The security around the base is significant, but we've captured a few men who have seen the inside, and apparently it's quite lax inside the vehicle plant itself.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Here's what you are going to do.  You are going to slip saboteurs into the Mammoth Tank plant. You will not shoot at GDI or get discovered by GDI troops.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    AddRADisplayImage(storyDisplay, "Story/PTTP8/small3.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "You will also destroy the Nod base planning an offensive across the river using our new V2 Launchers - this plan might as well be dust in the wind if the Brotherhood levels the facility.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(5);
                    storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate;
                    storyDisplay.FindStoryImageById(6).AlphaRate = -crRAdisplayAlphaRate;
                    bleep17.Play();
                }));


            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Go now, and godspeed.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
             storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP8/slide1.png", 1);
                    toney7.Play();
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        private List<Phase> PTTP8Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP8/slide1.png", 1);
                    country4.Play();

                },
                null,
                null,
                null));


            var wfCapturedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_PTTP_WF_CAPTURED");
            if (wfCapturedVariable != null && wfCapturedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(1,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP8/slide3.png", 2);
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "Comrade.  I'm not sure I fully understand why you decided to continue with the operation.";
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(1),
                    null,
                    null));
      
                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It appears Jamal was attempting to slip one by us - neither I nor the Liberation Council had anything to do with those orders.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Our soldiers tried to apprehend him, but -- he's already gone without a trace.  I've examined his plan; I have to for all of our sakes, since you've put it in motion.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "But -- I'm troubled by his deceit, and I... can't say I'm not rethinking many things.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The Liberation Council may choose to punish you for your insubordination. Be prepared for anything.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.TextColor = Color.White;
                        storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                        storyDisplay.AddSimpleStoryImage("Story/PTTP8/slide1.png", 1);
                        toney7.Play();
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }
            else
            {
                phases.Add(new Phase(1,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP8/slide3.png", 2);
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "Comrade.  Thank you for abandoning the operation so quickly.";
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(1),
                    null,
                    null));

                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It appears Jamal was attempting to slip one by us - neither I nor the Liberation Council had anything to do with those orders.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Our soldiers tried to apprehend him but -- he's already gone without a trace.  I'm sure nothing good would have come of this.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Rest up -- we need you for the big offensive soon.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.TextColor = Color.White;
                        storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                        storyDisplay.AddSimpleStoryImage("Story/PTTP8/slide1.png", 1);
                        toney7.Play();
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            return phases;
        }

        private List<Phase> PTTP9()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide1.png", 1);
                    country4.Play();
                },
                null,
                null,
                null));

            var wfCapturedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_PTTP_WF_CAPTURED");
            if (wfCapturedVariable != null && wfCapturedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(1,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide2.png", 2);
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "The day is finally here.  Across Serunda, our well-equipped forces are prepared to launch a simultaneous offensive that will throw the occupiers out of our country!";
                        country1.Play();

                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(1),
                    null,
                    null));

                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        bleep11.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide3.png", 3);
                        storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                        storyDisplay.ConversationDisplay.ConversationText = "I don't understand why our \"friend\" is here.  Has he not proved himself to be in line with the traitor Jamal?";
                    },
                    null,
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(3); bleep12.Play(); }));

                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "Mayardit, you know as well as I that the Council has narrowly elected to proceed with the plans implemented by Jamal and our comrade here, on the basis that they may indeed assist the offensive.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        bleep11.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide3.png", 3);
                        storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                        storyDisplay.ConversationDisplay.ConversationText = "Approving the plan is one thing. Placing our friend in charge of it is asking for trouble.";
                    },
                    null,
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(3); bleep12.Play(); }));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "Enough. Your presence here is a courtesy, nothing more.  You are dismissed.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        bleep17.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide4.png", 3);
                        storyDisplay.ConversationDisplay.ConversationText = "Comrade - the Council did not want to place you in charge of this operation.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "And yet, this latest scheme of yours and Jamal's - I can find no treason in it.  I personally recommended you for this offensive based on your prior performance - and that of Jamal.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Comrade, you will ignite the fire that burns Serunda clean.";
                    },
                    null,
                    null,
                    storyDisplay => AddRADisplay(storyDisplay, 4)));

                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        AddRADisplayImage(storyDisplay, "Story/PTTP9/small1.png", 5);
                        storyDisplay.ConversationDisplay.ConversationText = "You will attack and level GDI's largest stronghold in our country at Jazahiri, which contains a sizable air contingent and their Ion Cannon Uplink.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(10,
                    storyDisplay =>
                    {
                        AddRADisplayImage(storyDisplay, "Story/PTTP9/small4.png", 6);
                        storyDisplay.ConversationDisplay.ConversationText = "You will use the saboteurs placed in GDI's Mammoth Tanks to launch a sneak attack on their operations there and cripple their infrastructure.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(5)));

                phases.Add(new Phase(11,
                    storyDisplay =>
                    {
                        AddRADisplayImage(storyDisplay, "Story/PTTP9/small2.png", 7);
                        storyDisplay.ConversationDisplay.ConversationText = "You will also eliminate the Brotherhood forces.  They have constructed a Temple in a desperate effort to justify their relevance.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(6)));

                phases.Add(new Phase(12,
                    storyDisplay =>
                    {
                        AddRADisplayImage(storyDisplay, "Story/PTTP9/small3.png", 8);
                        storyDisplay.ConversationDisplay.ConversationText = "This is the most fortified area in Serunda. Liberating it is the signal to our troops that the day has come, and the signal to the occupiers that their days are numbered.";
                    },
                    null,
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.RemoveStoryImageById(7);
                        storyDisplay.FindStoryImageById(4).AlphaRate = -crRAdisplayAlphaRate;
                        storyDisplay.FindStoryImageById(8).AlphaRate = -crRAdisplayAlphaRate;
                    }));

                phases.Add(new Phase(13,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It all ends here.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(14,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It's time.  Go, Comrade!";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(15,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.TextColor = Color.White;
                        storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide1.png", 1);
                        toney7.Play();
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }
            else
            {
                phases.Add(new Phase(1,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide2.png", 2);
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "The day is finally here.  Across Serunda, our well-equipped forces are prepared to launch a simultaneous offensive that will throw the occupiers out of our country!";
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(1),
                    null,
                    null));

                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        bleep11.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide3.png", 3);
                        storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                        storyDisplay.ConversationDisplay.ConversationText = "With the traitor Jamal gone, nothing can undermine our success now.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The Council has agreed to transfer executive authority to Ulwazi and I for the duration of the upcoming offensive, in order to prevent further such incidents.";
                    },
                    null,
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(3); bleep12.Play(); }));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "Yes, indeed.  Comrade, you will ignite the fire that burns Serunda clean.";
                    },
                    null,
                    null,
                    storyDisplay => AddRADisplay(storyDisplay, 4)));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        AddRADisplayImage(storyDisplay, "Story/PTTP9/small1.png", 5);
                        storyDisplay.ConversationDisplay.ConversationText = "You will attack and level GDI's largest stronghold in our country at Jazahiri, which contains a sizable air contingent and their Ion Cannon Uplink.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        AddRADisplayImage(storyDisplay, "Story/PTTP9/small2.png", 6);
                        storyDisplay.ConversationDisplay.ConversationText = "You will also eliminate the Brotherhood forces.  They have constructed a Temple in a desperate effort to justify their relevance.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(5)));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        AddRADisplayImage(storyDisplay, "Story/PTTP9/small3.png", 7);
                        storyDisplay.ConversationDisplay.ConversationText = "This is the most fortified area in Serunda. Liberating it is the signal to our troops that the day has come, and the signal to the occupiers that their days are numbered.";
                    },
                    null,
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.RemoveStoryImageById(6);
                        storyDisplay.FindStoryImageById(4).AlphaRate = -crRAdisplayAlphaRate;
                        storyDisplay.FindStoryImageById(7).AlphaRate = -crRAdisplayAlphaRate;
                    }));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It all ends here.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        bleep11.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide3.png", 3);
                        storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                        storyDisplay.ConversationDisplay.ConversationText = "I will personally assist you with my forces on the battlefield, Comrade.  It will be a most glorious day.";
                    },
                    null,
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(3); bleep12.Play(); }));

                phases.Add(new Phase(10,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "It's time.  Go, Comrade!";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(11,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.TextColor = Color.White;
                        storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide1.png", 1);
                        toney7.Play();
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }
            return phases;
        }

        private List<Phase> PTTPEnd()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    TryPlaySong(chrg226m);

                    storyDisplay.ConversationDisplay.ConversationText = "The SLF's overwhelming victory at Jazahiri caught both GDI and the Brotherhood by surprise, and threw their troops into disarray.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide5.png", 1);
                    mapwipe2.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "All across Serunda, GDI and Nod troops retreated across international borders, surrendered, or were destroyed in the face of demoralization, disorganization and determined SLF opposition.";
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            var wfCapturedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_PTTP_WF_CAPTURED");
            if (wfCapturedVariable != null && wfCapturedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide6.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "Ulwazi Gumede was different than her predecessor. Under her guidance, the SLF had become more true to its anarchist principles.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Even after infighting within the SLF developed into actual fighting at Jazahiri, she refused to use it as an excuse to centralize her power or punish those with different opinions.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide10.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "Ulwazi was the face of the new government, but she was not the ruler - the Liberation Council and the various sub-councils acted as a new kind of leadership for a new kind of state.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Within months, it was not just Ulwazi, but many new individuals acting as the face of the Federation of Serundan Councils.  It was a new era.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide12.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "While the war raged on in Africa between GDI and Nod, in Serunda, the war was over.  However, the war was not gone.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide8.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "It was etched in the faces of those who had survived, and scarred in the craters. It burned in the still-growing Tiberium fields of the countryside.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide9.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "As the devastated Serundan people attempted to piece their lives back together with hopeful eyes and a new feeling of inspiration, Ulwazi looked into the distance.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "She pondered the fate of their experiment. Would they succeed at their goal and build a new kind of society? Or would they fail, and become another ash heap on the mantle of history?";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(10,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide11.png", 1);
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "...";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                        storyDisplay.ConversationDisplay.IsCentered = false;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }
            else
            {
                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide6.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "It had seemed for a while that Ulwazi Gumede might be different than her predecessor. Under her guidance, the SLF had at first become more true to its anarchist principles.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "However, Ulwazi used the final offensive as an excuse to recentralize the power structure of the SLF into something approaching the authoritarianism of her father's leadership - and indeed, that of the former USSR.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide7.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "Ulwazi did not rule for long.  Within months, she had vanished from the public eye, her co-executive Mayardit now the face of the People's Republic of Serunda. She was not the last to disappear.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide12.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "While the war raged on in Africa between GDI and Nod, in Serunda, the war was over.  However, the war was not gone.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide8.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "It was etched in the faces of those who had survived, and scarred in the craters. It burned in the still-growing Tiberium fields of the countryside.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide9.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "As the devastated Serundan people attempted to piece their lives back together under the watchful eyes of their new government, Mayardit looked into the distance.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "He pondered: which group would support him more? The eagle, or the scorpion?";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP9/slide11.png", 1);
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "...";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                        storyDisplay.ConversationDisplay.IsCentered = false;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            return phases;
        }

        #endregion
    }
}
