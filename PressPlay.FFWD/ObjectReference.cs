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
                    base.gameObject = Application.Instance.Find(Id);
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
