using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PressPlay.FFWD.Exporter
{
    public class ScriptTranslator
    {
        public ScriptTranslator(string[] originalScriptText)
        {
            scriptLines = new List<string>(originalScriptText);
        }

        public static string ScriptNamespace { get; set; }
        private List<string> scriptLines;
        public static List<string> DefaultUsings = new List<string> { "System", "System.Collections.Generic", "System.Text", "PressPlay.FFWD", "PressPlay.FFWD.Components", "Microsoft.Xna.Framework.Content" };
        public static Dictionary<string, string> ReplaceAttributes = new Dictionary<string, string>() { { "HideInInspector", "ContentSerializerIgnore" } };

        public void Translate()
        {
            ReplaceUsings();

            InsertNameSpace();

            OverrideMethods();

            ReplaceAllAttributes();
        }

        private void ReplaceAllAttributes()
        {
            foreach (var item in ReplaceAttributes)
            {
                int line = -1;
                while ((line = scriptLines.FindIndex(s => s.Contains("[" + item.Key + "]"))) > -1)
                {
                    scriptLines[line] = scriptLines[line].Replace(item.Key, item.Value);
                }
            }
        }

        private void OverrideMethods()
        {
            string[] methods = new string[] { "Start", "Update", "Awake", "OnTriggerEnter", "OnTriggerExit", "OnTriggerStay", "OnCollisionEnter", "OnCollisionExit", "OnCollisionStay" };
            foreach (string method in methods)
            {
                Regex methEx = new Regex(@"void\s+" + method + @"\s?\(");
                int startLine = scriptLines.FindIndex(s => methEx.IsMatch(s));
                if (startLine >= 0)
                {
                    if (scriptLines[startLine].Contains("public void"))
                    {
                        scriptLines[startLine] = scriptLines[startLine].Replace("public void", "public override void");
                    }
                    else
                    {
                        scriptLines[startLine] = scriptLines[startLine].Replace("void", "public override void");
                    }
                }
            }
        }

        public void CreateStub()
        {
            ReplaceUsings();
            int classDef = scriptLines.FindIndex(s => s.Contains(" class "));
            scriptLines.RemoveRange(classDef + 1, scriptLines.Count - classDef - 1);
            scriptLines.Add("}");
            InsertNameSpace();
        }

        private void InsertNameSpace()
        {
            // Insert namespace
            if (!String.IsNullOrEmpty(ScriptNamespace))
            {
                int classDef = scriptLines.FindIndex(s => s.Contains(" class "));
                scriptLines.Insert(classDef, "namespace " + ScriptNamespace + " {");
                for (int i = classDef + 1; i < scriptLines.Count; i++)
                {
                    scriptLines[i] = "\t" + scriptLines[i];
                }
                scriptLines.Add("}");
            }
        }

        private void ReplaceUsings()
        {
            // Replace usings
            scriptLines.RemoveAll(s => s.StartsWith("using"));
            //scriptLines.InsertRange(0, DefaultUsings.Select(s => "using " + s + ";"));
            int line = 0;
            DefaultUsings.ForEach(s => scriptLines.Insert(line++, "using " + s + ";"));
        }

        public override string ToString()
        {
            return String.Join(Environment.NewLine, scriptLines.ToArray());
        }

    }
}
