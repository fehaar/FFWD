using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    internal class SampleTester<T>
    {
        public T property { get; set; }
        public T field;
        private T privateField;
        public void Method() { }

        public T GetPrivateField()
        {
            return privateField;
        }
    }
}
