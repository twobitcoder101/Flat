using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flat;
using Flat.Graphics;

namespace Flat.Input
{
    using Ray = Microsoft.Xna.Framework.Ray;
    
    public sealed class FlatMouse
    {
        private static Lazy<FlatMouse> LazyInstance = new Lazy<FlatMouse>(() => new FlatMouse());

        public static FlatMouse Instance
        {
            get { return LazyInstance.Value; }
        }

        private MouseState currMouseState;
        private MouseState prevMouseState;

        public Point MouseWindowPosition
        {
            get
            {
                return this.currMouseState.Position;
            }
        }

        private FlatMouse()
        {
            this.currMouseState = Mouse.GetState();
            this.prevMouseState = this.currMouseState;
        }

        public void Update()
        {
            this.prevMouseState = this.currMouseState;
            this.currMouseState = Mouse.GetState();
        }

        public bool IsLeftMouseButtonDown()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightMouseButtonDown()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMiddleMouseButtonDown()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsLeftMouseButtonPressed()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed && this.prevMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsRightMouseButtonPressed()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed && this.prevMouseState.RightButton == ButtonState.Released;
        }

        public bool IsMiddleMouseButtonPressed()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed && this.prevMouseState.MiddleButton == ButtonState.Released;
        }

        public bool IsLeftMouseButtonReleased()
        {
            return this.currMouseState.LeftButton == ButtonState.Released && this.prevMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightMouseButtonReleased()
        {
            return this.currMouseState.RightButton == ButtonState.Released && this.prevMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMiddleMouseButtonReleased()
        {
            return this.currMouseState.MiddleButton == ButtonState.Released && this.prevMouseState.MiddleButton == ButtonState.Pressed;
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

        public Vector2 GetMouseWorldPosition(Game game, Screen screen, Camera camera)
        {
            // Create a viewport based on the game screen.
            Viewport screenViewport = new Viewport(0, 0, screen.Width, screen.Height);

            // Get the mouse pixel coordinates in that screen.
            Vector2 mouseScreenPosition = this.GetMouseScreenPosition(game, screen);

            // Create a ray that starts at the mouse screen position and points "into" the screen towards the game world plane.
            Ray mouseRay = this.CreateMouseRay(mouseScreenPosition, screenViewport, camera);

            // Plane where the flat 2D game world takes place.
            Plane worldPlane = new Plane(new Vector3(0, 0, 1f), 0f);

            // Determine the point where the ray intersects the game world plane.
            float? dist = mouseRay.Intersects(worldPlane);
            Vector3 ip = mouseRay.Position + mouseRay.Direction * dist.Value;

            // Send the result as a 2D world position vector.
            Vector2 result = new Vector2(ip.X, ip.Y);
            return result;
        }

        private Ray CreateMouseRay(Vector2 mouseScreenPosition, Viewport viewport, Camera camera)
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
