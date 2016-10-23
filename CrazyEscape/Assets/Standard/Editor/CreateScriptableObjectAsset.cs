using UnityEngine;
using UnityEditor;

public class CreateScriptableObjectAsset : Editor
{
	[MenuItem ("Assets/Create/ScriptableObjectAsset")]
	static	void	Create ()
	{
		Object	selection	= Selection.activeObject;
		MonoScript	mono	= (MonoScript)selection;

		string	outputPath	= AssetDatabase.GetAssetPath (Selection.activeObject).Replace (".cs", ".asset");

		ScriptableObject	asset	= ScriptableObject.CreateInstance (mono.GetClass ());
		if (AssetDatabase.LoadAssetAtPath<Object> (outputPath) == null) {
			AssetDatabase.CreateAsset (asset, outputPath);
			Selection.activeObject	= AssetDatabase.LoadAssetAtPath<Object> (outputPath);
		} else {
			Debug.LogError ("Couldn't create.");
		}
	}

	[MenuItem ("Assets/Create/ScriptableObjectAsset", true)]
	static	bool	CreateCheck ()
	{
		return	IsScriptableObject (Selection.activeObject);
	}

	static	bool	IsScriptableObject (Object activeObject)
	{
		return	(activeObject != null && activeObject is MonoScript
			&&	((MonoScript)activeObject).GetClass ().IsSubclassOf (typeof (ScriptableObject)));
	}
}
