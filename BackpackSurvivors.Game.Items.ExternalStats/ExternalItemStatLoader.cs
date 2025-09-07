using System.Collections.Generic;
using System.IO;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.ExternalStats;

internal static class ExternalItemStatLoader
{
	private const string ExternalItemStatsFilename = "The Final Mountain - Items.tsv";

	private static int _idColumnIndex = 0;

	private static int _priceColumnIndex = 2;

	private static int _rarityColumnIndex = 3;

	internal static Dictionary<int, ExternalItemStat> GetExternalItemStats()
	{
		Dictionary<int, ExternalItemStat> dictionary = new Dictionary<int, ExternalItemStat>();
		string[] array = File.ReadAllLines(GetFilepath());
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (!array2[_rarityColumnIndex].ToString().Equals(string.Empty))
			{
				new Dictionary<Enums.ItemStatType, float>();
				ExternalItemStat externalItemStat = new ExternalItemStat();
				externalItemStat.ItemId = int.Parse(array2[_idColumnIndex]);
				externalItemStat.Price = int.Parse(array2[_priceColumnIndex]);
				externalItemStat.Rarity = GetRarityFromString(array2[_rarityColumnIndex]);
				dictionary.Add(externalItemStat.ItemId, externalItemStat);
			}
		}
		return dictionary;
	}

	internal static string GetFilepath()
	{
		return Path.Combine(Application.persistentDataPath, "The Final Mountain - Items.tsv");
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
