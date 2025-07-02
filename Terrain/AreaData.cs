using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class AreaData
{
	public string AreaName;
	public string SaveDirectoryPath; // このエリアのデータの保存先ディレクトリ
																	 // 例: public string WeatherSetting; // エリアの天候設定など

	public AreaData(string areaName, string saveDirectoryPath)
	{
		AreaName = areaName;
		SaveDirectoryPath = saveDirectoryPath;
	}

	/// <summary>
	/// エリアデータをファイルに保存します。
	/// </summary>
	public void Save()
	{
		EditFile.SaveObjectAsCompressedJson(SaveDirectoryPath, this);
	}
}