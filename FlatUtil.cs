using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Flat.Graphics;

using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Flat
{
    public static class FlatUtil
    {
        /// <summary>
        /// 1/10th of a millimeter.
        /// </summary>
        public static readonly float VerySmallAmount = 0.0001f;

        public static float Clamp(float value, float min, float max)
        {
            if(min == max)
            {
                return min;
            }

            if(min > max)
            {
                FlatUtil.Swap(ref min, ref max);
            }

            if(value < min)
            {
                return min;
            }

            if(value > max)
            {
                return max;
            }

            return value;
        }

        public static void Swap<T>(ref T a, ref T b) where T : struct
        {
            T t = a;
            a = b;
            b = t;
        }

        public static void ToggleFullScreen(GraphicsDeviceManager graphics)
        {
            if(graphics is null)
            {
                throw new ArgumentNullException("graphics");
            }

            graphics.HardwareModeSwitch = false;
            graphics.ToggleFullScreen();
        }

        public static void SetRelativeBackBufferSize(GraphicsDeviceManager graphics, float ratio)
        {
            ratio = FlatMath.Clamp(ratio, 0.25f, 1f);

            DisplayMode dm = graphics.GraphicsDevice.DisplayMode;

            graphics.PreferredBackBufferWidth = (int)MathF.Round((float)dm.Width * ratio);
            graphics.PreferredBackBufferHeight = (int)MathF.Round((float)dm.Height * ratio);
            graphics.ApplyChanges();
        }

        public static string GetGraphicsDeviceName(GraphicsDeviceManager graphics)
        {
            return graphics.GraphicsDevice.Adapter.Description;
        }

        public static void GetCurrentDisplaySize(GraphicsDeviceManager graphics, out int width, out int height)
        {
            DisplayMode dm = graphics.GraphicsDevice.Adapter.CurrentDisplayMode;
            width = dm.Width;
            height = dm.Height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetElapsedTimeInSeconds(GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static float GetElapsedTimeInSeconds(GameTime gameTime, int interval)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds / (float)interval;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ConvertMilerPerHourToMetersPerSecond(float mph)
        { 
            return mph * 0.44704f;
        }

        //public static Vector2 Transform(Vector2 value, in Transform transform)
        //{
        //    float sx = value.X * transform.ScaleX;
        //    float sy = value.Y * transform.ScaleY;

        //    /*   
        //     *   2D rotations:
        //     *   -------------
        //     *   x′=xcosθ−ysinθ 
        //     *   y′=xsinθ+ycosθ
        //     */

        //    float rx = sx * transform.Cos - sy * transform.Sin;
        //    float ry = sx * transform.Sin + sy * transform.Cos;

        //    float tx = rx + transform.PositionX;
        //    float ty = ry + transform.PositionY;

        //    return new Vector2(tx, ty);
        //}

        public static Vector2 Transform(Vector2 value, FlatTransform transform)
        {
            return new Vector2(
                value.X * transform.CosScaleX - value.Y * transform.SinScaleY + transform.PositionX, 
                value.X * transform.SinScaleX + value.Y * transform.CosScaleY + transform.PositionY);
        }

        public static Vector2 Transform(float x, float y, FlatTransform transform)
        {
            return new Vector2(
                x * transform.CosScaleX - y * transform.SinScaleY + transform.PositionX,
                x * transform.SinScaleX + y * transform.CosScaleY + transform.PositionY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Transform(float x, float y, float cosScaleX, float sinScaleX, float sinScaleY, float cosScaleY, float positionX, float positionY)
        {
            return new Vector2(
                x * cosScaleX - y * sinScaleY + positionX,
                x * sinScaleX + y * cosScaleY + positionY);
        }

        public static Vector2 ConvertAngleToDirection(float angle)
        {
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        public static bool IntersectBoxes(FlatBox a, FlatBox b)
        {
            if( a.Max.X <= b.Min.X || 
                b.Max.X <= a.Min.X ||
                a.Max.Y <= b.Min.Y || 
                b.Max.Y <= a.Min.Y)
            {
                return false;
            }

            return true;
        }

        public static bool IntersectCircles(in FlatCircle a, in FlatCircle b)
        {
            float dx = a.Center.X - b.Center.X;
            float dy = a.Center.Y - b.Center.Y;
            float dist = MathF.Sqrt(dx * dx + dy * dy);

            bool result = dist < a.Radius + b.Radius;
            return result;
        }

        public static bool IntersectBoxes(FlatBox a, FlatBox b, out Vector2 mtv)
        {
            mtv = Vector2.Zero;

            if(!FlatUtil.IntersectBoxes(a, b))
            {
                return false;
            }

            float dx1 = a.Min.X - b.Max.X;
            float dx2 = a.Max.X - b.Min.X;

            float dy1 = a.Min.Y - b.Max.Y;
            float dy2 = a.Max.Y - b.Min.Y;

            float dx;
            float dy;

            if (MathF.Abs(dx1) <= MathF.Abs(dx2))
            {
                dx = dx1;
            }
            else
            {
                dx = dx2;
            }

            if (MathF.Abs(dy1) <= MathF.Abs(dy2))
            {
                dy = dy1;
            }
            else
            {
                dy = dy2;
            }

            if(MathF.Abs(dx) <= MathF.Abs(dy))
            {
                mtv = new Vector2(dx, 0f);
            }
            else
            {
                mtv = new Vector2(0f, dy);
            }

            return true;
        }

        public static bool IntersectSegments(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 ip)
        {
            ip = Vector2.Zero;

            if(!FlatUtil.IntersectSegmentsFast(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y))
            {
                return false;
            }

            float dx1 = b.X - a.X;
            float dy1 = b.Y - a.Y;
            float dx2 = d.X - c.X;
            float dy2 = d.Y - c.Y;

            float x, y, m1, m2;

            if (dx1 == 0f)   // Line: a-b is vertical
            {
                m2 = dy2 / dx2;
                x = a.X;
                y = m2 * (x - c.X) + c.Y;
            }
            else if (dx2 == 0f)  // line: c-d is vertical
            {
                m1 = dy1 / dx1;
                x = c.X;
                y = m1 * (x - a.X) + a.Y;
            }
            else
            {
                m1 = dy1 / dx1;
                m2 = dy2 / dx2;

                x = m1 * a.X - m2 * c.X - a.Y + c.Y;
                x /= m1 - m2;

                y = m1 * (x - a.X) + a.Y;
            }

            ip = new Vector2(x, y);            
            return true;
        }

        public static bool IntersectSegments(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Vector2 ab = b - a;
            Vector2 ac = c - a;
            Vector2 ad = d - a;

            Vector2 cd = d - c;
            Vector2 ca = a - c;
            Vector2 cb = b - c;

            float c1 = FlatMath.Cross(ab, ac);
            float c2 = FlatMath.Cross(ab, ad);

            float c3 = FlatMath.Cross(cd, ca);
            float c4 = FlatMath.Cross(cd, cb);

            // Same sign = no intersection.
            // Result of Zero indicates that the intersection happens at an end point of one of the segments. Consider this an intersection.
            if(c1 * c2 > 0f || c3 * c4 > 0f)
            {
                return false;
            }

            return true;
        }

        public static bool IntersectSegmentsFast(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy)
        {
            // Assigns: 16
            // adds/subs: 16
            // muls: 10
            // divs: 0
            // comps: 2

            float abx = bx - ax;
            float aby = by - ay;

            float acx = cx - ax;
            float acy = cy - ay;

            float adx = dx - ax;
            float ady = dy - ay;

            float cdx = dx - cx;
            float cdy = dy - cy;

            float cax = ax - cx;
            float cay = ay - cy;

            float cbx = bx - cx;
            float cby = by - cy;

            float c1 = abx * acy - aby * acx;
            float c2 = abx * ady - aby * adx;

            float c3 = cdx * cay - cdy * cax;
            float c4 = cdx * cby - cdy * cbx;

            // Same sign = no intersection.
            // Result of Zero indicates that the intersection happens at an 
            //  end point of one of the segments. Consider this an intersection.
            if (c1 * c2 > 0f || c3 * c4 > 0f)
            {
                return false;
            }

            return true;
        }

        public static bool Intersect(in FlatCircle circle, in FlatBox box)
        {
            float cx = FlatMath.Clamp(circle.Center.X, box.Min.X, box.Max.X);
            float cy = FlatMath.Clamp(circle.Center.Y, box.Min.Y, box.Max.Y);

            Vector2 cp = new Vector2(cx, cy);

            return FlatMath.Distance(circle.Center, cp) < circle.Radius;
        }

        public static bool IntersectCirclesFast(float ax, float ay, float ar, float bx, float by, float br, out float depth, out Vector2 normal)
        {
            depth = 0f;
            normal = Vector2.Zero;

            float nx = bx - ax;
            float ny = by - ay;

            float nLenSq = nx * nx + ny * ny;

            float r2 = ar + br;
            float r2Sq = r2 * r2;

            if (nLenSq >= r2Sq)
            {
                return false;
            }

            // Actual distance between a and b.
            float d = MathF.Sqrt(nLenSq);

            if (d != 0f)
            {
                float invd = 1f / d;
                depth = r2 - d;
                normal = new Vector2(nx * invd, ny * invd);
            }
            // If d == 0f, then the circles are directly on top of eachother.
            //  Choose random but consistent normal and depth.
            else
            {
                depth = ar;
                normal = new Vector2(1f, 0f);
            }

            return true;
        }

        public static bool IntersectCircles(FlatCircle a, FlatCircle b, out float depth, out Vector2 normal)
        {
            depth = 0f;
            normal = Vector2.Zero;

            // Vector from a to b.
            Vector2 n = b.Center - a.Center;

            // Get the length from a to b squared.
            float nLenSq = n.LengthSquared();

            // Added radius squared.
            float r2 = a.Radius + b.Radius;
            float r2Sq = r2 * r2;

            if(nLenSq >= r2Sq)
            {
                return false;
            }

            // Actual distance between a and b.
            float d = MathF.Sqrt(nLenSq);

            if(d != 0f)
            {
                depth = r2 - d;
                normal = n / d;
            }
            // If d == 0f, then the circles are directly on top of eachother.
            //  Choose random but consistent normal and depth.
            else
            {
                depth = a.Radius;
                normal = new Vector2(1f, 0f);
            }

            return true;
        }

        //public static bool IntersectPolygonCircle(PolygonShape polygon, FlatCircle circle, out float depth, out Vector2 normal)
        //{
        //    throw new NotImplementedException();
        //}

        public static bool IntersectPointCircle(Vector2 p, Vector2 circleCenter, float circleRadius)
        {
            return Vector2.DistanceSquared(p, circleCenter) <= circleRadius * circleRadius;
        }

        public static Vector2 Round(Vector2 value, int digits)
        {
            value.X = MathF.Round(value.X, digits);
            value.Y = MathF.Round(value.Y, digits);
            return value;
        }

        public static T GetItem<T>(T[] array, int index)
        {
            if (index >= array.Length)
            {
                return array[index % array.Length];
            }
            else if (index < 0)
            {
                return array[index % array.Length + array.Length];
            }
            else
            {
                return array[index];
            }
        }

        public static T GetItem<T>(List<T> list, int index)
        {
            if (index >= list.Count)
            {
                return list[index % list.Count];
            }
            if (index < 0)
            {
                return list[index % list.Count + list.Count];
            }
            else
            {
                return list[index];
            }
        }

        public static void CreateIndexList<T>(T[] array, out List<int> indices)
        {
            indices = new List<int>(array.Length);

            for(int i = 0; i < array.Length; i++)
            {
                indices.Add(i);
            }
        }

        //public static void CreateBoundingBox(PolygonShape polygon, out FlatBox box)
        //{
        //    Vector2[] vertices = polygon.Vertices;
        //    int[] hull = polygon.ConvexHull;

        //    float xmin = float.MaxValue;
        //    float xmax = float.MinValue;
        //    float ymin = float.MaxValue;
        //    float ymax = float.MinValue;

        //    for (int i = 0; i < hull.Length; i++)
        //    {
        //        Vector2 v = vertices[hull[i]];

        //        if (v.X < xmin) { xmin = v.X; }
        //        if (v.X > xmax) { xmax = v.X; }
        //        if (v.Y < ymin) { ymin = v.Y; }
        //        if (v.Y > ymax) { ymax = v.Y; }
        //    }

        //    box = new FlatBox(new Vector2(xmin, ymin), new Vector2(xmax, ymax));
        //}

        //public static void CreateBoundingBox(PolygonShape polygon, FlatTransform transform, out FlatBox box)
        //{
        //    Vector2[] vertices = polygon.Vertices;
        //    int[] hull = polygon.ConvexHull;

        //    float xmin = float.MaxValue;
        //    float xmax = float.MinValue;
        //    float ymin = float.MaxValue;
        //    float ymax = float.MinValue;

        //    for (int i = 0; i < hull.Length; i++)
        //    {
        //        Vector2 v = vertices[hull[i]];
        //        v = FlatUtil.Transform(v, transform);

        //        if (v.X < xmin) { xmin = v.X; }
        //        if (v.X > xmax) { xmax = v.X; }
        //        if (v.Y < ymin) { ymin = v.Y; }
        //        if (v.Y > ymax) { ymax = v.Y; }
        //    }

        //    box = new FlatBox(xmin, xmax, ymin, ymax);
        //}

        public static void CreateBoundingBox(Vector2[] vertices, int[] hull, out FlatBox box)
        {
            float xmin = float.MaxValue;
            float xmax = float.MinValue;
            float ymin = float.MaxValue;
            float ymax = float.MinValue;

            for (int i = 0; i < hull.Length; i++)
            {
                Vector2 v = vertices[hull[i]];

                if (v.X < xmin) { xmin = v.X; }
                if (v.X > xmax) { xmax = v.X; }
                if (v.Y < ymin) { ymin = v.Y; }
                if (v.Y > ymax) { ymax = v.Y; }
            }

            box = new FlatBox(xmin, xmax, ymin, ymax);
        }

        public static void CreateBoundingBox(Vector2[] vertices, out FlatBox box)
        {
            float xmin = float.MaxValue;
            float xmax = float.MinValue;
            float ymin = float.MaxValue;
            float ymax = float.MinValue;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 v = vertices[i];

                if (v.X < xmin) { xmin = v.X; }
                if (v.X > xmax) { xmax = v.X; }
                if (v.Y < ymin) { ymin = v.Y; }
                if (v.Y > ymax) { ymax = v.Y; }
            }

            box = new FlatBox(xmin, xmax, ymin, ymax);
        }

        public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            // TODO: Inline each of these lines for performance?
            Vector2 ab = b - a;
            Vector2 bc = c - b;
            Vector2 ca = a - c;

            Vector2 ap = p - a;
            Vector2 bp = p - b;
            Vector2 cp = p - c;

            float c1 = FlatMath.Cross(ab, ap);
            float c2 = FlatMath.Cross(bc, bp);
            float c3 = FlatMath.Cross(ca, cp);

            if(c1 < 0f && c2 < 0f && c3 < 0f)
            {
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ConvertRadiansToDegrees(float radians)
        {
            return radians * 180f / MathF.PI;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ConvertDegreesToRadians(float degrees)
        {
            return degrees * MathF.PI / 180f;
        }

        public static float PercievedBrightness(float r, float g, float b)
        {
            // https://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx

            return MathF.Sqrt((r * r * 0.241f) + (g * g * 0.691f) + (b * b * 0.068f));
        }

        public static float PercievedBrightness(Color color)
        {
            // https://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx

            int r = color.R;
            int g = color.G;
            int b = color.B;

            float value = MathF.Sqrt((r * r * 0.241f) + (g * g * 0.691f) + (b * b * 0.068f));
            return value / 255f;
        }

        public static bool IsNearlyEqual(float a, float b)
        {
            return MathF.Abs(a - b) <= FlatUtil.VerySmallAmount;
        }

        public static bool IsNearlyEqual(Vector2 a, Vector2 b)
        {
            return
                (MathF.Abs(a.X - b.X) <= FlatUtil.VerySmallAmount) && 
                (MathF.Abs(a.Y - b.Y) <= FlatUtil.VerySmallAmount);
        }
    }
}
