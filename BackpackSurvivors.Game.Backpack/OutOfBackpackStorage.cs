using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack.Interfaces;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Backpack;

public class OutOfBackpackStorage : IBackpackStorage
{
	private bool[] _gridCellStatuses;

	private Dictionary<int, Guid> _filledCellsWithGuid;

	private Dictionary<int, BagInstance> _bagsInGridCells = new Dictionary<int, BagInstance>();

	private Dictionary<int, WeaponInstance> _weaponsInGridCells = new Dictionary<int, WeaponInstance>();

	private Dictionary<int, ItemInstance> _itemsInGridCells = new Dictionary<int, ItemInstance>();

	private float _timeOfLastChange;

	public Enums.Backpack.GridType GridType { get; private set; }

	public List<ItemInstance> ItemsToMoveToStorage { get; private set; } = new List<ItemInstance>();

	public List<WeaponInstance> WeaponsToMoveToStorage { get; private set; } = new List<WeaponInstance>();

	public OutOfBackpackStorage(int gridWidth, int gridHeight, Enums.Backpack.GridType gridType)
	{
		_gridCellStatuses = new bool[gridWidth * gridHeight];
		_filledCellsWithGuid = new Dictionary<int, Guid>();
		GridType = gridType;
	}

	public void ClearStorage()
	{
		Array.Clear(_gridCellStatuses, 0, _gridCellStatuses.Length);
		_bagsInGridCells.Clear();
		_weaponsInGridCells.Clear();
		_itemsInGridCells.Clear();
		_filledCellsWithGuid.Clear();
	}

	public float GetTimeOfLastChange()
	{
		return _timeOfLastChange;
	}

	public bool CanStoreInCells(List<int> cellIdsToPlaceOn, out List<int> invalidCellIds)
	{
		bool num = cellIdsToPlaceOn.Any((int c) => c >= _gridCellStatuses.Count());
		bool flag = cellIdsToPlaceOn.Any((int c) => c < 0);
		if (num || flag)
		{
			invalidCellIds = new List<int>();
			return false;
		}
		invalidCellIds = cellIdsToPlaceOn.Where((int c) => _gridCellStatuses[c]).ToList();
		return invalidCellIds.Count == 0;
	}

	public bool StoreInCells(Guid guid, List<int> cellIdsToPlaceOn)
	{
		List<int> invalidCellIds = new List<int>();
		if (!CanStoreInCells(cellIdsToPlaceOn, out invalidCellIds))
		{
			return false;
		}
		foreach (int item in cellIdsToPlaceOn)
		{
			_filledCellsWithGuid.Add(item, guid);
			_gridCellStatuses[item] = true;
		}
		return true;
	}

	public void RemoveFromStorage(Guid guid)
	{
		foreach (int item in (from fc in _filledCellsWithGuid
			where fc.Value == guid
			select fc.Key).ToList())
		{
			_gridCellStatuses[item] = false;
			_filledCellsWithGuid.Remove(item);
		}
	}

	public bool CanMoveBag(BagInstance bagInstance)
	{
		return true;
	}

	public bool CanPlaceBag(BagInstance bag, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds)
	{
		bool num = cellIdsToPlaceOn.Any((int c) => c >= _gridCellStatuses.Count());
		bool flag = cellIdsToPlaceOn.Any((int c) => c < 0);
		bool flag2 = CanPlaceEntireItemInCells(cellIdsToPlaceOn, bag.TotalPlaceableSize);
		if (num || flag || !flag2)
		{
			invalidCellIds = new List<int>();
			return false;
		}
		invalidCellIds = cellIdsToPlaceOn.Where((int c) => _gridCellStatuses[c]).ToList();
		return invalidCellIds.Count == 0;
	}

	public bool CanPlaceItem(ItemInstance item, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds)
	{
		bool num = cellIdsToPlaceOn.Any((int c) => c >= _gridCellStatuses.Count());
		bool flag = cellIdsToPlaceOn.Any((int c) => c < 0);
		bool flag2 = CanPlaceEntireItemInCells(cellIdsToPlaceOn, item.TotalPlaceableSize);
		if (num || flag || !flag2)
		{
			invalidCellIds = new List<int>();
			return false;
		}
		invalidCellIds = cellIdsToPlaceOn.Where((int c) => _gridCellStatuses[c]).ToList();
		return invalidCellIds.Count == 0;
	}

	public bool CanPlaceWeapon(WeaponInstance weapon, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds)
	{
		bool num = cellIdsToPlaceOn.Any((int c) => c >= _gridCellStatuses.Count());
		bool flag = cellIdsToPlaceOn.Any((int c) => c < 0);
		bool flag2 = CanPlaceEntireItemInCells(cellIdsToPlaceOn, weapon.TotalPlaceableSize);
		if (num || flag || !flag2)
		{
			invalidCellIds = new List<int>();
			return false;
		}
		invalidCellIds = cellIdsToPlaceOn.Where((int c) => _gridCellStatuses[c]).ToList();
		return invalidCellIds.Count == 0;
	}

	public bool PlaceBag(BagInstance bag, List<int> cellIdsToPlaceOn)
	{
		List<int> invalidCellIds = new List<int>();
		if (!CanStoreInCells(cellIdsToPlaceOn, out invalidCellIds))
		{
			return false;
		}
		foreach (int item in cellIdsToPlaceOn)
		{
			_filledCellsWithGuid.Add(item, bag.Guid);
			_gridCellStatuses[item] = true;
			_bagsInGridCells.Add(item, bag);
		}
		_timeOfLastChange = Time.unscaledTime;
		return true;
	}

	public bool PlaceItem(ItemInstance item, List<int> cellIdsToPlaceOn, List<int> starredCellids)
	{
		List<int> invalidCellIds = new List<int>();
		if (!CanStoreInCells(cellIdsToPlaceOn, out invalidCellIds))
		{
			return false;
		}
		foreach (int item2 in cellIdsToPlaceOn)
		{
			_filledCellsWithGuid.Add(item2, item.Guid);
			_gridCellStatuses[item2] = true;
			_itemsInGridCells.Add(item2, item);
		}
		_timeOfLastChange = Time.unscaledTime;
		return true;
	}

	public bool PlaceWeapon(WeaponInstance weapon, List<int> cellIdsToPlaceOn, List<int> starredCellids)
	{
		List<int> invalidCellIds = new List<int>();
		if (!CanStoreInCells(cellIdsToPlaceOn, out invalidCellIds))
		{
			return false;
		}
		foreach (int item in cellIdsToPlaceOn)
		{
			_filledCellsWithGuid.Add(item, weapon.Guid);
			_gridCellStatuses[item] = true;
			_weaponsInGridCells.Add(item, weapon);
		}
		_timeOfLastChange = Time.unscaledTime;
		return true;
	}

	public void RemoveBag(BagInstance bag)
	{
		foreach (int item in (from fc in _filledCellsWithGuid
			where fc.Value == bag.Guid
			select fc.Key).ToList())
		{
			_gridCellStatuses[item] = false;
			_filledCellsWithGuid.Remove(item);
			_bagsInGridCells.Remove(item);
		}
		_timeOfLastChange = Time.unscaledTime;
	}

	public void RemoveItem(ItemInstance item)
	{
		foreach (int item2 in (from fc in _filledCellsWithGuid
			where fc.Value == item.Guid
			select fc.Key).ToList())
		{
			_gridCellStatuses[item2] = false;
			_filledCellsWithGuid.Remove(item2);
			_itemsInGridCells.Remove(item2);
		}
		_timeOfLastChange = Time.unscaledTime;
	}

	public void RemoveWeapon(WeaponInstance weapon)
	{
		foreach (int item in (from fc in _filledCellsWithGuid
			where fc.Value == weapon.Guid
			select fc.Key).ToList())
		{
			_gridCellStatuses[item] = false;
			_filledCellsWithGuid.Remove(item);
			_weaponsInGridCells.Remove(item);
		}
		_timeOfLastChange = Time.unscaledTime;
	}

	private bool CanPlaceEntireItemInCells(List<int> itemSlotIds, int totalPlaceableSize)
	{
		return itemSlotIds.Count == totalPlaceableSize;
	}

	public WeaponInstance GetWeaponFromSlot(int slotId)
	{
		return null;
	}

	public ItemInstance GetItemFromSlot(int slotId)
	{
		return null;
	}

	public bool SlotContainsOnlyBag(int slotId)
	{
		return false;
	}

	internal List<WeaponInstance> GetWeaponsInStorage()
	{
		return (from kvp in _weaponsInGridCells
			orderby kvp.Key
			select kvp.Value).Distinct().ToList();
	}

	internal List<ItemInstance> GetItemsInStorage()
	{
		return (from kvp in _itemsInGridCells
			orderby kvp.Key
			select kvp.Value).Distinct().ToList();
	}

	internal List<BagInstance> GetBagsInBackpack()
	{
		return (from kvp in _bagsInGridCells
			orderby kvp.Key
			select kvp.Value).Distinct().ToList();
	}
}
