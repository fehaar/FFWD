using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test.ScriptTranslation
{
    [TestFixture]
    public class WhenConvertingTheUsingClauses
    {
        private string[] testScript = new string[] {
            "using System;",
            "using SysDebug = System.Diagnostics;",
            "using UnityEngine;",
            "using SomeSystem;",
            "",
            "public class TestScript : MonoBehaviour {",
            "}"
        };

        [Test]
        public void WeWillRemoveUsingsNotSetToBeKept()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            ScriptTranslator.UsingsThatShouldBeKept = new List<string> { "System", "System.Diagnostics" };
            trans.Translate();
            string newScript = trans.ToString();
            Assert.That(newScript, Is.StringContaining("System;"));
            Assert.That(newScript, Is.StringContaining("System.Diagnostics;"));
            Assert.That(newScript, Is.Not.StringContaining("UnityEngine;"));
            Assert.That(newScript, Is.Not.StringContaining("SomeSystem;"));
        }

        [Test]
        public void WeWillAddTheDefinedUsings()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();

            foreach (string u in ScriptTranslator.DefaultUsings)
            {
                Assert.That(newScript, Is.StringContaining("using " + u + ";"));
            }
        }

        [Test]
        public void WeCanAddExtraUsings()
        {
            string myNamespace = "MyNamespace";
            ScriptTranslator trans = new ScriptTranslator(testScript);
            ScriptTranslator.DefaultUsings.Add(myNamespace);
            trans.Translate();
            string newScript = trans.ToString();
            Assert.That(newScript, Is.StringContaining("using " + myNamespace + ";"));
        }
    }
}
