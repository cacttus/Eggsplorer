using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Core
{

    public class Frame
    {
        public Sprite Sprite { get; private set; }
        public Frame(Sprite s)
        {
            Sprite = s;
        }
        public int Index { get; set; } = 0;
        public Rectangle R { get; set; }
        public float Delay { get; set; } = 0;
        public vec2 PixelCenter()
        {
            vec2 v = new vec2(
                0 + (float)R.Width * 0.5f,
                0 + (float)R.Height * 0.5f
                );

            return v;
        }
    }
    public class Sprite
    {
        public Tiles Tiles { get; private set; }
        public string Name { get; set; } = "";
        public List<Frame> Frames { get; set; } = new List<Frame>();
        public Sprite(string name, Tiles tiles) { Name = name; Tiles = tiles; }
    }
    public class Tiles
    {
        public int TileWidthPixels { get; private set; } = 12;
        public int TileHeightPixels { get; private set; } = 12;
        int _iSpacing = 1;
        int _iOffset = 1;
        //public HashSet<string> sprites { get; set; } = new HashSet<string>();


        public Texture2D Texture { get; set; } = null;

        public Dictionary<string,Sprite> Sprites { get; set; } = new Dictionary<string, Sprite>();
        public Sprite GetSprite(string n)
        {
            Sprite s = null;
            Sprites.TryGetValue(n, out s);
            return s;
        }
        public Tiles()
        {

    }
        //  public void Init(int width, int height)
        //  {
        //      TilesWidth = width;
        //      TilesHeight = height;
        //  }
        public Rectangle FrameRect(Rectangle tiles)
        {
            return FrameRect(tiles.X, tiles.Y, tiles.Width, tiles.Height);
        }
        public Rectangle FrameRect(int x, int y, int w, int h)
        {
            return new Rectangle(
                      _iOffset + (int)x * (TileWidthPixels + _iSpacing)
                    , _iOffset + (int)y * (TileHeightPixels + _iSpacing)
                    , (w * TileWidthPixels) + ((_iSpacing) * (w - 1))
                    , (h * TileHeightPixels) + ((_iSpacing) * (h - 1))
                    );
        }
        public Sprite AddSprite(string name, List<Rectangle> frames, float duration_seconds)
        {

            Sprite s = new Sprite(name,this);
            int i = 0;
            foreach (Rectangle iframe in frames)
            {
                Frame f = new Frame(s);
                f.R = FrameRect((int)iframe.X, (int)iframe.Y, iframe.Width, iframe.Height);
                f.Delay = duration_seconds / (float)frames.Count;
                f.Index = i++;
                s.Frames.Add(f);
            }

            Sprites.Add(name, s);

            return s;
        }
        public Frame GetSpriteFrame(string n, int idx)
        {
            Sprite sp = GetSprite(n);
            if (sp != null)
            {
                if (sp.Frames.Count > idx)
                {
                    return sp.Frames[idx];
                }
            }
            return null;
        }
    }

}
