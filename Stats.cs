using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Flat.Graphics;
using System.Collections.Generic;

namespace Flat
{
    public sealed class Stats
    {
        private static Lazy<Stats> LazyInstance = new Lazy<Stats>(() => new Stats());

        public static Stats Instance
        {
            get { return LazyInstance.Value; }
        }

        private Sprites sprites;
        private SpriteFont font;
        private bool started;
        private float y;

        private Stats()
        {
            this.sprites = null;
            this.font = null;
            this.started = false;
            this.y = 0f;
        }

        public void Begin(Sprites sprites, SpriteFont font)
        { 
            if(this.started)
            {
                throw new Exception("Already started.");
            }

            this.sprites = sprites ?? throw new ArgumentNullException("sprites");
            this.font = font ?? throw new ArgumentNullException("font");

            this.y = 0;
            this.started = true;

            this.sprites.Begin(textureFiltering: true);
        }

        public void End()
        {
            if(!this.started)
            {
                throw new Exception("Never started.");
            }

            this.started = false;
            this.sprites.End();
        }

        public void Draw(object obj)
        {
            this.Draw(obj, Color.White);
        }

        public void Draw(object obj, Color color)
        {
            if(!this.started)
            {
                throw new Exception("Not started.");
            }

            string text = obj.ToString();
            Vector2 size = this.font.MeasureString(text);

            this.sprites.DrawString(this.font, text, new Vector2(2, this.y - 2), Color.Black);
            this.sprites.DrawString(this.font, text, new Vector2(0, this.y), color);

            this.y += size.Y;
        }

        public void Draw(string text)
        {
            this.Draw(text, Color.White);
        }

        public void Draw(string text, Color color)
        {
            if (!this.started)
            {
                throw new Exception("Not started.");
            }

            Vector2 size = this.font.MeasureString(text);

            this.sprites.DrawString(this.font, text, new Vector2(2, this.y - 2), Color.Black);
            this.sprites.DrawString(this.font, text, new Vector2(0, this.y), color);

            this.y += size.Y;
        }
    }
}
