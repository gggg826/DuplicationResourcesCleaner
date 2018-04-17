/********************************************************************
*	created:	17/4/2018   0:34
*	filename: 	AssetBase
*	author:		Bing Lau
*
*	purpose:	https://github.com/gggg826/DuplicationResourcesCleaner
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DuplicaionCleaner
{
	public enum ASSETTYPE
	{
		Materials,
		FBX,
		Textures,
	}

	public class AssetFileData
	{
		public string GUID;
		public string AssetPath;
		public string AssetName;
		public bool IsChoosed;

		public void Draw()
		{
			IsChoosed = EditorGUILayout.ToggleLeft(AssetPath, IsChoosed);
		}
	}

	public class AssetGroupData
	{
		public string GroupName;
		public bool IsChoosed;
		public bool IsDropDown;
		public List<AssetFileData> AssetGroupList;

		public AssetGroupData(string groupName)
		{
			GroupName = groupName;
			IsChoosed = true;
			IsDropDown = true;
			AssetGroupList = new List<AssetFileData>();
		}

		public void Draw()
		{
			GUILayout.BeginHorizontal();
			IsChoosed = GUILayout.Toggle(IsChoosed, GroupName, "Button");
			//IsDropDown = GUILayout.Toggle(IsDropDown, GroupName, "Button");
			GUILayout.EndHorizontal();
			if (IsDropDown)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < AssetGroupList.Count; i++)
				{
					AssetGroupList[i].Draw();
				}
				EditorGUI.indentLevel--;
			}
		}
	}

	public class AssetBase
	{
		public Dictionary<string, AssetGroupData> DuplicationAssetGroupDic;
		public string ChooseConfigSavePath;
		public string ReplaceAssetsExtensions;
		public string[] AssetExtensions;
		public ASSETTYPE AssetType;

		//protected Dictionary<string, bool> m_AssetDownShowStates;
		
		public void DeleteUnusefulAssets()
		{
			List<string> unusefulAssetsPath = new List<string>();
			List<AssetFileData> fileGroup = null;
			foreach (var duplicaionGroupAsset in DuplicationAssetGroupDic)
			{
				if (duplicaionGroupAsset.Value == null || duplicaionGroupAsset.Value.AssetGroupList.Count == 0)
				{
					continue;
				}
				if(!duplicaionGroupAsset.Value.IsChoosed)
				{
					continue;
				}

				fileGroup = duplicaionGroupAsset.Value.AssetGroupList;
				for (int i = 0; i < fileGroup.Count; i++)
				{
					if (!fileGroup[i].IsChoosed)
					{
						unusefulAssetsPath.Add(fileGroup[i].AssetPath);
					}
				}
			}
			for (int i = 0; i < unusefulAssetsPath.Count; i++)
			{
				AssetDatabase.DeleteAsset(unusefulAssetsPath[i]);
			}
			CheckDuplication();
		}

		public void ReplaceAssets(string relativePath, string extensions)
		{
			string[] assetsPath = DuplicationCleanerHelper.CollectAssets(relativePath, extensions.Split(','));

			if(assetsPath == null)
			{
				return;
			}

			//for (int i = 0; i < assetsPath.Length; i++)
			//{
			//	SingleFileReplace(assetsPath[i]);
			//}


			int currentIndex = 0;
			EditorApplication.update = delegate ()
			{
				string assetPath = assetsPath[currentIndex];

				bool isCancel = EditorUtility.DisplayCancelableProgressBar("正在替换中..."
																			, assetPath
																			, (float)currentIndex / (float)assetsPath.Length);

				SingleFileReplace(assetPath);
				currentIndex++;
				if (isCancel || currentIndex >= assetsPath.Length)
				{
					EditorUtility.ClearProgressBar();
					EditorApplication.update = null;
					currentIndex = 0;
				}
			};
		}

		public void DropUpDownAll(bool isDropDown)
		{
			if(DuplicationAssetGroupDic == null || DuplicationAssetGroupDic.Count == 0)
			{
				return;
			}
			foreach (var groupData in DuplicationAssetGroupDic)
			{
				groupData.Value.IsDropDown = isDropDown;
			}
		}

		public void LoadChooseConfig()
		{
			if (string.IsNullOrEmpty(ChooseConfigSavePath))
			{
				return;
			}

			if (File.Exists(ChooseConfigSavePath))
			{
				DuplicationCleanerHelper.ReadConfig(ChooseConfigSavePath, ref DuplicationAssetGroupDic);
			}
		}

		public void SaveChooseConfig()
		{
			DuplicationCleanerHelper.SaveConfig(ChooseConfigSavePath, DuplicationAssetGroupDic);
			LoadChooseConfig();
		}

		public void CheckDuplication()
		{
			DuplicationCleanerHelper.CheckDuplication(this);
		}

		public void CheckDuplicationResource(string name, string relativePath)
		{
			if(DuplicationAssetGroupDic == null)
			{
				return;
			}

			AddAssetData(name, relativePath);
		}

		public void RemoveUnDuplicationKey()
		{
			if(DuplicationAssetGroupDic == null)
			{
				return;
			}
			
			List<string> unDuplicationList = new List<string>();
			foreach (var duplicaionAsset in DuplicationAssetGroupDic)
			{
				if(duplicaionAsset.Value == null || duplicaionAsset.Value.AssetGroupList.Count <= 1)
				{
					unDuplicationList.Add(duplicaionAsset.Key);
				}
			}
			for (int i = 0; i < unDuplicationList.Count; i++)
			{
				DuplicationAssetGroupDic.Remove(unDuplicationList[i]);
			}
		}
		
		public void Draw()
		{
			foreach (var duplicaionGroupAsset in DuplicationAssetGroupDic)
			{
				duplicaionGroupAsset.Value.Draw();
			}
		}

		protected bool IsContainsAsset(string key, string relativePath)
		{
			AssetGroupData duplicaionAsset = DuplicationAssetGroupDic[key];
			return IsContainsAsset(duplicaionAsset, relativePath);
		}

		protected bool IsContainsAsset(AssetGroupData assetGroup, string relativePath)
		{
			if (assetGroup == null)
			{
				return false;
			}
			for (int i = 0; i < assetGroup.AssetGroupList.Count; i++)
			{
				if (assetGroup.AssetGroupList[i].AssetPath == relativePath)
				{
					return true;
				}
			}
			return false;
		}

		protected void AddAssetData(string key, string relativePath)
		{
			//string assetPath = AssetDatabase.GUIDToAssetPath(relativePath);
			if(string.IsNullOrEmpty(relativePath))
			{
				Debug.Log("Asset not found!!  SourceName:" + key + "   Path : " + relativePath);
				return;
			}
			
			AssetFileData asset = new AssetFileData();
			asset.GUID = AssetDatabase.AssetPathToGUID(relativePath);
			asset.AssetPath = relativePath;
			asset.AssetName = key;

			AssetGroupData assetGroup = DuplicationAssetGroupDic.ContainsKey(key) ? DuplicationAssetGroupDic[key] : null;
			if (assetGroup == null)
			{
				assetGroup = new AssetGroupData(key);
				assetGroup.IsChoosed = true;
				asset.IsChoosed = true;
				assetGroup.AssetGroupList.Add(asset);
				DuplicationAssetGroupDic.Add(key, assetGroup);
			}
			else
			{
				if(IsContainsAsset(key, relativePath))
				{
					Debug.LogError("Guid Duplication!!  SourceName:" + key);
				}
				asset.IsChoosed = false;
				DuplicationAssetGroupDic[key].AssetGroupList.Add(asset);
			}
		}

		protected void SingleFileReplace(string fileFullPath)
		{
			foreach (var duplicationAssetGroup in DuplicationAssetGroupDic)
			{
				if (!duplicationAssetGroup.Value.IsChoosed)
				{
					continue;
				}

				//筛选出最终使用资源和要替换的资源的GUID
				List<string> oldAssetGUIDs = new List<string>();
				string newAssetGUIDs = string.Empty;
				bool choosedNewAssetGUID = false;
				int i = 0;
				for (i = 0; i < duplicationAssetGroup.Value.AssetGroupList.Count; i++)
				{
					string guid = duplicationAssetGroup.Value.AssetGroupList[i].GUID;
					if (string.IsNullOrEmpty(guid))
					{
						continue;
					}
					if (duplicationAssetGroup.Value.AssetGroupList[i].IsChoosed && !choosedNewAssetGUID)
					{
						newAssetGUIDs = guid;
						choosedNewAssetGUID = true;
						continue;
					}
					oldAssetGUIDs.Add(guid);
				}
				if (string.IsNullOrEmpty(newAssetGUIDs) || oldAssetGUIDs.Count <= 0)
				{
					continue;
				}

				for (i = 0; i < oldAssetGUIDs.Count; i++)
				{
					DuplicationCleanerHelper.DoReplace(fileFullPath, newAssetGUIDs, oldAssetGUIDs[i]);
				}
			}
		}
	}
}