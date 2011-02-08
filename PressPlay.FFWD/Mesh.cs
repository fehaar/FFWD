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
    public class Mesh : Asset
    {
        public string asset { get; set; }

        [ContentSerializerIgnore]
        public Model model;
        [ContentSerializerIgnore]
        public CpuSkinnedModel skinnedModel;
        private int meshIndex;

        [ContentSerializerIgnore]
        public Microsoft.Xna.Framework.Vector3[] vertices;
        [ContentSerializerIgnore]
        public Microsoft.Xna.Framework.Vector3[] normals;
        [ContentSerializerIgnore]
        public Microsoft.Xna.Framework.Vector2[] uv;
        [ContentSerializerIgnore]
        public short[] triangles;

        internal BoundingSphere boundingSphere;

        internal override void LoadAsset(AssetHelper assetHelper)
        {
            // TODO: Optimize this by bundling everything into the same structure.
            if (!String.IsNullOrEmpty(asset))
            {
                skinnedModel = assetHelper.Load<CpuSkinnedModel>("Models/" + asset);
                if (skinnedModel != null)
                {
                    boundingSphere = skinnedModel.BoundingSphere;
                    for (int i = 0; i < skinnedModel.Parts.Count; i++)
                    {
                        if (skinnedModel.Parts[i].name == name)
                        {
                            meshIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    model = assetHelper.Load<Model>("Models/" + asset);
                    if (model != null)
                    {
                        for (int i = 0; i < model.Meshes.Count; i++)
                        {
                            if (model.Meshes[i].Name == name)
                            {
                                meshIndex = i;
                                boundingSphere = model.Meshes[i].BoundingSphere;
                                break;
                            }
                        }
                    }
                    else
                    {
                        MeshDataContent data = assetHelper.Load<MeshDataContent>("Models/" + asset);
                        if (data != null)
                        {
                            // This is hardcoded to make it work. The uvs and tris from the Mesh seems broken. Uhh...
                            vertices = data.vertices;
                            //triangles = new short[6] { 2, 0, 1, 2, 1, 3 };
                            triangles = data.triangles;
                            uv = new Microsoft.Xna.Framework.Vector2[4] {
                                new Microsoft.Xna.Framework.Vector2(0, 0),
                                new Microsoft.Xna.Framework.Vector2(1, 0),
                                new Microsoft.Xna.Framework.Vector2(0, 1),
                                new Microsoft.Xna.Framework.Vector2(1, 1)
                            };
                            normals = data.normals;
                        }
                        else
                        {
                            Debug.LogWarning("Cannot find a way to load the mesh " + asset);
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

        internal CpuSkinnedModelPart GetSkinnedModelPart()
        {
            if (skinnedModel != null)
            {
                return skinnedModel.Parts[meshIndex];
            }
            return null;
        }
    }
}
