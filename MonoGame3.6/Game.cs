using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core
{
    public class World : WorldBase
    {
        public Res Res { get; set; }
        public Guy Player;
        public List<List<Tile>> GroundTiles;
        public ScreenShake ScreenShake = new ScreenShake();
        public int GridWidth { get; set; } = 50;
        public int GridHeight { get; set; } = 50;
        public int RockCollideCount;
        public List<Level> Levels = new List<Level>();
        public Level CurLevel = null;
        public List<Artifact> Artifacts = new List<Artifact>();
        public List<DigPile> DigPiles = new List<DigPile>();
        public List<Particle> Particles = new List<Particle>();
        public List<Guy> BadGuys = new List<Guy>();
        public bool InstructionsShown = false;
        public float InstructionsShowTimer = 3;
        public Artifact FoundArtifact = null;
        public float FoundArtifactMoveTimer = .9f;
        public float FoundArtifactMoveTimerMax = .9f;
        public GameState GameState = GameState.Intro;
        public float IntroTimer = 5;
        public float IntroTimerMax = 5;
        public float PlayTime = 30;
        public float PlayTimeMax = 30;
        public Keys ActionKey = Keys.OemPeriod;     //****TODO: Map this to the actual arcade console.
        public float LevelStartTextTimer = 0.9f;
        public float LevelStartTextTimerMax = 0.9f;
        public float LevelEndTextTimer = 4f;
        public float LevelEndTextTimerMax = 4f;
        public bool IntroSongPlayed = false;
        public float GameOverTimer = 3.0f;
        public float GameOverTimerMax = 3.0f;
        public bool DrawNothingButChar = false;
        public bool DrawGameOverText = false;
        public bool DrawPressAnyKey = false;
        public int Score = 0;
        public bool PickOut = false;
        int NumArtifacts = 0;
        int NumPickaxes = 0;
        bool LevelCompletePlayed = false;
        public GameObject Pick;
        private int PickaxeUsage = 1;

        public World(Screen screen, Res res) : base(screen)
        {
            Res = res;

            Levels.Add(new Level(1, 50, 100, 3, Res.SprHudLevel1, 1, 0, 1));
            Levels.Add(new Level(2, 75, 200, 5, Res.SprHudLevel2, 2, 10, 2));
            Levels.Add(new Level(3, 100, 300, 7, Res.SprHudLevel3, 3, 10, 3));

            Pick = new GameObject(this);

            Pick.Frame = Res.Tiles.GetSpriteFrame(Res.SprPickaxe, 0);
        }
        public void CreateColoredParticles(vec2 pos, List<Color> colors)
        {
            float pi = 3.141593f;
            CreateParticles(new ParticleParams(
              Res.Tiles.GetSpriteFrame(Res.SprParticle, 0),

                  new MpInt(10, 15),
                  new MpVec2(Player.GetCenter(), Player.GetCenter()),
                  new MpVec2(new vec2(-2.2f, -2.2f), new vec2(2.2f, 2.2f)),
                  new MpFloat(0, pi * 2),
                  new MpFloat(-pi * 1.6f, pi * 1.6f),
                  new MpFloat(0.5f, 0.6f),
                  new MpFloat(0.8f, 2.9f),
                  new MpFloat(1, 1),
                  new MpFloat(0.58f, 1.77f),
                  colors

              ));
        }
        public void FindArtifact(Artifact a)
        {
            //show above player head
            FoundArtifact = a;
            FoundArtifactMoveTimer = FoundArtifactMoveTimerMax;

            if (a.ArtifactType == ArtifactType.Pickaxe)
            {
                Res.Audio.PlaySound(Res.SfxGetPickaxe);
                PickaxeUsage++;
                CreateColoredParticles(Player.GetCenter(), new List<Color> { Color.Gray, Color.SlateBlue});
            }
            else
            {
                CreateColoredParticles(Player.GetCenter(), new List<Color> { Color.White, Color.LightYellow });

                NumArtifacts--;

                //foreach(Guy b in this.BadGuys)
                //{
                //    b.Stunned = 3.0f;
                //}

                if (NumArtifacts == 0)
                {
                    GameState = GameState.LevelEnd;
                    LevelEndTextTimer = LevelEndTextTimerMax;
                }
                else
                {
                    PlayTime += 5;//5s
                }
                Res.Audio.PlaySound(Res.SfxGetItem);
            }

        }

        public void StartGame(int levelNumber, bool playIntro=false)
        {
            if (playIntro)
            {
                GameState = GameState.Intro;
            }
            else
            {
                GameState = GameState.LevelStart;
            }

            PickOutDelay = 0;
            PickOutTimer = 0;
            PickOut = false;

            LevelStartTextTimer = LevelStartTextTimerMax;

            if (levelNumber >= Levels.Count)
            {
                levelNumber = 0;
            }
            CurLevel = Levels[levelNumber];
            GridWidth = GridHeight = CurLevel.Gridsize;

            IntroTimer = IntroTimerMax;



            //Main Setup stuff
            MakeTileGrid();
            CreateCaveGround();
            CreateDigPiles();

            Player = new Guy(this, Res.SprPasPasLeft, Res.SprPasPasRight, Res.SprPasPasUp, Res.SprPasPasDown, Res.SprPasPasDown);
            Player.Animate = true;

            //Place Player in empty tile
            Tile found = null;
            for(int i=0; i<5000; ++i)
            {
                Tile t = CaveGround[Globals.RandomInt(0, CaveGround.Count)];
                if (t.Pile == null)
                {
                    found = t;
                }
            }
            if (found == null)
            {
                found = CaveGround[0];
            }
            Player.Pos = found.Pos;
            Player.Update(null, 0);

            CreateArtifacts();
            CreateBadGuys();


        }

        public void CreateParticles(ParticleParams pr)
        {
            for (int i = 0; i < pr.Count.Value; ++i)
            {
                Particle p = new Particle(this);
                p.Fade = pr.InitialFade.NewValue();
                p.DeltaFade = pr.DeltaFade.NewValue();
                p.Pos = pr.Pos.NewValue();
                p.Rotation = pr.InitialRotation.NewValue();
                p.RotationDelta = pr.DeltaRotation.NewValue();
                p.LifeMillis = pr.LifespanMillis.NewValue();
                p.Scale = pr.InitialScale.NewValue();
                p.DeltaScale = pr.DeltaScale.NewValue();
                p.Vel = pr.Velocity.NewValue();
                p.Origin = new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileWidthPixels) * 0.5f;

                p.Color = pr.PossibleColors[Globals.RandomInt(0, pr.PossibleColors.Count)];


                if (pr.PossibleSprites.Count > 0 && pr.PossibleFrames.Count > 0)
                {
                    if (Globals.RandomBool())
                    {
                        p.Sprite = pr.PossibleSprites[Globals.RandomInt(0, pr.PossibleSprites.Count)];
                        p.Animate = true;
                        p.Frame = p.Sprite.Frames[0];
                    }
                    else
                    {
                        p.Frame = pr.PossibleFrames[Globals.RandomInt(0, pr.PossibleFrames.Count)];
                    }
                }
                else if (pr.PossibleFrames.Count > 0)
                {
                    p.Frame = pr.PossibleFrames[Globals.RandomInt(0, pr.PossibleFrames.Count)];
                }
                else if (pr.PossibleSprites.Count > 0)
                {
                    p.Sprite = pr.PossibleSprites[Globals.RandomInt(0, pr.PossibleSprites.Count)];
                    p.Animate = true;
                    p.Frame = p.Sprite.Frames[0];
                }

                Particles.Add(p);
            }
        }

        public void UpdateParticles(float dt)
        {
            for (int i = Particles.Count - 1; i >= 0; i--)
            {
                Particle p = Particles[i];

                if (p.LifeMillis > 0)
                {
                    p.LifeMillis -= dt;
                    if (p.LifeMillis <= 0)
                    {
                        Particles.Remove(p);
                        continue;
                    }
                }

                if (p.Fade <= 0)
                {
                    Particles.Remove(p);
                    continue;
                }

                p.Update(Screen.Game.Input, dt);

                p.Pos += p.Vel;
            }
        }

        public void DrawParticles(SpriteBatch sb)
        {
            foreach (Particle p in Particles)
            {
                Screen.DrawFrame(sb, p.Frame, p.Pos, new vec2(12, 12), Color.White, p.Fade, p.Scale, p.Rotation, p.Origin);
            }
        }

        public bool TilesAreAtSamePos(vec2 tilpos_a, vec2 tilepos_b)
        {
            vec2 a = RoundPosToTilePos(tilpos_a);
            vec2 b = RoundPosToTilePos(tilepos_b);

            return ((int)a.x == (int)b.x && (int)a.y == (int)b.y);
        }

        public vec2 R2PosToTilePos(vec2 r2Pos)
        {
            vec2 v = new vec2(r2Pos.x / Res.Tiles.TileWidthPixels, r2Pos.y / Res.Tiles.TileWidthPixels);
            return v;
        }

        public vec2 RoundPosToTilePos(vec2 pos)
        {
            float tw = Res.Tiles.TileWidthPixels;
            vec2 ret = new vec2((float)Math.Round(pos.x / tw), (float)Math.Round(pos.y / tw));

            return ret;
        }

        public Tile GetTileAtPoint(vec2 pos_in_tiles)
        {
            int x = (int)Math.Floor(pos_in_tiles.x);
            int y = (int)Math.Floor(pos_in_tiles.y);

            if (x < GroundTiles.Count)
            {
                if (y < GroundTiles[x].Count)
                {
                    return GroundTiles[x][y];
                }
            }

            return null;
        }

        private void MakeTileGrid()
        {
            GroundTiles = new List<List<Tile>>();
            for (int i = 0; i <= GridWidth; i++)
            {
                List<Tile> tileList = new List<Tile>();

                for (int j = 0; j <= GridHeight; j++)
                {
                    tileList.Add(null);
                }

                GroundTiles.Add(tileList);
            }
        }

        private List<Tile> CaveGround;
        private void CreateCaveGround()
        {
            CaveGround = new List<Tile>();

            int steps = CurLevel.NumCarveSteps;
            int x = GridWidth / 2;
            int y = GridHeight / 2;
            Random rand = new Random();

            for (int i = 0; i < steps; i++)
            {
                StepCaveGround(x, y);

                if (rand.Next(2) == 0)
                {
                    x = rand.Next(2) == 0 ? x + 1 : x - 1;
                }
                else
                {
                    y = rand.Next(2) == 0 ? y + 1 : y - 1;
                }
            }
        }

        private void StepCaveGround(int x, int y)
        {
            List<Tile> tiles = GroundTiles[x];

            if (tiles[y] == null)
            {
                Tile t = new Tile(this);
                t.Frame = Res.Tiles.GetSpriteFrame(Res.SprDirt, 0);
                t.Pos = new vec2(x * 12, y * 12);
                tiles[y] = t;
                CaveGround.Add(t);
            }
        }

        private void CreateBadGuys()
        {
            BadGuys = new List<Guy>();
            for (int i = 0; i < CurLevel.NumBadGuys; ++i)
            {
                Guy b = new Guy(this, Res.SprDinoL, Res.SprDinoR, Res.SprDinoU, Res.SprDinoD, Res.SprDinoStun);
                b.speed = 22 + 10 * CurLevel.Number;

                if (CurLevel.Number > 1)
                {
                    b.AIType = Globals.RandomInt(0, 10) > 8 ? AIType.BackNForth : AIType.Wandering;
                }
                else
                {
                    b.AIType = AIType.Wandering;
                }

                //Place bad guy
                for(int n=0; n<5000; ++n)
                {
                    int a = Globals.RandomInt(0, CaveGround.Count);
                    b.Pos = CaveGround[a].Pos;
                     
                    //Make sure we aren't too close to player
                    if((b.Pos-Player.Pos).Len() > 24)
                    {
                        break;
                    }
                }


                b.Color = Color.White;
                b.Update(null, 0);
                BadGuys.Add(b);
            }
        }

        private void CreateDigPiles()
        {
            DigPiles = new List<DigPile>();
            Random random = new Random();

            for (int i = 0; i < GroundTiles.Count; i++)
            {
                for (int j = 0; j < GroundTiles[i].Count; j++)
                {
                    if (i == GridWidth / 2 && j == GridHeight / 2)
                    {
                        //The guy is here
                        continue;
                    }
                    else
                    {
                        Tile tile = GroundTiles[i][j];
                        if (tile != null && random.Next(101) <= 65)
                        {
                            DigPile newPile = new DigPile(this, i, j);
                            newPile.Frame = Res.Tiles.GetSpriteFrame(Res.SprDigPile, 0);
                            newPile.Pos = new vec2(tile.Pos.x, tile.Pos.y);
                            tile.Pile = newPile;
                            DigPiles.Add(newPile);
                        }
                    }
                }
            }
        }

        private void CreateArtifacts()
        {
            Artifacts = new List<Artifact>();
            DigPile found;
            Artifact art;
            NumArtifacts = 0;
            NumPickaxes = 0;

            //Create Bones
            for (int i = 0; i < CurLevel.NumBones; ++i)
            {
                art = new Artifact(this, (ArtifactType)Globals.RandomInt(0, 7));
                Artifacts.Add(art);
                found = FindEmptyPile();
                found.Artifact = art;
                found.Frame = Res.Tiles.GetSpriteFrame(Res.SprArtifactGem, 0);

                NumArtifacts++;
            }
            for (int i = 0; i < CurLevel.NumPickaxes; ++i)
            {
                art = new Artifact(this, ArtifactType.Pickaxe);
                Artifacts.Add(art);

                found = FindEmptyPile();
                found.Artifact = art;
                found.Frame = Res.Tiles.GetSpriteFrame(Res.SprPickaxe, 0);

                NumPickaxes++;
            }
            //Create Dino Egg
            art = new Artifact(this, (ArtifactType)Globals.RandomInt(7, 9));
            Artifacts.Add(art);
            found = FindEmptyPile();
            found.Artifact = art;
            found.Frame = Res.Tiles.GetSpriteFrame(Res.SprArtifactGem, 0);
            NumArtifacts++;
        }

        public DigPile FindEmptyPile()
        {
            DigPile found = null;
            while (true)
            {
                int n = Globals.RandomInt(0, DigPiles.Count);
                found = DigPiles[n];
                if (found.Artifact == null)
                {
                    break;
                }
            }
            return found;
        }
        private vec2 PickaxePoint = new vec2(0, 0);

        public override void Draw(SpriteBatch sb)
        {
            vec2 spriteWh = new vec2(Res.Tiles.TileWidthPixels, Res.Tiles.TileHeightPixels);
            if (DrawNothingButChar == false)
            {
                foreach (List<Tile> tileList in GroundTiles)
                {
                    foreach (Tile t in tileList)
                    {
                        if (t != null)
                        {
                            Screen.DrawFrame(sb, t.Frame, t.Pos, spriteWh, Color.White);
                        }
                    }
                }

                //Dig Piles
                foreach (DigPile dp in DigPiles)
                {
                    Screen.DrawFrame(sb, dp.Frame, dp.Pos, spriteWh, Color.White);
                }
            }

            //Draw Guy
            if (Player != null)
            {
                Screen.DrawFrame(sb, Player.Frame, Player.Pos, spriteWh, Player.Color);

                if (Player.Artifacts != null)
                {
                    int x = 0;
                    foreach (Artifact artifact in Player.Artifacts)
                    {
                        DrawHudItem(sb, new vec2(x * 12, Screen.Viewport.HeightPixels - 12), new vec2(12, 12), artifact.SpriteName);
                        x++;
                    }
                }
            }

            if (PickOut)
            {
                //float r = 0;//rotationradians
                vec2 dp = new vec2(0, 0); //sprite origin relative to 12x12 sprite  and player
                vec2 pickaxeOrigin = new vec2(6, 12);
                PickaxePoint = new vec2(0, 0);

                float a = ((PickOutTimerMax - PickOutTimer) / PickOutTimerMax) * 3.1415927f * 2;
                vec2 playerOrigin = Player.Pos + new vec2(6, 6);
                vec2 pickaxeNormal = new vec2((float)Math.Cos(a), (float)Math.Sin(a));
                vec2 pickaxePos = playerOrigin + pickaxeNormal * 6.0f;

                PickaxePoint = playerOrigin + pickaxeNormal * 13.0f;

                Frame pa = Res.Tiles.GetSpriteFrame(Res.SprPickaxe, 0);
                Screen.DrawFrame(sb, pa, pickaxePos, spriteWh, Color.White, 1, 1, a + (3.1415927f*0.5f), pickaxeOrigin);
            }

            if (DrawNothingButChar == false)
            {
                //Artifact the guy holds
                if (FoundArtifact != null)
                {
                    vec2 pos = Player.Pos;

                    pos.y -= 5 + 12 * ((FoundArtifactMoveTimerMax-FoundArtifactMoveTimer) / FoundArtifactMoveTimerMax);

                    Screen.DrawFrame(sb, FoundArtifact.Frame, FoundArtifact.Pos, spriteWh, FoundArtifact.Color);
                }

                //Draw Bad Guys
                if (BadGuys != null)
                {
                    foreach (Guy g in BadGuys)
                    {
                        if (g.Frame != null)
                        {
                            Screen.DrawFrame(sb, g.Frame, g.Pos, spriteWh, g.Color);
                        }
                    }
                }

                DrawParticles(sb);

                //Draw HUD and STATE stuff
                if (GameState == GameState.Intro)
                {
                    vec2 pos = new vec2(), wh = new vec2();
                    CenterHudItem(0.8f, 0.2f, ref pos, ref wh);

                    //Move the text down for 1/2 of the intro time
                    float ypct = Math.Min((IntroTimerMax - IntroTimer) / (IntroTimerMax * 0.2f), 1.0f);
                    pos.y = ypct * pos.y;

                    DrawHudItem(sb, pos, wh, Res.SprHudTitle, ypct, ypct, 6.28f * ypct, wh * 0.5f);

                    if (IntroSongPlayed == false)
                    {
                        IntroSongPlayed = true;
                        Res.Audio.PlaySound(Res.SfxIntro);
                    }
                }
                else if (GameState == GameState.ShowInstructions)
                {
                    vec2 pos = new vec2(), wh = new vec2();
                    CenterHudItem(0.8f, 0.8f, ref pos, ref wh);
                    if (InstructionsShowTimer <= 0)
                    {
                        DrawHudItem(sb, pos, wh, Res.SprHudInstructionsWithOk);
                    }
                    else
                    {
                        DrawHudItem(sb, pos, wh, Res.SprHudInstructions);
                    }
                }
                else if (GameState == GameState.LevelStart)
                {
                    vec2 pos = new vec2(), wh = new vec2();
                    CenterHudItem(0.6f, 0.2f, ref pos, ref wh);
                    DrawHudItem(sb, pos, wh, CurLevel.LevelSpriteName);
                }
                else if (GameState == GameState.LevelPlay)
                {
                    string timeVal = Math.Round(PlayTime, 3).ToString();
                    Color haloColor = Color.White;
                    Color textColor = Color.Black;
                    if (PlayTime < 10)
                    {
                        haloColor = Color.Red;
                        textColor = Color.Red;
                    }
                    DrawStringHalo(sb, timeVal, 1, 1, haloColor, textColor);
                }

                else if (GameState == GameState.LevelEnd)
                {
                    if (LevelCompletePlayed == false)
                    {
                        Res.Audio.PlaySound(Res.SfxLevelComplete);
                        LevelCompletePlayed = true;
                    }
                    vec2 pos = new vec2(), wh = new vec2();
                    CenterHudItem(0.7f, 0.2f, ref pos, ref wh);
                    DrawHudItem(sb, pos, wh, Res.SprHudSuccess);
                }
            }
            if (GameState != GameState.Intro && GameState != GameState.ShowInstructions)
            {
                float w = this.Screen.Game.GraphicsDevice.Viewport.Width;
                float h = this.Screen.Game.GraphicsDevice.Viewport.Height;
                float textx = w - w * 0.38f;
                DrawStringHalo(sb, "Score:" +Score.ToString(), (int)textx, 1, Color.Fuchsia, Color.Yellow);

                float paw = w - w * 0.08f ;
                DrawHudItem(sb, new vec2(Screen.Viewport.WidthPixels - 22, Screen.Viewport.HeightPixels - 12), new vec2(12, 12), Res.SprPickaxe,0.5f);
                DrawStringHalo(sb, "x" + PickaxeUsage.ToString(), (int)(paw), (int) h-32, Color.Blue, Color.Orange);
            }

            if (GameState == GameState.GameOver)
            {
                if (DrawGameOverText == true)
                {
                    vec2 pos = new vec2(), wh = new vec2();
                    CenterHudItem(0.7f, 0.2f, ref pos, ref wh);

                    pos.y -= wh.y * 0.75f;//move the text so we can see the char

                    DrawHudItem(sb, pos, wh, Res.SprHudGameOver);
                }
                if (DrawPressAnyKey == true)
                {
                    vec2 pos = new vec2(), wh = new vec2();
                    CenterHudItem(0.7f, 0.2f, ref pos, ref wh);

                    pos.y += wh.y * 0.75f;//move the text so we can see the char

                    DrawHudItem(sb, pos, wh, Res.SprHudPressAnyKey);
                }
            }
        }

        public void CenterHudItem(float pctWidth, float pctHeight, ref vec2 pos, ref vec2 wh)
        {
            pos = new vec2();
            wh = new vec2();

            pos.x = Screen.Viewport.WidthPixels - Screen.Viewport.WidthPixels * pctWidth;
            pos.y = Screen.Viewport.HeightPixels - Screen.Viewport.HeightPixels * pctHeight;

            pos.x *= 0.5f;
            pos.y *= 0.5f;

            wh.x = Screen.Viewport.WidthPixels * pctWidth;
            wh.y = Screen.Viewport.HeightPixels * pctHeight;
        }

        private void DrawHudItem(SpriteBatch sb, vec2 screen_xy, vec2 screen_wh, string spriteName, float a = 1.0f, float scale = 1.0f, float rotation = 0.0f, vec2 origin = default(vec2))
        {
            vec2 rel_pos = screen_xy;

            Frame f = Res.Tiles.GetSpriteFrame(spriteName, 0);

            //Screen.DrawFrame(sb, f, Screen.Viewport.Pos + screen_xy, screen_wh, Color.White, a, scale, rotation, origin);
            Screen.DrawFrame(sb, f, Screen.Viewport.Pos + screen_xy + (origin * 0.5f), screen_wh, Color.White, a, scale, rotation, origin);
        }

        private void DrawStringHalo(SpriteBatch sb, string value, int x, int y, Color haloColor, Color textColor)
        {
            sb.DrawString(Res.Font, value, new Vector2(x - 1, y), haloColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);
            sb.DrawString(Res.Font, value, new Vector2(x - 1, y - 1), haloColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);
            sb.DrawString(Res.Font, value, new Vector2(x - 1, y + 1), haloColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);
            sb.DrawString(Res.Font, value, new Vector2(x + 1, y), haloColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);
            sb.DrawString(Res.Font, value, new Vector2(x + 1, y - 1), haloColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);
            sb.DrawString(Res.Font, value, new Vector2(x + 1, y + 1), haloColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);
            sb.DrawString(Res.Font, value, new Vector2(x, y - 1), haloColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);
            sb.DrawString(Res.Font, value, new Vector2(x, y + 1), haloColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);
            sb.DrawString(Res.Font, value, new Vector2(x, y), textColor, 0.0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0.0f);

        }

        public override void Update(float dt)
        {

            if (GameState == GameState.LevelPlay || GameState == GameState.GameOver)
            {
                if (FoundArtifactMoveTimer >= 0)
                {
                    FoundArtifactMoveTimer -= dt;
                }


                Player.Update(Screen.Game.Input, dt);

                if (GameState == GameState.LevelPlay)
                {

                    foreach (Guy b in BadGuys)
                    {
                        b.Update(Screen.Game.Input, dt);
                    }
                    UpdateParticles(dt);

                    foreach (Guy g in BadGuys)
                    {
                        //Player Die
                        if (g.CollidesWith(Player))
                        {
                            if(g.Stunned <= 0)
                            {
                                vec2 pos = g.GetCenter() + (Player.GetCenter() - g.GetCenter()) * 0.5f;
                                CreateColoredParticles(pos, new List<Color> { Color.Red, Color.IndianRed, Color.HotPink });

                                Res.Audio.PlaySound(Res.SfxCharHit);

                                ShowGameOver();
                                ScreenShake.Shake(10);
                                break;
                            }

                        }

                        if (PickOut)
                        {
                            if (g.CollidesWidth_Inclusive(PickaxePoint))
                            {
                                g.Stunned = 3f;
                                Res.Audio.PlaySound(Res.SfxHitGuy);
                            }
                        }

                    }
                }
            }

            ScreenShake.Update(dt);

            UpdateViewport(dt);

            UpdateGameState(dt);
        }
        private void ShowGameOver()
        {
            GameOverTimer = GameOverTimerMax;
            GameState = GameState.GameOver;
            Player.Sprite = Res.Tiles.GetSprite(Res.SprCharDie);
            Player.Animate = true;
            Player.Loop = false;
            DrawNothingButChar = true;
            Res.Audio.PlaySound(Res.SfxDie);
        }

        public float PickOutTimer = 0.3f;
        public float PickOutTimerMax = 0.3f;
        public float PickOutDelay = 0.0f;
        public float PickOutDelayMax = 3.0f;

        public void UpdateGameState(float dt)
        {
            if (GameState == GameState.Intro)
            {
                IntroTimer -= dt;
                if (IntroTimer <= 0)
                {
                    GameState = GameState.ShowInstructions;
                }
            }
            if (GameState == GameState.ShowInstructions)
            {
                InstructionsShowTimer -= dt;
                if (InstructionsShowTimer <= 0) { InstructionsShowTimer = 0; }

                if (Keyboard.GetState().IsKeyDown(ActionKey))
                {
                    StartGameFromBeginning();
                }
            }
            if (GameState == GameState.LevelStart)
            {
                LevelStartTextTimer -= dt;

                if (LevelStartTextTimer <= 0)
                {
                    GameState = GameState.LevelPlay;
                }
            }
            if (GameState == GameState.LevelPlay)
            {
                PlayTime -= dt;

                if (PlayTime <= 0)
                {
                    PlayTime = 0;
                    ShowGameOver();
                }

                if (Keyboard.GetState().IsKeyDown(ActionKey))
                {
                    if (PickaxeUsage > 0)
                    {
                        if (PickOutDelay <= 0.0001)
                        {
                            if (PickOut == false)
                            {
                                PickOut = true;
                                PickOutTimer = PickOutTimerMax;
                                Res.Audio.PlaySound(Res.SfxPickaxeSwing);
                                PickaxeUsage--;
                            }
                        }
                    }

                }
                PickOutTimer -= dt;
                if(PickOutTimer <= 0)
                {
                    PickOutTimer = 0;
                    PickOut = false;
                }
                PickOutDelay -= dt;
                if(PickOutDelay <= 0)
                {
                    PickOutDelay = 0;
                }
            }
            if (GameState == GameState.LevelEnd)
            {
                LevelEndTextTimer -= dt;
                if (LevelEndTextTimer <= 0)
                {
                    StartGame(CurLevel.Number);//We're passing azero based number here so curlevel.Number is Level # + 1
                }
            }
            if (GameState == GameState.GameOver)
            {
                if (Player.AnimationEnded())
                {
                    DrawGameOverText = true;
                }
                GameOverTimer -= dt;
                if (GameOverTimer <= 0)
                {
                    GameOverTimer = 0;
                    DrawPressAnyKey = true;
                }
                if (DrawPressAnyKey)
                {
                    if (Keyboard.GetState().IsKeyDown(ActionKey))
                    {
                        DrawNothingButChar = false;
                        DrawGameOverText = false;
                        DrawPressAnyKey = false;
                        StartGameFromBeginning();
                    }
                }

            }

        }
        public void StartGameFromBeginning()
        {
            //Start New Game
            Score = 0;
            PickaxeUsage = 1;
            PlayTime = PlayTimeMax;
            StartGame(0);
        }
        public void UpdateViewport(float dt)
        {
            //Viewport Update
            //Makes ure the viewport doesn't go past these values.
            Screen.Viewport.Pos = new vec2(
                Player.Pos.x - Res.Tiles.TileWidthPixels * Screen.Viewport.TilesWidth * 0.5f + Res.Tiles.TileWidthPixels * 0.5f
                , -Res.Tiles.TileHeightPixels * 2.0f) + ScreenShake.ScreenShakeOffset; ;

            //Guy Bounds.
            float YBorder = Res.Tiles.TileHeightPixels;
            if (Player.Pos.y < Screen.Viewport.Pos.y + YBorder)
            {
                Screen.Viewport.Pos = new vec2(Screen.Viewport.Pos.x, Player.Pos.y - YBorder);
            }
            if (Player.Pos.y > Screen.Viewport.Pos.y + Screen.Viewport.HeightPixels - YBorder)
            {
                Screen.Viewport.Pos = new vec2(Screen.Viewport.Pos.x, Player.Pos.y - Screen.Viewport.HeightPixels + YBorder);
            }
            Screen.Viewport.Pos = new vec2(Screen.Viewport.Pos.x, Screen.Viewport.Pos.y + Screen.Viewport.TilesHeight * Res.Tiles.TileWidthPixels * 0.5f);


            //We need second bounds check for the actual "game area'
            //If the guy falls out of the screen where we don't want to go
        }
    }
    public class GameScreen : Screen
    {
        public World World;

        public override void Init(GameBase game)
        {
            base.Init(game);
            World = new World(this, (game as MainGame).Res);
            World.StartGame(0, true);

        }
        public override void Update(float dt)
        {
            base.Update(dt);
            World.Update(dt);
        }
        public override void Draw()
        {
            base.BeginDraw();
            World.Draw(SpriteBatch);
            base.DrawMenu();//Must come after world.draw
            base.EndDraw();
        }
    }
    public class MainGame : GameBase
    {
        GraphicsDeviceManager graphics;
        GameScreen GameScreen;
        Screen _objCurScreen = null;
        GameData GameData;

        public float GetHighScore()
        {
            GameData.Load();
            return GameData.HighScore;
        }
        public void SetHighScore(float f)
        {
            GameData.HighScore = f;
            GameData.Save();
        }
        public void Init(bool bFullscreen, GameSystem gs)
        {
            GameSystem = gs;
            GameData = new GameData(this);
            GameData.Load();

            //TODO: reset this for winnitron derek 10/2/18
            graphics.IsFullScreen =  false;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.ApplyChanges();

            Window.Title = "Eggsplorer";
        }





        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //**on Android, fullscreen just hides the menu bar.
            //On Desktop - it's, well fullscreen..

            this.IsMouseVisible = true;//Dnr
                                       //We can't have constructors because of XAML
                                       //Fuxking XAML

            ///Variable time setp.
            /////So with fixed stepping, XNA will call Upate() multiple times to keep up.
            //Setting this to false, makes it variable, and XNA executes Update/Draw in succession
            this.IsFixedTimeStep = false;

        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            Res = new Res(Content);
            Res.Load(this.GraphicsDevice);

            //Do not do any usage of GameSystem ehre

            ShowScreen = ShowScreen.Game;
        }
        protected override void UnloadContent()
        {
        }
        float waitmax = 2.0f;
        float wait = 0.0f;
        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(dt);


            //Wait state - to allow the screen to rotate
            if (true)//wait >= waitmax)
            {
                if (ShowScreen == ShowScreen.Game)
                {
                    if (GameScreen == null)
                    {
                        GameScreen = new GameScreen();
                        GameScreen.Init(this);
                    }

                    _objCurScreen = GameScreen;
                }
                else if (ShowScreen == ShowScreen.Battle)
                {
                }
                ShowScreen = ShowScreen.None;
            }
            else
            {
            }

            if (_objCurScreen != null)
            {
                _objCurScreen.Update(dt);
            }
            //If we touch screen, then hide the nav if it isn't hidden.
            if (Input.Global.TouchState == TouchState.Press || Input.Global.TouchState == TouchState.Release)
            {
                GameSystem.HideNav();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_objCurScreen != null)
            {
                _objCurScreen.Draw();
            }

            base.Draw(gameTime);
        }
    }
}