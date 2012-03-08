using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenComparingTags
    {
        [Test]
        public void WeWillFindACorrectTag()
        {
            GameObject go = new GameObject();
            go.tag = "NewTag";
            Assert.That(go.CompareTag("NewTag"), Is.True);
        }

        [Test]
        public void WeWillNotFindOtherTags()
        {
            GameObject go = new GameObject();
            go.tag = "NewTag";
            Assert.That(go.CompareTag("OtherTag"), Is.False);
        }

        [Test]
        public void WeDisregardCasing()
        {
            GameObject go = new GameObject();
            go.tag = "NewTag";
            Assert.That(go.CompareTag("newtag"), Is.True);
        }
	
	
    }
}
