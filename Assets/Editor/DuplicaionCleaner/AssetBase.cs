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
		public Dictionary<string, List<AssetFileData>> AssetDic;
		public string ChooseConfigSavePath;
		public string AssetExtension;
		public ASSETTYPE AssetType;
		private Dictionary<string, bool> m_AssetDownShowStates;

		public void LoadChooseConfig()
		{
			if (string.IsNullOrEmpty(ChooseConfigSavePath))
			{
				return;
			}

			if (File.Exists(ChooseConfigSavePath))
			{
				DuplicationCleanerHelper.ReadConfig(ChooseConfigSavePath, ref AssetDic);
			}
		}

		public void SaveChooseConfig()
		{
			DuplicationCleanerHelper.SaveConfig(ChooseConfigSavePath, AssetDic);
			LoadChooseConfig();
		}

		public void CheckDuplication()
		{
			DuplicationCleanerHelper.CheckDuplication(this);
		}

		public void CheckResource(string name, string relativePath)
		{
			if(AssetDic == null)
			{
				return;
			}

			AddAssetData(name, relativePath);
		}

		public void RemoveUnDuplicationKey()
		{
			if(AssetDic == null)
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
			foreach (var assets in AssetDic)
			{
				if(assets.Value == null || assets.Value.Count <= 1)
				{
					unDuplicationList.Add(assets.Key);
				}
				else
				{
					m_AssetDownShowStates.Add(assets.Key, false);
				}
			}
			for (int i = 0; i < unDuplicationList.Count; i++)
			{
				AssetDic.Remove(unDuplicationList[i]);
			}

			
		}

		public void Draw()
		{
			foreach (var assetData in AssetDic)
			{
				m_AssetDownShowStates[assetData.Key] = GUILayout.Toggle(m_AssetDownShowStates[assetData.Key], assetData.Key, "Button");
				if (m_AssetDownShowStates[assetData.Key])
				{
					EditorGUI.indentLevel++;
					for (int i = 0; i < assetData.Value.Count; i++)
					{
						assetData.Value[i].Draw();
					}
					EditorGUI.indentLevel--;
				}
			}
		}

		protected bool IsContainsAsset(string key, string relativePath)
		{
			List<AssetFileData> assets = AssetDic[key];
			return IsContainsAsset(assets, relativePath);
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

			List<AssetFileData> assets = AssetDic.ContainsKey(key) ? AssetDic[key] : null;
			if (assets == null)
			{
				assets = new List<AssetFileData>();
				asset.IsChoosed = true;
				assets.Add(asset);
				AssetDic.Add(key, assets);
			}
			else
			{
				if(IsContainsAsset(key, relativePath))
				{
					Debug.LogError("Guid Duplication!!  SourceName:" + key);
				}
				asset.IsChoosed = false;
				AssetDic[key].Add(asset);
			}
		}
	}

	public class MaterialAsset : AssetBase
	{
		public MaterialAsset(string chooseConfigPath)
		{
			AssetDic = new Dictionary<string, List<AssetFileData>>();
			AssetExtension = "*.mat";
			AssetType = ASSETTYPE.Materials;
			ChooseConfigSavePath = chooseConfigPath;
		}
	}
}