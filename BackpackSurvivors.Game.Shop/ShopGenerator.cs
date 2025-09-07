using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shop;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Shop;

public class ShopGenerator
{
	private ShopController _shopController;

	private Enums.PlaceableTag _characterTagAffinities => SingletonController<GameController>.Instance.Player.BaseCharacter.CombinedPlaceableTags;

	private float _affinityTagChance => GetAffinityTagChanceFromPlayer();

	private float GetAffinityTagChanceFromPlayer()
	{
		return SingletonController<GameController>.Instance.Player.BaseCharacter.ChanceForPlaceableWithAffinityTag;
	}

	internal ShopGenerator Init(ShopController shopController)
	{
		_shopController = shopController;
		return this;
	}

	private bool UniqueIsAlreadyFound(BaseItemSO baseItemSO)
	{
		return baseItemSO.ItemType switch
		{
			Enums.PlaceableType.Weapon => GetFoundUniqueWeapons().Contains(baseItemSO.Id), 
			Enums.PlaceableType.Bag => GetFoundUniqueBags().Contains(baseItemSO.Id), 
			Enums.PlaceableType.Item => GetFoundUniqueItems().Contains(baseItemSO.Id), 
			_ => false, 
		};
	}

	private List<int> GetUniquePlaceableIdsCurrentlyOnOffer(Enums.PlaceableType placeableType)
	{
		if (_shopController != null)
		{
			return (from x in _shopController.ShopOfferSlots
				where x.DraggableForSale.BaseItemSO.ItemType == placeableType && x.DraggableForSale.BaseItemSO.ItemRarity == Enums.PlaceableRarity.Unique
				select x.DraggableForSale.BaseItemSO.Id).ToList();
		}
		return new List<int>();
	}

	private List<int> GetFoundUniqueItems()
	{
		List<int> list = new List<int>();
		list.AddRange((from x in SingletonController<BackpackController>.Instance.GetItemsFromBackpack()
			where x.ItemSO.ItemRarity == Enums.PlaceableRarity.Unique
			select x.Id).ToList());
		list.AddRange((from x in SingletonController<BackpackController>.Instance.GetItemsFromStorage()
			where x.ItemSO.ItemRarity == Enums.PlaceableRarity.Unique
			select x.Id).ToList());
		list.AddRange(GetUniquePlaceableIdsCurrentlyOnOffer(Enums.PlaceableType.Item));
		return list;
	}

	private List<int> GetFoundUniqueWeapons()
	{
		List<int> list = new List<int>();
		list.AddRange((from x in SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack()
			where x.BaseWeaponSO.ItemRarity == Enums.PlaceableRarity.Unique
			select x.Id).ToList());
		list.AddRange((from x in SingletonController<BackpackController>.Instance.GetWeaponsFromStorage()
			where x.BaseWeaponSO.ItemRarity == Enums.PlaceableRarity.Unique
			select x.Id).ToList());
		list.AddRange(GetUniquePlaceableIdsCurrentlyOnOffer(Enums.PlaceableType.Weapon));
		return list;
	}

	private List<int> GetFoundUniqueBags()
	{
		List<int> list = new List<int>();
		list.AddRange((from x in SingletonController<BackpackController>.Instance.GetBagsFromBackpack()
			where x.BaseItemSO.ItemRarity == Enums.PlaceableRarity.Unique
			select x.Id).ToList());
		list.AddRange((from x in SingletonController<BackpackController>.Instance.GetBagsFromStorage()
			where x.BaseItemSO.ItemRarity == Enums.PlaceableRarity.Unique
			select x.Id).ToList());
		list.AddRange(GetUniquePlaceableIdsCurrentlyOnOffer(Enums.PlaceableType.Bag));
		return list;
	}

	public BagSO GetRandomBagByRarity(Enums.PlaceableRarity rarity)
	{
		List<BagSO> bagsOfCorrectRarity = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableBags.Where((BagSO b) => b.AvailableInShop && b.ItemRarity == rarity && !UniqueIsAlreadyFound(b)).ToList();
		bagsOfCorrectRarity = ApplyAffinitiesFromCharacterAffinity(bagsOfCorrectRarity);
		bagsOfCorrectRarity = ApplyAffinitiesFromBackpackAffinity(bagsOfCorrectRarity);
		bagsOfCorrectRarity = bagsOfCorrectRarity.Where((BagSO b) => !SingletonController<BanishedShopOfferController>.Instance.BanishedBags.Contains(b)).ToList();
		if (bagsOfCorrectRarity.Count == 0 && rarity > Enums.PlaceableRarity.Common)
		{
			rarity--;
			return GetRandomBagByRarity(rarity);
		}
		if (bagsOfCorrectRarity.Any())
		{
			return GetRandomFromList(bagsOfCorrectRarity);
		}
		List<BagSO> sourceList = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableBags.Where((BagSO b) => b.AvailableInShop && !UniqueIsAlreadyFound(b)).ToList();
		return GetRandomFromList(sourceList);
	}

	public ItemSO GetRandomItemByRarity(Enums.PlaceableRarity rarity)
	{
		List<ItemSO> itemsOfCorrectRarity = (from i in GameDatabaseHelper.GetItems()
			where i.AvailableInShop && i.ItemRarity == rarity && !UniqueIsAlreadyFound(i)
			select i).ToList();
		itemsOfCorrectRarity = ApplyAffinities(itemsOfCorrectRarity);
		itemsOfCorrectRarity = ApplyAffinitiesFromBackpackAffinity(itemsOfCorrectRarity);
		itemsOfCorrectRarity = itemsOfCorrectRarity.Where((ItemSO i) => !SingletonController<BanishedShopOfferController>.Instance.BanishedItems.Contains(i)).ToList();
		if (itemsOfCorrectRarity.Count == 0 && rarity > Enums.PlaceableRarity.Common)
		{
			rarity--;
			return GetRandomItemByRarity(rarity);
		}
		if (itemsOfCorrectRarity.Any())
		{
			return GetRandomFromList(itemsOfCorrectRarity);
		}
		List<ItemSO> list = (from b in GameDatabaseHelper.GetItems()
			where b.AvailableInShop && b.ItemRarity == rarity && !UniqueIsAlreadyFound(b)
			select b).ToList();
		if (list.Any())
		{
			return GetRandomFromList(list);
		}
		List<ItemSO> sourceList = (from b in GameDatabaseHelper.GetItems()
			where b.AvailableInShop
			select b).ToList();
		return GetRandomFromList(sourceList);
	}

	public WeaponSO GetRandomWeaponByRarity(Enums.PlaceableRarity rarity)
	{
		List<WeaponSO> weaponsOfCorrectRarity = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableWeapons.Where((WeaponSO b) => b.AvailableInShop && b.ItemRarity == rarity).ToList();
		weaponsOfCorrectRarity = ApplyAffinities(weaponsOfCorrectRarity);
		weaponsOfCorrectRarity = ApplyAffinitiesFromBackpackAffinity(weaponsOfCorrectRarity);
		weaponsOfCorrectRarity = weaponsOfCorrectRarity.Where((WeaponSO w) => !SingletonController<BanishedShopOfferController>.Instance.BanishedWeapons.Contains(w)).ToList();
		if (weaponsOfCorrectRarity.Count == 0 && rarity > Enums.PlaceableRarity.Common)
		{
			rarity--;
			return GetRandomWeaponByRarity(rarity);
		}
		if (weaponsOfCorrectRarity.Any())
		{
			return GetRandomFromList(weaponsOfCorrectRarity);
		}
		List<WeaponSO> sourceList = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableWeapons.Where((WeaponSO b) => b.AvailableInShop && !UniqueIsAlreadyFound(b)).ToList();
		return GetRandomFromList(sourceList);
	}

	private List<ItemSO> ApplyAffinitiesFromBackpackAffinity(List<ItemSO> itemsOfCorrectRarity)
	{
		List<ItemSO> list = new List<ItemSO>();
		foreach (KeyValuePair<Enums.PlaceableTag, float> backpackAndStorageAffinityTagsAndWeight in SingletonController<BackpackController>.Instance.GetBackpackAndStorageAffinityTagsAndWeights())
		{
			if (!RandomHelper.GetRollSuccess(backpackAndStorageAffinityTagsAndWeight.Value))
			{
				continue;
			}
			foreach (ItemSO item in itemsOfCorrectRarity)
			{
				if (ItemHasTagAffinity(item, backpackAndStorageAffinityTagsAndWeight.Key))
				{
					list.Add(item);
				}
			}
		}
		if (list.Any())
		{
			return list;
		}
		return itemsOfCorrectRarity;
	}

	private List<WeaponSO> ApplyAffinitiesFromBackpackAffinity(List<WeaponSO> weaponsOfCorrectRarity)
	{
		List<WeaponSO> list = new List<WeaponSO>();
		foreach (KeyValuePair<Enums.PlaceableTag, float> backpackAndStorageAffinityTagsAndWeight in SingletonController<BackpackController>.Instance.GetBackpackAndStorageAffinityTagsAndWeights())
		{
			if (!RandomHelper.GetRollSuccess(backpackAndStorageAffinityTagsAndWeight.Value))
			{
				continue;
			}
			foreach (WeaponSO item in weaponsOfCorrectRarity)
			{
				if (WeaponHasTagAffinity(item, backpackAndStorageAffinityTagsAndWeight.Key))
				{
					list.Add(item);
				}
			}
		}
		if (list.Any())
		{
			return list;
		}
		return weaponsOfCorrectRarity;
	}

	private List<BagSO> ApplyAffinitiesFromBackpackAffinity(List<BagSO> bagsOfCorrectRarity)
	{
		return bagsOfCorrectRarity;
	}

	private List<WeaponSO> ApplyAffinities(List<WeaponSO> weaponsOfCorrectRarity)
	{
		if (!(UnityEngine.Random.Range(0f, 1f) < _affinityTagChance))
		{
			return weaponsOfCorrectRarity;
		}
		List<WeaponSO> list = weaponsOfCorrectRarity.Where((WeaponSO w) => WeaponHasTagAffinity(w, _characterTagAffinities)).ToList();
		if (list.Count() == 0)
		{
			return weaponsOfCorrectRarity;
		}
		return list;
	}

	private bool WeaponHasTagAffinity(WeaponSO weapon, Enums.PlaceableTag characterTagAffinities)
	{
		Enums.PlaceableTag combinedPlaceableTags = EnumHelper.GetCombinedPlaceableTags(weapon);
		foreach (Enum uniqueFlag in characterTagAffinities.GetUniqueFlags())
		{
			if (combinedPlaceableTags.HasFlag(uniqueFlag))
			{
				return true;
			}
		}
		return false;
	}

	private List<ItemSO> ApplyAffinities(List<ItemSO> itemsOfCorrectRarity)
	{
		if (!(UnityEngine.Random.Range(0f, 1f) < _affinityTagChance))
		{
			return itemsOfCorrectRarity;
		}
		List<ItemSO> list = itemsOfCorrectRarity.Where((ItemSO i) => ItemHasTagAffinity(i, _characterTagAffinities)).ToList();
		if (list.Count() == 0)
		{
			return itemsOfCorrectRarity;
		}
		return list;
	}

	private bool ItemHasTagAffinity(ItemSO item, Enums.PlaceableTag characterTagAffinities)
	{
		Enums.PlaceableTag combinedPlaceableTags = EnumHelper.GetCombinedPlaceableTags(item);
		foreach (Enum uniqueFlag in characterTagAffinities.GetUniqueFlags())
		{
			if (combinedPlaceableTags.HasFlag(uniqueFlag))
			{
				return true;
			}
		}
		return false;
	}

	private List<BagSO> ApplyAffinitiesFromCharacterAffinity(List<BagSO> bagsOfCorrectRarity)
	{
		if (!(UnityEngine.Random.Range(0f, 1f) < _affinityTagChance))
		{
			return bagsOfCorrectRarity;
		}
		List<BagSO> list = bagsOfCorrectRarity.Where((BagSO b) => BagHasTagAffinity(b, _characterTagAffinities)).ToList();
		if (list.Count() == 0)
		{
			return bagsOfCorrectRarity;
		}
		return list;
	}

	private bool BagHasTagAffinity(BagSO bag, Enums.PlaceableTag characterTagAffinities)
	{
		return false;
	}

	private T GetRandomFromList<T>(List<T> sourceList)
	{
		int count = sourceList.Count;
		if (count > 0)
		{
			int index = UnityEngine.Random.Range(0, count);
			return sourceList[index];
		}
		Debug.LogError($"{typeof(T)} GetRandomFromList had no items in SourceList");
		return default(T);
	}

	[Command("shop.logContent", Platform.AllPlatforms, MonoTargetType.Single)]
	public static void LogShopContents()
	{
		LogWeaponsInShop();
		LogItemsInShop();
		LogBagsInShop();
	}

	private static void LogItemsInShop()
	{
		Debug.Log("-= Items in shop: =-");
		foreach (ItemSO item in from i in GameDatabaseHelper.GetItems()
			where i.AvailableInShop
			orderby i.Name
			select i)
		{
			Debug.Log("- " + item.Name);
		}
		Debug.Log("");
	}

	private static void LogBagsInShop()
	{
		Debug.Log("-= Bags in shop: =-");
		foreach (BagSO item in from b in SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableBags
			where b.AvailableInShop
			orderby b.Name
			select b)
		{
			Debug.Log("- " + item.Name);
		}
		Debug.Log("");
	}

	public static void LogWeaponsInShop()
	{
		Debug.Log("-= Weapons in shop: =-");
		foreach (WeaponSO item in from w in SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableWeapons
			where w.AvailableInShop
			orderby w.Name
			select w)
		{
			Debug.Log("- " + item.Name);
		}
		Debug.Log("");
	}
}
