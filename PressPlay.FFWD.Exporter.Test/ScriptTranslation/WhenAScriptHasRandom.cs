using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test.ScriptTranslation
{
    [TestFixture]
    public class WhenAScriptHasRandom
    {
        private string[] testScript = new string[] {
            "using UnityEngine;",
            "using SomeSystem;",
            "",
            "public class TestScript : MonoBehaviour {",
            "\tpublic void Update() {",
            "\t\tint i = Random.Range(1, 12);",
            "\t\tint j = Random.Range(1, 12); int k = Random.Range(1, 12);",
            "\t\tif (Random.Range(1,10) > 5) { DoStuff(); }",
            "\t}",
            "}"
        };

        [Test]
        public void WeWillTranslateItToFFWDRandom()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();
            Assert.That(newScript, Is.StringContaining("PressPlay.FFWD.Random"));
            Assert.That(newScript, Is.Not.StringContaining("PressPlay.FFWD.PressPlay.FFWD."));
        }

        [Test]
        public void WeWillNotDeleteSymboldBeforeRandom()
        {
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();
            Assert.That(newScript, Is.StringContaining("(PressPlay.FFWD.Random"));
            Assert.That(newScript, Is.Not.StringContaining("(Random."));
        }
    }
}
