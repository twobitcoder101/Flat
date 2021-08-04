using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Flat.Physics;

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

        public static bool FindSmallestBoundingCircle(Vector2[] points, out FlatCircle circle, out int loopCount)
        {
            circle = default(FlatCircle);
            loopCount = 0;
            const int minPoints = 2;
            const int maxPoints = 256;

            if (points is null)
            {
                return false;
            }

            if (points.Length < minPoints)
            {
                return false;
            }

            if (points.Length > maxPoints)
            {
                return false;
            }

            float smallestCircleRadiusSquared = float.MaxValue;
            Vector2 smallestCircleCenter = Vector2.Zero;

            float radiusSquared = 0f;
            Vector2 center = Vector2.Zero;

            // 1) Test for bounding circle made by a pair of points.

            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 a = points[i];

                for (int j = i + 1; j < points.Length; j++)
                {
                    loopCount++;

                    Vector2 b = points[j];

                    FlatCircle.FindCircle(a, b, out center, out radiusSquared);

                    if (radiusSquared < smallestCircleRadiusSquared)
                    {
                        if (FlatCircle.CircleContainsAllPoints(points, i, j, center, radiusSquared))
                        {
                            smallestCircleCenter = center;
                            smallestCircleRadiusSquared = radiusSquared;
                        }
                    }
                }
            }

            // 2) Test for bounding circle made by a trio of points.

            if (points.Length >= 3)
            {
                for (int i = 0; i < points.Length - 2; i++)
                {
                    Vector2 a = points[i];

                    for (int j = i + 1; j < points.Length - 1; j++)
                    {
                        Vector2 b = points[j];

                        for (int k = j + 1; k < points.Length; k++)
                        {
                            loopCount++;

                            Vector2 c = points[k];

                            FlatCircle.FindCircle(a, b, c, out center, out radiusSquared);

                            if (radiusSquared < smallestCircleRadiusSquared)
                            {
                                if (FlatCircle.CircleContainsAllPoints(points, i, j, k, center, radiusSquared))
                                {
                                    smallestCircleCenter = center;
                                    smallestCircleRadiusSquared = radiusSquared;
                                }
                            }
                        }
                    }
                }
            }

            circle = new FlatCircle(smallestCircleCenter, MathF.Sqrt(smallestCircleRadiusSquared));
            return true;
        }

        public static bool FindSmallestBoundingCircleFast(Vector2[] points, out FlatCircle circle, out int loopCount)
        {
            circle = default(FlatCircle);
            loopCount = 0;
            const int minPoints = 2;
            const int maxPoints = 256;

            if (points is null)
            {
                return false;
            }

            if (points.Length < minPoints)
            {
                return false;
            }

            if (points.Length > maxPoints)
            {
                return false;
            }

            float smallestCircleRadiusSquared = float.MaxValue;
            float smallestCircleCenterX = 0f;
            float smallestCircleCenterY = 0f;

            float radiusSquared = 0f;
            float cx = 0f;
            float cy = 0f;

            // 1) Test for bounding circle made by a pair of points.

            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 a = points[i];

                for (int j = i + 1; j < points.Length; j++)
                {
                    loopCount++;

                    Vector2 b = points[j];

                    FlatCircle.FindCircleFast(a, b, out cx, out cy, out radiusSquared);

                    if (radiusSquared < smallestCircleRadiusSquared)
                    {
                        if (FlatCircle.CircleContainsAllPoints(points, i, j, cx, cy, radiusSquared))
                        {
                            smallestCircleCenterX = cx;
                            smallestCircleCenterY = cy;
                            smallestCircleRadiusSquared = radiusSquared;
                        }
                    }
                }
            }

            // 2) Test for bounding circle made by a trio of points.

            if (points.Length >= 3)
            {
                for (int i = 0; i < points.Length - 2; i++)
                {
                    Vector2 a = points[i];

                    for (int j = i + 1; j < points.Length - 1; j++)
                    {
                        Vector2 b = points[j];

                        for (int k = j + 1; k < points.Length; k++)
                        {
                            loopCount++;

                            Vector2 c = points[k];

                            FlatCircle.FindCircleFast(a, b, c, out cx, out cy, out radiusSquared);

                            if (radiusSquared < smallestCircleRadiusSquared)
                            {
                                if (FlatCircle.CircleContainsAllPoints(points, i, j, k, cx, cy, radiusSquared))
                                {
                                    smallestCircleCenterX = cx;
                                    smallestCircleCenterY = cy;
                                    smallestCircleRadiusSquared = radiusSquared;
                                }
                            }
                        }
                    }
                }
            }

            circle = new FlatCircle(smallestCircleCenterX, smallestCircleCenterY, MathF.Sqrt(smallestCircleRadiusSquared));
            return true;
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

        private static void FindCircle(Vector2 a, Vector2 b, Vector2 c, out Vector2 center, out float radiusSquared)
        {
            Vector2 r = (a + b) / 2f;   // Center point of segment "a" to "b".
            Vector2 s = (b + c) / 2f;   // Center point of segment "b" to "c".

            Vector2 ab = b - a;     // Vector starting at "a" and pointing at "b".
            Vector2 bc = c - b;     // Vector starting at "b" and pointing at "c".

            Vector2 n1 = new Vector2(ab.Y, -ab.X);      // Normal of "ab" (note: this has not been normalized).
            Vector2 n2 = new Vector2(bc.Y, -bc.X);      // Normal of "bc" (note: this has not been normalized).

            Vector2 t = r + n1;     // Get another random point on "n1" starting at "r" to define a line segment.
            Vector2 u = s + n2;     // Get another random point on "n2" starting at "s" to define a line segment.

            // Determine if and where the lines intersect.
            // The intersection point of the lines will be the triangle circumcenter or the center of the circle.
            if (!Collision.IntersectLines(r, t, s, u, out center))
            {
                throw new Exception("Unable to find circle. Arguments 'a', 'b', and 'c' do not form a valid triangle.");
            }

            // Find the radius squared (just the distance squared from the circumcenter to any of the triangle vertices).
            radiusSquared = Vector2.DistanceSquared(center, a);
        }

        private static void FindCircleFast(Vector2 a, Vector2 b, Vector2 c, out float cx, out float cy, out float radiusSquared)
        {
            //Vector2 r = (a + b) / 2f;   // Center point of segment "a" to "b".
            //Vector2 s = (b + c) / 2f;   // Center point of segment "b" to "c".

            float rx = (a.X + b.X) * 0.5f;
            float ry = (a.Y + b.Y) * 0.5f;

            float sx = (b.X + c.X) * 0.5f;
            float sy = (b.Y + c.Y) * 0.5f;

            //Vector2 ab = b - a;     // Vector starting at "a" and pointing at "b".
            //Vector2 bc = c - b;     // Vector starting at "b" and pointing at "c".

            float abx = b.X - a.X;
            float aby = b.Y - a.Y;

            float bcx = c.X - b.X;
            float bcy = c.Y - b.Y;

            //Vector2 n1 = new Vector2(ab.Y, -ab.X);      // Normal of "ab" (note: this has not been normalized).
            //Vector2 n2 = new Vector2(bc.Y, -bc.X);      // Normal of "bc" (note: this has not been normalized).

            float n1x = aby;
            float n1y = -abx;

            float n2x = bcy;
            float n2y = -bcx;

            //Vector2 t = r + n1;     // Get another random point on "n1" starting at "r" to define a line segment.
            //Vector2 u = s + n2;     // Get another random point on "n2" starting at "s" to define a line segment.

            float tx = rx + n1x;
            float ty = ry + n1y;

            float ux = sx + n2x;
            float uy = sy + n2y;

            // Determine if and where the lines intersect.
            // The intersection point of the lines will be the triangle circumcenter or the center of the circle.
            if (!Collision.IntersectLinesFast(rx, ry, tx, ty, sx, sy, ux, uy, out cx, out cy))
            {
                throw new Exception("Unable to find circle. Arguments 'a', 'b', and 'c' do not form a valid triangle.");
            }

            // Find the radius squared (just the distance squared from the circumcenter to any of the triangle vertices).
            //radiusSquared = Vector2.DistanceSquared(center, a);

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
