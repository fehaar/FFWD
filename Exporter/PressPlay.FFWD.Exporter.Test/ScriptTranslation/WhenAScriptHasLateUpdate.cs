using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test.ScriptTranslation
{
    [TestFixture]
    public class WhenAScriptHasLateUpdate
    {
        private string[] testScript = new string[] {
            "public class TestScript : MonoBehaviour {",
            "\tvoid LateUpdate() {",
            "\t}",
            "}"
        };

        [Test]
        public void WeWillOverrideTheMethod()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.StringContaining("public override void LateUpdate"));
        }
    }
}
