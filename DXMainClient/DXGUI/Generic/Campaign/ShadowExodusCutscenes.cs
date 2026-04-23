using ClientCore;
using DTAClient.Domain.Singleplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Rampastring.XNAUI;
using System;
using System.Collections.Generic;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class ShadowExodusCutscenes
    {
        private const float ENDING_ALPHA_RATE = -0.25f;

        public ShadowExodusCutscenes(CutsceneResources assets, IStoryDisplay storyDisplay, ICutsceneManager cutsceneManager, WindowManager windowManager)
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
        private const double crRAdisplayImageX = crRAdisplayX + 30.0; // the RA border thingies are 30 pixels wide at left and right
        private const double crTDdisplayImageY = crRAdisplayY + 19.0; // the TD bordier thingies are 19 pixels high at the top
        private const float crRAdisplayAlphaRate = 5.0f;
        private const float crTDdisplayImageDisappearAlphaRate = -2.5f;

        private void AddTDDisplay(int id, string path = "Story/CR/tdsides.png", bool playSound = true, float alphaRate = 5.0f)
        {
            var tdsides = storyDisplay.AddSimpleStoryImage(path, id, 0);
            tdsides.AlphaRate = alphaRate;
            tdsides.ImageX.SnapToValue(crRAdisplayX);
            tdsides.ImageY.SnapToValue(crRAdisplayY);

            if (playSound)
                assets.world2.Play();
        }

        private void AddTDDisplayImage(string imagePath, int id, bool playSound = true, float alphaRate = 5.0f)
        {
            var image = storyDisplay.AddSimpleStoryImage(imagePath, id, 0);
            image.ImageY.SnapToValue(crTDdisplayImageY);
            image.ImageX.SnapToValue(crRAdisplayImageX);
            image.AlphaRate = alphaRate;

            if (playSound)
                assets.beepy3.Play();
        }

        public List<Phase> SE01()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.tdmaptheme);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;
                
                    storyDisplay.ConversationDisplay.ConversationText = "* * * ACCESSING BROTHERHOOD ARCHIVES * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNodArchives.png", 2);
                    storyDisplay.ConversationDisplay.ConversationText = "The Brotherhood desires a world of peace, unity, and eternal brotherhood.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    assets.mapwipe2.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Brotherhood springs from the lowest of places, offering unity and peace to otherwise neglected and abused nations.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE01/slide2.png", 3);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(2),
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Tiberium heralds the dawn of a new age.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE01/slide3.png", 4);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(3),
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Brotherhood embraces this age, harvesting Tiberium to further expand our collective---";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE01/slide4.png", 5, 0f);
                    assets.bleep11.Play();
                },
                storyDisplay =>
                {
                    cutsceneManager.StopMusic();
                    assets.powrdn1.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "* * * ARCHIVES CONNECTION INTERRUPTED * * *";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/largestatic.png", 6, 1f);
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.RemoveStoryImageById(4);
                    storyDisplay.RemoveStoryImageById(5);
                },
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 7);
                    assets.newtarg1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(6),
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.Text = "???";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Enough. I won't tolerate you wasting time on that orientation video.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 8);

                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(7),
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    assets.bleep11.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "I'm Lydia. Let's get something straight...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You may technically report to General Morgan, but the General is a busy man. It would be beneath him to meet and greet with every fresh faced officer that steps in to replace the last casualty.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As the General's adjutant, it therefore falls to me to assess whether or not you're worthy of command. Lucky me.";
                    AddTDDisplay(50);
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Your task is simple. GDI has outstretched their supply lines along the Serbian-Romanian border, and they've left a moderately important logistics hub with nothing but a meager garrison force.";
                    AddTDDisplayImage("Story/SE/SE01/small1.png", 51);
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We can't afford to hold the area long-term, but a raiding party should be able to inflict serious damage and draw GDI scum away from battles that are actually important to the Brotherhood.";
                    AddTDDisplayImage("Story/SE/SE01/small2.png", 52);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(51),
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Destroy everything. Kill them all.";
                    AddTDDisplayImage("Story/SE/SE01/small3.png", 53);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(52),
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(53).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If your forces get bogged down out there, GDI reinforcements will likely arrive before you can finish the job. So move with purpose.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Remember, if you fail, it's your head on the chopping block - not mine.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(53); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(8).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
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

        public List<Phase> SE02()
        {
            var phases = new List<Phase>();
            var movedFastVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_GDI_BASE_DESTROYED_QUICKLY");

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 2);
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "You're just in time. General Morgan wants to speak to you.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I'll put him through.";
                    AddTDDisplay(50);

                },
                null,
                storyDisplay => { AddTDDisplayImage("Story/SE/Shared/SmallMorgan.png", 51); },
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(51);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "You must be my new Commander. Getting your hands nice and bloody, I see.";

                },
                null,
                null,
                null));

            if (movedFastVariable != null && movedFastVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You were able to raze that logistics facility impressively quickly, Commander. You clearly know how to move rapidly.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "I'll dispatch a few of my more experienced Recon Bike drivers to you for the next operation. Lydia will give you more details on the operation itself. Morgan out.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        StoryImage borderDraw = storyDisplay.FindStoryImageById(51);
                        borderDraw.DrawBorders = false;
                        assets.bleep12.Play();
                        AddTDDisplayImage("Story/SE/Shared/SmallTerm.png", 52);
                        storyDisplay.HeaderDisplay.Text = "LYDIA";
                        storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                        storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                        storyDisplay.ConversationDisplay.ConversationText = "Do you actually know how to move rapidly? Or was that just beginner's luck out there?";
                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(51),
                    null,
                    null));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Let's see if you can repeat your little performance...";
                    },
                    null,
                    null,
                    null));
            }

            else
            {
                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Still, I'm unimpressed by how slowly you moved out there. You're not at the head of some GDI armored column, Commander.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Try to keep that in mind during your next operation. Lydia will give you more details. Morgan out.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        StoryImage borderDraw = storyDisplay.FindStoryImageById(51);
                        borderDraw.DrawBorders = false;
                        assets.bleep12.Play();
                        AddTDDisplayImage("Story/SE/Shared/SmallTerm.png", 52);
                        storyDisplay.HeaderDisplay.Text = "LYDIA";
                        storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                        storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                        storyDisplay.ConversationDisplay.ConversationText = "Wow. Really starting off on the right foot with the General, aren't you?";
                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(51),
                    null,
                    null));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Well, you know what they say - second impressions are everything...";
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As I predicted, GDI retook the area holding the former logistics hub relatively quickly.";
                    AddTDDisplayImage("Story/SE/SE02/small3.png", 53);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(52),
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, the troops they used to launch their counteroffensive had to be pulled from other parts of their frontline.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Notably, GDI vermin around the mountain town of Sinaia are now considerably weaker in military strength.";
                    AddTDDisplayImage("Story/SE/SE02/small4.png", 54);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(53),
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Local government officials love to holiday there - which makes it a perfect place to manipulate public opinion and embarrass GDI.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You see, the mountainous terrain surrounding Sinaia is difficult enough - and wartime conditions strenuous enough - that food now has to be imported into the town via supply truck convoys.";
                    AddTDDisplayImage("Story/SE/SE02/small5.png", 55);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(54),
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Intercept these convoys and prevent them from reaching Sinaia.  Starve the population until they realize that no amount of GDI protection is worth an empty stomach.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Keep in mind that while your troops should be able to roam relatively freely around the mountain passes, the enemy still has heavy defenses at the entrances to Sinaia itself.";
                    AddTDDisplayImage("Story/SE/SE02/small6.png", 56);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(55),
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "These defenses include what seem like particularly well armed Advanced Guard Towers that will likely make short work of any raiding party you send too close to the town.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Those GDI fools are relying on such static emplacements to make up for their sudden lack of manpower, but holing up behind those walls only cedes the mobility advantage to your forces.";
                    AddTDDisplayImage("Story/SE/SE02/small7.png", 57);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(56),
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(57).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Get out there and drive a wedge between GDI and the locals. Any instability you can create here might help relieve pressure on our Commanders fighting elsewhere.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You know - the Commanders we trust enough to give operations that are actually important.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(57); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
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

        public List<Phase> SE02End()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * OPENING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoWWN.png", 1);
                    assets.mapwipe5.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE02End/slide1.png", 2);
                    storyDisplay.HeaderDisplay.Text = "BURDETTE";
                    storyDisplay.HeaderDisplay.SetColors(Color.AntiqueWhite);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.AntiqueWhite;
                    storyDisplay.ConversationDisplay.ConversationText = "The streets of Sinaia are littered with the bodies of its citizens. Innocent, everyday people, who rose up against GDI occupation and paid the price.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Why did GDI terrorists murder the citizens of Sinaia? There's a simple reason: the residents were hungry.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE02End/slide3.png", 3);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(2),
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI's rationing regime has kept the town in a state of near starvation.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "After the contents of several supply convoys were confiscated by GDI higher ups, the desperate townspeople could take no more.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE02End/slide2.png", 4);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(3),
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They assembled in the center of town for a peaceful protest - only to be slaughtered by machine-gun and cannon fire at the hands of GDI war criminals.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    assets.world2.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "Government officials saw the horrific massacre and are demanding answers.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE02End/slide4.png", 5);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But the only answer to be found in Sinaia is this: General Sheppard and those who serve him won't tolerate anyone who speaks out against the authoritarian rule of GDI's new world order.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    assets.bleep17.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "This is Greg Burdette, Sinaia.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE02End/slide1.png", 6);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(6).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * CLOSING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoWWN.png", 99);
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

        public List<Phase> SE03()
        {
            var phases = new List<Phase>();
            var blueprintVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_CAPTURED_MAMMOTH_BLUEPRINTS");

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;
                
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 2);
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Well, well. You caused quite the stir at Sinaia, didn't you?";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            if (blueprintVariable != null && blueprintVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(3,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You even managed to steal what looks like Mammoth Tank blueprints from those GDI idiots. An interesting find, although I'm not sure we'll be able to make use of them right away.";

                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "That being said, I think it's time for you to take a backseat for a while.";
                    AddTDDisplay(50);
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There's a large Tiberium harvesting facility south of the Danube that requires...";
                    AddTDDisplayImage("Story/SE/SE03/small1.png", 51);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...hmm. General Morgan is calling.  Let me put him through.";
                },
                null,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/SmallMorgan.png", 52);
                },
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(52);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Lydia. I want you to put the Commander on the Brasov operation.";

                },
                storyDisplay => storyDisplay.RemoveStoryImageById(51),
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(52);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Respectfully, General, I don't think the Commander is-";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(52);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Just do it.  I won't tell you again.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(52);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                    AddTDDisplayImage("Story/SE/Shared/SmallTerm.png", 53);
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Since the General insists... I'll fill you in.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(52),
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE03/small2.png", 54);
                    storyDisplay.ConversationDisplay.ConversationText = "In the wake of the riot at Sinaia, the local government has become divided. The politicians who witnessed the massacre have fallen prey to their emotions and are now firmly anti-GDI.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(53),
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE03/small3.png", 55);
                    storyDisplay.ConversationDisplay.ConversationText = "However, those far from Sinaia, within the capital, understand that the Brotherhood was responsible for halting the food convoys.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(54),
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This faction of politicians has requested a larger troop commitment from GDI.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE03/small4.png", 56);
                    storyDisplay.ConversationDisplay.ConversationText = "No doubt hoping to smooth things over with the local elites, GDI has since publicly stated that they will set up a new base outside of the capital.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(55),
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE03/small5.png", 57);
                    storyDisplay.ConversationDisplay.ConversationText = "Unfortunately for them, I have it on good authority that their MCV will pass near the city of Brasov.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(56),
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This route is close to our frontlines - presenting us with an opportunity to drive the knife deeper into the wound you created at Sinaia.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE03/small6.png", 58);
                    storyDisplay.ConversationDisplay.ConversationText = "Take a small group of our forces and hijack GDI's MCV. Then use GDI's equipment to destroy everything that matters in Brasov - its history, its institutions, its sanctuaries.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(57),
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It must be made absolutely clear who the real enemy of the people is. Make sure to leave the mayor alive, so that we have a credible authority who can testify to \"GDI's\" crimes. ";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(58).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "And keep Brotherhood equipment away from the locals - we can't have them figuring out who's really responsible for what's about to happen.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You seem to have caught the General's interest. Pray you're worthy of it.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(58); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
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

        public List<Phase> SE04()
        {
            var phases = new List<Phase>();
            var powerVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_DESTROYED_POWER_STATION");

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * OPENING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoWWN.png", 1);
                    assets.mapwipe5.Play();
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "Horrific news from the city of Brasov today, where indiscriminate violence was unleashed upon innocent civilians by Global Defense Initiative \"peacekeepers\".";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE04/slide1.png", 2, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The mayor of Brasov, who has lived in the area his entire life, stated today that \"the city has never seen such carnage, not even during the Soviet occupation decades ago.\" Most of Brasov now lies in ruin.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE04/slide2.png", 3, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            if (powerVariable != null && powerVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(4,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Electrical infrastructure was also destroyed during the attack, plunging the city into darkness. Deaths among the hospitalized and elderly have been attributed to the subsequent loss of power.";
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE04/slide3.png", 4, 0f);
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The attack has caused tensions between local forces and GDI to boil over. Numerous skirmishes have broken out, and one local general has even called for the military to support the Brotherhood of Nod in their effort to push GDI out of the region.";
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE04/slide4.png", 5, 0f);
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }
            else
            {
                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The attack has caused tensions between local forces and GDI to boil over. Numerous skirmishes have broken out between the two factions all across the country.";
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE04/slide4.png", 5, 0f);
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI General Mark Jamison Sheppard has denied responsibility for the incident, claiming that the assault on Brasov was a \"sinister Brotherhood trick.\"";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE04/slide5.png", 6, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, it's easy to see how GDI troops may be choosing to vent their frustration elsewhere. A recent Brotherhood counter-offensive has pushed GDI forces back across several borders, and shows no sign of stopping.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE04/slide6.png", 7, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Who will police the world's police? How many towns, how many cities must suffer to ensure GDI's victory in this terrible war? Unfortunately, these are questions that right now, the free world has no answers to.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE04/slide7.png", 8, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => { storyDisplay.ClearStoryImages(); assets.toney4.Play(); }));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "* * * CLOSING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoWWN.png", 9);
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 10);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Morgan.png", 11);
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Don't be fooled by the media, Commander. Remember that they're in our pocket.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(10),
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You may have bloodied GDI's nose, but the reality is this: the enemy is winning this war.";
                    AddTDDisplay(50);
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Nod counterattack the reporters are making so much of was blunted nearly immediately. GDI is on the offensive again. Their forces outnumber ours everywhere.";
                    AddTDDisplayImage("Story/SE/SE04/small1.png", 51);
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The GDI lapdog on our front, General Howard, is not leading a particularly rapid advance, especially because of the political mess you created at Brasov.";
                    AddTDDisplayImage("Story/SE/Shared/SmallHoward.png", 52);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(51),
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, he still has overwhelming armor superiority, and his aviation is eating us alive.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Even Kane's forces in Bosnia are under serious threat. GDI has closed the borders, and contact with Sarajevo has been cut off.";
                    AddTDDisplayImage("Story/SE/SE04/small3.png", 53);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(52),
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(53).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You've proven that it's time to take you off the sideshows. I need you to consolidate what assets you can while we still have the opportunity to do so. Lydia will give you more specifics.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(52),
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));
            ;
            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "One last thing, Commander. Be careful who you share information with. The Brotherhood doesn't look kindly on \"defeatists.\"";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(53); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(11).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * REROUTING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 12);
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 13);

                    AddTDDisplay(60, "Story/CR/tdsides.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/Shared/SmallKorkut.png", 61, false, 1f);
                    storyDisplay.HeaderDisplay.Show();
                    storyDisplay.HeaderDisplay.Text = "???";
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(61);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;

                    storyDisplay.HeaderDisplay.SetColors(Color.BurlyWood);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.BurlyWood;
                    storyDisplay.ConversationDisplay.ConversationText = "...very well.  I will see to the necessary arrangements.";

                    assets.country1.Play();
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(11); storyDisplay.RemoveStoryImageById(12); },
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ah, it seems we have company. You must be General Morgan's new Commander.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I've heard about your work in Brasov - what strife you've sown among the enemies of Kane! A pleasure to meet you - I am General Korkut.";
                    storyDisplay.HeaderDisplay.Text = "KORKUT";
                    assets.bleep11.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(61);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Apologies for the interruption, General.  We can finish this later.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(61);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "KORKUT";
                    storyDisplay.HeaderDisplay.SetColors(Color.BurlyWood);
                    storyDisplay.ConversationDisplay.TextColor = Color.BurlyWood;
                    storyDisplay.ConversationDisplay.ConversationText = "Of course. Goodbye for now.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(61);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                    AddTDDisplayImage("Story/SE/Shared/SmallTerm.png", 62);
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "You don't have clearance for what you just saw, so let's just move on, shall we?";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(61),
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "General Morgan told me he would fill you in on the strategic situation. That means it's time to finally put you on something important.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE04/small4.png", 63);
                    storyDisplay.ConversationDisplay.ConversationText = "Most of our forces are in full-scale retreat after a failed offensive against GDI's frontline.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(62),
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "One notable contingent is carrying a large quantity of critical supplies and equipment, and must be protected from GDI's jack-booted thugs.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE04/small5.png", 64);
                    storyDisplay.ConversationDisplay.ConversationText = "I've redirected this detachment towards one of our port facilities on the Danube.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(63),
                null,
                null));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "This base has already been hit by an oddly precise GDI attack, but it should still be intact enough for you to commandeer it for an evacuation operation.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(31,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Ensure that the port remains operational. Our retreating troops are expendable, but the supplies and equipment moving with them are not.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(32,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Any vehicles containing those materials must be protected until they can be evacuated downriver.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(33,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I lost contact with other, smaller outposts we had in the area during the first GDI attack.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(64).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(34,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You might be able to re-establish contact with those outposts if you can locate them before the bulk of General Howard's forces arrive.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(60).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));
            ;
            phases.Add(new Phase(35,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "No more games, Commander. You're in the thick of it now.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(64); storyDisplay.RemoveStoryImageById(60); },
                storyDisplay => { storyDisplay.FindStoryImageById(13).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(36,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
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

        public List<Phase> SE05()
        {
            var phases = new List<Phase>();
            var radarVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_DESTROYED_RADAR_JAMMER");

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * OPENING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoW3N.png", 1);
                    assets.mapwipe5.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05/slide1.png", 2);
                    storyDisplay.HeaderDisplay.Text = "ANCHOR";
                    storyDisplay.HeaderDisplay.SetColors(Color.Linen);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Linen;
                    storyDisplay.ConversationDisplay.ConversationText = "U.N. spokesmen have confirmed that massive destruction to the alleged Sarajevo headquarters of the world terrorist organization known as the Brotherhood of Nod...";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...was the direct result of an orbiting satellite weapon described by officials as an Ion Cannon.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Destruction of the site ended a three day siege which saw GDI and Nod forces engaged in mortal combat.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05/slide2.png", 3);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(2),
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05/slide3.png", 4);
                    assets.beepy3.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "Eyewitnesses and survivors report that the skirmish may have continued for weeks had it not been for the awesome offensive power of the satellite-based air to land cannon.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(3),
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Quoting from a statement issued today, U.N. Secretary of Defense Charles Olivetti confirmed that heavy casualties were sustained by Nod forces.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05/slide4.png", 5);
                    assets.bleep17.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "However, the use of the star wars technology cut GDI and civilian losses to \"technically acceptable levels.\"";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Contradicting earlier U.N. Security Council denials, Dr. Olivetti confirmed that the Ion Cannon has in fact been under development for several years.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05/slide5.png", 6);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Discovery and destruction of the Nod command center ends more than three years of continued escalating violence sparked by that organization's international terrorist activities.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05/slide6.png", 7);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(6),
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Spokesmen for the Nod consortium were unavailable for comment...";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05/slide6.png", 7);
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(7).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * CLOSING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoW3N.png", 8);
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 9);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    AddTDDisplay(50, "Story/CR/tdsides.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/Shared/SmallKorkut.png", 51, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Morgan.png", 10);
                    storyDisplay.HeaderDisplay.Show();
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "I called you both because I need you to understand something.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(9),
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE05/small1.png", 52);
                    storyDisplay.ConversationDisplay.ConversationText = "Kane is dead. This image has been floating around the Brotherhood intranet and it appears to be genuine.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(51),
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/SmallKorkut.png", 61, false);
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(61);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "KORKUT";
                    storyDisplay.HeaderDisplay.SetColors(Color.BurlyWood);
                    storyDisplay.ConversationDisplay.TextColor = Color.BurlyWood;
                    storyDisplay.ConversationDisplay.ConversationText = "I have already seen this image. But it changes nothing. Kane cannot be defeated by some weapon of GDI. He will return, and in the meantime... we will band together to survive.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(52),
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Though I can't say I find the notion of working with you more closely very appealing, General.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(61);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Yeah, well, that feeling's mutual. Commander, General Korkut here is my \"esteemed colleague\" in charge of Nod forces in Anatolia.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Make no mistake - despite what some are saying, the fall of Sarajevo doesn't mean we suddenly get to lay down arms.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Surrender to GDI means prosecution at the hands of whatever war crimes tribunal the United Nations can string up for us.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(61);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "KORKUT";
                    storyDisplay.HeaderDisplay.SetColors(Color.BurlyWood);
                    storyDisplay.ConversationDisplay.TextColor = Color.BurlyWood;
                    storyDisplay.ConversationDisplay.ConversationText = "As high ranking officers in Kane's army, they will no doubt intend to punish us the most severely.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(61);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "Not that the rank and file will see much better. They'll be in camps for years even if they're not charged with anything.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But we can't hold out in Eastern Europe, Commander. The situation here is completely untenable.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE05/small2.png", 73);
                    storyDisplay.ConversationDisplay.ConversationText = "That really only leaves us with one option - we retreat to General Korkut's base of power in Anatolia, and integrate our forces there with his.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(61),
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/SmallKorkut.png", 74, false);
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(74);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "KORKUT";
                    storyDisplay.HeaderDisplay.SetColors(Color.BurlyWood);
                    storyDisplay.ConversationDisplay.TextColor = Color.BurlyWood;
                    storyDisplay.ConversationDisplay.ConversationText = "The plan is already in motion. General Morgan had enough... forethought... to reach out to me to discuss the situation even before we got word about Sarajevo.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(73),
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Needless to say I am prepared to assist your retreat however I can.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI will find it impossible to crack our combined defenses once your forces make it to Anatolia.  I am sure of this.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(74);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "That's the hope. General, I'll need to follow up with you later.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(74);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "KORKUT";
                    storyDisplay.HeaderDisplay.SetColors(Color.BurlyWood);
                    storyDisplay.ConversationDisplay.TextColor = Color.BurlyWood;
                    storyDisplay.ConversationDisplay.ConversationText = "Of course.  I wouldn't want to take up too much of your valuable time...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(74);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                    AddTDDisplayImage("Story/SE/Shared/SmallTerm.png", 75);
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "That slimy bastard has tried to throw me under the bus more times than I can count. Still, he's the best chance we've got right now...";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(74),
                null,
                null));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/SmallHoward.png", 76);
                    storyDisplay.ConversationDisplay.ConversationText = "I have Lydia working on preparing our retreat, but we aren't ready to move yet. Meanwhile, GDI's General Howard has launched a new offensive all along the front, hoping to force our surrender.";
                },
                null,
                null,
                storyDisplay => storyDisplay.RemoveStoryImageById(75)));

            phases.Add(new Phase(31,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE05/small3.png", 77);
                    storyDisplay.ConversationDisplay.ConversationText = "A major research and development hub containing top scientific personnel and advanced technological prototypes is in danger of falling to Howard's troops.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(32,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I've ordered extraction of these vital assets via rail, but the train won't arrive before GDI does.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(76);
                    storyDisplay.FindStoryImageById(77).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(33,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I need you to hold the lab until extraction, no matter the cost. Even I'm not completely sure what's inside, but we can't afford to lose any technological advantages we may have in there.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(34,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unless you want to see the inside of a cell at the Hague, this is the plan. Get moving.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(77); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(10).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(35,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            if (radarVariable != null && radarVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(36,
                    storyDisplay =>
                    {
                        cutsceneManager.StopMusic();
                        assets.world2.Play();
                        storyDisplay.ConversationDisplay.ConversationText = "---------";

                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(37,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "* * * FETCHING REPORT ITEM * * *";
                        cutsceneManager.TryPlaySong(assets.tdmaptheme);
                    },
                    null,
                    null,
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(38,
                    storyDisplay =>
                    {
                        assets.country4.Play();
                        storyDisplay.ConversationDisplay.ConversationText = "";
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/TextBriefBG.png", 100, 0f).AlphaRate = 2.0f;
                    },
                    null,
                    storyDisplay => assets.bleep11.Play(),
                    null));

                const float briefingTextAlphaRate = 1.0f;

                phases.Add(new Phase(39,
                    null,
                    storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE05/text1.png", 101, 0f).AlphaRate = briefingTextAlphaRate,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(101).AlphaRate = -briefingTextAlphaRate;
                        assets.bleep11.Play();
                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(101)));

                phases.Add(new Phase(40,
                    null,
                    storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE05/text2.png", 102, 0f).AlphaRate = briefingTextAlphaRate,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(102).AlphaRate = -briefingTextAlphaRate;
                        assets.bleep11.Play();
                    },
                    storyDisplay => storyDisplay.RemoveStoryImageById(102)));

                phases.Add(new Phase(41,
                    null,
                    storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE05/text3.png", 103, 0f).AlphaRate = briefingTextAlphaRate,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(103).AlphaRate = -briefingTextAlphaRate;
                        storyDisplay.FindStoryImageById(100).AlphaRate = -0.8f;
                        assets.toney4.Play();
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            return phases;
        }

        public List<Phase> SE05End()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.chrg226m);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "Your defense of the lab stalled the enemy long enough for critical researchers and technologies to be evacuated.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05End/slide1.png", 1);
                    assets.toney7.Play();
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The train containing those strategic assets departed not a moment too soon - for GDI's satellite-based Ion Cannon erased the lab shortly afterward.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05End/slide2.png", 2, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Despite your victory, the situation on the ground deteriorated quickly. Contact was lost unexpectedly with the former lab site as more of General Howard's troops arrived to secure the area.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05End/slide3.png", 3, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Worse still, somehow GDI learned of your location and launched a precision strike against your elite guard before you could react. Such an attack should have been impossible...";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05End/slide4.png", 4, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You were able to escape the chaos of the GDI assault with a small number of your troops, but you are now cut off from the bulk of General Morgan's forces as they begin their retreat.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE05End/slide5.png", 5, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The situation looks hopeless...";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        public List<Phase> SE06()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * OPENING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoW3N.png", 1);
                    assets.mapwipe5.Play();
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "United Nations investigators continue to dig through the burnt out rubble of what used to be the Brotherhood of Nod's Sarajevo headquarters.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/slide1.png", 1);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Despite days of excavation, the fate of Kane, the charismatic leader of the Brotherhood, remains unknown - although GDI General Mark Jamison Sheppard has publicly stated that \"there is no way Kane could have survived the destruction of his Temple.\"";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/slide2.png", 2, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Kane is presumed to have been on-site at the time of the compound's destruction.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "U.N. officials have meanwhile called for a formal end to hostilities, but their appeals have been met with mixed results.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/slide3.png", 3, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While some of Nod's military forces have surrendered or chosen to disband, other Nod armies continue to fight on, and many areas remain under Brotherhood control.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "What remains of Nod resistance may soon disintegrate, however. Sources within GDI report that the Brotherhood's trade network appears to have virtually collapsed following the fall of Sarajevo.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/slide4.png", 4, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We here at W3N can only speculate that arms manufacturers and black market suppliers are reluctant to continue to back a client fighting for a lost cause - especially one who might soon be writing checks they can no longer cash.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unless Nod remnants can develop the industrial capacity to manufacture their own weapons of war, it seems that---";
                },
                storyDisplay =>
                {
                    cutsceneManager.StopMusic();
                    assets.powrdn1.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "* * * CONNECTION INTERRUPTED * * *";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/largestatic.png", 5, 1f);
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.RemoveStoryImageById(4);
                },
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.vector1a);
                    storyDisplay.ConversationDisplay.ConversationText = "* * * TRANSMISSION FOUND * * *";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Black.png", 6);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoGDI.png", 7);
                    assets.newtarg1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.Text = "HOWARD";
                    storyDisplay.HeaderDisplay.SetColors(Color.Khaki);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Attention, Commander! This is General Howard in command of the GDI 3rd Army.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Howard.png", 8);

                    assets.country1.Play();
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(6); storyDisplay.RemoveStoryImageById(7); },
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I ask for your unconditional surrender. Kane is dead. The war is over. Why do you continue to fight?";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You are surrounded. My men will soon arrive to take you and your troops into custody.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I would prefer to end this matter without further bloodshed, but if you resist disarmament, GDI forces have my full authority to retaliate.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I urge you to do the right thing. Stand down, Commander, and answer for your crimes. Surrender, so we can put an end to the killing.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(8).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Black.png", 9);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoGDI.png", 10);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "* * * REROUTING CONNECTION * * *";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LogoNodStatic.png", 11);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    AddTDDisplay(50, "Story/SE/SE06/tdsidesStatic25.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/SmallMorganStatic25.png", 101, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic25.png", 12);
                    storyDisplay.HeaderDisplay.Show();
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander -  communications are in disarray. You're lucky I was able to reach you.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(11),
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    AddTDDisplay(52, "Story/CR/tdsides.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/SmallMorganStatic50.png", 103, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 13);

                    StoryImage borderDraw = storyDisplay.FindStoryImageById(103);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();

                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "GDI hit us hard in the last attack. We can't wait any longer. I've ordered a full scale retreat to the Bosporus.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(50);
                    storyDisplay.RemoveStoryImageById(101);
                    storyDisplay.RemoveStoryImageById(12);
                },
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE06/SmallMorganStatic75.png", 104, false, 1f);
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(104);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    storyDisplay.ConversationDisplay.ConversationText = "Lydia tells me you're cut off, Commander.  That's unacceptable.  I need you to link up with the rest of our forces before---";
                },
                storyDisplay =>
                {
                    assets.powrdn1.Play();
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = ". . .";

                    StoryImage borderDraw1 = storyDisplay.FindStoryImageById(103);
                    borderDraw1.DrawBorders = false;
                    storyDisplay.RemoveStoryImageById(103);

                    StoryImage borderDraw2 = storyDisplay.FindStoryImageById(104);
                    borderDraw2.DrawBorders = false;

                    AddTDDisplay(55, "Story/SE/SE06/tdsidesStatic25.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/smallstatic.png", 106, false);
                    storyDisplay.RemoveStoryImageById(104);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic25.png", 13);

                },
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    AddTDDisplay(57, "Story/CR/tdsides.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/smallstatic.png", 108, false);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 14);
                    storyDisplay.ConversationDisplay.ConversationText = "Not good - we seem to have lost the General's transmission. I'm not sure how long I'll be able to hold this line open...";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(12);
                    storyDisplay.RemoveStoryImageById(13);
                    storyDisplay.RemoveStoryImageById(52);
                    storyDisplay.RemoveStoryImageById(55);
                    storyDisplay.RemoveStoryImageById(106);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(108).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Listen. According to these coordinates, your current position is almost completely surrounded by GDI forces... almost.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(108);
                }));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You might still be able to escape if you move fast.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE06/small1.png", 109);
                    storyDisplay.ConversationDisplay.ConversationText = "There's an arterial to the east that should connect you with the rest of our retreating forces, but you'll have to traverse a valley full of some of the most advanced Tiberium life on record.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    AddTDDisplay(60, "Story/SE/SE06/tdsidesStatic25.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small1Static25.png", 111, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic25.png", 15);

                    storyDisplay.ConversationDisplay.ConversationText = "You should be able to commandeer a base at the western end of the valley that I still have contact with, but there's another complication.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(57);
                    storyDisplay.RemoveStoryImageById(109);
                    storyDisplay.RemoveStoryImageById(14);
                },
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    AddTDDisplay(61, "Story/CR/tdsides.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small2.png", 112);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 16);

                    storyDisplay.ConversationDisplay.ConversationText = "Our Airstrips are effectively useless. The arms dealers have cut us off - nobody is willing to sell us vehicles. Turns out losing a war causes liquidity issues.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(60);
                    storyDisplay.RemoveStoryImageById(111);
                    storyDisplay.RemoveStoryImageById(15);
                },
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE06/small3.png", 113);
                    storyDisplay.ConversationDisplay.ConversationText = "To make matters worse, some of those corporate suits must be feeling the heat, because a private military company, Vanguard, has been deployed to block your route of retreat.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(112);
                },
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    AddTDDisplay(62, "Story/SE/SE06/tdsidesStatic25.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small3static25.png", 114, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic25.png", 17);
                    storyDisplay.ConversationDisplay.ConversationText = "Vanguard usually operates at the behest of our largest corporate suppliers, so a move like this can only indicate that they've seen the way the wind is blowing.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(61);
                    storyDisplay.RemoveStoryImageById(113);
                    storyDisplay.RemoveStoryImageById(16);
                },
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Likely the fat cats in charge are hoping to hand you over to GDI on a silver platter.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    AddTDDisplay(63, "Story/CR/tdsides.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small4.png", 115);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 18);
                    storyDisplay.ConversationDisplay.ConversationText = "You'll have to cut through Vanguard's positions to avoid capture by GDI, but you won't be able to do so without combat vehicles.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(62);
                    storyDisplay.RemoveStoryImageById(114);
                    storyDisplay.RemoveStoryImageById(17);
                },
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    AddTDDisplay(64, "Story/SE/SE06/tdsidesStatic25.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small4static.png", 116, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic25.png", 19);
                    storyDisplay.ConversationDisplay.ConversationText = "Don't worry - yours truly has already come up with a solution. Needless to say, you're not the only one in need of new equipment.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(63);
                    storyDisplay.RemoveStoryImageById(115);
                    storyDisplay.RemoveStoryImageById(18);
                },
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    AddTDDisplay(65, "Story/SE/SE06/tdsidesStatic50.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small5Static50.png", 117);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic50.png", 20);
                    storyDisplay.ConversationDisplay.ConversationText = "There's an old Soviet War Factory in the valley - capture it.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(64);
                    storyDisplay.RemoveStoryImageById(116);
                    storyDisplay.RemoveStoryImageById(19);
                },
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I'll transmit the relevant vehicle schematics to your Engineers now - once you've reactivated that facility, you should be able to produce our vehicles there, including a new MCV. You'll need it.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    AddTDDisplay(66, "Story/SE/SE06/tdsidesStatic25.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small6Static25.png", 118);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic25.png", 21);
                    storyDisplay.ConversationDisplay.ConversationText = "One last thing - General Howard's minions outnumber you ten to one - and that's without taking into account his reserves.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(65);
                    storyDisplay.RemoveStoryImageById(117);
                    storyDisplay.RemoveStoryImageById(20);
                },
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "He may decide to commit those reserves to the battlefield if he feels sufficiently threatened.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    AddTDDisplay(67, "Story/CR/tdsides.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small6.png", 119, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 22);
                    storyDisplay.ConversationDisplay.ConversationText = "My recommendation would be to try to stay as far ahead of General Howard's offensive line as you can, and to refrain from counterattacking whatever base positions he may set up.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(66);
                    storyDisplay.RemoveStoryImageById(118);
                    storyDisplay.RemoveStoryImageById(21);
                },
                null,
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Assuage the good General's sense of danger.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    AddTDDisplay(68, "Story/SE/SE06/tdsidesStatic25.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/SE06/small6Static25.png", 120, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic25.png", 23);
                    storyDisplay.ConversationDisplay.ConversationText = "Even holding a location too long may convince him to deploy additional GDI forces into the area.";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(67);
                    storyDisplay.RemoveStoryImageById(119);
                    storyDisplay.RemoveStoryImageById(22);
                },
                null,
                null));

            phases.Add(new Phase(31,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You will likely need to abandon several of your base sites and rebuild further to the east as you push towards the edge of the valley.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(120).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(32,
                storyDisplay =>
                {
                    AddTDDisplay(69, "Story/SE/SE06/tdsidesStatic50.png", false, 0.75f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic50.png", 24);
                    storyDisplay.ConversationDisplay.ConversationText = "So - capture the old War Factory, build an MCV, stay ahead of General Howard, and break through Vanguard's lines to safety. Simple, right?";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(68);
                    storyDisplay.RemoveStoryImageById(23);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(69).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(33,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/LydiaStatic75.png", 25);
                    storyDisplay.ConversationDisplay.ConversationText = "What could go wrong?";
                },
                storyDisplay =>
                {
                    storyDisplay.RemoveStoryImageById(24);
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(120); storyDisplay.RemoveStoryImageById(69); },
                null));

            phases.Add(new Phase(34,
                storyDisplay =>
                {
                    cutsceneManager.StopMusic();
                    assets.powrdn1.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "* * * CONNECTION LOST * * *";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE06/largestatic.png", 25, 1f);
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.RemoveStoryImageById(24);
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        public List<Phase> SE07()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.tdmaptheme);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * ACCESSING BROTHERHOOD ARCHIVES * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE07/NodLogoStatic25.png", 2);
                    storyDisplay.ConversationDisplay.ConversationText = "* * * WARNING - CONNECTION UNSTABLE * * *";
                    assets.world2.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "The Brotherhood sits at the forefront of technological evolution.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNodArchives.png", 3);
                    assets.mapwipe2.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(2),
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Brotherhood works with the leading minds of top research and development teams, offering them the chance to help build a peaceful future.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE07/slide1.png", 4);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(3),
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The technology of peace takes many forms.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE07/slide2.png", 5);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "High-power laser systems safeguard many Brotherhood sanctuaries, and efforts to develop mobile applications continue.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE07/slide3.png", 6);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(5),
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The Lazarus Shield, responsible for the unique stealth capabilities of the Ezekiel's Wheel, has seen intensive study as our scientists work to enhance its functionality.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE07/slide4.png", 7);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(6),
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Other experimental technologies and prototypes, including advanced Tiberium research projects, also remain in development.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE07/slide5.png", 8);
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(7),
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(8).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * CLOSING ARCHIVE CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE07/NodLogoStatic25.png", 99);
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
                    cutsceneManager.StopMusic();
                    assets.world2.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "---------";

                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "* * * FETCHING BRIEFING ITEM * * *";
                    cutsceneManager.TryPlaySong(assets.recon);
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    assets.country4.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/TextBriefBG.png", 100, 0f).AlphaRate = 2.0f;
                },
                null,
                storyDisplay => assets.bleep11.Play(),
                null));

            const float briefingTextAlphaRate = 1.0f;

            phases.Add(new Phase(13,
                null,
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE07/text1.png", 101, 0f).AlphaRate = briefingTextAlphaRate,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(101).AlphaRate = -briefingTextAlphaRate;
                    assets.bleep11.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(101)));

            phases.Add(new Phase(14,
                null,
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE07/text2.png", 102, 0f).AlphaRate = briefingTextAlphaRate,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(102).AlphaRate = -briefingTextAlphaRate;
                    assets.bleep11.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(102)));

            phases.Add(new Phase(15,
                null,
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE07/text3.png", 103, 0f).AlphaRate = briefingTextAlphaRate,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(103).AlphaRate = -briefingTextAlphaRate;
                    assets.bleep11.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(103)));

            phases.Add(new Phase(16,
                null,
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE07/text4.png", 104, 0f).AlphaRate = briefingTextAlphaRate,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(104).AlphaRate = -briefingTextAlphaRate;
                    assets.bleep11.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(104)));

            phases.Add(new Phase(17,
                null,
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE07/text5.png", 105, 0f).AlphaRate = briefingTextAlphaRate,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(105).AlphaRate = -briefingTextAlphaRate;
                    assets.bleep11.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(105)));

            phases.Add(new Phase(18,
                null,
                storyDisplay => storyDisplay.AddSimpleStoryImage("Story/SE/SE07/text6.png", 106, 0f).AlphaRate = briefingTextAlphaRate,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(106).AlphaRate = -briefingTextAlphaRate;
                    storyDisplay.FindStoryImageById(100).AlphaRate = -0.8f;
                    assets.toney4.Play();
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        public List<Phase> SE08()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Morgan.png", 2);
                    storyDisplay.HeaderDisplay.Text = "MORGAN";
                    storyDisplay.HeaderDisplay.SetColors(Color.Tomato);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Tomato;
                    storyDisplay.ConversationDisplay.ConversationText = "You're a sight for sore eyes, Commander. It took Lydia some time to re-encrypt our communications systems, but everything is finally back online.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Let me be blunt. The attacks on the port and the lab, the attempted decapitation strike on your position, the disruption of our communications...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...none of it would have been possible without someone on the inside.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Clearly, a traitor within the Brotherhood has been trying to sabotage our operations for some time now.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I ask that you keep your eyes and ears open until I can flush out whichever disloyal rat is responsible and purge them from our ranks.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    AddTDDisplay(50);
                    storyDisplay.ConversationDisplay.ConversationText = "In the meantime, there's still work to do.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE08/small1.png", 51);
                    storyDisplay.ConversationDisplay.ConversationText = "The bulk of our forces are nearing the shore of the Black Sea, but the only viable approach to General Korkut's stronghold in Anatolia requires us to traverse the Bosporus Strait.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unfortunately, General Howard and his GDI lackeys remain in close pursuit.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE08/small2.png", 52);
                    storyDisplay.ConversationDisplay.ConversationText = "To make matters worse, they've anticipated our route of retreat - a large GDI naval fleet has moved in and set up a blockade of the strait.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(51),
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "We're not going to break that blockade with a flotilla of small boats and a few barely functional prototypes, Commander. We need an edge. Luckily, I know just where to find it.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/smallsub.png", 53);
                    storyDisplay.ConversationDisplay.ConversationText = "There's long been rumors of a hidden Russian submarine base somewhere near Sevastopol. Commander, I want you to steal those submarines.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(52),
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE08/small4.png", 54);
                    storyDisplay.ConversationDisplay.ConversationText = "First you'll need to locate the submarine base. Take a small team into Sevastopol proper, and capture the Russian Black Sea fleet's headquarters.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(53),
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Any part of the city that gets in your way is expendable.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Once you have the location of the submarine base, I'll send a second team to you, including an... interesting asset R&D just spit out. Use that team to capture the submarines from drydock.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE08/small5.png", 55);
                    storyDisplay.ConversationDisplay.ConversationText = "We've compromised Russian intelligence for a long time, and so I can tell you that at one point the Russians were working on an experimental nuclear-armed submarine there.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(54),
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I don't know if that sub is still there or what shape it might be in, but it could be a very valuable asset to us. Be on the lookout for it.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(55).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "While the Russian government has been nominally aligned with GDI, political instability at home has largely kept their hands full.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They won't be expecting your attack - you should have the element of surprise.";
                },
                null,
                null,
                null));
            ;

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I have to admit I didn't think much of you at first Commander, but you continue to surprise me. It seems like it might be time to discuss soon what might lie ahead for you and I.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But that talk can wait until you bring back those subs. I'm counting on you. Don't screw this up.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(55); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
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

        public List<Phase> SE08End()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.stopthem);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 2);
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                storyDisplay => AddTDDisplay(50),
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE08End/small6.png", 51);
                    storyDisplay.ConversationDisplay.ConversationText = "While you were out on yet another covert operation, a helicopter carrying General Morgan and his personal bodyguard exploded in midair.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There were no survivors.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(51).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Convenient that it happened while you were away. Convenient that it leaves you as the highest ranking officer in charge of our retreating forces.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If you're coming for me next, at least look me in the eye when you take your knife out.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(51); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
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

        public List<Phase> SE09()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    AddTDDisplay(50, "Story/CR/tdsides.png", false, 0.75f);
                    AddTDDisplayImage("Story/SE/Shared/SmallKorkut.png", 51, false, 1f);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 2);
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander. Our forces have embarked onto naval transports and have arrived at the entrance to the Bosporus strait.";
                    assets.country1.Play();

                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE09/small1.png", 52);
                    storyDisplay.ConversationDisplay.ConversationText = "GDI ships remain in defensive positions throughout the strait, in an attempt to block our route of egress from the Black Sea.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/SmallKorkut.png", 53, false);
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(53);
                    borderDraw.DrawBorders = true;
                    borderDraw.BorderColor = Color.Yellow;
                    assets.bleep11.Play();
                    storyDisplay.HeaderDisplay.Text = "KORKUT";
                    storyDisplay.HeaderDisplay.SetColors(Color.BurlyWood);
                    storyDisplay.ConversationDisplay.TextColor = Color.BurlyWood;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander - my condolences regarding the death of General Morgan. We did not always get along, he and I, but he was a formidable man.";

                },
                storyDisplay => storyDisplay.RemoveStoryImageById(52),
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I was just informing Lydia of the specific port facilities I have prepared to receive your troops.";

                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unfortunately, due to the termination of support from our suppliers, I have had to devote significant resources to establishing local weapons production.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As a result of this unexpected development, I do not have spare assets that can assist in the crossing of the strait.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If Kane wills it, you will succeed in your efforts today, and I will shake your hand on Turkish soil.  Good luck.";
                },
                null,
                storyDisplay =>
                {
                    StoryImage borderDraw = storyDisplay.FindStoryImageById(53);
                    borderDraw.DrawBorders = false;
                    assets.bleep12.Play();
                },
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/SmallTerm.png", 54);
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "We don't need luck. We have submarines.";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(51); storyDisplay.RemoveStoryImageById(53); },
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/smallsub.png", 55);
                    storyDisplay.ConversationDisplay.ConversationText = "While our attack on Sevastopol has become public news, the Russians aren't eager to share the whole picture, and so far have covered up any mention of stolen vessels.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(54),
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "GDI's sailors will be easy marks - completely unprepared to engage in anti-submarine warfare.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You should be able to ambush enemy ships from whatever position and angle of attack you deem appropriate. Send GDI's ships to the bottom of the ocean.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/Shared/small941.png", 56);
                    storyDisplay.ConversationDisplay.ConversationText = "While the majority of your submarine fleet is fully functional, we haven't had time to get the prototype sub you stole, Project 941, completely weapons-ready yet.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(55),
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE09/small3.png", 57);
                    storyDisplay.ConversationDisplay.ConversationText = "From what I've been told, Project 941 should eventually be able to fire nuclear warheads from both its torpedo and missile systems.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(56),
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Our technicians are having what sounds like just a spectacular time trying to figure out how it all works from Russian instruction manuals.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In the meantime, I've had the 941 loaded with standard torpedoes.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "That sub should prove useful here - but losing it in the battle for the strait would be a costly mistake, considering its potential capabilities. So try not to do that, hmm?";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(57).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Once you clear the strait of GDI ships, I'll signal the rest of our fleet to move in, and we can make a dash for Korkut's ports on the western shores of Anatolia.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There's nowhere for us to regroup if this goes wrong, Commander. Failure is not an option.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(57); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    cutsceneManager.StopMusic();
                    assets.world2.Play();
                    storyDisplay.ConversationDisplay.ConversationText = "---------";

                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.terminat);
                    storyDisplay.ConversationDisplay.ConversationText = "* * * ADDITIONAL TRANSMISSION FOUND * * *";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Black.png", 100);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoGDI.png", 101);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.Show();
                    storyDisplay.HeaderDisplay.Text = "HOWARD";
                    storyDisplay.HeaderDisplay.SetColors(Color.Khaki);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander - it's time to end this foolishness.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Howard.png", 102);
                    assets.country1.Play();
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(100); storyDisplay.RemoveStoryImageById(101); },
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "My ships block your passage through the strait, and my tanks are on the shores behind you. You have nowhere to go.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I urge you again to surrender. Consider the reality of your situation.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Your improvised squadron of small boats and transport craft will not stand up to the full power of the GDI navy.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If you try to push through the strait, you will ensure nothing but the destruction of your ships and the deaths of your men.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Maybe it's a mistake to appeal to your conscience. I know your superior officer had an \"accident.\" I heard too about what you did in Sevastopol.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Senseless carnage. And for what?";
                },
                null,
                null,
                null));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Enough. I expect your surrender within the hour. If you do not comply, I will be forced to take aggressive action.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(31,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The lives of your troops are in your hands, Commander.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(102).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(32,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Black.png", 103);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoGDI.png", 104);
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

        public List<Phase> SE09End()
        {
            var phases = new List<Phase>();

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.chrg226m);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "The arrival of Russian forces was an unexpected surprise, but in the end, they acted as little more than a temporary roadblock to your passage through the strait.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE09End/slide1.png", 1);
                    assets.toney7.Play();
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Once your vessels passed near Istanbul, the remaining assets GDI had in the area were forced to disengage for fear of inflicting collateral damage upon the city itself.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE09End/slide2.png", 2, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Your forces soon disembarked in General Korkut's port bases to raucous cheers and applause from the soldiers stationed there. The garrisons had heard the Battle of the Bosporus from shore and seen it on TV.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE09End/slide3.png", 3, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Just as you seemed to be riding high on a new wave of popularity with the troops, one of Korkut's officers asked for a private aside.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The officer handed you a copy of his orders from General Korkut - orders that stated the port garrisons were to open fire on your \"traitorous\" forces before they could reach the shore.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE09End/slide4.png", 4, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It soon became clear that Korkut had issued these orders to every port he had \"prepared\" for you. Only the audacity of your battle plans and your personal popularity had saved you and your men from a watery grave in the Mediterranean.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "And so, after you consolidated the port garrisons into your existing forces, you began a march on Korkut's headquarters...";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE09End/slide5.png", 5, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "There is only one punishment acceptable for Korkut's betrayal. Death.";
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }
        public List<Phase> SE10()
        {
            var phases = new List<Phase>();
            var enforcerVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_CAPTURED_ENFORCER_BLUEPRINTS");
            var behemothVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_CAPTURED_BEHEMOTH_BLUEPRINTS");

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.stopthem);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 1);
                    assets.newtarg1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE10/Korkut.png", 2);
                    storyDisplay.HeaderDisplay.Text = "KORKUT";
                    storyDisplay.HeaderDisplay.SetColors(Color.BurlyWood);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.BurlyWood;
                    storyDisplay.ConversationDisplay.ConversationText = "Ah, Commander! Don't bother attempting to record this transmission - the connection will close automatically if my E.V.A. detects anything improper.";
                    assets.country1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "So. You made it here in one piece.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I have to admit I did not expect your little convoy to survive contact with GDI's navy, but those submarines you pulled from nowhere were certainly a neat trick.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If that irritating woman hadn't figured out that your communications systems were compromised when she did, perhaps you and I would not be sitting here, having this conversation...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Then again, the GDI general pursuing you is a buffoon who has utterly failed to put you down at every turn...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...even when he still had the aid of my jammers and leaked information regarding your troop strength and positions.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "So perhaps this outcome was inevitable after all.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "At least General Morgan is dead...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I've gotten to know his routine over the years. It was surprisingly easy to ensure a little... malfunction occurred on his helicopter, even after I was locked out of your network.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Morgan was a fool.  Worse than that, he was a Judas.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "When he had that woman reach out to me to coordinate a retreat, Sarajevo was still standing!";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "He should have pushed to relieve Kane's forces, but instead, he was focused on SAVING HIS OWN SKIN!";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "And so... in the end, here you are, Morgan's protege, your forces massing outside my temple grounds. You will claim I am a traitor, and I will claim you are a traitor.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But we both know the truth. Only one of us has the spirit hand of Kane guiding them today, and it is not you, Commander.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In the end, you will beg for Kane's forgiveness. This much, I promise you.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(2).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * TRANSMISSION TERMINATED * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 12);
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    storyDisplay.ConversationDisplay.ConversationText = "* * * REROUTING CONNECTION * * *";
                    assets.world2.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.Show();
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "I should never have doubted you. It won't happen again.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 13);
                    assets.country1.Play();
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(12); },
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The loyalty of many of your troops, however, remains in question.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    AddTDDisplay(50);
                    storyDisplay.ConversationDisplay.ConversationText = "While your personal forces will fight for you to the end, many of your auxiliary officers seem to be unnerved by the idea of challenging that wretch Korkut's power base.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE10/small1.png", 51);
                    storyDisplay.ConversationDisplay.ConversationText = "Nobody wants to be on the losing end of this power struggle. I expect that your supporting forces will defect to Korkut's side if they believe their base positions to be overrun.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE10/small2.png", 52);
                    storyDisplay.ConversationDisplay.ConversationText = "Korkut's troops aren't entirely devoted to their \"fearless leader\" either, but I won't lie to you: the odds favor Korkut. He has numerically superior forces in entrenched positions.";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(51); },
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "To make matters worse, Korkut had access to our systems for so long that we can't expect to have any technological advantage.";
                },
                null,
                null,
                null));

            if (enforcerVariable != null && enforcerVariable.EnabledThroughPreviousScenario || behemothVariable != null && behemothVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(25,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "The only thing he might not have pilfered from us is the research related to the blueprints you acquired during previous operations...";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(26,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "....which has remained isolated from our primary network in the haste of our retreat.";
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE10/small3.png", 53);
                    storyDisplay.ConversationDisplay.ConversationText = "All that being said, if you can capture Korkut's Temple, it would be an undeniable blow to his prestige - and it might just convince any wavering officers that you are the one who's meant to lead.";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(52); },
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Just don't destroy it unless you want to become the newest heretic in Anatolia.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(53).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                    assets.beepy3.Play();
                }));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It's time for you to take control, Commander. Storm Korkut's compound and put down that miserable snake.";
                },
                null,
                null,
                storyDisplay =>
                {
                    storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                    assets.bleep17.Play();
                }));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "If anyone dares to oppose you - silence them.";
                },
                null,
                storyDisplay => { storyDisplay.RemoveStoryImageById(53); storyDisplay.RemoveStoryImageById(50); },
                storyDisplay => { storyDisplay.FindStoryImageById(13).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(31,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
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

        public List<Phase> SE10End()
        {
            var phases = new List<Phase>();
            var vassalsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_MAJORITY_NOD_VASSALS_ALIVE");

            if (vassalsVariable != null && vassalsVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(1,
                    storyDisplay =>
                    {
                        cutsceneManager.TryPlaySong(assets.stopthem);
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                        storyDisplay.ConversationDisplay.ConversationText = "Your battlefield prowess convinced most of Korkut's officers to defect, which turned the tide against his remaining loyalists and led to a decisive victory.";
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/officers.png", 1);
                        assets.toney7.Play();
                    },
                    null,
                    null,
                    null));
            }
            else
            {
                phases.Add(new Phase(1,
                    storyDisplay =>
                    {
                        cutsceneManager.TryPlaySong(assets.stopthem);
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                        storyDisplay.ConversationDisplay.ConversationText = "After a bloody internecine struggle, your forces emerged victorious over those of Korkut. Few of his officers survived, and those who did soon wished they hadn't.";
                        storyDisplay.ConversationDisplay.IsCentered = false;
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/wrecks.png", 1);
                        assets.toney7.Play();
                    },
                    null,
                    null,
                    null));
            }

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Korkut himself was only located several hours after the battle's conclusion, following a thorough investigation of his underground bunker complex.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Unfortunately, in an attempt to avoid capture, General Korkut drew his weapon, and one of your Flamethrower infantry reacted in self-defense.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE10End/slide3.png", 3, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Although the offending soldier was subsequently executed for his rash action, the fact is that nothing remains of General Korkut but a charred corpse.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Despite your victory, there's no time to sit on your laurels - you're urgently needed back at the coast, as reports indicate General Howard has begun staging his forces for an amphibious invasion while you've been away.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE10End/slide4.png", 4, 0f);
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It seems nothing can make him break off his pursuit...";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            return phases;
        }

        public List<Phase> SE11()
        {
            var phases = new List<Phase>();
            var vassalsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_MAJORITY_NOD_VASSALS_ALIVE");
            var blueprintsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_CAPTURED_ALL_BLUEPRINTS");

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.secondhand);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "* * * OPENING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoW3N.png", 1);
                    assets.mapwipe5.Play();
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Tension is in the air tonight among GDI forces in the Mediterranean, as General Howard continues his buildup of land, air and sea assets off the western shores of Turkey.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11/slide1.png", 2);
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(1),
                null,
                null));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "His aim? To capture the Brotherhood Commander who has so far defied every GDI attempt to disarm and disband their forces.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    assets.country1.Play();
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11/slide2.png", 3, 0f);
                    storyDisplay.ConversationDisplay.ConversationText = "Originally a mere junior Commander, this elusive shadow has only risen in significance since the fall of Sarajevo. They've defeated GDI troops, the Russian military, and even corporate mercenaries in a dramatic flight from the Balkans.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(2),
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Now, more powerful than ever before, they've built a new Nod Temple on the Anatolian coast.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay =>
                {
                    assets.country1.Play();
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11/slide3.png", 4, 0f);
                    storyDisplay.ConversationDisplay.ConversationText = "Their most recent escape through the Bosporus strait led to the annihilation of multiple Russian armies and an entire GDI fleet.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(3),
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Public furor over these casualties has some analysts questioning whether combat operations of this scale are sustainable in an increasingly weary post-war atmosphere.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay =>
                {
                    assets.country1.Play();
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11/slide4.png", 5, 0f);
                    storyDisplay.ConversationDisplay.ConversationText = "Whatever the future may have in store, it's clear that as long as bulldogs like General Howard remain at the forefront of GDI command, Kane's followers will find no safe haven to shield them from the echoes of this conflict.";
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(4),
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(5).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "* * * CLOSING CONNECTION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoW3N.png", 6);
                },
                null,
                null,
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.fac2226m);
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * TRANSMISSION FOUND * * *";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Black.png", 7);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoGDI.png", 8);
                    assets.newtarg1.Play();
                },
                storyDisplay => storyDisplay.RemoveStoryImageById(6),
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.Text = "HOWARD";
                    storyDisplay.HeaderDisplay.SetColors(Color.Khaki);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.Khaki;
                    storyDisplay.ConversationDisplay.ConversationText = "Commander. I know by now that it's a mistake to appeal to your conscience. You have none.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Howard.png", 9);

                    assets.country1.Play();
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(7); storyDisplay.RemoveStoryImageById(8); },
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But if you thought that I was going to let you just slip away, you're wrong. Especially now that the Russians tell me you have a submarine with nuclear weapons on-board.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I wouldn't be able to live with myself if I pulled back from these shores knowing you had access to such a vessel...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...let alone this Anatolian power base you seem to have inherited after your little Nod civil war.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "So we'll meet on the field of battle once more. I see no alternative.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Of course, my terms still stand - offer your unconditional surrender, and we can end this at any point.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "But I know you won't.";
                },
                null,
                null,
                storyDisplay => { storyDisplay.FindStoryImageById(9).AlphaRate = -2.0f; assets.toney4.Play(); }));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Black.png", 10);
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoGDI.png", 11);
                },
                null,
                storyDisplay =>
                {
                    storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                    storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                },
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(19,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.recon);
                    storyDisplay.ConversationDisplay.ConversationText = "* * * REROUTING CONNECTION * * *";
                    assets.world2.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(20,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "* * * INCOMING TRANSMISSION * * *";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 12);
                },
                null,
                null,
                null));

            phases.Add(new Phase(21,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.Show();
                    storyDisplay.HeaderDisplay.Text = "LYDIA";
                    storyDisplay.HeaderDisplay.SetColors(Color.LightSteelBlue);
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.TextColor = Color.LightSteelBlue;
                    storyDisplay.ConversationDisplay.ConversationText = "Well, well, if it isn't everyone's favorite Commander.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/Lydia.png", 13);
                    assets.country1.Play();
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(12); },
                null,
                null));

            phases.Add(new Phase(22,
                storyDisplay =>
                {
                    AddTDDisplay(50);
                    storyDisplay.ConversationDisplay.ConversationText = "How do you like your new Temple?";
                },
                null,
                null,
                null));

            phases.Add(new Phase(23,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE10/small3.png", 51);
                    storyDisplay.ConversationDisplay.ConversationText = "It practically radiates authority from that seaside overlook. Any wayward follower of Kane lucky enough to see your Temple - or even hear of it - will no doubt find their will to fight strengthened.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(24,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE06/small6.png", 52);
                    storyDisplay.ConversationDisplay.ConversationText = "General Howard, on the other hand, aims to give you an eviction notice. His GDI flunkies have already landed at multiple points and set up several bases that threaten your headquarters.";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(51); },
                null,
                null));

            phases.Add(new Phase(25,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE11/small3.png", 53);
                    storyDisplay.ConversationDisplay.ConversationText = "I've also picked up numerous transmissions suggesting that Howard and the Russians are still in close communication. It's likely that Russian reinforcements will arrive to bolster Howard's forces soon.";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(52); },
                null,
                null));

            phases.Add(new Phase(26,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "It's only a matter of time, therefore, before GDI and Russian forces launch a combined offensive on your Temple.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(27,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "How lovely to see that they've taken the bait.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(28,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE11/small2.png", 54);
                    storyDisplay.ConversationDisplay.ConversationText = "While destruction of your new Temple would be unacceptable - for obvious reasons - those fools have all fallen right into a well-laid trap.";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(53); },
                null,
                null));

            phases.Add(new Phase(29,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "How amusing it will be to watch you ruin their carefully prepared battle plans with your new toys...";
                },
                null,
                null,
                null));

            phases.Add(new Phase(30,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE11/small4.png", 55);
                    storyDisplay.ConversationDisplay.ConversationText = "Korkut had been stockpiling nuclear warheads in the interior, and I've had them brought to the coast for use in your Temple and missile silos.";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(54); },
                null,
                null));

            phases.Add(new Phase(31,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The warheads are nearly ready for use - the longer you can hold on, the more missile silos we can bring online, and the uglier the situation will become for Howard and his allies of convenience.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(32,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE11/small5.png", 56);
                    storyDisplay.ConversationDisplay.ConversationText = "Meanwhile, you can use Korkut's stealth generator prototypes to confuse enemy forces and shield essential assets.";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(55); },
                null,
                null));

            phases.Add(new Phase(33,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Keep in mind that the devices are small, and power-hungry.  I've uploaded the schematics to your construction facilities.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(34,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I would thank the General for these contributions, if he wasn't already a pile of ash...";
                },
                null,
                null,
                null));

            if (blueprintsVariable != null && blueprintsVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(35,
                    storyDisplay =>
                    {
                        AddTDDisplayImage("Story/SE/SE11/small7.png", 57);
                        storyDisplay.ConversationDisplay.ConversationText = "Your researchers have also become quite excitable about some secret project they've been working on.";
                    },
                    storyDisplay => { storyDisplay.RemoveStoryImageById(56); },
                    null,
                    null));

                phases.Add(new Phase(36,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "They've apparently created functional prototypes of a new combat vehicle based on the various blueprints you've acquired.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(37,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Given enough time, they should be able to deploy these vehicles for your use in the field.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(38,
                    storyDisplay =>
                    {
                        AddTDDisplayImage("Story/SE/Shared/small941.png", 58);
                        storyDisplay.ConversationDisplay.ConversationText = "The most important element of our trap, however, is of course, Project 941. We are finally in a position to equip its torpedoes and missiles with the nuclear warheads they are designed to carry.";
                    },
                    storyDisplay => { storyDisplay.RemoveStoryImageById(57); },
                    null,
                    null));
            }
            else
            {
                phases.Add(new Phase(38,
                    storyDisplay =>
                    {
                        AddTDDisplayImage("Story/SE/Shared/small941.png", 58);
                        storyDisplay.ConversationDisplay.ConversationText = "The most important element of our trap, however, is of course, Project 941. We are finally in a position to equip its torpedoes and missiles with the nuclear warheads they are designed to carry.";
                    },
                    storyDisplay => { storyDisplay.RemoveStoryImageById(56); },
                    null,
                    null));
            }

            phases.Add(new Phase(39,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As with the silos, it will take some time before Project 941's nuclear capabilities come online. However, once they do, that submarine will cause immense devastation to enemy forces.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(40,
                storyDisplay =>
                {
                    AddTDDisplayImage("Story/SE/SE11/small6.png", 59);
                    storyDisplay.ConversationDisplay.ConversationText = "Howard has built his offshore command center to be practically impregnable to any type of assault we can muster...";
                },
                storyDisplay => { storyDisplay.RemoveStoryImageById(58); },
                null,
                null));

            phases.Add(new Phase(41,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "...but I doubt the defenses are strong enough to withstand a barrage of nuclear warheads from Project 941's missile racks.";
                },
                null,
                null,
                null));

            if (vassalsVariable != null && vassalsVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(41,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It's time to force the good General into retirement.  Permanently.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(42,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "One last thing - the armies of several of your officers are currently cut off by damaged infrastructure.";
                        AddTDDisplayImage("Story/SE/SE11/small8.png", 60);
                    },
                    storyDisplay => { storyDisplay.RemoveStoryImageById(59); },
                    null,
                    null));

                phases.Add(new Phase(43,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "If you can repair the relevant bridges, these troops should be able to reinforce your position and help take the pressure off of your forces while you prepare for the killing blow.";
                    },
                    null,
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(60).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                        assets.beepy3.Play();
                    }));

                phases.Add(new Phase(44,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "I never thought I'd say this, but it's been a singular pleasure to watch you work. I'm intrigued to see how you handle this particular bloodbath.";
                    },
                    null,
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                        assets.bleep17.Play();
                    }));

                phases.Add(new Phase(45,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Go out there and show them who they're dealing with.";
                    },
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(60); storyDisplay.RemoveStoryImageById(50); },
                    storyDisplay => { storyDisplay.FindStoryImageById(13).AlphaRate = -2.0f; assets.toney4.Play(); }));
            }
            else
            {
                phases.Add(new Phase(42,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It's time to force the good General into retirement.  Permanently.";
                    },
                    null,
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(59).AlphaRate = crTDdisplayImageDisappearAlphaRate;
                        assets.beepy3.Play();
                    }));

                phases.Add(new Phase(43,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "I never thought I'd say this, but it's been a singular pleasure to watch you work. I'm intrigued to see how you handle this particular bloodbath.";
                    },
                    null,
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.FindStoryImageById(50).AlphaRate = -crRAdisplayAlphaRate;
                        assets.bleep17.Play();
                    }));

                phases.Add(new Phase(44,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Go out there and show them who they're dealing with.";
                    },
                    null,
                    storyDisplay => { storyDisplay.RemoveStoryImageById(59); storyDisplay.RemoveStoryImageById(50); },
                    storyDisplay => { storyDisplay.FindStoryImageById(13).AlphaRate = -2.0f; assets.toney4.Play(); }));
            }

            phases.Add(new Phase(46,
                storyDisplay =>
                {
                    storyDisplay.HeaderDisplay.InstantHide();
                    storyDisplay.ConversationDisplay.TextColor = Color.White;
                    storyDisplay.ConversationDisplay.ConversationText = "* * * END OF TRANSMISSION * * *";
                    storyDisplay.ConversationDisplay.IsCentered = true;
                    storyDisplay.AddSimpleStoryImage("Story/SE/Shared/LogoNod.png", 99);
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

        public List<Phase> SE11End()
        {
            var phases = new List<Phase>();
            var vassalsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_MAJORITY_NOD_VASSALS_ALIVE");
            var blueprintsVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_CAPTURED_ALL_BLUEPRINTS");
            var microwaveVariable = CampaignHandler.Instance.GlobalVariables.Find(gv => gv.InternalName == "GV_SE_PURGED_MICROWAVE_SCHEMATICS");

            phases.Add(new Phase(1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.stopthem);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "It was a symphony of violence. A shocked world looked on as General Howard and his unlikely allies were slowly and deliberately incinerated by an ever intensifying bombardment of nuclear hellfire.";
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide1.png", 1);
                    assets.toney7.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(2,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "None of your enemies on the battlefield that day survived.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(3,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Still reeling from the political instability occurring at home, the Russian government begrudgingly accepted its military's defeat in Anatolia, and refused to send more assets to the region, choosing instead to focus on internal affairs.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide2.png", 2, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(4,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Vanguard was effectively shattered as a fighting force, its elite assets squandered in what had proven to be costly battles against your forces, and the private mercenary company soon had to file for bankruptcy.";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide3.png", 3, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            if (microwaveVariable != null && !microwaveVariable.EnabledThroughPreviousScenario)
            {
                phases.Add(new Phase(5,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "In the years to come, a splinter faction of Vanguard attempted to further develop and monetize the Microwave Tank vehicle they had reverse-engineered from Nod technological data.";
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide4.png", 4, 0f);
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(6,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "However, when GDI demanded they hand over their schematics and prototypes, the ex-Vanguard staff refused.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(7,
                    storyDisplay =>
                    {
                        assets.country1.Play();
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide5.png", 5, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "The ensuing confrontation saw a GDI raid on the research facility that went horribly wrong.";
                    },
                    storyDisplay => { storyDisplay.RemoveStoryImageById(4); },
                    null,
                    null));

                phases.Add(new Phase(8,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "In an act of defiance, the last surviving staff members of the base activated a self-destruct mechanism that destroyed not just all of the Microwave Tank data, but killed everyone on-site.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));
            }

            phases.Add(new Phase(9,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide6.png", 6, 0f);
                    storyDisplay.ConversationDisplay.ConversationText = "The death of General Howard and the annihilation of such a large GDI task force inflamed public opinion against General Sheppard and his seeming willingness to spend lives so casually in the pursuit of an officially defeated foe.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(10,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The political fallout led to a shift in United Nations policy.  While eliminating Brotherhood remnants remained GDI's top priority, the defeat of Nod's scattered armies could no longer come at an equivalent exchange in human lives.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(11,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "As long as GDI believed you sat on a stockpile of nuclear weapons, you would never be their first target in this new world.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(12,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They didn't need to know that you had very few remaining - that you had spent most of them just trying to keep the eagle at bay.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide7.png", 7, 0f);
                    storyDisplay.ConversationDisplay.ConversationText = "In the aftermath of the battle, you took the opportunity to officially promote yourself to General. It was long past time.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "In turn, you kept Lydia on as your unofficial second in command. There were still a great many things you would need her competent eye to sort out for the foreseeable future, both military and domestic.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "And... keeping her close meant you could make sure her activities were always being focused outwards, rather than inwards.";
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The road ahead would not be easy. You and your troops were effectively outcasts, building a new type of state for a new type of society - though, always in the name of Kane, of course...";
                    storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide8.png", 8, 0f);
                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));

            if ((vassalsVariable != null && vassalsVariable.EnabledThroughPreviousScenario) & (blueprintsVariable != null && blueprintsVariable.EnabledThroughPreviousScenario))
            {

                phases.Add(new Phase(17,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/officers.png", 9, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "You had gone out of your way to impress Korkut's former officers and soldiers during the battle with the traitorous general, and the defection of these troops to your side paid dividends.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(19,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "During the subsequent battle with Howard, Korkut's former troops fought bravely, and proved their dedication to your cause.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(20,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "In the aftermath, you found yourself with a devoted and competent army composed of the best of the two combined forces.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(21,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Better yet, you still had significant reserves.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(22,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide10.png", 10, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "Your research team was highly motivated by the various blueprints you had acquired during your retreat, and soon you were producing new, terrifying weapons of war - in what limited numbers you could afford, anyway.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(23,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You soon realized that keeping your research team motivated was a critical priority, and you went out of your way to acquire whatever interesting technology you could via various covert operations.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(24,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Friend and foe alike came to respect the bewildering array of advanced technologies your forces came to employ.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(25,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide11.png", 11, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "As the years passed, your would-be opponents found you an unyielding foe. Your troops were both zealous and numerous, and your technologies new and terrifying.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(26,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide12.png", 12, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "GDI troops, warlord armies and mutant raiders alike met grisly ends at the hands of what seemed to be the legion of the future. Your borders soon became known to be nearly impenetrable.";
                    },
                    null,
                    storyDisplay => storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(27,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You watched from the highest window in your temple as your troops chanted your name below you, and your newest advanced aircraft prototypes flew overhead.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(28,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide13.png", 13, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "And so, as the sun rose on another day of your rule, you knew: when Kane returns, he will be most impressed with all you have done.";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(29,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/endingtitle.png", 14);
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "      ENDING IV" + Environment.NewLine + "Peace Through Power";
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
            else if (vassalsVariable != null && vassalsVariable.EnabledThroughPreviousScenario)
            {

                phases.Add(new Phase(17,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/officers.png", 9, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "You had gone out of your way to impress Korkut's former officers and soldiers during the battle with the traitorous general, and the defection of these troops to your side paid dividends.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(19,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "During the subsequent battle with Howard, Korkut's former troops fought bravely, and proved their dedication to your cause.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(20,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "In the aftermath, you found yourself with a devoted and competent army composed of the best of the two combined forces.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(21,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Better yet, you still had significant reserves.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(22,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide9.png", 10, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "The various technologies you had acquired gave you an edge on the battlefield for a short time, but they soon proved difficult to maintain, and they rapidly grew obsolete in the face of GDI's own technological advancements.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(23,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Your research staff were poached by other Nod warlords over the years, and rumors of new Brotherhood weapon development seemed to indicate that your arsenal was gradually falling behind.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(25,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide11.png", 11, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "As the years passed, the ardor of your army compensated for the increasing obsolescence of your military hardware.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(26,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "When inevitably, a rival warlord came crashing over your border with a huge force, your troops charged his with incredible zeal and forced the enemy back in an incredible rout - although your casualties were tremendous.";
                    },
                    null,
                    storyDisplay => storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(27,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You watched from the highest window in your temple as your warriors paraded those who had surrendered before you - then cut their throats, one by one.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(28,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide13.png", 12, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "And so, as the sun rose on another day of your rule, you wondered: what will Kane think of you, when he returns?";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(29,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/endingtitle.png", 12);
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "         ENDING III" + Environment.NewLine + "Brotherhood, Unity, Strength";
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
            else if (blueprintsVariable != null && blueprintsVariable.EnabledThroughPreviousScenario)
            {

                phases.Add(new Phase(17,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/wrecks.png", 9, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "The infighting with Korkut had been extremely costly, with many lives on both sides lost. Although you ultimately prevailed, the officers and soldiers killed that day were irreplaceable.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(18,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "In the aftermath of the subsequent battle with Howard, you found your forces running critically low on manpower.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(19,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You were soon forced to resort to conscripting unwilling locals just to bolster your ranks against the various threats on your borders.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(20,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Many of your exhausted troops were not enthusiastic about merging with the new arrivals, and quickly grew sullen. Only the utter ferocity with which you had dispatched Korkut's men kept them from taking up arms against you.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(21,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It turns out some things are hard to forget.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(22,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide10.png", 10, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "Your research team was highly motivated by the various blueprints you had acquired during your retreat, and soon you were producing new, terrifying weapons of war - in what limited numbers you could afford, anyway.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(23,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You soon realized that keeping your research team motivated was a critical priority, and you went out of your way to acquire whatever interesting technology you could via various covert operations.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(24,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Friend and foe alike came to respect the bewildering array of advanced technologies your forces came to employ.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(25,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide12.png", 11, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "As the years passed, your technological advancements compensated for the imperfection of the human element of your forces.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(26,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "When inevitably, an internal revolt blossomed from within your ranks, you were able to use cybernetic soldiers, high tech vehicles, and never before seen weapons to crush the traitors.";
                    },
                    null,
                    storyDisplay => storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(27,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You watched from the highest window in your temple as your cyborgs corralled those who had surrendered and paraded them before you.  Soon enough the disloyal would be led to their fate.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(28,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide13.png", 12, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "And so, as the sun rose on another day of your rule, you wondered: what will Kane think of you, when he returns?";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(29,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/endingtitle.png", 12);
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "        ENDING II" + Environment.NewLine + "The Technology Of Peace";
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
                phases.Add(new Phase(17,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/wrecks.png", 9, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "The infighting with Korkut had been extremely costly, with many lives on both sides lost. Although you ultimately prevailed, the officers and soldiers killed that day were irreplaceable.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(18,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "In the aftermath of the subsequent battle with Howard, you found your forces running critically low on manpower.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(19,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You were soon forced to resort to conscripting unwilling locals just to bolster your ranks against the various threats on your borders.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(20,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Many of your exhausted troops were not enthusiastic about merging with the new arrivals, and quickly grew sullen. Only the utter ferocity with which you had dispatched Korkut's men kept them from taking up arms against you.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(21,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "It turns out some things are hard to forget.";
                    },
                    null,
                    storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(22,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide9.png", 10, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "The various technologies you had acquired gave you an edge on the battlefield for a short time, but they soon proved difficult to maintain, and they rapidly grew obsolete in the face of GDI's own technological advancements.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(23,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "Your research staff were poached by other Nod warlords over the years, and rumors of new Brotherhood weapon development seemed to indicate that your arsenal was gradually falling behind.";
                    },
                    null,
                    storyDisplay => storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f),
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(24,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "As your enemies gradually encroached on your territory, the culture of fear you had established amongst your followers seemed to slowly disintegrate.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(25,
                    storyDisplay =>
                    {
                        storyDisplay.ConversationDisplay.ConversationText = "You began to see danger around every corner of your Temple.";
                    },
                    null,
                    null,
                    null));

                phases.Add(new Phase(26,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/SE11End/slide13.png", 11, 0f);
                        storyDisplay.ConversationDisplay.ConversationText = "And so, as the sun rose on another day of your rule, you wondered: will you still be here, when Kane returns?";
                    },
                    null,
                    storyDisplay =>
                    {
                        storyDisplay.GetAllStoryImages().ForEach(sti => sti.AlphaRate = -1.0f);
                        storyDisplay.ConversationDisplay.ConversationText = string.Empty;
                    },
                    storyDisplay => storyDisplay.ClearStoryImages()));

                phases.Add(new Phase(27,
                    storyDisplay =>
                    {
                        storyDisplay.AddSimpleStoryImage("Story/SE/Shared/endingtitle.png", 12);
                        storyDisplay.ConversationDisplay.IsCentered = true;
                        storyDisplay.ConversationDisplay.ConversationText = "   ENDING I" + Environment.NewLine + "A Tenuous Grip";
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

            AddSECreditPhases(phases, new Color(176, 196, 222));

            return phases;
        }

        private void AddSECreditPhases(List<Phase> phases, Color color)
        {
            int lastPhaseID = phases[phases.Count - 1].ID;

            phases.Add(new Phase(lastPhaseID + 1,
                storyDisplay =>
                {
                    cutsceneManager.TryPlaySong(assets.tdmaptheme);
                    storyDisplay.ConversationDisplay.ConversationText = "";

                    const double rate = 60.0;

                    var creditsStoryImage = new StoryImage(windowManager, 1);
                    creditsStoryImage.Texture = AssetLoader.LoadTextureUncached("Story/SE/SE11EndCred/secred1.png");
                    creditsStoryImage.Color = color;
                    creditsStoryImage.ImageHeight.SnapToValue(creditsStoryImage.Texture.Height);
                    creditsStoryImage.ImageY.Value = windowManager.RenderResolutionY;
                    creditsStoryImage.ImageY.TargetValue = -creditsStoryImage.Texture.Height;
                    creditsStoryImage.ImageY.Rate = rate;
                    storyDisplay.AddStoryImage(creditsStoryImage);

                    var creditsStoryImage2 = new StoryImage(windowManager, 2);
                    creditsStoryImage2.Texture = AssetLoader.LoadTextureUncached("Story/SE/SE11EndCred/secred2.png");
                    creditsStoryImage2.Color = color;
                    creditsStoryImage2.ImageHeight.SnapToValue(creditsStoryImage2.Texture.Height);
                    creditsStoryImage2.ImageY.Value = creditsStoryImage.ImageY.Value + creditsStoryImage.ImageHeight.Value;
                    creditsStoryImage2.ImageY.TargetValue = -creditsStoryImage2.Texture.Height;
                    creditsStoryImage2.ImageY.Rate = rate;
                    storyDisplay.AddStoryImage(creditsStoryImage2);

                    var creditsStoryImage3 = new StoryImage(windowManager, 3);
                    creditsStoryImage3.Texture = AssetLoader.LoadTextureUncached("Story/SE/SE11EndCred/secred3.png");
                    creditsStoryImage3.Color = color;
                    creditsStoryImage3.ImageHeight.SnapToValue(creditsStoryImage3.Texture.Height);
                    creditsStoryImage3.ImageY.Value = creditsStoryImage2.ImageY.Value + creditsStoryImage2.ImageHeight.Value;
                    creditsStoryImage3.ImageY.TargetValue = -creditsStoryImage3.Texture.Height;
                    creditsStoryImage3.ImageY.Rate = rate;
                    storyDisplay.AddStoryImage(creditsStoryImage3);

                    var creditsStoryImage4 = new StoryImage(windowManager, 4);
                    creditsStoryImage4.Texture = AssetLoader.LoadTextureUncached("Story/SE/SE11EndCred/secred4.png");
                    creditsStoryImage4.Color = color;
                    creditsStoryImage4.ImageHeight.SnapToValue(creditsStoryImage4.Texture.Height);
                    creditsStoryImage4.ImageY.Value = creditsStoryImage3.ImageY.Value + creditsStoryImage3.ImageHeight.Value;
                    creditsStoryImage4.ImageY.TargetValue = -creditsStoryImage4.Texture.Height;
                    creditsStoryImage4.ImageY.Rate = rate;
                    storyDisplay.AddStoryImage(creditsStoryImage4);

                },
                null,
                storyDisplay => cutsceneManager.HideAllStoryImagesWithSound(assets.country1),
                storyDisplay => storyDisplay.ClearStoryImages()));
        }
    }
}
