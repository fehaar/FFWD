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
            RotationY = 180;
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
        [DefaultValue(180.0f)]
        [Description("The rotation of the model in the game.")]
        public float RotationY { get; set; }
        [DisplayName("Rotation Z")]
        [DefaultValue(0.0f)]
        [Description("The rotation of the model in the game.")]
        public float RotationZ { get; set; }

        private MeshDataContent meshData;
        private Matrix preTransform;

        private Dictionary<string, BoneWeightCollection[]> boneWeights = new Dictionary<string, BoneWeightCollection[]>();

        private ContentBuildLogger logger;

        public override MeshDataContent Process(NodeContent input, ContentProcessorContext context)
        {
            meshData = new MeshDataContent();
            logger = context.Logger;
            if (SkinningHelpers.MeshHasSkinning(input))
            {
                // This is a skinned model, so treat is as one
                CpuSkinnedModelProcessor proc = new CpuSkinnedModelProcessor();
                proc.RotationX = this.RotationX;
                proc.RotationY = this.RotationY;
                proc.RotationZ = this.RotationZ;
                proc.Scale = this.Scale;
                meshData.skinnedModel = proc.Process(input, context);
                meshData.boundingBox = BoundingBox.CreateFromSphere(meshData.skinnedModel.BoundingSphere);
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
                    if (Scale == 0)
                    {
                        throw new Exception("Scale of model is 0!");
                    }
                    preTransform = Matrix.CreateScale(new Microsoft.Xna.Framework.Vector3(Scale, Scale, -Scale)) * Matrix.CreateFromQuaternion(rotation);
                    ProcessNode(input);

                    ProcessBoneWeights();
                }
            }
            return meshData;
        }

        void ProcessNode(NodeContent node)
        {
            // Is this node in fact a mesh?
            MeshContent mesh = node as MeshContent;
            //logger.LogMessage("Node {0} - {3} abs: {1} - t: {2}", node.Name, node.AbsoluteTransform.Translation, node.Transform.Translation, node.Transform == node.AbsoluteTransform);

            if (mesh != null)
            {
                // Reorder vertex and index data so triangles will render in
                // an order that makes efficient use of the GPU vertex cache.
                MeshHelper.OptimizeForCache(mesh);

                // Process all the geometry in the mesh.
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    Matrix m = node.AbsoluteTransform;
                    if (node.AbsoluteTransform == node.Transform)
                    {
                        m = m * Matrix.CreateScale(0.01f);
                    }
                    ProcessGeometry(node.Name, geometry, m);
                }
            }

            // Recurse over any child nodes.
            foreach (NodeContent child in node.Children)
            {
                ProcessNode(child);
            }
        }

        private void ProcessBoneWeights()
        {
            meshData.boneIndices = new Dictionary<string, byte>();
            // NOTE: It is assumed that every 
            foreach (var mesh in boneWeights.Keys)
            {
                MeshDataPart part = meshData.meshParts[mesh];
                part.blendIndices = new byte[4];
                int blendWeightIndex = 0;
                part.blendWeights = new Vector4[boneWeights[mesh].Length];
                Dictionary<int, int> indexLookup = new Dictionary<int, int>();
                foreach (var bwc in boneWeights[mesh])
                {
                    Vector4 w = new Vector4();
                    foreach (var weight in bwc)
                    {
                        if (!meshData.boneIndices.ContainsKey(weight.BoneName))
                        {
                            meshData.boneIndices.Add(weight.BoneName, (byte)meshData.boneIndices.Count);
                        }
                        byte idx = meshData.boneIndices[weight.BoneName];
                        if (!indexLookup.ContainsKey(idx))
                        {
                            indexLookup.Add(idx, indexLookup.Count);
                        }
                        switch (indexLookup[idx])
                        {
                            case 0:
                                part.blendIndices[0] = idx;
                                w.X = weight.Weight;
                                break;
                            case 1:
                                part.blendIndices[1] = idx;
                                w.Y = weight.Weight;
                                break;
                            case 2:
                                part.blendIndices[2] = idx;
                                w.Z = weight.Weight;
                                break;
                            case 3:
                                part.blendIndices[3] = idx;
                                w.W = weight.Weight;
                                break;
                            default:
                                //throw new Exception("We have more than 4 bone indexes for a single mesh!");
                                break;
                        }
                    }
                    part.blendWeights[blendWeightIndex++] = w;
                }
            }
        }

        void ProcessGeometry(string name, GeometryContent geometry, Matrix transform)
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
                    Microsoft.Xna.Framework.Vector3.TransformNormal(normals.ToArray(), ref preTransform, mesh.normals);
                }
                if (channel.Name == texCoordName)
                {
                    VertexChannel<Microsoft.Xna.Framework.Vector2> texCoords = channel as VertexChannel<Microsoft.Xna.Framework.Vector2>;
                    mesh.uv = new Microsoft.Xna.Framework.Vector2[texCoords.Count];
                    texCoords.CopyTo(mesh.uv, 0);
                }
                if (channel.Name == "Weights0")
                {
                    VertexChannel<BoneWeightCollection> weights = channel as VertexChannel<BoneWeightCollection>;
                    BoneWeightCollection[] bwc = weights.ToArray<BoneWeightCollection>();
                    boneWeights.Add(name, bwc);
                }
            }

            Microsoft.Xna.Framework.Vector3[] verts = new Microsoft.Xna.Framework.Vector3[geometry.Vertices.Positions.Count];
            mesh.vertices = new Microsoft.Xna.Framework.Vector3[geometry.Vertices.Positions.Count];
            geometry.Vertices.Positions.CopyTo(verts, 0);
            Matrix t = preTransform * Matrix.Invert(transform);
//            t.Translation += transform.Translation;
            Microsoft.Xna.Framework.Vector3 scale;
            Microsoft.Xna.Framework.Quaternion rot;
            Microsoft.Xna.Framework.Vector3 trans;
            transform.Decompose(out scale, out rot, out trans);
            t.Decompose(out scale, out rot, out trans);

            //Microsoft.Xna.Framework.Vector3.Transform(verts, ref transform, mesh.vertices);
            Microsoft.Xna.Framework.Vector3.Transform(verts, ref preTransform, mesh.vertices);
            //Microsoft.Xna.Framework.Vector3.Transform(verts, ref t, mesh.vertices);

            mesh.triangles = new short[][] { new short[geometry.Indices.Count] };
            for (int i = 0; i < geometry.Indices.Count; i++)
            {
                mesh.triangles[0][i] = (short)geometry.Indices[i];
            }

            mesh.boundingBox = BoundingBox.CreateFromPoints(mesh.vertices);

            // Add the new piece of geometry to our output model. If the name is already here, it is a submesh and the triangles will be combined.
            if (meshData.meshParts.ContainsKey(name))
            {
                MeshDataPart oldPart = meshData.meshParts[name];
                oldPart.AddSubMesh(mesh);
            }
            else
            {
                meshData.meshParts.Add(name, mesh);
            }
        }

    }
}
