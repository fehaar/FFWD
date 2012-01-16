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
            scene.WriteElement("enabled", anim.enabled);
            if (anim.clip != null)
            {
                scene.WriteElement("clip", scene.SanitizeFileName(anim.name + "-" + anim.clip.name));
            }
            scene.WriteElement("playAutomatically", anim.playAutomatically);
            scene.WriteElement("wrapMode", anim.wrapMode);
            AnimationClip[] clips = AnimationUtility.GetAnimationClips(anim);
            string[] clipNames = new string[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == null)
                {
                    continue;
                }
                clipNames[i] = scene.SanitizeFileName(anim.name + "-" + clips[i].name);
                scene.AddAnimationClip(clipNames[i], clips[i]);
            }
            scene.WriteElement("clips", clipNames);
        }
        #endregion
    }
}
