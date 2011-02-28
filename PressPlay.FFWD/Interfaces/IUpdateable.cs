using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Interfaces
{
    public interface IUpdateable
    {
        GameObject gameObject { get; }

        void Update();
        void LateUpdate();
    }
}
