using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Extensions;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    internal abstract class RenderItem
    {
        private string name;

        public Material Material;
        public Transform Transform;
        public Bounds? Bounds;
        protected int batches = -1;
        public int Priority;
        public bool Enabled;
        protected bool UseVertexColor = false;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        private int referenceCount = 0;

#if XBOX
        protected const int MAX_INDEX_BUFFER_SIZE = Int32.MaxValue;
        internal int[] indexData;
#else
        protected const int MAX_INDEX_SIZE = Int16.MaxValue;
        internal short[] indexData;
#endif

        public abstract bool AddMesh(Mesh mesh, Matrix matrix, int subMeshIndex);
        public abstract void Initialize(GraphicsDevice device);

        private static Dictionary<string, RenderItem> RenderItemPool = new Dictionary<string, RenderItem>();

        private static int hitPool = 0;
        private static int missPool = 0;

        internal static RenderItem Create(Material material, Mesh mesh, int subMeshIndex, Transform t)
        {
            RenderItem item;

            string id = material.GetInstanceID() + ":" + mesh.GetInstanceID();
            if (RenderItemPool.ContainsKey(id))
            {
                item = RenderItemPool[id];
                hitPool++;
            }
            else
            {
                missPool++;
                // TODO: The selection needs to be configurable from outside
                if (material.shaderName.StartsWith("Unlit") || !mesh._normals.HasElements())
                {
                    if (mesh.colors.HasElements())
                    {
                        item = new RenderItem<VertexPositionColorTexture>(material, AddVertexPositionColorTexture);
                    }
                    else
                    {
                        item = new RenderItem<VertexPositionTexture>(material, AddVertexPositionTexture);
                    }
                }
                else
                {
                    item = new RenderItem<VertexPositionNormalTexture>(material, AddVertexPositionNormalTexture);
                }

                item.Transform = t;
                item.Priority = material.shader.renderQueue;
                item.AddMesh(mesh, t.world, subMeshIndex);
                item.name = String.Format("{0} - {1} on {2} rendering {3}", item.Priority, item.Material.name, t.ToString(), mesh.name);
                RenderItemPool[id] = item;
            }

            item.AddReference();
            return item;
        }

        private void AddReference()
        {
            referenceCount++;
        }

        internal void RemoveReference()
        {
            referenceCount--;
        }

        private bool Alive()
        {
            return referenceCount > 0;
        }

        public void Render(GraphicsDevice device, Camera cam)
        {
            if (!Transform.renderer.enabled)
            {
                return;
            }

            device.SetVertexBuffer(VertexBuffer);
            device.Indices = IndexBuffer;

            Effect e = Material.shader.effect;
            Material.shader.ApplyPreRenderSettings(Material, UseVertexColor);
            Material.SetBlendState(device);

            IEffectMatrices ems = e as IEffectMatrices;
            if (ems != null)
            {
                ems.World = Transform.world;
                ems.View = cam.view;
                ems.Projection = cam.projectionMatrix;
            }
            foreach (EffectPass pass in e.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    VertexBuffer.VertexCount,
                    0,
                    IndexBuffer.IndexCount / 3
                );
            }
            RenderStats.AddDrawCall(batches, VertexBuffer.VertexCount, IndexBuffer.IndexCount / 3);
        }

        private static VertexPositionNormalTexture AddVertexPositionNormalTexture(Microsoft.Xna.Framework.Vector3 position, Microsoft.Xna.Framework.Vector3 normal, Microsoft.Xna.Framework.Vector2 tex0, Microsoft.Xna.Framework.Vector2 tex1, Microsoft.Xna.Framework.Color c)
        {
            return new VertexPositionNormalTexture(position, normal, tex0);
        }

        private static VertexPositionTexture AddVertexPositionTexture(Microsoft.Xna.Framework.Vector3 position, Microsoft.Xna.Framework.Vector3 normal, Microsoft.Xna.Framework.Vector2 tex0, Microsoft.Xna.Framework.Vector2 tex1, Microsoft.Xna.Framework.Color c)
        {
            return new VertexPositionTexture(position, tex0);
        }

        private static VertexPositionColorTexture AddVertexPositionColorTexture(Microsoft.Xna.Framework.Vector3 position, Microsoft.Xna.Framework.Vector3 normal, Microsoft.Xna.Framework.Vector2 tex0, Microsoft.Xna.Framework.Vector2 tex1, Microsoft.Xna.Framework.Color c)
        {
            return new VertexPositionColorTexture(position, c, tex0);
        }

        private static VertexPositionColor AddVertexPositionColor(Microsoft.Xna.Framework.Vector3 position, Microsoft.Xna.Framework.Vector3 normal, Microsoft.Xna.Framework.Vector2 tex0, Microsoft.Xna.Framework.Vector2 tex1, Microsoft.Xna.Framework.Color c)
        {
            return new VertexPositionColor(position, c);
        }

        public override string ToString()
        {
            return name;
        }
    }

    /// <summary>
    /// This contains an item that is to be rendered.
    /// </summary>
    internal class RenderItem<T> : RenderItem where T : struct
    {
        internal T[] vertexData;
        private AddVertex addVertex;

        public RenderItem(Material mat, AddVertex addV)
        {
            Material = mat;
            addVertex = addV;
        }

        public delegate T AddVertex(Microsoft.Xna.Framework.Vector3 position, Microsoft.Xna.Framework.Vector3 normal, Microsoft.Xna.Framework.Vector2 tex0, Microsoft.Xna.Framework.Vector2 tex1, Microsoft.Xna.Framework.Color c);

        /// <summary>
        /// Adds a mesh to be rendered by this render item.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public override bool AddMesh(Mesh mesh, Matrix matrix, int subMeshIndex)
        {
            if (!mesh._vertices.HasElements())
	        {
                return false;
	        }

            int vertexOffset = 0;
            if (vertexData == null)
            {
                if (mesh.vertices.Length > RenderItem.MAX_INDEX_SIZE)
                {
                    return false;
                }
                vertexData = new T[mesh.vertexCount];
            }
            else
            {
                vertexOffset = vertexData.Length;
                if (mesh.vertices.Length + vertexOffset > RenderItem.MAX_INDEX_SIZE)
                {
                    return false;
                }
                T[] oldVerts = vertexData;
                vertexData = new T[vertexOffset + mesh.vertexCount];
                oldVerts.CopyTo(vertexData, 0);
            }
            batches++;

            for (int i = 0; i < mesh._vertices.Length; i++)
            {
                // Modify UV coordinates for tiling
                Microsoft.Xna.Framework.Vector2 uv1 = new Vector2(
                        mesh._uv[i].X * Material.mainTextureScale.x + Material.mainTextureOffset.x,
                        1 - ((1 - mesh._uv[i].Y) * Material.mainTextureScale.y + Material.mainTextureOffset.y));

                vertexData[i + vertexOffset] = addVertex(mesh._vertices[i], (mesh._normals.HasElements()) ? mesh._normals[i] : Microsoft.Xna.Framework.Vector3.Zero, uv1, (mesh._uv2.HasElements()) ? mesh._uv2[i] : Microsoft.Xna.Framework.Vector2.Zero, (mesh.colors.HasElements()) ? mesh.colors[i] : Color.white);

            }
            if (Bounds.HasValue)
	        {
                Bounds.Value.Encapsulate(mesh.bounds);
	        }
            else
	        {
                Bounds = mesh.bounds;
            }

#if XBOX
            int[] 
#else
            short[]
#endif
            tris = mesh.GetTriangles(subMeshIndex);
            if (indexData == null)
            {
                indexData = tris.ToArray();
            }
            else
            {
#if XBOX
                int[] oldIndexData = indexData;
                indexData = new short[oldIndexData.Length + tris.Length];
#else
                short[] oldIndexData = indexData;
                indexData = new short[oldIndexData.Length + tris.Length];
#endif
                oldIndexData.CopyTo(indexData, 0);
                tris.CopyTo(indexData, oldIndexData.Length);
            }
            return true;
        }

        /// <summary>
        /// Sets up the VertexBuffers and IndexBuffers with the data.
        /// </summary>
        public override void Initialize(GraphicsDevice device)
        {
            if (VertexBuffer == null)
            {
                VertexBuffer = new VertexBuffer(device, typeof(T), vertexData.Length, BufferUsage.WriteOnly);
                VertexBuffer.SetData<T>(vertexData);
                vertexData = null;

#if XBOX
            IndexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indexData.Length, BufferUsage.WriteOnly);
#else
                IndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indexData.Length, BufferUsage.WriteOnly);
#endif
                IndexBuffer.SetData(indexData);
                indexData = null;
            }
        }
    }
}
