using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using UnityEditor;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class AnimationWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            Animation anim = component as Animation;
            if (anim == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            if (anim.clip != null)
            {
                scene.WriteElement("clip", anim.clip.GetInstanceID());
            }
            scene.WriteElement("playAutomatically", anim.playAutomatically);
            scene.WriteElement("wrapMode", anim.wrapMode);
            AnimationClip[] clips = AnimationUtility.GetAnimationClips(anim);
            int[] clipIds = new int[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                clipIds[i] = clips[i].GetInstanceID();
                scene.AddAnimationClip(clips[i]);
            }
            scene.WriteElement("clips", clipIds);
        }
        #endregion
    }
}
