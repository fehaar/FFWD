using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Components
{
    internal class RenderQueue : SortedList<float, RenderItem>
    {
        public RenderQueue()
            : base(ApplicationSettings.DefaultCapacities.RenderQueues)
        {
        }
    }
}
