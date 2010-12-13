using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Components
{
    public class MonoBehaviour : Behaviour, IUpdateable
    {
        public virtual void Update()
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }
    }
}
