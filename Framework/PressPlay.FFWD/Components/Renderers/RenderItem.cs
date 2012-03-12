using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    internal abstract class RenderItem
    {
        public Shader Shader;
        public Material Material;
        public Bounds Bounds;
        public bool Enabled;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

#if XBOX
        private const int MAX_INDEX_BUFFER_SIZE = Int32.MaxValue;
        internal int[] indexData;
#else
        private const int MAX_INDEX_SIZE = Int16.MaxValue;
        internal short[] indexData;
#endif

        public abstract bool RenderMesh(Mesh mesh);
        public abstract void Initialize(GraphicsDevice device);
    }

    /// <summary>
    /// This contains an item that is to be rendered.
    /// </summary>
    internal class RenderItem<T> : RenderItem where T : struct
    {
        internal T[] vertexData;

        public RenderItem(Material mat, Shader s)
        {
            Material = mat;
            Shader = s;
        }

        /// <summary>
        /// Adds a mesh to be rendered by this render item.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public override bool RenderMesh(Mesh mesh)
        {
            return true;
        }

        /// <summary>
        /// Sets up the VertexBuffers and IndexBuffers with the data.
        /// </summary>
        public override void Initialize(GraphicsDevice device)
        {
            VertexBuffer = new VertexBuffer(device, typeof(T), vertexData.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<T>(vertexData);

            // Add to the global render queue
        }
    }
}
