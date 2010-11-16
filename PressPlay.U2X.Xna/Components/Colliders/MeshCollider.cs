using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.U2X.Xna.Components
{
    public class MeshCollider : Component
    {
        #region ContentProperties
        public string Material { get; set; }
        public bool IsTrigger { get; set; }
        public string Mesh { get; set; }
        #endregion

        private int meshIndex = 0;

        public override void Awake()
        {
            ContentHelper.LoadModel(Mesh);
        }

        public override void Start()
        {
            Model model = ContentHelper.GetModel(Mesh) ?? FindSubmeshInParents();
            if (model != null)
            {
                // Build the collider
                BuildMeshCollider(model.Meshes[meshIndex]);
            }
        }

        private Model FindSubmeshInParents()
        {
            MeshRenderer[] renderers = gameObject.GetComponentsInParents<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].model != null)
                {
                    for (int j = 0; j < renderers[i].model.Meshes.Count; j++)
                    {
                        if (renderers[i].model.Meshes[j].Name == Mesh)
                        {
                            meshIndex = j;
                            return renderers[i].model;
                        }
                    }
                }
            }
            return null;
        }

        private void BuildMeshCollider(ModelMesh modelMesh)
        {
            ModelMeshPart part = modelMesh.MeshParts[0];
            Int16[] indices = new Int16[part.NumVertices];
            part.IndexBuffer.GetData(indices, part.StartIndex, part.NumVertices);
            int i = 0;
        }
    }
}
