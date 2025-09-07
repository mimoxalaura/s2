using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack.Interfaces;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Backpack;

public class BackpackStorage : IBackpackStorage
{
	private int _gridWidth;

	private int _gridHeight;

	private Enums.Backpack.GridCellStatus[] _gridCells;

	private Dictionary<int, BagInstance> _bagsInGridCells;

	private Dictionary<int, WeaponInstance> _weaponsInGridCells;

	private Dictionary<int, ItemInstance> _itemsInGridCells;

	private List<KeyValuePair<int, ItemInstance>> _starredCellsByItems;

	private List<KeyValuePair<int, WeaponInstance>> _starredCellsByWeapons;

	private float _timeOfLastChange;

	public Enums.Backpack.GridType GridType { get; private set; }

	public List<ItemInstance> ItemsToMoveToStorage { get; private set; } = new List<ItemInstance>();

	public List<WeaponInstance> WeaponsToMoveToStorage { get; private set; } = new List<WeaponInstance>();

	public BackpackStorage(int gridWidth, int gridHeight, Enums.Backpack.GridType gridType)
	{
		_gridWidth = gridWidth;
		_gridHeight = gridHeight;
		_gridCells = new Enums.Backpack.GridCellStatus[gridWidth * gridHeight];
		_bagsInGridCells = new Dictionary<int, BagInstance>();
		_weaponsInGridCells = new Dictionary<int, WeaponInstance>();
		_itemsInGridCells = new Dictionary<int, ItemInstance>();
		_starredCellsByItems = new List<KeyValuePair<int, ItemInstance>>();
		_starredCellsByWeapons = new List<KeyValuePair<int, WeaponInstance>>();
		GridType = gridType;
	}

	public void ClearStorage()
	{
		Array.Clear(_gridCells, 0, _gridCells.Length);
		_bagsInGridCells.Clear();
		_weaponsInGridCells.Clear();
		_itemsInGridCells.Clear();
		_starredCellsByItems.Clear();
		_starredCellsByWeapons.Clear();
	}

	public float GetTimeOfLastChange()
	{
		return _timeOfLastChange;
	}

	public void UpdateTimeOfLastChange()
	{
		_timeOfLastChange = Time.unscaledTime;
	}

	public void ClearMoveToStorage()
	{
		ItemsToMoveToStorage.Clear();
		WeaponsToMoveToStorage.Clear();
	}

	public BagInstance GetBagFromSlot(int slotId)
	{
		if (!_bagsInGridCells.ContainsKey(slotId))
		{
			return null;
		}
		return _bagsInGridCells[slotId];
	}

	public WeaponInstance GetWeaponFromSlot(int slotId)
	{
		if (!_weaponsInGridCells.ContainsKey(slotId))
		{
			return null;
		}
		return _weaponsInGridCells[slotId];
	}

	public ItemInstance GetItemFromSlot(int slotId)
	{
		if (!_itemsInGridCells.ContainsKey(slotId))
		{
			return null;
		}
		return _itemsInGridCells[slotId];
	}

	public BackpackStorageSaveState GetSaveState()
	{
		BackpackStorageSaveState backpackStorageSaveState = new BackpackStorageSaveState();
		backpackStorageSaveState.SetState(_gridCells, _bagsInGridCells, _weaponsInGridCells, _itemsInGridCells, _starredCellsByItems, _starredCellsByWeapons);
		return backpackStorageSaveState;
	}

	public List<WeaponInstance> GetWeaponsInBackpack()
	{
		return (from kvp in _weaponsInGridCells
			orderby kvp.Key
			select kvp.Value).Distinct().ToList();
	}

	public List<ItemInstance> GetItemsInBackpack()
	{
		return (from kvp in _itemsInGridCells
			orderby kvp.Key
			select kvp.Value).Distinct().ToList();
	}

	public List<BagInstance> GetBagsInBackpack()
	{
		return (from kvp in _bagsInGridCells
			orderby kvp.Key
			select kvp.Value).Distinct().ToList();
	}

	public List<WeaponInstance> GetWeaponsStarredByItem(ItemInstance item)
	{
		IEnumerable<int> starredCellsByIems = (from x in _starredCellsByItems
			where x.Value == item
			select x.Key).Distinct();
		return (from x in _weaponsInGridCells
			where starredCellsByIems.Contains(x.Key)
			select x.Value).ToList();
	}

	public List<WeaponInstance> GetWeaponsStarredByWeapon(WeaponInstance item)
	{
		IEnumerable<int> starredCellsByIems = (from x in _starredCellsByWeapons
			where x.Value == item
			select x.Key).Distinct();
		return (from x in _weaponsInGridCells
			where starredCellsByIems.Contains(x.Key)
			select x.Value).ToList();
	}

	public List<ItemInstance> GetItemsStarringWeapon(WeaponInstance weapon)
	{
		List<int> weaponCells = (from kvp in _weaponsInGridCells
			where kvp.Value.Equals(weapon)
			select kvp.Key).ToList();
		return (from isw in (from kvp in _starredCellsByItems
				where weaponCells.Contains(kvp.Key)
				select kvp.Value).Distinct()
			orderby _itemsInGridCells.Where((KeyValuePair<int, ItemInstance> iig) => iig.Value == isw).Min((KeyValuePair<int, ItemInstance> iig) => iig.Key)
			select isw).ToList();
	}

	public List<WeaponInstance> GetWeaponsStarringWeapon(WeaponInstance weapon)
	{
		List<int> weaponCells = (from kvp in _weaponsInGridCells
			where kvp.Value.Equals(weapon)
			select kvp.Key).ToList();
		return (from wsw in (from kvp in _starredCellsByWeapons
				where weaponCells.Contains(kvp.Key)
				select kvp.Value).Distinct()
			orderby _weaponsInGridCells.Where((KeyValuePair<int, WeaponInstance> wig) => wig.Value == wsw).Min((KeyValuePair<int, WeaponInstance> wig) => wig.Key)
			select wsw).ToList();
	}

	public Dictionary<WeaponInstance, List<BagInstance>> GetWeaponsAndContainingBags(List<WeaponInstance> weaponsInBackpack)
	{
		Dictionary<WeaponInstance, List<BagInstance>> dictionary = new Dictionary<WeaponInstance, List<BagInstance>>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			List<BagInstance> bagsWeaponIsIn = GetBagsWeaponIsIn(item);
			dictionary.Add(item, bagsWeaponIsIn);
		}
		return dictionary;
	}

	public List<int> LockCells(List<int> lockedRowIds, List<int> lockedColumnIds)
	{
		List<int> list = new List<int>();
		foreach (int lockedRowId in lockedRowIds)
		{
			for (int i = 0; i < _gridWidth; i++)
			{
				int num = lockedRowId * _gridWidth + i;
				_gridCells[num] = Enums.Backpack.GridCellStatus.Locked;
				list.Add(num);
			}
		}
		foreach (int lockedColumnId in lockedColumnIds)
		{
			for (int j = 0; j < _gridHeight; j++)
			{
				int num2 = j * _gridWidth + lockedColumnId % _gridWidth;
				_gridCells[num2] = Enums.Backpack.GridCellStatus.Locked;
				list.Add(num2);
			}
		}
		return list;
	}

	private List<BagInstance> GetBagsWeaponIsIn(WeaponInstance weapon)
	{
		List<int> cellsWeaponIsIn = GetCellsPlaceableIsIn(weapon);
		return (from kvp in _bagsInGridCells
			where cellsWeaponIsIn.Contains(kvp.Key)
			orderby kvp.Key
			select kvp.Value).Distinct().ToList();
	}

	public List<int> GetCellsPlaceableIsIn(BaseItemInstance item)
	{
		if (item.BaseItemSO.ItemType == Enums.PlaceableType.Item)
		{
			return (from kvp in _itemsInGridCells
				where kvp.Value == item
				select kvp.Key).ToList();
		}
		if (item.BaseItemSO.ItemType == Enums.PlaceableType.Weapon)
		{
			return (from kvp in _weaponsInGridCells
				where kvp.Value == item
				select kvp.Key).ToList();
		}
		if (item.BaseItemSO.ItemType == Enums.PlaceableType.Bag)
		{
			return (from kvp in _bagsInGridCells
				where kvp.Value == item
				select kvp.Key).ToList();
		}
		return new List<int>();
	}

	public bool CanPlace(BaseItemInstance item, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds)
	{
		List<int> list = cellIdsToPlaceOn.Where((int c) => c >= _gridCells.Count() || c < 0).ToList();
		bool flag = CanPlaceEntireItemInCells(cellIdsToPlaceOn, item.TotalPlaceableSize);
		if (list.Any() || !flag)
		{
			invalidCellIds = new List<int>();
			invalidCellIds.AddRange(list);
			return false;
		}
		invalidCellIds = GetInvalidCellIds(cellIdsToPlaceOn);
		return invalidCellIds.Count == 0;
	}

	public bool CanPlaceBag(BagInstance bag, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds)
	{
		bool num = cellIdsToPlaceOn.Any((int c) => c >= _gridCells.Count());
		bool flag = cellIdsToPlaceOn.Any((int c) => c < 0);
		bool flag2 = CanPlaceEntireItemInCells(cellIdsToPlaceOn, bag.TotalPlaceableSize);
		if (num || flag || !flag2)
		{
			invalidCellIds = new List<int>();
			return false;
		}
		invalidCellIds = cellIdsToPlaceOn.Where((int c) => _gridCells[c] != Enums.Backpack.GridCellStatus.Empty).ToList();
		return invalidCellIds.Count == 0;
	}

	public bool CanPlaceItem(ItemInstance item, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds)
	{
		bool num = cellIdsToPlaceOn.Any((int c) => c >= _gridCells.Count());
		bool flag = cellIdsToPlaceOn.Any((int c) => c < 0);
		bool flag2 = CanPlaceEntireItemInCells(cellIdsToPlaceOn, item.TotalPlaceableSize);
		if (num || flag || !flag2)
		{
			invalidCellIds = new List<int>();
			return false;
		}
		invalidCellIds = GetInvalidCellIds(cellIdsToPlaceOn);
		return invalidCellIds.Count == 0;
	}

	public bool CanPlaceWeapon(WeaponInstance weapon, List<int> cellIdsToPlaceOn, out List<int> invalidCellIds)
	{
		bool num = cellIdsToPlaceOn.Any((int c) => c >= _gridCells.Count());
		bool flag = cellIdsToPlaceOn.Any((int c) => c < 0);
		bool flag2 = CanPlaceEntireItemInCells(cellIdsToPlaceOn, weapon.TotalPlaceableSize);
		if (num || flag || !flag2)
		{
			invalidCellIds = new List<int>();
			return false;
		}
		invalidCellIds = GetInvalidCellIds(cellIdsToPlaceOn);
		return invalidCellIds.Count == 0;
	}

	private List<int> GetInvalidCellIds(List<int> cellIdsToPlaceOn)
	{
		List<int> list = cellIdsToPlaceOn.Where((int c) => _gridCells[c] != Enums.Backpack.GridCellStatus.ContainsBag).ToList();
		if (CanReplaceExistingPlaceable(list))
		{
			list.Clear();
		}
		return list;
	}

	private bool CanReplaceExistingPlaceable(List<int> cellIds)
	{
		if (cellIds.Any((int c) => _gridCells[c] == Enums.Backpack.GridCellStatus.Empty || _gridCells[c] == Enums.Backpack.GridCellStatus.Locked))
		{
			return false;
		}
		int num = GetItemInstancesInCells(cellIds).Count();
		int num2 = GetWeaponInstancesInCells(cellIds).Count();
		return num + num2 <= 1;
	}

	private IEnumerable<ItemInstance> GetItemInstancesInCells(List<int> cellIds)
	{
		return (from iig in _itemsInGridCells
			where cellIds.Contains(iig.Key)
			select iig.Value).Distinct().ToList();
	}

	private IEnumerable<WeaponInstance> GetWeaponInstancesInCells(List<int> cellIds)
	{
		return (from wig in _weaponsInGridCells
			where cellIds.Contains(wig.Key)
			select wig.Value).Distinct().ToList();
	}

	public void GetAdjacentPlaceables(BaseItemInstance baseItemInstance, out List<BaseItemInstance> adjacentBaseItemInstances)
	{
		List<int> collection = (from wig in _weaponsInGridCells
			where wig.Value.Equals(baseItemInstance)
			select wig.Key).ToList();
		List<int> collection2 = (from wig in _itemsInGridCells
			where wig.Value.Equals(baseItemInstance)
			select wig.Key).ToList();
		List<int> collection3 = (from wig in _bagsInGridCells
			where wig.Value.Equals(baseItemInstance)
			select wig.Key).ToList();
		List<int> list = new List<int>();
		list.AddRange(collection);
		list.AddRange(collection2);
		list.AddRange(collection3);
		List<int> adjacentCells = GetAdjacentCells(list);
		GetAdjacentPlaceablesByCells(adjacentCells, out adjacentBaseItemInstances);
	}

	private void GetAdjacentPlaceablesByCells(List<int> adjacentCells, out List<BaseItemInstance> adjacentBaseItemInstances)
	{
		adjacentBaseItemInstances = new List<BaseItemInstance>();
		adjacentBaseItemInstances.AddRange(GetWeaponInstancesInCells(adjacentCells));
		adjacentBaseItemInstances.AddRange(GetItemInstancesInCells(adjacentCells));
	}

	private List<int> GetAdjacentCells(List<int> cellIds)
	{
		List<int> list = new List<int>();
		int gridWidth = _gridWidth;
		int num = _gridHeight * _gridWidth;
		foreach (int cellId in cellIds)
		{
			bool num2 = cellId % gridWidth != 0;
			bool flag = cellId % gridWidth != 11;
			bool flag2 = cellId >= gridWidth;
			bool flag3 = cellId < num - 12;
			if (num2 && !cellIds.Contains(cellId - 1))
			{
				list.Add(cellId - 1);
			}
			if (flag && !cellIds.Contains(cellId + 1))
			{
				list.Add(cellId + 1);
			}
			if (flag2 && !cellIds.Contains(cellId - gridWidth))
			{
				list.Add(cellId - gridWidth);
			}
			if (flag3 && !cellIds.Contains(cellId + gridWidth))
			{
				list.Add(cellId + gridWidth);
			}
		}
		return list;
	}

	public bool CanMoveBag(BagInstance bag)
	{
		foreach (KeyValuePair<int, BagInstance> item in _bagsInGridCells.Where((KeyValuePair<int, BagInstance> c) => c.Value.Equals(bag)).ToList())
		{
			if (_gridCells[item.Key] != Enums.Backpack.GridCellStatus.ContainsBag)
			{
				return false;
			}
		}
		return true;
	}

	public bool PlaceBag(BagInstance bag, List<int> cellIdsToPlaceOn)
	{
		List<int> invalidCellIds = new List<int>();
		if (!CanPlaceBag(bag, cellIdsToPlaceOn, out invalidCellIds))
		{
			return false;
		}
		foreach (int item in cellIdsToPlaceOn)
		{
			_gridCells[item] = Enums.Backpack.GridCellStatus.ContainsBag;
			_bagsInGridCells.Add(item, bag);
		}
		UpdateTimeOfLastChange();
		return true;
	}

	public bool PlaceItem(ItemInstance item, List<int> cellIdsToPlaceOn, List<int> starredCellids)
	{
		List<int> invalidCellIds = new List<int>();
		bool num = CanPlaceItem(item, cellIdsToPlaceOn, out invalidCellIds);
		bool flag = CanPlaceEntireItemInCells(cellIdsToPlaceOn, item.TotalPlaceableSize);
		if (num && !flag)
		{
			return false;
		}
		if (!TryReplaceExistingPlaceable(cellIdsToPlaceOn))
		{
			return false;
		}
		foreach (int item2 in cellIdsToPlaceOn)
		{
			_gridCells[item2] = Enums.Backpack.GridCellStatus.ContainsItem;
			_itemsInGridCells.Add(item2, item);
		}
		foreach (int starredCellid in starredCellids)
		{
			_starredCellsByItems.Add(new KeyValuePair<int, ItemInstance>(starredCellid, item));
		}
		UpdateTimeOfLastChange();
		return true;
	}

	public bool PlaceWeapon(WeaponInstance weapon, List<int> cellIdsToPlaceOn, List<int> starredCellids)
	{
		List<int> invalidCellIds = new List<int>();
		bool num = CanPlaceWeapon(weapon, cellIdsToPlaceOn, out invalidCellIds);
		bool flag = CanPlaceEntireItemInCells(cellIdsToPlaceOn, weapon.TotalPlaceableSize);
		if (num && !flag)
		{
			return false;
		}
		if (!TryReplaceExistingPlaceable(cellIdsToPlaceOn))
		{
			return false;
		}
		foreach (int item in cellIdsToPlaceOn)
		{
			_gridCells[item] = Enums.Backpack.GridCellStatus.ContainsItem;
			_weaponsInGridCells.Add(item, weapon);
		}
		foreach (int starredCellid in starredCellids)
		{
			_starredCellsByWeapons.Add(new KeyValuePair<int, WeaponInstance>(starredCellid, weapon));
		}
		UpdateTimeOfLastChange();
		return true;
	}

	private bool TryReplaceExistingPlaceable(List<int> cellIdsToPlaceOn)
	{
		IEnumerable<ItemInstance> itemInstancesInCells = GetItemInstancesInCells(cellIdsToPlaceOn);
		IEnumerable<WeaponInstance> weaponInstancesInCells = GetWeaponInstancesInCells(cellIdsToPlaceOn);
		if (itemInstancesInCells != null && itemInstancesInCells.Count() > 1)
		{
			return false;
		}
		if (weaponInstancesInCells != null && weaponInstancesInCells.Count() > 1)
		{
			return false;
		}
		if (itemInstancesInCells != null && itemInstancesInCells.Count() == 1)
		{
			if (!SingletonController<BackpackController>.Instance.CanPlaceItemBackIntoStorage(itemInstancesInCells.First()))
			{
				return false;
			}
			RemoveItem(itemInstancesInCells.First());
			ItemsToMoveToStorage.AddRange(itemInstancesInCells);
			UpdateTimeOfLastChange();
			return true;
		}
		if (weaponInstancesInCells != null && weaponInstancesInCells.Count() == 1)
		{
			if (!SingletonController<BackpackController>.Instance.CanPlaceWeaponBackIntoStorage(weaponInstancesInCells.First()))
			{
				return false;
			}
			RemoveWeapon(weaponInstancesInCells.First());
			WeaponsToMoveToStorage.AddRange(weaponInstancesInCells);
			UpdateTimeOfLastChange();
			return true;
		}
		return true;
	}

	public void RemoveBag(BagInstance bag)
	{
		List<int> list = (from kvp in _bagsInGridCells
			where kvp.Value.Equals(bag)
			select kvp.Key).ToList();
		SetCellsToNewStatus(list, Enums.Backpack.GridCellStatus.Empty);
		list.ForEach(delegate(int c)
		{
			_bagsInGridCells.Remove(c);
		});
		UpdateTimeOfLastChange();
	}

	public void RemoveItem(ItemInstance item)
	{
		List<int> list = (from kvp in _itemsInGridCells
			where kvp.Value.Equals(item)
			select kvp.Key).ToList();
		SetCellsToNewStatus(list, Enums.Backpack.GridCellStatus.ContainsBag);
		list.ForEach(delegate(int c)
		{
			_itemsInGridCells.Remove(c);
		});
		_starredCellsByItems.Where((KeyValuePair<int, ItemInstance> kvp) => kvp.Value.Equals(item)).ToList().ForEach(delegate(KeyValuePair<int, ItemInstance> c)
		{
			_starredCellsByItems.Remove(c);
		});
		UpdateTimeOfLastChange();
	}

	public void RemoveBaseItemInstance(BaseItemInstance baseItemInstance)
	{
		if (baseItemInstance is WeaponInstance)
		{
			RemoveWeapon((WeaponInstance)baseItemInstance);
		}
		if (baseItemInstance is ItemInstance)
		{
			RemoveItem((ItemInstance)baseItemInstance);
		}
		if (baseItemInstance is BagInstance)
		{
			RemoveBag((BagInstance)baseItemInstance);
		}
	}

	public void RemoveWeapon(WeaponInstance weapon)
	{
		List<int> list = (from kvp in _weaponsInGridCells
			where kvp.Value.Equals(weapon)
			select kvp.Key).ToList();
		SetCellsToNewStatus(list, Enums.Backpack.GridCellStatus.ContainsBag);
		list.ForEach(delegate(int c)
		{
			_weaponsInGridCells.Remove(c);
		});
		_starredCellsByWeapons.Where((KeyValuePair<int, WeaponInstance> kvp) => kvp.Value.Equals(weapon)).ToList().ForEach(delegate(KeyValuePair<int, WeaponInstance> c)
		{
			_starredCellsByWeapons.Remove(c);
		});
		UpdateTimeOfLastChange();
	}

	private void SetCellsToNewStatus(List<int> cellIds, Enums.Backpack.GridCellStatus newStatus)
	{
		foreach (int cellId in cellIds)
		{
			_gridCells[cellId] = newStatus;
		}
	}

	private bool CanPlaceEntireItemInCells(List<int> itemSlotIds, int totalPlaceableSize)
	{
		return itemSlotIds.Count == totalPlaceableSize;
	}

	public Enums.Backpack.GridCellStatus GetCellStatus(int slotId)
	{
		return _gridCells[slotId];
	}

	public bool SlotContainsOnlyBag(int slotId)
	{
		bool num = _bagsInGridCells.ContainsKey(slotId);
		bool flag = !_itemsInGridCells.ContainsKey(slotId) && !_weaponsInGridCells.ContainsKey(slotId);
		return num && flag;
	}
}
