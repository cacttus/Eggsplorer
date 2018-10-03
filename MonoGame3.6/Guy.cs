using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core
{

    public class Guy : GameObject
    {
        public float speed = 48f;
        private vec2 LastAlignTilePos;
        public Direction FacingDirection { get; set; } = Direction.Down;
        private Direction NextDirection = Direction.Down;
        private bool AlignActionsHaveRun = false;
        public AIType AIType = AIType.Player;
        private Direction LastDirection = Direction.Down; // Prevent pacing
        public List<Artifact> Artifacts = new List<Artifact>();

        public string SpriteLeft;
        public string SpriteUp;
        public string SpriteDown;
        public string SpriteRight;
        public string SpriteStun;

        public float AiNextTurn = 1000;
        public float Stunned = 0;

        private new World World;
        public Guy(World w, string l, string r, string u, string d, string stun) : base(w)
        {
            LastAlignTilePos = Pos;
            World = w;

            SpriteLeft = l;
            SpriteRight = r;
            SpriteUp = u;
            SpriteDown = d;
            SpriteStun = stun;

            Sprite = World.Res.Tiles.GetSprite(d);
        }

        public bool KeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public override void Update(Input inp, float dt, Action physics = null)
        {
            base.Update(inp, dt, physics);
            World world = (World as World);

            if (World.GameState != GameState.GameOver)
            {
                if (Stunned <= 0)
                {
                    if (AIType == AIType.Player)
                    {
                        if (KeyDown(Keys.Left) || KeyDown(Keys.A))
                        {
                            NextDirection = Direction.Left;
                        }
                        if (KeyDown(Keys.Right) || KeyDown(Keys.D))
                        {
                            NextDirection = Direction.Right;
                        }
                        if (KeyDown(Keys.Down) || KeyDown(Keys.S))
                        {
                            NextDirection = Direction.Down;
                        }
                        if (KeyDown(Keys.Up) || KeyDown(Keys.W))
                        {
                            NextDirection = Direction.Up;
                        }
                    }
                    else
                    {

                        AiNextTurn -= dt;
                        if (AiNextTurn <= 0)
                        {
                            AiNextTurn = Globals.RandomInt(2000, 4000);

                            PickNewDirection();
                        }
                    }

                    //We are aligned with tile.
                    if (CheckAlignedTile())
                    {
                        //Change to user direct
                        ChangeDirection(dt);

                        if (AlignActionsHaveRun == false)
                        {
                            //Set the last aligned tile pos - this is an escape check
                            LastAlignTilePos = (World as World).RoundPosToTilePos(Pos);

                            if (AIType == AIType.Player)
                            {
                                CollidePiles();
                            }

                            AlignActionsHaveRun = true;
                        }

                        if (DoesNeighborPosHaveTile(FacingDirection))
                        {
                            //We have a ground tile in the direction we're going, so just keep walking.
                        }
                        else
                        {
                            if (AIType != AIType.Player)
                            {
                                //AI hit a wall
                                if (AIType == AIType.Wandering)
                                {
                                    PickNewDirection();

                                }
                                else if (AIType == AIType.BackNForth)
                                {
                                    //Lollygag back n forth
                                    if (FacingDirection == Direction.Left) { NextDirection = Direction.Right; }
                                    if (FacingDirection == Direction.Right) { NextDirection = Direction.Left; }
                                    if (FacingDirection == Direction.Up) { NextDirection = Direction.Down; }
                                    if (FacingDirection == Direction.Down) { NextDirection = Direction.Up; }
                                }

                                ChangeDirection(dt);
                            }
                            else
                            {

                                //Player: player hit a wall
                                Animate = false;
                                Frame = this.Sprite.Frames[1];

                            }

                            //Very important to realign to the tile

                            Vel = new vec2(0, 0);
                            Pos = LastAlignTilePos * (World as World).Res.Tiles.TileWidthPixels;
                        }

                    }
                    else
                    {
                        AlignActionsHaveRun = false;
                    }


                    Pos += Vel;


                }
                else
                {
                    Stunned -= dt;
                    if (Stunned <= 0)
                    {
                        Stunned = 0;

                        SetSpriteForDir(dt, NextDirection);
                    }
                    else
                    {
                        Sprite sp = this.World.Res.Tiles.GetSprite(this.SpriteStun);
                        if (Sprite != sp)
                        {
                            Animate = true;
                            Sprite = sp;
                        }
                    }

                }

            }
        }
        private void PickNewDirection()
        {
            //Go in random alternative direction
            List<Direction> dirs = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

            //Remove lastDirection to prevent annoying "Pacing"
            //**This doesn't work
            dirs.RemoveAll(x => x.Equals(LastDirection));


            bool bSet = false;
            dirs = Permute(dirs, 10);
            for (int i = 0; i < dirs.Count; ++i)
            {
                if (DoesNeighborPosHaveTile(dirs[i]))
                {
                    bSet = true;
                    NextDirection = dirs[i];
                    break;
                }
            }

            if (bSet == false)
            {
                //We removed lastdirection to avoid pacing, however it's the only available path.
                NextDirection = LastDirection;
            }
        }
        private void ChangeDirection(float dt)
        {
            if (DoesNeighborPosHaveTile(NextDirection))
            {
                SetSpriteForDir(dt, NextDirection);
            }

        }
        private void SetSpriteForDir(float dt, Direction d)
        {
            if (d == Direction.Left)
            {
                LastDirection = FacingDirection;
                FacingDirection = Direction.Left;
                Animate = true;
                Vel = new vec2(-speed, 0) * dt;
                Sprite = World.Res.Tiles.GetSprite(SpriteLeft);
            }
            else if (d == Direction.Right)
            {
                LastDirection = FacingDirection;
                FacingDirection = Direction.Right;
                Animate = true;
                Vel = new vec2(speed, 0) * dt;
                Sprite = World.Res.Tiles.GetSprite(SpriteRight);
            }
            else if (d == Direction.Up)
            {
                LastDirection = FacingDirection;
                FacingDirection = Direction.Up;
                Animate = true;
                Vel = new vec2(0, -speed) * dt;
                Sprite = World.Res.Tiles.GetSprite(SpriteUp);
            }
            else if (d == Direction.Down)
            {
                LastDirection = FacingDirection;
                FacingDirection = Direction.Down;
                Animate = true;
                Vel = new vec2(0, speed) * dt;
                Sprite = World.Res.Tiles.GetSprite(SpriteDown);
            }
        }
        private List<T> Permute<T>(List<T> stuff, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                stuff = Permute(stuff);
            }
            return stuff;
        }
        private List<T> Permute<T>(List<T> stuff)
        {
            if (stuff.Count <= 2)
            {
                return stuff;
            }

            int a = Globals.RandomInt(0, stuff.Count);
            int b = a;
            while (b == a)
            {
                b = Globals.RandomInt(0, stuff.Count);
            }

            T tmp = stuff[a];
            stuff[a] = stuff[b];
            stuff[b] = tmp;

            return stuff;
        }
        private void CollidePiles()
        {

            DigPile pileUnderChar = null;
            foreach (DigPile dp in World.DigPiles)
            {
                if (World.TilesAreAtSamePos(Pos, dp.Pos))
                {
                    pileUnderChar = dp;
                    break;
                }
            }
            if (pileUnderChar != null)
            {
                World.DigPiles.Remove(pileUnderChar);
                World.Res.Audio.PlaySound(World.Res.SfxUncoverItem);
                float pi = (float)Math.PI;

                if (pileUnderChar.Artifact == null)
                {
                    World.Score += 10;
                }
                else
                {
                    World.Score += 100;
                }
                //Create smoke Particles
                Frame f = World.Res.Tiles.GetSpriteFrame(World.Res.SprDustParticle, 0);
                World.CreateParticles(new ParticleParams(
                    f,
                    new MpInt(3, 5),
                    new MpVec2(Pos, Pos),
                    new MpVec2(new vec2(-.2f, -.2f), new vec2(.2f, .2f)),
                    new MpFloat(0, pi * 2),
                    new MpFloat(-pi * 1.6f, pi * 1.6f),
                    new MpFloat(0.5f, 0.9f),
                    new MpFloat(-2.9f, -0.8f),
                    new MpFloat(1, 1),
                    new MpFloat(0.58f, 1.77f),
                    new List<Color>() { Color.White }
                    ));

                if (pileUnderChar.Artifact != null)
                {
                    World.FindArtifact(pileUnderChar.Artifact);
                    if(pileUnderChar.Artifact.ArtifactType != ArtifactType.Pickaxe)
                    {
                        Artifacts.Add(pileUnderChar.Artifact);
                    }
                }
            }

        }
        private bool CheckAlignedTile()
        {
            float fuzzyRange = 0.1f;
            float a = Math.Abs(Pos.x % (World as World).Res.Tiles.TileWidthPixels);
            float b = Math.Abs(Pos.y % (World as World).Res.Tiles.TileWidthPixels);

            if (a < fuzzyRange && b < fuzzyRange)
            {
                //Aligned = true;//Boolean prevents multiple aligns
                return true;
            }
            else
            {
                float tw = (World as World).Res.Tiles.TileWidthPixels;
                if (FacingDirection == Direction.Left || FacingDirection == Direction.Right)
                {
                    if (Math.Abs(Pos.x - LastAlignTilePos.x * 12) > tw)
                    {
                        return true;
                    }
                }
                else if (FacingDirection == Direction.Up || FacingDirection == Direction.Down)
                {
                    if (Math.Abs(Pos.y - LastAlignTilePos.y * 12) > tw)
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        private bool DoesNeighborPosHaveTile(Direction direction)
        {
            vec2 checkpos = Pos;
            float tw = (World as World).Res.Tiles.TileWidthPixels;
            checkpos += new vec2(tw, tw) * 0.5f;

            if (direction == Direction.Left)
            {
                checkpos = new vec2(checkpos.x - tw, checkpos.y);
            }
            else if (direction == Direction.Right)
            {
                checkpos = new vec2(checkpos.x + tw, checkpos.y);
            }
            else if (direction == Direction.Up)
            {
                checkpos = new vec2(checkpos.x, checkpos.y - tw);
            }
            else if (direction == Direction.Down)
            {
                checkpos = new vec2(checkpos.x, checkpos.y + tw);
            }

            vec2 r2ToTile = (World as World).R2PosToTilePos(checkpos);
            if ((World as World).GetTileAtPoint(r2ToTile) == null)
            {
                return false;
            }

            return true;
        }

        public static vec2 RoundPosToTile(vec2 pos)
        {
            int x = ((int)Math.Round(pos.x / 12.0)) * 12;
            int y = ((int)Math.Round(pos.y / 12.0)) * 12;

            return new vec2(x, y);
        }

    }

}
