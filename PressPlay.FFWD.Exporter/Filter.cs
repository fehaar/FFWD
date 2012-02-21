using System;
using System.Collections.Generic;
using System.Text;

namespace PressPlay.FFWD.Exporter
{
    public class Filter
    {
        public enum FilterType { None, ExcludeAll, IncludeAll, Include, Exclude }

        public FilterType filterType { get; set; }

        public IEnumerable<string> items { get; set; }

        public bool Includes(string item)
        {
            if (filterType == FilterType.IncludeAll) return true;
            if (filterType == FilterType.ExcludeAll) return false;

            bool result = false;
            // TODO: Change to LINQ for .NET 4
            foreach (string i in items)
            {
                if (i.Equals(item, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = true;
                }
            }
            return (filterType == FilterType.Include) ? result : !result;
        }

        public bool Excludes(string item)
        {
            return !Includes(item);
        }
    }
}
