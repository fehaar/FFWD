#region File Description
//-----------------------------------------------------------------------------
// CpuSkinningContentData.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using PressPlay.FFWD;
using PressPlay.FFWD.SkinnedModel;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Import.Animation
{
    /// <summary>
    /// Represents an entire skinned model that will be animated on the CPU.
    /// </summary>
    class CpuSkinnedModelContent
    {
        public List<CpuSkinnedModelPartContent> ModelParts = new List<CpuSkinnedModelPartContent>();
        public SkinningData SkinningData;

        public void AddModelPart(
            string name,
            int triangleCount,
            IndexCollection indexCollection,
            CpuVertex[] vertices,
            BasicMaterialContent material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            ModelParts.Add(new CpuSkinnedModelPartContent
            {
                Name = name,
                TriangleCount = triangleCount,
                IndexCollection = indexCollection,
                Vertices = vertices,
                Material = material,
            });
        }
    }

    /// <summary>
    /// Represents a single part of the skinned model.
    /// </summary>
    class CpuSkinnedModelPartContent
    {
        public string Name;
        public int TriangleCount;
        public CpuVertex[] Vertices;
        public IndexCollection IndexCollection;
        public BasicMaterialContent Material;
    }
}
