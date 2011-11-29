using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test.ScriptTranslation
{
    [TestFixture]
    public class WhenAScriptReferencesObject
    {
        private string[] testScript = new string[] {
            "using System;",
            "",
            "public class TestScript : MonoBehaviour {",
            "  public Object myObj;",
            "  public void DoInstantiate(Object myObject) {}",
            "}"
        };

        [Test]
        public void ItWillBeReplacedWithUnityObject()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();
            Assert.That(newScript, Is.StringContaining("UnityObject myObj;"));
            Assert.That(newScript, Is.StringContaining("(UnityObject myObject"));
        }
    }
}
