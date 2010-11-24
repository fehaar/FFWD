﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace PressPlay.U2X.Xna.Test.Physics_facts
{
    [TestFixture]
    public class WhenPerformingARaycast
    {
        [SetUp]
        public void Setup()
        {
            Physics.Initialize();
        }
	
        [Test]
        public void WeCanDetermineAColliderHit()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            bool hit = Physics.Raycast(Vector2.Zero, Vector2.UnitY, 100, 0);
            Assert.That(hit, Is.True);
        }

        [Test]
        public void WeCanDetermineAColliderHitUsing3d()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            bool hit = Physics.Raycast(Vector3.Zero, Vector3.UnitZ, 100, 0);
            Assert.That(hit, Is.True);
        }

        [Test]
        public void WeCanDetermineAColliderHitUsingRay()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            Ray ray = new Ray(Vector3.Zero, Vector3.UnitZ);
            bool hit = Physics.Raycast(ray, 100, 0);
            Assert.That(hit, Is.True);
        }

        [Test]
        public void WeCanMissAllColliders()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            bool hit = Physics.Raycast(Vector2.Zero, Vector2.UnitX, 100, 0);
            Assert.That(hit, Is.False);
        }

        [Test]
        public void WeCanMissAllCollidersUsing3d()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            bool hit = Physics.Raycast(Vector2.Zero, Vector2.UnitX, 100, 0);
            Assert.That(hit, Is.False);
        }

        [Test]
        public void WeCanMissAllCollidersUsingRay()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            Ray ray = new Ray(Vector3.Zero, Vector3.UnitX);
            bool hit = Physics.Raycast(ray, 100, 0);
            Assert.That(hit, Is.False);
        }

        [Test]
        public void WeWillNotHitAColliderIfTheRayStartsInsideIt()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 0), 0, 1);
            bool hit = Physics.Raycast(Vector2.Zero, Vector2.UnitY, 100, 0);
            Assert.That(hit, Is.False);
        }

        [Test]
        public void WeCanGetHitInfoOnTheObjectThatWasHit()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            RaycastHit hit = new RaycastHit();
            bool hasHit = Physics.Raycast(Vector2.Zero, Vector2.UnitY, out hit, 100, 0);
            Assert.That(hasHit, Is.True);
            Assert.That(hit.body, Is.SameAs(body));
            Assert.That(hit.point, Is.EqualTo(new Vector2(0, 45)));
            Assert.That(hit.normal, Is.EqualTo(new Vector2(0, -1)));
            Assert.That(hit.distance, Is.EqualTo(45));
        }

        [Test]
        public void WeWillGetInfOnTheClosestObjectHit()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 20), 0, 1);
            Body body1 = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            RaycastHit hit = new RaycastHit();
            bool hasHit = Physics.Raycast(Vector2.Zero, Vector2.UnitY, out hit, 100, 0);
            Assert.That(hasHit, Is.True);
            Assert.That(hit.body, Is.SameAs(body));
        }

        [Test]
        public void WeWillGetHitsOnAllObjectsOnTheRay()
        {
            Body body = Physics.AddBox(10, 10, new Vector2(0, 20), 0, 1);
            Body body1 = Physics.AddBox(10, 10, new Vector2(0, 50), 0, 1);
            RaycastHit[] hits = Physics.RaycastAll(Vector2.Zero, Vector2.UnitY, 100, 0);
            Assert.That(hits.Length, Is.EqualTo(2));
        }
	
    }
}