using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public enum TouchPhase { Began, Moved, Stationary, Ended, Canceled }

    public struct Touch
    {
        public int fingerId;
        public Vector2 position;
        public Vector2 deltaPosition;
        public Vector2 deltaTime;
        public int tapCount;
        public TouchPhase phase;
    }
}
