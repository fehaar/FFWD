using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test.ScriptTranslation
{
    [TestFixture]
    public class WhenAScriptHasAttributes
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
        public void WeWillReplaceAttributes()
        {
            ScriptTranslator.ReplaceAttributes = new System.Collections.Generic.Dictionary<string, string>() { { "HideInInspector", "InspectorGadget" } };
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.StringContaining("[InspectorGadget]"));
            Assert.That(newScript, Is.Not.StringContaining("[HideInInspector]"));
        }

        [Test]
        public void WeCanRemoveAnAttribute()
        {
            ScriptTranslator.ReplaceAttributes = new System.Collections.Generic.Dictionary<string, string>() { { "HideInInspector", "" } };
            ScriptTranslator trans = new ScriptTranslator(testScript);
            trans.Translate();
            string newScript = trans.ToString();

            Assert.That(newScript, Is.Not.StringContaining("[]"));
            Assert.That(newScript, Is.Not.StringContaining("[HideInInspector]"));
        }

    }
}
