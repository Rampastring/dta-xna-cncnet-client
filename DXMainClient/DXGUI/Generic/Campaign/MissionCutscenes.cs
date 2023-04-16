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

        public MissionCutscenes()
        {
            bleep9 = new EnhancedSoundEffect("Story/Sounds/BLEEP9.WAV");
            bleep11 = new EnhancedSoundEffect("Story/Sounds/BLEEP11.WAV");
            bleep12 = new EnhancedSoundEffect("Story/Sounds/BLEEP12.WAV");
            bleep17 = new EnhancedSoundEffect("Story/Sounds/BLEEP17.WAV");
            country1 = new EnhancedSoundEffect("Story/Sounds/COUNTRY1.WAV");
            country4 = new EnhancedSoundEffect("Story/Sounds/COUNTRY4.WAV");
            toney4 = new EnhancedSoundEffect("Story/Sounds/TONEY4.WAV");
            toney7 = new EnhancedSoundEffect("Story/Sounds/TONEY7.WAV");
            mapwipe2 = new EnhancedSoundEffect("Story/Sounds/MAPWIPE2.WAV");
            mapwipe5 = new EnhancedSoundEffect("Story/Sounds/MAPWIPE5.WAV");

            ramap = AssetLoader.LoadSong("Story/Music/ramap");
            raintro = AssetLoader.LoadSong("Story/Music/raintro");
            fac2226m = AssetLoader.LoadSong("Story/Music/fac2226m");
            secondhand = AssetLoader.LoadSong("Story/Music/2nd_hand");
            terminat = AssetLoader.LoadSong("Story/Music/terminat");
            chrg226m = AssetLoader.LoadSong("Story/Music/chrg226m");
            vector1a = AssetLoader.LoadSong("Story/Music/vector1a");
        }

        private readonly EnhancedSoundEffect bleep9;
        private readonly EnhancedSoundEffect bleep11;
        private readonly EnhancedSoundEffect bleep12;
        private readonly EnhancedSoundEffect bleep17;
        private readonly EnhancedSoundEffect country1;
        private readonly EnhancedSoundEffect country4;
        private readonly EnhancedSoundEffect toney4;
        private readonly EnhancedSoundEffect toney7;
        private readonly EnhancedSoundEffect mapwipe2;
        private readonly EnhancedSoundEffect mapwipe5;

        private readonly Song ramap;
        private readonly Song raintro;
        private readonly Song fac2226m;
        private readonly Song secondhand;
        private readonly Song terminat;
        private readonly Song chrg226m;
        private readonly Song vector1a;

        private const double crRAdisplayX = 4.0;
        private const double crRAdisplayY = 20.0;
        private const double crRAdisplayMiddle = 234.0;
        private const double crRAdisplayImageX = crRAdisplayX + 30.0; // the RA border thingies are 30 pixels wide at left and right
        private const double crRAdisplayImageRate = 300.0;
        private const double crRAdisplayImageWidth = 400.0;
        private const double crRAdisplayImageY = crRAdisplayY + 16.0; // the RA border thingies are 16 pixels high at the top
        private const float crRAdisplayAlphaRate = 5.0f;

        private IStoryDisplay storyDisplay;
        private WindowManager windowManager;

        private void TryPlaySong(Song song)
        {
            if (song == null)
                return;

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;
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

        private void HideAllStoryImagesWithSound(IStoryDisplay storyDisplay, EnhancedSoundEffect sound)
        {
            storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);

            if (sound != null)
                sound.Play();
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
                null
                ));

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
                    storyDisplay.ConversationDisplay.ConversationText = "Toikka has been making progress on the frontline again with the help of the Sensor Arrays that have prevented Nod flanks.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "Considering your latest mission, the Communist scum has allowed Nod to poison the countryside - the very land they say they are \"liberating\" - with Tiberium to fund their war effort.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "I'll soon be off at negotiations with GDI for some days. I'll be waiting news of your victory.";
                },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -crRAdisplayAlphaRate; bleep17.Play(); },
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(1).AlphaRate = -2.0f; toney4.Play(); }));

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
                null
                ));

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
                    storyDisplay.ConversationDisplay.ConversationText = "The main reason appears to be Nod's \"Ezekiel's Wheel\" tanks - stealth tanks that perform surgical strikes on bases, harvesters and vulnerable supplies, and then disappear before Government military can catch them.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "This trifecta of Stealth tanks, increased Tiberium harvesting and foreign reinforcements threats turning the tide of the war.";
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

            var genocideVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ALL_CHURCHES_DESTROYED");
            if (genocideVariable != null && genocideVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(0,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The Government forces stand victorious, with the victory again enabled by their new star of a commander.";
                        TryPlaySong(raintro);
                    },
                    null,
                    null,
                    null
                    ));

                phases.Add(new Phase(1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Meanwhile, the Global Defense Initiative has made progress in its research into the allegations of genocide performed by the Government.";
                    },
                    null,
                    null,
                    null
                    ));

                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "GDI has determined there to be some truth to the claims. As a result, they have decided to lessen their military support for the Government until the Government takes serious steps to fix the situation.";
                    },
                    null,
                    null,
                    null
                    ));
            }
            else
            {
                phases.Add(new Phase(0,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The recent addition to the Government's military leadership has once again brought victory for the Government.";
                        storyDisplay.AddSimpleStoryImage("Story/CR08/victorybg01.png", 1);
                        country4.Play();
                        TryPlaySong(raintro);
                    },
                    null,
                    null,
                    null
                    ));

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
                        null
                        ));

                    phases.Add(new Phase(5,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "You could ask Ivanov about it, but there's a chance that'd go rather wrong...";
                        },
                        null,
                        null,
                        null
                        ));

                    phases.Add(new Phase(6,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                            storyDisplay.AddSimpleStoryImage("Story/CR08/bg04.png", 3);
                            storyDisplay.ConversationDisplay.ConversationText = "... civilian bodies, huh?";
                        },
                        null,
                        null,
                        null
                        ));

                    phases.Add(new Phase(7,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "What's wrong with that? They collaborated with the enemy, worked on the factory controlled by the Communist scum.";
                        },
                        null,
                        null,
                        null
                        ));

                    phases.Add(new Phase(8,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "Have you grown a sweet spot for the enemy, Commander?";
                        },
                        null,
                        storyDisplay => storyDisplay.FindStoryImageById(3).AlphaRate = -1.0f,
                        storyDisplay => storyDisplay.RemoveStoryImageById(3)
                        ));

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

            var genocideVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_CR_ALL_CHURCHES_DESTROYED");
            if (genocideVariable != null && genocideVariable.EnabledThroughPreviousScenario)
            {
                // TODO
            }
            else
            {
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
                    null
                    ));

                phases.Add(new Phase(1,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The archipelago had worked as a troop and equipment transportation route, and when frozen, would've allowed tanks to drive through the area and attack the Government shoreline.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()
                    ));

                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The information gathered from the command center revealed that industry under Communist and Nod controlled territory is producing surprisingly large numbers of military equipment.";
                        storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg02.png", 1);
                    },
                    null,
                    null,
                    null
                    ));

                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "For that, the industry must have access to more raw materials than has earlier been possible.";
                        storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg02.png", 1);
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()
                    ));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "An anonymous source from GDI intelligence has suspected that Nod is providing them with cheap Tiberium-extracted minerals.";
                        storyDisplay.AddSimpleStoryImage("Story/CR07/victorybg03.png", 1);
                    },
                    null,
                    null,
                    null
                    ));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "To achieve a lower price for the minerals, a deal with Nod is suspected where Nod gives the tiberium-extracted minerals at a discount to enterprises that build military equipment for them or the Communists.";
                    },
                    null,
                    null,
                    null
                    ));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It is necessary for the Government and GDI to strike the manufacturing sites or cut the source of Tiberium minerals if they want to reduce the number of enemy tanks.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()
                    ));

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
                        null
                        ));

                    phases.Add(new Phase(7,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "They claim that Government forces under other commanders' command have committed attacks on civilian villages, killing and torturing entire villages if they've suspected collaboration with the enemy.";
                        },
                        null,
                        null,
                        null
                        ));

                    phases.Add(new Phase(8,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "According to them, this has also eroded public support for the Government and GDI, and increased support for the Communists and Nod. Many people who were loyal to the Government have defected due to this.";
                        },
                        null,
                        null,
                        null
                        ));

                    phases.Add(new Phase(9,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "One of the affected villages is supposedly located near a vehicle manufacturing plant that the Government forces and GDI have been assaulting recently.";
                        },
                        null,
                        null,
                        null
                        ));

                    phases.Add(new Phase(10,
                        storyDisplay =>
                        {
                            storyDisplay.ConversationDisplay.ConversationText = "If your forces participate in combat there in the future, it could give you a chance to check whether these claims hold truth.";
                        },
                        null,
                        null,
                        null
                        ));
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
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Poster and other activity by Communist partisans had greatly increased, mocking the sudden loss of the GDI and Government shoreline bases.";
                    storyDisplay.AddSimpleStoryImage("Story/CR05/victorybg02.png", 1);
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, now it has all quieted down due to the unexpectedly successful Government counterattack, supported by the full fury of the Eagle. The new Government Commander is also gaining widespread popularity among their troops.";
                    storyDisplay.AddSimpleStoryImage("Story/CR05/victorybg01.png", 1);
                },
                null,
                null,
                null
                ));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Only a rumor exists that Nod had captured major GDI technology during the operation. GDI has refused to comment on this rumor, citing operation security as the reason for the silence.";
                },
                null,
                storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

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
                    null
                    ));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The Government has denied these claims, stating that they only deal with collaborators and even those are tried in court. No regular civilian needs to be afraid of their forces.";
                    },
                    null,
                    storyDisplay => HideAllStoryImagesWithSound(storyDisplay, country1),
                    storyDisplay => storyDisplay.ClearStoryImages()
                    ));
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
                    storyDisplay.ConversationDisplay.ConversationText = "There is a tiberium-infested river basin where GDI had a Tiberium research and containment facility, which they were transforming into a base to counter Nod.";
                },
                null,
                storyDisplay => AddRADisplayImage(storyDisplay, "Story/CR05/tibriver.png", 4),
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "At the height their incompetence, GDI didn't pay enough attention that Nod noticed their efforts and was preparing a strike.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "Our other commanders are saying that the situation is desperate and we should just fall back, but I'm not giving up so easily.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We're already getting ridiculed in pro-Soviet posters and messages in the networks, giving up here would make it far worse.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "GDI has expressed worry about the escalating levels of violence that the modernized equipment will bring, and strongly comdemned the use of chemical weapons.";
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
                null,
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "To offset the loss of one military base, the insurgents are attempting to set up another one.",
                null,
                null,
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
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

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
                    null
                    ));
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
                null,
                null,
                null
                ));

            phases.Add(new Phase(2,
               storyDisplay =>
               {
                   storyDisplay.AddSimpleStoryImage("Story/CR01/bg02.png", 2);
                   mapwipe5.Play();
               },
               storyDisplay => storyDisplay.RemoveStoryImageById(1),
               null, null
               ));

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
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Allied Nations, which became the newly-formed GDI, kept an eye, but did not micromanage the countries. The ex-Soviet republics were mostly self-governing, with support from GDI.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg02.png", 1);
                },
                null,
                null,
                null
                ));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI publicly advocates liberal democracy, but practically the republics have been free to decide their own structure of government.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Decades went by, but not all people lost their nostalgia for the Soviet lifestyle.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg04.png", 4);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "One of the republics in particular felt that they were coldly forgotten about after the dissolution of the Soviet empire, which left the area in an unstable state of partial political vacuum.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg05.png", 5);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This was worsened by the global economic crisis brought by the First Tiberium War. Exploiting the unrest generated by the crisis, a faction of Communist radicals took up arms and wanted to overthrow the government.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg06.png", 6);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The government response was to quench the unrest with their own forces. However, the Communist radicals' propaganda machine had grown too influential.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg07.png", 7);
                },
                null,
                null,
                null
                ));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They were accountable for the bloodshed, but their leaflets and posts on message boards managed to shift the blame towards the government for many people.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "After various smaller conflicts with mixed results, some of the republic's military generals defected to the Communist side and recommissioned abandoned Soviet bases, using them to produce military equipment.";
                    storyDisplay.AddSimpleStoryImage("Story/CR01/bg09.png", 9);
                },
                null,
                null,
                null
                ));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This is an unacceptable escalation in the situation. As the government military organization was ill-equipped to deal with this force, the government has assigned a new Commander to handle the situation.";
                },
                null,
                null,
                null
                ));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    TryPlaySong(fac2226m);

                    storyDisplay.ConversationDisplay.ConversationText = "This is your first assignment.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                },
                null,
                null,
                storyDisplay => { storyDisplay.AddSimpleStoryImage("Story/CR01/bg10.png", 10, 1.0f); }
                ));

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
                storyDisplay => storyDisplay.ClearStoryImages()
                ));

            return phases;
        }
    }
}
