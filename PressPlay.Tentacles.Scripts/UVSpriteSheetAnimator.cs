using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.Tentacles.Scripts {
	public class UVSpriteSheetAnimator : UVSpriteSheet {
		
		public delegate void AnimationCompleteCallback();
		private AnimationCompleteCallback animCompleteCallback;
		
		public SpriteSheetAnim[] anims;
		
		private SpriteSheetAnim currentPlayingAnim;
		protected int currentFrameIndex;	
		private float animTime;
		
		protected bool isPlaying = false;
		
		public string automaticPlayAnim;
		public bool automaticPlay= false;
		public bool randomizeAutomaticPlayStart = false;
		
		
		
		
		public override void Initialize()
		{
			if (isInitialized){return;}
			
			base.Initialize();
			if (automaticPlay && automaticPlayAnim != null)
			{
				Play(automaticPlayAnim);
			}
		}
		
		public void Stop()
		{
			isPlaying = false;
		}
		
		public void Play(string _str)
		{
			if (!isInitialized){Initialize();}
			
			for (int i = 0; i < anims.Length; i++) {
				if (anims[i].name == _str)
				{
					//Debug.Log("UVSpriteSheetAnimator AUTOPLAYING : "+_str);
					Play(anims[i]);
					return;
				}
			}
		}
		
		public void Play(SpriteSheetAnim _anim)
		{
			//Debug.Log("Playing spritesheet anim : "+_anim.name);
			
			if (_anim.startFrameIndex == _anim.endFrameIndex)
			{
				ShowFrameWithIndex(_anim.startFrameIndex);
				isPlaying = false;
				return;
			}
			
			
			isPlaying = true;
			ShowFrameWithIndex(_anim.startFrameIndex);
			currentPlayingAnim = _anim;
			animTime = 0;
		}
		
		public override void Update()
		{
			if (isPlaying)
			{
				UpdateAnim();
			}
		}
		
		protected void UpdateAnim()
		{
		
		
			animTime += Time.deltaTime;
			
			//Debug.Log("animTime "+animTime);
			
			int tmpIndex = (int)((animTime) / currentPlayingAnim.timePerFrame) + currentPlayingAnim.startFrameIndex;
			
			if (tmpIndex > currentPlayingAnim.endFrameIndex)
			{
				 HandleAnimationFinished();
				 return;
			}
			
			if (tmpIndex != currentFrameIndex)
			{
				ShowFrameWithIndex(tmpIndex);
			}
			
		}
		
		public void SetCurrentAnimFraction(float _fraction)
		{
			animTime = _fraction * currentPlayingAnim.length;
			
			UpdateAnim();
		}
		
		public void SetCurrentAnimTime(float _time)
		{
			animTime = _time;
			
			UpdateAnim();
		}
		
		/*void ShowNextFrame()
		{
			
			
			
			currentFrameIndex++;
			ShowFrameWithIndex(currentFrameIndex);
		}*/
		
		protected void ShowFrameWithIndex(int _index)
		{
			//Debug.Log("ShowFrameWithIndex "+_index);
			currentFrameIndex = _index;
			UpdateUVs(XPosFromIndex(_index), YPosFromIndex(_index));
		}
		
		void HandleAnimationFinished()
		{
			switch(currentPlayingAnim.wrapMode)
			{
			case SpriteSheetAnim.WrapMode.clamp:
				isPlaying = false;
				if (animCompleteCallback != null)
				{
					animCompleteCallback();
				}
				break;
				
			case SpriteSheetAnim.WrapMode.loop:
				Play(currentPlayingAnim);
				break;
			}
		}
	}
	
	public class SpriteSheetAnim{
		
		public enum WrapMode{
			clamp,
			loop
		}
		
		public int startFrameIndex;
		public int endFrameIndex;
		public float timePerFrame;
		public string name;
		public WrapMode wrapMode;
		
		public float length{
			get{return timePerFrame * (endFrameIndex - startFrameIndex);}
			//set{timePerFrame = value / (endFrameIndex - startFrameIndex);}
		}
		/*private int _totalFrames;
		public int totalFrames{
			get{
				if (_totalFrames == null)
				{
					_totalFrames = endFrame.
				}
				
				return _totalFrames
			}
		}*/
	}
}