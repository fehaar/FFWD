#region File Description
//-----------------------------------------------------------------------------
// AnimationClip.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    /// <summary>
    /// A model animation clip is the runtime equivalent of the
    /// Microsoft.Xna.Framework.Content.Pipeline.Graphics.AnimationContent type.
    /// It holds all the keyframes needed to describe a single model animation.
    /// </summary>
    public class AnimationClip : UnityObject
    {
        internal AnimationClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
        {
            // TODO: Reimplement this
            throw new NotImplementedException();
        }

        [ContentSerializer]
        public float length { get; internal set; }
        public string name;
        public WrapMode wrapMode = WrapMode.Once;
        [ContentSerializer]
        internal AnimationClipCurveData[] curves;

        internal float timeOffset;

        /// <summary>
        /// Private constructor for use by the XNB deserializer.
        /// </summary>
        internal AnimationClip()
        {
        }
    }
}
