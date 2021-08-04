using System;
using System.Runtime.CompilerServices;

namespace Flat
{
    public readonly struct FlatSize
    {
        public readonly int Width;
        public readonly int Height;

        public FlatSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FlatSize other)
        {
            return this.Width == other.Width && this.Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if(obj is FlatSize other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int result = new { this.Width, this.Height }.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            string result = string.Format("({0}, {1})", this.Width, this.Height);
            return result;
        }
    }
}
