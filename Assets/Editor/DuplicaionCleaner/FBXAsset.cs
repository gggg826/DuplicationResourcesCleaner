using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuplicaionCleaner
{
	public class FBXAsset : AssetBase
	{
		public FBXAsset(string chooseConfigFolder)
		{
			DuplicationAssetDic = new Dictionary<string, List<AssetFileData>>();
			AssetExtensions = new string[] { ".FBX" };
			AssetType = ASSETTYPE.FBX;
			ReplaceAssetsExtensions = ".prefab,.unity";
			ChooseConfigSavePath = chooseConfigFolder + "fbx_choosed_config.txt";
		}
	}
}
