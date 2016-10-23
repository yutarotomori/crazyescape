using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateLegacyAnimation
{
	[MenuItem ("Assets/Create/Animation (Legacy)")]
	static void Criate () {
		string dir = "Assets/";
		Object selected = Selection.activeObject;
		if (selected != null) {
			string assetDir = AssetDatabase.GetAssetPath(selected.GetInstanceID());
			if (assetDir.Length > 0 && Directory.Exists(assetDir))
				dir = assetDir + "/";
		}
		AnimationClip	clip	= new AnimationClip ();
		clip.frameRate	= 30.0f;
		clip.legacy		= true;
		string	name	= "New Animation (Legacy)";
		string	extension	= ".anim";
		if (AssetDatabase.LoadAssetAtPath<AnimationClip> (string.Format (dir+name+extension)) != null) {
			int prefix = 1;
			while (AssetDatabase.LoadAssetAtPath<AnimationClip> (string.Format (dir+name+" {0:0}" + extension, prefix)) != null) {
				prefix++;
			}
			name	=  string.Format (name+" {0:0}", prefix);
		}
		AssetDatabase.CreateAsset(clip, dir + name + extension);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = clip;
	}
}
