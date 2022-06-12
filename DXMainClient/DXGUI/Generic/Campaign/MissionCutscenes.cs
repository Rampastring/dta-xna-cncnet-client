using ClientCore;
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
            country1 = new EnhancedSoundEffect("Story/COUNTRY1.WAV");
            country4 = new EnhancedSoundEffect("Story/COUNTRY4.WAV");
            toney4 = new EnhancedSoundEffect("Story/TONEY4.WAV");
            toney7 = new EnhancedSoundEffect("Story/TONEY7.WAV");
            mapwipe2 = new EnhancedSoundEffect("Story/MAPWIPE2.WAV");
            mapwipe5 = new EnhancedSoundEffect("Story/MAPWIPE5.WAV");

            ramap = AssetLoader.LoadSong("Story/Music/ramap");
            raintro = AssetLoader.LoadSong("Story/Music/raintro");
            fac2226m = AssetLoader.LoadSong("Story/Music/fac2226m");
            secondhand = AssetLoader.LoadSong("Story/Music/2nd_hand");
        }

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

        private IStoryDisplay storyDisplay;
        private WindowManager windowManager;

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
            }

            return null;
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
                    MediaPlayer.Play(ramap);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;
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
                    storyDisplay.ConversationDisplay.ConversationText = "Afterwards the Government was making quick progress. It seemed that the resistance was on its last legs and would fall within weeks.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "The sudden shift in the situation was recently discussed by experts in live TV. While discussing Government military failures, the transmission was suddenly cut, with the broadcasting company pointing to technical issues as the reason.";
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
                null,
                storyDisplay =>
                {
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

                    MediaPlayer.Play(secondhand);
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value * CONVERSATION_MUSIC_VOLUME_MODIFIER;
                },
                null,
                null,
                null));

            phases.Add(new Phase(13,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.IsCentered = false;
                    storyDisplay.ConversationDisplay.ConversationText = "As you already know from the news, the enemy has suddenly strengthened their forces and is in the process of conquering an industrial area.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Turquoise;
                    storyDisplay.AddSimpleStoryImage("Story/CR03/officebg01.png", 2, 0f);
                    country1.Play();
                },
                null,
                null,
                null));

            phases.Add(new Phase(14,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "They've already captured the area's oil refineries, but we still have a slight foothold with a base.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(15,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "I want you to reinforce the base and take that industrial area back. We cannot let it fall.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(16,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "The GDI isn't taking this seriously enough. They haven't yet realized that we're fighting not only for ourselves, but also for them. I bet the Brotherhood of Nod would like to form an alliance with a renewed Soviet Union.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(17,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "Hopefully they'll change their minds soon enough. Until then, we have to do with our own forces and the limited support we get from them.";
                },
                null,
                null,
                null));

            phases.Add(new Phase(18,
                storyDisplay =>
                {
                    storyDisplay.ConversationDisplay.ConversationText = "You have proven to be a great asset so far. I hope you do not disappoint here either.";
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

                    MediaPlayer.Play(secondhand);
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
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "The Global Defense Initiative has finally displayed some signs of usefulness and they've provided us with information.",
                null,
                null,
                null));

            phases.Add(new Phase(5,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "They say they can use airplanes to bomb the insurgents' Construction Yard, if we first take out their anti-air defenses.",
                null,
                null,
                null));

            phases.Add(new Phase(6,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "They also provided us some Orca aircraft. You'll get our best Commando under your command. Use him and the Orcas to destroy the SAM sites, and the GDI best do the rest.",
                null,
                null,
                null));

            phases.Add(new Phase(7,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "We have an informant in the local church who might be able to provide knowledge of enemy movements to your Commando.",
                null,
                null,
                null));

            phases.Add(new Phase(8,
                storyDisplay => storyDisplay.ConversationDisplay.ConversationText = "We will win this fight. Wherever they settle, we'll find them and quench them like bugs.",
                null,
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
                    MediaPlayer.Play(raintro);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = (float)UserINISettings.Instance.ScoreVolume.Value;

                    storyDisplay.ConversationDisplay.ConversationText = "After the fall of the Soviet Union in the Second World War, most of the Soviet republics gained independence and were militarily integrated into the newly-formed Global Defense Initiative.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "For some areas this transition led into a successful, prosperous future...";
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
                    storyDisplay.ConversationDisplay.ConversationText = "... while for others, it led to stagnation where the damages of the great war were just barely being repaired.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "The Allies, now GDI, while keeping an eye, did not micromanage the countries. The ex-Soviet republics were mostly self-governing, with support from the Allies.";
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
                    storyDisplay.ConversationDisplay.ConversationText = "After various smaller conflicts with mixed results, the latest move of the Communist radicals was to take control of abandoned Soviet bases and recommission them, using them to produce military equipment.";
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
                    MediaPlayer.Play(fac2226m);

                    storyDisplay.ConversationDisplay.ConversationText = "This is your first assignment.";
                    storyDisplay.ConversationDisplay.TextColor = Color.Yellow;
                },
                null,
                null,
                storyDisplay => { storyDisplay.ClearStoryImages(); storyDisplay.AddSimpleStoryImage("Story/CR01/bg10.png", 10, 1.0f); }
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
