using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
    public class Mesh
    {
        public string name { get; set; }
        public string asset { get; set; }

        [ContentSerializerIgnore]
        public Model model;
        private int meshIndex;

        [ContentSerializerIgnore]
        public Vector3[] vertices { get; set; }
        [ContentSerializerIgnore]
        public Vector3[] normals { get; set; }
        [ContentSerializerIgnore]
        public Vector2[] uv { get; set; }
        [ContentSerializerIgnore]
        public short[] triangles { get; set; }

        public void Awake()
        {
            ContentHelper.LoadModel(asset);
        }

        public void Start()
        {
            model = ContentHelper.GetModel(asset);
            if (model != null)
            {
                for (int i = 0; i < model.Meshes.Count; i++)
                {
                    if (model.Meshes[i].Name == name)
                    {
                        meshIndex = i;
                        break;
                    }
                }
            }
        }

        public void Clear()
        {
            vertices = null;
            normals = null;
            uv = null;
            triangles = null;
        }

        internal ModelMesh GetModelMesh()
        {
            if (model != null)
            {
                return model.Meshes[meshIndex];
            }
            return null;
        }
    }
}
