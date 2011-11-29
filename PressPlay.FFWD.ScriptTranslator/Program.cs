using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace PressPlay.FFWD.ScriptTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] { @"UnitProperties" };
            }

            Program p = new Program();

            p.FindAllClasses();
            foreach (var item in args)
            {
                foreach (var scriptFile in p.FindScriptsToTranslate(item))
                {
                    Console.WriteLine("Converting script " + Path.GetFileName(scriptFile));
                    string newText = p.TranslateScript(scriptFile);
                    string newPath = scriptFile.Replace(ConfigurationManager.AppSettings["UnityScriptsBasePath"], ConfigurationManager.AppSettings["XNAScriptsBasePath"]);
                    if (!Directory.Exists(Path.GetDirectoryName(newPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    }
                    File.WriteAllText(newPath, newText);
                }
            }
        }

        public Program()
        {
            foreach (string file in Directory.GetFiles(ConfigurationManager.AppSettings["UnityScriptsBasePath"], "*.cs", SearchOption.AllDirectories))
            {
                scriptFiles[Path.GetFileNameWithoutExtension(file)] = file;
            }
        }

        private Dictionary<string, string> scriptFiles = new Dictionary<string,string>();
        private Dictionary<string, string> scriptTexts = new Dictionary<string, string>();
        private Dictionary<string, string> typeFiles = new Dictionary<string, string>();
        private Dictionary<string, HashSet<string>> typesInFile = new Dictionary<string, HashSet<string>>();

        private HashSet<string> visitedTypes = new HashSet<string>();

        private void FindAllClasses()
        {
            foreach (var path in scriptFiles.Values)
            {
                string file = File.ReadAllText(path);
                scriptTexts.Add(path, file);
                SyntaxTree tree = SyntaxTree.ParseCompilationUnit(file);
                var root = (CompilationUnitSyntax)tree.Root;

                for (int i = 0; i < root.Members.Count; i++)
                {
                    var types = root.Members[i].DescendentNodesAndSelf().OfType<ClassDeclarationSyntax>().ToArray();
                    foreach (var item in types)
                    {
                        typeFiles.Add(item.Identifier.ValueText, path);
                    }
                    var enums = root.Members[i].DescendentNodesAndSelf().OfType<EnumDeclarationSyntax>().ToArray();
                    foreach (var item in enums)
                    {
                        typeFiles.Add(item.Identifier.ValueText, path);
                    }
                }
            }
        }

        private IEnumerable<string> FindScriptsToTranslate(string className)
        {
            visitedTypes.Clear();
            return FindScriptsToTranslateIterator(className);
        }

        private IEnumerable<string> FindScriptsToTranslateIterator(string className)
        {
            HashSet<string> scripts = new HashSet<string>();
            if (visitedTypes.Contains(className))
            {
                return scripts;
            }
            visitedTypes.Add(className);
            scripts.Add(typeFiles[className]);

            foreach (string item in FindTypesInFile(typeFiles[className]))
            {
                foreach (string file in FindScriptsToTranslateIterator(item))
                {
                    scripts.Add(file);
                }
            }
            return scripts;
        }


        private IEnumerable<string> FindTypesInFile(string file)
        {
            if (typesInFile.ContainsKey(file))
            {
                return typesInFile[file];
            }

            SyntaxTree tree = SyntaxTree.ParseCompilationUnit(scriptTexts[file]);
            HashSet<string> typesToConvert = new HashSet<string>();
            var root = (CompilationUnitSyntax)tree.Root;
            var types = root.Members[0].DescendentNodes().OfType<IdentifierNameSyntax>().ToArray();
            Assembly unityEngine = typeof(UnityEngine.Application).Assembly;
            var compilation = Compilation.CreateSubmission(Path.GetTempFileName())
                .AddReferences(new AssemblyFileReference(typeof(object).Assembly.Location))
                .AddReferences(new AssemblyFileReference(unityEngine.Location))
                .AddSyntaxTrees(tree);
            var model = compilation.GetSemanticModel(tree);
            for (int i = 0; i < types.Length; i++)
            {
                var info = model.GetSemanticInfo(types.ElementAt(i));
                if (info.ConvertedType is ErrorTypeSymbol)
                {
                    string typeName = info.ConvertedType.ToString();
                    if (typeFiles.ContainsKey(typeName))
                    {
                        typesToConvert.Add(typeName);
                    }
                }
            }
            typesInFile[file] = typesToConvert;
            return typesToConvert;
        }

        private string TranslateScript(string path)
        {
            string[] textLines = scriptTexts[path].Split('\n');
            PressPlay.FFWD.Exporter.ScriptTranslator trans = new Exporter.ScriptTranslator(textLines);
            trans.Translate();
            return trans.ToString();
        }
    }
}
