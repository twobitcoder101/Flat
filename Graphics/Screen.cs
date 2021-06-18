using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flat.Graphics
{
    public sealed class Screen : IDisposable
    {
        public readonly static int MinDim = 64;
        public readonly static int MaxDim = 4096;

        private bool isDisposed;
        private Game game;

        public readonly int Width;
        public readonly int Height;

        private RenderTarget2D target;
        private bool isSet;

        public Screen(Game game, int width, int height)
        {
            this.isDisposed = false;
            this.game = game ?? throw new ArgumentNullException("game");

            this.Width = FlatMath.Clamp(width, Screen.MinDim, Screen.MaxDim);
            this.Height = FlatMath.Clamp(height, Screen.MinDim, Screen.MaxDim);

            this.target = new RenderTarget2D(this.game.GraphicsDevice, this.Width, this.Height);
            this.isSet = false;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if(this.isDisposed)
            {
                return;
            }

            if(disposing)
            {
                this.target?.Dispose();
                this.target = null;
            }

            this.isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public void Set()
        {
            if (this.isSet)
            {
                throw new Exception("The Screen is already set as the rendering target.");
            }

            this.game.GraphicsDevice.SetRenderTarget(this.target);
            this.isSet = true;
        }

        public void Unset()
        {
            if(!this.isSet)
            {
                throw new Exception("Function \"Set\" must be called before \"UnSet\" as pairs.");
            }

            this.game.GraphicsDevice.SetRenderTarget(null);
            this.isSet = false;
        }

        public void Present(Sprites sprites)
        {
            this.Present(sprites, Color.Black);
        }

        public void Present(Sprites sprites, Color backgroundColor, bool textureFiltering = true)
        {
            if(this.isSet)
            {
                throw new Exception("The \"Screen\" is currently set as the render target. \"UnSet\" the \"Screen\" before presenting.");
            }

            if(sprites is  null)
            {
                throw new ArgumentNullException("sprites");
            }

            this.game.GraphicsDevice.Clear(backgroundColor);

            Rectangle destinationRectangle = this.CalculateDestinationRectangle();

            sprites.Begin(null, textureFiltering, SpriteBlendType.Alpha);
            sprites.Draw(this.target, destinationRectangle, Color.White);
            sprites.End();
        }

        internal Rectangle CalculateDestinationRectangle()
        {
            // TODO: Should I recalculate the destination rectangle everytime or just calculate it when the game window size changes?

            Rectangle backbufferRectangle = this.game.GraphicsDevice.PresentationParameters.Bounds;
            float backbuffer_aspect = (float)backbufferRectangle.Width / (float)backbufferRectangle.Height;
            float screen_aspect = (float)this.Width / (float)this.Height;

            float rx = 0;
            float ry = 0;
            float rw = backbufferRectangle.Width;
            float rh = backbufferRectangle.Height;

            if (screen_aspect > backbuffer_aspect)
            {
                rh = rw / screen_aspect;
                ry = (backbufferRectangle.Height - rh) / 2f;
            }
            else if (screen_aspect < backbuffer_aspect)
            {
                rw = rh * screen_aspect;
                rx = (backbufferRectangle.Width - rw) / 2f;
            }

            Rectangle result = new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
            return result;
        }
    }
}
