//using System;
//using System.Runtime.CompilerServices;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Flat.Graphics
//{
//    public class Model2D : IDisposable
//    {
//        private bool disposed;
//        private GraphicsDevice device;
//        private VertexBuffer vertexBuffer;
//        private IndexBuffer indexBuffer;
//        private BasicEffect effect;

//        private Model2D(GraphicsDevice device, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
//        {
//            this.disposed = false;
//            this.device = device;
//            this.vertexBuffer = vertexBuffer;
//            this.indexBuffer = indexBuffer;

//            this.effect = new BasicEffect(this.device);
//            this.effect.FogEnabled = false;
//            this.effect.TextureEnabled = false;
//            this.effect.VertexColorEnabled = true;
//            this.effect.LightingEnabled = false;
//            this.effect.View = Matrix.Identity;
//            this.effect.Projection = Matrix.Identity;
//            this.effect.World = Matrix.Identity;
//        }

//        public void Dispose()
//        {
//            if(this.disposed)
//            {
//                return;
//            }

//            this.effect?.Dispose();
//            this.indexBuffer?.Dispose();
//            this.vertexBuffer?.Dispose();
//        }

//        public static bool Create(GraphicsDevice device, VertexPositionColor[] vertices, int[] indices, out Model2D model, out string errorMsg)
//        {
//            model = null;
//            errorMsg = string.Empty;

//            if(device is null)
//            {
//                errorMsg = "Argument \"device\" is null.";
//                return false;
//            }

//            if(vertices is null)
//            {
//                errorMsg = "Argument \"vertices\" does not contain any data.";
//                return false;
//            }

//            if(indices is null)
//            {
//                errorMsg = "Argument \"indices\" does not contain any data.";
//                return false;
//            }

//            if(vertices.Length < 3)
//            {
//                errorMsg = "\"vertices\" must contain at least 3 elements.";
//                return false;
//            }

//            if(indices.Length < 3)
//            {
//                errorMsg = "\"indices\" must contain at least 3 elements.";
//                return false;
//            }

//            VertexBuffer vertexBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
//            vertexBuffer.SetData(vertices);

//            IndexBuffer indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
//            indexBuffer.SetData(indices);

//            model = new Model2D(device, vertexBuffer, indexBuffer);

//            return true;
//        }

//        public void Draw(Vector2 position, float rotation, float scale, Camera camera)
//        {
//            position.X -= camera.Position.X;
//            position.Y -= camera.Position.Y;

//            Matrix transform = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position.X, position.Y, 0f);
            
//            this.effect.View = camera.View;
//            this.effect.Projection = camera.Projection;
//            this.effect.World = transform;

//            this.device.SetVertexBuffer(this.vertexBuffer);
//            this.device.Indices = this.indexBuffer;

//            RasterizerState rasterizerState = new RasterizerState();
//            rasterizerState.CullMode = CullMode.None;
//            this.device.RasterizerState = rasterizerState;


//            int primitiveCount = this.indexBuffer.IndexCount / 3;

//            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
//            {
//                pass.Apply();
//                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);
//            }

//        }
//    }
//}
