using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class Res
    {
        public Audio Audio { get; private set; }
        public Tiles Tiles { get; private set; }
        public SpriteFont Font { get; private set; }
        public SpriteFont Font2 { get; private set; }
        ContentManager Content;

        public string SprDirt = "SprDirt";
        public string SprPasPasDown = "SprPasPasDown";
        public string SprPasPasUp = "SprPasPasUp";
        public string SprPasPasRight = "SprPasPasRight";
        public string SprPasPasLeft = "SprPasPasLeft";
        public string SprBackground = "SprBackground";
        public string SprDigPile = "SprDigPile";
        public string SprDialogBox = "SprDialogBox";
        public string SprBattleBackground = "SprBattleBackground";
        public string SprHPBar = "SprHPBar";
        public string SprPickaxe = "SprPickaxe";
        public string SprDustParticle = "SprDustParticle";

        public string SprDinoU = "SprDinoU";
        public string SprDinoD = "SprDinoD";
        public string SprDinoL = "SprDinoL";
        public string SprDinoR = "SprDinoR";

        public string SprHudTitle = "SprHudTitle";
        public string SprHudLevel1 = "SprHudLevel1";
        public string SprHudLevel2 = "SprHudLevel2";
        public string SprHudLevel3 = "SprHudLevel3";
        public string SprHudGameOver = "SprHudGameOver";
        public string SprHudSuccess = "SprHudSuccess";
        public string SprArtifactGem = "SprArtifactGem";
        public string SprHudPressAnyKey = "SprHudPressAnyKey";

        public string SprHudInstructions = "SprHudInstructions";
        public string SprHudInstructionsWithOk = "SprHudInstructionsWithOk";
        public string SprParticle = "SprParticle";


        public string SprArtifactBone1 = "SprArtifactBone1";
        public string SprArtifactBone2 = "SprArtifactBone2";
        public string SprArtifactBone3 = "SprArtifactBone3";
        public string SprArtifactBone4 = "SprArtifactBone4";
        public string SprArtifactBone5 = "SprArtifactBone5";
        public string SprArtifactBone6 = "SprArtifactBone6";
        public string SprArtifactBone7 = "SprArtifactBone7";
        public string SprArtifactEgg1 = "SprArtifactEgg1";
        public string SprArtifactEgg2 = "SprArtifactEgg2";
        public string SprArtifactEgg3 = "SprArtifactEgg3";
        public string SprCharDie = "SprCharDie";

        public string SfxIntro = "Intro";
        public string SfxDie = "Die";
        public string SfxGetItem = "GetItem";
        public string SfxLevelComplete = "LevelComplete";
        public string SfxUncoverItem = "UncoverItem";
        public string SfxCharHit = "CharHit";

        public const float guySpeed = 0.6f;

        public Res(ContentManager c)
        {
            Content = c;
            Audio = new Audio();
            Tiles = new Tiles();

        }
        public void Load(GraphicsDevice d)
        {
            Font = Content.Load<SpriteFont>("Font");
            Font2 = Content.Load<SpriteFont>("Font2");

            Tiles.Texture = Content.Load<Texture2D>("monsters-12x12"); ;

            Tiles.AddSprite(SprPickaxe, new List<Rectangle>() { new Rectangle(3, 0, 1, 1) }, 0f);
            Tiles.AddSprite(SprBackground, new List<Rectangle>() { new Rectangle(5, 0, 1, 1) }, 0f);
            Tiles.AddSprite(SprDirt, new List<Rectangle>() { new Rectangle(1, 8, 1, 1) }, 0f);
            Tiles.AddSprite(SprDigPile, new List<Rectangle>() { new Rectangle(0, 8, 1, 1) }, 0f);
            Tiles.AddSprite(SprBattleBackground, new List<Rectangle>() { new Rectangle(0, 16, 7, 6) }, 0f);
            Tiles.AddSprite(SprHPBar, new List<Rectangle>() { new Rectangle(2, 8, 3, 1) }, 0f);
            Tiles.AddSprite(SprHudTitle, new List<Rectangle>() { new Rectangle(4, 0, 13, 2) }, 0);
            Tiles.AddSprite(SprHudLevel1, new List<Rectangle>() { new Rectangle(4, 2, 8, 2) }, 0);
            Tiles.AddSprite(SprHudLevel2, new List<Rectangle>() { new Rectangle(4, 4, 8, 2) }, 0);
            Tiles.AddSprite(SprHudLevel3, new List<Rectangle>() { new Rectangle(4, 6, 8, 2) }, 0);
            Tiles.AddSprite(SprHudGameOver, new List<Rectangle>() { new Rectangle(4, 10, 12, 2) }, 0);
            Tiles.AddSprite(SprDustParticle, new List<Rectangle>() { new Rectangle(0, 9, 1, 1) }, 0);
            Tiles.AddSprite(SprHudInstructions, new List<Rectangle>() { new Rectangle(12, 2, 12, 8) }, 0);
            Tiles.AddSprite(SprHudInstructionsWithOk, new List<Rectangle>() { new Rectangle(24, 2, 12, 8) }, 0);
            Tiles.AddSprite(SprHudSuccess, new List<Rectangle>() { new Rectangle(4, 8, 8, 2) }, 0);

            Tiles.AddSprite(SprParticle, new List<Rectangle>() { new Rectangle(1, 11, 1, 1) }, 0);


            Tiles.AddSprite(SprArtifactGem, new List<Rectangle>() { new Rectangle(0, 11, 1, 1) }, 0);

            Tiles.AddSprite(SprHudPressAnyKey, new List<Rectangle>() { new Rectangle(10, 12, 8, 1) },0);

            Tiles.AddSprite(SprArtifactBone1, new List<Rectangle>() { new Rectangle(3, 1, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactBone2, new List<Rectangle>() { new Rectangle(3, 2, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactBone3, new List<Rectangle>() { new Rectangle(3, 3, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactBone4, new List<Rectangle>() { new Rectangle(3, 4, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactBone5, new List<Rectangle>() { new Rectangle(3, 5, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactBone6, new List<Rectangle>() { new Rectangle(3, 6, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactBone7, new List<Rectangle>() { new Rectangle(3, 7, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactEgg1, new List<Rectangle>() { new Rectangle(2, 8, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactEgg2, new List<Rectangle>() { new Rectangle(2, 9, 1, 1) }, 0);
            Tiles.AddSprite(SprArtifactEgg3, new List<Rectangle>() { new Rectangle(2, 10, 1, 1) }, 0);

            Tiles.AddSprite(SprCharDie, new List<Rectangle>() {

                new Rectangle(1, 12, 1, 1),
                new Rectangle(2, 12, 1, 1),
                new Rectangle(1, 1, 1, 1),
                new Rectangle(3, 12, 1, 1),

                new Rectangle(1, 12, 1, 1),
                new Rectangle(2, 12, 1, 1),
                new Rectangle(1, 1, 1, 1),
                new Rectangle(3, 12, 1, 1),

                new Rectangle(1, 12, 1, 1),
                new Rectangle(2, 12, 1, 1),
                new Rectangle(1, 1, 1, 1),
                new Rectangle(3, 12, 1, 1),

                new Rectangle(4, 12, 1, 1),
                new Rectangle(4, 12, 1, 1),
                new Rectangle(4, 12, 1, 1),
                new Rectangle(4, 12, 1, 1),
                new Rectangle(4, 12, 1, 1),
                new Rectangle(4, 12, 1, 1),
                new Rectangle(5, 12, 1, 1),
                new Rectangle(5, 12, 1, 1),
                new Rectangle(5, 12, 1, 1),
                new Rectangle(5, 12, 1, 1),
                new Rectangle(5, 12, 1, 1),
                new Rectangle(6, 12, 1, 1),
            }, 1.8f);

            CharBlock12(new ivec2(0, 0), SprPasPasUp, SprPasPasDown, SprPasPasLeft, SprPasPasRight);
            CharBlock12(new ivec2(0, 4), SprDinoU, SprDinoD, SprDinoL, SprDinoR);

            Tiles.AddSprite(SprDialogBox, new List<Rectangle>() {
               new Rectangle(3, 2, 1, 1),
               new Rectangle(4, 2, 1, 1),
               new Rectangle(5, 2, 1, 1),
               new Rectangle(3, 3, 1, 1),
               new Rectangle(4, 3, 1, 1),
               new Rectangle(5, 3, 1, 1)}, 0f);

            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxIntro));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxDie));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGetItem));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxLevelComplete));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxUncoverItem));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxCharHit));
        }

        private void CharBlock12(ivec2 a, string u, string d, string l, string r)
        {
            Tiles.AddSprite(u, new List<Rectangle>() {
               new Rectangle(a.x + 0, a.y + 1, 1, 1),
               new Rectangle(a.x + 1, a.y + 1, 1, 1),
               new Rectangle(a.x + 2, a.y + 1, 1, 1),
               new Rectangle(a.x + 1, a.y + 1, 1, 1)}, guySpeed);

            Tiles.AddSprite(d, new List<Rectangle>() {
               new Rectangle(a.x + 0, a.y + 0, 1, 1),
               new Rectangle(a.x + 1, a.y + 0, 1, 1),
               new Rectangle(a.x + 2, a.y + 0, 1, 1),
               new Rectangle(a.x + 1, a.y + 0, 1, 1)}, guySpeed);

            Tiles.AddSprite(l, new List<Rectangle>() {
               new Rectangle(a.x + 0, a.y + 3, 1, 1),
               new Rectangle(a.x + 1, a.y + 3, 1, 1),
               new Rectangle(a.x + 2, a.y + 3, 1, 1),
               new Rectangle(a.x + 1, a.y + 3, 1, 1)}, guySpeed);

            Tiles.AddSprite(r, new List<Rectangle>() {
               new Rectangle(a.x + 0, a.y + 2, 1, 1),
               new Rectangle(a.x + 1, a.y + 2, 1, 1),
               new Rectangle(a.x + 2, a.y + 2, 1, 1),
               new Rectangle(a.x + 1, a.y + 2, 1, 1)}, guySpeed);

        }


    }
}
