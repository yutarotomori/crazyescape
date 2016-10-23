#if !UNITY_EDITOR
//#define DEBUG_LOG_OVERWRAP
#define OUTPUT_LOG
#endif

#if DEBUG_LOG_OVERWRAP
using UnityEngine;
public static class Debug
{
	static Debug () {
		UnityEngine.Debug.Log ("construct debug overwrap");
	}

	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void Break() {
		if(IsEnable()) {
			UnityEngine.Debug.Break();
		}
	}
	
	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void Log (object message) {
		if(IsEnable()){
			UnityEngine.Debug.Log (message);
		}
	}

	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void Log(object message, Object context) {
		if(IsEnable()) {
			UnityEngine.Debug.Log(message, context);
		}
	}
	
	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void LogError (object message) {
		if (IsEnable ()) {
			UnityEngine.Debug.LogError (message);
		}
	}
	
	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void LogError (object message, Object context) {
		if (IsEnable ()) {
			UnityEngine.Debug.LogError (message, context);
		}
	}
	
	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void LogWarning (object message) {
		if (IsEnable ()) {
			UnityEngine.Debug.LogWarning (message);
		}
	}
	
	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void LogWarning (object message, Object context) {
		if (IsEnable ()) {
			UnityEngine.Debug.LogError (message, context);
		}
	}

	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void DrawLine (Vector3 start, Vector3 end) {
		if (IsEnable()) {
			UnityEngine.Debug.DrawLine (start, end);
		}
	}

	[System.Diagnostics.Conditional ("OUTPUT_LOG")]
	static public void DrawLine (Vector3 start, Vector3 end, Color color) {
		if (IsEnable ()) {
			UnityEngine.Debug.DrawLine (start, end, color);
		}
	}
	
	static bool IsEnable() {
//		return UnityEngine.Debug.isDebugBuild;
		return	true;
	}
}
#endif