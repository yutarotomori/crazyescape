using UnityEngine;
using UnityEditor;
using System.IO;

public class CopyAssetPath : Editor
{
    [MenuItem("Assets/CopyAssetPath %c")]
    static public void Copy()
    {
        string pathes = "";
        string newLine = "";
        foreach (var selection in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(selection).Remove (0, 7);
            if (string.IsNullOrEmpty (path))
            {
                continue;
            }

            var file = new FileInfo(path);
            if (!string.IsNullOrEmpty (file.Extension))
            {
                path = path.Replace(file.Extension, "");
            }
            pathes += path + newLine;
            newLine = "\n";
        }
        GUIUtility.systemCopyBuffer = pathes;
    }

	[MenuItem("Assets/CopyAssetPath %c", true, 45)]
    static public bool CopyCheck()
    {
        return (Selection.objects.Length > 0);
    }
}
