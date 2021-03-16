using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class T_BatchFile {
  
    public T_BatchFile (string inFileName, string inPath) {
        fileName = inFileName;
        path = inPath;
    }
    public T_BatchFile() { }

#region [vars]
    
    public string fileName;
    public string path;

#endregion

}
