using Microsoft.Xna.Framework.Media;
using Rampastring.XNAUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTAClient.DXGUI.Generic.Campaign
{
    public class CutsceneResources
    {
        public CutsceneResources()
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
            ionbeam = new EnhancedSoundEffect("Story/Sounds/IONBEAM.WAV");

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

        public readonly EnhancedSoundEffect beepy2;
        public readonly EnhancedSoundEffect beepy3;
        public readonly EnhancedSoundEffect bleep9;
        public readonly EnhancedSoundEffect bleep11;
        public readonly EnhancedSoundEffect bleep12;
        public readonly EnhancedSoundEffect bleep17;
        public readonly EnhancedSoundEffect country1;
        public readonly EnhancedSoundEffect country4;
        public readonly EnhancedSoundEffect tone5;
        public readonly EnhancedSoundEffect toney4;
        public readonly EnhancedSoundEffect toney7;
        public readonly EnhancedSoundEffect mapwipe2;
        public readonly EnhancedSoundEffect mapwipe5;
        public readonly EnhancedSoundEffect newtarg1;
        public readonly EnhancedSoundEffect powrdn1;
        public readonly EnhancedSoundEffect world2;
        public readonly EnhancedSoundEffect ionbeam;

        public readonly Song ramap;
        public readonly Song raintro;
        public readonly Song fac2226m;
        public readonly Song secondhand;
        public readonly Song terminat;
        public readonly Song chrg226m;
        public readonly Song vector1a;
        public readonly Song hellmarch;
        public readonly Song tdmaptheme;
        public readonly Song stopthem;
        public readonly Song gloom;
    }
}
