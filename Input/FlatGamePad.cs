using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flat.Input
{
    public enum DPadButtons
    {
        Up, Down, Left, Right
    }

    public enum GamePadButtons
    {
        A, B, X, Y, Start, Back, LeftShoulder, LeftStick, RightShoulder, RightStick
    }

    public sealed class FlatGamePad
    {
        public static int MaxGamePadCount = 4;

        private static Lazy<FlatGamePad> LazyInstance = new Lazy<FlatGamePad>(() => new FlatGamePad());

        private GamePadState[] currGamePadStates;
        private GamePadState[] prevGamePadStates;

        public static FlatGamePad Instance
        {
            get { return LazyInstance.Value; }
        }

        private int gamePadCount;

        public int GamePadCount
        {
            get { return this.gamePadCount; }
        }

        private FlatGamePad()
        {
            // Get the number of connected GamePads.
            for (int i = 0; i < FlatGamePad.MaxGamePadCount; i++)
            {
                GamePadCapabilities caps = GamePad.GetCapabilities(i);

                // First, determine if one is connected.
                if(caps.IsConnected)
                {
                    // Second, only allow GamePad type of controllers.
                    if (caps.GamePadType == GamePadType.GamePad)
                    {
                        // Third, make sure that there are a certain set of controls on the gamepad.
                        if (caps.HasAButton && caps.HasBButton && caps.HasXButton && caps.HasYButton &&
                            caps.HasStartButton && caps.HasBackButton &&
                            caps.HasDPadUpButton && caps.HasDPadDownButton && caps.HasDPadLeftButton && caps.HasDPadRightButton &&
                            caps.HasLeftStickButton && caps.HasRightStickButton &&
                            caps.HasLeftShoulderButton && caps.HasRightShoulderButton &&
                            caps.HasLeftTrigger && caps.HasRightTrigger &&
                            caps.HasLeftVibrationMotor && caps.HasRightVibrationMotor)
                        {
                            this.gamePadCount++;
                        }
                    }
                }
            }

            this.currGamePadStates = new GamePadState[this.gamePadCount];
            this.prevGamePadStates = new GamePadState[this.gamePadCount];

            for(int i = 0; i < this.gamePadCount; i++)
            {
                this.currGamePadStates[i] = GamePad.GetState(i);
                this.prevGamePadStates[i] = this.currGamePadStates[i];
            }
        }

        public void Update()
        {
            if(this.GamePadCountChanged(out int count))
            {
                this.gamePadCount = count;
                this.currGamePadStates = new GamePadState[this.gamePadCount];
                this.prevGamePadStates = new GamePadState[this.gamePadCount];
            }

            if(this.gamePadCount == 0)
            {
                return;
            }

            // Save the previous state.
            for(int i = 0; i < this.prevGamePadStates.Length; i++)
            {
                this.prevGamePadStates[i] = this.currGamePadStates[i];
            }

            // Get the current state.
            for (int i = 0; i < this.currGamePadStates.Length; i++)
            {
                GamePadState state = GamePad.GetState(i);

                if (state.IsConnected)
                {
                    this.currGamePadStates[i] = state;
                }
            }
        }

        private bool GamePadCountChanged(out int count)
        {
            count = 0;

            // Recound the gamepads to see if any have been added or removed.
            for (int i = 0; i < FlatGamePad.MaxGamePadCount; i++)
            {
                if (GamePad.GetState(i).IsConnected)
                {
                    count++;
                }
            }

            return count != this.gamePadCount;
        }

        private bool IsValidGamePadIndex(int index)
        {
            return this.gamePadCount > 0 && index >= 0 && index < this.gamePadCount;
        }

        public bool IsDPadPressed(int index, DPadButtons button)
        {
            if(!this.IsValidGamePadIndex(index))
            {
                return false;
            }

            switch(button)
            {
                case DPadButtons.Up: return this.currGamePadStates[index].DPad.Up == ButtonState.Pressed;
                case DPadButtons.Down: return this.currGamePadStates[index].DPad.Down == ButtonState.Pressed;
                case DPadButtons.Left: return this.currGamePadStates[index].DPad.Left == ButtonState.Pressed;
                case DPadButtons.Right: return this.currGamePadStates[index].DPad.Right == ButtonState.Pressed;
                default: return false;
            }
        }

        public Vector2 LeftThumbStick(int index)
        {
            if (!this.IsValidGamePadIndex(index))
            {
                return Vector2.Zero;
            }

            return this.currGamePadStates[index].ThumbSticks.Left;
        }

        public Vector2 RightThumbStick(int index)
        {
            if (!this.IsValidGamePadIndex(index))
            {
                return Vector2.Zero;
            }

            return this.currGamePadStates[index].ThumbSticks.Right;
        }

        public float RightTrigger(int index)
        {
            if (!this.IsValidGamePadIndex(index))
            {
                return 0f;
            }

            return this.currGamePadStates[index].Triggers.Right;
        }

        public float LefetTrigger(int index)
        {
            if (!this.IsValidGamePadIndex(index))
            {
                return 0f;
            }

            return this.currGamePadStates[index].Triggers.Left;
        }

        public bool IsButtonDown(int index, GamePadButtons button)
        {
            if (!this.IsValidGamePadIndex(index))
            {
                return false;
            }

            switch(button)
            {
                case GamePadButtons.A: return this.currGamePadStates[index].Buttons.A == ButtonState.Pressed;
                case GamePadButtons.B: return this.currGamePadStates[index].Buttons.B == ButtonState.Pressed;
                case GamePadButtons.X: return this.currGamePadStates[index].Buttons.X == ButtonState.Pressed;
                case GamePadButtons.Y: return this.currGamePadStates[index].Buttons.Y == ButtonState.Pressed;
                case GamePadButtons.Start: return this.currGamePadStates[index].Buttons.Start == ButtonState.Pressed;
                case GamePadButtons.Back: return this.currGamePadStates[index].Buttons.Back == ButtonState.Pressed;
                case GamePadButtons.LeftShoulder: return this.currGamePadStates[index].Buttons.LeftShoulder == ButtonState.Pressed;
                case GamePadButtons.LeftStick: return this.currGamePadStates[index].Buttons.LeftStick == ButtonState.Pressed;
                case GamePadButtons.RightShoulder: return this.currGamePadStates[index].Buttons.RightShoulder == ButtonState.Pressed;
                case GamePadButtons.RightStick: return this.currGamePadStates[index].Buttons.RightStick == ButtonState.Pressed;
                default: return false;
            }
        }

    }
}
