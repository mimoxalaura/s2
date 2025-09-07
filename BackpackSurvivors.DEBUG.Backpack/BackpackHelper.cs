using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.DEBUG.Backpack;

public class BackpackHelper : MonoBehaviour
{
	[Command("backpack.add.bag", Platform.AllPlatforms, MonoTargetType.Single)]
	public void AddBagToBackpack(int bagId)
	{
		BagSO bagSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableBags.FirstOrDefault((BagSO x) => x.Id == bagId);
		List<int> invalidCellIds = new List<int>();
		BagInstance bag = new BagInstance(bagSO);
		PlaceableInfo placeable = new PlaceableInfo(bagSO.ItemSize.SizeInfo.ToList());
		for (int num = 0; num < 144; num++)
		{
			HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(num, isHoveredSlotOnRight: false, isHoveredSlotOnBottom: false, Enums.Backpack.GridType.Backpack);
			List<int> slotIdsToPlaceItemIn = SingletonController<BackpackController>.Instance.BackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
			if (SingletonController<BackpackController>.Instance.BackpackStorage.CanPlaceBag(bag, slotIdsToPlaceItemIn, out invalidCellIds))
			{
				bool flag = SingletonController<BackpackController>.Instance.BackpackStorage.PlaceBag(bag, slotIdsToPlaceItemIn);
				Debug.Log($"Placed bag: {flag}");
				break;
			}
		}
	}

	[Command("backpack.add.weapon", Platform.AllPlatforms, MonoTargetType.Single)]
	public void AddWeaponToBackpack(int weaponId)
	{
		WeaponSO weaponSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableWeapons.FirstOrDefault((WeaponSO x) => x.Id == weaponId);
		if (weaponSO == null)
		{
			Debug.Log($"Cannot find weapon with Id {weaponId}");
			return;
		}
		List<int> invalidCellIds = new List<int>();
		WeaponInstance weapon = new WeaponInstance(weaponSO);
		PlaceableInfo placeable = new PlaceableInfo(weaponSO.ItemSize.SizeInfo.ToList());
		for (int num = 0; num < 144; num++)
		{
			HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(num, isHoveredSlotOnRight: false, isHoveredSlotOnBottom: false, Enums.Backpack.GridType.Backpack);
			List<int> slotIdsToPlaceItemIn = SingletonController<BackpackController>.Instance.BackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
			if (SingletonController<BackpackController>.Instance.BackpackStorage.CanPlaceWeapon(weapon, slotIdsToPlaceItemIn, out invalidCellIds))
			{
				bool flag = SingletonController<BackpackController>.Instance.BackpackStorage.PlaceWeapon(weapon, slotIdsToPlaceItemIn, new List<int>());
				Debug.Log($"Placed weapon: {flag}");
				break;
			}
		}
	}
}
