using System.Collections.Generic;
using System.IO;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.Weapons.ExternalStats;

internal static class ExternalWeaponStatLoader
{
	private const string ExternalWeaponStatsFilename = "The Final Mountain - Weapons.tsv";

	private static int _idColumnIndex = 0;

	private static int _priceColumnIndex = 2;

	private static int _rarityColumnIndex = 3;

	private static int _minDamageColumnIndex = 4;

	private static int _maxDamageColumnIndex = 5;

	private static int _critChanceColumnIndex = 6;

	private static int _critMultiplierColumnIndex = 7;

	private static int _rangeColumnIndex = 8;

	private static int _cooldownColumnIndex = 9;

	private static int _piercingColumnIndex = 10;

	internal static Dictionary<int, ExternalWeaponStat> GetExternalWeaponStats()
	{
		Dictionary<int, ExternalWeaponStat> dictionary = new Dictionary<int, ExternalWeaponStat>();
		string[] array = File.ReadAllLines(GetFilepath());
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (!array2[_rarityColumnIndex].ToString().Equals(string.Empty))
			{
				ExternalWeaponStat externalWeaponStat = new ExternalWeaponStat();
				externalWeaponStat.Id = int.Parse(array2[_idColumnIndex]);
				externalWeaponStat.Price = int.Parse(array2[_priceColumnIndex]);
				externalWeaponStat.Rarity = GetRarityFromString(array2[_rarityColumnIndex]);
				externalWeaponStat.MinDamage = float.Parse(array2[_minDamageColumnIndex]);
				externalWeaponStat.MaxDamage = float.Parse(array2[_maxDamageColumnIndex]);
				externalWeaponStat.CritChance = float.Parse(array2[_critChanceColumnIndex]);
				externalWeaponStat.CritMultiplier = float.Parse(array2[_critMultiplierColumnIndex]);
				externalWeaponStat.Range = float.Parse(array2[_rangeColumnIndex]);
				externalWeaponStat.Cooldown = float.Parse(array2[_cooldownColumnIndex]);
				externalWeaponStat.Piercing = int.Parse(array2[_piercingColumnIndex]);
				dictionary.Add(externalWeaponStat.Id, externalWeaponStat);
			}
		}
		return dictionary;
	}

	internal static string GetFilepath()
	{
		return Path.Combine(Application.persistentDataPath, "The Final Mountain - Weapons.tsv");
	}

	private static Enums.PlaceableRarity GetRarityFromString(string rarityString)
	{
		return rarityString switch
		{
			"C" => Enums.PlaceableRarity.Common, 
			"U" => Enums.PlaceableRarity.Uncommon, 
			"R" => Enums.PlaceableRarity.Rare, 
			"E" => Enums.PlaceableRarity.Epic, 
			"L" => Enums.PlaceableRarity.Legendary, 
			"M" => Enums.PlaceableRarity.Mythic, 
			_ => Enums.PlaceableRarity.Common, 
		};
	}
}
