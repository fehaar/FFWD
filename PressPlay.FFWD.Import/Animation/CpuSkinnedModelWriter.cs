#region File Description
//-----------------------------------------------------------------------------
// CpuSkinnedModelWriter.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Import.Animation
{
    /// <summary>
    /// Writes out a CpuSkinnedModelContent object to an XNB file to be read in as
    /// a CpuSkinnedModel.
    /// </summary>
    [ContentTypeWriter]
    class CpuSkinnedModelWriter : ContentTypeWriter<CpuSkinnedModelContent>
    {
        protected override void Write(ContentWriter output, CpuSkinnedModelContent value)
        {
            output.WriteObject(value.ModelParts);
            output.WriteObject(value.SkinningData);

            Microsoft.Xna.Framework.Quaternion rotation = Microsoft.Xna.Framework.Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(value.Rotation.Y), MathHelper.ToRadians(value.Rotation.X), MathHelper.ToRadians(value.Rotation.Z));
            Matrix m = Matrix.CreateScale(value.Scale) * Matrix.CreateFromQuaternion(rotation);

            output.Write(m);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "PressPlay.FFWD.SkinnedModel.CpuSkinnedModel, PressPlay.FFWD";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "PressPlay.FFWD.SkinnedModel.CpuSkinnedModelReader, PressPlay.FFWD";
        }
    }

    /// <summary>
    /// Writes out a CpuSkinnedModelPartContent object to be read in as a CpuSkinnedModelPart
    /// </summary>
    [ContentTypeWriter]
    class CpuSkinnedModelPartWriter : ContentTypeWriter<CpuSkinnedModelPartContent>
    {
        protected override void Write(ContentWriter output, CpuSkinnedModelPartContent value)
        {
            output.Write(value.Name);
            output.Write(value.TriangleCount);
            output.WriteObject(value.Vertices);
            output.WriteObject(value.IndexCollection);
            output.WriteSharedResource(value.Material);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "PressPlay.FFWD.SkinnedModel.CpuSkinnedModelPart, PressPlay.FFWD";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "PressPlay.FFWD.SkinnedModel.CpuSkinnedModelPartReader, PressPlay.FFWD";
        }
    }
}
