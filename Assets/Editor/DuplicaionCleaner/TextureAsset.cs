/********************************************************************
*	created:	18/4/2018   0:33
*	filename: 	TextureAsset
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/DuplicationResourcesCleaner
*********************************************************************/

using System.Collections.Generic;

namespace DuplicaionCleaner
{
	public class TextureAsset : AssetBase
	{
		public TextureAsset(string chooseConfigFolder)
		{
			DuplicationAssetGroupDic = new Dictionary<string, AssetGroupData>();
			AssetExtensions = new string[] { ".TGA", ".png" };
			AssetType = ASSETTYPE.Textures;
			ReplaceAssetsExtensions = ".prefab,.unity";
			ChooseConfigSavePath = chooseConfigFolder + "texture_choosed_config.txt";
		}
	}
}