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
        // TODO: Reimplement this
        internal AnimationClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
        {
            this.name = newName;
            if (firstFrame < 0)
            {
                firstFrame = 0;
            }

            float startTime = firstFrame * (1.0f / 30.0f);
            float endTime = lastFrame * (1.0f / 30.0f);

            if (clip.Keyframes != null)
            {
                Keyframes = new List<Keyframe>();
                timeOffset = (float)startTime;
                int cnt = clip.Keyframes.Count;
                for (int i = 0; i < cnt; i++)
                {
                    Keyframe frame = clip.Keyframes[i];
                    float keySecs = frame.Time;
                    if (keySecs >= startTime && keySecs < endTime)
                    {
                        Keyframes.Add(frame);
                    }
                }
            }
            this.Duration = TimeSpan.FromSeconds(endTime - startTime);
        }

        [ContentSerializer]
        public float length { get; private set; }
        public string name;
        public WrapMode wrapMode = WrapMode.Once;
        [ContentSerializer]
        private AnimationClipCurveData[] curves;

        internal float timeOffset;

        /// <summary>
        /// Gets the total length of the model animation clip
        /// </summary>
        [ContentSerializerIgnore]
        public TimeSpan Duration { get; private set; }
        
        /// <summary>
        /// Gets a combined list containing all the keyframes for all bones,
        /// sorted by time.
        /// </summary>
        [ContentSerializerIgnore]
        public List<Keyframe> Keyframes { get; private set; }

        /// <summary>
        /// Constructs a new model animation clip object.
        /// </summary>
        public AnimationClip(TimeSpan duration, List<Keyframe> keyframes)
        {
            Duration = duration;
            Keyframes = keyframes;
        }

        /// <summary>
        /// Private constructor for use by the XNB deserializer.
        /// </summary>
        private AnimationClip()
        {
        }
    }
}
