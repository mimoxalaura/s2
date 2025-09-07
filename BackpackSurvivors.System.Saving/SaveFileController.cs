using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using BackpackSurvivors.Game.Saving;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.System.Saving;

public static class SaveFileController
{
	internal static int LowestSupportedSaveBuildNumber = 96;

	public static void Save(SaveGame saveGame, string fileName)
	{
		saveGame.UpdateSavedAtBuildNumber();
		string totalFilePath = GetTotalFilePath(fileName);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using FileStream fileStream = new FileStream(totalFilePath, FileMode.Create);
		binaryFormatter.Serialize(fileStream, saveGame);
		fileStream.Close();
	}

	public static SaveGame Load(string fileName)
	{
		string totalFilePath = GetTotalFilePath(fileName);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using FileStream fileStream = new FileStream(totalFilePath, FileMode.Open);
		SaveGame result = binaryFormatter.Deserialize(fileStream) as SaveGame;
		fileStream.Close();
		return result;
	}

	public static SaveGame Load(Guid key)
	{
		return Load(CreateAndReturnFullFilePath(key));
	}

	public static string CreateAndReturnFullFilePath(Guid key)
	{
		string fileName = $"save{key}.bps";
		CheckForDirectory();
		string totalFilePath = GetTotalFilePath(fileName);
		if (!File.Exists(totalFilePath))
		{
			File.Create(totalFilePath).Close();
			Save(new SaveGame(key), fileName);
		}
		return totalFilePath;
	}

	public static string GetFilePath(string fileName)
	{
		return GetTotalFilePath(fileName);
	}

	private static void CheckForDirectory()
	{
		if (!Directory.Exists(GetTotalDirPath()))
		{
			Directory.CreateDirectory(GetTotalDirPath());
		}
	}

	public static void LoadDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionaryToFill, Dictionary<TKey, TValue> source)
	{
		dictionaryToFill.Clear();
		if (source == null)
		{
			return;
		}
		foreach (KeyValuePair<TKey, TValue> item in source)
		{
			dictionaryToFill.Add(item.Key, item.Value);
		}
	}

	private static string GetTotalFilePath(string fileName)
	{
		return Path.Combine(GetTotalDirPath(), fileName);
	}

	private static string GetTotalDirPath()
	{
		string path = "Saves";
		return Path.Combine(Application.persistentDataPath, path);
	}

	internal static List<SaveGame> LoadAllFiles()
	{
		List<SaveGame> list = new List<SaveGame>();
		foreach (string saveFilePath in GetSaveFilePaths())
		{
			list.Add(Load(saveFilePath));
		}
		RemoveUnsupportedSaveVersions(list);
		if (!list.Any((SaveGame x) => !x.HasData()))
		{
			Guid guid = Guid.NewGuid();
			CreateAndReturnFullFilePath(guid);
			SaveGame item = new SaveGame(guid);
			list.Add(item);
		}
		return list;
	}

	internal static void RemoveUnsupportedSaveVersions(List<SaveGame> saveGames)
	{
		foreach (SaveGame item in saveGames.Where((SaveGame x) => x.SavedAtBuildNumber < LowestSupportedSaveBuildNumber).ToList())
		{
			saveGames.Remove(item);
		}
	}

	[Command("Build.UpdateSaveGameBuildNumber", Platform.AllPlatforms, MonoTargetType.Single)]
	internal static void CHEAT_UpdateSavegamesBuildNumber()
	{
		foreach (string saveFilePath in GetSaveFilePaths())
		{
			SaveGame saveGame = Load(saveFilePath);
			saveGame.UpdateSavedAtBuildNumber();
			Save(saveGame, saveFilePath);
		}
	}

	private static List<string> GetSaveFilePaths()
	{
		CheckForDirectory();
		return Directory.GetFiles(GetTotalDirPath(), "*.bps").ToList();
	}

	internal static void Delete(Guid key)
	{
		File.Delete(CreateAndReturnFullFilePath(key));
	}
}
