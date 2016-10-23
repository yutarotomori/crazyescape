using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AutoRenamer : EditorWindow
{
	enum RenameMode
	{
		File,
		Directry,
	}

	static	Dictionary<Object,RenameMode>	objectDict	= new Dictionary<Object, RenameMode>();

	static	string	fromName	= "";
	static	string	toName		= "";
	static	bool	useSerial	= false;
	static	int		startSerialNo	= 1;
	static	int		increment		= 1;
	static	string	prefix	= "";
	static	string	retrofit = "";
	static	string	format		= "0";

	static	RenameMode	renameMode = RenameMode.File;

	[MenuItem("Custom/AutoRenamer")]
	static void Init ()
	{
		GetWindow <AutoRenamer>().minSize	= Vector2.one*275.0f;
	}

	private	void	Clear ()
	{
		objectDict.Clear ();
	}

	private	void	Rename ()
	{
		int no	= startSerialNo;

		foreach (Object key in objectDict.Keys) {
			if (objectDict[key] != renameMode) {
				continue;
			}
			string path		= AssetDatabase.GetAssetPath (key);
			if (useSerial == true) {
				string	newName	= string.Format (toName+prefix+"{0:"+format+"}"+retrofit, no);
				Debug.Log ("\"" + path + "\" to \"" + newName + "\"");
				AssetDatabase.RenameAsset (path, newName);
				no	+= increment;
			} else if (fromName.Length > 0) {
				if (key.name.Contains (fromName) == true) {
					Debug.Log ("\"" + path + "\" to \"" + key.name.Replace (fromName, toName) + "\"");
					AssetDatabase.RenameAsset (path, key.name.Replace (fromName, toName));
				}
			} else if (toName.Length > 0) {
				Debug.Log ("\"" + path + "\" to \"" + toName + "\"");
				AssetDatabase.RenameAsset (path, toName);
			}
		}

		AssetDatabase.Refresh ();
		Debug.Log ("Complete!");
	}

	private	void	AddItem (Object asset, RenameMode objectType)
	{
		string filePath	= AssetDatabase.GetAssetPath (asset);
		
		if (Directory.Exists (filePath)) {
			string[] directories	= Directory.GetDirectories (filePath);
			foreach (string nextPath in directories) {
				AddItem (AssetDatabase.LoadAssetAtPath (nextPath, typeof (Object)), RenameMode.Directry);
			}
			if (objectDict.ContainsKey (asset) == false) {
				objectDict.Add (asset, objectType);
			}
		}
		
		string[] filePathes = Directory.GetFiles (filePath);

		for (int i=filePathes.Length-1; i>=0; i--) {
			Object obj	= AssetDatabase.LoadAssetAtPath (filePathes[i], typeof (Object));
			if (obj == null || objectDict.ContainsKey (obj)) {
				continue;
			}
			
			objectDict.Add (obj, RenameMode.File);
		}
	}

	private	void	SortByFileName ()
	{
		List<KeyValuePair<Object,RenameMode>>	objectList;
		objectList	= new List<KeyValuePair<Object, RenameMode>> (objectDict);

		objectList.Sort ( (KeyValuePair<Object,RenameMode> a, KeyValuePair<Object,RenameMode> b)
			=> string.Compare (a.Key.name, b.Key.name));

		objectDict.Clear ();
		foreach (KeyValuePair<Object, RenameMode> pair in objectList) {
			objectDict.Add (pair.Key, pair.Value);
		}
	}

	Vector2	scroll;
	private	void	OnGUI ()
	{
		useSerial	= EditorGUILayout.Toggle ("Use Serial", useSerial);
		if (useSerial == false) {
			fromName	= EditorGUILayout.TextField ("From", fromName);
			toName		= EditorGUILayout.TextField ("To", toName);
		} else {
			prefix		= EditorGUILayout.TextField (" + Delimiter", prefix);
			retrofit	= EditorGUILayout.TextField (" + Retrofit", retrofit);
			startSerialNo	= EditorGUILayout.IntField (" + Start No", startSerialNo);
			increment		= EditorGUILayout.IntField (" + Increment(s)", increment);
			format			= EditorGUILayout.TextField (" + Format", format);
		}

		renameMode	= (RenameMode)EditorGUILayout.EnumPopup ("Rename Mode", renameMode);

		Event dropEvent	= Event.current;
		Rect dropArea	= GUILayoutUtility.GetRect (0.0f, 70.0f, GUILayout.ExpandWidth(true));

		GUI.Box(dropArea,"Drag & Drop (File or Directry)");
		int id = GUIUtility.GetControlID(FocusType.Passive);
		if (dropEvent.type == EventType.DragExited) {
			if(dropArea.Contains(dropEvent.mousePosition)) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				DragAndDrop.activeControlID = id;
				
				DragAndDrop.AcceptDrag();
				
				foreach(Object draggedObject in DragAndDrop.objectReferences) {
					RenameMode objectType	= RenameMode.File;
					if (Directory.Exists (AssetDatabase.GetAssetPath(draggedObject))) {
						objectType	= RenameMode.Directry;
					}
					AddItem (draggedObject, objectType);
				}
				DragAndDrop.activeControlID = 0;
				Event.current.Use();
			}
		}
		
		if (objectDict.Count > 0) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Rename") == true) {
				SortByFileName ();
				Rename ();
			}
			if (GUILayout.Button ("Clear") == true) {
				Clear ();
			}
			GUILayout.EndHorizontal ();
		}

		scroll	= EditorGUILayout.BeginScrollView (scroll);
		foreach (Object obj in objectDict.Keys) {
			if (obj == null) {
				continue;
			}
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Remove", GUILayout.Width (65)) == true) {
				objectDict.Remove (obj);
				break;
			} else {
				Color fontColor	= Color.blue;
				string filePath	= AssetDatabase.GetAssetPath (obj);
				string fileName	= obj.name;
				if (fileName.Contains (fromName) == false || objectDict[obj] != renameMode) {
					fontColor	= Color.red;
				}
				GUI.contentColor	= fontColor;
				GUILayout.Label (filePath.Replace ("Assets/", ""));
			}
			GUI.contentColor	= Color.white;
			EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.EndScrollView ();
	}
}
