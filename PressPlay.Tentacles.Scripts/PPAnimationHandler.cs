using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.Tentacles.Scripts {
	public class PPAnimationHandler : MonoBehaviour {

	    public delegate void PPAnimationCallback();

	    private PPAnimationCallback OnCompleteCallback;
	
        [ContentSerializerIgnore]
	    public Animation animationComponent;
		public int startFrame;
		public bool playAutomatically;
		public string clipToPlay;
		
		public PPAnimationClip[] clips;
	
	    private AnimationClip currentClip;
	    private bool isPlayingAnimation = false;
	    private float startTime;
	    private float duration;
	
	    public bool destroyOnComplete = false;
		
		public float currentClipDuration{
			get{
				if (currentClip == null)
				{return 0;}
				
				return currentClip.length;
			}
		}
		public string currentClipName{
			get{
				if (currentClip == null)
				{return "";}
				
				return currentClip.name;
			}
		}
		
		
		private bool _isInitialized = false;
		public bool isInitialized{
			get{return _isInitialized;}
		}

		public override void Start()
		{
            animationComponent = GetComponentInChildren<Animation>();
            Initialize();
		}
		
		// Use this for initialization
		public void Initialize () {
			
			if (isInitialized){return;}
			_isInitialized = true;
	        if (animationComponent == null) return;
	
	        animationComponent.AddClip(animationComponent.clip, "default", startFrame - 1, startFrame);
	        animationComponent["default"].wrapMode = WrapMode.Default;
	        animationComponent["default"].speed = 1;
			
			foreach (PPAnimationClip item in clips) {
	            animationComponent.AddClip(animationComponent.clip, item.id, item.firstFrame, item.lastFrame, item.addLoopFrame);
	            animationComponent[item.id].wrapMode = item.wrapMode;
	            animationComponent[item.id].speed = item.speed;
				
				if (item.randomizeStartTime)
				{
	                animationComponent[item.id].time = Random.Range(0f, animationComponent[item.id].length);
				}
				
			}
	        animationComponent.playAutomatically = playAutomatically;
			
			if(playAutomatically){
	            animationComponent.Play(clipToPlay);	
			}else{
	            animationComponent.Play("default");
			}
		}
	
	    public override void Update()
	    {
	        if (isPlayingAnimation)
	        {
	            if (Time.time > startTime + duration)
	            {
	                if (OnCompleteCallback != null)
	                {
	                    OnCompleteCallback();
	                }
	
	                if (currentClip.wrapMode != WrapMode.Loop)
	                {
	                    isPlayingAnimation = false;
	                }
	
	                if (destroyOnComplete)
	                {
	                    Destroy(gameObject);
	                }
	            }
	        }
	    }
	
	    public void CrossFade(string id, PPAnimationCallback callback)
	    {
	        if (animationComponent == null || animationComponent[id] == null) return;
	        currentClip = GetAnimation(id);
	
	        animationComponent.CrossFade(id);
	
	        PrepareAnimationStart(callback);
		}
	
	    public void CrossFade(string id, float fadeLength, PPAnimationCallback callback)
	    {
	        if (animationComponent == null || animationComponent[id] == null) return;
	        currentClip = GetAnimation(id);
	
	        animationComponent.CrossFade(id, fadeLength);
	
	        PrepareAnimationStart(callback);
		}
	
	    public void Blend(string id, PPAnimationCallback callback)
	    {
	        if (animationComponent == null || animationComponent[id] == null) return;
	        currentClip = GetAnimation(id);
	
	        animationComponent.Blend(id);
	
	        PrepareAnimationStart(callback);
		}
	
	    public void Blend(string id, float weight, float length, PPAnimationCallback callback)
	    {
	        if (animationComponent == null || animationComponent[id] == null) return;
	        currentClip = GetAnimation(id);
	
	        animationComponent.Blend(id, weight, length);
	
	        PrepareAnimationStart(callback);
		}
		
		public void Stop(){
	        if (animationComponent == null) return;
	
	        animationComponent.Stop();
	
	        isPlayingAnimation = false;
		}
	
		public void Stop(string name){
	        if (animationComponent == null) return;
	
	        animationComponent.Stop(name);
	
	        isPlayingAnimation = false;
		}	
		
	    public void Play(string id, PPAnimationCallback callback)
	    {
	        if (animationComponent ==null || animationComponent[id] == null) return;
	        currentClip = GetAnimation(id);
	
	        animationComponent.Play(id);
	
	        PrepareAnimationStart(callback);
	    }
	
	    public void Play(string id)
	    {
	        Play(id, null);
	    }
	
	    public void PlayQueued(string id, QueueMode mode)
	    {
	        if (animationComponent == null || animationComponent[id] == null) return;
	        currentClip = GetAnimation(id);
	
	        animationComponent.PlayQueued(id, mode);
	    }
	
		public AnimationClip GetAnimation(string id){
	        return animationComponent.GetClip(id);	
		}
	
	    private void PrepareAnimationStart(PPAnimationCallback callback)
	    {       
	        startTime = Time.time;
	        duration = currentClip.length;
	        
	        OnCompleteCallback = callback;
	
	        isPlayingAnimation = true;
	    }
	}
}