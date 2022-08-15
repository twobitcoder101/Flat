using System;
using Microsoft.Xna.Framework;

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

        private float angle;
        private Vector2 up;

        private const float MinZ = 1f;
        private const float MaxZ = 2000f;

        private const int MinZoom = 1;
        private const int MaxZoom = 64;

        #endregion

        #region Properties

        public Vector2 Position
        {
            get { return this.position; }
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
                this.zoom = FlatMath.Clamp(value, Camera.MinZoom, Camera.MaxZoom);
                this.z = this.baseZ * (1d / (double)this.zoom);

                this.updateRequired = true;
            }
        }

        public Vector2 Up
        {
            get { return this.up; }
        }

        public float Angle
        {
            get { return this.angle; }
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

            this.angle = 0f;
            this.up = new Vector2(MathF.Sin(angle), MathF.Cos(angle));

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
            if (!this.updateRequired)
            {
                return;
            }

            this.view = Matrix.CreateLookAt(new Vector3(0, 0, (float)this.z), Vector3.Zero, new Vector3(this.up, 0f));
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

        public void MoveTo(Vector2 position)
        {
            this.position = position;
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

        public void Rotate(float amount)
        {
            this.angle += amount;
            this.up = new Vector2(MathF.Sin(angle), MathF.Cos(angle));
            this.updateRequired = true;
        }

        public void GetExtents(out float width, out float height)
        {
            height = (float)this.GetVisibleHeight();
            width = height * this.aspectRatio;
        }

        public void GetExtents(out float left, out float right, out float bottom, out float top)
        {
            this.GetExtents(out float width, out float height);

            left = this.position.X - width * 0.5f;
            right = left + width;
            bottom = this.position.Y - height * 0.5f;
            top = bottom + height;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            this.GetExtents(out float left, out float right, out float bottom, out float top);

            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }

    }
}

