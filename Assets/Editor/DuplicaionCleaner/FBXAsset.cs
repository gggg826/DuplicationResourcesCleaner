/********************************************************************
*	created:	18/4/2018   0:34
*	filename: 	FBXAsset
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/DuplicationResourcesCleaner
*********************************************************************/

using System.Collections.Generic;

namespace DuplicaionCleaner
{
	public class FBXAsset : AssetBase
	{
		public FBXAsset(string chooseConfigFolder)
		{
			DuplicationAssetGroupDic = new Dictionary<string, AssetGroupData>();
			AssetExtensions = new string[] { ".FBX" };
			AssetType = ASSETTYPE.FBX;
			ReplaceAssetsExtensions = ".prefab,.unity";
			ChooseConfigSavePath = chooseConfigFolder + "fbx_choosed_config.txt";
		}
	}
}