using System;
using Microsoft.Xna.Framework;

namespace Flat
{
    public readonly struct FlatBox
    {
        public readonly Vector2 Min;
        public readonly Vector2 Max;

        public static readonly FlatBox Empty = new FlatBox(Vector2.Zero, Vector2.Zero);

        public FlatBox(Vector2 min, Vector2 max)
        {
            this.Min = min;
            this.Max = max;
        }

        public FlatBox(Vector2 center, float width, float height)
        {
            float left = center.X - width * 0.5f;
            float right = left + width;
            float bottom = center.Y - height * 0.5f;
            float top = bottom + height;

            this.Min = new Vector2(left, bottom);
            this.Max = new Vector2(right, top);
        }

        public FlatBox(float minX, float maxX, float minY, float maxY)
        {
            this.Min = new Vector2(minX, minY);
            this.Max = new Vector2(maxX, maxY);
        }

        public bool Equals(in FlatBox other)
        {
            return this.Min == other.Min && this.Max == other.Max;
        }

        public override bool Equals(object obj)
        {
            if(obj is FlatBox other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int result = new { this.Min, this.Max }.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            string result = string.Format("Min: {0}, Max: {1}", this.Min, this.Max);
            return result;
        }
    }
}
