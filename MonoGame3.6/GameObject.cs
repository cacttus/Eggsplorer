using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class CollisionFlags
    {
        public static int CollideTop = 0x01;
        public static int CollideBot = 0x02;
        public static int CollidePixel = 0x04;
    }
    public class GameObject : Touchable
    {
        public List<Rectangle> HitBoxes = new List<Rectangle>();

        public float Speed { get; set; } = 1;
        public float Power { get; set; } = 0;
        public float Health { get; set; } = 5;
        public int Value { get; set; } = 1;//Price
        public int CollisionFlags { get; set; } = 0;// CollisionFlags.CollideTop;

        public bool Blocked = false;//if we can move..
        public vec2 Vel;
        public vec2 Acc;
        public Sprite Sprite { get; set; }
        public Frame Frame { get; set; }
        public vec2 Pos { get; set; }
        public vec2 Size = new vec2(1, 1);
        public float frame = 0;
        public bool Animate { get; set; } = false;
        public bool Visible { get; set; } = true;
        public bool Loop { get; set; } = true;//loop aniamtino
        public TouchState TouchState { get; set; } = TouchState.Up;
        public float DeltaScale { get; set; } = 0.0f;
        public float Scale { get; set; } = 1.0f;
        public float DeltaFade { get; set; } = 0.0f;//Fade animation
        public float Fade { get; set; } = 1.0f;//Addition of fade
        public Color Color { get; set; } = Color.White;
        public float RotationDelta { get; set; } = 0.0f;
        public float Rotation { get; set; } = 0.0f;
        public vec2 Origin { get; set; } = new vec2(0, 0);
        public Rectangle Bounds = new Rectangle(0, 0, 0, 0);

        public vec2 GetCenter() { return new vec2(6, 6) + Pos; }
        public WorldBase World { get; private set; }

        private float _fNextFrame = 0;

        public bool AnimationEnded()
        {
            if (Sprite != null)
            {
                if(Sprite.Frames!=null && Sprite.Frames.Count > 0)
                {
                    if (Frame == Sprite.Frames[Sprite.Frames.Count - 1])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CollidesWidth_Inclusive(vec2 point)
        {
            //Grid REct
            float l = Pos.x; ;// * World.Tiles.SpriteWidth;
            float t = Pos.y; ;// * World.Tiles.SpriteWidth;
            float r = l + 12;
            float b = t + 12;
            if (point.x >= l && point.x <= r && point.y >= t && point.y <= b)
            {
                return true;
            }
            return false;
        }
        public bool CollidesWith(GameObject g)
        {
            float pad = 4.0f;
            //Grid REct
            float l = Pos.x + pad ;// * World.Tiles.SpriteWidth;
            float t = Pos.y + pad ;// * World.Tiles.SpriteWidth;
            float r = l + (World as World).Res.Tiles.TileWidthPixels - pad;
            float b = t + (World as World).Res.Tiles.TileWidthPixels - pad;

            float gl = g.Pos.x + pad;
            float gt = g.Pos.y + pad;// World.Tiles.SpriteWidth;
            float gr = gl + (World as World).Res.Tiles.TileWidthPixels - pad;
            float gb = gt + (World as World).Res.Tiles.TileWidthPixels - pad;

            if (gr < l) { return false; }
            else if (gb < t) { return false; }
            else if (gl > r) { return false; }
            else if (gt > b) { return false; }
            else
            {
                return true;
            }
        }
        public bool OutsideWindow()
        {
            float w = (float)World.Screen.Game.GraphicsDevice.Viewport.Width;
            float h = (float)World.Screen.Game.GraphicsDevice.Viewport.Height;

            float tile_w = w / World.Screen.Viewport.TilesWidth;
            //R2 Rect
            float l = Pos.x * (float)tile_w;
            float t = Pos.y * (float)tile_w;
            float r = l + (float)tile_w;
            float b = t + (float)tile_w;

            if (r < 0) { return true; }
            if (b < 0) { return true; }
            if (l > w) { return true; }
            if (t > h) { return true; }

            return false;
        }
        public GameObject(WorldBase w)
        {
            World = w;

        }
        public GameObject(WorldBase w, string SpriteName)
        {
            World = w;
            Sprite = World.Screen.Game.Res.Tiles.GetSprite(SpriteName);
        }

        public override Rectangle GetDest()
        {
            return World.Screen.Viewport.WorldToDevice(Pos, Size);
        }
        public virtual void Update(Input inp, float dt, Action physics = null)
        {
            base.Update(inp);

            if (Animate == true && Frame == null && Sprite != null)
            {
                Frame = Sprite.Frames[0];
            }
            if (physics != null)
            {
                physics();
            }
            else
            {
                //Don't make automatic physics.
                //if (Blocked == false)
                //{
                //    Vel += Acc;
                //    Vel += World.Gravity;
                //    Pos += Vel /*Dir * Speed*/ * dt;
                //}
            }

            //Bounds.X = (int)Pos.X;
            //Bounds.Y = (int)Pos.Y;


            Fade -= DeltaFade * dt;
            if (Fade < 0)
            {
                Fade = 0;
            }

            Scale -= DeltaScale * dt;
            if (Scale < 0)
            {
                Scale = 0;
            }

            Rotation += RotationDelta * dt;
            Rotation = Rotation % (float)(Math.PI * 2.0f);

            if (Animate == true)
            {

                _fNextFrame -= dt;

                int i = 0;
                while (_fNextFrame <= 0 && i < 500)
                {
                    if (Sprite != null && Frame != null)
                    {
                        if (Sprite.Frames.Count > 0)
                        {
                            if ((Frame.Index == Sprite.Frames.Count - 1) && Loop == false)
                            {
                                //, No loop, Don't change frame
                            }
                            else if (Frame == null)
                            {
                                Frame = Sprite.Frames[0];
                                _fNextFrame = 0;
                            }
                            else
                            {
                                int frame = (Frame.Index + 1) % Sprite.Frames.Count;
                                Frame = Sprite.Frames[frame];
                                _fNextFrame += Frame.Delay;
                            }

                        }

                    }
                    i++;
                }
            }


        }
        //public void Draw(SpriteBatch sb)
        //{

        //}
    }
}
