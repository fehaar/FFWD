using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test
{
    [TestFixture]
    public class WhenBuildingBounds
    {
        Mesh m;

        [SetUp]
        public void SetUp()
        {
            m = new Mesh();
        }

        [Test]
        public void BoundsWillContainAllVerticesAfterCalculation()
        {
            m.vertices = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) };
            m.RecalculateBounds();

            for (int i = 0; i < m.vertices.Length; i++)
            {
                Assert.That(m.bounds.Contains(m.vertices[i]));
            }
        }
	
    }
}
