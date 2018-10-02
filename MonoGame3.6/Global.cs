using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;

namespace Core
{
    public enum Platform { Android, Desktop, WindowsPhone, iOS }
    public enum TouchState { Up, Press, Down, Release }
    public enum MenuState { Closed, PickObject, PlaceObject, PickWand, GameOver }
    public enum WorldState { Play, Pause, GameOver }
    public enum ShowScreen { None, Game, Battle, Title, Tutorial }

    public class MpInt
    {
        private int? _value = null;
        public int NewValue() {  return Globals.RandomInt(Min, Max);  }
        public int Value
        {
            get
            {
                if (_value == null)
                {
                    _value = NewValue();
                }
                return _value.Value;
            }
        }
        public int Min;
        public int Max;
        public MpInt(int min, int max) { Min = min; Max = max; }
    }
    public class MpVec2
    {
        private vec2? _value = null;
        public vec2 NewValue() {  return Globals.RandomVec2(Min, Max); }
        public vec2 Value
        {
            get
            {
                if (_value == null)
                {
                    _value = NewValue();
                }
                return _value.Value;
            }
        }
        public vec2 Min;
        public vec2 Max;
        public MpVec2(vec2 min, vec2 max) { Min = min; Max = max; }
    }
    public class MpFloat
    {
        private float? _value = null;
        public float NewValue() { return Globals.Random(Min, Max); }
        public float Value
        {
            get
            {
                if (_value == null)
                {
                    _value = NewValue();
                }
                return _value.Value;
            }
        }
        public float Min;
        public float Max;
        public MpFloat(float min, float max) { Min = min; Max = max; }
    }



    public class Plane2f
    {
        public Plane2f() { }
        public float D;
        public vec2 N;
        public Plane2f(vec2 n, vec2 pt)
        {
            D = -n.Dot(pt);
            N = n;
        }
        public float IntersectLine(vec2 p1, vec2 p2)
        {
            float t = -(N.Dot(p1) + D) / ((p2 - p1).Dot(N));
            return t;
        }
    }
    public class ivec2
    {
        public ivec2() { }
        public ivec2(int dx, int dy) { x = dx; y = dy; }
        public int x { get; set; }
        public int y { get; set; }
    }
    public class GfxRect
    {
        public Rectangle Rect;
        public Color Color;
        public GfxRect(GfxRect rc) { Rect = new Rectangle(rc.Rect.X, rc.Rect.Y, rc.Rect.Width, rc.Rect.Height); Color = new Color(rc.Color.R, rc.Color.G, rc.Color.B, rc.Color.A); }
        public GfxRect() { }
        public GfxRect(Rectangle r, Color c) { Rect = r; Color = c; }
    }
    public static class Globals
    {
        public static vec2 RandomDirection()
        {
            vec2 ret = new vec2();
            ret.x = (float)Globals.Rng.NextDouble() - 0.5f;
            ret.y = (float)Globals.Rng.NextDouble() - 0.5f;
            ret.Normalize();
            return ret;
        }
        public static bool FuzzyEquals(float a, float b, float e = 0.001f)
        {
            return (((a - e) <= b) && ((a + e) >= b));
        }
        public static Random Rng { get; set; } = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static bool Pick(Rectangle r, vec2 pos)
        {
            if (pos.x < r.X) { return false; }
            if (pos.y < r.Y) { return false; }
            if (pos.x > (r.X + r.Width)) { return false; }
            if (pos.y > (r.Y + r.Height)) { return false; }
            return true;
        }
        public static float Random(float min, float max)
        {
            return min + ((float)Rng.NextDouble() * (max - min));
        }
        public static int RandomInt(int min, int max)
        {
            int denom = max - min;
            if (denom == 0)
            {
                return 0;
            }
            return min + (Rng.Next() % denom);
        }
        public static bool RandomBool()
        {
            return Random(0, 1) >= 0.5f;
        }
        public static vec2 RandomVec2(vec2 a, vec2 b)
        {
            vec2 ret = new vec2();
            ret.x = a.x + (b.x - a.x) * (float)Rng.NextDouble();
            ret.y = a.y + (b.y - a.y) * (float)Rng.NextDouble();
            return ret;
        }
        public enum WorldState { Play, Pause, GameOver }
    }
    public class DelayedAction
    {
        public DelayedAction(Action action, float delay, bool repeat = true)
        {
            TimeRemaining = delay;
            Action = action;
            MaxDelay = delay;
            Delay = delay;
            Repeat = repeat;
        }

        public float MaxDelay { get; set; }
        public Action Action { get; private set; }
        public float Delay { get; private set; }
        public float TimeRemaining { get; set; }
        public bool Stopped = false;

        public bool Repeat = false;
        public bool Update(float deltaTime)
        {
            if (Stopped == false)
            {
                TimeRemaining -= deltaTime;

                if (TimeRemaining <= 0)
                {
                    Action();
                    if (Repeat)
                    {
                        TimeRemaining = MaxDelay;
                    }
                    else
                    {
                        Stopped = true;
                    }
                    return false;
                }
            }

            return true;
        }
    }
    public class ScreenShake
    {
        public vec2 ScreenShakeOffset = new vec2(0, 0);
        public vec2 Vel = new vec2(0, 0);
        float Max = 100;//Pixels
        float CurMax = 0;//Pixels
        float Damp = 0.89f;//The length of shake time
        public void Shake(int amount, float damping = 0.89f)
        {
            //Prevent screen from flying away
            if (CurMax == 0)
            {
                Damp = damping;
                Max = amount;
                CurMax = Max;
                Vel.x = -Max;
            }
        }
        public void Update(float dt)
        {
            if (CurMax < 0.01f)
            {
                CurMax = 0;
            }
            if (CurMax > 0)
            {
                ScreenShakeOffset += Vel;

                if (Globals.FuzzyEquals(Vel.x, CurMax))
                {
                    CurMax = CurMax * Damp;
                    Vel.x = CurMax;
                }

                Vel.x = -Vel.x;
            }
            else
            {
                ScreenShakeOffset.x = 0;
            }
        }
    }

}
