using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD
{
    public class LifecycleTracer : ILifecycleTracker
    {
        private static bool isRegistered = false;

        private static List<int> trackedInstances = new List<int>();

        public static void TrackInstance(int id)
        {
            if (!trackedInstances.Contains(id))
            {
                trackedInstances.Add(id);
            }
            if (!isRegistered)
            {
                Application.RegisterLifecycleTracker(new LifecycleTracer());
            }
        }

        public void TrackEvent(UnityObject obj, string eventName)
        {
            if (trackedInstances.Contains(obj.GetInstanceID()))
            {
                Debug.Log("Lifecycle event: " + eventName + " for " + obj);
            }
        }
    }
}
