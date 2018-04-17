using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuplicaionCleaner
{
	public class TextureAsset : AssetBase
	{
		public TextureAsset(string chooseConfigFolder)
		{
			DuplicationAssetDic = new Dictionary<string, List<AssetFileData>>();
			AssetExtensions = new string[] { ".TGA", ".png" };
			AssetType = ASSETTYPE.Textures;
			ReplaceAssetsExtensions = ".prefab,.unity";
			ChooseConfigSavePath = chooseConfigFolder + "texture_choosed_config.txt";
		}
	}
}
