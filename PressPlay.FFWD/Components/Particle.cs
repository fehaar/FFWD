using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public struct Particle
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public float Energy;
        public float StartingEnergy;
        public float Size;
    }
}
