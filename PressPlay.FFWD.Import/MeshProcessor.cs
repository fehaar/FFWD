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

namespace PressPlay.FFWD.Import
{
    [ContentProcessor(DisplayName = "FFWD - Mesh processor")]
    class MeshProcessor : ContentProcessor<NodeContent, MeshDataContent>
    {
        public MeshProcessor()
        {
            ReadNormals = true;
            ReadUVs = true;
            WriteAsModel = true;
        }

        [DefaultValue(true)]
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


        public override MeshDataContent Process(NodeContent input, ContentProcessorContext context)
        {
            MeshDataContent mesh = new MeshDataContent();

            if (SkinningHelpers.MeshHasSkinning(input))
            {
                // This is a skinned model, so treat is as one
                CpuSkinnedModelProcessor proc = new CpuSkinnedModelProcessor();
                proc.RotationX = this.RotationX;
                proc.RotationY = this.RotationY;
                proc.RotationZ = this.RotationZ;
                proc.Scale = this.Scale;
                mesh.skinnedModel = proc.Process(input, context);
                mesh.boundingSphere = mesh.skinnedModel.BoundingSphere;
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
                    mesh.model = proc.Process(input, context);
                }
                else
                {
                    if (input is MeshContent)
                    {
                        if (input.Name == "sprite_square")
                        {

                        }
                        ProcessSpriteSquareMesh(mesh, input as MeshContent);
                    }
                }
            }
            return mesh;
        }

        private void ProcessSpriteSquareMesh(MeshDataContent meshData, MeshContent input)
        {
            Microsoft.Xna.Framework.Quaternion rotation = Microsoft.Xna.Framework.Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(RotationY), MathHelper.ToRadians(RotationX), MathHelper.ToRadians(RotationZ));
            Matrix m = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(rotation);

            MeshDataPart mesh = new MeshDataPart();

            Microsoft.Xna.Framework.Vector3[] verts = new Microsoft.Xna.Framework.Vector3[input.Positions.Count];
            mesh.vertices = new Microsoft.Xna.Framework.Vector3[input.Positions.Count];
            input.Positions.CopyTo(verts, 0);
            Microsoft.Xna.Framework.Vector3.Transform(verts, ref m, mesh.vertices);

            // We need to unwrap from triangle strip to triangle list - this does not work in general...
            List<short> tris = new List<short>();
            tris.Add((short)input.Geometry[0].Vertices.PositionIndices[0]);
            tris.Add((short)input.Geometry[0].Vertices.PositionIndices[1]);
            tris.Add((short)input.Geometry[0].Vertices.PositionIndices[2]);
            for (int i = 3; i < input.Geometry[0].Vertices.PositionIndices.Count; i++)
            {
                tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i]);
                tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i - 3]);
                tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i - 1]);
            }
            //for (int i = 3; i < input.Geometry[0].Vertices.PositionIndices.Count; i++)
            //{
            //    if ((i % 2) == 1)
            //    {
            //        tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i - 1]);
            //        tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i - 2]);
            //        tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i]);
            //    }
            //    else
            //    {
            //        tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i - 2]);
            //        tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i - 1]);
            //        tris.Add((short)input.Geometry[0].Vertices.PositionIndices[i]);
            //    }
            //}

            mesh.triangles = tris.ToArray();

            if (ReadNormals && input.Geometry[0].Vertices.Channels.Contains(VertexChannelNames.Normal(0)))
            {
                VertexChannel<Microsoft.Xna.Framework.Vector3> normals = input.Geometry[0].Vertices.Channels.Get<Microsoft.Xna.Framework.Vector3>(VertexChannelNames.Normal(0));
                mesh.normals = normals.ToArray();
            }

            if (ReadUVs && input.Geometry[0].Vertices.Channels.Contains(VertexChannelNames.TextureCoordinate(0)))
            {
                VertexChannel<Microsoft.Xna.Framework.Vector2> uv = input.Geometry[0].Vertices.Channels.Get<Microsoft.Xna.Framework.Vector2>(VertexChannelNames.TextureCoordinate(0));
                mesh.uv = uv.Select(v => new Microsoft.Xna.Framework.Vector2(v.X, v.Y)).ToArray();
            }
            mesh.boundingSphere = BoundingSphere.CreateFromPoints(mesh.vertices);

            meshData.meshParts.Add(input.Name, mesh);
        }
    }
}
