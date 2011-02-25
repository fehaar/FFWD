using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public class TransformAnimation : Component, PressPlay.FFWD.Interfaces.IUpdateable
    {
        public Matrix BaseTransform { get; set; }

        private Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
        private Dictionary<string, AnimationState> states = new Dictionary<string, AnimationState>();

        private GameObjectAnimationPlayer player;
        private string key;

        public override void Awake()
        {
            player = new GameObjectAnimationPlayer();
            if (!String.IsNullOrEmpty(key))
	        {
                player.StartClip(clips[key]);
	        }
        }

        #region IUpdateable Members
        public void Update()
        {
            if (player != null)
            {
                player.Update();
            }
            Matrix t;
            player.GetTransform(out t);
            t = BaseTransform * t;
            t.Translation *= 0.01f;
            transform.SetLocalTransform(t);
        }

        public void LateUpdate()
        {
        }
        #endregion

        internal void AddClip(AnimationClip animationClip, string name)
        {
            clips.Add(name, animationClip);
            states.Add(name, new AnimationState() { length = animationClip.length, wrapMode = animationClip.wrapMode });
            if (String.IsNullOrEmpty(key))
            {
                key = name;
            }
        }
    }
}
