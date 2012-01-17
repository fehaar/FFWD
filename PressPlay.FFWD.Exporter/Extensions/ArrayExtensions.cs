using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Exporter.Extensions
{
    public static class ArrayExtensions
    {
        public static bool HasElements(this Array array)
        {
            return array != null && array.Length > 0;
        }
    }
}
