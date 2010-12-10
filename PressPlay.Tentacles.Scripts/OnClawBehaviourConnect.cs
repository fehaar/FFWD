using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;

namespace PressPlay.Tentacles.Scripts
{
    public class OnClawBehaviourConnect : MonoBehaviour
    {
        public delegate void DoOnConnectDelegate(ClawBehaviour _clawBehaviour, Vector3 _hitDir);
        public DoOnConnectDelegate doOnConnectDelegate;

        public virtual void DoOnClawBehaviourConnect(ClawBehaviour _clawBehaviour, Vector3 _hitDir)
        {
            if (doOnConnectDelegate != null)
            { doOnConnectDelegate(_clawBehaviour, _hitDir); }
        }
    }
}
