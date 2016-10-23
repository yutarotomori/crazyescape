using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleBuildEditor : EditorWindow
{
	[MenuItem ("Custom/AssetBundle/BuildEditor")]
	static	public	void	Open ()
	{
		GetWindow<AssetBundleBuildEditor> (true);
	}

	#region consts
	const	string	ROOT	= "AssetBundles";
	const	string	SLASH	= "/";
	const	string	OUTPUT_PATH	= "Output path";
	#endregion

	#region valiables (instance)
	private	List<Item>	m_AssetBundleItems;
	private	Fold		m_AssetFold;
	private	Color		m_DefaultColor;
	private	GUIStyle	m_DefaultStyle;
	private	GUIStyle	m_ToggleStyle;
	private	GUIStyle	m_ItemStyle;
	private	GUIStyle	m_PreTextStyle;
	private	int			m_ItemCount;
	private	Vector2		m_ScrollArea;
	private	BuildAssetBundleOptions	m_BuildOption;
	private	bool		m_IsOpenTargetList;
	private	List<BuildTargetWithPath>	m_BuildTargetList;
	private	bool		m_IsBuildFlag;
	#endregion

	#region save params
	private	string	m_SaveKey {
		get {
			return	EditorApplication.applicationPath+":"+"AssetBundleBuildManager_";
		}
	}
	private	string	m_SaveIsOpen				{	get	{ return	"_IsOpen"; }				}
	private	string	m_SaveIsChecked				{	get { return	"_IsChecked"; }				}
	private	string	m_SaveOutputPath 			{	get { return	"OutputPath"; }				}
	private	string	m_SaveBuildOption			{	get { return	"BuildOption"; }			}
	private	string	m_SaveBuildTarget			{	get { return	"TargetPlatform"; }			}
	private	string	m_SaveIsOpenTargetList		{	get	{ return	"IsOpenTargetList"; }		}
	private	string	m_SaveBuildTargetCount		{	get	{ return	"BuildTargetList"; }		}
	private	string	m_SaveTargetListIsValidate	{	get	{ return	"TargetListIsValidate"; }	}
	#endregion

	#region call by Initialize
	private	void	LoadSavedValues ()
	{
		m_BuildOption	= (BuildAssetBundleOptions)EditorPrefs.GetInt (m_SaveKey+m_SaveBuildOption, 0);

		m_IsOpenTargetList	= EditorPrefs.GetBool (m_SaveKey+m_SaveIsOpenTargetList, false);
		m_BuildTargetList	= new List<BuildTargetWithPath> ();
		BuildTargetWithPath	targetWithPath;
		for (int i=0; i<EditorPrefs.GetInt (m_SaveKey+m_SaveBuildTargetCount, 0); i++) {
			targetWithPath	= new BuildTargetWithPath ();
			targetWithPath.isValidate	= EditorPrefs.GetBool (m_SaveKey+m_SaveTargetListIsValidate+"_"+i, true);
			targetWithPath.buildTarget	= (BuildTarget)EditorPrefs.GetInt (m_SaveKey+m_SaveBuildTarget+"_"+i, 9);
			targetWithPath.outputPath	= EditorPrefs.GetString (m_SaveKey+m_SaveOutputPath+"_"+i, OUTPUT_PATH);
			m_BuildTargetList.Add (targetWithPath);
		}
	}

	private	void	LoadAssetBundleItems ()
	{
		string[] assetBundleNames	= AssetDatabase.GetAllAssetBundleNames ();
		string str;
		Item temp;
		Fold fold;
		Item item;

		m_AssetBundleItems	= new List<Item> ();
		m_AssetFold	= new Fold (ROOT);
		if (EditorPrefs.GetBool (m_SaveKey+ROOT+m_SaveIsOpen, false)) {
			m_AssetFold.Open ();
		}
		if (EditorPrefs.GetBool (m_SaveKey+ROOT+m_SaveIsChecked, false)) {
			m_AssetFold.CheckOn ();
		}

		for (int i=0; i<assetBundleNames.Length; i++) {
			str = assetBundleNames[i];

			if (!TryGetItem (str, out item)) {
				item	= new Item (str);
				m_AssetBundleItems.Add (item);
			}
			if (EditorPrefs.GetBool (m_SaveKey+item.name+m_SaveIsChecked, false)) {
				item.CheckOn ();
			}

			temp	= item;

			while (str.Contains (SLASH)) {
				str	= str.Substring (0, str.LastIndexOf (SLASH));
				if (!TryGetFold (str, out fold)) {
					fold	= new Fold (str);
					m_AssetBundleItems.Add (fold);
				}
				if (EditorPrefs.GetBool (m_SaveKey+str+m_SaveIsOpen, false)) {
					fold.Open ();
				}
				if (EditorPrefs.GetBool (m_SaveKey+str+m_SaveIsChecked, false)) {
					fold.CheckOn ();
				}
				if (!fold.ContainsItem (temp)) {
					if (	(temp is Fold && !fold.ContainsFold ((Fold)temp))
						||	(!(temp is Fold) && !fold.ContainsItem (temp))) {

						fold.AddItem (temp);
						temp.SetParent (fold);
					}
				}
				temp	= fold;
			}
			if (	(temp is Fold && !m_AssetFold.ContainsFold ((Fold)temp))
				||	(!(temp is Fold) && !m_AssetFold.ContainsItem (temp))) {

				m_AssetFold.AddItem (temp);
				temp.SetParent (m_AssetFold);
			}
		}
		m_AssetBundleItems.Add (m_AssetFold);
	}

	private	bool	TryGetItem (string name, out Item item)
	{
		item	= null;
		if (m_AssetBundleItems == null) {
			return	false;
		}

		for (int i=0; i<m_AssetBundleItems.Count; i++) {
			if (m_AssetBundleItems[i] is Fold) {
				continue;
			}
			if (m_AssetBundleItems[i].name == name) {
				item	= m_AssetBundleItems[i];
				return	true;
			}
		}
		return	false;
	}

	private	bool	TryGetFold (string name, out Fold fold)
	{
		fold	= null;
		if (m_AssetBundleItems == null) {
			return	false;
		}

		for (int i=0; i<m_AssetBundleItems.Count; i++) {
			if (!(m_AssetBundleItems[i] is Fold)) {
				continue;
			}
			if (m_AssetBundleItems[i].name == name) {
				fold	= (Fold)m_AssetBundleItems[i];
				return	true;
			}
		}
		return	false;
	}
	#endregion

	private void	Initialize ()
	{
		m_IsBuildFlag	= false;
		minSize	= new Vector2 (256.0f, 256.0f);
		LoadAssetBundleItems ();
		LoadSavedValues ();
	}

	#region call by OnGUI
	private	void	ShowBuildTarget ()
	{
		GUILayout.BeginVertical (GUI.skin.box);

		GUILayout.BeginHorizontal ();

		m_IsOpenTargetList	= EditorGUILayout.Foldout (m_IsOpenTargetList, "BuildTargetList");

		m_BuildOption	= (BuildAssetBundleOptions)EditorGUILayout.EnumMaskField (m_BuildOption, GUILayout.Width (150));
		GUILayout.EndHorizontal ();
		if (m_IsOpenTargetList) {
			EditorGUI.indentLevel++;
			for (int i=0; i<m_BuildTargetList.Count; i++) {
				GUILayout.BeginHorizontal ();
				{
					m_BuildTargetList[i].isValidate		= GUILayout.Toggle (m_BuildTargetList[i].isValidate, "");
					m_BuildTargetList[i].buildTarget	= (BuildTarget)EditorGUILayout.EnumPopup (m_BuildTargetList[i].buildTarget);
					if (m_BuildTargetList[i].outputPath.Length == 0 || m_BuildTargetList[i].outputPath == OUTPUT_PATH) {
						m_BuildTargetList[i].outputPath		= EditorGUILayout.TextField (OUTPUT_PATH, m_PreTextStyle);
					} else {
						m_BuildTargetList[i].outputPath		= EditorGUILayout.TextField (m_BuildTargetList[i].outputPath);
					}

					if (GUILayout.Button ("Remove")) {
						m_BuildTargetList.RemoveAt (i);
						break;
					}
				}
				GUILayout.EndHorizontal ();
			}
			EditorGUI.indentLevel--;

			if (GUILayout.Button ("Add Target")) {
				m_BuildTargetList.Add (new BuildTargetWithPath ());
			}
		}
		GUILayout.EndVertical ();
	}

	private	void	ListUpAssetBundleItems ()
	{
		if (m_AssetBundleItems == null || m_AssetBundleItems.Count == 0) {
			EditorGUILayout.LabelField ("No AssetBudles.");
			return;
		}

		m_ItemCount	= 0;
		m_ScrollArea	= EditorGUILayout.BeginScrollView (m_ScrollArea);
		ListUpFoldInItems (m_AssetFold);
		EditorGUILayout.EndScrollView ();
		GUI.backgroundColor	= m_DefaultColor;
	}

	private	void	ListUpFoldInItems (Fold fold)
	{
		bool	isOpen;
		bool	isChecked;
		string	itemName;
		string	foldName		= fold.name.Remove (0, fold.name.LastIndexOf (SLASH)+1);
		List<Item>	itemList	= new List<Item>();

		GUI.backgroundColor	= GetBackColor ();
		if (fold.name == ROOT) {
			GUILayout.BeginHorizontal (GUI.skin.box);
		} else {
			GUILayout.BeginHorizontal (m_DefaultStyle);
		}
		{
			isOpen		= EditorGUI.Foldout (EditorGUILayout.GetControlRect(), fold.isOpen, foldName);
			GUI.backgroundColor	= m_DefaultColor;
			isChecked	= GUILayout.Toggle (fold.isChecked, "", m_ToggleStyle, GUILayout.ExpandWidth (false));
		}
		GUILayout.EndHorizontal ();

		if (fold.isChecked != isChecked) {
			fold.Check (isChecked);
		}

		if (isOpen) {
			fold.Open ();
			EditorGUI.indentLevel++;
			itemList.Clear ();
			for (int i=0; i<fold.children.Count; i++) {
				if (fold.children[i] is Fold) {
					ListUpFoldInItems ((Fold)fold.children[i]);
				} else {
					itemList.Add (fold.children[i]);
				}
			}
			for (int i=0; i<itemList.Count; i++) {
				itemName = "- "+itemList[i].name.Remove (0, itemList[i].name.LastIndexOf (SLASH)+1);

				GUI.backgroundColor	= GetBackColor ();
				GUILayout.BeginHorizontal (m_DefaultStyle);
				{
					EditorGUI.LabelField (EditorGUILayout.GetControlRect(), itemName, m_ItemStyle);
					GUI.backgroundColor	= m_DefaultColor;
					isChecked	= GUILayout.Toggle (itemList[i].isChecked, "", m_ToggleStyle, GUILayout.ExpandWidth (false));
					if (itemList[i].isChecked != isChecked) {
						itemList[i].Check (isChecked);
					}
				}
				GUILayout.EndHorizontal ();
			}
			EditorGUI.indentLevel--;
		} else {
			fold.Close ();
		}
	}

	private	void	ShowBuildButton ()
	{
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button ("\u21bb", GUILayout.Width (25.0f))) {
			//		if (GUILayout.Button ("\u21bb", GUILayout.Width (55.0f))) {
			AssetDatabase.RemoveUnusedAssetBundleNames ();
			SaveValues ();
			Initialize ();
		}
		if (GUILayout.Button ("Build")) {
			m_IsBuildFlag	= true;
		} else if (GUILayout.Button ("Preview", GUILayout.Width (55.0f))) {
			BuildListPopup.ShowList (GetCheckedBundleNames (), this);
		} else if (GUILayout.Button ("Clear", GUILayout.Width (40.0f))) {
			m_AssetFold.Check (false);
		}
		EditorGUILayout.EndHorizontal ();
	}

	private	Color	GetBackColor ()
	{
		if (m_ItemCount++ % 2 == 0) {
			return	Color.white*0.64f;
		} else {
			return	Color.white*0.7f;
		}
	}

	private	void	LoadGUIStyles ()
	{
		if (m_DefaultStyle == null) {
			CopyGUIStyle (out m_DefaultStyle, GUI.skin.label);
			m_DefaultStyle.normal.background	= Texture2D.whiteTexture;
		}
		if (m_ToggleStyle == null) {
			CopyGUIStyle (out m_ToggleStyle, GUI.skin.toggle);
		}
		if (m_ItemStyle == null) {
			CopyGUIStyle (out m_ItemStyle, GUI.skin.label);
			m_ItemStyle.fontStyle	= FontStyle.Bold;
		}
		if (m_PreTextStyle == null) {
			CopyGUIStyle (out m_PreTextStyle, GUI.skin.textField);
			m_PreTextStyle.normal.textColor	= Color.gray;
			m_PreTextStyle.focused.textColor	= Color.gray;
		}
		m_DefaultColor	= GUI.backgroundColor;
	}

	private	void	CopyGUIStyle (out GUIStyle oStyle, GUIStyle iOrigin)
	{
		oStyle	= new GUIStyle ();
		oStyle.active	= iOrigin.active;
		oStyle.alignment	= iOrigin.alignment;
		oStyle.border	= iOrigin.border;
		oStyle.clipping	= iOrigin.clipping;
		oStyle.contentOffset	= iOrigin.contentOffset;
		oStyle.fixedHeight	= iOrigin.fixedHeight;
		oStyle.fixedWidth	= iOrigin.fixedWidth;
		oStyle.focused	= iOrigin.focused;
		oStyle.font	= iOrigin.font;
		oStyle.fontSize	= iOrigin.fontSize;
		oStyle.fontStyle	= iOrigin.fontStyle;
		oStyle.hover	= iOrigin.hover;
		oStyle.imagePosition	= iOrigin.imagePosition;
		oStyle.margin	= iOrigin.margin;
		oStyle.normal	= iOrigin.normal;
		oStyle.onActive	= iOrigin.onActive;
		oStyle.onFocused	= iOrigin.onFocused;
		oStyle.onHover	= iOrigin.onHover;
		oStyle.onNormal	= iOrigin.onNormal;
		oStyle.overflow	= iOrigin.overflow;
		oStyle.padding	= iOrigin.padding;
		oStyle.richText	= iOrigin.richText;
		oStyle.stretchHeight	= iOrigin.stretchHeight;
		oStyle.stretchWidth	= iOrigin.stretchWidth;
		oStyle.wordWrap	= iOrigin.wordWrap;
	}
	#endregion

	private	void	OnGUI ()
	{
		LoadGUIStyles ();

		ShowBuildTarget ();
		ListUpAssetBundleItems ();
		ShowBuildButton ();
	}

	private	void	BuildStart ()
	{
		if (m_AssetBundleItems == null || m_AssetBundleItems.Count == 0) {
			Debug.LogError ("No AssetBundles.");
			ShowNotification ("Build Failed.");
			return;
		}

		if (m_BuildTargetList == null || m_BuildTargetList.Count == 0) {
			Debug.LogError ("No BuildTargets.");
			ShowNotification ("Build Failed.");
			return;
		}

		string[]	buildNames	= GetCheckedBundleNames ();

		List<AssetBundleBuild>	buildList	= new List<AssetBundleBuild> ();
		AssetBundleBuild	build;
		for (int i=0; i<buildNames.Length; i++) {
			build	= new AssetBundleBuild ();
			build.assetNames	= AssetDatabase.GetAssetPathsFromAssetBundle (buildNames[i]);
			build.assetBundleName	= buildNames[i];
			buildList.Add (build);
		}

		if (buildList == null || buildList.Count == 0) {
			Debug.LogError ("No Checked AssetBundles.");
			ShowNotification ("Build Failed.");
			return;
		}

		for (int i=0; i<m_BuildTargetList.Count; i++) {
			if (!m_BuildTargetList[i].isValidate) {
				continue;
			}

			if (m_BuildTargetList[i].outputPath == null || m_BuildTargetList[i].outputPath.Length == 0) {
				Debug.LogError ("BuildTarget ("+(i+1)+") path is empty.");
				ShowNotification ("Build Failed.");
				continue;
			}
			if (!System.IO.Directory.Exists (m_BuildTargetList[i].outputPath)) {
				System.IO.Directory.CreateDirectory (m_BuildTargetList[i].outputPath);
			}
			BuildPipeline.BuildAssetBundles (m_BuildTargetList[i].outputPath, buildList.ToArray (), m_BuildOption, m_BuildTargetList[i].buildTarget);
		}
		ShowNotification ("Build Complete!");
	}

	private	string[]	GetCheckedBundleNames ()
	{
		List<string>	checkedList	= new List<string> ();
		for (int i=0; i<m_AssetBundleItems.Count; i++) {
			if (m_AssetBundleItems[i].isChecked && !(m_AssetBundleItems[i] is Fold)) {
				checkedList.Add (m_AssetBundleItems[i].name);
			}
		}
		return	checkedList.ToArray ();
	}

	private	void	ShowNotification (string iText)
	{
		GUIContent content	= new GUIContent (iText);
		ShowNotification (content);
	}

	private	void	Update ()
	{
		if (m_IsBuildFlag) {
			BuildStart ();
			m_IsBuildFlag	= false;
		}
	}

	#region call by OnDisable
	private	void	SaveValues ()
	{
		EditorPrefs.SetInt (m_SaveKey+m_SaveBuildOption, (int)m_BuildOption);
		EditorPrefs.SetBool (m_SaveKey+m_SaveIsOpenTargetList, m_IsOpenTargetList);

		if (m_BuildTargetList != null) {
			EditorPrefs.SetInt (m_SaveKey+m_SaveBuildTargetCount, m_BuildTargetList.Count);
			for (int i=0; i<m_BuildTargetList.Count; i++) {
				EditorPrefs.SetBool (m_SaveKey+m_SaveTargetListIsValidate+"_"+i, m_BuildTargetList[i].isValidate);
				EditorPrefs.SetInt (m_SaveKey+m_SaveBuildTarget+"_"+i, (int)m_BuildTargetList[i].buildTarget);
				EditorPrefs.SetString (m_SaveKey+m_SaveOutputPath+"_"+i, m_BuildTargetList[i].outputPath);
			}
		}

		if (m_AssetBundleItems != null) {
			Item item;
			Fold fold;
			for (int i=0; i<m_AssetBundleItems.Count; i++) {
				item	= m_AssetBundleItems[i];
				EditorPrefs.SetBool (m_SaveKey+item.name+m_SaveIsChecked, item.isChecked);
				if (!(m_AssetBundleItems[i] is Fold)) {
					continue;
				}
				fold	= (Fold)item;
				EditorPrefs.SetBool (m_SaveKey+fold.name+m_SaveIsOpen, fold.isOpen);
			}
		}
	}
	#endregion

	#region call SaveValues
	private	void	OnDisable ()		{ SaveValues (); }
	private	void	OnLostFocus ()		{ SaveValues (); }
	private	void	OnProjectChange ()	{ SaveValues (); }
	#endregion

	#region call Initialize
	private	void	OnEnable ()	{ Initialize (); }
	private	void	OnFocus ()	{ Initialize (); }
	#endregion


	#region classes

	private	class BuildTargetWithPath
	{
		public	BuildTargetWithPath ()
		{
			this.isValidate		= true;
			this.buildTarget	= BuildTarget.iOS;
			this.outputPath		= "";
		}

		public	bool	isValidate;
		public	BuildTarget	buildTarget;
		public	string	outputPath;
	}

	private	class Fold : Item
	{
		#region Constructor
		public	Fold (string name) : base (name)
		{
			this.children	= new List<Item> ();
		}
		#endregion

		#region params
		public	bool	isOpen {
			get;
			private	set;
		}

		public	List<Item>	children {
			get;
			private	set;
		}
		#endregion


		public	void	Open ()
		{
			this.isOpen	= true;
		}

		public	void	Close ()
		{
			this.isOpen	= false;
		}

		public	void	AddItem (Item item)
		{
			this.children.Add (item);
		}

		public	void	RemoveItem (Item item)
		{
			this.children.Remove (item);
		}

		public override void Check (bool check)
		{
			for (int i=0; i<this.children.Count; i++) {
				if (check) {
					this.children[i].CheckOn ();
				} else {
					this.children[i].CheckOff ();
				}
				if (this.children[i] is Fold) {
					((Fold)this.children[i]).Check (check);
				}
			}
			base.Check (check);
		}

		public	bool	ContainsItem (Item item)
		{
			for (int i=0; i<this.children.Count; i++) {
				if (!(this.children[i] is Fold) && this.children[i].name == item.name) {
					return	true;
				}
			}
			return	false;
		}

		public	bool	ContainsFold (Fold fold)
		{
			for (int i=0; i<this.children.Count; i++) {
				if ((this.children[i] is Fold) && this.children[i].name == fold.name) {
					return	true;
				}
			}
			return	false;
		}
	}

	private	class Item
	{
		#region Constructor
		public	Item (string name)
		{
			this.name	= name;
		}
		#endregion

		#region params
		public	Fold	parent {
			get;
			private	set;
		}
		public	string	name {
			get;
			private	set;
		}
		public	bool	isChecked {
			get;
			private	set;
		}
		#endregion

		public	void	SetParent (Fold parent)
		{
			this.parent	= parent;
		}

		public	virtual	void	Check (bool check)
		{
			this.isChecked	= check;

			Fold fold = this.parent;
			while (fold != null) {
				bool	checkTo = true;
				for (int i=0; i<fold.children.Count; i++) {
					if (!fold.children[i].isChecked) {
						checkTo	= false;
						break;
					}
				}
				if (checkTo) {
					fold.CheckOn ();
				} else {
					fold.CheckOff ();
				}
				fold	= fold.parent;
			}
		}

		public	void	CheckOn ()
		{
			this.isChecked	= true;
		}

		public	void	CheckOff ()
		{
			this.isChecked	= false;
		}
	}
	#endregion
}

public	class BuildListPopup : PopupWindowContent
{

	static	public	BuildListPopup	ShowList (string[] iTexts, EditorWindow iFrom)
	{
		BuildListPopup	window	= new BuildListPopup ();

		window.SetList (iTexts);
		PopupWindow.Show (new Rect (iFrom.position.width, iFrom.position.height, 200.0f, 400.0f), window);

		return	window;
	}

	private	string[]	m_Texts;
	private	Vector2		m_ScrollArea;

	public	void	SetList (string[] iTexts)
	{
		m_Texts	= iTexts;

		if (m_Texts == null) {
			m_Texts	= new string[0];
		}
	}

	public override void OnGUI (Rect rect)
	{
		m_ScrollArea	= EditorGUILayout.BeginScrollView (m_ScrollArea);

		if (m_Texts == null || m_Texts.Length == 0) {
			GUILayout.Label ("No checked asset bundles.");
		} else {
			for (int i=0; i<m_Texts.Length; i++) {
				GUILayout.Label (m_Texts[i]);
			}
		}

		EditorGUILayout.EndScrollView ();
	}

	public override Vector2 GetWindowSize ()
	{
		return	new Vector2 (300.0f, 18*Mathf.Max (m_Texts.Length, 1)+2);
	}
}
