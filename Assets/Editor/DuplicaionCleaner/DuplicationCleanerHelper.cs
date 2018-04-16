/********************************************************************
*	created:	17/4/2018   0:19
*	filename: 	DuplicationCleanerHelper
*	author:		Bing Lau
*
*	purpose:	https://github.com/gggg826/DuplicationResourcesCleaner
*********************************************************************/

using System.Collections.Generic;
using System.IO;

namespace DuplicaionCleaner
{
	public class DuplicationCleanerHelper
	{
		public static void CheckDuplication(AssetBase assetData)
		{
			if(assetData == null)
			{
				return;
			}

			if(assetData.AssetDic == null)
			{
				assetData.AssetDic = new Dictionary<string, List<AssetFileData>>();
			}
			else
			{
				assetData.AssetDic.Clear();
			}

			string path = GetArtPathByAbsolute(Config.Art_Path);
			if (!string.IsNullOrEmpty(path))
			{
				string[] assetsPathArray = Directory.GetFiles(path, assetData.AssetType, SearchOption.AllDirectories);

				for (int i = 0; i < assetsPathArray.Length; i++)
				{
					string assetName = Path.GetFileNameWithoutExtension(assetsPathArray[i]);
					assetData.CheckResource(assetName, GetPathByRelative(assetsPathArray[i]));
				}
			}
		}

		public static void ReadConfig(string path, ref Dictionary<string, List<AssetFileData>> assetDic)
		{

		}

		public static void SaveConfig(string path, Dictionary<string, List<AssetFileData>> assetDic)
		{

		}

		public static string GetPathByRelative(string path)
		{
			int positionIndex = path.IndexOf("Assets");
			if (positionIndex == -1)
			{
				return string.Empty;
			}
			return path.Substring(positionIndex);
		}

		public static string GetArtPathByAbsolute(string path)
		{
			return Path.GetFullPath(path);
		}
	}
}