using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core
{
    //http://xboxforums.create.msdn.com/forums/t/34356.aspx
    public class Ray2D
    {
        private Vector2 startPos;
        private Vector2 endPos;

        public Ray2D(Vector2 startPos, Vector2 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
        }

        /// <summary> 
        /// Determine if the ray intersects the rectangle 
        /// </summary> 
        /// <param name="rectangle">Rectangle to check</param> 
        /// <returns></returns> 
        public Vector2 Intersects(Rectangle rectangle)
        {
            Point p0 = new Point((int)startPos.X, (int)startPos.Y);
            Point p1 = new Point((int)endPos.X, (int)endPos.Y);

            foreach (Point testPoint in BresenhamLine(p0, p1))
            {
                if (rectangle.Contains(testPoint))
                    return new Vector2((float)testPoint.X, (float)testPoint.Y);
            }

            return Vector2.Zero;
        }

        // Swap the values of A and B 

        private void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        // Returns the list of points from p0 to p1  

        private List<Point> BresenhamLine(Point p0, Point p1)
        {
            return BresenhamLine(p0.X, p0.Y, p1.X, p1.Y);
        }

        // Returns the list of points from (x0, y0) to (x1, y1) 

        private List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
        {
            // Optimization: it would be preferable to calculate in 
            // advance the size of "result" and to use a fixed-size array 
            // instead of a list. 

            List<Point> result = new List<Point>();

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int ystep;
            int y = y0;
            if (y0 < y1) ystep = 1; else ystep = -1;
            for (int x = x0; x <= x1; x++)
            {
                if (steep) result.Add(new Point(y, x));
                else result.Add(new Point(x, y));
                error += deltay;
                if (2 * error >= deltax)
                {
                    y += ystep;
                    error -= deltax;
                }
            }

            return result;
        }
        //http://xboxforums.create.msdn.com/forums/t/34356.aspx
    }
    public class MathUtils
    {
        //http://xboxforums.create.msdn.com/forums/t/34356.aspx
        public float? RayIntersectRect(Rectangle rectangle, Ray ray)
        {
            float num = 0f;
            float maxValue = float.MaxValue;
            if (Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if ((ray.Position.X < rectangle.Left) || (ray.Position.X > rectangle.Right))
                {
                    return null;
                }
            }
            else
            {
                float num11 = 1f / ray.Direction.X;
                float num8 = (rectangle.Left - ray.Position.X) * num11;
                float num7 = (rectangle.Right - ray.Position.X) * num11;
                if (num8 > num7)
                {
                    float num14 = num8;
                    num8 = num7;
                    num7 = num14;
                }
                num = MathHelper.Max(num8, num);
                maxValue = MathHelper.Min(num7, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }
            if (Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if ((ray.Position.Y < rectangle.Top) || (ray.Position.Y > rectangle.Bottom))
                {
                    return null;
                }
            }
            else
            {
                float num10 = 1f / ray.Direction.Y;
                float num6 = (rectangle.Top - ray.Position.Y) * num10;
                float num5 = (rectangle.Bottom - ray.Position.Y) * num10;
                if (num6 > num5)
                {
                    float num13 = num6;
                    num6 = num5;
                    num5 = num13;
                }
                num = MathHelper.Max(num6, num);
                maxValue = MathHelper.Min(num5, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }

            return new float?(num);
        }
    }
    public struct vec2
    {
        public float x, y;
        //public vec2() { }
        public vec2(float dx, float dy) { x = dx; y = dy; }
        public vec2(Vector2 v) { x = v.X; y = v.Y; }//From XNA's Vector2
        public float Len() { return (float)Math.Sqrt((x * x) + (y * y)); }
        public void Normalize() {
            float l = Len();
            if (l != 0)
            {
                x /= l;
                y /= l;
            }
            else
            {
                x = 0; y = 0;
            }
            
        }
        public Vector2 toXNA() { return new Vector2(x, y); }
        public static vec2 operator -(vec2 d)
        {
            return new vec2(-d.x, -d.y);
        }
        public static float Dot(vec2 a, vec2 b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }
        public float Dot(vec2 b)
        {
            return (x * b.x) + (y * b.y);
        }
        public static vec2 operator +(vec2 a, vec2 b)
        {
            return new vec2(a.x + b.x, a.y + b.y);
        }
        public static vec2 operator -(vec2 a, vec2 b)
        {
            return new vec2(a.x - b.x, a.y - b.y);
        }
        public static vec2 operator *(vec2 a, float b)
        {
            return new vec2(a.x *b, a.y *b);
        }
        public static vec2 Minv(vec2 a, vec2 b){
            vec2 ret = new vec2();
            ret.x = (float)Math.Min(a.x, b.x);
            ret.y = (float)Math.Min(a.y, b.y);

            return ret;
        }
        public static vec2 Maxv(vec2 a, vec2 b)
        {
            vec2 ret = new vec2();
            ret.x = (float)Math.Max(a.x, b.x);
            ret.y = (float)Math.Max(a.y, b.y);
            return ret;
        }

    }
    public struct Box2f
    {
        public Box2f(float x, float y, float w, float h)
        {
            Min = new vec2(x, y);
            Max = new vec2(w, h) + Min;
        }
        public Box2f(vec2 min, vec2 max) { Min = min; Max = max; }
        public vec2 Min;
        public vec2 Max;
        public void ExpandByPoint(vec2 v)
        {
            Min = vec2.Minv(Min, v);
            Max = vec2.Maxv(Max, v);
        }
        public bool BoxIntersect_EasyOut_Inclusive(Box2f cc)
        {
            return cc.Min.x <= Max.x && cc.Min.y <= Max.y &&  Min.x <= cc.Max.x && Min.y <= cc.Max.y ;
        }
        public bool ContainsPointInclusive(vec2 point)
        {
            if (point.x < Min.x)
                return false;
            if (point.y < Min.y)
                return false;
            if (point.x > Max.x)
                return false;
            if (point.y > Max.y)
                return false;
            return true;
        }
    }
    //    vec2 bounds(int in__)
    //    {
    //        if (in__ == 0) return _min;
    //        return _max;
    //    }

    //    public bool RayIntersect(vec2 ray)
    //    {

    //        float txmin, txmax, tymin, tymax, tzmin, tzmax;
    //        bool bHit;

    //        txmin = (bounds(ray->Sign[0]).x - ray->Origin.x) * ray->InvDir.x;
    //        txmax = (bounds(1 - ray->Sign[0]).x - ray->Origin.x) * ray->InvDir.x;

    //        tymin = (bounds(ray->Sign[1]).y - ray->Origin.y) * ray->InvDir.y;
    //        tymax = (bounds(1 - ray->Sign[1]).y - ray->Origin.y) * ray->InvDir.y;

    //        if ((txmin > tymax) || (tymin > txmax))
    //        {
    //            if (bh != NULL)
    //            {
    //                bh->_bHit = false;
    //            }
    //            return false;
    //        }
    //        if (tymin > txmin)
    //            txmin = tymin;
    //        if (tymax < txmax)
    //            txmax = tymax;

    //        tzmin = (bounds(ray->Sign[2]).z - ray->Origin.z) * ray->InvDir.z;
    //        tzmax = (bounds(1 - ray->Sign[2]).z - ray->Origin.z) * ray->InvDir.z;

    //        if ((txmin > tzmax) || (tzmin > txmax))
    //        {
    //            if (bh != NULL)
    //            {
    //                bh->_bHit = false;
    //            }
    //            return false;
    //        }
    //        if (tzmin > txmin)
    //            txmin = tzmin;
    //        if (tzmax < txmax)
    //            txmax = tzmax;

    //        bHit = ((txmin >= 0.0f) && (txmax <= ray->Length));

    //        //**Note changed 20151105 - this is not [0,1] this is the lenth along the line in which 
    //        // the ray enters and exits the cube, so any value less than the maximum is valid
    //        //bh->_bHit = ( (txmin <= 1.0f) && (txmax >= 0.0f) );
    //        if (bh != NULL)
    //        {
    //            bh->_bHit = bHit;
    //            bh->_t = txmin;
    //        }

    //        return bHit;
    //    }
    //}
    //class ProjectedRay2  { 
    //    vec2 Origin;
    //    vec2 Dir;

    //    // Found the following two cool optimizations on WIlliams et. al (U. Utah)
    //    vec2 InvDir;
    //    int[] Sign = new int[3];

    //    bool _isOpt;    // - return true if  we optimized this

    //    float Length;// Max length

    //    public vec2 getOrigin() const { return Origin; }
    //    public vec2 getBegin() const { return getOrigin(); }
    //    public vec2 getDir() const { return Dir; }
    //    public vec2 getEnd() const { return getOrigin()+getDir(); }
    //  //  public vec2 getHitPoint(PlaneHit& ph) const;



    //    public void opt()
    //    {
    //        //**New - optimization
    //        //http://people.csail.mit.edu/amy/papers/box-jgt.pdf
    //        //Don't set to zero. We need infinity (or large value) here.
    //        //if(Dir.x != 0.0f)    
    //        InvDir.x = 1.0f / Dir.x;
    //        //else
    //        //        InvDir.x = 0.0f;
    //        //    if(Dir.y != 0.0f)    
    //        InvDir.y = 1.0f / Dir.y;
    //        //    else
    //        //        InvDir.y = 0.0f;
    //        //    if(Dir.z != 0.0f)    
    //        //InvDir.z = 1.0f / Dir.z;
    //        //    else
    //        //        InvDir.z = 0.0f;

    //        Sign[0] = InvDir.x < 0;
    //        Sign[1] = InvDir.y < 0;
    //        Sign[2] = InvDir.z < 0;

    //        _isOpt = true;
    //    }

    //    public ProjectedRay2()  {
    //        _isOpt = false; Length = 10000.0f;
    //    }

    //}

//class Ray_t : public ProjectedRay { 
//    float _t;
//vec3 _vNormal;
//public:
//    Ray_t()
//{
//    _t = FLT_MAX;
//    _vNormal.construct(0, 0, 0);
//}
//virtual ~Ray_t() override {
//    }

//    const vec3& getNormal() { return _vNormal; }
//void setNormal(vec3& v) { _vNormal = v; }
//float getTime() const {
//        return _t;
//    }
//    void setTime(float f)
//{
//    _t = f;
//}
//bool isHit()  const {
//        return _t >= 0.0f && _t <= 1.0f;
//    }
//    vec3 getHitPoint() const {
//        vec3 ret = getBegin() + (getEnd() - getBegin()) * getTime();
//        return ret;
//    }

//};

}
