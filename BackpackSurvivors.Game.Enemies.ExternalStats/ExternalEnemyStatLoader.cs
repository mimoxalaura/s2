using System.Collections.Generic;
using System.IO;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.ExternalStats;

internal static class ExternalEnemyStatLoader
{
	private const string ExternalEnemyStatsFilename = "The Final Mountain - Enemies.tsv";

	private static int _idColumnIndex = 0;

	private static int _hPColumnIndex = 2;

	private static int _moveSpeedColumnIndex = 3;

	private static int _minDamageColumnIndex = 4;

	private static int _maxDamageColumnIndex = 5;

	internal static Dictionary<int, ExternalEnemyStat> GetExternalEnemyStats()
	{
		Dictionary<int, ExternalEnemyStat> dictionary = new Dictionary<int, ExternalEnemyStat>();
		string[] array = File.ReadAllLines(GetFilepath());
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (!array2[_hPColumnIndex].ToString().Equals(string.Empty))
			{
				Dictionary<Enums.ItemStatType, float> dictionary2 = new Dictionary<Enums.ItemStatType, float>();
				int num = int.Parse(array2[_idColumnIndex]);
				float value = float.Parse(array2[_hPColumnIndex]);
				dictionary2.Add(Enums.ItemStatType.Health, value);
				float value2 = float.Parse(array2[_moveSpeedColumnIndex]);
				dictionary2.Add(Enums.ItemStatType.SpeedPercentage, value2);
				float minDamage = float.Parse(array2[_minDamageColumnIndex]);
				float maxDamage = float.Parse(array2[_maxDamageColumnIndex]);
				ExternalEnemyStat externalEnemyStat = new ExternalEnemyStat();
				externalEnemyStat.EnemyId = num;
				externalEnemyStat.EnemyStats = dictionary2;
				externalEnemyStat.MinDamage = minDamage;
				externalEnemyStat.MaxDamage = maxDamage;
				dictionary.Add(num, externalEnemyStat);
			}
		}
		return dictionary;
	}

	internal static string GetFilepath()
	{
		return Path.Combine(Application.persistentDataPath, "The Final Mountain - Enemies.tsv");
	}
}
