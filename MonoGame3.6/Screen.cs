using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Core
{
    public class Viewport
    {
        public vec2 Pos { get; set; }//Pixels
        public float WidthPixels {
            get {
                return TilesWidth * Screen.Game.Res.Tiles.TileWidthPixels;
            }
        }
        public float HeightPixels {
            get {
                return TilesHeight * Screen.Game.Res.Tiles.TileHeightPixels;
            }
        }
        //public vec2 WH { get; set; }
        //Not XNA viewprot, this is how many "tiles" width 
        public float TilesWidth {
            get
            {
                return (float)Math.Ceiling(TilesHeight * (float)((float)Screen.Game.GraphicsDevice.Viewport.Width / (float)Screen.Game.GraphicsDevice.Viewport.Height));
            }
        } // This is calculated automatically based on idsplay size.
        //Height is always 6 tiles.
        public float TilesHeight { get; set; } = 6;
        public Screen Screen;

        public Viewport(Screen s)
        {
            Screen = s;

            float ratio;
            //ratio = Screen.Game.GraphicsDevice.Viewport.Height / (TilesHeight * Screen.Game.Res.Tiles.TileHeightPixels);

        }
        public vec2 MeasureString(SpriteFont font, string str)
        {
            Vector2 v = font.MeasureString(str);
            float w_ratio_inv = 1.0f / (Screen.Game.GraphicsDevice.Viewport.Width / WidthPixels);
            float h_ratio_inv = 1.0f / (Screen.Game.GraphicsDevice.Viewport.Height / HeightPixels);

            return new vec2(v.X * w_ratio_inv, v.Y * h_ratio_inv);


        }
        public vec2 ScreenPixelsToScreenRaster(vec2 xy)
        {
            float w_ratio = Screen.Game.GraphicsDevice.Viewport.Width / WidthPixels;
            float h_ratio = Screen.Game.GraphicsDevice.Viewport.Height / HeightPixels;

            return new vec2(xy.x * w_ratio, xy.y * h_ratio);
        }
        public Rectangle WorldToDevice(vec2 pos_pixels, vec2 wh_pixels)
        {
            //Converts Pos + wh from WORLD to SCREEN coordinates
            vec2 dp = pos_pixels - Pos;

            float w_ratio = Screen.Game.GraphicsDevice.Viewport.Width / WidthPixels;
            float h_ratio = Screen.Game.GraphicsDevice.Viewport.Height/ HeightPixels;

            Rectangle ret = new Rectangle(
                (int)(Math.Round(dp.x * w_ratio)), 
                (int)(Math.Round(dp.y * h_ratio)),
                (int)(Math.Round((wh_pixels.x) * w_ratio)), 
                (int)(Math.Round((wh_pixels.y) * h_ratio))
                );

            return ret;
        }
    }

    public abstract class Screen
    {
        public List<DelayedAction> Actions { get; private set; } = new List<DelayedAction>();
        public Viewport Viewport { get; private set; }
        public ScreenShake ScreenShake { get; private set; } = new ScreenShake();
        public SpriteBatch SpriteBatch { get; private set; }
        public List<MenuButton> MenuButtons = new List<MenuButton>();
        public GameBase Game { get; private set; } = null;

        public bool DisableMenuInput { get; set; } = false;

        public virtual void Init(GameBase game)
        {
            Game = game;
            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
 
            Viewport = new Viewport(this);
        }
        public void UpdateActions(float dt)
        {

        }
        public virtual void Update(float dt)
        {
            UpdateActions(dt);

            ScreenShake.Update(dt);

            if (DisableMenuInput == false)
            {
                //Update and also calcualte whether the user clicked a menu item.
                Game.Input.UIClicked = false;
                foreach (MenuButton mb in MenuButtons)
                {
                    if (mb.Visible)
                    {
                        bool click = mb.Update(Game.Input);
                        Game.Input.UIClicked = Game.Input.UIClicked || click;
                    }
                }
            }
        }
        public void BeginDraw()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }
        public abstract void Draw();
        public virtual void DrawMenu()
        {
            foreach (MenuButton mb in MenuButtons)
            {
                if (mb.Visible)
                {
                    mb.Draw(SpriteBatch);
                }
            }
        }
        public void EndDraw()
        {

            SpriteBatch.End();
        }
        public void DrawStringFit(SpriteBatch sb, string str, SpriteFont f, vec2 pos, float width)
        {
            //Draws a string at Pos fitted to Width, which is relative to the viewport width in pixesl

            Vector2 siz = f.MeasureString(str);
            //+new vec2(Screen.Viewport.WidthPixels - siz.X, siz.Y)
            //Here we can add adndoird scaling.
            sb.DrawString(
                f,
                str,
                (pos).toXNA(),
                Color.Black
                );
        }
        public void DrawFrame(SpriteBatch sb, Frame f, vec2 pos, vec2 wh, Color color, 
            float a = 1.0f, float scale = 1.0f, float rotation = 0.0f, vec2 origin = default(vec2))
        {
            DrawFrameRect(sb, f.R, pos, wh, color, a, scale, rotation, origin);
        }
        public void DrawFrameRect(SpriteBatch sb, Rectangle rect_tiles, vec2 pos_pixels, vec2 wh_pixels, Color color,
            float a = 1.0f, float scale = 1.0f, float rotation = 0.0f, vec2 origin = default(vec2))
        {
            //apply scale 
            vec2 tl = (pos_pixels - (pos_pixels + origin)) * scale  + pos_pixels;
            vec2 br = ((wh_pixels + pos_pixels) - (origin + pos_pixels)) * scale + pos_pixels;

            //adjust origin to new scale
            vec2 new_wh = br - tl;

            tl += origin;

            Rectangle r = Viewport.WorldToDevice(tl, new_wh);
            sb.Draw(Game.Res.Tiles.Texture,
                r,
                rect_tiles
                , color * a, rotation, origin.toXNA(), SpriteEffects.None, 0.0f);
        }
        public float ScreenTileWidthMultiplier(float addl = 0)
        {
            float w = Game.GraphicsDevice.Viewport.Width;
            return (float)Math.Round((float)w / ((float)Viewport.TilesWidth - addl));//Important - or else you get a pixel glitch
        }
        public vec2 TouchToGrid(vec2 touch)
        {
            //a / (b / c) = a*(c/b)
            int w = Game.GraphicsDevice.Viewport.Width;
            int h = Game.GraphicsDevice.Viewport.Height;
            float reverse1 = (float)Viewport.TilesWidth / (float)w;
            // float reverse2 = (float)Tiles.TilesHeight / (float)h;
            vec2 ret = new vec2(touch.x * reverse1, touch.y * reverse1);
            return ret;
        }
    }
}
