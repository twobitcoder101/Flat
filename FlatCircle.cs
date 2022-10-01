using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flat
{
    public readonly struct FlatCircle
    {
        public readonly Vector2 Center;
        public readonly float Radius;

        public FlatCircle(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public FlatCircle(float x, float y, float radius)
        {
            this.Center = new Vector2(x, y);
            this.Radius = radius;
        }

        public bool Equals(FlatCircle other)
        {
            return this.Center == other.Center && this.Radius == other.Radius;
        }

        public bool Intersects(Vector2 point)
        {
            float distance = FlatMath.Distance(this.Center, point);
            return distance < this.Radius;
        }

        public void GetExtents(out FlatBox box)
        {
            Vector2 min = new Vector2(this.Center.X - this.Radius, this.Center.Y - this.Radius);
            Vector2 max = new Vector2(this.Center.X + this.Radius, this.Center.Y + this.Radius);
            box = new FlatBox(min, max);
        }

        public static bool FindApproximateBoundingCircle(Vector2[] points, out FlatCircle circle)
        {
            circle = default(FlatCircle);
            const int minPoints = 2;

            if (points is null)
            {
                return false;
            }

            if (points.Length < minPoints)
            {
                return false;
            }

            float minx = float.MaxValue;
            float maxx = float.MinValue;
            float miny = float.MaxValue;
            float maxy = float.MinValue;

            for(int i = 0; i < points.Length; i++)
            {
                Vector2 p = points[i];

                if (p.X < minx) { minx = p.X; }
                if (p.X > maxx) { maxx = p.X; }
                if (p.Y < miny) { miny = p.Y; }
                if (p.Y > maxy) { maxy = p.Y; }
            }

            float cx = (maxx + minx) * 0.5f;        // x component of the bounding rectangle center.
            float cy = (maxy + miny) * 0.5f;        // y component of the bounding rectangle center.

            float dx = cx - minx;       // x and y component distance from the bounding rectangle center to a vertex on the bounding rectangle.
            float dy = cy - miny;

            float radius = MathF.Sqrt(dx * dx + dy * dy);   // Radius is the distance from the center to the bounding rectangle vertex.

            circle = new FlatCircle(cx, cy, radius);
            return true;
        }

        private static bool CircleContainsAllPoints(Vector2[] points, int i0, int i1, Vector2 center, float radiusSquared)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (i == i0 || i == i1)
                {
                    continue;
                }

                Vector2 p = points[i];

                float dx = p.X - center.X;
                float dy = p.Y - center.Y;

                float distanceSquared = (dx * dx + dy * dy);

                if (distanceSquared > radiusSquared)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CircleContainsAllPoints(Vector2[] points, int i0, int i1, float cx, float cy, float radiusSquared)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (i == i0 || i == i1)
                {
                    continue;
                }

                Vector2 p = points[i];

                float dx = p.X - cx;
                float dy = p.Y - cy;

                float distanceSquared = (dx * dx + dy * dy);

                if (distanceSquared > radiusSquared)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CircleContainsAllPoints(Vector2[] points, int i0, int i1, int i2, Vector2 center, float radiusSquared)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (i == i0 || i == i1 || i == i2)
                {
                    continue;
                }

                Vector2 p = points[i];

                float dx = p.X - center.X;
                float dy = p.Y - center.Y;

                float distanceSquared = (dx * dx + dy * dy);

                if (distanceSquared > radiusSquared)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CircleContainsAllPoints(Vector2[] points, int i0, int i1, int i2, float cx, float cy, float radiusSquared)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (i == i0 || i == i1 || i == i2)
                {
                    continue;
                }

                Vector2 p = points[i];

                float dx = p.X - cx;
                float dy = p.Y - cy;

                float distanceSquared = (dx * dx + dy * dy);

                if (distanceSquared > radiusSquared)
                {
                    return false;
                }
            }

            return true;
        }

        private static void FindCircle(Vector2 a, Vector2 b, out Vector2 center, out float radiusSquared)
        {
            center = (a + b) / 2f;
            radiusSquared = Vector2.DistanceSquared(center, a);
        }

        private static void FindCircleFast(Vector2 a, Vector2 b, out float cx, out float cy, out float radiusSquared)
        {
            cx = (a.X + b.X) * 0.5f;
            cy = (a.Y + b.Y) * 0.5f;

            float dx = cx - a.X;
            float dy = cy - a.Y;

            radiusSquared = dx * dx + dy * dy;
        }

        public override bool Equals(object obj)
        {
            if(obj is FlatCircle other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int result = new { this.Center, this.Radius }.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            string result = string.Format("Center: {0}, Radius: {1}", this.Center, this.Radius);
            return result;
        }
    }
}
