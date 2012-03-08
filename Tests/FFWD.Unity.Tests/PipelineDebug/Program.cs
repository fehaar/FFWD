#region Copyright and License
/* Copyright (c) 2010 Stephen Styrchak
 * 
 * Microsoft Public License (Ms-PL)
 * 
 * This license governs use of the accompanying software. If you use the
 * software, you accept this license. If you do not accept the license, do not
 * use the software.
 * 
 * 1. Definitions
 * 
 * The terms "reproduce," "reproduction," "derivative works," and "distribution"
 * have the same meaning here as under U.S. copyright law.
 * 
 * A "contribution" is the original software, or any additions or changes to the
 * software.
 * 
 * A "contributor" is any person that distributes its contribution under this license.
 * 
 * "Licensed patents" are a contributor's patent claims that read directly on its
 * contribution.
 * 
 * 2. Grant of Rights
 * 
 * (A) Copyright Grant- Subject to the terms of this license, including the license
 *     conditions and limitations in section 3, each contributor grants you a non-
 *     exclusive, worldwide, royalty-free copyright license to reproduce its contribution,
 *     prepare derivative works of its contribution, and distribute its contribution or
 *     any derivative works that you create.
 * 
 * (B) Patent Grant- Subject to the terms of this license, including the license
 *     conditions and limitations in section 3, each contributor grants you a non-exclusive,
 *     worldwide, royalty-free license under its licensed patents to make, have made, use,
 *     sell, offer for sale, import, and/or otherwise dispose of its contribution in the
 *     software or derivative works of the contribution in the software.
 * 
 * 3. Conditions and Limitations
 * 
 * (A) No Trademark License- This license does not grant you rights to use any contributors'
 *     name, logo, or trademarks.
 * 
 * (B) If you bring a patent claim against any contributor over patents that you claim are
 *     infringed by the software, your patent license from such contributor to the software
 *     ends automatically.
 * 
 * (C) If you distribute any portion of the software, you must retain all copyright, patent,
 *     trademark, and attribution notices that are present in the software.
 * 
 * (D) If you distribute any portion of the software in source code form, you may do so only
 *     under this license by including a complete copy of this license with your distribution.
 *     If you distribute any portion of the software in compiled or object code form, you may
 *     only do so under a license that complies with this license.
 * 
 * (E) The software is licensed "as-is." You bear the risk of using it. The contributors give
 *     no express warranties, guarantees or conditions. You may have additional consumer
 *     rights under your local laws which this license cannot change. To the extent permitted
 *     under your local laws, the contributors exclude the implied warranties of
 *     merchantability, fitness for a particular purpose and non-infringement. 
 */
#endregion
#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PipelineDebug
{
    /// <summary>
    /// To debug the XNA Framework Content Pipeline:
    ///     1. Modify the constants below to match your project
    ///     2. Set this project to be the Startup Project
    ///     3. Start debugging
    /// </summary>
    class Program
    {
        /// <summary>
        /// TODO: Change this to the full path of the content project whose pipeline you want to debug.
        ///       Example:
        ///               private const string ProjectToDebug = @"C:\XNA Samples\SkinnedModelExtensions\SkinningSample\Content\SkinningSampleContent.contentproj";
        /// </summary>
        private const string ProjectToDebug = @"..\..\..\FFWD.Unity.TestsContent\FFWD.Unity.TestsContent.contentproj";

        /// <summary>
        /// TODO: Change this to the content item you want to debug. The content pipeline will only
        ///       build this one item and no others. Leave SingleItem null or empty to build the entire
        ///       content project while debugging.
        ///       Example:
        ///               private const string SingleItem = @"dude.fbx";
        /// </summary>
        private const string SingleItem = @"Scenes\Lightmap.xml";

        /// <summary>
        /// TODO: Set the XnaProfile to HiDef or Reach, depending on your target graphics profile.
        /// NOTE: Windows Phone projects only support content built for the Reach profile.
        /// </summary>
        private const GraphicsProfile XnaProfile = GraphicsProfile.HiDef;

        /// <summary>
        /// TODO: You generally don't need to change this unless your custom importer or processor uses
        ///       the TargetPlatform property of its context object.
        /// </summary>
        private const TargetPlatform XnaPlatform = TargetPlatform.Windows;

        /// <summary>
        /// TODO: Change this if you want to see more output from MSBuild.
        /// NOTE: Detailed and Diagnostic output makes builds noticeably slower.
        /// </summary>
        private const LoggerVerbosity LoggingVerbosity = LoggerVerbosity.Normal;

        #region MSBuild hosting and execution

        /// <summary>
        /// This program hosts the MSBuild engine and builds the content project with parameters based
        /// on the constant values specified above.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!File.Exists(ProjectToDebug))
            {
                throw new FileNotFoundException(String.Format("The project file '{0}' does not exist. Set the ProjectToDebug field to the full path of the project you want to debug.", ProjectToDebug), ProjectToDebug);
            }
            if (!String.IsNullOrEmpty(SingleItem) && !File.Exists(Path.Combine(WorkingDirectory, SingleItem)))
            {
                throw new FileNotFoundException(String.Format("The project item '{0}' does not exist. Set the SingleItem field to the relative path of the content item you want to debug, or leave it empty to debug the whole project.", SingleItem), SingleItem);
            }
            Environment.CurrentDirectory = WorkingDirectory;

            var globalProperties = new Dictionary<string, string>();

            globalProperties.Add("Configuration", Configuration);
            globalProperties.Add("XnaProfile", XnaProfile.ToString());
            globalProperties.Add("XNAContentPipelineTargetPlatform", XnaContentPipelineTargetPlatform);
            globalProperties.Add("SingleItem", SingleItem);
            globalProperties.Add("CustomAfterMicrosoftCommonTargets", DebuggingTargets);

            var project = ProjectCollection.GlobalProjectCollection.LoadProject(ProjectName, globalProperties, MSBuildVersion);
            bool succeeded = project.Build("rebuild", Loggers);

            // To read the build output in the console window, place a breakpoint on the
            // Debug.WriteLine statement below.
            Debug.WriteLine("Build " + (succeeded ? "Succeeded." : "Failed."));
        }

        #region Additional, rarely-changing property values

        private const string Configuration = "Debug";
        private const string MSBuildVersion = "4.0";

        private static IEnumerable<ILogger> Loggers
        {
            get
            {
                return new ILogger[] { new ConsoleLogger(LoggingVerbosity) };
            }
        }

        private static string WorkingDirectory
        {
            get { return Path.GetDirectoryName(Path.GetFullPath(ProjectToDebug)); }
        }

        private static string BuildToolDirectory
        {
            get
            {
                string startupExe = System.Reflection.Assembly.GetEntryAssembly().Location;
                return Path.GetDirectoryName(startupExe);
            }
        }

        private static string ProjectName
        {
            get { return Path.GetFileName(Path.GetFullPath(ProjectToDebug)); }
        }

        private static string XnaContentPipelineTargetPlatform
        {
            get
            {
                return XnaPlatform.ToString();
            }
        }

        public static string DebuggingTargets
        {
            get
            {
                if (String.IsNullOrEmpty(SingleItem))
                {
                    return String.Empty;
                }

                string targetsPath = @"Targets\Debugging.targets";
                return Path.Combine(BuildToolDirectory, targetsPath);
            }
        }

        #endregion

        #endregion
    }
}
