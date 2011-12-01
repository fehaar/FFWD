using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Transforms
{
    [TestFixture]
    public class WhenGettingChildren
    {
        TestHierarchy h;

        [SetUp]
        public void CreateHierarchy( )
        {
            h = new TestHierarchy();
        }
	
        [Test]
        public void WeCanFindAChildByName()
        {
            Transform t = h.rootTrans.FindChild("child");
            Assert.That(t, Is.Not.Null);
            Assert.That(t, Is.SameAs(h.childTrans));
        }

        [Test]
        public void WeCanFindAChildByNameUsingFind()
        {
            Transform t = h.rootTrans.Find("child");
            Assert.That(t, Is.Not.Null);
            Assert.That(t, Is.SameAs(h.childTrans));
        }

        [Test]
        public void WeCanOnlyFindDirectDescendants()
        {
            Transform t = h.rootTrans.FindChild("childOfChild");
            Assert.That(t, Is.Null);
        }

        [Test]
        public void WeCanUseAPathConstruct()
        {
            Transform t = h.rootTrans.FindChild("child/childOfChild");
            Assert.That(t, Is.Not.Null);
            Assert.That(t, Is.SameAs(h.childOfChildTrans));
        }

        [Test]
        public void PathConstructsCanFail()
        {
            Transform t = h.rootTrans.FindChild("child/childOfChildness");
            Assert.That(t, Is.Null);
        }

        [Test]
        public void WeCanGetAChildByIndex()
        {
            Transform t = h.rootTrans.GetChild(0);
            Assert.That(t, Is.Not.Null);
            Assert.That(t, Is.SameAs(h.childTrans));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QWeWillGetAnExceptionWhenAskingForAnOutOfRangeChild()
        {
            Transform t = h.rootTrans.GetChild(10);
        }
	
	
    }
}
