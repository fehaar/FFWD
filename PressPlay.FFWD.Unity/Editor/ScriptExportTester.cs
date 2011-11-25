using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptExportTester
{
    [MenuItem("CONTEXT/MonoBehaviour/FFWD Export Script - EXPERIMENTAL")]
    static void ExportScript(MenuCommand command)
    {
        ExportSceneWizard wiz = new ExportSceneWizard();
        
    }
}
