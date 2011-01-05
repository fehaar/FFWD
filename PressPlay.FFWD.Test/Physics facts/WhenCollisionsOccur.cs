using NUnit.Framework;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD.Test.Physics_facts
{
    [TestFixture]   
    public class WhenCollisionsOccur
    {
        class MyBehaviour : MonoBehaviour
        {
            public bool collisionEnterCalled = false;
            public bool collisionStayCalled = false;
            public bool collisionExitCalled = false;
            public bool triggerEnterCalled = false;
            public bool triggerExitCalled = false;
            public bool triggerStayCalled = false;

            public override void OnCollisionEnter(Collision collision)
            {
                collisionEnterCalled = true;
            }

            public override void OnCollisionStay(Collision collision)
            {
                collisionStayCalled = true;
            }

            public override void OnCollisionExit(Collision collision)
            {
                collisionExitCalled = true;
            }

            public override void OnTriggerEnter(Collider collider)
            {
                triggerEnterCalled = true;
            }

            public override void OnTriggerStay(Collider collider)
            {
                triggerStayCalled = true;
            }

            public override void OnTriggerExit(Collider collider)
            {
                triggerExitCalled = true;
            }
        }

        [Test]
        public void CollisionEnterWillBeCalledOnMonoBehavioursOnTheGameObject()
        {
            GameObject go = new GameObject();
            MyBehaviour beh = new MyBehaviour();
            go.AddComponent(beh);

            go.OnCollisionEnter(null);

            Assert.That(beh.collisionEnterCalled, Is.True);
        }

        [Test]
        public void CollisionExitWillBeCalledOnMonoBehavioursOnTheGameObject()
        {
            GameObject go = new GameObject();
            MyBehaviour beh = new MyBehaviour();
            go.AddComponent(beh);

            go.OnCollisionExit(null);

            Assert.That(beh.collisionExitCalled, Is.True);
        }

        [Test]
        public void CollisionStayWillBeCalledOnMonoBehavioursOnTheGameObject()
        {
            GameObject go = new GameObject();
            MyBehaviour beh = new MyBehaviour();
            go.AddComponent(beh);

            go.OnCollisionStay(null);

            Assert.That(beh.collisionStayCalled, Is.True);
        }

        [Test]
        public void TriggerEnterWillBeCalledOnMonoBehavioursOnTheGameObject()
        {
            GameObject go = new GameObject();
            MyBehaviour beh = new MyBehaviour();
            go.AddComponent(beh);

            go.OnTriggerEnter(null);

            Assert.That(beh.triggerEnterCalled, Is.True);
        }

        [Test]
        public void TriggerExitWillBeCalledOnMonoBehavioursOnTheGameObject()
        {
            GameObject go = new GameObject();
            MyBehaviour beh = new MyBehaviour();
            go.AddComponent(beh);

            go.OnTriggerExit(null);

            Assert.That(beh.triggerExitCalled, Is.True);
        }

        [Test]
        public void TriggerStayWillBeCalledOnMonoBehavioursOnTheGameObject()
        {
            GameObject go = new GameObject();
            MyBehaviour beh = new MyBehaviour();
            go.AddComponent(beh);

            go.OnTriggerStay(null);

            Assert.That(beh.triggerStayCalled, Is.True);
        }
    }
}
