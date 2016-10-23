using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorLoadLevelWindow : EditorWindow
{
	[MenuItem ("Custom/Scene")]
	static	public	void	Open ()
	{
		EditorLoadLevelWindow	window	= GetWindow<EditorLoadLevelWindow> (false);
		window.minSize	= new Vector2 (200.0f,22.5f);
		window.maxSize	= new Vector2 (400.0f,22.5f);
	}


	private	string[]	sceneFiles;
	private	EditorBuildSettingsScene[] scenes;
	private	Object	currentSceneFile;
	private	string[]	sceneNames;
	private	string		currentScene;
	private	int	current;
	private	bool	wasInitialized = false;

	private	void	Initialize ()
	{
		#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		currentScene	= EditorApplication.currentScene;
		#else
		currentScene	= UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ().name;
		#endif

		string[]	sceneGUIDs	= AssetDatabase.FindAssets ("t:scene");
		sceneFiles	= new string[sceneGUIDs.Length];
		for (int i=0; i<sceneGUIDs.Length; i++) {
			sceneFiles[i]	= AssetDatabase.GUIDToAssetPath (sceneGUIDs[i]);
		}

		sceneNames	= new string[sceneFiles.Length];
		string	path;
		for (int i=0; i<sceneFiles.Length; i++) {
			path	= sceneFiles[i];
			sceneNames[i]	= TrimSceneNameFromPath (path);
			if (sceneNames[i] == TrimSceneNameFromPath (currentScene)) {
				currentSceneFile	= AssetDatabase.LoadAssetAtPath<Object> (currentScene);
				current		= i;
			}
		}

		wasInitialized	= true;
	}


	private	string	TrimSceneNameFromPath (string iPath)
	{
		if (iPath == null || iPath.Length <= 0 || !iPath.Contains (".")) {
			return	iPath;
		}

		int trimStart;
		int trimEnd;

		if (iPath.Contains ("/")) {
			trimStart = iPath.LastIndexOf ("/");
		} else {
			trimStart = 0;
		}

		trimEnd = iPath.LastIndexOf (".")-1 - trimStart;

		return	iPath.Substring (trimStart+1, trimEnd);
	}


	private	void	OnGUI ()
	{
		if (sceneNames == null) {
			Initialize ();
			return;
		}
		EditorGUI.BeginDisabledGroup (EditorApplication.isPlayingOrWillChangePlaymode);
		EditorGUILayout.BeginHorizontal ();

		int beforeIndex = current;
		current	= EditorGUILayout.Popup (current, sceneNames);
		if (beforeIndex != current) {
			#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			if (EditorApplication.SaveCurrentSceneIfUserWantsTo ()) {
			#else
			if (UnityEditor.SceneManagement.EditorSceneManager.SaveModifiedScenesIfUserWantsTo (
				new UnityEngine.SceneManagement.Scene[] {UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ()})) {
			#endif
				currentSceneFile	= AssetDatabase.LoadAssetAtPath<Object> (sceneFiles[current]);
				#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				EditorApplication.OpenScene (sceneFiles[current]);
				#else
				UnityEditor.SceneManagement.EditorSceneManager.OpenScene (sceneFiles[current]);
				#endif
			} else {
				current	= beforeIndex;
			}
		}

		if (GUILayout.Button ("Select")) {
			Selection.activeObject	= currentSceneFile;
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUI.EndDisabledGroup ();
	}

	private	void	Update ()
	{
		if (!wasInitialized) {
			Initialize ();
			return;
		}

		minSize	= new Vector2 (200.0f,22.5f);
		maxSize	= new Vector2 (400.0f,22.5f);

		#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		if (currentScene != TrimSceneNameFromPath (EditorApplication.currentScene)) {
		#else
		if (currentScene != UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ().name) {
		#endif

			#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			currentScene		= TrimSceneNameFromPath (EditorApplication.currentScene);
			currentSceneFile	= AssetDatabase.LoadAssetAtPath<Object> (EditorApplication.currentScene);
			#else
			currentScene		= TrimSceneNameFromPath (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ().name);
			currentSceneFile	= AssetDatabase.LoadAssetAtPath<Object> (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ().path);
			#endif
			for (int i=0; i<sceneNames.Length; i++) {
				if (currentScene == sceneNames[i]) {
					current = i;
					break;
				}
			}
			Repaint ();
		}
	}

	private	void	OnEnable ()
	{
		Initialize ();
	}
}
