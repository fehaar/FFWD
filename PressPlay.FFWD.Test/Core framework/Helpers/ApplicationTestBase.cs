using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public abstract class ApplicationTestBase
    {
        [TearDown]
        public void TearDown()
        {
            Application.AwakeNewComponents();
            Application.Reset();
        }
    }
}
