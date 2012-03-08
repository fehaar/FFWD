using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

namespace PressPlay.FFWD.Farseer.Collision
{
    public interface IRayCastCallback
    {
        float RayCastCallback(ref RayCastInput input, int proxyId);
    }

    public interface IElementRayCastCallback<T>
    {
        float RayCastCallback(ref RayCastInput input, Element<T> proxyId);
    }
}
