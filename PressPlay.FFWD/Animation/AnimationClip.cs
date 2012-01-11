#region File Description
//-----------------------------------------------------------------------------
// AnimationClip.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    /// <summary>
    /// A model animation clip is the runtime equivalent of the
    /// Microsoft.Xna.Framework.Content.Pipeline.Graphics.AnimationContent type.
    /// It holds all the keyframes needed to describe a single model animation.
    /// </summary>
    public class AnimationClip : UnityObject
    {
        internal AnimationClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
        {
            // TODO: Reimplement this
            throw new NotImplementedException();
        }

        [ContentSerializer]
        public float length { get; internal set; }
        public string name;
        public WrapMode wrapMode = WrapMode.Once;
        [ContentSerializer]
        internal AnimationClipCurveData[] curves;

        private Sampler[] samplers;

        /// <summary>
        /// Private constructor for use by the XNB deserializer.
        /// </summary>
        internal AnimationClip()
        {
        }

        internal void InitializeSamplers(GameObject g)
        {
            Dictionary<string, Sampler> samps = new Dictionary<string, Sampler>();
            for (int i = 0; i < curves.Length; i++)
            {
                AnimationClipCurveData curveData = curves[i];
                string sampleKey = curveData.path + "/" + curveData.type + "." + ((curveData.propertyName.Contains(".")) ? curveData.propertyName.Substring(0, curveData.propertyName.LastIndexOf('.')) : curveData.propertyName);

                Sampler sampler = null;
                if (!samps.ContainsKey(sampleKey))
	            {
                    GameObject go = g;
                    if (!String.IsNullOrEmpty(curveData.path))
                    {
                        go = g.transform.Find(curveData.path).gameObject;
                    }

                    sampler = GetSampler(go, curveData);
                    if (sampler != null)
	                {
                        Debug.Log("Added a sampler for " + sampleKey);
                        samps.Add(sampleKey, sampler);
	                }
	            }

                if (sampler != null)
                {
                    sampler.AddCurveData(curveData);
                }
            }
            samplers = new Sampler[samps.Count];
            samps.Values.CopyTo(samplers, 0);
        }

        private Sampler GetSampler(GameObject g, AnimationClipCurveData curveData)
        {
            if (curveData.type == typeof(Transform).FullName && curveData.propertyName.StartsWith("m_LocalPosition"))
            {
                return new Vector3Sampler(g.transform, "localPosition");
            }
            return null;
        }

        internal void Sample(float time)
        {
            for (int i = 0; i < samplers.Length; i++)
            {
                samplers[i].Sample(time);
            }
        }
    }
}
