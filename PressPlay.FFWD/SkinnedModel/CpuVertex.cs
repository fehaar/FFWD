#region File Description
//-----------------------------------------------------------------------------
// CpuVertex.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.SkinnedModel
{
    /// <summary>
    /// A struct that contains the vertex information we need on the CPU.
    /// This type is not used for a GPU vertex.
    /// </summary>
    public struct CpuVertex
    {
        public Microsoft.Xna.Framework.Vector3 Position;
        public Microsoft.Xna.Framework.Vector3 Normal;
        public Microsoft.Xna.Framework.Vector2 TextureCoordinate;
        public Vector4 BlendWeights;
        public Vector4 BlendIndices;
    }
}
