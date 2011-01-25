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

namespace PressPlay.FFWD
{
    /// <summary>
    /// A model animation clip is the runtime equivalent of the
    /// Microsoft.Xna.Framework.Content.Pipeline.Graphics.AnimationContent type.
    /// It holds all the keyframes needed to describe a single model animation.
    /// </summary>
    public class AnimationClip
    {
        internal AnimationClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
        {
            this.name = newName;
            if (firstFrame < 0)
            {
                firstFrame = 0;
            }
            double startTime = firstFrame * (1.0 / 30.0);
            double endTime = lastFrame * (1.0 / 30.0);
            if (clip.Keyframes != null)
            {
                Keyframes = new List<Keyframe>();
                foreach (Keyframe frame in clip.Keyframes)
                {
                    double keySecs = frame.Time.TotalSeconds;
                    if (keySecs >= startTime && keySecs < endTime)
                    {
                        Keyframes.Add(new Keyframe(frame.Bone, TimeSpan.FromSeconds(keySecs - startTime), frame.Transform));
                    }
                }
            }
            this.Duration = TimeSpan.FromSeconds(endTime - startTime);
        }

        [ContentSerializerIgnore]
        public float length
        {
            get
            {
                return (float)Duration.TotalSeconds;
            }
        }

        public string name;
        public WrapMode wrapMode = WrapMode.Once;

        /// <summary>
        /// Gets the total length of the model animation clip
        /// </summary>
        [ContentSerializer]
        public TimeSpan Duration { get; private set; }
        
        /// <summary>
        /// Gets a combined list containing all the keyframes for all bones,
        /// sorted by time.
        /// </summary>
        [ContentSerializer]
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
