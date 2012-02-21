using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FarseerPhysics.Collision;

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

        [Test]
        public void WeCanBuildItFromAnAABB()
        {
            AABB aabb = new AABB(new Microsoft.Xna.Framework.Vector2(-10), new Microsoft.Xna.Framework.Vector2(10));
            Bounds b = Bounds.FromAABB(ref aabb, Physics.To2dMode.DropZ, Vector3.one);
            Assert.That(b.min, Is.EqualTo(new Vector3(-10, -10, -0.5f)));
            Assert.That(b.max, Is.EqualTo(new Vector3(10, 10, 0.5f)));
        }


        [Test]
        public void WeCanBuildItFromAnAABBOffCenter()
        {
            AABB aabb = new AABB(Microsoft.Xna.Framework.Vector2.Zero, Microsoft.Xna.Framework.Vector2.One);
            Bounds b = Bounds.FromAABB(ref aabb, Physics.To2dMode.DropY, Vector3.one * 4);
            Assert.That(b.min, Is.EqualTo(new Vector3(0, -2f, 0)));
            Assert.That(b.max, Is.EqualTo(new Vector3(1, 2f, 1)));
        }

    }
}
