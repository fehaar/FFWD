using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PressPlay.FFWD.Extensions
{
    public static class ArrayExtensions
    {
        public static bool HasElements(this Array array)
        {
            return array != null && array.Length > 0;
        }

        public static bool HasElements(this IList list)
        {
            return list != null && list.Count > 0;
        }

        public static bool HasElements(this IDictionary dict)
        {
            return dict != null && dict.Count > 0;
        }
    }
}
