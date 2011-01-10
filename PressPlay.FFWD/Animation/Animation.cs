using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Components
{
    public class Animation : Behaviour, IUpdateable
    {
        public bool playAutomatically;

        // TODO: This should be moved to a content processor!
        [ContentSerializer]
        private string[] animations;

        [ContentSerializerIgnore]
        public AnimationClip clip
        {
            get
            {
                return clips[animationIndex];
            }
        }

        private Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
        private Dictionary<string, AnimationState> states = new Dictionary<string, AnimationState>();
        private string animationIndex;
        private SkinnedAnimationPlayer animationPlayer;

        public AnimationState this[string index]
        {
            get
            {
                return states[index];
            }
        }

        public AnimationClip GetClip(string name)
        {
            return clips[name];
        }

        public void Play()
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public void Play(string name)
        {
            if (animationPlayer != null)
            {
                animationPlayer.StartClip(clips[name], states[name]);
            }
        }

        public void PlayQueued(string name)
        {
            PlayQueued(name, QueueMode.CompleteOthers);
        }

        public void PlayQueued(string name, QueueMode mode)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public void Stop()
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public void Stop(string name)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }
	
        public void AddClip(AnimationClip clip, string newName)
        {
            if (String.IsNullOrEmpty(clip.name))
            {
                clip.name = newName;
            }
            clips[newName] = clip;
            states[newName] = new AnimationState() { length = clip.length, wrapMode = clip.wrapMode };
            if (String.IsNullOrEmpty(animationIndex))
            {
                animationIndex = newName;
            }
        }

        public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
        {
            AddClip(clip, newName);
            states[newName].firstFrame = firstFrame;
            states[newName].lastFrame = lastFrame;
            states[newName].length = (lastFrame - firstFrame) / 30.0f;
        }

        public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame, bool addLoopFrame)
        {
            AddClip(clip, newName, firstFrame, lastFrame);
            // TODO: Add the loop frame - which is a frame identical to the first frame at the end. We need to figure out how to do that properly.
        }

        public void Blend(string name)
        {
            Blend(name, 1.0f, 1.0f);
        }

        public void Blend(string name, float weight)
        {
            Blend(name, weight, 1.0f);
        }

        public void Blend(string name, float weight, float length)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public void CrossFade(string name)
        {
            CrossFade(name, 0.3f);
        }

        public void CrossFade(string name, float fadeLength)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        internal void Initialize(ModelData modelData)
        {
            foreach (string name in modelData.ModelAnimationClips.Keys)
            {
                AddClip(modelData.ModelAnimationClips[name], name);
            }
            animationPlayer = new SkinnedAnimationPlayer(modelData.BindPose, modelData.InverseBindPose, modelData.SkeletonHierarchy);
            if (playAutomatically)
            {
                animationPlayer.StartClip(clip, states[clip.name]);
            }
        }

        #region IUpdateable Members
        public void Update()
        {
            if (animationPlayer != null)
            {
                animationPlayer.Update();
            }
        }
        #endregion

        internal Microsoft.Xna.Framework.Matrix[] GetTransforms()
        {
            if (animationPlayer != null)
            {
                return animationPlayer.GetSkinTransforms();
            }
            return null;
        }

        internal override void AfterLoad()
        {
            foreach (string item in animations)
            {
                AddClip(new AnimationClip(new TimeSpan(), null), item);
            }
        }
    }
}
