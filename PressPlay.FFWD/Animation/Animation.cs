using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
	public class Animation : Behaviour, IEnumerable<AnimationState>
	{
        [ContentSerializer(ElementName = "clip", Optional=true)]
        private int clipId;
        public bool playAutomatically;
        public WrapMode wrapMode;
        [ContentSerializer(ElementName = "clips")]
        private int[] clipsId;

		// TODO: This should be moved to a content processor!
		[ContentSerializerIgnore]
		private string[] animations = null;

		[ContentSerializerIgnore]
		public AnimationClip clip;

		private Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
		private Dictionary<string, AnimationState> states = new Dictionary<string, AnimationState>();

		public override void Awake()
		{
		}

		public AnimationState this[string index]
		{
			get
			{
				if (states.ContainsKey(index))
				{
					return states[index];
				}
				return null;
			}
		}

		public AnimationClip GetClip(string name)
		{
            if (clips.ContainsKey(name))
            {
                return clips[name];
            }
            return null;
		}

		public bool isPlaying {
			get { return true; } //TODO check if animation is playing or not
			private set { }
		}

		public void Rewind()
		{
			// TODO : Add implementation of method
			throw new NotImplementedException("Method not implemented.");
		}

		public void Play()
		{
			Play(animations[0]);
		}

		public void Play(string name)
		{
            throw new NotImplementedException("Method not implemented.");
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
			foreach (AnimationState state in states.Values)
			{
				state.enabled = false;
			}
		}

		public void Stop(string name)
		{
			if (states.ContainsKey(name))
			{
				states[name].enabled = false;
			}
		}
	
		public void AddClip(AnimationClip clip, string newName)
		{
			if (String.IsNullOrEmpty(newName))
			{
				return;
			}
			clip.name = newName;
			clips[newName] = clip;
			states[newName] = new AnimationState() { length = (float)clip.Duration.TotalSeconds, wrapMode = clip.wrapMode };
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
			//throw new NotImplementedException("Method not implemented.");
            //Rewind();
            Stop();
            Play(name);
		}

		public void CrossFade(string name)
		{
			CrossFade(name, 0.3f);
		}

		public void CrossFade(string name, float fadeLength)
		{
            //Rewind();
            Stop();
			Play(name);
		}

		public void Sample()
		{
		}

        public bool IsPlaying(string name)
        {
            if (states.ContainsKey(name))
            {
                return states[name].enabled;
            }
            return false;
        }

        public int GetClipCount()
        {
            return clips.Count;
        }

        public IEnumerator<AnimationState> GetEnumerator()
        {
            return states.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return states.Values.GetEnumerator();
        }
    }
}
