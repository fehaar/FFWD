using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD.Test.Core_framework.Invoke
{
    [TestFixture]
    public class WhenInvokingAMethod
    {
        GameObject go;
        InvokeBehaviour comp;

        class InvokeBehaviour : MonoBehaviour
        {
            public bool invokeMethodCalled = false;

            public void DoSomething( )
            {
                invokeMethodCalled = true;
            }
        }

        [SetUp]
        public void Setup( )
        {
            go = new GameObject();
            comp = go.AddComponent(new InvokeBehaviour());
        }

        [TearDown]
        public void TearDown( )
        {
            Time.Reset();
        }

        [Test]
        public void WeCanSeeThatWeAreInvokingAMethod()
        {
            Assert.That(comp.IsInvoking("DoSomething"), Is.False);

            comp.Invoke("DoSomething", 2);

            Assert.That(comp.IsInvoking("DoSomething"), Is.True);
        }

        [Test]
        public void TheInvokeCallWillBeCalledWhenTheTimeHasPassed()
        {
            comp.Invoke("DoSomething", 1);

            Time.Update(1.0f, 1.0f);
            Application.UpdateInvokeCalls();

            Assert.That(comp.invokeMethodCalled, Is.True);
        }

        [Test]
        public void TheInvokedMethodWillNotBeCalledBeforeTimeHasPassed()
        {
            comp.Invoke("DoSomething", 2);

            Time.Update(1.0f, 1.0f);
            Application.UpdateInvokeCalls();

            Assert.That(comp.invokeMethodCalled, Is.False);
        }

        [Test]
        public void AfterTheInvokeCallHasBeenCalledItWillBeRemoved()
        {
            comp.Invoke("DoSomething", 1);

            Time.Update(1.0f, 1.0f);
            Application.UpdateInvokeCalls();

            Assert.That(comp.IsInvoking("DoSomething"), Is.False);
        }

        [Test]
        public void TheInvokeCallWillBeCalledWhenTimeRunsInIncrements()
        {
            comp.Invoke("DoSomething", 2);

            Time.Update(1.0f, 1.0f);
            Application.UpdateInvokeCalls();
            Time.Update(1.0f, 1.0f);
            Application.UpdateInvokeCalls();

            Assert.That(comp.invokeMethodCalled, Is.True);
        }
	
    }
}
