using System.Collections.Generic;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Backpack.Interfaces;

public interface IBackpackStorage
{
	Enums.Backpack.GridType GridType { get; }

	List<ItemInstance> ItemsToMoveToStorage { get; }

	List<WeaponInstance> WeaponsToMoveToStorage { get; }

	bool CanMoveBag(BagInstance bag);

	bool CanPlaceBag(BagInstance bag, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds);

	bool CanPlaceItem(ItemInstance item, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds);

	bool CanPlaceWeapon(WeaponInstance weapon, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds);

	bool PlaceBag(BagInstance bag, List<int> cellIdsToPlaceOn);

	bool PlaceItem(ItemInstance item, List<int> cellIdsToPlaceOn, List<int> starredCellids);

	bool PlaceWeapon(WeaponInstance weapon, List<int> cellIdsToPlaceOn, List<int> starredCellids);

	void RemoveBag(BagInstance bag);

	void RemoveItem(ItemInstance item);

	void RemoveWeapon(WeaponInstance weapon);

	void ClearStorage();

	WeaponInstance GetWeaponFromSlot(int slotId);

	ItemInstance GetItemFromSlot(int slotId);

	bool SlotContainsOnlyBag(int slotId);

	float GetTimeOfLastChange();
}
