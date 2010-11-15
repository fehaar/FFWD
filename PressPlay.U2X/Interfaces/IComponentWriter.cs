using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.U2X.Writers;

namespace PressPlay.U2X.Interfaces
{
    public interface IComponentWriter
    {
        void Write(SceneWriter scene, UnityEngine.Component component);
    }
}
