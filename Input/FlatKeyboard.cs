using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flat.Input
{
    public sealed class FlatKeyboard
    {
        private static Lazy<FlatKeyboard> LazyInstance = new Lazy<FlatKeyboard>(() => new FlatKeyboard());

        public static FlatKeyboard Instance
        {
            get { return LazyInstance.Value; }
        }

        private KeyboardState currKeyboardState;
        private KeyboardState prevKeyboardState;
        
        public bool IsKeyAvailable
        {
            get { return this.currKeyboardState.GetPressedKeyCount() > 0; }
        }
        
        private FlatKeyboard()
        {
            this.currKeyboardState = Keyboard.GetState();
            this.prevKeyboardState = this.currKeyboardState;
        }

        public void Update()
        {
            this.prevKeyboardState = this.currKeyboardState;
            this.currKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyClicked(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key) && !this.prevKeyboardState.IsKeyDown(key);
        }
    }
}
