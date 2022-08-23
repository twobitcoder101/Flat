using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flat.Graphics
{
    public sealed class Shapes : IDisposable
    {
        private bool isDisposed;
        private Game game;
        private BasicEffect effect;
        private Camera camera;
        private bool usingCamera;

        private VertexPositionColor[] vertices;
        private int[] indices;

        private int shapeCount;
        private int indexCount;
        private int vertexCount;

        private bool started;

        public Shapes(Game game)
        {
            this.isDisposed = false;
            this.game = game ?? throw new ArgumentNullException("game");

            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.TextureEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.PreferPerPixelLighting = false;

            this.camera = null;
            this.usingCamera = false;

            this.vertices = new VertexPositionColor[1024];
            this.indices = new int[this.vertices.Length * 3];

            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;

            this.started = false;
        }
        
        ~Shapes()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.effect?.Dispose();
                this.effect = null;
            }

            this.isDisposed = true;
        }


        public void Begin(Camera camera = null)
        {
            if (this.started)
            {
                throw new Exception("Batch was already started.\n" +
                                    "Batch must call \"End\" before new batching can start.");
            }

            if (camera is null)
            {
                Viewport viewport = this.game.GraphicsDevice.Viewport;
                this.effect.View = Matrix.Identity;
                this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, 0, viewport.Height, 0, 1);

                this.camera = null;
                this.usingCamera = false;
            }
            else
            {
                // Update the camera's view and projection matrices if the camera Z position has changed.
                camera.Update();

                this.effect.View = camera.View;
                this.effect.Projection = camera.Projection;

                this.camera = camera;
                this.usingCamera = true;
            }

            this.started = true;
        }

        public void End()
        {
            if (!this.started)
            {
                throw new Exception("Batch was not started.\n" +
                                    "Batch must call \"Begin\" before ending the batch.");
            }

            this.Flush();
            this.started = false;
        }

        private void Flush()
        {
            if (this.shapeCount == 0)
            {
                return;
            }

            GraphicsDevice device = this.game.GraphicsDevice;
            int primitiveCount = this.indexCount / 3;

            EffectPassCollection passes = this.effect.CurrentTechnique.Passes;
            for(int i = 0; i < passes.Count; i++)
            {
                EffectPass pass = passes[i];
                pass.Apply();

                device.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this.vertices,
                    0,
                    this.vertexCount,
                    this.indices,
                    0,
                    primitiveCount);
            }

            this.shapeCount = 0;
            this.vertexCount = 0;
            this.indexCount = 0;
        }

        public void EnsureStarted()
        {
            if (!this.started)
            {
                throw new Exception("Shape batching must be started first.");
            }
        }

        private void EnsureSpace(int shapeVertexCount, int shapeIndexCount)
        {
            int maxVertexCount = this.vertices.Length;
            int maxIndexCount = this.indices.Length;

            if (shapeVertexCount > maxVertexCount || shapeIndexCount > maxIndexCount)
            {
                throw new Exception("Max vertex or index count reached for one draw.");
            }

            if (this.vertexCount + shapeVertexCount > maxVertexCount || this.indexCount + shapeIndexCount > maxIndexCount)
            {
                this.Flush();
            }
        }

        public void DrawTriangleFill(Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            this.EnsureStarted();

            int shapeVertexCount = 3;
            int shapeIndexCount = 3;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
        }

        public void DrawBoxFill(Vector2 min, Vector2 max, Color color)
        {
            this.EnsureStarted();

            int shapeVertexCount = 4;
            int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            Vector3 a = new Vector3(min.X, max.Y, 0f);
            Vector3 b = new Vector3(max.X, max.Y, 0f);
            Vector3 c = new Vector3(max.X, min.Y, 0f);
            Vector3 d = new Vector3(min.X, min.Y, 0f);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            this.vertices[this.vertexCount++] = new VertexPositionColor(a, color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(b, color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(c, color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(d, color);

            this.shapeCount++;
        }

        public void DrawBoxFill(Vector2 center, float width, float height, Color color)
        {
            float left = center.X - width * 0.5f;
            float right = left + width;
            float bottom = center.Y - height * 0.5f;
            float top = bottom + height;

            Vector2 min = new Vector2(left, bottom);
            Vector2 max = new Vector2(right, top);

            this.DrawBoxFill(min, max, color);
        }

        public void DrawBoxFill(float x, float y, float width, float height, Color color)
        {
            Vector2 min = new Vector2(x, y);
            Vector2 max = new Vector2(x + width, y + height);

            this.DrawBoxFill(min, max, color);
        }

        public void DrawBoxFill(float x, float y, float width, float height, Color[] colors)
        {
            Vector2 min = new Vector2(x, y);
            Vector2 max = new Vector2(x + width, y + height);

            this.DrawBoxFill(min, max, colors);
        }

        public void DrawBoxFill(Vector2 min, Vector2 max, Color[] colors)
        {
            if (colors is null || colors.Length != 4)
            {
                throw new ArgumentOutOfRangeException("colors array must have exactly 4 items.");
            }

            this.EnsureStarted();

            int shapeVertexCount = 4;
            int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            Vector3 a = new Vector3(min.X, max.Y, 0f);
            Vector3 b = new Vector3(max.X, max.Y, 0f);
            Vector3 c = new Vector3(max.X, min.Y, 0f);
            Vector3 d = new Vector3(min.X, min.Y, 0f);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            this.vertices[this.vertexCount++] = new VertexPositionColor(a, colors[0]);
            this.vertices[this.vertexCount++] = new VertexPositionColor(b, colors[1]);
            this.vertices[this.vertexCount++] = new VertexPositionColor(c, colors[2]);
            this.vertices[this.vertexCount++] = new VertexPositionColor(d, colors[3]);

            this.shapeCount++;
        }

        public void DrawBoxFill(Vector2 center, float width, float height, float rotation, Color color)
        {
            this.DrawBoxFill(center, width, height, rotation, Vector2.One, color);
        }

        public void DrawBoxFill(Vector2 center, float width, float height, float rotation, float scale, Color color)
        {
            this.DrawBoxFill(center, width, height, rotation, new Vector2(scale, scale), color);
        }

        public void DrawBoxFill_Slow(Vector2 center, float width, float height, float rotation, Vector2 scale, Color color)
        {
            this.EnsureStarted();

            int shapeVertexCount = 4;
            int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = -width * 0.5f;
            float right = left + width;
            float bottom = -height * 0.5f;
            float top = bottom + height;

            FlatTransform transform = new FlatTransform(center, rotation, scale);

            Vector2 a = FlatUtil.Transform(new Vector2(left, top), transform);
            Vector2 b = FlatUtil.Transform(new Vector2(right, top), transform);
            Vector2 c = FlatUtil.Transform(new Vector2(right, bottom), transform);
            Vector2 d = FlatUtil.Transform(new Vector2(left, bottom), transform);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

            this.shapeCount++;
        }

        public void DrawBoxFill(Vector2 center, float width, float height, float rotation, Vector2 scale, Color color)
        {
            this.EnsureStarted();

            int shapeVertexCount = 4;
            int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = -width * 0.5f;
            float right = left + width;
            float bottom = -height * 0.5f;
            float top = bottom + height;

            // Precompute the trig. functions.
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            // Vector components:

            float ax = left;
            float ay = top;
            float bx = right;
            float by = top;
            float cx = right;
            float cy = bottom;
            float dx = left;
            float dy = bottom;

            // Scale transform:

            float sx1 = ax * scale.X;
            float sy1 = ay * scale.Y;
            float sx2 = bx * scale.X;
            float sy2 = by * scale.Y;
            float sx3 = cx * scale.X;
            float sy3 = cy * scale.Y;
            float sx4 = dx * scale.X;
            float sy4 = dy * scale.Y;

            // Rotation transform:

            float rx1 = sx1 * cos - sy1 * sin;
            float ry1 = sx1 * sin + sy1 * cos;
            float rx2 = sx2 * cos - sy2 * sin;
            float ry2 = sx2 * sin + sy2 * cos;
            float rx3 = sx3 * cos - sy3 * sin;
            float ry3 = sx3 * sin + sy3 * cos;
            float rx4 = sx4 * cos - sy4 * sin;
            float ry4 = sx4 * sin + sy4 * cos;

            // Translation transform:

            ax = rx1 + center.X;
            ay = ry1 + center.Y;
            bx = rx2 + center.X;
            by = ry2 + center.Y;
            cx = rx3 + center.X;
            cy = ry3 + center.Y;
            dx = rx4 + center.X;
            dy = ry4 + center.Y;

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(ax, ay, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(bx, by, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(cx, cy, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(dx, dy, 0f), color);

            this.shapeCount++;
        }

        public void DrawQuadFill(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy, Color color)
        {
            this.EnsureStarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 1 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = 2 + this.vertexCount;
            this.indices[this.indexCount++] = 3 + this.vertexCount;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(ax, ay, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(bx, by, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(cx, cy, 0f), color);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(dx, dy, 0f), color);

            this.shapeCount++;
        }

        public void DrawQuadFill(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color)
        {
            this.DrawQuadFill(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y, color);
        }

        public void DrawCircleFill(in FlatCircle circle, int points, Color color)
        {
            this.DrawCircleFill(circle.Center.X, circle.Center.Y, circle.Radius, points, color);
        }

        public void DrawCircleFill(Vector2 center, float radius, int points, Color color)
        {
            this.DrawCircleFill(center.X, center.Y, radius, points, color);
        }

        public void DrawCircleFill(float x, float y, float radius, int points, Color color)
        {
            this.EnsureStarted();

            const int MinCirclePoints = 3;
            const int MaxCirclePoints = 256;

            int shapeVertexCount = FlatMath.Clamp(points, MinCirclePoints, MaxCirclePoints);    // The vertex count will just be the number of points requested by the user.
            int shapeTriangleCount = shapeVertexCount - 2;      // The triangle count of a convex polygon is alway 2 less than the vertex count.
            int shapeIndexCount = shapeTriangleCount * 3;       // The indices count will just be 3 times the triangle count.

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            float angle = MathHelper.TwoPi / (float)shapeVertexCount;
            float sin = MathF.Sin(angle);
            float cos = MathF.Cos(angle);

            int index = 1;

            // Indicies;
            for (int i = 0; i < shapeTriangleCount; i++)
            {
                this.indices[this.indexCount++] = 0 + this.vertexCount;
                this.indices[this.indexCount++] = index + this.vertexCount;
                this.indices[this.indexCount++] = index + 1 + this.vertexCount;

                index++;
            }

            // Vertices;

            float ax = radius;
            float ay = 0f;

            // Save all remaining vertices.
            for (int i = 0; i < shapeVertexCount; i++)
            {
                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(ax + x, ay + y, 0f), color);

                float bx = ax * cos - ay * sin;
                float by = ax * sin + ay * cos;

                ax = bx;
                ay = by;
            }

            this.shapeCount++;
        }

        public void DrawEllipseFill(FlatEllipse ellipse, int points, Color color)
        {
            this.DrawEllipseFill(ellipse.Center, ellipse.Radius, points, color);
        }

        public void DrawEllipseFill(float x, float y, float xRadius, float yRadius, int points, Color color)
        {
            this.DrawEllipseFill(new Vector2(x, y), new Vector2(xRadius, yRadius), points, color);
        }

        public void DrawEllipseFill(Vector2 center, Vector2 radius, int points, Color color)
        {
            /*
             * How to draw elipses:
             * https://community.khronos.org/t/how-to-draw-an-oval/13428
             */

            this.EnsureStarted();

            const int MinElipsePoints = 3;
            const int MaxElipsePoints = 256;

            int shapeVertexCount = FlatMath.Clamp(points, MinElipsePoints, MaxElipsePoints);    // The vertex count will just be the number of points requested by the user.
            int shapeTriangleCount = shapeVertexCount - 2;      // The triangle count of a convex polygon is alway 2 less than the vertex count.
            int shapeIndexCount = shapeTriangleCount * 3;       // The indices count will just be 3 times the triangle count.

            this.EnsureSpace(shapeVertexCount, shapeIndexCount);

            float deltaAngle = MathHelper.TwoPi / (float)shapeVertexCount;
            float angle = 0f;

            int index = 1;

            // Indicies;
            for (int i = 0; i < shapeTriangleCount; i++)
            {
                this.indices[this.indexCount++] = 0 + this.vertexCount;
                this.indices[this.indexCount++] = index + this.vertexCount;
                this.indices[this.indexCount++] = index + 1 + this.vertexCount;
            }

            // Vertices;

            // Save all remaining vertices.
            for (int i = 1; i < shapeVertexCount; i++)
            {
                float x = MathF.Cos(angle) * radius.X + center.X;
                float y = MathF.Sin(angle) * radius.Y + center.Y;

                angle += deltaAngle;

                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(x, y, 0f), color);
            }

            this.shapeCount++;
        }

        public void DrawPolygonFill(Vector2[] vertices, int[] triangles, FlatTransform transform, Color color)
        {
            this.EnsureStarted();
            this.EnsureSpace(vertices.Length, indices.Length);

            for (int i = 0; i < triangles.Length; i++)
            {
                this.indices[this.indexCount++] = this.vertexCount + triangles[i];
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 v = vertices[i];
                v = FlatUtil.Transform(v.X, v.Y, transform);
                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(v.X, v.Y, 0f), color);
            }

            this.shapeCount++;
        }

        public void DrawPolygonFill(Vector2[] vertices, int[] triangles, Color color)
        {
            this.EnsureStarted();
            this.EnsureSpace(vertices.Length, indices.Length);

            for (int i = 0; i < triangles.Length; i++)
            {
                this.indices[this.indexCount++] = this.vertexCount + triangles[i];
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 v = vertices[i];
                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(v.X, v.Y, 0f), color);
            }

            this.shapeCount++;
        }

        public void DrawPolygonFill(ReadOnlySpan<Vector2> vertices, int[] triangles, Color color)
        {
            this.EnsureStarted();
            this.EnsureSpace(vertices.Length, indices.Length);

            for (int i = 0; i < triangles.Length; i++)
            {
                this.indices[this.indexCount++] = this.vertexCount + triangles[i];
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 v = vertices[i];
                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(v.X, v.Y, 0f), color);
            }

            this.shapeCount++;
        }


        public void DrawPolygonFill(Span<Vector2> vertices, int[] triangles, FlatTransform transform, Color color)
        {
            this.EnsureStarted();
            this.EnsureSpace(vertices.Length, indices.Length);

            for (int i = 0; i < triangles.Length; i++)
            {
                this.indices[this.indexCount++] = this.vertexCount + triangles[i];
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 v = vertices[i];
                v = FlatUtil.Transform(v.X, v.Y, transform);
                this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(v.X, v.Y, 0f), color);
            }

            this.shapeCount++;
        }

        public void DrawLine_Slowest(Vector2 a, Vector2 b, Color color)
        {
            // Default thickness with no zoom.
            float thickness = 2f;

            // If we are using the world camera then we need to adjust the "thickness" of the line
            //  so no matter how far we have "zoomed" into the world the line will look the same.
            if (this.usingCamera)
            {
                thickness /= (float)this.camera.Zoom;
            }

            Vector2 edge = b - a;
            float angle = MathF.Atan2(edge.Y, edge.X);
            Vector2 center = new Vector2((a.X + b.X) / 2f, (a.Y + b.Y) / 2f);
            Vector2 scale = new Vector2((b - a).Length() + thickness, thickness);

            this.DrawBoxFill_Slow(center, 1f, 1f, angle, scale, color);
        }

        public void DrawLine_Slow(Vector2 a, Vector2 b, Color color)
        {
            this.DrawLine_Slow(a.X, a.Y, b.X, b.Y, color);
        }

        public void DrawLine_Slow(float ax, float ay, float bx, float by, Color color)
        {
            // Default thickness with not zoom.
            float thickness = 2f;

            // If we are using the world camera then we need to adjust the "thickness" of the line
            //  so no matter how far we have "zoomed" into the world the line will look the same.
            if (this.usingCamera)
            {
                thickness /= (float)this.camera.Zoom;
            }

            // Line edge pointing from "b" to "a".
            float ex = bx - ax;
            float ey = by - ay;

            // Angle of this edge.
            float angle = MathF.Atan2(ey, ex);

            // Center of the line segment.
            float cx = (ax + bx) * 0.5f;
            float cy = (ay + by) * 0.5f;

            // Length of the line segment.
            float len = MathF.Sqrt(ex * ex + ey * ey);

            // Scale required to make the line the right size.
            Vector2 scale = new Vector2(len + thickness, thickness);

            this.DrawBoxFill(new Vector2(cx, cy), 1f, 1f, angle, scale, color);
        }

        //public void DrawLine(Vector2 a, Vector2 b, Color color)
        //{
        //    this.DrawLine(a.X, a.Y, b.X, b.Y, color);
        //}

        public void DrawLine_Old(Vector2 a, Vector2 b, Color color)
        {
            // Default thickness with not zoom.
            float thickness = 2f;

            // If we are using the world camera then we need to adjust the "thickness" of the line
            //  so no matter how far we have "zoomed" into the world the line will look the same.
            if (this.usingCamera)
            {
                thickness /= (float)this.camera.Zoom;
            }

            float halfThickness = thickness / 2f;

            // Line edge pointing from "b" to "a".
            Vector2 e1 = b - a;
            Vector2 n1 = new Vector2(-e1.Y, e1.X);

            e1.Normalize();
            n1.Normalize();

            e1 *= halfThickness;
            n1 *= halfThickness;

            Vector2 e2 = -e1;
            Vector2 n2 = -n1;

            Vector2 q1 = a + n1 + e2;
            Vector2 q2 = b + n1 + e1;
            Vector2 q3 = b + n2 + e1;
            Vector2 q4 = a + n2 + e2;

            this.DrawQuadFill(q1, q2, q3, q4, color);
        }

        public void DrawLine(Vector2 a, Vector2 b, Color color)
        {
            this.DrawLine(a.X, a.Y, b.X, b.Y, color);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            // Default thickness with no zoom.
            float thickness = 2f;

            // If we are using the world camera then we need to adjust the "thickness" of the line
            //  so no matter how far we have "zoomed" into the world the line will look the same.
            if (this.usingCamera)
            {
                thickness /= (float)this.camera.Zoom;
            }

            float halfThickness = thickness * 0.5f;

            // Line edge pointing from "b" to "a".
            float e1x = x2 - x1;
            float e1y = y2 - y1;

            FlatMath.Normalize(ref e1x, ref e1y);

            float n1x = -e1y;
            float n1y = e1x;

            e1x *= halfThickness;
            e1y *= halfThickness;

            n1x *= halfThickness;
            n1y *= halfThickness;

            float e2x = -e1x;
            float e2y = -e1y;

            float n2x = -n1x;
            float n2y = -n1y;

            float qax = x1 + n1x + e2x;
            float qay = y1 + n1y + e2y;

            float qbx = x2 + n1x + e1x;
            float qby = y2 + n1y + e1y;

            float qcx = x2 + n2x + e1x;
            float qcy = y2 + n2y + e1y;

            float qdx = x1 + n2x + e2x;
            float qdy = y1 + n2y + e2y;

            this.DrawQuadFill(qax, qay, qbx, qby, qcx, qcy, qdx, qdy, color);
        }

        public void GetLine(float x1, float y1, float x2, float y2, Color color, VertexPositionColor[] vertices, int[] indices, ref int vertexCount, ref int indexCount)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException("vertices");
            }

            if (indices == null)
            {
                throw new ArgumentNullException("indices");
            }

            // Default thickness with not zoom.
            float thickness = 2f;

            // If we are using the world camera then we need to adjust the "thickness" of the line
            //  so no matter how far we have "zoomed" into the world the line will look the same.
            if (this.usingCamera)
            {
                thickness /= (float)this.camera.Zoom;
            }

            float halfThickness = thickness * 0.5f;

            // Line edge pointing from "b" to "a".
            float e1x = x2 - x1;
            float e1y = y2 - y1;

            FlatMath.Normalize(ref e1x, ref e1y);

            float n1x = -e1y;
            float n1y = e1x;

            e1x *= halfThickness;
            e1y *= halfThickness;

            n1x *= halfThickness;
            n1y *= halfThickness;

            float e2x = -e1x;
            float e2y = -e1y;

            float n2x = -n1x;
            float n2y = -n1y;

            float qax = x1 + n1x + e2x;
            float qay = y1 + n1y + e2y;

            float qbx = x2 + n1x + e1x;
            float qby = y2 + n1y + e1y;

            float qcx = x2 + n2x + e1x;
            float qcy = y2 + n2y + e1y;

            float qdx = x1 + n2x + e2x;
            float qdy = y1 + n2y + e2y;

            indices[indexCount++] = vertexCount + 0;
            indices[indexCount++] = vertexCount + 1;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount + 0;
            indices[indexCount++] = vertexCount + 2;
            indices[indexCount++] = vertexCount + 3;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(qax, qay, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(qbx, qby, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(qcx, qcy, 0f), color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(qdx, qdy, 0f), color);
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            this.DrawLine(a, b, color);
            this.DrawLine(b, c, color);
            this.DrawLine(c, a, color);
        }

        public void DrawBox(Vector2 min, Vector2 max, Color color)
        {
            this.DrawLine(min.X, max.Y, max.X, max.Y, color);
            this.DrawLine(max.X, max.Y, max.X, min.Y, color);
            this.DrawLine(max.X, min.Y, min.X, min.Y, color);
            this.DrawLine(min.X, min.Y, min.X, max.Y, color);
        }

        public void DrawBox(float x, float y, float width, float height, Color color)
        {
            Vector2 min = new Vector2(x, y);
            Vector2 max = new Vector2(x + width, y + height);

            this.DrawBox(min, max, color);
        }

        public void DrawBox(Vector2 center, float width, float height, Color color)
        {
            Vector2 min = new Vector2(center.X - width * 0.5f, center.Y - height * 0.5f);
            Vector2 max = new Vector2(min.X + width, min.Y + height);

            this.DrawBox(min, max, color);
        }

        public void DrawBox(Vector2 center, float width, float height, float angle, Color color)
        {
            float left = -width * 0.5f;
            float right = left + width;
            float bottom = -height * 0.5f;
            float top = bottom + height;

            // Precompute the trig. functions.
            float sin = MathF.Sin(angle);
            float cos = MathF.Cos(angle);

            // Vector components:

            float ax = left;
            float ay = top;
            float bx = right;
            float by = top;
            float cx = right;
            float cy = bottom;
            float dx = left;
            float dy = bottom;

            // Rotation transform:

            float rx1 = ax * cos - ay * sin;
            float ry1 = ax * sin + ay * cos;
            float rx2 = bx * cos - by * sin;
            float ry2 = bx * sin + by * cos;
            float rx3 = cx * cos - cy * sin;
            float ry3 = cx * sin + cy * cos;
            float rx4 = dx * cos - dy * sin;
            float ry4 = dx * sin + dy * cos;

            // Translation transform:

            ax = rx1 + center.X;
            ay = ry1 + center.Y;
            bx = rx2 + center.X;
            by = ry2 + center.Y;
            cx = rx3 + center.X;
            cy = ry3 + center.Y;
            dx = rx4 + center.X;
            dy = ry4 + center.Y;

            this.DrawLine(ax, ay, bx, by, color);
            this.DrawLine(bx, by, cx, cy, color);
            this.DrawLine(cx, cy, dx, dy, color);
            this.DrawLine(dx, dy, ax, ay, color);
        }

        public void DrawCircle(in FlatCircle circle, int points, Color color)
        {
            this.DrawCircle(circle.Center, circle.Radius, points, color);
        }

        public void DrawCircle(Vector2 center, float radius, int points, Color color)
        {
            this.DrawCircle(center.X, center.Y, radius, points, color);
        }

        public void DrawCircle(float x, float y, float radius, int points, Color color)
        {
            const int MinCirclePoints = 3;
            const int MaxCirclePoints = 256;

            points = FlatMath.Clamp(points, MinCirclePoints, MaxCirclePoints);

            float angle = MathHelper.TwoPi / (float)points;

            // Precalculate the trig. functions.
            float sin = MathF.Sin(angle);
            float cos = MathF.Cos(angle);

            // Initial point location on the unit circle.
            float ax = radius;
            float ay = 0f;

            for (int i = 0; i < points; i++)
            {
                // Perform a 2D rotation transform to get the next point on the circle.
                float bx = ax * cos - ay * sin;
                float by = ax * sin + ay * cos;

                this.DrawLine(ax + x, ay + y, bx + x, by + y, color);

                // Save the last transform for the next transform in the loop.
                ax = bx;
                ay = by;
            }
        }

        public void GetCircle(float radius, int points, Color color, out VertexPositionColor[] vertices, out int[] indices)
        {
            const int MinCirclePoints = 3;
            const int MaxCirclePoints = 256;

            points = FlatMath.Clamp(points, MinCirclePoints, MaxCirclePoints);

            int totalVertexCount = points * 4;
            int totalIndexCount = totalVertexCount * 3 / 2;
            int vertexCount = 0;
            int indexCount = 0;

            vertices = new VertexPositionColor[totalVertexCount];
            indices = new int[totalIndexCount];

            float angle = MathHelper.TwoPi / (float)points;

            // Precalculate the trig. functions.
            float sin = MathF.Sin(angle);
            float cos = MathF.Cos(angle);

            // Initial point location on the unit circle.
            float ax = radius;
            float ay = 0f;

            for (int i = 0; i < points; i++)
            {
                // Perform a 2D rotation transform to get the next point on the circle.
                float bx = ax * cos - ay * sin;
                float by = ax * sin + ay * cos;

                this.GetLine(ax, ay, bx, by, color, vertices, indices, ref vertexCount, ref indexCount);

                // Save the last transform for the next transform in the loop.
                ax = bx;
                ay = by;
            }
        }

        public void DrawEllipse(FlatEllipse ellipse, int points, Color color)
        {
            this.DrawEllipse(ellipse.Center, ellipse.Radius, points, color);
        }

        public void DrawEllipse(float x, float y, float xRadius, float yRadius, int points, Color color)
        {
            this.DrawEllipse(new Vector2(x, y), new Vector2(xRadius, yRadius), points, color);
        }

        public void DrawEllipse(Vector2 center, Vector2 radius, int points, Color color)
        {
            /*
             * How to draw elipses:
             * https://community.khronos.org/t/how-to-draw-an-oval/13428
             */

            const int MinPoints = 3;
            const int MaxPoints = 256;

            points = FlatMath.Clamp(points, MinPoints, MaxPoints);

            float deltaAngle = MathHelper.TwoPi / (float)points;
            float angle = 0f;

            float x1 = MathF.Cos(angle) * radius.X + center.X;
            float y1 = MathF.Sin(angle) * radius.Y + center.Y;

            for (int i = 0; i < points; i++)
            {
                angle += deltaAngle;

                float x2 = MathF.Cos(angle) * radius.X + center.X;
                float y2 = MathF.Sin(angle) * radius.Y + center.Y;

                this.DrawLine(x1, y1, x2, y2, color);

                x1 = x2;
                y1 = y2;
            }

            // Debugging: Draw the center of the elipse.
            //this.DrawBoxFill(center, 4, 4, color);
        }

        public void DrawPolygon_Slow(Vector2[] vertices, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            if (vertices is null || vertices.Length < 3)
            {
                return;
            }

            FlatTransform transform = new FlatTransform(position, rotation, scale);
            int len = vertices.Length;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % len];

                a = FlatUtil.Transform(a, transform);
                b = FlatUtil.Transform(b, transform);

                this.DrawLine_Slow(a, b, color);
            }
        }

        //public void DrawPolygon(PolygonShape polygon, Vector2 position, float rotation, Vector2 scale, Color color)
        //{
        //    this.DrawPolygon(polygon.Vertices, position, rotation, scale, color);
        //}

        public void DrawPolygon(Vector2[] vertices, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            if (vertices is null || vertices.Length < 3)
            {
                return;
            }

            // Precalculate the trig. functions.
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            // Get first vertex.
            Vector2 v = vertices[0];

            // Scale
            float sx = v.X * scale.X;
            float sy = v.Y * scale.Y;

            // Rotate
            float rx = sx * cos - sy * sin;
            float ry = sx * sin + sy * cos;

            // Translate
            float x1 = rx + position.X;
            float y1 = ry + position.Y;

            // Save these first coordinates to use at the end when drawing the final line segment.
            float firstX = x1;
            float firstY = y1;

            // Now perform the rest of the vertex transforms and draw a line between each, 
            //  except for the final line segment that connects back to the first.
            for (int i = 1; i < vertices.Length; i++)
            {
                v = vertices[i];

                // Scale
                sx = v.X * scale.X;
                sy = v.Y * scale.Y;

                // Rotate
                rx = sx * cos - sy * sin;
                ry = sx * sin + sy * cos;

                // Translate
                float x2 = rx + position.X;
                float y2 = ry + position.Y;

                this.DrawLine(x1, y1, x2, y2, color);

                // Make the 2nd vertex the first vertex for the next line.
                x1 = x2;
                y1 = y2;
            }

            // Draw the final line segment that connects back to the first.
            this.DrawLine(x1, y1, firstX, firstY, color);
        }

        public void DrawPolygon(Vector2[] vertices, FlatTransform transform, Color color)
        {
            if (vertices is null || vertices.Length < 3)
            {
                return;
            }

            // Now perform the rest of the vertex transforms and draw a line between each, 
            //  except for the final line segment that connects back to the first.
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % vertices.Length];

                a = FlatUtil.Transform(a, transform);
                b = FlatUtil.Transform(b, transform);

                this.DrawLine(a, b, color);
            }
        }

        public void DrawPolygon(Vector2[] vertices, Color color)
        {
            if (vertices is null || vertices.Length < 3)
            {
                return;
            }

            // Now perform the rest of the vertex transforms and draw a line between each, 
            //  except for the final line segment that connects back to the first.
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % vertices.Length];

                this.DrawLine(a, b, color);
            }
        }


        public void DrawPolygon(ReadOnlySpan<Vector2> vertices, FlatTransform transform, Color color)
        {
            if (vertices.Length < 3)
            {
                return;
            }

            int prev = vertices.Length - 1;
            int next = 0;

            // Now perform the rest of the vertex transforms and draw a line between each, 
            //  except for the final line segment that connects back to the first.
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = vertices[prev];
                Vector2 b = vertices[next];

                prev = next;
                next++;

                a = FlatUtil.Transform(a, transform);
                b = FlatUtil.Transform(b, transform);

                this.DrawLine(a, b, color);
            }
        }

        public void DrawPolygon(ReadOnlySpan<Vector2> vertices, Color color)
        {
            if (vertices.Length < 3)
            {
                return;
            }

            int prev = vertices.Length - 1;
            int next = 0;

            // Now perform the rest of the vertex transforms and draw a line between each, 
            //  except for the final line segment that connects back to the first.
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = vertices[prev];
                Vector2 b = vertices[next];

                prev = next;
                next++;

                this.DrawLine(a, b, color);
            }
        }


        public void DrawPolygonTriangles(Vector2[] vertices, int[] triangles, FlatTransform transform, Color color)
        {
            if(vertices is null || vertices.Length < 3 || triangles is null || triangles.Length < 3)
            {
                return;
            }

            for(int i = 0; i < triangles.Length; i += 3)
            {
                Vector2 a = vertices[triangles[i]];
                Vector2 b = vertices[triangles[i + 1]];
                Vector2 c = vertices[triangles[i + 2]];

                a = FlatUtil.Transform(a, transform);
                b = FlatUtil.Transform(b, transform);
                c = FlatUtil.Transform(c, transform);

                this.DrawLine(a, b, color);
                this.DrawLine(b, c, color);
                this.DrawLine(c, a, color);
            }
        }

    }
}
