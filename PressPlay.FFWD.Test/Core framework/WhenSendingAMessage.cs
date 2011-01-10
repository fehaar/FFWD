using System.IO;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenSendingAMessage
    {
        class MessageComponent : Component
        {
            public bool messageCalled = false;
            public int value = 0;

            public void MessageMethod()
            {
                messageCalled = true;
            }	

            void NonPublicMessage()
            {
                messageCalled = true;
            }

            public void SetValue(int value)
            {
                this.value = value;
            }
        }

        class InheritedMessageComponent : MessageComponent
        {

        }

        [Test]
        public void WeCanSendAMessageToAComponentOnAGameObject()
        {
            GameObject go = new GameObject();
            MessageComponent comp = new MessageComponent();
            go.AddComponent(comp);

            go.SendMessage("MessageMethod");

            Assert.That(comp.messageCalled, Is.True);
        }

        [Test]
        public void WeCanSendAParameterWithTheMessage()
        {
            GameObject go = new GameObject();
            MessageComponent comp = new MessageComponent();
            go.AddComponent(comp);

            go.SendMessage("SetValue", 42);

            Assert.That(comp.value, Is.EqualTo(42));
        }

        [Test]
        public void WeCanCallANonPublicMethodWithTheMessage()
        {
            GameObject go = new GameObject();
            MessageComponent comp = new MessageComponent();
            go.AddComponent(comp);

            go.SendMessage("NonPublicMessage");

            Assert.That(comp.messageCalled, Is.True);
        }

        [Test]
        public void AnErrorIsLoggedIfWeRequireAReceiverAndNoMethodAnswers()
        {
            GameObject go = new GameObject();
            MessageComponent comp = new MessageComponent();
            go.AddComponent(comp);
            MemoryStream ms = new MemoryStream();
            System.Diagnostics.TextWriterTraceListener listener = new System.Diagnostics.TextWriterTraceListener(ms);
            System.Diagnostics.Debug.Listeners.Clear();
            System.Diagnostics.Debug.Listeners.Add(listener);

            go.SendMessage("NonExisting", 42, SendMessageOptions.RequireReceiver);

            listener.Flush();
            Assert.That(ms.Position, Is.GreaterThan(0));
        }

        [Test]
        public void NoErrorIsLoggedIfWeDontRequireAReceiver()
        {
            GameObject go = new GameObject();
            MessageComponent comp = new MessageComponent();
            go.AddComponent(comp);
            MemoryStream ms = new MemoryStream();
            System.Diagnostics.TextWriterTraceListener listener = new System.Diagnostics.TextWriterTraceListener(ms);
            System.Diagnostics.Debug.Listeners.Clear();
            System.Diagnostics.Debug.Listeners.Add(listener);

            go.SendMessage("NonExisting", 42, SendMessageOptions.DontRequireReceiver);

            listener.Flush();
            Assert.That(ms.Position, Is.EqualTo(0));
        }

        [Test]
        public void WeCanSendAMessageUpwards()
        {
            TestHierarchy h = new TestHierarchy();
            MessageComponent comp = new MessageComponent();
            h.root.AddComponent(comp);

            h.childOfChild.SendMessageUpwards("MessageMethod");

            Assert.That(comp.messageCalled, Is.True);
        }

        [Test]
        public void WeCanBroadcastAMessage()
        {
            TestHierarchy h = new TestHierarchy();
            MessageComponent comp = new MessageComponent();
            MessageComponent comp1 = new MessageComponent();
            h.child.AddComponent(comp);
            h.childOfChild.AddComponent(comp1);

            h.root.BroadcastMessage("MessageMethod");

            Assert.That(comp.messageCalled, Is.True);
            Assert.That(comp1.messageCalled, Is.True);
        }

        [Test]
        public void WeCanSendAMessageToAPublicBaseClassMethod()
        {
            GameObject go = new GameObject();
            InheritedMessageComponent comp = new InheritedMessageComponent();
            go.AddComponent(comp);

            go.SendMessage("MessageMethod");

            Assert.That(comp.messageCalled, Is.True);
        }

        [Test]
        public void WeCanSendAMessageToAPrivateBaseClassMethod()
        {
            GameObject go = new GameObject();
            InheritedMessageComponent comp = new InheritedMessageComponent();
            go.AddComponent(comp);

            go.SendMessage("NonPublicMessage");

            Assert.That(comp.messageCalled, Is.True);
        }
	
    }
}
