using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public interface ILifecycleTracker
    {
        void TrackEvent(UnityObject obj, string eventName);
    }
}
