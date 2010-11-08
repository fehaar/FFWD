using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.U2X.Xna.Test.Core_framework
{
    [TestFixture]
    public class WhenLoadingAScene
    {
        Scene scene;
        GameObject go;
        TestComponent component;

        [SetUp]
        public void Setup()
        {
            scene = new Scene();
            go = new GameObject();
            component = new TestComponent();

            go.components.Add(component);
            scene.gameObjects.Add(go);
        }

        [Test]
        public void AfterLoadWillEnsureThatAllReferencesAreSetOnComponents()
        {
            Assert.That(component.gameObject, Is.Null);
            scene.AfterLoad();
            Assert.That(component.gameObject, Is.Not.Null);
            Assert.That(component.gameObject, Is.EqualTo(go));
        }
	
    }
}
