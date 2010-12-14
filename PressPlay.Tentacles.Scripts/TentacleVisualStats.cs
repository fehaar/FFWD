using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts
{
    [System.Serializable]
    public class TentacleVisualStats
    {

        public int joints;
        public float startWidth;
        public float endWidth;
        public float jointLinearStiffnes;
        public float curvature = 0.05f;

        public int physicsIterations;
    }
}