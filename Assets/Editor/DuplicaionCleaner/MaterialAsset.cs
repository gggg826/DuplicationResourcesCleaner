/********************************************************************
*	created:	18/4/2018   0:33
*	filename: 	MaterialAsset
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/DuplicationResourcesCleaner
*********************************************************************/

using System.Collections.Generic;

namespace DuplicaionCleaner
{
	public class MaterialAsset : AssetBase
	{
		public MaterialAsset(string chooseConfigFolder)
		{
			DuplicationAssetGroupDic = new Dictionary<string, AssetGroupData>();
			AssetExtensions = new string[] { ".mat" };
			AssetType = ASSETTYPE.Materials;
			ReplaceAssetsExtensions = ".prefab,.unity";
			ChooseConfigSavePath = chooseConfigFolder + "mat_choosed_config.txt";
		}
	}
}