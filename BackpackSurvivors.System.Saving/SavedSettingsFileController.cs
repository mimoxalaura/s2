using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BackpackSurvivors.Game.Saving;
using UnityEngine;

namespace BackpackSurvivors.System.Saving;

internal static class SavedSettingsFileController
{
	public static void Save(SettingsSaveState settingsState)
	{
		string savedSettingsFilePath = GetSavedSettingsFilePath();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using FileStream fileStream = new FileStream(savedSettingsFilePath, FileMode.Create);
		binaryFormatter.Serialize(fileStream, settingsState);
		fileStream.Close();
	}

	public static SettingsSaveState Load()
	{
		try
		{
			string savedSettingsFilePath = GetSavedSettingsFilePath();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using FileStream fileStream = new FileStream(savedSettingsFilePath, FileMode.Open);
			SettingsSaveState result = binaryFormatter.Deserialize(fileStream) as SettingsSaveState;
			fileStream.Close();
			return result;
		}
		catch (Exception)
		{
			File.Delete(GetSavedSettingsFilePath());
		}
		return null;
	}

	private static string GetSavedSettingsFilePath()
	{
		return Path.Combine(Application.persistentDataPath, "BackpackSurvivors.settings");
	}
}
