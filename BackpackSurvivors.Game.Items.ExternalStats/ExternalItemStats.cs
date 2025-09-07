using System.Collections.Generic;
using System.IO;
using BackpackSurvivors.ScriptableObjects.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.ExternalStats;

internal static class ExternalItemStats
{
	private static bool _externalStatsLoaded;

	private static Dictionary<int, ExternalItemStat> _itemStats = new Dictionary<int, ExternalItemStat>();

	internal static void ReloadStats()
	{
		_externalStatsLoaded = false;
	}

	internal static bool ExternalItemStatsAvailable()
	{
		return File.Exists(ExternalItemStatLoader.GetFilepath());
	}

	internal static ItemSO GetItemSO(int itemId, ItemSO originalItemSO)
	{
		if (!_externalStatsLoaded)
		{
			LoadExternalItemStats();
		}
		if (!_itemStats.ContainsKey(itemId))
		{
			return originalItemSO;
		}
		return GetExternalItemSO(itemId, originalItemSO);
	}

	private static ItemSO GetExternalItemSO(int itemId, ItemSO originalItemSO)
	{
		ExternalItemStat externalItemStat = _itemStats[itemId];
		ItemSO itemSO = Object.Instantiate(originalItemSO);
		itemSO.ItemRarity = externalItemStat.Rarity;
		itemSO.BuyingPrice = externalItemStat.Price;
		itemSO.Name += "(ext)";
		return itemSO;
	}

	private static void LoadExternalItemStats()
	{
		_itemStats = ExternalItemStatLoader.GetExternalItemStats();
	}
}
