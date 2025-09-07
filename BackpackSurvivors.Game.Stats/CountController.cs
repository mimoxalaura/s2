using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Stats;

public class CountController : ICountController
{
	private Dictionary<Enums.PlaceableType, int> _placeableTypeCounts = new Dictionary<Enums.PlaceableType, int>();

	private Dictionary<Enums.PlaceableTag, int> _placeableTagCounts = new Dictionary<Enums.PlaceableTag, int>();

	private Dictionary<Enums.PlaceableRarity, int> _placeableRarityCounts = new Dictionary<Enums.PlaceableRarity, int>();

	public CountController()
	{
		InitCounts();
	}

	public int GetPlaceableTypeCount(Enums.PlaceableType placeableType)
	{
		return _placeableTypeCounts[placeableType];
	}

	public int GetPlaceableRarityCount(Enums.PlaceableRarity placeableRarity)
	{
		return _placeableRarityCounts[placeableRarity];
	}

	public int GetPlaceableTagCount(Enums.PlaceableTag placeableTag)
	{
		int num = 0;
		foreach (Enum uniqueFlag in placeableTag.GetUniqueFlags())
		{
			num += _placeableTagCounts[(Enums.PlaceableTag)(object)uniqueFlag];
		}
		return num;
	}

	public void UpdateCounts()
	{
		ClearCounts();
		UpdateFromWeapons();
		UpdateFromItems();
	}

	private void UpdateFromItems()
	{
		List<ItemInstance> itemsFromBackpack = SingletonController<BackpackController>.Instance.GetItemsFromBackpack();
		IEnumerable<Enums.PlaceableTag> placeableTags = itemsFromBackpack.Select((ItemInstance i) => i.CombinedPlaceableTags);
		UpdatePlaceableTagCount(placeableTags);
		IEnumerable<Enums.PlaceableType> values = itemsFromBackpack.Select((ItemInstance i) => i.ItemSO.ItemType);
		UpdateDictionaryCounts(values, _placeableTypeCounts);
		IEnumerable<Enums.PlaceableRarity> values2 = itemsFromBackpack.Select((ItemInstance i) => i.ItemSO.ItemRarity);
		UpdateDictionaryCounts(values2, _placeableRarityCounts);
	}

	private void UpdateFromWeapons()
	{
		List<WeaponInstance> weaponsFromBackpack = SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack();
		IEnumerable<Enums.PlaceableTag> placeableTags = weaponsFromBackpack.Select((WeaponInstance w) => w.CombinedPlaceableTags);
		UpdatePlaceableTagCount(placeableTags);
		IEnumerable<Enums.PlaceableType> values = weaponsFromBackpack.Select((WeaponInstance w) => w.BaseItemSO.ItemType);
		UpdateDictionaryCounts(values, _placeableTypeCounts);
		IEnumerable<Enums.PlaceableRarity> values2 = weaponsFromBackpack.Select((WeaponInstance w) => w.BaseItemSO.ItemRarity);
		UpdateDictionaryCounts(values2, _placeableRarityCounts);
	}

	private void UpdateDictionaryCounts<T>(IEnumerable<T> values, Dictionary<T, int> dictionary)
	{
		foreach (T value in values)
		{
			dictionary[value]++;
		}
	}

	private void UpdatePlaceableTagCount(IEnumerable<Enums.PlaceableTag> placeableTags)
	{
		foreach (object value in Enum.GetValues(typeof(Enums.PlaceableTag)))
		{
			Enums.PlaceableTag tagToCheck = (Enums.PlaceableTag)value;
			if (tagToCheck != Enums.PlaceableTag.None)
			{
				int num = placeableTags.Count((Enums.PlaceableTag pt) => (pt & tagToCheck) == tagToCheck);
				_placeableTagCounts[tagToCheck] += num;
			}
		}
	}

	private void ClearCounts()
	{
		ClearDictionary(_placeableTypeCounts);
		ClearDictionary(_placeableTagCounts);
		ClearDictionary(_placeableRarityCounts);
	}

	private void InitCounts()
	{
		InitDictionary(_placeableTypeCounts);
		InitDictionary(_placeableTagCounts);
		InitDictionary(_placeableRarityCounts);
	}

	private void InitDictionary<T>(Dictionary<T, int> dictionary) where T : Enum
	{
		foreach (object value in Enum.GetValues(typeof(T)))
		{
			dictionary.Add((T)value, 0);
		}
	}

	private void ClearDictionary<T>(Dictionary<T, int> dictionary) where T : Enum
	{
		foreach (object value in Enum.GetValues(typeof(T)))
		{
			dictionary[(T)value] = 0;
		}
	}
}
