using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomTextureImporter : AssetPostprocessor
{
	public override int GetPostprocessOrder ()
	{
		return base.GetPostprocessOrder ();
	}

	private void OnPreprocessTexture()
	{
		TextureImporter importer = assetImporter as TextureImporter;

		if (importer != null && AssetDatabase.LoadAssetAtPath<Texture>(importer.assetPath) == null) {
			importer.textureType = TextureImporterType.Sprite;
			importer.spritePixelsPerUnit = 100;
			importer.spriteImportMode = SpriteImportMode.Single;
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.filterMode = FilterMode.Trilinear;
			importer.maxTextureSize = 1024;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.crunchedCompression = false;
		}
	}
}
