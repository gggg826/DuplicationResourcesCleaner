/********************************************************************
*	created:	17/4/2018   0:34
*	filename: 	AssetBase
*	author:		Bing Lau
*
*	purpose:	https://github.com/gggg826/DuplicationResourcesCleaner
*********************************************************************/

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

	public class AssetBase
	{
		public Dictionary<string, List<AssetFileData>> DuplicationAssetDic;
		public string ChooseConfigSavePath;
		public string AssetExtension;
		public ASSETTYPE AssetType;
		
		private Dictionary<string, bool> m_AssetDownShowStates;
		private string SearchExtensions;

		public virtual void RemoveUnusefulAssets()
		{

		}

		public virtual void ReplaceAssets(string relativePath, string extensions)
		{
			List<string> assetsPath = DuplicationCleanerHelper.CollectAssets(relativePath, extensions.Split(','));
			foreach (var duplicationAsset in DuplicationAssetDic)
			{

				//筛选出最终使用资源和要替换的资源的GUID
				List<string> oldAssetGUIDs = new List<string>();
				string newAssetGUIDs = string.Empty;
				bool choosedNewAssetGUID = false;
				int i = 0;
				for (i = 0; i < duplicationAsset.Value.Count; i++)
				{
					string guid = duplicationAsset.Value[i].GUID;
					if(string.IsNullOrEmpty(guid))
					{
						continue;
					}
					if (duplicationAsset.Value[i].IsChoosed && !choosedNewAssetGUID)
					{
						newAssetGUIDs = guid;
						choosedNewAssetGUID = true;
						continue;
					}
					oldAssetGUIDs.Add(guid);
				}
				if(string.IsNullOrEmpty(newAssetGUIDs) || oldAssetGUIDs.Count <= 0)
				{
					continue;
				}

				for (i = 0; i < oldAssetGUIDs.Count; i++)
				{
					//DuplicationCleanerHelper.DoReplace()
				}
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
				DuplicationCleanerHelper.ReadConfig(ChooseConfigSavePath, ref DuplicationAssetDic);
			}
		}

		public void SaveChooseConfig()
		{
			DuplicationCleanerHelper.SaveConfig(ChooseConfigSavePath, DuplicationAssetDic);
			LoadChooseConfig();
		}

		public void CheckDuplication()
		{
			DuplicationCleanerHelper.CheckDuplication(this);
		}

		public void CheckResource(string name, string relativePath)
		{
			if(DuplicationAssetDic == null)
			{
				return;
			}

			AddAssetData(name, relativePath);
		}

		public void RemoveUnDuplicationKey()
		{
			if(DuplicationAssetDic == null)
			{
				return;
			}
			
			if (m_AssetDownShowStates == null)
			{
				m_AssetDownShowStates = new Dictionary<string, bool>();
			}
			else
			{
				m_AssetDownShowStates.Clear();
			}

			List<string> unDuplicationList = new List<string>();
			foreach (var duplicaionAsset in DuplicationAssetDic)
			{
				if(duplicaionAsset.Value == null || duplicaionAsset.Value.Count <= 1)
				{
					unDuplicationList.Add(duplicaionAsset.Key);
				}
				else
				{
					m_AssetDownShowStates.Add(duplicaionAsset.Key, false);
				}
			}
			for (int i = 0; i < unDuplicationList.Count; i++)
			{
				DuplicationAssetDic.Remove(unDuplicationList[i]);
			}

			
		}
		
		public void Draw()
		{
			foreach (var duplicaionAsset in DuplicationAssetDic)
			{
				m_AssetDownShowStates[duplicaionAsset.Key] = GUILayout.Toggle(m_AssetDownShowStates[duplicaionAsset.Key], duplicaionAsset.Key, "Button");
				if (m_AssetDownShowStates[duplicaionAsset.Key])
				{
					EditorGUI.indentLevel++;
					for (int i = 0; i < duplicaionAsset.Value.Count; i++)
					{
						duplicaionAsset.Value[i].Draw();
					}
					EditorGUI.indentLevel--;
				}
			}
		}

		protected bool IsContainsAsset(string key, string relativePath)
		{
			List<AssetFileData> duplicaionAsset = DuplicationAssetDic[key];
			return IsContainsAsset(duplicaionAsset, relativePath);
		}

		protected bool IsContainsAsset(List<AssetFileData> assets, string relativePath)
		{
			if (assets == null)
			{
				return false;
			}
			for (int i = 0; i < assets.Count; i++)
			{
				if (assets[i].AssetPath == relativePath)
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

			List<AssetFileData> assetFiles = DuplicationAssetDic.ContainsKey(key) ? DuplicationAssetDic[key] : null;
			if (assetFiles == null)
			{
				assetFiles = new List<AssetFileData>();
				asset.IsChoosed = true;
				assetFiles.Add(asset);
				DuplicationAssetDic.Add(key, assetFiles);
			}
			else
			{
				if(IsContainsAsset(key, relativePath))
				{
					Debug.LogError("Guid Duplication!!  SourceName:" + key);
				}
				asset.IsChoosed = false;
				DuplicationAssetDic[key].Add(asset);
			}
		}
	}

	public class MaterialAsset : AssetBase
	{
		public MaterialAsset(string chooseConfigPath)
		{
			DuplicationAssetDic = new Dictionary<string, List<AssetFileData>>();
			AssetExtension = "*.mat";
			AssetType = ASSETTYPE.Materials;
			ChooseConfigSavePath = chooseConfigPath;
		}
	}
}