using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Exporter.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this String toCapitalize)
        {
            if (toCapitalize.Length > 1)
            {
                toCapitalize = toCapitalize.Substring(0, 1).ToUpper() + toCapitalize.Substring(1);
            }
            return toCapitalize;
        }
    }
}
