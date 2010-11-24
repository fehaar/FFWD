using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace PressPlay.FFWD.Interfaces
{
    /// <summary>
    /// This interface denotes a class that will process contactes that have been collected in the world
    /// </summary>
    public interface IContactProcessor: IContactListener
    {
        void Update();
    }
}
