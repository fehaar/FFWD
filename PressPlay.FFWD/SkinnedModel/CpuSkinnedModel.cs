#region File Description
//-----------------------------------------------------------------------------
// CpuSkinnedModel.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD;

namespace PressPlay.FFWD.SkinnedModel
{
    public class CpuSkinnedModel
    {
        private readonly List<CpuSkinnedModelPart> modelParts = new List<CpuSkinnedModelPart>();

        /// <summary>
        /// Gets the SkinningData associated with this model.
        /// </summary>
        public SkinningData SkinningData { get; internal set; }

        /// <summary>
        /// The baked transform in terms of scale and rotation
        /// </summary>
        public Matrix BakedTransform;

        public BoundingSphere BoundingSphere { get; internal set; }

        /// <summary>
        /// Gets a collection of the model parts that make up this model.
        /// </summary>
        public ReadOnlyCollection<CpuSkinnedModelPart> Parts { get; internal set; }

        internal CpuSkinnedModel(List<CpuSkinnedModelPart> modelParts, SkinningData skinningData)
        {
            this.modelParts = modelParts;
            this.SkinningData = skinningData;
            this.Parts = new ReadOnlyCollection<CpuSkinnedModelPart>(this.modelParts);
        }

        /// <summary>
        /// Sets the bone matrices for all model parts.
        /// </summary>
        //public void SetBones(Matrix[] bones)
        //{
        //    foreach (var part in modelParts)
        //    {
        //        part.SetBones(bones, ref BakedTransform);
        //    }
        //}
    }
}
