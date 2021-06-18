using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flat.Graphics
{
    public enum SpriteBlendType
    {
        Additive, Alpha
    }

    public sealed class Sprites : IDisposable
    {
        private bool isDisposed;
        private Game game;
        private SpriteBatch sprites;
        private BasicEffect effect;

        public Sprites(Game game)
        {
            this.isDisposed = false;
            this.game = game ?? throw new ArgumentNullException("game");
            this.sprites = new SpriteBatch(this.game.GraphicsDevice);

            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.PreferPerPixelLighting = false;
            this.effect.VertexColorEnabled = true;
            this.effect.Texture = null;
            this.effect.TextureEnabled = true;
            this.effect.Projection = Matrix.Identity;
            this.effect.View = Matrix.Identity;
            this.effect.World = Matrix.Identity;
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
                this.effect?.Dispose();
                this.sprites?.Dispose();
            }

            this.isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public void Begin(Camera camera = null, bool textureFiltering = false, SpriteBlendType blendType = SpriteBlendType.Alpha)
        {
            SamplerState samplerState = SamplerState.PointClamp;
            if(textureFiltering)
            {
                samplerState = SamplerState.AnisotropicClamp;
            }
            
            // TODO: Do I need to offset the projection by 1/2 pixel?

            if (camera is null)
            {
                Viewport viewport = this.game.GraphicsDevice.Viewport;
                this.effect.View = Matrix.Identity;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, 0, viewport.Height, 0, 1);
            }
            else
            {
                // Update the camera's view and projection matrices if the camera Z position has changed.
                camera.Update();

                this.effect.View = camera.View;
                this.effect.Projection = camera.Projection;

                // TODO: Do I really want anisotropic filtering whenever the camera is farther away then the base Z.
                if (camera.Z > camera.BaseZ)
                {
                    samplerState = SamplerState.AnisotropicClamp;
                }
            }

            BlendState blendState = BlendState.AlphaBlend;
            if(blendType == SpriteBlendType.Additive)
            {
                blendState = BlendState.Additive;
            }

            this.sprites.Begin(samplerState: samplerState, blendState: blendState, rasterizerState: RasterizerState.CullNone, effect: this.effect);
        }

        public void End()
        {
            this.sprites.End();
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            this.sprites.Draw(texture, destinationRectangle, null, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? sourceRectangle, Vector2 origin, Vector2 position, float rotation, float scale, Color color)
        {
            this.sprites.Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale), SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? sourceRectangle, Vector2 origin, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            this.sprites.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? sourceRectangle, Vector2 origin, Vector2 position, Color color)
        {
            this.sprites.Draw(texture, position, sourceRectangle, color, 0f, origin, 1f, SpriteEffects.FlipVertically, 0f);
        }

        public void DrawString(SpriteFont font, string text, Vector2 position, Color color)
        {
            this.sprites.DrawString(font, text, position, color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }

        public void DrawString(SpriteFont font, StringBuilder text, Vector2 position, Color color)
        {
            this.sprites.DrawString(font, text, position, color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }

        public void DrawString(SpriteFont font, string text, Vector2 position, float rotation, Vector2 origin, float scale, Color color)
        {
            this.sprites.DrawString(font, text, position, color, rotation, origin, scale, SpriteEffects.FlipVertically, 0f);
        }
    }
}
