using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuplicaionCleaner
{
	public class MaterialAsset : AssetBase
	{
		public MaterialAsset(string chooseConfigFolder)
		{
			DuplicationAssetDic = new Dictionary<string, List<AssetFileData>>();
			AssetExtensions = new string[] { ".mat" };
			AssetType = ASSETTYPE.Materials;
			ReplaceAssetsExtensions = ".prefab,.unity";
			ChooseConfigSavePath = chooseConfigFolder + "mat_choosed_config.txt";
		}
	}
}
