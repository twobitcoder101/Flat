using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

////////////////////////////////////////////////////////////////////////
///     This camera code is kept here for reference only.
////////////////////////////////////////////////////////////////////////

/*
namespace Flat.Graphics
{
    public sealed class Camera
    {
        #region Fields

        private Screen screen;
        private Vector2 position;
        private Matrix view;
        private Matrix proj;

        private float aspectRatio;
        private float fieldOfView;
        private double baseZ;
        private double z;

        private int zoom;
        private bool updateRequired;

        private const float MinZ = 1f;
        private const float MaxZ = 2000f;

        private const int MinZoom = 1;
        private const int MaxZoom = 32;

        #endregion

        #region Properties

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public float X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }

        public float Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }

        public Matrix View
        {
            get { return this.view; }
        }

        public Matrix Projection
        {
            get { return this.proj; }
        }

        internal float AspectRatio
        {
            get { return this.aspectRatio; }
        }

        internal float FieldOfView
        {
            get { return this.fieldOfView; }
        }

        public double BaseZ
        {
            get { return this.baseZ; }
        }

        public double Z
        {
            get { return this.z; }
        }

        public int Zoom
        {
            get { return this.zoom; }
            set
            {
                if (value < Camera.MinZoom || value > Camera.MaxZoom)
                {
                    return;
                }

                this.zoom = value;
                this.z = this.baseZ * (1d / (double)this.zoom);
                
                this.updateRequired = true;
            }
        }

        #endregion

        public Camera(Screen screen)
        {
            this.screen = screen ?? throw new ArgumentNullException("screen");
            this.position = Vector2.Zero;
            this.view = Matrix.Identity;
            this.proj = Matrix.Identity;

            this.aspectRatio = (float)screen.Width / (float)screen.Height;
            this.fieldOfView = MathHelper.PiOver2;
            this.baseZ = this.GetZFromHeight(screen.Height);
            this.z = baseZ;

            this.zoom = 1;
            this.updateRequired = true;

            this.Update();

        }

        private double GetZFromHeight(double height)
        {
            double result = (height * 0.5d) / Math.Tan(this.fieldOfView * 0.5d);
            return result;
        }

        private double GetVisibleHeightFromZ(double z)
        {
            double result = z * Math.Tan(this.fieldOfView * 0.5f) * 2f;
            return result;
        }

        private double GetVisibleHeight()
        {
            double result = 2d * Math.Tan(this.fieldOfView * 0.5d) * this.z;
            return result;
        }

        public void Update()
        {
            // Only update the camera view and projection when changes have occured.
            if(!this.updateRequired)
            {
                return;
            }

            this.view = Matrix.CreateLookAt(new Vector3(0, 0, (float)this.z), Vector3.Zero, Vector3.Up);
            this.proj = Matrix.CreatePerspectiveFieldOfView(this.fieldOfView, this.aspectRatio, Camera.MinZ, Camera.MaxZ);

            this.updateRequired = false;
        }

        public void ResetZ()
        {
            this.z = this.baseZ;
            this.Update();
        }

        public void Move(Vector2 amount)
        {
            this.position += amount;
        }

        public void MoveZ(float amount)
        {
            double new_z = this.z + amount;

            if (new_z < Camera.MinZ ||
                new_z > Camera.MaxZ)
            {
                return;
            }

            this.z = new_z;
            this.updateRequired = true;
        }

        public void IncZoom()
        {
            int new_zoom = this.zoom + 1;

            if (new_zoom < Camera.MinZoom || new_zoom > Camera.MaxZoom)
            {
                return;
            }

            this.zoom = new_zoom;
            this.z = this.baseZ * (1d / (double)this.zoom);

            this.updateRequired = true;
        }

        public void DecZoom()
        {
            int new_zoom = this.zoom - 1;

            if (new_zoom < Camera.MinZoom || new_zoom > Camera.MaxZoom)
            {
                return;
            }

            this.zoom = new_zoom;
            this.z = this.baseZ * (1d / (double)this.zoom);

            this.updateRequired = true;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            // Get the width and height in pixels.
            float height = (float)this.GetVisibleHeight();
            float width = height * this.aspectRatio;

            // Get the half sizes.
            float half_width = width / 2f;
            float half_height = height / 2f;

            // Calculate the min and max vectors.
            min = new Vector2(this.position.X - half_width, this.position.Y - half_height);
            max = new Vector2(this.position.X + half_width, this.position.Y + half_height);
        }
    }
}
*/