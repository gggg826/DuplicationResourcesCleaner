/********************************************************************
*	created:	16/4/2018   23:36
*	filename: 	CleanerWindow
*	author:		Bing Lau
*
*	purpose:	https://github.com/gggg826/DuplicationResourcesCleaner
*********************************************************************/

using System;
using UnityEditor;
using UnityEngine;

namespace DuplicaionCleaner
{
	public class CleanerWindow : EditorWindow
	{
		protected AssetBase m_CurrentAssetBase;
		protected FBXAsset m_FBXAssets;
		protected MaterialAsset m_MaterialAssets;
		protected TextureAsset m_TextureAsset;
		protected Vector2 m_DuplicationAssetScrollPosition;
		protected string[] m_AssetTypes;

		[MenuItem("Window/Duplication Cleaner &d")]
		public static void OpenLableModifier()
		{
			GetWindow<CleanerWindow>("Dup Cleaner", true).Show();
		}

		protected void Initialize()
		{
			if(m_FBXAssets == null)
			{
				m_FBXAssets = new FBXAsset(Config.Asset_Choosed_Config_Save_Folder);
				m_FBXAssets.LoadChooseConfig();
			}

			if (m_MaterialAssets == null)
			{
				m_MaterialAssets = new MaterialAsset(Config.Asset_Choosed_Config_Save_Folder);
				m_MaterialAssets.LoadChooseConfig();
			}

			if (m_TextureAsset == null)
			{
				m_TextureAsset = new TextureAsset(Config.Asset_Choosed_Config_Save_Folder);
				m_TextureAsset.LoadChooseConfig();
			}

			if (m_CurrentAssetBase == null)
			{
				m_CurrentAssetBase = m_MaterialAssets;
				Config.Search_Extensions = m_MaterialAssets.ReplaceAssetsExtensions;
				m_CurrentAssetBase.CheckDuplication();
			}

			if(m_AssetTypes == null)
			{
				m_AssetTypes = Enum.GetNames(typeof(ASSETTYPE));
			}
		}

		protected void OnGUI()
		{
			Initialize();

			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginVertical();
			DrawView();
			GUILayout.EndVertical();
			GUILayout.Space(5);
			GUILayout.EndHorizontal();
			
		}

		protected void DrawView()
		{
			GUILayout.Space(10);
			Config.Art_Path = EditorGUILayout.TextField("Art_Path", Config.Art_Path, GUILayout.MinWidth(100));
			Config.Search_Path = EditorGUILayout.TextField("Search_Path", Config.Search_Path, GUILayout.MinWidth(100));
			Config.Search_Extensions = EditorGUILayout.TextField("Search_Extensions", Config.Search_Extensions, GUILayout.MinWidth(100));
			GUILayout.Space(10);
			string newSelectAssetType = DrawHeaderButtons(m_AssetTypes, 10, m_CurrentAssetBase.AssetType.ToString());
			if(!string.Equals(newSelectAssetType, m_CurrentAssetBase.AssetType.ToString()))
			{
				ChangeCurrentSelectType(newSelectAssetType);
			}

			GUILayout.Space(10);
			GUILayout.Label("Current Duplication Count : " + m_CurrentAssetBase.DuplicationAssetDic.Count);
			GUILayout.Space(10);

			m_DuplicationAssetScrollPosition = GUILayout.BeginScrollView(m_DuplicationAssetScrollPosition);
			m_CurrentAssetBase.Draw();
			GUILayout.EndScrollView();

			GUILayout.Space(10);
			//底部按钮
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Check Duplication", GUILayout.MinHeight(30)))
			{
				m_CurrentAssetBase.CheckDuplication();
			}

			if (GUILayout.Button("Save Choose", GUILayout.MinHeight(30)))
			{
				m_CurrentAssetBase.SaveChooseConfig();
			}

			if (GUILayout.Button("Replace Resoures", GUILayout.MinHeight(30)))
			{
				m_CurrentAssetBase.ReplaceAssets(Config.Search_Path, Config.Search_Extensions);
			}

			if (GUILayout.Button("Delete Unuseful", GUILayout.MinHeight(30)))
			{
				m_CurrentAssetBase.DeleteUnusefulAssets();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
		}

		protected void ChangeCurrentSelectType(string newSelectAssetType)
		{
			ASSETTYPE assetType = (ASSETTYPE)Enum.Parse(typeof(ASSETTYPE), newSelectAssetType);
			switch (assetType)
			{
				case ASSETTYPE.FBX:
					m_CurrentAssetBase = m_FBXAssets;
					break;
				case ASSETTYPE.Materials:
					m_CurrentAssetBase = m_MaterialAssets;
					break;
				case ASSETTYPE.Textures:
					m_CurrentAssetBase = m_TextureAsset;
					break;
			}
			Config.Search_Extensions = m_CurrentAssetBase.ReplaceAssetsExtensions;
			m_CurrentAssetBase.CheckDuplication();
		}

		protected string DrawHeaderButtons(string[] texts, int padding, string selectionIndex, int minHeight = 20)
		{
			if (texts == null || texts.Length == 0)
				return string.Empty;
			string newSelect = selectionIndex;
			string style = null;
			GUILayout.BeginHorizontal();
			GUILayout.Space(padding);
			for (int index = 0; index < texts.Length; index++)
			{
				if (index == 0 && index != texts.Length - 1)
					style = "ButtonLeft";
				else if (index == texts.Length - 1 && index != 0)
					style = "ButtonRight";
				else
					style = "ButtonMid";

				if (GUILayout.Toggle(newSelect == texts[index], texts[index], style, GUILayout.MinHeight(minHeight)))
					newSelect = texts[index];
				if (newSelect != selectionIndex)
					selectionIndex = newSelect;
			}
			GUILayout.Space(padding);
			GUILayout.EndHorizontal();
			return newSelect;
		}
	}
}