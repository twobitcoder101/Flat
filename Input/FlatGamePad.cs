using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flat.Input
{
    public enum DPadButtons
    {
        Up, Down, Left, Right
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

            bool result = false;

            if (button == DPadButtons.Up)
            {
                result = this.currGamePadStates[index].DPad.Up == ButtonState.Pressed;
            }
            else if (button == DPadButtons.Down)
            {
                result = this.currGamePadStates[index].DPad.Down == ButtonState.Pressed;
            }
            else if (button == DPadButtons.Left)
            {
                result = this.currGamePadStates[index].DPad.Left == ButtonState.Pressed;
            }
            else if (button == DPadButtons.Right)
            {
                result = this.currGamePadStates[index].DPad.Right == ButtonState.Pressed;
            }

            return result;
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
    }
}
