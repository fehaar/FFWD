using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using System.Collections;

namespace PressPlay.Tentacles.Scripts {
    public class PPAnimationClip
    {
        public PPAnimationClip()
        {
            speed = 1;
            randomizeStartTime = false;
            wrapMode = WrapMode.Default;
        }

	    public string id { get; set; }
        public int firstFrame { get; set; }
        public int lastFrame { get; set; }
        public bool addLoopFrame { get; set; }
        public float speed { get; set; }
        public bool randomizeStartTime { get; set; }	
	    public WrapMode wrapMode { get; set; }
    }
}