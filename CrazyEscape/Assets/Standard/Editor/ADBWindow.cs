using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class ADBWindow : EditorWindow
{
	private	enum LogType
	{
		Standard,
		Error
	}

	const string	KEY_ADB_PATH = "KEY_ADB_PATH";
	const string	KEY_ADB_ARGS = "KEY_ADB_ARGS";
	const string	KEY_ADB_LOCK_THE_END = "KEY_ADB_LOCK_THE_END";
	const string	KEY_LOG_FILTER = "KEY_LOG_FILTER";

	static	public	string	sAdb;
	static	public	string	sArgs;
	static	public	bool	sLockTheEnd;
	static	public	string	sLogFilter;

	[MenuItem ("Custom/ADB")]
	static	public	void	OpenWindow ()
	{
		GetWindow<ADBWindow> (false);
	}


	private	Dictionary<int,KeyValuePair<LogType,string>>	logDict;
	private	List<KeyValuePair<LogType,string>>	addLogList;
	private	Vector2	scrollArea;
	private	bool	callRepaint;

	private	LogType[]	logTypeDraws;
	private	string[]	logDraws;

	private	GUIStyle	standardStyle;
	private	GUIStyle	errorStyle;

	public	void Init ()
	{
		sAdb	= EditorPrefs.GetString (KEY_ADB_PATH, EditorPrefs.GetString ("AndroidSdkRoot"));
		sArgs	= EditorPrefs.GetString (KEY_ADB_ARGS, "logcat");
		sLockTheEnd	= EditorPrefs.GetBool (KEY_ADB_LOCK_THE_END, true);
		sLogFilter	= EditorPrefs.GetString (KEY_LOG_FILTER, "");

		logDict	= new Dictionary<int, KeyValuePair<LogType, string>> ();
		addLogList	= new List<KeyValuePair<LogType, string>> ();
		callRepaint	= true;

	}

	private	void	setGUIStyles ()
	{
		GUIStyle baseStyle = EditorStyles.numberField;
		standardStyle = new GUIStyle();
		standardStyle.border	= baseStyle.border;
		standardStyle.contentOffset	= baseStyle.contentOffset;            
		standardStyle.padding	= baseStyle.padding;
		standardStyle.normal.textColor	= Color.blue;

		errorStyle = new GUIStyle();
		errorStyle.border	= baseStyle.border;
		errorStyle.contentOffset	= baseStyle.contentOffset;            
		errorStyle.padding	= baseStyle.padding;
		errorStyle.normal.textColor	= Color.red;
	}

	private	void	OnInspectorUpdate ()
	{
		if (callRepaint) {
			for (int i=0; i<addLogList.Count; i++) {
				logDict.Add (logDict.Count, addLogList[i]);
			}
			addLogList.Clear ();

			logTypeDraws	= new LogType[logDict.Count];
			logDraws		= new string[logDict.Count];
			for (int i=0; i<logDict.Count; i++) {
				logTypeDraws[i]	= logDict[i].Key;
				logDraws[i]		= logDict[i].Value;
			}

			Repaint ();
			callRepaint	= false;
		}
	}

	private	void	OnGUI ()
	{
		for (int i=0; i<addLogList.Count; i++) {
			logDict.Add (logDict.Count, addLogList[i]);
		}

		addLogList.Clear ();

		sAdb		= EditorGUILayout.TextField ("ADB-Path", sAdb);
		sArgs		= EditorGUILayout.TextField ("ADB-Args", sArgs);
		sLogFilter	= EditorGUILayout.TextField ("Filter", sLogFilter);

		if (process == null || process.HasExited) {
			if (GUILayout.Button ("Start Process")) {
				setGUIStyles ();
				StartProcess ();
			}
		} else {
			if (GUILayout.Button ("Stop")) {
				if (!process.HasExited) {
					process.Kill ();
				} else {
					process.Close ();
				}
				process	= null;
			}
		}


		if (logDict.Count > 0) {
			if (sLockTheEnd) {
				scrollArea.y	= float.MaxValue;
			}

			scrollArea	= EditorGUILayout.BeginScrollView (scrollArea, true, true);
			for (int i=0; i<logDraws.Length; i++) {
				if (logDraws[i].Contains (sLogFilter)) {
					if (logTypeDraws[i] == LogType.Standard) {
						GUILayout.TextField (logDraws[i], standardStyle);
					} else {
						GUILayout.TextField (logDraws[i], errorStyle);
					}
				}
			}
			EditorGUILayout.EndScrollView ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Clear")) {
				logDict.Clear ();
			}
			sLockTheEnd	= GUILayout.Toggle (sLockTheEnd, "\u21b4", "button");
			EditorGUILayout.EndHorizontal ();
		}
	}

	private	void	OnEnable ()
	{
		if (process != null) {
			process.Kill ();
		}
		Init ();
	}

	private	void	OnDisable ()
	{
		if (process != null && !process.HasExited) {
			process.Kill ();
		}

		EditorPrefs.SetString (KEY_ADB_PATH, sAdb);
		EditorPrefs.SetString (KEY_ADB_ARGS, sArgs);
		EditorPrefs.SetBool (KEY_ADB_LOCK_THE_END, sLockTheEnd);
		EditorPrefs.SetString (KEY_LOG_FILTER, sLogFilter);
	}

	private	void	OnProjectChange ()
	{
		if (process != null) {
			process.Kill ();
		}

		Init ();
	}

	#region Process
	static	private	Process process;
	private	void StartProcess ()
	{
		try {
			if (process != null && !process.HasExited) {
				process.WaitForExit ();
			}

			process	= new Process ();
			process.StartInfo.FileName = sAdb;
			process.StartInfo.Arguments = sArgs;

			process.StartInfo.UseShellExecute	= false;
			process.StartInfo.CreateNoWindow	= false;

			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.EnableRaisingEvents = true;

			process.OutputDataReceived	+= new DataReceivedEventHandler (OutputHandler);
			process.ErrorDataReceived	+= new DataReceivedEventHandler (ErrorOutputHanlder);
			process.Exited	+= new System.EventHandler (Process_Exit);

			if (process.Start ()) {
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
			}
		} catch (System.Exception e) {
			UnityEngine.Debug.LogError (e);
		}
	}
	
	private void OutputHandler(object sender, System.Diagnostics.DataReceivedEventArgs args)
	{
		if (!string.IsNullOrEmpty(args.Data))
		{
			addLogList.Add (new KeyValuePair<LogType, string> (LogType.Standard, args.Data));
			callRepaint	= true;
		}
	}

	private void ErrorOutputHanlder(object sender, System.Diagnostics.DataReceivedEventArgs args)
	{
		if (!string.IsNullOrEmpty(args.Data))
		{
			addLogList.Add (new KeyValuePair<LogType, string> (LogType.Error, args.Data));
			callRepaint	= true;
		}
	}

	private void Process_Exit(object sender, System.EventArgs e)
	{
		System.Diagnostics.Process proc = (System.Diagnostics.Process)sender;

		proc.Kill();
	}
	#endregion
}
