using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    internal struct SMeshInfo
    {
        public Mesh mesh;
        public Renderer renderer;
        public BoundingBox bbox;
    };

    internal struct SQuadTreeTile
    {
        public BoundingBox boundingBox;
        public Array vertices;
        public short[][] indices;

        public VertexBuffer vertexBuffer;
        public IndexBuffer[] indexBuffer;
        public bool visible;

        public int lightmapIndex;

        public bool useLightMap { get { return lightmapIndex > -1; } }

        public int vertexIndex;

        public bool InitializeArray(int size)
        {
            if (vertexIndex + size > UInt16.MaxValue)
            {
                return false;
            }
            if (useLightMap)
            {
                MakeRoomInArray<VertexPositionNormalDualTexture>(size);
            }
            else
            {
                MakeRoomInArray<VertexPositionNormalTexture>(size);
            }
            return true;
        }

        private void MakeRoomInArray<T>(int size)
        {
            if (vertices == null)
            {
                vertices = new T[size];
            }
            else
            {
                Array oldVerts = vertices;
                vertices = new T[oldVerts.Length + size];
                oldVerts.CopyTo(vertices, 0);
            }
        }

        public void AddVertex(Vector3 position, Vector3 normal, Vector2 tex0, Vector2 tex1)
        {
            boundingBox.Min = Vector3.Min(boundingBox.Min, position);
            boundingBox.Max = Vector3.Max(boundingBox.Max, position);
            object vert;
            if (useLightMap)
            {
                vert = new VertexPositionNormalDualTexture()
                {
                    Position = position,
                    Normal = normal,
                    TextureCoordinate = tex0,
                    TextureCoordinate2 = tex1
                };
            }
            else
            {
                vert = new VertexPositionNormalTexture()
                {
                    Position = position,
                    Normal = normal,
                    TextureCoordinate = tex0
                };
            }
            vertices.SetValue(vert, vertexIndex++);
        }

        internal void InitializeBuffers(GraphicsDevice graphicsDevice)
        {
            if (useLightMap)
            {
                InitializeBuffers<VertexPositionNormalDualTexture>(graphicsDevice);
            }
            else
            {
                InitializeBuffers<VertexPositionNormalTexture>(graphicsDevice);
            }
        }

        private void InitializeBuffers<T>(GraphicsDevice graphicsDevice) where T : struct
        {
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(T), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<T>(vertices.Cast<T>().ToArray());
            indexBuffer = new IndexBuffer[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indexBuffer[i] = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices[i].Length, BufferUsage.WriteOnly);
                indexBuffer[i].SetData(indices[i]);
            }
            vertices = null;
            indices = null;
        }
    }

    public class StaticBatchRenderer : Renderer
    {
        internal List<SMeshInfo> tmpMeshes;
        private Effect effect;
        private Microsoft.Xna.Framework.Graphics.Texture2D grey;
        
        [ContentSerializer]
        internal BoundingBox boundingBox;

        [ContentSerializer]
        internal SQuadTreeTile[] quadTreeTiles;

        [ContentSerializer]
        internal UInt32 tilesU;

        [ContentSerializer]
        internal UInt32 tilesV;

        public override void Awake()
        {
            base.Awake();

            UInt32 uTile = 0;
            bool hasLightMaps = false;
            for (UInt32 v = 0; v < tilesV; ++v)
            {
                for (UInt32 u = 0; u < tilesU; ++u)
                {
                    if (quadTreeTiles[uTile].vertices != null &&
                        quadTreeTiles[uTile].indices != null)
                    {
                        hasLightMaps |= quadTreeTiles[uTile].useLightMap;
                        quadTreeTiles[uTile].InitializeBuffers(Application.Instance.GraphicsDevice);
                        if (sharedMaterials.Length > quadTreeTiles[uTile].indexBuffer.Length)
                        {
                            throw new Exception("The static batch renderer does not have enough submeshes for all materials!");
                        }
                    }
                    ++uTile;
                }
            }
            if (Application.Instance != null)
            {
                if (hasLightMaps)
                {
                    DualTextureEffect dtEffect = new DualTextureEffect(Application.Instance.GraphicsDevice);
                    dtEffect.VertexColorEnabled = false;
                    effect = dtEffect;
                }
                else
                {
                    BasicEffect bEffect = new BasicEffect(Application.Instance.GraphicsDevice);
                    bEffect.VertexColorEnabled = false;
                    effect = bEffect;
                }
                grey = new Microsoft.Xna.Framework.Graphics.Texture2D(Application.Instance.GraphicsDevice, 1, 1);
                grey.SetData(new Microsoft.Xna.Framework.Color[] { new Microsoft.Xna.Framework.Color(128, 128, 128, 255) });
            }
        }

        internal bool AddMesh(Mesh m, Renderer renderer)
        {
            if (tmpMeshes == null)
            {
                tmpMeshes = new List<SMeshInfo>();
            }

            if (m == null)
            {
                throw new Exception("Trying to add null mesh to static batch");
            }
            if (m._vertices == null)
            {
                throw new Exception("Mesh " + m.name + " does not have any vertices and is being added to a static batch");
            }
            
            // build/update full bbox
            if (m._vertices.Length > 0)
            {
                SMeshInfo meshInfo = new SMeshInfo();
                meshInfo.mesh = m;
                meshInfo.renderer = renderer;
                meshInfo.bbox = new BoundingBox(Microsoft.Xna.Framework.Vector3.Transform(m.bounds.min, renderer.transform.world),
                                                  Microsoft.Xna.Framework.Vector3.Transform(m.bounds.max, renderer.transform.world));
                tmpMeshes.Add(meshInfo);
                if (tmpMeshes.Count == 0)
                {
                    boundingBox = meshInfo.bbox;
                }
                else
                {
                    boundingBox = BoundingBox.CreateMerged(boundingBox, meshInfo.bbox);
                }
                return true;
            }
            return false;
        }

        internal bool PrepareQuadTree()
        {
            // initialize quad tree
            float tileSize = 50.0f;
            float fTilesU = (boundingBox.Max.X - boundingBox.Min.X) / tileSize;
            float fTilesV = (boundingBox.Max.Z - boundingBox.Min.Z) / tileSize;

            tilesU = Math.Max((UInt32)Mathf.CeilToInt(fTilesU), 1);
            tilesV = Math.Max((UInt32)Mathf.CeilToInt(fTilesV), 1);

            quadTreeTiles = new SQuadTreeTile[tilesU * tilesV];

            UInt32 uTile = 0;
            Vector3 vMin = new Vector3(boundingBox.Min);
            Vector3 vInc = new Vector3(tileSize, boundingBox.Max.Y - boundingBox.Min.Y, tileSize);

            for (UInt32 v = 0; v < tilesV; ++v)
            {
                vMin.x = boundingBox.Min.X;
                for (UInt32 u = 0; u < tilesU; ++u)
                {
                    quadTreeTiles[uTile].boundingBox.Min = vMin;
                    quadTreeTiles[uTile].boundingBox.Max = vMin + vInc;

                    vMin.x += tileSize;

                    ++uTile;
                }

                vMin.z += tileSize;
            }

            // transform and clasify meshes
            foreach (SMeshInfo meshInfo in tmpMeshes)
            {
                // find tile
                float fTileU = ((meshInfo.bbox.Max.X + meshInfo.bbox.Min.X) * 0.5f - boundingBox.Min.X) / tileSize;
                float fTileV = ((meshInfo.bbox.Max.Z + meshInfo.bbox.Min.Z) * 0.5f - boundingBox.Min.Z) / tileSize;

                UInt32 uTileU = (UInt32)Mathf.FloorToInt(fTileU);
                UInt32 uTileV = (UInt32)Mathf.FloorToInt(fTileV);

                uTileU = Math.Max(0, Math.Min(uTileU, tilesU - 1));
                uTileV = Math.Max(0, Math.Min(uTileV, tilesV - 1));

                UInt32 uTileIdx = uTileV * tilesU + uTileU;

                quadTreeTiles[uTileIdx].lightmapIndex = meshInfo.renderer.lightmapIndex;
                lightmapIndex = quadTreeTiles[uTileIdx].lightmapIndex;

                int vertexOffset = quadTreeTiles[uTileIdx].vertexIndex;
                if (!quadTreeTiles[uTileIdx].InitializeArray(meshInfo.mesh._vertices.Length))
                {
                    return false;
                }
                for (int i = 0; i < meshInfo.mesh._vertices.Length; i++)
                {
                    Vector2 uv1 = new Vector2(
                            meshInfo.mesh._uv[i].X * meshInfo.renderer.material.mainTextureScale.x + meshInfo.renderer.material.mainTextureOffset.x,
                            1 - ((1 - meshInfo.mesh._uv[i].Y) * meshInfo.renderer.material.mainTextureScale.y + meshInfo.renderer.material.mainTextureOffset.y));

                    Vector2 uv2 = Vector2.zero;
                    if (quadTreeTiles[uTileIdx].useLightMap)
                    {
                        uv2 = new Vector2(
                            meshInfo.mesh._uv2[i].X * meshInfo.renderer.lightmapTilingOffset.x + meshInfo.renderer.lightmapTilingOffset.z,
                            1 - ((1 - meshInfo.mesh._uv2[i].Y) * meshInfo.renderer.lightmapTilingOffset.y + meshInfo.renderer.lightmapTilingOffset.w));
                    }

                    quadTreeTiles[uTileIdx].AddVertex(
                        Microsoft.Xna.Framework.Vector3.Transform(meshInfo.mesh._vertices[i], meshInfo.renderer.transform.world),
                        Microsoft.Xna.Framework.Vector3.Normalize(Microsoft.Xna.Framework.Vector3.TransformNormal(meshInfo.mesh._normals[i], meshInfo.renderer.transform.world)),
                        uv1,
                        uv2);
                }
                if (quadTreeTiles[uTileIdx].indices == null)
                {
                    quadTreeTiles[uTileIdx].indices = new short[meshInfo.mesh.subMeshCount][];
                }
                int indexOffset = 0;
                for (int i = 0; i < meshInfo.mesh.subMeshCount; i++)
                {
                    short[] tris = meshInfo.mesh.GetTriangles(i);
                    if (quadTreeTiles[uTileIdx].indices[i] == null)
                    {
                        quadTreeTiles[uTileIdx].indices[i] = (short[])tris.Clone();
                    }
                    else
                    {
                        short[] newTris = new short[quadTreeTiles[uTileIdx].indices[i].Length + tris.Length];
                        quadTreeTiles[uTileIdx].indices[i].CopyTo(newTris, 0);
                        indexOffset = quadTreeTiles[uTileIdx].indices[i].Length;
                        for (int t = 0; t < tris.Length; t++)
                        {
                            newTris[indexOffset + t] = (short)(tris[t] + vertexOffset);
                        }
                        quadTreeTiles[uTileIdx].indices[i] = newTris;
                    }
                }
            }

            tmpMeshes.Clear();

            return true;
        }

        public override int Draw(GraphicsDevice device, Camera cam)
        {
            int iAcum = 0;

            UInt32 tiles = tilesV * tilesU;
            for (UInt32 i = 0; i < tiles; ++i)
            {
                quadTreeTiles[i].visible = !cam.DoFrustumCulling(ref quadTreeTiles[i].boundingBox);

#if DEBUG
                if (Camera.logRenderCalls && quadTreeTiles[i].visible)
                {
                    Debug.LogFormat("Static batch: Tile {0} on {1} on {2}", i, gameObject, cam.gameObject);
                }
#endif
            }

            (effect as IEffectMatrices).World = Matrix.Identity;
            (effect as IEffectMatrices).View = cam.view;
            (effect as IEffectMatrices).Projection = cam.projectionMatrix;

            if (effect is IEffectLights)
            {
                (effect as IEffectLights).LightingEnabled = Light.HasLights;
            }

            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (effect is DualTextureEffect)
                {
                    (effect as DualTextureEffect).DiffuseColor = sharedMaterials[i].color;
                    (effect as DualTextureEffect).Alpha = sharedMaterials[i].color.a;
                    (effect as DualTextureEffect).Texture = sharedMaterials[i].mainTexture; // grey;
                    (effect as DualTextureEffect).Texture2 = LightmapSettings.lightmaps[lightmapIndex].lightmapFar; // grey;
                }
                else
                {
                    sharedMaterials[i].SetTextureState(effect as BasicEffect);
                }
                sharedMaterials[i].SetBlendState(device);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    for (UInt32 j = 0; j < tiles; ++j)
                    {
                        if (!quadTreeTiles[j].visible ||
                             quadTreeTiles[j].vertexBuffer == null ||
                             quadTreeTiles[j].indexBuffer == null)
                        {
                            continue;
                        }

                        device.SetVertexBuffer(quadTreeTiles[j].vertexBuffer);
                        device.Indices = quadTreeTiles[j].indexBuffer[i];

                        device.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            0,
                            0,
                            quadTreeTiles[j].vertexBuffer.VertexCount,
                            0,
                            quadTreeTiles[j].indexBuffer[i].IndexCount / 3
                        );

                        iAcum++;
                    }
                }
            }

            return iAcum;
        }
    }
}
