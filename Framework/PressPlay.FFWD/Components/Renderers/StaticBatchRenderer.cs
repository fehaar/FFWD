using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Extensions;

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

        public bool useLightMap;

        public int vertexIndex;
        public int batches;

        public bool InitializeArray(int size)
        {
            batches++;
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

        internal void ComputeBoundingBox()
        {
            if (vertices == null)
            {
                return;
            }
            List<Microsoft.Xna.Framework.Vector3> points = new List<Microsoft.Xna.Framework.Vector3>(vertices.Length);
            if (useLightMap)
            {
                points.AddRange(vertices.Cast<VertexPositionNormalDualTexture>().Select(v => v.Position));
            }
            else
            {
                points.AddRange(vertices.Cast<VertexPositionNormalTexture>().Select(v => v.Position));
            }
            boundingBox = BoundingBox.CreateFromPoints(points);
        }
    }

    public class StaticBatchRenderer : Renderer
    {
        internal List<SMeshInfo> tmpMeshes;
        [ContentSerializer]
        private int vertexCount;
        private static DualTextureEffect dtEffect;
        
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

            renderQueue /= 10;

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
                if (dtEffect == null)
                {
                    dtEffect = new DualTextureEffect(Application.Instance.GraphicsDevice);
                    dtEffect.VertexColorEnabled = false;
                }
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
                vertexCount += m._vertices.Length;
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
            float tileSize = ApplicationSettings.DefaultValues.StaticBatchTileSize;
            // If we have a small static batch, do not cut it up.
            if (vertexCount <= ApplicationSettings.DefaultValues.StaticBatchVertexLimit)
            {
                tilesU = tilesV = 1;
            }
            else
            {
                float fTilesU = (boundingBox.Max.X - boundingBox.Min.X) / tileSize;
                float fTilesV = (boundingBox.Max.Z - boundingBox.Min.Z) / tileSize;
                tilesU = Math.Max((UInt32)Mathf.CeilToInt(fTilesU), 1);
                tilesV = Math.Max((UInt32)Mathf.CeilToInt(fTilesV), 1);
            }

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
                    quadTreeTiles[uTile].lightmapIndex = lightmapIndex;
                    quadTreeTiles[uTile].useLightMap = useLightMap;

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

                    Vector2 uv2 = (meshInfo.mesh._uv2.HasElements()) ? meshInfo.mesh._uv2[i] : meshInfo.mesh._uv[i];
                    if (quadTreeTiles[uTileIdx].useLightMap)
                    {
                        uv2 = new Vector2(
                            uv2.x * meshInfo.renderer.lightmapTilingOffset.x + meshInfo.renderer.lightmapTilingOffset.z,
                            1 - ((1 - uv2.y) * meshInfo.renderer.lightmapTilingOffset.y + meshInfo.renderer.lightmapTilingOffset.w));
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
            for (int i = 0; i < quadTreeTiles.Length; i++)
            {
                quadTreeTiles[i].ComputeBoundingBox();
            }

            tmpMeshes.Clear();

            return true;
        }

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            UInt32 tiles = tilesV * tilesU;
            for (UInt32 i = 0; i < tiles; ++i)
            {
                quadTreeTiles[i].visible = !cam.DoFrustumCulling(ref quadTreeTiles[i].boundingBox);
#if DEBUG
                if (Camera.logRenderCalls)
                {
                    if (!quadTreeTiles[i].visible)
                    {
                        Debug.LogFormat("VP Cull Static batch: Tile {0} on {1} on {2}", i, gameObject, cam.gameObject);
                    }
                }                
#endif
            }

            for (int i = 0; i < _sharedMaterials.Length; i++)
            {
                Material mat = _sharedMaterials[i];
                Effect effect = cam.BasicEffect;
                if (useLightMap)
	            {
                    effect = dtEffect;
	            }
#if DEBUG
                if (Camera.logRenderCalls)
                {
                    Debug.LogFormat("Static batch RQ({6}): Tile {0} go {1} cam {2}. Using {3} and material {4} and shader {5}", i, gameObject, cam.gameObject, effect.GetType().Name, mat, mat.shaderName, mat.finalRenderQueue);
                }
#endif

                (effect as IEffectMatrices).World = Matrix.Identity;
                (effect as IEffectMatrices).View = cam.view;
                (effect as IEffectMatrices).Projection = cam.projectionMatrix;

                if (effect is IEffectLights)
                {
                    (effect as IEffectLights).LightingEnabled = Light.HasLights;
                }

                if (effect is DualTextureEffect)
                {
                    (effect as DualTextureEffect).DiffuseColor = new Microsoft.Xna.Framework.Vector3(mat.color.r / 2, mat.color.g / 2, mat.color.b / 2);
                    (effect as DualTextureEffect).Alpha = mat.color.a;
                    (effect as DualTextureEffect).Texture = mat.mainTexture;
                    (effect as DualTextureEffect).Texture2 = LightmapSettings.lightmaps[lightmapIndex].lightmapFar;
                }
                if (effect is BasicEffect)
                {
                    mat.SetTextureState(effect as BasicEffect);
                }
                mat.SetBlendState(device);

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
                        RenderStats.AddDrawCall(quadTreeTiles[j].batches, quadTreeTiles[j].vertexBuffer.VertexCount, quadTreeTiles[j].indexBuffer[i].IndexCount / 3);
                    }
                }
            }
        }
    }
}
