using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Interfaces
{
    public interface IFixedUpdateable
    {
        GameObject gameObject { get; }

        void FixedUpdate();
    }
}
