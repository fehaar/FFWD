using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public struct LayerMask
    {
        public int value { get; set; }

        public static implicit operator int(LayerMask mask)
        {
            return mask.value;
        }

        public static implicit operator LayerMask(int mask)
        {
            return new LayerMask() { value = mask };
        }

        public static int NameToLayer(string name)
        {
            // TODO: Implement this!
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
