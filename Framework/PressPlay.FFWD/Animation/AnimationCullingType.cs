using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public enum AnimationCullingType
    {
        AlwaysAnimate,
        BasedOnRenderers,
        BasedOnClipBounds,
        BasedOnUserBounds
    }
}
