using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Game;

public class StartingEquipmentController : SingletonController<StartingEquipmentController>
{
	private List<BagSO> _startingBags = new List<BagSO>();

	private List<WeaponSO> _startingWeapons = new List<WeaponSO>();

	private List<ItemSO> _startingItems = new List<ItemSO>();

	private List<RelicSO> _startingRelics = new List<RelicSO>();

	private int _startingCoins;

	private bool _isStartingEquipmentInitialized;

	private List<int> _backpackRowsLockedByDefault = new List<int> { 0, 1, 2, 9, 10, 11 };

	private List<int> _backpackColumnsLockedByDefault = new List<int> { 0, 1, 2, 9, 10, 11 };

	private bool _lockedCellsCompleted;

	private void Start()
	{
		base.IsInitialized = true;
	}

	private void BackpackController_OnUIShown()
	{
		StartCoroutine(InstantiateStartingEquipment());
	}

	public void SwitchStartingCharacter(CharacterSO newStartingCharacterSO)
	{
		AddStartingEquipment(newStartingCharacterSO);
	}

	private void AddStartingEquipment(CharacterSO newStartingCharacterSO)
	{
		Clear();
		AddStartingBags(newStartingCharacterSO.StartingBags);
		AddStartingWeapons(newStartingCharacterSO.StartingWeapons);
		AddStartingItems(newStartingCharacterSO.StartingItems);
		AddStartingRelics(newStartingCharacterSO.StartingRelics);
		AddStartingCoins(newStartingCharacterSO.StartingCoins);
		List<WeaponSO> unlockedWeaponsForCharacter = SingletonController<UnlockedEquipmentController>.Instance.GetUnlockedWeaponsForCharacter(newStartingCharacterSO.Id);
		AddStartingWeapons(unlockedWeaponsForCharacter);
		List<ItemSO> unlockedItemsForCharacter = SingletonController<UnlockedEquipmentController>.Instance.GetUnlockedItemsForCharacter(newStartingCharacterSO.Id);
		AddStartingItems(unlockedItemsForCharacter);
		List<BagSO> unlockedBagsForCharacter = SingletonController<UnlockedEquipmentController>.Instance.GetUnlockedBagsForCharacter(newStartingCharacterSO.Id);
		AddStartingBags(unlockedBagsForCharacter);
		List<RelicSO> unlockedRelicsForCharacter = SingletonController<UnlockedEquipmentController>.Instance.GetUnlockedRelicsForCharacter(newStartingCharacterSO.Id);
		AddStartingRelics(unlockedRelicsForCharacter);
	}

	public void AddStartingBags(IEnumerable<BagSO> bags)
	{
		_startingBags.AddRange(bags);
	}

	public void AddStartingWeapons(IEnumerable<WeaponSO> weapons)
	{
		_startingWeapons.AddRange(weapons);
	}

	public void AddStartingItems(IEnumerable<ItemSO> items)
	{
		_startingItems.AddRange(items);
	}

	public void AddStartingRelics(IEnumerable<RelicSO> relics)
	{
		_startingRelics.AddRange(relics);
	}

	public void AddStartingCoins(int amount)
	{
		_startingCoins += amount;
	}

	public void RemoveStartingBags(IEnumerable<BagSO> bags)
	{
		foreach (BagSO bag in bags)
		{
			_startingBags.Remove(bag);
		}
	}

	public override void Clear()
	{
		ResetStartingEquipment();
	}

	public override void ClearAdventure()
	{
		ResetStartingEquipment();
		_lockedCellsCompleted = false;
	}

	private void ResetStartingEquipment()
	{
		_startingBags.Clear();
		_startingWeapons.Clear();
		_startingItems.Clear();
		_startingRelics.Clear();
		_startingCoins = 0;
	}

	public IEnumerator InstantiateStartingEquipment(bool closeUI = false)
	{
		yield return null;
		if (_isStartingEquipmentInitialized)
		{
			yield break;
		}
		ResetInitializedEquipment();
		SingletonController<BackpackController>.Instance.OpenUI();
		StartCoroutine(FillBackpackWithStartingItems());
		foreach (RelicSO startingRelic in _startingRelics)
		{
			SingletonController<RelicsController>.Instance.AddRelicById(startingRelic.Id);
		}
		SingletonController<CurrencyController>.Instance.SetCurrency(Enums.CurrencyType.Coins, _startingCoins);
		ApplyUnlockedStartingGold();
		_isStartingEquipmentInitialized = true;
		LockBackpackCells();
		SingletonController<BackpackController>.Instance.OnUIShown -= BackpackController_OnUIShown;
		if (closeUI)
		{
			yield return null;
			SingletonController<BackpackController>.Instance.CloseUI();
		}
	}

	private void ApplyUnlockedStartingGold()
	{
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ExtraStartingGold);
		int numberOfCurrency = 25 * unlockedCount;
		SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.Coins, numberOfCurrency, Enums.CurrencySource.PrerunSetup);
	}

	private void LockBackpackCells()
	{
		if (!_lockedCellsCompleted)
		{
			_lockedCellsCompleted = true;
			List<int> lockedRowIds = GetLockedRowIds();
			List<int> lockedColumnIds = GetLockedColumnIds();
			SingletonController<BackpackController>.Instance.LockBackpackCells(lockedRowIds, lockedColumnIds);
		}
	}

	private List<int> GetLockedRowIds()
	{
		List<int> list = new List<int>();
		list.AddRange(_backpackRowsLockedByDefault);
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.BagSpace);
		for (int i = 0; i < unlockedCount; i++)
		{
			int item = 2 - i;
			int item2 = 9 + i;
			list.Remove(item);
			list.Remove(item2);
		}
		return list;
	}

	private List<int> GetLockedColumnIds()
	{
		List<int> list = new List<int>();
		list.AddRange(_backpackColumnsLockedByDefault);
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.BagSpace);
		for (int i = 0; i < unlockedCount; i++)
		{
			int item = 2 - i;
			int item2 = 9 + i;
			list.Remove(item);
			list.Remove(item2);
		}
		return list;
	}

	public void Reset()
	{
		_isStartingEquipmentInitialized = false;
		ResetInitializedEquipment();
		SingletonController<BackpackController>.Instance.OnUIShown += BackpackController_OnUIShown;
	}

	private IEnumerator FillBackpackWithStartingItems()
	{
		yield return null;
		foreach (BagSO startingBag in _startingBags)
		{
			SingletonController<BackpackController>.Instance.AddBagToStorage(startingBag);
		}
		foreach (WeaponSO startingWeapon in _startingWeapons)
		{
			SingletonController<BackpackController>.Instance.AddWeaponToStorage(startingWeapon);
		}
		foreach (ItemSO startingItem in _startingItems)
		{
			if (startingItem != null)
			{
				SingletonController<BackpackController>.Instance.AddItemToStorage(startingItem);
			}
		}
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: true);
		SingletonController<MergeController>.Instance.UpdateMergePossibilities();
	}

	private void ResetInitializedEquipment()
	{
		SingletonController<BackpackController>.Instance.ClearBackpack();
		SingletonController<RelicsController>.Instance.ClearRelics();
		SingletonController<CurrencyController>.Instance.SetCurrency(Enums.CurrencyType.Coins, 0);
	}
}
