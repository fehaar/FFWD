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
	public class Animation : Behaviour, PressPlay.FFWD.Interfaces.IUpdateable
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
				if (clips.Count == 0)
				{
					return null;
				}
				return clips[animationIndex];
			}
		}

		private Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
		private Dictionary<string, AnimationState> states = new Dictionary<string, AnimationState>();
		private string animationIndex;
		private SkinnedAnimationPlayer animationPlayer;

		public override void Awake()
		{
			SkinnedMeshRenderer smr = GetComponentInChildren<SkinnedMeshRenderer>();
			if ((smr != null) && (smr.sharedMesh != null) && (smr.sharedMesh.skinnedModel != null))
			{
				if (smr.sharedMesh.skinnedModel.SkinningData.AnimationClips != null)
				{
					Initialize(smr.sharedMesh.skinnedModel.SkinningData, smr.sharedMesh.skinnedModel.BakedTransform);
				}
			}
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
			return clips[name];
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
			//throw new NotImplementedException("Method not implemented.");
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
			if (String.IsNullOrEmpty(animationIndex))
			{
				animationIndex = newName;
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
			//throw new NotImplementedException("Method not implemented.");
		}

		public void CrossFade(string name)
		{
			CrossFade(name, 0.3f);
		}

		public void CrossFade(string name, float fadeLength)
		{
			Play(name);
		}

		internal void Initialize(SkinningData modelData, Matrix bakedTransform)
		{
			foreach (string name in modelData.AnimationClips.Keys)
			{
				AddClip(modelData.AnimationClips[name], name);
			}
			animationPlayer = new SkinnedAnimationPlayer(modelData, bakedTransform);
			animationPlayer.SetTransforms(transform.GetComponentsInChildren<Transform>());
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

				// Update child gameobject world positions
				//foreach (var item in transform.GetComponentsInChildren<Transform>())
				//{
				//    Matrix boneTransform;
				//    if (animationPlayer.WorldTransformForBone(item.name, out boneTransform))
				//    {
				//        item.localPosition = Matrix.Invert(boneTransform).Translation;
				//        //item.SetLocalTransform(Matrix.Invert(boneTransform));
				//    }
				//} 
			}
		}

		public void LateUpdate()
		{
		}
		#endregion

		internal Microsoft.Xna.Framework.Matrix[] GetTransforms()
		{
			if (animationPlayer != null)
			{
				return animationPlayer.SkinTransforms;
			}
			return null;
		}

		internal override void AfterLoad(Dictionary<int, UnityObject> idMap)
		{
			base.AfterLoad(idMap);
			foreach (string item in animations)
			{
				// TODO: Why are we doing this when we are using non-existing data?!?!?!
				AddClip(new AnimationClip(new TimeSpan(), null), item);
			}
		}
	}
}
