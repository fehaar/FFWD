#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    /// <summary>
    /// The animation player is in charge of decoding bone position
    /// matrices from an animation clip.
    /// 
    /// This class was taken from the original Skinned Model Sample and reworked to fit in the FFWD engine
    /// http://creators.xna.com/en-US/sample/skinnedmodel 
    /// </summary>
    public class SkinnedAnimationPlayer
    {
        // Information about the currently playing animation clip.
        private AnimationClip currentClipValue;
        private AnimationState currentStateValue;
        private TimeSpan currentTimeValue;
        private int currentKeyframe;
        
        // Current animation transform matrices.
        private Matrix[] boneTransforms;
        private Matrix[] worldTransforms;
        private Matrix[] skinTransforms;
        private Matrix bakedTransform;
        private Transform[] transforms;

        // Backlink to the bind pose and skeleton hierarchy data.
        private SkinningData skinningDataValue;
        
        /// <summary>
        /// Gets the current bone transform matrices, relative to their parent bones.
        /// </summary>
        public Matrix[] BoneTransforms
        {
            get { return boneTransforms; }
        }

        /// <summary>
        /// Gets the current bone transform matrices, in absolute format.
        /// </summary>
        public Matrix[] WorldTransforms
        {
            get { return worldTransforms; }
        }

        /// <summary>
        /// Gets the current bone transform matrices,
        /// relative to the skinning bind pose.
        /// </summary>
        public Matrix[] SkinTransforms
        {
            get { return skinTransforms; }
        }

        /// <summary>
        /// Constructs a new animation player.
        /// </summary>
        public SkinnedAnimationPlayer(SkinningData skinningData, Matrix bakedTransform)
        {
            if (skinningData == null)
                throw new ArgumentNullException("skinningData");

            skinningDataValue = skinningData;

            boneTransforms = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms = new Matrix[skinningData.BindPose.Count];
            transforms = new Transform[worldTransforms.Length];
            this.bakedTransform = bakedTransform;
        }

        /// <summary>
        /// Starts decoding the specified animation clip.
        /// </summary>
        public void StartClip(AnimationClip clip, AnimationState state)
        {
            if (clip == null)
                throw new ArgumentNullException("clip");

            if (currentClipValue == clip)
            {
                return;
            }

            currentClipValue = clip;
            currentStateValue = state;
            state.time = 0;
            state.enabled = true;

            currentTimeValue = TimeSpan.FromSeconds(state.time);

            currentKeyframe = 0;

            ResetAnimation();
        }

        private void ResetAnimation()
        {
            if (currentClipValue.Keyframes.Count > 0)
            {
                TimeSpan ts = currentClipValue.Keyframes[0].Time;
                int index = 0;
                while (index < currentClipValue.Keyframes.Count)
                {
                    Keyframe frame = currentClipValue.Keyframes[index];

                    if (frame.Time != ts)
                    {
                        break;
                    }
                    boneTransforms[frame.Bone] = frame.Transform;
                    index++;
                }
            }
            else
            {
                skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
            }
        }
        
        /// <summary>
        /// Advances the current animation position.
        /// </summary>
        public void FixedUpdate()
        {
            UpdateBoneTransforms(TimeSpan.FromSeconds(Time.deltaTime * currentStateValue.speed));
            UpdateWorldTransforms(bakedTransform);
            UpdateSkinTransforms();
        }

        /// <summary>
        /// Helper used by the Update method to refresh the BoneTransforms data.
        /// </summary>
        public void UpdateBoneTransforms(TimeSpan time)
        {
            if (currentClipValue == null)
                throw new InvalidOperationException("AnimationPlayer.Update was called before StartClip");

            if (!currentStateValue.enabled)
            {
                return;
            }

            //set the current time of the animation, to what is set in the current AnimationState. This is how we scrub through animations
            currentTimeValue = TimeSpan.FromSeconds(currentStateValue.time);
            time += currentTimeValue;

            // See if we should terminate
            if (time.TotalSeconds > currentStateValue.length)
            {
                switch (currentStateValue.wrapMode)
                {
                    case WrapMode.Once:
                        currentStateValue.enabled = false;
                        return;
                    case WrapMode.Loop:
                        time = TimeSpan.FromSeconds(0);
                        break;
                    case WrapMode.PingPong:
                        currentStateValue.speed *= -1;
                        break;
                    case WrapMode.Default:
                        break;
                    case WrapMode.Clamp:
                        time = TimeSpan.FromSeconds(currentStateValue.length);
                        break;
                    default:
                        throw new NotImplementedException("What to do here?");
                }
            }

            // If the position moved backwards, reset the keyframe index.
            if (time < currentTimeValue)
            {
                currentKeyframe = 0;
                ResetAnimation();
            }

            //set current time values, both locally and in AnimationState
            currentStateValue.time = (float)time.TotalSeconds;
            // move the current time according to the time offset so keyframes get evaluated correctly
            currentTimeValue = time + TimeSpan.FromSeconds(currentClipValue.timeOffset);

            int keyframeCount = currentClipValue.Keyframes.Count;
            while (currentKeyframe < keyframeCount)
            {
                Keyframe keyframe = currentClipValue.Keyframes[currentKeyframe];

                // Stop when we've read up to the current time position.
                if (keyframe.Time > currentTimeValue)
                {
                    break;
                }

                // Use this keyframe.
                boneTransforms[keyframe.Bone] = keyframe.Transform;

                currentKeyframe++;
            }
        }
        
        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone.
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            // Child bones.
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skinningDataValue.SkeletonHierarchy[bone];

                worldTransforms[bone] = boneTransforms[bone] * worldTransforms[parentBone];
                // Move transforms according to bone
                if (transforms[bone] != null)
                {
                    transforms[bone].localPosition = Matrix.Invert(worldTransforms[bone]).Translation * 0.01f;
                }
            }

        }
        
        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningDataValue.InverseBindPose[bone] * worldTransforms[bone];
            }
        }

        //internal bool WorldTransformForBone(string boneName, out Matrix m)
        //{
        //    if (currentClipValue != null)
        //    {
        //        if (skinningDataValue.BoneMap.ContainsKey(boneName))
        //        {
        //            m = worldTransforms[skinningDataValue.BoneMap[boneName]];
        //            // TODO: This is a very brutal hardcoded hack as the animation does not work very well with hiearchical scales
        //            m.Translation = m.Translation * 0.01f;
                    
        //            return true;
        //        }
        //    }
        //    m = Matrix.Identity;
        //    return false;
        //}

        internal void SetTransforms(Transform[] transform)
        {
            for (int i = 0; i < transform.Length; i++)
            {
                if (skinningDataValue.BoneMap.ContainsKey(transform[i].name))
                {
                    transforms[skinningDataValue.BoneMap[transform[i].name]] = transform[i];
                }
            }
        }
    }
}
