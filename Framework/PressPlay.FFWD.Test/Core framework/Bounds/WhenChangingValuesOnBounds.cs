using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenChangingValuesOnBounds
    {
        [Test]
        public void WeCanSetTheMinMaxWithoutMovingTheCenter()
        {
            Bounds b = new Bounds(Vector3.zero, Vector3.one);
            Vector3 min = b.min;
            Vector3 max = b.max;
            Vector3 center = b.center;

            Vector3 newMin = min - Vector3.one;
            Vector3 newMax = max + Vector3.one;
            b.SetMinMax(newMin, newMax);

            Assert.That(b.min, Is.EqualTo(newMin));
            Assert.That(b.max, Is.EqualTo(newMax));
            Assert.That(b.center, Is.EqualTo(center));
        }

        [Test]
        public void WeCanSetTheMinMaxAndMoveTheCenter()
        {
            Bounds b = new Bounds(Vector3.zero, Vector3.one);
            Vector3 min = b.min;
            Vector3 max = b.max;
            Vector3 center = b.center;

            Vector3 newMax = max + Vector3.one;
            b.SetMinMax(min, newMax);

            Assert.That(b.min, Is.EqualTo(min));
            Assert.That(b.max, Is.EqualTo(newMax));
            Assert.That(b.center, Is.Not.EqualTo(center));
        }
    }
}
