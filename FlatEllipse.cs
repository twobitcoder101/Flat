using System;
using Microsoft.Xna.Framework;

namespace Flat
{
    public readonly struct FlatEllipse
    {
        public readonly Vector2 Center;
        public readonly Vector2 Radius;

        public FlatEllipse(Vector2 center, Vector2 radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public void GetExtents(out FlatBox box)
        {
            Vector2 min = new Vector2(this.Center.X - Radius.X, this.Center.Y - this.Radius.Y);
            Vector2 max = new Vector2(this.Center.X + Radius.X, this.Center.Y + this.Radius.Y);
            box = new FlatBox(min, max);
        }

        public bool Contains(Vector2 v, out float d)
        {
            // https://math.stackexchange.com/questions/76457/check-if-a-point-is-within-an-ellipse

            float dx = v.X - this.Center.X;
            float dy = v.Y - this.Center.Y;

            float num1 = dx * dx;
            float den1 = this.Radius.X * this.Radius.X;

            float num2 = dy * dy;
            float den2 = this.Radius.Y * this.Radius.Y;

            d = num1 / den1 + num2 / den2;

            // todo: Should this be less than OR EQUAL TO instead of just less than?
            return d < 1f;
        }

        public bool Equals(FlatEllipse other)
        {
            return this.Center == other.Center && this.Radius == other.Radius;
        }

        public override bool Equals(object obj)
        {
            if(obj is FlatEllipse other)
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
            string result = string.Format("Center: {0}, Radius(X): {1}, Radius(Y): {2}", this.Center, this.Radius.X, this.Radius.Y);
            return result;
        }
    }
}
