using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Characters;

internal class UnlockedEquipmentController : SingletonController<UnlockedEquipmentController>
{
	private List<int> _genericUnlockedWeaponIds = new List<int>();

	private List<int> _genericUnlockedItemIds = new List<int>();

	private List<int> _genericUnlockedBagIds = new List<int>();

	private List<int> _genericUnlockedRelicIds = new List<int>();

	private Dictionary<int, int> _characterSpecificUnlockedWeaponIds = new Dictionary<int, int>();

	private Dictionary<int, int> _characterSpecificUnlockedItemIds = new Dictionary<int, int>();

	private Dictionary<int, int> _characterSpecificUnlockedBagIds = new Dictionary<int, int>();

	private Dictionary<int, int> _characterSpecificUnlockedRelicIds = new Dictionary<int, int>();

	private List<int> _receivedRewardIds = new List<int>();

	private void Start()
	{
		RegisterEvents();
	}

	internal List<WeaponSO> GetUnlockedWeaponsForCharacter(int characterId)
	{
		List<WeaponSO> list = new List<WeaponSO>();
		AddWeaponsToList(list, _genericUnlockedWeaponIds);
		List<int> weaponIds = (from w in _characterSpecificUnlockedWeaponIds
			where w.Key == characterId
			select w.Value).ToList();
		AddWeaponsToList(list, weaponIds);
		return list;
	}

	internal List<ItemSO> GetUnlockedItemsForCharacter(int characterId)
	{
		List<ItemSO> list = new List<ItemSO>();
		AddItemsToList(list, _genericUnlockedItemIds);
		List<int> characterSpecificStartingItemIds = (from w in _characterSpecificUnlockedItemIds
			where w.Key == characterId
			select w.Value).ToList();
		AddItemsToList(list, characterSpecificStartingItemIds);
		return list;
	}

	internal List<BagSO> GetUnlockedBagsForCharacter(int characterId)
	{
		List<BagSO> list = new List<BagSO>();
		AddBagsToList(list, _genericUnlockedBagIds);
		List<int> characterSpecificStartingBagIds = (from w in _characterSpecificUnlockedBagIds
			where w.Key == characterId
			select w.Value).ToList();
		AddBagsToList(list, characterSpecificStartingBagIds);
		return list;
	}

	internal List<RelicSO> GetUnlockedRelicsForCharacter(int characterId)
	{
		List<RelicSO> list = new List<RelicSO>();
		AddRelicsToList(list, _genericUnlockedRelicIds);
		List<int> characterSpecificStartingRelicIds = (from w in _characterSpecificUnlockedRelicIds
			where w.Key == characterId
			select w.Value).ToList();
		AddRelicsToList(list, characterSpecificStartingRelicIds);
		return list;
	}

	internal void AddWeaponReward(RewardSO rewardSO)
	{
		if (CanReceiveReward(rewardSO))
		{
			int characterId = ((rewardSO.BoundToCharacter != null) ? rewardSO.BoundToCharacter.Id : 0);
			AddUnlockedWeapon(rewardSO.CompletionReward as WeaponSO, characterId);
			AddReceivedReWardToList(rewardSO);
		}
	}

	internal void AddItemReward(RewardSO rewardSO)
	{
		if (CanReceiveReward(rewardSO))
		{
			int characterId = ((rewardSO.BoundToCharacter != null) ? rewardSO.BoundToCharacter.Id : 0);
			AddUnlockedItem(rewardSO.CompletionReward as ItemSO, characterId);
			AddReceivedReWardToList(rewardSO);
		}
	}

	internal void AddBagReward(RewardSO rewardSO)
	{
		if (CanReceiveReward(rewardSO))
		{
			int characterId = ((rewardSO.BoundToCharacter != null) ? rewardSO.BoundToCharacter.Id : 0);
			AddUnlockedBag(rewardSO.CompletionReward as BagSO, characterId);
			AddReceivedReWardToList(rewardSO);
		}
	}

	internal void AddRelicReward(RewardSO rewardSO)
	{
		if (CanReceiveReward(rewardSO))
		{
			int characterId = ((rewardSO.BoundToCharacter != null) ? rewardSO.BoundToCharacter.Id : 0);
			AddUnlockedRelic(rewardSO.CompletionReward as RelicSO, characterId);
			AddReceivedReWardToList(rewardSO);
		}
	}

	private void AddUnlockedWeapon(WeaponSO weaponSO, int characterId = 0)
	{
		if (characterId <= 0)
		{
			_genericUnlockedWeaponIds.Add(weaponSO.Id);
		}
		if (characterId > 0)
		{
			_characterSpecificUnlockedWeaponIds.Add(characterId, weaponSO.Id);
		}
	}

	private void AddUnlockedItem(ItemSO itemSO, int characterId = 0)
	{
		if (characterId <= 0)
		{
			_genericUnlockedItemIds.Add(itemSO.Id);
		}
		if (characterId > 0)
		{
			_characterSpecificUnlockedItemIds.Add(characterId, itemSO.Id);
		}
	}

	private void AddUnlockedBag(BagSO bagSO, int characterId = 0)
	{
		if (characterId <= 0)
		{
			_genericUnlockedBagIds.Add(bagSO.Id);
		}
		if (characterId > 0)
		{
			_characterSpecificUnlockedBagIds.Add(characterId, bagSO.Id);
		}
	}

	private void AddUnlockedRelic(RelicSO relicSO, int characterId = 0)
	{
		if (characterId <= 0)
		{
			_genericUnlockedRelicIds.Add(relicSO.Id);
		}
		if (characterId > 0)
		{
			_characterSpecificUnlockedRelicIds.Add(characterId, relicSO.Id);
		}
	}

	private void AddReceivedReWardToList(RewardSO rewardSO)
	{
		if (!_receivedRewardIds.Contains(rewardSO.Id))
		{
			_receivedRewardIds.Add(rewardSO.Id);
		}
	}

	internal UnlockedEquipmentSaveState GetSaveState()
	{
		UnlockedEquipmentSaveState unlockedEquipmentSaveState = new UnlockedEquipmentSaveState();
		unlockedEquipmentSaveState.SetState(_genericUnlockedWeaponIds, _characterSpecificUnlockedWeaponIds, _genericUnlockedItemIds, _characterSpecificUnlockedItemIds, _genericUnlockedRelicIds, _characterSpecificUnlockedRelicIds, _genericUnlockedBagIds, _characterSpecificUnlockedItemIds, _receivedRewardIds);
		return unlockedEquipmentSaveState;
	}

	private void AddWeaponsToList(List<WeaponSO> weapons, List<int> weaponIds)
	{
		foreach (int weaponId in weaponIds)
		{
			WeaponSO weaponById = GameDatabaseHelper.GetWeaponById(weaponId);
			weapons.Add(weaponById);
		}
	}

	private void AddItemsToList(List<ItemSO> items, List<int> characterSpecificStartingItemIds)
	{
		foreach (int characterSpecificStartingItemId in characterSpecificStartingItemIds)
		{
			ItemSO itemById = GameDatabaseHelper.GetItemById(characterSpecificStartingItemId);
			items.Add(itemById);
		}
	}

	private void AddBagsToList(List<BagSO> bags, List<int> characterSpecificStartingBagIds)
	{
		foreach (int characterSpecificStartingBagId in characterSpecificStartingBagIds)
		{
			BagSO bagById = GameDatabaseHelper.GetBagById(characterSpecificStartingBagId);
			bags.Add(bagById);
		}
	}

	private void AddRelicsToList(List<RelicSO> relics, List<int> characterSpecificStartingRelicIds)
	{
		foreach (int characterSpecificStartingRelicId in characterSpecificStartingRelicIds)
		{
			RelicSO relicById = GameDatabaseHelper.GetRelicById(characterSpecificStartingRelicId);
			relics.Add(relicById);
		}
	}

	private bool CanReceiveReward(RewardSO rewardSO)
	{
		if (rewardSO.Repeatable)
		{
			return true;
		}
		return !_receivedRewardIds.Contains(rewardSO.Id);
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameLoadedEvent);
	}

	private void RegisterSaveGameLoadedEvent()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		if (e.SaveGame.EquipmentSaveState.HasData())
		{
			LoadFromSave(e.SaveGame.EquipmentSaveState);
		}
	}

	private void LoadFromSave(UnlockedEquipmentSaveState equipmentSaveState)
	{
		_genericUnlockedWeaponIds = equipmentSaveState.GenericUnlockedWeaponIds;
		_characterSpecificUnlockedWeaponIds = equipmentSaveState.CharacterSpecificUnlockedWeaponIds;
		_genericUnlockedItemIds = equipmentSaveState.GenericUnlockedItemIds;
		_characterSpecificUnlockedItemIds = equipmentSaveState.CharacterSpecificUnlockedItemIds;
		_genericUnlockedBagIds = equipmentSaveState.GenericUnlockedBagIds;
		_characterSpecificUnlockedBagIds = equipmentSaveState.CharacterSpecificUnlockedBagIds;
		_genericUnlockedRelicIds = equipmentSaveState.GenericUnlockedRelicIds;
		_characterSpecificUnlockedRelicIds = equipmentSaveState.CharacterSpecificUnlockedRelicIds;
		_receivedRewardIds = equipmentSaveState.ReceivedRewardIds;
		base.IsInitialized = true;
	}

	private void LogUnlockedEquipment()
	{
		Debug.Log("Unlocked equipment:");
		Debug.Log($"Weapons: {_genericUnlockedWeaponIds.Count} generic + {_characterSpecificUnlockedWeaponIds.Count} char specific");
		Debug.Log($"Items: {_genericUnlockedItemIds.Count} generic + {_characterSpecificUnlockedItemIds.Count} char specific");
		Debug.Log($"Bags: {_genericUnlockedBagIds.Count} generic + {_characterSpecificUnlockedBagIds.Count} char specific");
		Debug.Log($"Relics: {_genericUnlockedRelicIds.Count} generic + {_characterSpecificUnlockedRelicIds.Count} char specific");
	}

	public override void Clear()
	{
		_genericUnlockedItemIds.Clear();
		_genericUnlockedRelicIds.Clear();
		_genericUnlockedWeaponIds.Clear();
		_genericUnlockedBagIds.Clear();
		_characterSpecificUnlockedItemIds.Clear();
		_characterSpecificUnlockedRelicIds.Clear();
		_characterSpecificUnlockedWeaponIds.Clear();
		_characterSpecificUnlockedBagIds.Clear();
		_receivedRewardIds.Clear();
	}

	public override void ClearAdventure()
	{
	}
}
