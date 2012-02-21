using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Windows.Forms;

namespace PressPlay.FFWD.ScriptTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            p.UnityScriptsBasePath = Path.Combine(appPath, ConfigurationManager.AppSettings["UnityScriptsBasePath"]);
            p.XNAScriptsBasePath = Path.Combine(appPath, ConfigurationManager.AppSettings["XNAScriptsBasePath"]);

            List<string> excludedScripts = new List<string>();
            string exclude = ConfigurationManager.AppSettings["ExcludeScripts"];
            if (!String.IsNullOrEmpty(exclude))
            {
                excludedScripts.AddRange(exclude.Split(','));
            }
            List<string> excludedPaths = new List<string>();
            exclude = ConfigurationManager.AppSettings["ExcludePaths"];
            if (!String.IsNullOrEmpty(exclude))
            {
                excludedPaths.AddRange(exclude.Split(','));
            }

            bool exportAllScripts = false;
            bool noDependencies = false;
            bool purgeScripts = true;
            List<string> scriptsToExport = new List<string>();

            foreach (var item in args)
            {
                if (item.StartsWith("-"))
                {
                    if (item == "-all")
                    {
                        exportAllScripts = true;
                    }
                    if (item == "-d")
                    {
                        noDependencies = true;
                        Console.WriteLine("Do not convert dependencies");
                    }
                    if (item == "-p")
                    {
                        purgeScripts = false;
                        Console.WriteLine("Do not purge XNA scripts");
                    }
                }
                else
                {
                    scriptsToExport.Add(item);
                }
            }

            p.FindAllUnityScripts();
            Console.WriteLine("Found " + p.unityScriptFiles.Count + " Unity scripts.");
            p.FindAllXnaScripts();
            Console.WriteLine("Found " + p.xnaScriptFiles.Count + " XNA scripts.");
            p.FindAllClasses();

            int scripts = 0;

            if (exportAllScripts)
            {
                foreach (var className in p.unityScriptFiles.Keys)
                {
                    if (excludedScripts.Contains(className))
                    {
                        continue;
                    }
                    string scriptFile = p.unityScriptFiles[className];
                    if (excludedPaths.Any(s => scriptFile.Contains("\\" + s + "\\")))
                    {
                        continue;
                    }
                    p.TranslateScriptFile(scriptFile);
                    scripts++;
                }
            }
            else
            {
                foreach (var item in scriptsToExport)
                {
                    if (noDependencies)
                    {
                        string scriptFile = p.unityScriptFiles[item];
                        p.TranslateScriptFile(scriptFile);
                        scripts++;
                    }
                    else
                    {
                        foreach (var scriptFile in p.FindDependentScripts(item))
                        {
                            p.TranslateScriptFile(scriptFile);
                            scripts++;
                        }
                    }
                }
            }
            Console.WriteLine("Conversion done. Converted " + scripts + " scripts.");

            scripts = 0;
            if (purgeScripts)
            {
                foreach (var className in p.xnaScriptFiles.Keys)
                {
                    if (excludedScripts.Contains(className))
                    {
                        continue;
                    }
                    if (!p.unityScriptFiles.ContainsKey(className))
                    {
                        File.Delete(p.xnaScriptFiles[className]);
                    }
                    scripts++;
                }
            }
        }

        private void TranslateScriptFile(string scriptFile)
        {
            Console.WriteLine("Converting script " + Path.GetFileName(scriptFile));
            string newText = TranslateScript(scriptFile);
            string newPath = scriptFile.Replace(UnityScriptsBasePath, XNAScriptsBasePath);
            if (!Directory.Exists(Path.GetDirectoryName(newPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            }
            File.WriteAllText(newPath, newText);
        }

        private void FindAllUnityScripts()
        {
            foreach (string file in Directory.GetFiles(UnityScriptsBasePath, "*.cs", SearchOption.AllDirectories))
            {
                unityScriptFiles[Path.GetFileNameWithoutExtension(file)] = file;
            }
        }

        private void FindAllXnaScripts()
        {
            foreach (string file in Directory.GetFiles(XNAScriptsBasePath, "*.cs", SearchOption.AllDirectories))
            {
                xnaScriptFiles[Path.GetFileNameWithoutExtension(file)] = file;
            }
        }

        public string UnityScriptsBasePath;
        public string XNAScriptsBasePath;

        private Dictionary<string, string> unityScriptFiles = new Dictionary<string,string>();
        private Dictionary<string, string> xnaScriptFiles = new Dictionary<string, string>();
        private Dictionary<string, string> scriptTexts = new Dictionary<string, string>();
        private Dictionary<string, string> typeFiles = new Dictionary<string, string>();
        private Dictionary<string, HashSet<string>> typesInFile = new Dictionary<string, HashSet<string>>();

        private HashSet<string> visitedTypes = new HashSet<string>();

        private void FindAllClasses()
        {
            foreach (var path in unityScriptFiles.Values)
            {
                string file = File.ReadAllText(path);
                scriptTexts.Add(path, file);
                SyntaxTree tree = SyntaxTree.ParseCompilationUnit(file);
                var root = (CompilationUnitSyntax)tree.Root;

                for (int i = 0; i < root.Members.Count; i++)
                {
                    var classes = root.Members[i].DescendentNodesAndSelf().OfType<ClassDeclarationSyntax>();
                    foreach (var item in classes)
                    {
                        typeFiles.Add(item.Identifier.ValueText, path);
                    }
                    var interfaces = root.Members[i].DescendentNodesAndSelf().OfType<InterfaceDeclarationSyntax>();
                    foreach (var item in interfaces)
                    {
                        typeFiles.Add(item.Identifier.ValueText, path);
                    }
                    var enums = root.Members[i].DescendentNodesAndSelf().OfType<EnumDeclarationSyntax>();
                    foreach (var item in enums)
                    {
                        typeFiles.Add(item.Identifier.ValueText, path);
                    }
                }
            }
        }

        private IEnumerable<string> FindDependentScripts(string className)
        {
            visitedTypes.Clear();
            return FindDependentScriptsIterator(className);
        }

        private IEnumerable<string> FindDependentScriptsIterator(string className)
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
                foreach (string file in FindDependentScriptsIterator(item))
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
                    else if (typeName == "<Error>" && typeFiles.ContainsKey(types.ElementAt(i).PlainName))
                    {
                        typesToConvert.Add(types.ElementAt(i).PlainName);
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
