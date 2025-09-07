using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Shop;

public class ShopRandomizer
{
	public Enums.PlaceableRarity GetRandomRarity(Dictionary<Enums.PlaceableRarity, float> rarityChances, float playerLuck)
	{
		Enums.PlaceableRarity placeableRarity = Enums.PlaceableRarity.Unique;
		foreach (Enums.PlaceableRarity item in GetPlaceableRaritiesByDescendingRarity())
		{
			if (rarityChances[item] > 0f)
			{
				placeableRarity = item;
			}
		}
		foreach (Enums.PlaceableRarity item2 in GetPlaceableRaritiesByDescendingRarity())
		{
			if (item2 > placeableRarity)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				float num2 = MathF.Min(rarityChances[item2] * playerLuck, 0.5f);
				if (num < num2)
				{
					placeableRarity = item2;
				}
			}
		}
		return placeableRarity;
	}

	private IEnumerable<Enums.PlaceableRarity> GetPlaceableRaritiesByDescendingRarity()
	{
		return from Enums.PlaceableRarity r in Enum.GetValues(typeof(Enums.PlaceableRarity))
			orderby (int)r descending
			select r;
	}

	public Enums.PlaceableType GetRandomPlaceableType(float bagChance, float weaponChance, float itemChance)
	{
		bool num = UnityEngine.Random.Range(0f, 1f) < bagChance;
		bool flag = UnityEngine.Random.Range(0f, 1f) < weaponChance;
		bool flag2 = UnityEngine.Random.Range(0f, 1f) < itemChance;
		List<Enums.PlaceableType> list = new List<Enums.PlaceableType>();
		if (num)
		{
			list.Add(Enums.PlaceableType.Bag);
		}
		if (flag)
		{
			list.Add(Enums.PlaceableType.Weapon);
		}
		if (flag2)
		{
			list.Add(Enums.PlaceableType.Item);
		}
		Array values = Enum.GetValues(typeof(Enums.PlaceableType));
		if (list.Count == 0)
		{
			return ReturnRandomPlaceableType(values);
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private Enums.PlaceableType ReturnRandomPlaceableType(Array allTypes)
	{
		List<Enums.PlaceableType> list = new List<Enums.PlaceableType>();
		foreach (object allType in allTypes)
		{
			list.Add((Enums.PlaceableType)allType);
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}
}
