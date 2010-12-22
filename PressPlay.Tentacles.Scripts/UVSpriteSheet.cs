using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.Tentacles.Scripts {
	public class UVSpriteSheet : MonoBehaviour {

        public SkinnedMeshRenderer skinnedMeshRenderer;
        public MeshFilter meshFilter;

        protected Mesh mesh;
        protected Vector2[] baseUVs;
        protected Vector2[] tmpUVs;

        public int xCount;
        public int yCount;

        private int currentX = -1;
        private int currentY = -1;

        protected Vector2 tileSize;

        public bool scaleMeshUVsOnInitialize = true;

        protected Vector2 offset;

        protected bool isInitialized = false;

        public bool autoInitializeOnStart = true;

        private Texture2D[,] frames;

        public override void Start()
        {
            if (autoInitializeOnStart)
            {
                Initialize();
            }
        }

        // Use this for initialization
        public virtual void Initialize()
        {
            //Debug.Log("INITIALIZING UV SPRITE SHEET");
            if (isInitialized) { return; }
            isInitialized = true;

            tileSize = new Vector2(1f / (float)xCount, 1f / (float)yCount);

            if (skinnedMeshRenderer != null)
            {
                CreateTextureFrames();
            }
            else if (meshFilter != null)
            {
                Mesh tmpMesh = new Mesh();
                tmpMesh.vertices = meshFilter.sharedMesh.vertices;
                tmpMesh.uv = meshFilter.sharedMesh.uv;
                tmpMesh.normals = meshFilter.sharedMesh.vertices;
                tmpMesh.triangles = meshFilter.sharedMesh.triangles;
                meshFilter.sharedMesh = tmpMesh;
                mesh = meshFilter.sharedMesh;
            }

            if (mesh != null)
            {
                baseUVs = mesh.uv;
                tmpUVs = new Vector2[baseUVs.Length];

                for (int i = 0; i < baseUVs.Length; i++)
                {

                    if (scaleMeshUVsOnInitialize)
                    {
                        baseUVs[i].x = baseUVs[i].x / xCount;
                        baseUVs[i].y = baseUVs[i].y / yCount;
                    }

                    tmpUVs[i] = new Vector2(0, 0);
                }
            }

            currentX = -1;
            currentY = -1;

            UpdateUVs(0, 0);
        }

        private void CreateTextureFrames()
        {
            frames = new Texture2D[xCount, yCount];
            skinnedMeshRenderer.sharedMaterial.EndLoadContent();
            Texture2D spriteSheet = skinnedMeshRenderer.sharedMaterial.texture;
            int width = (int)((float)spriteSheet.Width * tileSize.x);
            int height = (int)((float)spriteSheet.Height * tileSize.y);
            for (int x = 0; x < xCount; x++)
			{
                for (int y = 0; y < yCount; y++)
			    {
                    Texture2D frame = new Texture2D(spriteSheet.GraphicsDevice, width, height);
                    Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width * height];
                    spriteSheet.GetData<Microsoft.Xna.Framework.Color>(0, new Microsoft.Xna.Framework.Rectangle(x * width, y * height, width, height), data, 0, width * height);
                    frame.SetData(data);
                    frames[x, y] = frame;
			    }
			}
        }

        // Update is called once per frame
        protected void UpdateUVs(int _x, int _y)
        {

            if (_x == currentX && _y == currentY) { return; }

            currentX = _x;
            currentY = _y;

            if (frames != null)
            {
                skinnedMeshRenderer.sharedMaterial.texture = frames[currentX, currentY];
            }
            else if (mesh != null)
            {
                mesh.uv = CreateUVs(currentX, currentY);
            }
        }

        protected Vector2[] CreateUVs(int _x, int _y)
        {
            offset = new Vector2(_x * tileSize.x, 1 - ((_y + 1) * tileSize.y));

            //Debug.Log("Creating UVs : "+_x+ ", "+_y + " offset : "+offset + " baseUVs.Length "+baseUVs.Length);

            for (int i = 0; i < baseUVs.Length; i++)
            {
                tmpUVs[i].x = baseUVs[i].x + offset.x;
                tmpUVs[i].y = baseUVs[i].y + offset.y;
            }

            return tmpUVs;
        }


        public int XPosFromIndex(int _index)
        {
            return _index % xCount;
        }

        public int YPosFromIndex(int _index)
        {
            return (int)(_index / xCount);
        }
    }
}