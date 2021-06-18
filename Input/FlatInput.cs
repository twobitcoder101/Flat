using System;
using Flat.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Flat.Input
{
    using Ray = Microsoft.Xna.Framework.Ray;
    
    [Obsolete("Use \"FlatKeyboard\" and \"FlatMouse\" instead.", false)]
    public sealed class FlatInput
    {
        private static Lazy<FlatInput> LazyInstance = new Lazy<FlatInput>(() => new FlatInput());

        public static FlatInput Instance
        {
            get { return LazyInstance.Value; }
        }

        private KeyboardState currKeyboardState;
        private KeyboardState prevKeyboardState;

        private MouseState currMouseState;
        private MouseState prevMouseState;

        public Point MouseWindowPosition
        {
            get
            {
                return this.currMouseState.Position;
            }
        }

        private FlatInput()
        {
            this.currKeyboardState = Keyboard.GetState();
            this.prevKeyboardState = this.currKeyboardState;

            this.currMouseState = Mouse.GetState();
            this.prevMouseState = this.currMouseState;
        }

        public void Update()
        {
            this.prevKeyboardState = this.currKeyboardState;
            this.currKeyboardState = Keyboard.GetState();

            this.prevMouseState = this.currMouseState;
            this.currMouseState = Mouse.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyClicked(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key) && !this.prevKeyboardState.IsKeyDown(key);
        }

        public bool IsLeftMouseButtonDown()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightMouseButtonDown()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsLeftMouseButtonClicked()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed && this.prevMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsRightMouseButtonClicked()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed && this.prevMouseState.RightButton == ButtonState.Released;
        }

        public Vector2 GetMouseScreenPosition(Game game, Screen screen)
        {
            // Get the size and position of the screen when stretched to fit into the game window (keeping the correct aspect ratio).
            Rectangle screenDestinationRectangle = screen.CalculateDestinationRectangle();

            // Get the position of the mouse in the game window backbuffer coordinates.
            Point mouseWindowPosition = this.MouseWindowPosition;

            // Get the position of the mouse relative to the screen destination rectangle position.
            float sx = mouseWindowPosition.X - screenDestinationRectangle.X;
            float sy = mouseWindowPosition.Y - screenDestinationRectangle.Y;

            // Convert the position to a normalized ratio inside the screen destination rectangle.
            sx /= (float)screenDestinationRectangle.Width;
            sy /= (float)screenDestinationRectangle.Height;

            // Multiply the normalized coordinates by the actual size of the screen to get the location in screen coordinates.
            float x = sx * (float)screen.Width;
            float y = sy * (float)screen.Height;

            return new Vector2(x, y);
        }

        public Vector2 GetMouseWorldPositionInPixels(Game game, Screen screen, Camera camera)
        {
            // Create a viewport based on the game screen.
            Viewport screenViewport = new Viewport(0, 0, screen.Width, screen.Height);

            // Get the mouse pixel coordinates in that screen.
            Vector2 mouseScreenPosition = this.GetMouseScreenPosition(game, screen);

            // Create a ray that starts at the mouse screen position and points "into" the screen towards the game world plane.
            Ray mouseRay = this.GetMouseRay(mouseScreenPosition, screenViewport, camera);

            // Plane where the flat 2D game world takes place.
            Plane worldPlane = new Plane(new Vector3(0, 0, 1f), 0f);

            // Determine the point where the ray intersects the game world plane.
            float? dist = mouseRay.Intersects(worldPlane);
            Vector3 ip = mouseRay.Position + mouseRay.Direction * dist.Value;

            // Send the result as a 2D world position vector.
            Vector2 result = new Vector2(ip.X, ip.Y);
            return result;
        }

        //public Vector2 GetMouseWorldPositionInMeters(Game game, Screen screen, Camera camera)
        //{
        //    return Util.ConvertPixelsToMeters(this.GetMouseWorldPositionInPixels(game, screen, camera));
        //}

        private Ray GetMouseRay(Vector2 mouseScreenPosition, Viewport viewport, Camera camera)
        {
            // Near and far points that will indicate the line segment used to define the ray.
            Vector3 nearPoint = new Vector3(mouseScreenPosition, 0);
            Vector3 farPoint = new Vector3(mouseScreenPosition, 1);

            // Convert the near and far points to world coordinates.
            nearPoint = viewport.Unproject(nearPoint, camera.Projection, camera.View, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, camera.Projection, camera.View, Matrix.Identity);

            // Determine the direction.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // Resulting ray starts at the near mouse position and points "into" the screen.
            Ray result = new Ray(nearPoint, direction);
            return result;
        }
    }
}
