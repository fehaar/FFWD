using System;
using System.IO;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test
{
	[TestFixture]
	public class WhenTranslatingAScript
	{
        private string[] testScript = new string[] {
            "using UnityEngine;",
            "using SomeSystem;",
            "",
            "public class TestScript : MonoBehaviour {",
            "[HideInInspector]",
            "public string iAmLegion;",
            "\tvoid Start() {",
            "\t\tVector3 dir = Vector3.up + Vector3.forward;",
            "\t\tif (dir==Vector3.zero) return;",
            "\t}",
            "",
            "\tpublic void Update() {",
            "\t}",
            "\tvirtual protected void FixedUpdate() {",
            "\t}",
            "}"
        };

        [Test]
        public void WeWillRemoveOldUsings()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();
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


        [Test]
        public void WeWillAddANamespaceAfterUsingStatements()
        {
            ScriptTranslator.ScriptNamespace = "TestNamespace";
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.StringContaining(ScriptTranslator.ScriptNamespace));
            Assert.That(newScript.IndexOf("using"), Is.LessThan(newScript.IndexOf(ScriptTranslator.ScriptNamespace)));
        }

        [Test]
        public void WeWillOverrideTheStartMethod()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.StringContaining("public override void Start"));
        }

        [Test]
        public void WeWillOverrideTheUpdateMethod()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.StringContaining("public override void Update"));
            Assert.That(newScript, Is.Not.StringContaining("public public override void Update"));
        }

        [Test]
        public void WeWillOverrideTheFixedUpdateMEthodEvenWithABrokenSignature()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.StringContaining("public override void FixedUpdate"));
            Assert.That(newScript, Is.Not.StringContaining("virtual protected public override void"));
        }
	
        [Test]
        public void WeCanTranslateTheTestScripts()
        {
            foreach (string filename in Directory.GetFiles("TestScripts", "*.cs"))
            {
                try
                {
                    ScriptTranslator trans = new ScriptTranslator(File.ReadAllLines(filename));
                    trans.Translate();
                }
                catch (Exception ex)
                {
                    Assert.Fail("Exception while translating " + Path.GetFileNameWithoutExtension(filename) + ": " + ex.Message);
                }
            }
            Assert.Pass("We have translated all scripts");
        }
	}
}
