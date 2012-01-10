using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PressPlay.FFWD.Exporter
{
    public class ScriptTranslator
    {
        public ScriptTranslator(string[] originalScriptText)
        {
            scriptLines = new List<string>(originalScriptText.Select(s => s.TrimEnd('\r')));
        }

        public static string ScriptNamespace { get; set; }
        private List<string> scriptLines;
        public static List<string> UsingsThatShouldBeKept = new List<string> { "System", "System.Collections.Generic", "System.Text", "System.Diagnostics", "System.Xml", "System.Xml.Linq", "System.IO", "Microsoft.Xna.Framework.Media", "System.Collections.Specialized", "System.Linq" };
        public static List<string> DefaultUsings = new List<string> { "PressPlay.FFWD", "PressPlay.FFWD.Components", "Microsoft.Xna.Framework.Content" };
        public static Dictionary<string, string> ReplaceAttributes = new Dictionary<string, string>() { 
            { "HideInInspector", "ContentSerializerIgnore" },
            { "System.Serializable", "" },
            { "System.NonSerialized", "ContentSerializerIgnore" },
            { "Serializable", "" },
            { "AddComponentMenu", "" },
            { "RequireComponent", "" },
            { "ExecuteInEditMode", "" },
            { "FFWD_ExportOptions", "" },
            { "FFWD_DontExport", "" }
        };
        public static Dictionary<string, string> ReplaceClasses = new Dictionary<string, string>() {
            { "Object", "UnityObject" }
        };
        public static string[] MethodsToOverride = new string[] { "Start", "Update", "FixedUpdate", "LateUpdate", "Awake", "OnTriggerEnter", "OnTriggerExit", "OnTriggerStay", "OnCollisionEnter", "OnCollisionExit", "OnCollisionStay", "OnGUI", "OnEnable", "OnDisable" };

        public void Translate()
        {
            ReplaceUsings();

            InsertNameSpace();

            OverrideMethods();

            ReplaceAllAttributes();

            AddFFWDNamespace();

            ReplaceAllClasses();
        }

        private void ReplaceAllClasses()
        {
            foreach (var item in ReplaceClasses)
            {
                int line = -1;
                Regex rex = new Regex(String.Format(@"(\W){0}(\W)", item.Key));
                while ((line = scriptLines.FindIndex(s => rex.IsMatch(s))) > -1)
                {
                    scriptLines[line] = rex.Replace(scriptLines[line], "$1" + item.Value + "$2");
                }
            }
        }

        private void AddFFWDNamespace()
        {
            int line = -1;
            Regex rex = new Regex(@"UnityEngine\.");
            while ((line = scriptLines.FindIndex(s => rex.IsMatch(s))) > -1)
            {
                scriptLines[line] = rex.Replace(scriptLines[line], "PressPlay.FFWD.");
            }

            rex = new Regex(@"([^.])Random\.");
            while ((line = scriptLines.FindIndex(s => rex.IsMatch(s))) > -1)
            {
                scriptLines[line] = rex.Replace(scriptLines[line], "$1PressPlay.FFWD.Random.");
            }
        }

        private void ReplaceAllAttributes()
        {
            foreach (var item in ReplaceAttributes)
            {
                int line = -1;
                Regex rex = new Regex(String.Format(@"(\[{0}(\(.+\))?\])", item.Key));
                Match m;
                while ((line = scriptLines.FindIndex(s => (m = rex.Match(s)).Success)) > -1)
                {                    
                    if (String.IsNullOrEmpty(item.Value))
                    {
                        scriptLines[line] = rex.Replace(scriptLines[line], "");
                    }
                    else
                    {
                        scriptLines[line] = rex.Replace(scriptLines[line], "[" + item.Value + "$2]");
                    }
                }
            }
        }

        private void OverrideMethods()
        {
            foreach (string method in MethodsToOverride)
            {
                Regex methEx = new Regex(@"void\s+" + method + @"\s*\(");
                int startLine = scriptLines.FindIndex(s => methEx.IsMatch(s));
                if (startLine >= 0)
                {
                    scriptLines[startLine] = scriptLines[startLine]
                        .Replace("public ", "")
                        .Replace("protected ", "")
                        .Replace("override ", "")
                        .Replace("virtual ", "");
                    scriptLines[startLine] = scriptLines[startLine].Replace("void", "public override void");
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
            scriptLines.RemoveAll(s => s.StartsWith("using ") && UsingsThatShouldBeKept.FindIndex(us => s.Contains(" " + us + ";")) == -1);
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
