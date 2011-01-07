using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

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
            scene.WriteElement("playAutomatically", anim.playAutomatically);
            string[] clips = new string[anim.GetClipCount()];
            int index = 0;
            foreach (AnimationState state in anim)
            {
                clips[index++] = state.name;
            }
            scene.WriteElement("animations", clips);
        }
        #endregion
    }
}
