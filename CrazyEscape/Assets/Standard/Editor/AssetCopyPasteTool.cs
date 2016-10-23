using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class AssetCopyPasteTool : Editor {

	static	private	Object[]	cacheObjects;

	[MenuItem ("Assets/Copy&Paste/Copy &c")]
	static	public	void	Copy ()
	{
		cacheObjects	= Selection.objects;
	}

	[MenuItem ("Assets/Copy&Paste/Copy &c", true, 50)]
	static	public	bool	CopyCheck ()
	{
		return	(Selection.objects.Length > 0);
	}


	[MenuItem ("Assets/Copy&Paste/Paste &v")]
	static	public	void	Paste ()
	{
		string	fromPath;
		string	toPath;
		Object[]	selectObjects	= Selection.objects;

		if (selectObjects == null || selectObjects.Length == 0) {
			foreach (Object copyObject in cacheObjects) {
				fromPath	= AssetDatabase.GetAssetPath (copyObject);
				toPath		= IncrementPath ("Assets/" + GetAssetNameFromPath (fromPath));
				AssetDatabase.CopyAsset (fromPath, toPath);
			}
		} else {
			foreach (Object dirObject in selectObjects) {
				string directryPath	= AssetDatabase.GetAssetPath (dirObject);
				if (!directryPath.Contains ("Assets/")) {
					continue;
				}

				if (!IsDirectryPath (directryPath)) {
					directryPath	= directryPath.Replace ("/"+GetAssetNameFromPath (directryPath), "");
				}
				foreach (Object copyObject in cacheObjects) {
					fromPath	= AssetDatabase.GetAssetPath (copyObject);
					toPath		= directryPath;

					if (IsDirectryPath (fromPath)) {
						string	newDirGUID	= AssetDatabase.CreateFolder (toPath, GetAssetNameFromPath (fromPath));
						string	newDirectry	= AssetDatabase.GUIDToAssetPath (newDirGUID);

						string[]	searchDirs	= new string[1];
						searchDirs[0]	= fromPath;
						foreach (string inDirGUID in AssetDatabase.FindAssets ("", searchDirs)) {
							fromPath	= AssetDatabase.GUIDToAssetPath (inDirGUID);
							toPath		= IncrementPath (newDirectry + "/" + GetAssetNameFromPath (fromPath));

							if (fromPath == newDirectry) {
								continue;
							}
							AssetDatabase.CopyAsset (fromPath, toPath);
						}
					} else {
						toPath		= IncrementPath (toPath + "/" + GetAssetNameFromPath (fromPath));
						AssetDatabase.CopyAsset (fromPath, toPath);
					}
				}
			}
		}

		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}

	[MenuItem ("Assets/Copy&Paste/Paste &v", true, 51)]
	static	public	bool	PasteCheck ()
	{
		return	(cacheObjects != null && cacheObjects.Length > 0);
	}



	static	private	string	IncrementPath (string path)
	{
		string newPath = path;
		int	increments	= 1;
		while (AssetDatabase.LoadAssetAtPath<Object> (newPath) != null) {
			if (path.Contains (".")) {
				newPath	= path.Replace (".", " "+increments.ToString ()+".");
			} else {
				newPath	= path + " " + increments.ToString ();
			}
			increments++;
		}
		return	newPath;
	}

	static	private	bool	IsDirectryPath (string path)
	{
		return	Directory.Exists (path);
	}

	static	private	string	GetAssetNameFromPath (string path)
	{
		string slash = "/";
		if (!path.Contains (slash)) {
			return	path;
		} else {
			return	path.Replace ( path.Substring(0, path.LastIndexOf (slash)+1), "");
		}
	}
}
