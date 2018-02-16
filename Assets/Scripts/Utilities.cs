﻿using System.IO;
using System.Collections;
using System.Collections.Generic;




public class Utilities : UnityEditor.ScriptableWizard
{
    public static FileInfo[] FilesInDirectory(string _path)
    { 
        var path     = new DirectoryInfo(_path);
        var fileInfo = path.GetFiles("*.*", SearchOption.AllDirectories);
        
        return fileInfo;
    }

    [UnityEditor.MenuItem("Tools/Clear Console %c")]
    static void ClearConsoleWizard()
    {
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear",
            System.Reflection.BindingFlags.Static
            | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);

    }
}
