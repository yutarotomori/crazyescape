using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ComponentViewOnHierarchy
{
	
	private const int WIDTH     = 16;
	private const int HEIGHT    = 16;
	
	#if EXAMPLE_CLASS_ACTIVITY
	private static readonly Color mDisabledColor = new Color( 1, 1, 1, 0.5f );
	static	ComponentViewOnHierarchy ()
	{
		EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
	}
	#endif
	[PreferenceItem("Custom")]
	static void PreferencesGUI ()
	{
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Component Icon");
		if (GUILayout.Button("Enable", GUILayout.Width(64))) {
			EnableExampleClass();
		}
		if (GUILayout.Button("Disable", GUILayout.Width(64))) {
			DisableExampleClass();
		}
		GUILayout.EndHorizontal ();
	}

	const string EXAMPLE_CLASS_ACTIVITY = "EXAMPLE_CLASS_ACTIVITY";
	
	static void EnableExampleClass () {
		bool added = false;
		foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup))) {
			string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
			if (!defines.Contains(EXAMPLE_CLASS_ACTIVITY)) {
				added = true;
				if (defines.EndsWith(";"))
					defines = defines + EXAMPLE_CLASS_ACTIVITY;
				else
					defines = defines + ";" + EXAMPLE_CLASS_ACTIVITY;
				
				PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
			}
		}
		
		if (added) {
			Debug.LogWarning("Setting Scripting Define Symbol " + EXAMPLE_CLASS_ACTIVITY);
		} else {
			Debug.LogWarning("Already Set Scripting Define Symbol " + EXAMPLE_CLASS_ACTIVITY);
		}
	}
	
	
	static void DisableExampleClass () {
		bool removed = false;
		foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup))) {
			string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
			if (defines.Contains(EXAMPLE_CLASS_ACTIVITY)) {
				removed = true;
				if (defines.Contains(EXAMPLE_CLASS_ACTIVITY + ";"))
					defines = defines.Replace(EXAMPLE_CLASS_ACTIVITY + ";", "");
				else
					defines = defines.Replace(EXAMPLE_CLASS_ACTIVITY, "");
				
				PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
			}
		}
		
		if (removed) {
			Debug.LogWarning("Removing Scripting Define Symbol " + EXAMPLE_CLASS_ACTIVITY);
		} else {
			Debug.LogWarning("Already Removed Scripting Define Symbol " + EXAMPLE_CLASS_ACTIVITY);
		}
	}
	
	
	#if EXAMPLE_CLASS_ACTIVITY
	private static void OnGUI( int instanceID, Rect selectionRect )
	{
		var go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;
		
		if ( go == null )
		{
			return;
		}
		
		var pos     = selectionRect;
		pos.x       = pos.xMax - WIDTH;
		pos.width   = WIDTH;
		pos.height  = HEIGHT;
		
		var components = go
			.GetComponents<Component>()
				.Where( c => c != null )
				.Where( c => !( c is Transform ) )
				.Reverse();
		
		//        var current = Event.current;
		
		foreach ( var c in components )
		{
			Texture image = AssetPreview.GetMiniThumbnail( c );
			
			if ( image == null && c is MonoBehaviour )
			{
				var ms      = MonoScript.FromMonoBehaviour( c as MonoBehaviour );
				var path    = AssetDatabase.GetAssetPath( ms );
				image       = AssetDatabase.GetCachedIcon( path );
			}
			
			if ( image == null )
			{
				continue;
			}
			
			//            if ( current.type == EventType.MouseDown && 
			//                 pos.Contains( current.mousePosition ) )
			//            {
			//                c.SetEnable( !c.IsEnabled() );
			//            }
			
			var color = GUI.color;
			GUI.color = c.IsEnabled() ? Color.white : mDisabledColor;
			GUI.DrawTexture( pos, image, ScaleMode.ScaleToFit );
			GUI.color = color;
			pos.x -= pos.width;
		}
	}
	
	public static bool IsEnabled( this Component self )
	{
		if ( self == null )
		{
			return true;
		}
		
		var type        = self.GetType();
		var property    = type.GetProperty( "enabled", typeof( bool ) );
		
		if ( property == null )
		{
			return true;
		}
		
		return ( bool )property.GetValue( self, null );
	}
	
	public static void SetEnable( this Component self, bool isEnabled )
	{
		if ( self == null )
		{
			return;
		}
		
		var type        = self.GetType();
		var property    = type.GetProperty( "enabled", typeof( bool ) );
		
		if ( property == null )
		{
			return;
		}
		
		property.SetValue( self, isEnabled, null );
	}
	#endif
}
