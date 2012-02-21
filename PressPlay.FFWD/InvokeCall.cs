using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    internal struct InvokeCall
    {
        internal string methodName;
        internal float time;
        internal float repeatRate;
        internal MonoBehaviour behaviour;

        public bool Update(float deltaTime)
        {
            time -= deltaTime;
            if (time <= 0)
            {
                return true;
            }
            return false;
        }
    }
}
