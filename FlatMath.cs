using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Flat
{
    public static class FlatMath
    {
        public static readonly float TwoPi = 6.283185307179586476925286766559f;
        public static readonly float Pi = 3.1415926535897932384626433832795f;
        public static readonly float PiOverTwo = 1.5707963267948966192313216916398f;

        public static float Min(float a, float b)
        {
            if (float.IsNaN(a))
            {
                throw new ArithmeticException("a");
            }

            if (float.IsNaN(b))
            {
                throw new ArithmeticException("b");
            }

            if (a <= b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        public static float Max(float a, float b)
        {
            if (float.IsNaN(a))
            {
                throw new ArithmeticException("a");
            }

            if (float.IsNaN(b))
            {
                throw new ArithmeticException("b");
            }

            if (a >= b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min > max)
            {
                FlatUtil.Swap(ref min, ref max);
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static float Clamp(float value, float min, float max)
        {
            if (float.IsNaN(min))
            {
                throw new ArithmeticException("min is NaN.");
            }

            if (float.IsNaN(max))
            {
                throw new ArithmeticException("max is NaN");
            }

            if (min > max)
            {
                FlatUtil.Swap(ref min, ref max);
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static double Clamp(double value, double min, double max)
        {
            if (double.IsNaN(min))
            {
                throw new ArithmeticException("min is NaN.");
            }

            if (double.IsNaN(max))
            {
                throw new ArithmeticException("max is NaN");
            }

            if (min > max)
            {
                FlatUtil.Swap(ref min, ref max);
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalize(Vector2 value)
        {
            float invLen = 1f / MathF.Sqrt(value.X * value.X + value.Y * value.Y);
            return new Vector2(value.X * invLen, value.Y * invLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(ref float x, ref float y)
        {
            float invLen = 1f / MathF.Sqrt(x * x + y * y);
            x *= invLen;
            y *= invLen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalize(float x, float y)
        {
            float invLen = 1f / MathF.Sqrt(x * x + y * y);
            return new Vector2(x * invLen, y * invLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(Vector2 value)
        {
            return MathF.Sqrt(value.X * value.X + value.Y * value.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(float x, float y)
        {
            return MathF.Sqrt(x * x + y * y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSquared(Vector2 value)
        {
            return value.X * value.X + value.Y * value.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSquared(float x, float y)
        {
            return x * x + y * y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector2 a, Vector2 b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(Vector2 a, Vector2 b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float ax, float ay, float bx, float by)
        {
            float dx = ax - bx;
            float dy = ay - by;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(float ax, float ay, float bx, float by)
        {
            float dx = ax - bx;
            float dy = ay - by;
            return dx * dx + dy * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(float ax, float ay, float bx, float by)
        {
            return ax * bx + ay * by;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(float ax, float ay, float bx, float by)
        {
            return ax * by - ay * bx;
        }

        public static Vector2 FindArithmeticMean(Vector2[] vertices)
        {
            float sx = 0f;
            float sy = 0f;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 v = vertices[i];
                sx += v.X;
                sy += v.Y;
            }

            float invArrayLen = 1f / vertices.Length;
            return new Vector2(sx * invArrayLen, sy * invArrayLen);
        }

        public static Vector2 FindArithmeticMean(Vector2 min, Vector2 max)
        {
            return new Vector2((min.X + max.X) * 0.5f, (min.Y + max.Y) * 0.5f);
        }

    }
}
