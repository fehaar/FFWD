using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine;
using PressPlay.FFWD.Exporter.Extensions;

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
        public static List<string> DefaultUsings = new List<string> { "System", "System.Collections.Generic", "System.Text", "Microsoft.Xna.Framework", "PressPlay.FFWD", "PressPlay.FFWD.Components" };

        public void Translate()
        {
            // Replace usings
            scriptLines.RemoveAll(s => s.StartsWith("using"));
            //scriptLines.InsertRange(0, DefaultUsings.Select(s => "using " + s + ";"));
            DefaultUsings.ForEach(s => scriptLines.Insert(0, "using " + s + ";"));

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

            // Override methods
            string[] methods = new string[] { "Start", "Update" };
            foreach (string method in methods)
            {
                Regex methEx = new Regex(@"void\s+" + method + @"\s?\(");
                int startLine = scriptLines.FindIndex(s => methEx.IsMatch(s));
                scriptLines[startLine] = scriptLines[startLine].Replace("void", "public override void");
            }            

            // Replace Vector3 static method names
            Type tp = typeof(Vector3);
            foreach (PropertyInfo prop in tp.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                for (int i = 0; i < scriptLines.Count; i++)
                {
                    if (scriptLines[i].Contains("Vector3." + prop.Name))
                    {
                        scriptLines[i] = scriptLines[i].Replace("Vector3." + prop.Name, "Vector3." + StringExtensions.Capitalize(prop.Name));
                    }
                }
            }
        }

        public override string ToString()
        {
            return String.Join(Environment.NewLine, scriptLines.ToArray());
        }
   
    }
}
