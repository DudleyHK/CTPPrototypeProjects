using System.IO;
using System.Collections;
using System.Collections.Generic;




public class Utilities
{
    public static FileInfo[] FilesInDirectory(string _path)
    { 
        var path     = new DirectoryInfo(_path);
        var fileInfo = path.GetFiles("*.*", SearchOption.AllDirectories);
        
        return fileInfo;
    }
}
