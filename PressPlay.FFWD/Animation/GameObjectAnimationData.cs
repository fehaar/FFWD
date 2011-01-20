using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public class GameObjectAnimationData
    {
        public GameObjectAnimationData(Dictionary<string, AnimationClip> animationClips, Dictionary<string, Matrix> childAbsoluteTransforms)
        {
            AnimationClips = animationClips;
            ChildAbsoluteTransforms = childAbsoluteTransforms;
        }

        /// <summary>
        /// The baked transform in terms of scale and rotation
        /// </summary>
        [ContentSerializer]
        public Matrix BakedTransform { get; set; }

        /// <summary>
        /// Gets a collection of animation clips. These are stored by name in a
        /// dictionary, so there could for instance be clips for "Walk", "Run",
        /// "JumpReallyHigh", etc.
        /// </summary>
        [ContentSerializer]
        public Dictionary<string, AnimationClip> AnimationClips { get; private set; }

        /// <summary>
        /// Gets a collection of animation clips. These are stored by name in a
        /// dictionary, so there could for instance be clips for "Walk", "Run",
        /// "JumpReallyHigh", etc.
        /// </summary>
        [ContentSerializer]
        public Dictionary<string, Matrix> ChildAbsoluteTransforms { get; private set; }

        /// <summary>
        /// Private constructor for use by the XNB deserializer.
        /// </summary>
        private GameObjectAnimationData()
        {

        }
    }
}
