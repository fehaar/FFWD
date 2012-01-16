using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class AnimationClipCurveData
    {
        public string path;
        public string propertyName;
        [ContentSerializer(Optional=true)]
        public string type;
        public AnimationCurve curve;
    }
}
