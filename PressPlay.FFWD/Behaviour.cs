using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class Behaviour : Component
    {
        [ContentSerializer(Optional=true)]
        public bool enabled { get; set; }
    }
}
