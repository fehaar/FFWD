using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test
{
    [TestFixture]
    public class WhenCreatingAScriptStub
    {
        private string[] testScript = new string[] {
            "using UnityEngine;",
            "using SomeSystem;",
            "",
            "public class TestScript : TestBase {",
            "\tvoid Start() {",
            "\t\tVector3 dir = Vector3.up + Vector3.forward;",
            "\t\tif (dir==Vector3.zero) return;",
            "\t}",
            "",
            "\tvoid Update() {",
            "\t}",
            "}"
        };

        [Test]
        public void WeWillRemoveOldUsings()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.CreateStub();
            string newScript = trans.ToString();
            Assert.That(newScript, Is.Not.StringContaining("UnityEngine;"));
            Assert.That(newScript, Is.Not.StringContaining("SomeSystem;"));
        }

        [Test]
        public void WeWillAddTheDefinedUsings()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.CreateStub();
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
            trans.CreateStub();
            string newScript = trans.ToString();
            Assert.That(newScript, Is.StringContaining("using " + myNamespace + ";"));
        }


        [Test]
        public void WeWillAddANamespaceAfterUsingStatements()
        {
            ScriptTranslator.ScriptNamespace = "TestNamespace";
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.CreateStub();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.StringContaining(ScriptTranslator.ScriptNamespace));
            Assert.That(newScript.IndexOf("using"), Is.LessThan(newScript.IndexOf(ScriptTranslator.ScriptNamespace)));
        }

        [Test]
        public void WeWillGetTheClassDefinitionAsWritten()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.CreateStub();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.StringContaining(testScript.First(s => s.Contains(" class "))));
        }

        [Test]
        public void AllCodeInTheClassWillBeRemoved()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.CreateStub();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.Not.StringContaining(testScript.First(s => s.Contains("Start()"))));
            Assert.That(newScript, Is.Not.StringContaining(testScript.First(s => s.Contains("Vector3"))));
            Assert.That(newScript, Is.Not.StringContaining(testScript.First(s => s.Contains("Update()"))));
        }
    }
}
