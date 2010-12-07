using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class ObjectReference : Component
    {
        public int Id;

        public override GameObject gameObject
        {
            get
            {
                if (base.gameObject == null)
                {
                    UnityObject obj = Application.Instance.Find(Id);
                    base.gameObject = (obj is GameObject) ? (obj as GameObject) : (obj as Component).gameObject;
                }
                return base.gameObject;
            }
            internal set
            {
                base.gameObject = value;
            }
        }
    }
}
