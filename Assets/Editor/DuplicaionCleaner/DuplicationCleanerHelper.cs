﻿/********************************************************************
*	created:	17/4/2018   0:19
*	filename: 	DuplicationCleanerHelper
*	author:		Bing Lau
*
*	purpose:	https://github.com/gggg826/DuplicationResourcesCleaner
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DuplicaionCleaner
{
	public class DuplicationCleanerHelper
	{
		private static string DoReplace(string fileFullPath, string strReplace, string strFindPattern, bool isBackup = true)
		{
			StringBuilder strError = new StringBuilder();
			string result = string.Empty;
			string inputText = string.Empty;
			string replacement = strReplace;
			string pat = strFindPattern;
			Regex r = new Regex(pat, RegexOptions.IgnoreCase);

			try
			{
				using (StreamReader sr = new StreamReader(fileFullPath))
				{
					inputText = sr.ReadToEnd();
				}

				if (r.IsMatch(inputText))
				{
					if (isBackup == true)
					{
						try
						{
							File.Copy(fileFullPath, fileFullPath + ".bak");
						}
						catch (System.IO.IOException ex)
						{
							File.Copy(fileFullPath, fileFullPath + ".bak", true);
							strError.Append("Error:  " + ex.Message);
						}
					}
					result = r.Replace(inputText, replacement);

					using (StreamWriter sw = new StreamWriter(fileFullPath))
					{
						sw.Write(result);
					}
				}
				strError.Append("Log:  " + fileFullPath + "替换完成！！！");
			}
			catch (Exception e)
			{
				strError.Append("Error:  " + string.Format("The process failed: {0}", e.ToString()));
			}
			return strError.ToString();
		}

		public static void CheckDuplication(AssetBase assetsData)
		{
			if (assetsData == null)
			{
				return;
			}

			if (assetsData.AssetDic == null)
			{
				assetsData.AssetDic = new Dictionary<string, List<AssetFileData>>();
			}
			else
			{
				assetsData.AssetDic.Clear();
			}

			string path = GetArtPathByAbsolute(Config.Art_Path);
			if (!string.IsNullOrEmpty(path))
			{
				string[] assetsPathArray = Directory.GetFiles(path, assetsData.AssetExtension, SearchOption.AllDirectories);

				for (int i = 0; i < assetsPathArray.Length; i++)
				{
					string assetName = Path.GetFileNameWithoutExtension(assetsPathArray[i]);
					assetsData.CheckResource(assetName, GetPathByRelative(assetsPathArray[i]));
				}
				assetsData.RemoveUnDuplicationKey();
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