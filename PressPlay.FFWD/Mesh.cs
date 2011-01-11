using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.SkinnedModel;

namespace PressPlay.FFWD
{
    public class Mesh
    {
        public string name { get; set; }
        public string asset { get; set; }

        [ContentSerializerIgnore]
        public Model model;
        [ContentSerializerIgnore]
        public CpuSkinnedModel skinnedModel;
        private int meshIndex;

        [ContentSerializerIgnore]
        public Vector3[] vertices { get; set; }
        [ContentSerializerIgnore]
        public Vector3[] normals { get; set; }
        [ContentSerializerIgnore]
        public Vector2[] uv { get; set; }
        [ContentSerializerIgnore]
        public short[] triangles { get; set; }

        public void Awake(bool skinnedModel)
        {
            ContentHelper.LoadModel(asset, skinnedModel);
        }

        public void Start(bool isSkinnedModel)
        {
            if (isSkinnedModel)
            {
                skinnedModel = ContentHelper.GetSkinnedModel(asset);
            }
            else
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
