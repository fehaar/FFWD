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
        public VertexPositionNormalTexture[] vertices;
        public VertexPositionNormalDualTexture[] lightmappedVertices;
        public short[][] indices;

        public VertexBuffer vertexBuffer;
        public IndexBuffer[] indexBuffer;
        public bool visible;

        public bool useLightMap;

        private int vertexIndex;

        public bool InitializeArray(int size)
        {
            if (vertexIndex + size > UInt16.MaxValue)
            {
                return false;
            }
            if (useLightMap)
            {
                MakeRoomInArray(ref lightmappedVertices, size);
            }
            else
            {
                MakeRoomInArray(ref vertices, size);
            }
            return true;
        }

        private void MakeRoomInArray<T>(ref T[] arr, int size)
        {
            if (arr == null)
            {
                arr = new T[size];
            }
            else
            {
                T[] oldVerts = arr;
                arr = new T[lightmappedVertices.Length + size];
                oldVerts.CopyTo(arr, 0);
            }
        }

        public void AddVertex(Vector3 position, Vector3 normal, Vector2 tex0, Vector2 tex1)
        {
            boundingBox.Min = Vector3.Min(boundingBox.Min, position);
            boundingBox.Max = Vector3.Max(boundingBox.Max, position);
            if (useLightMap)
            {
                lightmappedVertices[vertexIndex++] = new VertexPositionNormalDualTexture()
                {
                    Position = position,
                    Normal = normal,
                    TextureCoordinate = tex0,
                    TextureCoordinate2 = tex1
                };
            }
            else
            {
                vertices[vertexIndex++] = new VertexPositionNormalTexture()
                {
                    Position = position,
                    Normal = normal,
                    TextureCoordinate = tex0
                };
            }
        }
    }

    public class StaticBatchRenderer : Renderer
    {
        internal List<SMeshInfo> tmpMeshes;

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
            for (UInt32 v = 0; v < tilesV; ++v)
            {
                for (UInt32 u = 0; u < tilesU; ++u)
                {
                    if (quadTreeTiles[uTile].vertices != null &&
                        quadTreeTiles[uTile].indices != null)
                    {
                        quadTreeTiles[uTile].vertexBuffer = new VertexBuffer(Application.Instance.GraphicsDevice, quadTreeTiles[uTile].vertices.GetType().GetElementType(), quadTreeTiles[uTile].vertices.Length, BufferUsage.WriteOnly);
                        quadTreeTiles[uTile].vertexBuffer.SetData(quadTreeTiles[uTile].vertices);
                        quadTreeTiles[uTile].indexBuffer = new IndexBuffer[quadTreeTiles[uTile].indices.Length];
                        for (int i = 0; i < quadTreeTiles[uTile].indices.Length; i++)
                        {
                            quadTreeTiles[uTile].indexBuffer[i] = new IndexBuffer(Application.Instance.GraphicsDevice, IndexElementSize.SixteenBits, quadTreeTiles[uTile].indices[i].Length, BufferUsage.WriteOnly);
                            quadTreeTiles[uTile].indexBuffer[i].SetData(quadTreeTiles[uTile].indices[i]);
                        }
                        if (sharedMaterials.Length > quadTreeTiles[uTile].indexBuffer.Length)
                        {
                            throw new Exception("The static batch renderer does not have enough submeshes for all materials!");
                        }
                    }
                    ++uTile;
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

                quadTreeTiles[uTileIdx].useLightMap = false; //(meshInfo.renderer.lightmapIndex > -1);

                int vertexOffset = 0;
                if (!quadTreeTiles[uTileIdx].InitializeArray(meshInfo.mesh._vertices.Length))
                {
                    return false;
                }
                for (int i = 0; i < meshInfo.mesh._vertices.Length; i++)
                {
                    quadTreeTiles[uTileIdx].AddVertex(
                        Microsoft.Xna.Framework.Vector3.Transform(meshInfo.mesh._vertices[i], meshInfo.renderer.transform.world),
                        Microsoft.Xna.Framework.Vector3.Normalize(Microsoft.Xna.Framework.Vector3.TransformNormal(meshInfo.mesh._normals[i], meshInfo.renderer.transform.world)),
                        meshInfo.mesh._uv[i],
                        Vector2.zero
                        );
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

            cam.BasicEffect.World = Matrix.Identity;
            cam.BasicEffect.VertexColorEnabled = false;
            cam.BasicEffect.LightingEnabled = Light.HasLights;

            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                sharedMaterials[i].SetTextureState(cam.BasicEffect);
                sharedMaterials[i].SetBlendState(device);

                foreach (EffectPass pass in cam.BasicEffect.CurrentTechnique.Passes)
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
