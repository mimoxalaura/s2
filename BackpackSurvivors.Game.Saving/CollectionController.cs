using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Assets.UI.Book;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Saving;

public class CollectionController : BaseSingletonModalUIController<CollectionController>
{
	[SerializeField]
	private BookController _bookController;

	private Dictionary<int, bool> _availableWeaponsUnlockedStates = new Dictionary<int, bool>();

	private Dictionary<int, bool> _availableItemsUnlockedStates = new Dictionary<int, bool>();

	private Dictionary<int, bool> _availableRelicsUnlockedStates = new Dictionary<int, bool>();

	private Dictionary<EnemySO, bool> _availableEnemiesUnlockedStates = new Dictionary<EnemySO, bool>();

	private Dictionary<int, bool> _availableMergableUnlockedStates = new Dictionary<int, bool>();

	private Dictionary<CharacterSO, bool> _availableCharactersUnlockedStates = new Dictionary<CharacterSO, bool>();

	private Dictionary<int, bool> _availableBagsUnlockedStates = new Dictionary<int, bool>();

	private ModalUiController _modalUiController;

	public Dictionary<int, bool> AvailableWeaponsUnlockedStates => _availableWeaponsUnlockedStates;

	public Dictionary<int, bool> AvailableItemsUnlockedStates => _availableItemsUnlockedStates;

	public Dictionary<int, bool> AvailableRelicsUnlockedStates => _availableRelicsUnlockedStates;

	public Dictionary<EnemySO, bool> AvailableEnemiesUnlockedStates => _availableEnemiesUnlockedStates;

	internal Dictionary<int, bool> AvailableMergableUnlockedStates => _availableMergableUnlockedStates;

	public Dictionary<CharacterSO, bool> AvailableCharactersUnlockedStates => _availableCharactersUnlockedStates;

	public Dictionary<int, bool> AvailableBagsUnlockedStates => _availableBagsUnlockedStates;

	private void Start()
	{
		RegisterEvents();
		ClearDictionaries();
		InitDictionariesUncollected();
		base.IsInitialized = true;
	}

	public override bool CloseOnCancelInput()
	{
		return true;
	}

	public CollectionsSaveState GetSaveState()
	{
		CollectionsSaveState collectionsSaveState = new CollectionsSaveState();
		List<int> collectedWeapons = GetCollectedWeapons();
		List<int> collectedItems = GetCollectedItems();
		List<int> collectedRelics = GetCollectedRelics();
		List<int> collectedEnemies = GetCollectedEnemies();
		List<int> collectedCharacters = GetCollectedCharacters();
		List<int> collectedBags = GetCollectedBags();
		List<int> foundRecipes = GetFoundRecipes();
		collectionsSaveState.SetState(collectedItems, collectedWeapons, collectedRelics, collectedCharacters, collectedEnemies, collectedBags, foundRecipes);
		return collectionsSaveState;
	}

	private List<int> GetCollectedBags()
	{
		if (_availableRelicsUnlockedStates == null)
		{
			return new List<int>();
		}
		return (from b in _availableBagsUnlockedStates
			where b.Value
			select b.Key).ToList();
	}

	private List<int> GetFoundRecipes()
	{
		if (_availableMergableUnlockedStates == null)
		{
			return new List<int>();
		}
		return (from b in _availableMergableUnlockedStates
			where b.Value
			select b.Key).ToList();
	}

	private List<int> GetCollectedCharacters()
	{
		if (_availableRelicsUnlockedStates == null)
		{
			return new List<int>();
		}
		return (from c in _availableCharactersUnlockedStates
			where c.Value
			select c.Key.Id).ToList();
	}

	private List<int> GetCollectedEnemies()
	{
		if (_availableRelicsUnlockedStates == null)
		{
			return new List<int>();
		}
		return (from e in _availableEnemiesUnlockedStates
			where e.Value
			select e.Key.Id).ToList();
	}

	private List<int> GetCollectedRelics()
	{
		if (_availableRelicsUnlockedStates == null)
		{
			return new List<int>();
		}
		return (from r in _availableRelicsUnlockedStates
			where r.Value
			select r.Key).ToList();
	}

	private List<int> GetCollectedItems()
	{
		if (_availableItemsUnlockedStates == null)
		{
			return new List<int>();
		}
		return (from i in _availableItemsUnlockedStates
			where i.Value
			select i.Key).ToList();
	}

	private List<int> GetCollectedWeapons()
	{
		if (_availableWeaponsUnlockedStates == null)
		{
			return new List<int>();
		}
		return (from w in _availableWeaponsUnlockedStates
			where w.Value
			select w.Key).ToList();
	}

	[Command("collection.weapon", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UnlockWeapon(int id)
	{
		Unlock(id, _availableWeaponsUnlockedStates);
	}

	[Command("collection.item", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UnlockItem(int id)
	{
		Unlock(id, _availableItemsUnlockedStates);
	}

	[Command("collection.bag", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UnlockBag(int id)
	{
		Unlock(id, _availableBagsUnlockedStates);
	}

	[Command("collection.relic", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UnlockRelic(int id)
	{
		Unlock(id, _availableRelicsUnlockedStates);
	}

	[Command("collection.enemy", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UnlockEnemy(int enemyId)
	{
		EnemySO enemySO = GameDatabaseHelper.GetEnemies().FirstOrDefault((EnemySO x) => x.Id == enemyId);
		if (!(enemySO == null) && _availableEnemiesUnlockedStates.ContainsKey(enemySO))
		{
			_availableEnemiesUnlockedStates[enemySO] = true;
		}
	}

	internal void UnlockEnemy(EnemySO enemy)
	{
		if (_availableEnemiesUnlockedStates != null && _availableEnemiesUnlockedStates.ContainsKey(enemy))
		{
			_availableEnemiesUnlockedStates[enemy] = true;
		}
	}

	[Command("collection.recipe", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void UnlockRecipe(int id)
	{
		Unlock(id, _availableMergableUnlockedStates);
	}

	internal bool IsWeaponUnlocked(int id)
	{
		return GetUnlockedState(id, _availableWeaponsUnlockedStates);
	}

	internal bool IsItemUnlocked(int id)
	{
		return GetUnlockedState(id, _availableItemsUnlockedStates);
	}

	internal bool IsBagUnlocked(int id)
	{
		return GetUnlockedState(id, _availableBagsUnlockedStates);
	}

	internal bool IsRelicUnlocked(int id)
	{
		return GetUnlockedState(id, _availableRelicsUnlockedStates);
	}

	internal bool IsEnemyUnlocked(EnemySO enemySO)
	{
		if (_availableEnemiesUnlockedStates == null)
		{
			return false;
		}
		if (!_availableEnemiesUnlockedStates.ContainsKey(enemySO))
		{
			return false;
		}
		return _availableEnemiesUnlockedStates[enemySO];
	}

	internal bool IsRecipeUnlocked(int id)
	{
		return GetUnlockedState(id, _availableMergableUnlockedStates);
	}

	private bool GetUnlockedState(int id, Dictionary<int, bool> collectionRef)
	{
		if (collectionRef == null)
		{
			return false;
		}
		if (!collectionRef.ContainsKey(id))
		{
			return false;
		}
		return collectionRef[id];
	}

	private void Unlock(int id, Dictionary<int, bool> collectionRef)
	{
		if (collectionRef != null && collectionRef.ContainsKey(id))
		{
			collectionRef[id] = true;
		}
	}

	[Command("collection.weapons.all", Platform.AllPlatforms, MonoTargetType.Single)]
	public void UnlockAllWeapons()
	{
		foreach (WeaponSO weapon in GameDatabaseHelper.GetWeapons())
		{
			_availableWeaponsUnlockedStates[weapon.Id] = true;
		}
	}

	[Command("collection.weapons.None", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LockAllWeapons()
	{
		foreach (WeaponSO weapon in GameDatabaseHelper.GetWeapons())
		{
			_availableWeaponsUnlockedStates[weapon.Id] = false;
		}
	}

	[Command("collection.items.all", Platform.AllPlatforms, MonoTargetType.Single)]
	public void UnlockAllItems()
	{
		foreach (ItemSO item in GameDatabaseHelper.GetItems())
		{
			_availableItemsUnlockedStates[item.Id] = true;
		}
	}

	[Command("collection.items.None", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LockAllItems()
	{
		foreach (ItemSO item in GameDatabaseHelper.GetItems())
		{
			_availableItemsUnlockedStates[item.Id] = false;
		}
	}

	[Command("collection.bags.all", Platform.AllPlatforms, MonoTargetType.Single)]
	public void UnlockAllBags()
	{
		foreach (BagSO bag in GameDatabaseHelper.GetBags())
		{
			_availableBagsUnlockedStates[bag.Id] = true;
		}
	}

	[Command("collection.bags.None", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LockAllBags()
	{
		foreach (BagSO bag in GameDatabaseHelper.GetBags())
		{
			_availableBagsUnlockedStates[bag.Id] = false;
		}
	}

	[Command("collection.enemies.all", Platform.AllPlatforms, MonoTargetType.Single)]
	public void UnlockAllEnemies()
	{
		foreach (EnemySO enemy in GameDatabaseHelper.GetEnemies())
		{
			_availableEnemiesUnlockedStates[enemy] = true;
		}
	}

	[Command("collection.enemies.None", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LockAllEnemies()
	{
		foreach (EnemySO enemy in GameDatabaseHelper.GetEnemies())
		{
			_availableEnemiesUnlockedStates[enemy] = false;
		}
	}

	[Command("collection.recipes.all", Platform.AllPlatforms, MonoTargetType.Single)]
	public void UnlockAllRecipes()
	{
		foreach (MergableSO mergeRecipe in GameDatabaseHelper.GetMergeRecipes())
		{
			_availableMergableUnlockedStates[mergeRecipe.Id] = true;
		}
	}

	[Command("collection.recipes.None", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LockAllRecipes()
	{
		foreach (MergableSO mergeRecipe in GameDatabaseHelper.GetMergeRecipes())
		{
			_availableMergableUnlockedStates[mergeRecipe.Id] = false;
		}
	}

	[Command("collection.relics.all", Platform.AllPlatforms, MonoTargetType.Single)]
	public void UnlockAllRelics()
	{
		foreach (RelicSO relic in GameDatabaseHelper.GetRelics())
		{
			_availableRelicsUnlockedStates[relic.Id] = true;
		}
	}

	[Command("collection.relics.None", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LockAllRelics()
	{
		foreach (RelicSO relic in GameDatabaseHelper.GetRelics())
		{
			_availableRelicsUnlockedStates[relic.Id] = false;
		}
	}

	[Command("collection.unlock_all", Platform.AllPlatforms, MonoTargetType.Single)]
	public void UnlockAllCollections()
	{
		ClearDictionaries();
		InitDictionariesUncollected();
		UnlockAllRelics();
		UnlockAllBags();
		UnlockAllEnemies();
		UnlockAllItems();
		UnlockAllWeapons();
		UnlockAllRecipes();
	}

	[Command("collection.lock_all", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LockAllCollections()
	{
		ClearDictionaries();
		InitDictionariesUncollected();
		LockAllRelics();
		LockAllBags();
		LockAllEnemies();
		LockAllItems();
		LockAllWeapons();
		LockAllRecipes();
	}

	private void InitDictionariesUncollected()
	{
		ClearDictionaries();
		foreach (WeaponSO weapon in GameDatabaseHelper.GetWeapons())
		{
			_availableWeaponsUnlockedStates.Add(weapon.Id, value: false);
		}
		foreach (ItemSO item in GameDatabaseHelper.GetItems())
		{
			if (!_availableItemsUnlockedStates.ContainsKey(item.Id))
			{
				_availableItemsUnlockedStates.Add(item.Id, value: false);
			}
		}
		foreach (EnemySO enemy in GameDatabaseHelper.GetEnemies())
		{
			_availableEnemiesUnlockedStates.Add(enemy, value: false);
		}
		foreach (RelicSO relic in GameDatabaseHelper.GetRelics())
		{
			_availableRelicsUnlockedStates.Add(relic.Id, value: false);
		}
		foreach (CharacterSO character in GameDatabaseHelper.GetCharacters())
		{
			_availableCharactersUnlockedStates.Add(character, value: false);
		}
		foreach (BagSO bag in GameDatabaseHelper.GetBags())
		{
			_availableBagsUnlockedStates.Add(bag.Id, value: false);
		}
		foreach (MergableSO mergeRecipe in GameDatabaseHelper.GetMergeRecipes())
		{
			if (!_availableMergableUnlockedStates.ContainsKey(mergeRecipe.Id))
			{
				_availableMergableUnlockedStates.Add(mergeRecipe.Id, value: false);
			}
		}
	}

	private void ClearDictionaries()
	{
		_availableWeaponsUnlockedStates.Clear();
		_availableItemsUnlockedStates.Clear();
		_availableRelicsUnlockedStates.Clear();
		_availableEnemiesUnlockedStates.Clear();
		_availableCharactersUnlockedStates.Clear();
		_availableBagsUnlockedStates.Clear();
		_availableMergableUnlockedStates.Clear();
	}

	public void CloseButtonPressed()
	{
		GetModalUiController().CloseModalUI(Enums.ModalUITypes.Collection);
	}

	public ModalUiController GetModalUiController()
	{
		if (_modalUiController == null)
		{
			_modalUiController = UnityEngine.Object.FindAnyObjectByType<ModalUiController>();
		}
		return _modalUiController;
	}

	public override void OpenUI()
	{
		base.OpenUI();
		SingletonController<InputController>.Instance.OnCancelHandler += Instance_OnCancelHandler;
		SingletonController<InputController>.Instance.OnNextHandler += Instance_OnNextHandler;
		SingletonController<InputController>.Instance.OnPreviousHandler += Instance_OnPreviousHandler;
		_bookController.OpenBook();
	}

	public override void CloseUI()
	{
		base.CloseUI();
		SingletonController<InputController>.Instance.OnCancelHandler -= Instance_OnCancelHandler;
		SingletonController<InputController>.Instance.OnNextHandler -= Instance_OnNextHandler;
		SingletonController<InputController>.Instance.OnPreviousHandler -= Instance_OnPreviousHandler;
		_bookController.CloseBook();
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameLoaded);
	}

	public void RegisterSaveGameLoaded()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	private void Instance_OnPreviousHandler(object sender, EventArgs e)
	{
		_bookController.PreviousPage();
	}

	private void Instance_OnNextHandler(object sender, EventArgs e)
	{
		_bookController.NextPage();
	}

	private void Instance_OnCancelHandler(object sender, EventArgs e)
	{
		CloseUI();
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		ClearDictionaries();
		InitDictionariesUncollected();
		FillDictionariesFromSaveGame(e.SaveGame.CollectionsSaveState);
	}

	private void FillDictionariesFromSaveGame(CollectionsSaveState saveState)
	{
		foreach (WeaponSO weapon in GameDatabaseHelper.GetWeapons())
		{
			bool value = saveState.CollectedWeaponIds.Contains(weapon.Id);
			_availableWeaponsUnlockedStates[weapon.Id] = value;
		}
		foreach (ItemSO item in GameDatabaseHelper.GetItems())
		{
			bool value2 = saveState.CollectedItemIds.Contains(item.Id);
			_availableItemsUnlockedStates[item.Id] = value2;
		}
		foreach (EnemySO enemy in GameDatabaseHelper.GetEnemies())
		{
			bool value3 = saveState.CollectedEnemyIds.Contains(enemy.Id);
			_availableEnemiesUnlockedStates[enemy] = value3;
		}
		foreach (RelicSO relic in GameDatabaseHelper.GetRelics())
		{
			bool value4 = saveState.CollectedRelicIds.Contains(relic.Id);
			_availableRelicsUnlockedStates[relic.Id] = value4;
		}
		foreach (CharacterSO character in GameDatabaseHelper.GetCharacters())
		{
			bool value5 = saveState.CollectedCharacterIds.Contains(character.Id);
			_availableCharactersUnlockedStates[character] = value5;
		}
		foreach (BagSO bag in GameDatabaseHelper.GetBags())
		{
			bool value6 = saveState.CollectedBagIds.Contains(bag.Id);
			_availableBagsUnlockedStates[bag.Id] = value6;
		}
		foreach (MergableSO mergeRecipe in GameDatabaseHelper.GetMergeRecipes())
		{
			bool value7 = saveState.FoundRecipes.Contains(mergeRecipe.Id);
			_availableMergableUnlockedStates[mergeRecipe.Id] = value7;
		}
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Collection;
	}

	public override void Clear()
	{
		ClearDictionaries();
		InitDictionariesUncollected();
	}

	public override void ClearAdventure()
	{
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool AudioOnClose()
	{
		return true;
	}

	private void OnDestroy()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded -= SaveGameController_OnSaveGameLoaded;
		SingletonController<InputController>.Instance.OnCancelHandler -= Instance_OnCancelHandler;
		SingletonController<InputController>.Instance.OnNextHandler -= Instance_OnNextHandler;
		SingletonController<InputController>.Instance.OnPreviousHandler -= Instance_OnPreviousHandler;
	}

	internal int GetTotalAvailableUnlocks()
	{
		return _availableWeaponsUnlockedStates.Count() + _availableItemsUnlockedStates.Count() + _availableRelicsUnlockedStates.Count() + _availableEnemiesUnlockedStates.Count() + _availableBagsUnlockedStates.Count() + _availableMergableUnlockedStates.Count();
	}

	internal int GetTotalUnlockedCount()
	{
		return _availableWeaponsUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value) + _availableItemsUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value) + _availableRelicsUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value) + _availableEnemiesUnlockedStates.Count((KeyValuePair<EnemySO, bool> x) => x.Value) + _availableBagsUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value) + _availableMergableUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value);
	}
}
