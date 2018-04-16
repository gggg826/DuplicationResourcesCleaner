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
		protected static AssetBase m_CurrentAssetBase;
		protected static MaterialAsset m_MaterialAssets;
		protected Vector2 m_DuplicationAssetScrollPosition;

		[MenuItem("Window/Duplication Cleaner &d")]
		public static void OpenLableModifier()
		{
			GetWindow<CleanerWindow>("Dup Cleaner", true).Show();
		}

		protected static void Initialize()
		{
			if(m_MaterialAssets == null)
			{
				m_MaterialAssets = new MaterialAsset(Config.Material_Choosed_Config_Path);
				m_MaterialAssets.LoadChooseConfig();
			}

			if (m_CurrentAssetBase == null)
			{
				m_CurrentAssetBase = m_MaterialAssets;
			}
		}

		protected void OnGUI()
		{
			Initialize();

			GUILayout.Space(10);
			Config.Art_Path = EditorGUILayout.TextField("GameobjectName", Config.Art_Path, GUILayout.MinWidth(100));
			Config.Scenes_Path = EditorGUILayout.TextField("GameobjectName", Config.Scenes_Path, GUILayout.MinWidth(100));

			m_DuplicationAssetScrollPosition = GUILayout.BeginScrollView(m_DuplicationAssetScrollPosition);
			m_CurrentAssetBase.Draw();
			GUILayout.EndScrollView();

			//底部按钮
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Check Duplication", GUILayout.MinWidth(100)))
			{
				m_CurrentAssetBase.CheckDuplication();
			}

			if (GUILayout.Button("Save Choose", GUILayout.MinWidth(100)))
			{
				m_CurrentAssetBase.SaveChooseConfig();
			}

			if (GUILayout.Button("Replace Resoures", GUILayout.MinWidth(100)))
			{

			}

			if (GUILayout.Button("Delete Unuseful", GUILayout.MinWidth(100)))
			{

			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
		}
	}
}