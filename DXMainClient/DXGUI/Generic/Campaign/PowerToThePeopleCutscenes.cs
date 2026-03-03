using ClientCore;
using DTAClient.Domain.Singleplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Rampastring.XNAUI;
using System;
using System.Collections.Generic;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class PowerToThePeopleCutscenes
    {
        private const float ENDING_ALPHA_RATE = -0.25f;

        public PowerToThePeopleCutscenes(CutsceneResources assets, IStoryDisplay storyDisplay, ICutsceneManager cutsceneManager, WindowManager windowManager)
        {
            this.assets = assets;
            this.storyDisplay = storyDisplay;
            this.cutsceneManager = cutsceneManager;
            this.windowManager = windowManager;
        }

        private readonly CutsceneResources assets;
        private readonly IStoryDisplay storyDisplay;
        private readonly ICutsceneManager cutsceneManager;
        private readonly WindowManager windowManager;

        private const double crRAdisplayX = 4.0;
        private const double crRAdisplayY = 20.0;
        private const double crRAdisplayMiddle = 234.0;
        private const double crRAdisplayImageX = crRAdisplayX + 30.0; // the RA border thingies are 30 pixels wide at left and right
        private const double crRAdisplayImageRate = 300.0;
        private const double crRAdisplayImageWidth = 400.0;
        private const double crRAdisplayImageY = crRAdisplayY + 16.0; // the RA border thingies are 16 pixels high at the top
        private const double crTDdisplayImageY = crRAdisplayY + 19.0; // the TD bordier thingies are 19 pixels high at the top
        private const float crRAdisplayAlphaRate = 5.0f;

        private void AddRADisplay(int id, string path = "Story/CR/rasides.png")
        {
            var rasides = storyDisplay.AddSimpleStoryImage(path, id, 0);
            rasides.AlphaRate = crRAdisplayAlphaRate;
            rasides.ImageX.SnapToValue(crRAdisplayX);
            rasides.ImageY.SnapToValue(crRAdisplayY);
            assets.bleep9.Play();
        }

        private void AddRADisplayImage(string imagePath, int id)
        {
            var image = storyDisplay.AddSimpleStoryImage(imagePath, id, 1);
            image.ImageX.Value = crRAdisplayMiddle;
            image.ImageX.TargetValue = crRAdisplayImageX;
            image.ImageX.Rate = crRAdisplayImageRate;
            image.ImageWidth.Value = 1.0;
            image.ImageWidth.TargetValue = crRAdisplayImageWidth;
            image.ImageWidth.Rate = image.ImageX.Rate * 2.0;
            image.ImageY.SnapToValue(crRAdisplayImageY);
            assets.bleep11.Play();
        }

        private void AddTDDisplay(int id, string path = "Story/CR/tdsides.png")
        {
            var tdsides = storyDisplay.AddSimpleStoryImage(path, id, 0);
            tdsides.AlphaRate = crRAdisplayAlphaRate;
            tdsides.ImageX.SnapToValue(crRAdisplayX);
            tdsides.ImageY.SnapToValue(crRAdisplayY);
            assets.world2.Play();
        }

        private void AddTDDisplayImage(string imagePath, int id)
        {
            var image = storyDisplay.AddSimpleStoryImage(imagePath, id, 0);
            image.ImageY.SnapToValue(crTDdisplayImageY);
            image.ImageX.SnapToValue(crRAdisplayImageX);
            image.AlphaRate = 5.0f;
            assets.beepy2.Play();
        }

        public List<Phase> PTTP1()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
               storyDisplay =>
               {
                   cutsceneManager.TryPlaySong(assets.secondhand);
                   MediaPlayer.IsRepeating = true;
                   MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                   storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                   storyDisplay.ConversationDisplay.IsCentered = true;
                   storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide1.png", 1);
                   assets.country4.Play();
               },
               null,
               null,
               null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide2.png", 2);
                    storyDisplay.ConversationDisplay.ConversationText = "In today's news: what some are labeling as a \"Tiberium War\" continues to engulf the African continent.";
                    assets.mapwipe5.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    assets.country1.Play();
                },
                 storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The latest victim: the Republic of Serunda, which saw Nod forces cross the border only just last month.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide3.png", 3);
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
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Global Defense Initiative peacekeepers have stationed themselves at key positions across the nation and taken over the functions of the former Serundan government.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide4.png", 3);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI General Mark Jamison Sheppard has stated publicly that GDI has intervened in Serunda \"to prevent civil disorder.\"";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide5.png", 4);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Serunda, of course, is no stranger to conflict.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide6.png", 5);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The quasi-anarchist Serundan Liberation Front, or SLF, fought a guerrilla war against the government for decades.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide7.png", 6);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Their charismatic leader, Mthunzi Gumede, led SLF resistance right up until his passing just two years ago, with his daughter, Ulwazi, succeeding him as head of the group.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide8.png", 7);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But it's safe to say that in the face of this massive military and ideological conflict between GDI and Nod, whatever relevance the SLF may once have had in Serunda has long since deteriorated to insignificance.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide9.png", 9);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide1.png", 1);
                    assets.toney7.Play();
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide11.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide10.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Are you receiving this message? ...Good. Our equipment barely works sometimes.";
                    assets.country1.Play();
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
                 storyDisplay => AddRADisplay(3, "Story/PTTP/rasidesred.png")));

            phases.Add(new Phase(17,
                 storyDisplay =>
                 {
                     AddRADisplayImage("Story/PTTP/PTTP1/small1.png", 4);
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
                    AddRADisplayImage("Story/PTTP/PTTP1/small2.png", 5);
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
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(5).AlphaRate = -crRAdisplayAlphaRate; assets.bleep17.Play(); }));

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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP1/slide11.png", 1);
                    assets.toney7.Play();

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

        public List<Phase> PTTP2()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP2/slide1.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP2/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "You've done well - Mayardit is an important figure to the people locally, and he has many significant connections.";
                    assets.country1.Play();
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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP2/slide1.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP2/slide3.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "Ah, my friend! I am very grateful to you for your efforts in protecting myself and my village.";
                    assets.country1.Play();
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
                storyDisplay => AddRADisplay(3, "Story/PTTP/rasidesred.png")));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But when the eagle stepped in to take the reins, not only did they not release these people, but they moved them to a new prison deep in the desert.";
                    AddRADisplayImage("Story/PTTP/PTTP2/small1.png", 4);
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
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(4).AlphaRate = -crRAdisplayAlphaRate; assets.bleep17.Play(); }));

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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP2/slide1.png", 1);
                    assets.toney7.Play();
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

        public List<Phase> PTTP3()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP3/slide1.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP3/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Your actions are bearing fruit. Word spreads through the villages and the towns that the SLF is back, and that this time around it is truly acting in defence of the people.";
                    assets.country1.Play();
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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP3/slide3.png", 3);
                    assets.bleep11.Play();
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "My friend; Ulwazi. Things become worse.";
                },
                null,
                null,
                storyDisplay => AddRADisplay(4, "Story/PTTP/rasidesred.png")));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP3/small1.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "The historic villages of Ondari and Darshad are now between the frontlines of the eagle and the scorpion. They will be little but smoking rubble in a matter of days.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(3);
                    assets.bleep12.Play();
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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP3/slide3.png", 3);
                    assets.bleep11.Play();
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
                    assets.bleep12.Play();
                }));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Agreed.  It's decided then.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(4).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(5).AlphaRate = -crRAdisplayAlphaRate; assets.bleep17.Play(); }));

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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP3/slide1.png", 1);
                    assets.toney7.Play();

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

        public List<Phase> PTTP4()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide1.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "During the Second Great War, the Soviet Union supplied funds and conventional weapons to anti-colonial movements across Africa and Asia.";
                    assets.mapwipe5.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The SLF, initially founded as a response to British colonization of Serunda, took full advantage of the Soviet supplies on offer and fought the British for years.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide3.png", 3);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "At war's end, despite the collapse of the USSR, European powers found little relief in their beleaguered colonies, and abandoned them either by choice or at the end of a gun.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide4.png", 3);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In Serunda, there was a peaceful transition of power.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide11.png", 3);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It was to last only a matter of weeks.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide5.png", 5);
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Serundan military quickly overthrew the civilian government, turning the new Republic of Serunda into an autocratic dictatorship and forcing the SLF back into a campaign of guerrilla resistance.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The fighting reignited and intensified as both sides ruthlessly attempted to eradicate their opposition, committing numerous atrocities across Serunda.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide6.png", 7);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Decades of strife and suffering passed before the Serundan military came out on top.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide7.png", 9);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Republic of Serunda was in dire straits, ripe for yet another terrible upheaval as it entered a new era - the Tiberian era.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide8.png", 9);
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide1.png", 1);
                    assets.toney7.Play();
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide9.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide10.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Hello.  I am aware you do not know who I am.  You may call me Jamal.";
                    assets.country1.Play();
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
                storyDisplay => AddRADisplay(3, "Story/PTTP/rasidesred.png")));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP4/small1.png", 4);
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
                    AddRADisplayImage("Story/PTTP/PTTP4/small2.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "So, you will get these funds. You will harvest Tiberium from the Nkoma valley, where GDI and Nod are currently thrashing it out, and then you will retreat.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP4/small3.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "You will prioritize eliminating Nod's SAM Sites.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP4/small4.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "The local A-10 colonel is corrupt, and will gladly go after the Brotherhood if he can grab all the glory without risking his planes.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP4/small5.png", 8);
                    storyDisplay.ConversationDisplay.ConversationText = "You will NOT attack the major GDI airbase there - it is too high profile a target. Frankly, I think our past engagements with GDI have already flown us too close to the sun.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(7); storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(8).AlphaRate = -crRAdisplayAlphaRate; assets.bleep17.Play(); }));

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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP4/slide9.png", 1);
                    assets.toney7.Play();

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

        public List<Phase> PTTP5()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP5/slide1.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP5/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Good work.  Ulwazi is still busy.";
                    assets.country1.Play();
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
                storyDisplay => AddRADisplay(3, "Story/PTTP/rasidesred.png")));

            phases.Add(new Phase(4,
               storyDisplay =>
               {
                   AddRADisplayImage("Story/PTTP/PTTP5/small1.png", 4);
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
                    AddRADisplayImage("Story/PTTP/PTTP5/small2.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "I will be clear - we have no use for the detonator itself.  It would only bring heat on us politically, and potentially alienate us from the people.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP5/small3.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "However, a nuclear detonator is something we could definitely sell to whoever we end up bartering with in the former USSR.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(5)));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP5/small4.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "We know little of Nod's specific heist plans for the detonator, other than that they will launch the operation from a nearby base, and try to exfiltrate it out of the area through a route unknown to us.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(6)));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP5/small5.png", 8);
                    storyDisplay.ConversationDisplay.ConversationText = "You will lead a small team to capture one of their communications facilities.  Once inside, you should have a better idea of how to hijack their efforts.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(7)));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP5/small6.png", 9);
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
                storyDisplay => { storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate; storyDisplay.FindStoryImageById(9).AlphaRate = -crRAdisplayAlphaRate; assets.bleep17.Play(); }));

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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP5/slide1.png", 1);
                    assets.toney7.Play();
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

        public List<Phase> PTTP6()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide1.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "In today's news: the situation in the Republic of Serunda has undergone a number of surprising new developments.";
                    assets.mapwipe5.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The SLF, previously seen as a relic of a bygone era, has launched a series of small but undeniably successful operations across the country.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide3.png", 3);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Both GDI and Nod forces have found themselves surprised by the effect the SLF is having on their activities.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide4.png", 3);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide5.png", 3);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide6.png", 3);
                    storyDisplay.ConversationDisplay.ConversationText = "It's even been rumored that Nod's commander on the Serundan front has been replaced for incompetence - which, as we understand Nod's practices, usually means execution.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide7.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "Western intelligence analysts estimate that the number of SLF soldiers may have increased by as much as tenfold since they began this latest set of actions.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide8.png", 7);
                    storyDisplay.ConversationDisplay.ConversationText = "While the SLF has engaged GDI forces before, it has largely steered clear of attacking major facilities.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide9.png", 9);
                    storyDisplay.ConversationDisplay.ConversationText = "This may not stop GDI from taking a heavier hand with the rebel group, as General Mark Jamison Sheppard remarked today at a press conference that there is \"growing concern\" among the GDI leadership at what's happening in the Republic of Serunda.";
                },
                null,
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide1.png", 1);
                    assets.toney7.Play();
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide10.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide11.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Comrade - I have good news and bad news.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                storyDisplay => AddRADisplay(3, "Story/PTTP/rasidesred.png")));

            phases.Add(new Phase(15,
                 storyDisplay =>
                 {
                     AddRADisplayImage("Story/PTTP/PTTP6/small1.png", 4);
                     storyDisplay.ConversationDisplay.ConversationText = "The good news is that I successfully made contact with interested parties in the former Soviet Union.";
                 },
                 null,
                 null,
                 null));

            phases.Add(new Phase(16,
                 storyDisplay =>
                 {
                     AddRADisplayImage("Story/PTTP/PTTP6/small2.png", 5);
                     storyDisplay.ConversationDisplay.ConversationText = "We have agreed on a deal - the funds and the detonator for valuable Soviet military equipment and the schematics to produce it.";
                 },
                 null,
                 null,
                 storyDisplay => storyDisplay.RemoveStoryImageById(4)));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP6/small3.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "It is to arrive in two shipments - the first shipment made landfall a few days ago, and we have already begun making use of the technology.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    assets.bleep11.Play();
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide13.png", 7);
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
                    assets.bleep11.Play();
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide12.png", 8);
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
                    assets.bleep11.Play();
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Well, within a certain -";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    assets.bleep11.Play();
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide12.png", 8);
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
                    assets.bleep11.Play();
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "I'm not sure I -";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    assets.bleep11.Play();
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide12.png", 8);
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
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide11.png", 2);
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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP6/slide10.png", 1);
                    assets.toney7.Play();
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

        public List<Phase> PTTP7()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide1.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "The SLF's leadership structure has remained consistent for decades. In theory, if not in practice. ";
                    assets.mapwipe5.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In theory, the SLF is organized according to anarcho-syndicalist principles, and governed by a Liberation Council of a dozen different individuals.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide3.png", 3);
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
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Decision on lesser matters is handled by smaller sub-councils, sometimes affiliated with military units, sometimes affiliated with towns or villages.";
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide4.png", 3);
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
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide5.png", 3);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide6.png", 7);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide7.png", 7);
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
                storyDisplay => { storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f); assets.country1.Play(); },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "<CLOSING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide1.png", 1);
                    assets.toney7.Play();
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide8.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide9.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                    storyDisplay.ConversationDisplay.ConversationText = "Ah, my friend!  It is so good that you managed to save our facility.";
                    assets.country1.Play();
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
                storyDisplay => AddRADisplay(3, "Story/PTTP/rasidesred.png")));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP7/small1.png", 4);
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
                    AddRADisplayImage("Story/PTTP/PTTP7/small2.png", 5);
                    storyDisplay.ConversationDisplay.ConversationText = "With these troops, I must ask you to punish the eagle.  Destroy their holdings, raze their structures, kill their troops.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(4);
                    storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate;
                    storyDisplay.FindStoryImageById(5).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP7/slide8.png", 1);
                    assets.toney7.Play();
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

        public List<Phase> PTTP8()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP8/slide1.png", 1);
                    assets.country4.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP8/slide2.png", 2);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "We must speak quickly.  I don't know what Mayardit was thinking with that last operation.";
                    assets.country1.Play();
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
                storyDisplay => AddRADisplay(3, "Story/PTTP/rasidesred.png")));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    AddRADisplayImage("Story/PTTP/PTTP8/small1.png", 4);
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
                    AddRADisplayImage("Story/PTTP/PTTP8/small2.png", 5);
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
                    AddRADisplayImage("Story/PTTP/PTTP8/small3.png", 6);
                    storyDisplay.ConversationDisplay.ConversationText = "You will also destroy the Nod base planning an offensive across the river using our new V2 Launchers - this plan might as well be dust in the wind if the Brotherhood levels the facility.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(5);
                    storyDisplay.FindStoryImageById(3).AlphaRate = -crRAdisplayAlphaRate;
                    storyDisplay.FindStoryImageById(6).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
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
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP8/slide1.png", 1);
                    assets.toney7.Play();
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

        public List<Phase> PTTP8Victory()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP8/slide1.png", 1);
                    assets.country4.Play();

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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP8/slide3.png", 2);
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "Comrade.  I'm not sure I fully understand why you decided to continue with the operation.";
                        assets.country1.Play();
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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP8/slide1.png", 1);
                        assets.toney7.Play();
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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP8/slide3.png", 2);
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "Comrade.  Thank you for abandoning the operation so quickly.";
                        assets.country1.Play();
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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP8/slide1.png", 1);
                        assets.toney7.Play();
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

        public List<Phase> PTTP9()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.gloom);

                    storyDisplay.ConversationDisplay.ConversationText = "<OPENING CONNECTION...>";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide1.png", 1);
                    assets.country4.Play();
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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide2.png", 2);
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "The day is finally here.  Across Serunda, our well-equipped forces are prepared to launch a simultaneous offensive that will throw the occupiers out of our country!";
                        assets.country1.Play();

                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(1),
                    null,
                    null));

                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        assets.bleep11.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide3.png", 3);
                        storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                        storyDisplay.ConversationDisplay.ConversationText = "I don't understand why our \"friend\" is here.  Has he not proved himself to be in line with the traitor Jamal?";
                    },
                    null,
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(3); assets.bleep12.Play(); }));

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
                        assets.bleep11.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide3.png", 3);
                        storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                        storyDisplay.ConversationDisplay.ConversationText = "Approving the plan is one thing. Placing our friend in charge of it is asking for trouble.";
                    },
                    null,
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(3); assets.bleep12.Play(); }));

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
                        assets.bleep17.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide4.png", 3);
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
                    storyDisplay => AddRADisplay(4, "Story/PTTP/rasidesred.png")));

                phases.Add(new Phase(9,
                    storyDisplay =>
                    {
                        AddRADisplayImage("Story/PTTP/PTTP9/small1.png", 5);
                        storyDisplay.ConversationDisplay.ConversationText = "You will attack and level GDI's largest stronghold in our country at Jazahiri, which contains a sizable air contingent and their Ion Cannon Uplink.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(10,
                    storyDisplay =>
                    {
                        AddRADisplayImage("Story/PTTP/PTTP9/small4.png", 6);
                        storyDisplay.ConversationDisplay.ConversationText = "You will use the saboteurs placed in GDI's Mammoth Tanks to launch a sneak attack on their operations there and cripple their infrastructure.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(5)));

                phases.Add(new Phase(11,
                    storyDisplay =>
                    {
                        AddRADisplayImage("Story/PTTP/PTTP9/small2.png", 7);
                        storyDisplay.ConversationDisplay.ConversationText = "You will also eliminate the Brotherhood forces.  They have constructed a Temple in a desperate effort to justify their relevance.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(6)));

                phases.Add(new Phase(12,
                    storyDisplay =>
                    {
                        AddRADisplayImage("Story/PTTP/PTTP9/small3.png", 8);
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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide1.png", 1);
                        assets.toney7.Play();
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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide2.png", 2);
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "The day is finally here.  Across Serunda, our well-equipped forces are prepared to launch a simultaneous offensive that will throw the occupiers out of our country!";
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(1),
                    null,
                    null));

                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        assets.bleep11.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide3.png", 3);
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
                    storyDisplay => { storyDisplay.RemoveStoryImageById(3); assets.bleep12.Play(); }));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                        storyDisplay.ConversationDisplay.ConversationText = "Yes, indeed.  Comrade, you will ignite the fire that burns Serunda clean.";
                    },
                    null,
                    null,
                    storyDisplay => AddRADisplay(4, "Story/PTTP/rasidesred.png")));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        AddRADisplayImage("Story/PTTP/PTTP9/small1.png", 5);
                        storyDisplay.ConversationDisplay.ConversationText = "You will attack and level GDI's largest stronghold in our country at Jazahiri, which contains a sizable air contingent and their Ion Cannon Uplink.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        AddRADisplayImage("Story/PTTP/PTTP9/small2.png", 6);
                        storyDisplay.ConversationDisplay.ConversationText = "You will also eliminate the Brotherhood forces.  They have constructed a Temple in a desperate effort to justify their relevance.";
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.RemoveStoryImageById(5)));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        AddRADisplayImage("Story/PTTP/PTTP9/small3.png", 7);
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
                        assets.bleep11.Play();
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide3.png", 3);
                        storyDisplay.ConversationDisplay.TextColor = Color.SandyBrown;
                        storyDisplay.ConversationDisplay.ConversationText = "I will personally assist you with my forces on the battlefield, Comrade.  It will be a most glorious day.";
                    },
                    null,
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(3); assets.bleep12.Play(); }));

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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide1.png", 1);
                        assets.toney7.Play();
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

        public List<Phase> PTTPEnd()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(0,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.chrg226m);

                    storyDisplay.ConversationDisplay.ConversationText = "The SLF's overwhelming victory at Jazahiri caught both GDI and the Brotherhood by surprise, and threw their troops into disarray.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide5.png", 1);
                    assets.mapwipe2.Play();
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
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            var wfCapturedVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_PTTP_WF_CAPTURED");
            if (wfCapturedVariable != null && wfCapturedVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide6.png", 1);
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
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide10.png", 1);
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
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide12.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "While the war raged on in Africa between GDI and Nod, in Serunda, the war was over.  However, the war was not gone.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide8.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "It was etched in the faces of those who had survived, and scarred in the craters. It burned in the still-growing Tiberium fields of the countryside.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide9.png", 1);
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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/endingtitle.png", 1);
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "  ENDING II" + Environment.NewLine + "Leap of Faith";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                        storyDisplay.ConversationDisplay.ConversationText = "";
                        storyDisplay.ConversationDisplay.IsCentered = false;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }
            else
            {
                phases.Add(new Phase(2,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide6.png", 1);
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
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide7.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "Ulwazi did not rule for long.  Within months, she had vanished from the public eye, her co-executive Mayardit now the face of the People's Republic of Serunda. She was not the last to disappear.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide12.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "While the war raged on in Africa between GDI and Nod, in Serunda, the war was over.  However, the war was not gone.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide8.png", 1);
                        storyDisplay.ConversationDisplay.ConversationText = "It was etched in the faces of those who had survived, and scarred in the craters. It burned in the still-growing Tiberium fields of the countryside.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        assets.country1.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/slide9.png", 1);
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
                        storyDisplay.AddSimpleStoryImage("Story/PTTP/PTTP9/endingtitle.png", 1);
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "ENDING I" + Environment.NewLine + "Full Circle";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = ENDING_ALPHA_RATE);
                        storyDisplay.ConversationDisplay.ConversationText = "";
                        storyDisplay.ConversationDisplay.IsCentered = false;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }
            AddPTTPCreditPhases(phases, Color.Red);
            return phases;
        }

        private void AddPTTPCreditPhases(List<Phase> phases, Color color)
        {
            int lastPhaseID = phases[phases.Count - 1].ID;

            phases.Add(new Phase(lastPhaseID + 1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.tdmaptheme);
                    storyDisplay.ConversationDisplay.ConversationText = "";

                    const double rate = 60.0;

                    var creditsStoryImage = new StoryImage(windowManager, 1);
                    creditsStoryImage.Texture = AssetLoader.LoadTextureUncached("Story/PTTP/creditsp1.png");
                    creditsStoryImage.Color = color;
                    creditsStoryImage.ImageHeight.SnapToValue(creditsStoryImage.Texture.Height);
                    creditsStoryImage.ImageY.Value = windowManager.RenderResolutionY;
                    creditsStoryImage.ImageY.TargetValue = -creditsStoryImage.Texture.Height;
                    creditsStoryImage.ImageY.Rate = rate;
                    storyDisplay.AddStoryImage(creditsStoryImage);

                    var creditsStoryImage2 = new StoryImage(windowManager, 2);
                    creditsStoryImage2.Texture = AssetLoader.LoadTextureUncached("Story/PTTP/creditsp2.png");
                    creditsStoryImage2.Color = color;
                    creditsStoryImage2.ImageHeight.SnapToValue(creditsStoryImage2.Texture.Height);
                    creditsStoryImage2.ImageY.Value = creditsStoryImage.ImageY.Value + creditsStoryImage.ImageHeight.Value;
                    creditsStoryImage2.ImageY.TargetValue = 0;
                    creditsStoryImage2.ImageY.Rate = rate;
                    storyDisplay.AddStoryImage(creditsStoryImage2);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));
        }

    }
}
