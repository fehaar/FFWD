using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Import.Animation;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Import
{
    [ContentProcessor(DisplayName = "FFWD - Mesh processor")]
    class MeshProcessor : ContentProcessor<NodeContent, MeshDataContent>
    {
        public MeshProcessor()
        {
            ReadNormals = true;
            ReadUVs = true;
            WriteAsModel = false;
            Scale = 1.0f;
        }

        [DefaultValue(false)]
        [Description("Shall we write this as an FBX model or as mesh data.")]
        public bool WriteAsModel { get; set; }

        [DefaultValue(true)]
        [Description("Shall we read the normals of the model.")]
        public bool ReadNormals { get; set; }

        [DefaultValue(true)]
        [Description("Shall we read the uvs of the model.")]
        public bool ReadUVs { get; set; }

        [DefaultValue(1.0f)]
        [Description("The scale of the model in the game.")]
        public float Scale { get; set; }

        [DisplayName("Rotation X")]
        [DefaultValue(0.0f)]
        [Description("The rotation of the model in the game.")]
        public float RotationX { get; set; }
        [DisplayName("Rotation Y")]
        [DefaultValue(0.0f)]
        [Description("The rotation of the model in the game.")]
        public float RotationY { get; set; }
        [DisplayName("Rotation Z")]
        [DefaultValue(0.0f)]
        [Description("The rotation of the model in the game.")]
        public float RotationZ { get; set; }

        private MeshDataContent meshData;
        private Matrix preTransform;

        public override MeshDataContent Process(NodeContent input, ContentProcessorContext context)
        {
            meshData = new MeshDataContent();

            if (SkinningHelpers.MeshHasSkinning(input))
            {
                // This is a skinned model, so treat is as one
                CpuSkinnedModelProcessor proc = new CpuSkinnedModelProcessor();
                proc.RotationX = this.RotationX;
                proc.RotationY = this.RotationY;
                proc.RotationZ = this.RotationZ;
                proc.Scale = this.Scale;
                meshData.skinnedModel = proc.Process(input, context);
                meshData.boundingSphere = meshData.skinnedModel.BoundingSphere;
            }
            else
            {
                if (WriteAsModel)
                {
                    ModelProcessor proc = new ModelProcessor();
                    proc.RotationX = this.RotationX;
                    proc.RotationY = this.RotationY;
                    proc.RotationZ = this.RotationZ;
                    proc.Scale = this.Scale;
                    meshData.model = proc.Process(input, context);
                }
                else
                {
                    Microsoft.Xna.Framework.Quaternion rotation = Microsoft.Xna.Framework.Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(RotationY), MathHelper.ToRadians(RotationX), MathHelper.ToRadians(RotationZ));
                    preTransform = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(rotation);

                    if (input.Name == "sprite_square")
                    {
                        ProcessSpriteSquareMesh(input as MeshContent);
                    }
                    else
                    {
                        ProcessNode(input);
                    }
                }
            }
            return meshData;
        }

        private void ProcessSpriteSquareMesh(MeshContent input)
        {
            MeshDataPart mesh = new MeshDataPart();

            Microsoft.Xna.Framework.Vector3[] verts = new Microsoft.Xna.Framework.Vector3[input.Positions.Count];
            mesh.vertices = new Microsoft.Xna.Framework.Vector3[input.Positions.Count];
            input.Positions.CopyTo(verts, 0);
            Microsoft.Xna.Framework.Vector3.Transform(verts, ref preTransform, mesh.vertices);

            // This is hardcoded to make it work. The uvs and tris from the Mesh seems broken. Uhh...
            mesh.triangles = new short[6] { 2, 0, 1, 2, 1, 3 };
            mesh.uv = new Microsoft.Xna.Framework.Vector2[4] {
                                new Microsoft.Xna.Framework.Vector2(0, 0),
                                new Microsoft.Xna.Framework.Vector2(1, 0),
                                new Microsoft.Xna.Framework.Vector2(0, 1),
                                new Microsoft.Xna.Framework.Vector2(1, 1)
                            };

            if (ReadNormals && input.Geometry[0].Vertices.Channels.Contains(VertexChannelNames.Normal(0)))
            {
                VertexChannel<Microsoft.Xna.Framework.Vector3> normals = input.Geometry[0].Vertices.Channels.Get<Microsoft.Xna.Framework.Vector3>(VertexChannelNames.Normal(0));
                mesh.normals = normals.ToArray();
            }

            mesh.boundingSphere = BoundingSphere.CreateFromPoints(mesh.vertices);

            meshData.meshParts.Add(input.Name, mesh);
        }

        void ProcessNode(NodeContent node)
        {
            // Is this node in fact a mesh?
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Reorder vertex and index data so triangles will render in
                // an order that makes efficient use of the GPU vertex cache.
                MeshHelper.OptimizeForCache(mesh);

                // Process all the geometry in the mesh.
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    ProcessGeometry(node.Name, geometry);
                }
            }

            // Recurse over any child nodes.
            foreach (NodeContent child in node.Children)
            {
                ProcessNode(child);
            }
        }

        void ProcessGeometry(string name, GeometryContent geometry)
        {
            MeshDataPart mesh = new MeshDataPart();

            string normalName = VertexChannelNames.EncodeName(VertexElementUsage.Normal, 0);
            string texCoordName = VertexChannelNames.EncodeName(VertexElementUsage.TextureCoordinate, 0);
            foreach (var channel in geometry.Vertices.Channels)
            {
                if (channel.Name == normalName)
                {
                    VertexChannel<Microsoft.Xna.Framework.Vector3> normals = channel as VertexChannel<Microsoft.Xna.Framework.Vector3>;
                    mesh.normals = new Microsoft.Xna.Framework.Vector3[normals.Count];
                    normals.CopyTo(mesh.normals, 0);
                }
                if (channel.Name == texCoordName)
                {
                    VertexChannel<Microsoft.Xna.Framework.Vector2> texCoords = channel as VertexChannel<Microsoft.Xna.Framework.Vector2>;
                    mesh.uv = new Microsoft.Xna.Framework.Vector2[texCoords.Count];
                    texCoords.CopyTo(mesh.uv, 0);
                }
            }

            Microsoft.Xna.Framework.Vector3[] verts = new Microsoft.Xna.Framework.Vector3[geometry.Vertices.Positions.Count];
            mesh.vertices = new Microsoft.Xna.Framework.Vector3[geometry.Vertices.Positions.Count];
            geometry.Vertices.Positions.CopyTo(verts, 0);
            Microsoft.Xna.Framework.Vector3.Transform(verts, ref preTransform, mesh.vertices);

            mesh.triangles = new short[geometry.Indices.Count];
            for (int i = 0; i < geometry.Indices.Count; i++)
            {
                mesh.triangles[i] = (short)geometry.Indices[i];
            }

            mesh.boundingSphere = BoundingSphere.CreateFromPoints(mesh.vertices);

            // Add the new piece of geometry to our output model.
            meshData.meshParts.Add(name, mesh);
        }

    }
}
