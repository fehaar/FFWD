using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using Microsoft.Xna.Framework;
using System.IO;

namespace PressPlay.FFWD.Components
{
    public class Animation : Behaviour, IInitializable, IEnumerable<AnimationState>
	{
        [ContentSerializer(ElementName = "clip", Optional=true)]
        private string clipId = string.Empty;
        public bool playAutomatically;
        public WrapMode wrapMode;
        [ContentSerializer(ElementName = "clips")]
        private string[] clipsId = null;

        private string defaultClip;
        [ContentSerializerIgnore]
		public AnimationClip clip
        {
            get
            {
                return GetClip(defaultClip);
            }
            set
            {
                AddClip(value, value.name);
                defaultClip = value.name;
            }
        }

		private Dictionary<string, int> stateIndexes = new Dictionary<string, int>();
        private List<AnimationState> states;

        #region Static members for doing the animation
        private static List<Animation> animationComponents = new List<Animation>(ApplicationSettings.DefaultCapacities.AnimationComponents);

        internal static void SampleAnimations()
        {
            for (int i = 0; i < animationComponents.Count; i++)
            {
                Animation a = animationComponents[i];
                // TODO: Implement culling here
                if (a.enabled && a.isPlaying)
                {
                    a.UpdateAnimationStates(Time.deltaTime);
                    a.Sample();
                }
            }
        }
    	#endregion

        public void Initialize(AssetHelper assets)
        {
            if (clipsId != null)
            {
                states = new List<AnimationState>(clipsId.Length);
                stateIndexes = new Dictionary<string, int>();
                for (int i = 0; i < clipsId.Length; i++)
                {
                    if (clipsId[i] == null)
                    {
                        continue;
                    }
                    AnimationClip data = assets.LoadAsset<AnimationClip>(Path.Combine("Animations", clipsId[i]));
                    if (data != null)
                    {
                        AddClip(data, data.name);
                        if (clipsId[i] == clipId)
                        {
                            defaultClip = data.name;
                        }
                    }
                }
            }
        }

        public override void Awake()
        {
            base.Awake();
            for (int i = 0; i < states.Count; i++)
            {
                states[i].InitializeSamplers(gameObject);
            }
            if (playAutomatically && clip != null)
            {
                Play(defaultClip);
            }
        }

        protected override void Destroy()
        {
            base.Destroy();
            animationComponents.Remove(this);
        }

		public AnimationState this[string index]
		{
			get
			{
				if (stateIndexes.ContainsKey(index))
				{
					return states[stateIndexes[index]];
				}
				return null;
			}
		}

		public AnimationClip GetClip(string name)
		{
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }
            if (stateIndexes.ContainsKey(name))
            {
                return states[stateIndexes[name]].clip;
            }
            return null;
		}

		public bool isPlaying
        {
			get 
            {
                return states.Any(s => s.enabled);
            } 
		}

		public void Rewind()
		{
            for (int i = 0; i < states.Count; i++)
            {
                states[i].time = 0;
            }
		}

        public void Rewind(string name)
        {
            if (stateIndexes.ContainsKey(name))
            {
                this[name].time = 0;
            }
        }

        public bool Play()
		{
			return Play(defaultClip);
		}

		public bool Play(string name)
		{
            if (String.IsNullOrEmpty(name))
            {
                return false;
            }            
            if (!stateIndexes.ContainsKey(name))
            {
                return false;
            }
            Stop();
            this[name].enabled = true;
            if (!animationComponents.Contains(this))
            {
                animationComponents.Add(this);
            }
            return true;
        }

		public AnimationState PlayQueued(string name)
		{
			return PlayQueued(name, QueueMode.CompleteOthers);
		}

        public AnimationState PlayQueued(string name, QueueMode mode)
		{
            AnimationState state = this[name];
            if (state != null)
            {
                state = new AnimationState(this, state.clip);
                state.enabled = (!isPlaying);
                state.playQueuedReference = true;
                AddState(Guid.NewGuid().ToString(), state);
                return state;
            }
            return null;
		}

		public void Stop()
		{
            for (int i = 0; i < states.Count; i++)
            {
                states[i].enabled = false;
            }
		}

		public void Stop(string name)
		{
			if (stateIndexes.ContainsKey(name))
			{
				states[stateIndexes[name]].enabled = false;
			}
		}

		public void AddClip(AnimationClip clip, string newName)
		{
			if (String.IsNullOrEmpty(newName))
			{
				return;
			}
            AddState(newName, new AnimationState(this, clip));
		}

        private void AddState(string newName, AnimationState state)
        {
            if (states == null)
            {
                states = new List<AnimationState>(1);
            }
            state.name = newName;
            if (stateIndexes.ContainsKey(newName))
            {
                states[stateIndexes[newName]] = state;
            }
            else
            {
                stateIndexes[newName] = states.Count;
                states.Add(state);
            }
        }

		public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
		{
			AnimationClip newClip = new AnimationClip(clip, newName, firstFrame, lastFrame);
			AddClip(newClip, newName);
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
            Stop();
            Play(name);
		}

		public void CrossFade(string name)
		{
			CrossFade(name, 0.3f);
		}

		public void CrossFade(string name, float fadeLength)
		{
            // TODO : Add implementation of method
            Stop();
			Play(name);
		}

		public void Sample()
		{
            for (int i = 0; i < states.Count; i++)
            {
                states[i].Sample();
            }
		}

        public bool IsPlaying(string name)
        {
            if (stateIndexes.ContainsKey(name))
            {
                return states[stateIndexes[name]].enabled;
            }
            return false;
        }

        public int GetClipCount()
        {
            return states.Count;
        }

        public IEnumerator<AnimationState> GetEnumerator()
        {
            return states.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return stateIndexes.Values.GetEnumerator();
        }

        internal void UpdateAnimationStates(float deltaTime)
        {
            bool playQueued = false;
            bool hasQueued = false;
            for (int i = states.Count - 1; i >= 0; i--)
            {
                AnimationState state = states[i];
                hasQueued |= state.playQueuedReference;
                if (state.Update(deltaTime))
                {
                    if (state.playQueuedReference)
                    {
                        stateIndexes.Remove(state.name);
                        states.RemoveAt(i);
                    }
                    else
                    {
                        playQueued = true;
                    }
                }
            }
            if (hasQueued && playQueued)
            {
                for (int i = 0; i < states.Count; i++)
                {
                    if (states[i].playQueuedReference)
                    {
                        states[i].enabled = true;
                    }
                }
            }
        }
    }
}
