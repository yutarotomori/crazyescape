using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class ResourcePath : ScriptableObject
{
	[System.Serializable]
	public struct ResourceAsset
	{
		public GameObject _asset;
		public string _path;
	}


	static private ResourcePath instanceValue;

	static private ResourcePath instance {
		get {
			if (instanceValue == null) {
				instanceValue = Resources.Load<ResourcePath> ("Data/ResourcePath");
			}
			return instanceValue;
		}
	}


	static public string ostrich {
		get {
			return instance._ostrich._path;
		}
	}

	static public string enemy {
		get {
			return instance._enemy._path;
		}
	}

	static public string rock {
		get {
			return instance._rock._path;
		}
	}

	static public string corn {
		get {
			return instance._corn._path;
		}
	}

	static public string logs {
		get {
			return instance._logs._path;
		}
	}


	public ResourceAsset _ostrich;
	public ResourceAsset _enemy;
	public ResourceAsset _rock;
	public ResourceAsset _corn;
	public ResourceAsset _logs;
}


#if UNITY_EDITOR
[CustomEditor (typeof(ResourcePath))]
public class ResourcePathEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		var type = typeof(ResourcePath);
		var fieldInfos = type.GetFields ();

		foreach (var fieldInfo in fieldInfos) {
			if (fieldInfo.FieldType == typeof(ResourcePath.ResourceAsset)) {
				var prop = serializedObject.FindProperty (fieldInfo.Name);

				var asset = prop.FindPropertyRelative ("_asset").objectReferenceValue;
				if (asset != null) {
					var path = AssetDatabase.GetAssetPath (asset);
					path = path.Replace (Path.GetExtension (path), "");

					var trimIdx = path.IndexOf ("Resources/");
					if (trimIdx >= 0) {
						path = path.Remove (0, trimIdx + 10);
					}
					prop.FindPropertyRelative ("_path").stringValue = path;
				} else {
					prop.FindPropertyRelative ("_path").stringValue = "Null";
				}
			}
		}

		serializedObject.ApplyModifiedProperties ();
	}
}
#endif
