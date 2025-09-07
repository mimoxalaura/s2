using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Backpack.Events;
using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.Game.Talents;
using BackpackSurvivors.Game.Talents.Events;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using BackpackSurvivors.UI;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using QFSW.QC;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Items;

public class StatController : BaseSingletonModalUIController<StatController>
{
	[SerializeField]
	private StatUI _statUI;

	[SerializeField]
	private Button _statUIToggleButton;

	[SerializeField]
	private Transform _statUIToggleButtonClosedLocation;

	[SerializeField]
	private Transform _statUIToggleButtonOpenLocation;

	[SerializeField]
	private bool _displayToggleButton;

	[SerializeField]
	private UITextPopup _uiTextPopupPrefab;

	private bool _previousDisplayToggleButton;

	private void Start()
	{
		RegisterEvents();
		StartCoroutine(UpdateTimeCoroutine());
	}

	private IEnumerator UpdateTimeCoroutine()
	{
		float interval = 0.1f;
		while (true)
		{
			if (_displayToggleButton != _previousDisplayToggleButton)
			{
				_previousDisplayToggleButton = _displayToggleButton;
				_statUIToggleButton.gameObject.SetActive(_displayToggleButton);
			}
			yield return new WaitForSeconds(interval);
		}
	}

	public void ToggleOpenWithSoloBackdrop(bool openWithSoloBackdrop)
	{
		_statUI.OpenWithSoloBackdrop = openWithSoloBackdrop;
	}

	public void ToggleOpenWithBuffsAndDebuffs(bool openWithBuffsAndDebuffs)
	{
		_statUI.OpenWithBuffsAndDebuffs = openWithBuffsAndDebuffs;
	}

	public override bool CloseOnCancelInput()
	{
		return false;
	}

	public void ClearUIRelics()
	{
		_statUI.ClearUIRelics();
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<RelicsController>.Instance, RegisterRelicAdded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<BackpackController>.Instance, RegisterDraggableDropped);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, RegisterCharacterLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<NewTalentController>.Instance, RegisterTalentsLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SceneChangeController>.Instance, RegisterSceneChangeLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<GameController>.Instance, RegisterGameControllerLoaded);
		StatCalculator.OnStatsRecalulated += StatCalculator_OnStatsRecalulated;
	}

	private void StatCalculator_OnStatsRecalulated()
	{
		UpdateStatUI();
	}

	public void RegisterSceneChangeLoaded()
	{
		SingletonController<SceneChangeController>.Instance.OnLevelSceneLoaded += SceneChangeController_OnLevelSceneLoaded;
	}

	public void RegisterGameControllerLoaded()
	{
		SingletonController<GameController>.Instance.Player.OnBuffApplied -= CharactersController_OnBuffApplied;
		SingletonController<GameController>.Instance.Player.OnBuffApplied += CharactersController_OnBuffApplied;
	}

	private void SceneChangeController_OnLevelSceneLoaded(object sender, LevelSceneLoadedEventArgs e)
	{
		_statUI.BuffVisualUIContainer.SetBuffController(SingletonController<GameController>.Instance.Player.GetComponent<BuffController>());
	}

	public void RegisterTalentsLoaded()
	{
		SingletonController<NewTalentController>.Instance.OnTalentSelected += TalentController_OnTalentSelected;
	}

	public void RegisterRelicAdded()
	{
		SingletonController<RelicsController>.Instance.OnRelicAdded += RelicsController_OnRelicAdded;
	}

	public void RegisterDraggableDropped()
	{
		SingletonController<BackpackController>.Instance.OnDraggableDropped += BackpackController_OnDraggableDropped;
		SingletonController<BackpackController>.Instance.OnDraggableDragging += BackpackController_OnDraggableDrag;
	}

	public void RegisterCharacterLoaded()
	{
		SingletonController<CharactersController>.Instance.OnCharacterLoaded += CharactersController_OnCharacterLoaded;
	}

	public void ToggleStatUI()
	{
		if (_statUI.IsOpen)
		{
			CloseUI();
		}
		else
		{
			OpenUI();
		}
	}

	public void UpdateStats()
	{
		GetAndRecalculateStats(healHealth: false);
	}

	public void SetDisplayToggleButton(bool showButton)
	{
		_displayToggleButton = showButton;
	}

	public void AnimateStat(Enums.ItemStatType itemStatType, string value, string colorString = "#ff0000", int numberOfBlinks = 3, float timeBetweenBlinks = 0.1f)
	{
		_statUI.AnimateStat(itemStatType, value, colorString, numberOfBlinks, timeBetweenBlinks);
	}

	public void ShowTextPopup(string text, Color color, float fadeOutTime = 0f)
	{
		Object.Instantiate(_uiTextPopupPrefab).Init(text, color, fadeOutTime);
	}

	public bool WeaponCapacityReached(bool addingWeapon = false)
	{
		SingletonController<BackpackController>.Instance.CountController.UpdateCounts();
		int num = SingletonController<BackpackController>.Instance.CountController.GetPlaceableTypeCount(Enums.PlaceableType.Weapon);
		float calculatedStat = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.WeaponCapacity);
		if (addingWeapon)
		{
			num++;
		}
		return (float)num > calculatedStat;
	}

	public void ShowCapacityReachedPopup()
	{
		AnimateStat(Enums.ItemStatType.WeaponCapacity, SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.WeaponCapacity).ToString());
		ShowTextPopup("Weapon capacity reached!", Color.red, 2.5f);
	}

	private void UpdateStatUI()
	{
		if (_statUI.Initialized && SingletonController<GameController>.Instance.Player.CalculatedStats != null && SingletonController<GameController>.Instance.Player.CalculatedStatsWithSource != null)
		{
			_statUI.UpdateStats(SingletonController<GameController>.Instance.Player.CalculatedStats, SingletonController<GameController>.Instance.Player.CalculatedStatsWithSource, SingletonController<GameController>.Instance.Player.BaseStats, SingletonController<GameController>.Instance.Player.CalculatedDamageTypeValues, SingletonController<GameController>.Instance.Player.CalculatedDamageTypeValuesWithSource, SingletonController<GameController>.Instance.Player.BaseDamageTypeValues);
		}
	}

	private void TalentController_OnTalentSelected(object sender, TalentSelectedEventArgs e)
	{
		GetAndRecalculateStats(healHealth: true);
	}

	private void CharactersController_OnCharacterLoaded(object sender, CharacterSwitchedEventArgs e)
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<GameController>.Instance.Player, UpdateStatsAfterCharacterLoaded);
	}

	private void UpdateStatsAfterCharacterLoaded()
	{
		GetAndRecalculateStats(healHealth: true);
	}

	private void CharactersController_OnBuffApplied(object sender, BuffAppliedEventArgs e)
	{
		GetAndRecalculateStats(healHealth: false);
	}

	private void BackpackController_OnDraggableDropped(object sender, DraggableDroppedEventArgs e)
	{
		GetAndRecalculateStats(healHealth: false);
	}

	private void BackpackController_OnDraggableDrag(object sender, DraggableDraggingEventArgs e)
	{
	}

	private void RelicsController_OnRelicAdded(object sender, RelicAddedEventArgs e)
	{
		GetAndRecalculateStats(healHealth: false);
		_statUI.RelicAdded(e.Relic.RelicSO);
	}

	public void RecalculateStats(Dictionary<Enums.ItemStatType, List<ItemStatModifier>> basePlayerStats, List<WeaponInstance> weaponsInBackpack, List<ItemInstance> itemsInBackpack, out Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats, out Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> damageTypeValues, List<RelicSO> activeRelics, List<TalentSO> activeTalents, Dictionary<Enums.ItemStatType, float> playerStatBuffs, Dictionary<Enums.DamageType, float> baseDamageTypeValues, List<WeaponInstance> weaponsInStorage, List<ItemInstance> itemsInStorage, List<WeaponInstance> weaponsInShop, List<ItemInstance> itemsInShop)
	{
		StatCalculator.RecalculateStats(basePlayerStats, weaponsInBackpack, itemsInBackpack, out playerStats, out damageTypeValues, activeRelics, activeTalents, GetWeaponsAndItemStarSources(weaponsInBackpack), GetWeaponsAndWeaponStarSources(weaponsInBackpack), GetWeaponsAndContainingBags(weaponsInBackpack), playerStatBuffs, SingletonController<BackpackController>.Instance.CountController, baseDamageTypeValues, weaponsInStorage, itemsInStorage, weaponsInShop, itemsInShop);
	}

	private Dictionary<WeaponInstance, List<BagInstance>> GetWeaponsAndContainingBags(List<WeaponInstance> weaponsInBackpack)
	{
		return SingletonController<BackpackController>.Instance.BackpackStorage.GetWeaponsAndContainingBags(weaponsInBackpack);
	}

	private static Dictionary<WeaponInstance, List<ItemInstance>> GetWeaponsAndItemStarSources(List<WeaponInstance> weapons)
	{
		Dictionary<WeaponInstance, List<ItemInstance>> dictionary = new Dictionary<WeaponInstance, List<ItemInstance>>();
		foreach (WeaponInstance weapon in weapons)
		{
			dictionary.Add(weapon, new List<ItemInstance>());
			foreach (ItemInstance item in SingletonController<BackpackController>.Instance.BackpackStorage.GetItemsStarringWeapon(weapon))
			{
				dictionary[weapon].Add(item);
			}
		}
		return dictionary;
	}

	private static Dictionary<WeaponInstance, List<WeaponInstance>> GetWeaponsAndWeaponStarSources(List<WeaponInstance> weapons)
	{
		Dictionary<WeaponInstance, List<WeaponInstance>> dictionary = new Dictionary<WeaponInstance, List<WeaponInstance>>();
		foreach (WeaponInstance weapon in weapons)
		{
			dictionary.Add(weapon, new List<WeaponInstance>());
			foreach (WeaponInstance item in SingletonController<BackpackController>.Instance.BackpackStorage.GetWeaponsStarringWeapon(weapon))
			{
				dictionary[weapon].Add(item);
			}
		}
		return dictionary;
	}

	[Command("stats.open", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		base.OpenUI();
		_statUI.gameObject.SetActive(value: true);
		GetAndRecalculateStats(healHealth: false);
		if (!_statUI.Initialized)
		{
			_statUI.InitStats(SingletonController<GameController>.Instance.Player.CalculatedStats, SingletonController<GameController>.Instance.Player.CalculatedStatsWithSource, SingletonController<GameController>.Instance.Player.CalculatedDamageTypeValues, SingletonController<GameController>.Instance.Player.CalculatedDamageTypeValuesWithSource);
		}
		UpdateStatUI();
		_statUI.OpenUI(Enums.Modal.OpenDirection.Horizontal);
		LeanTween.moveX(_statUIToggleButton.gameObject, _statUIToggleButtonOpenLocation.position.x, 0.3f);
	}

	[Command("stats.recalculate", Platform.AllPlatforms, MonoTargetType.Single)]
	public void GetAndRecalculateStats(bool healHealth)
	{
		if (SingletonController<GameController>.Instance.Player.IsInitialized)
		{
			RecalculateStats(SingletonController<GameController>.Instance.Player.GetComplexBaseStats(), GetWeaponsFromBackpack(), GetItemsFromBackpack(), out var playerStats, out var damageTypeValues, GetRelicsFromBackpack(), GetTalents(), SingletonController<GameController>.Instance.Player.BuffStats, SingletonController<GameController>.Instance.Player.BaseDamageTypeValues, GetWeaponsFromStorage(), GetItemsFromStorage(), GetWeaponsFromShop(), GetItemsFromShop());
			BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
			player.UpdateStats(playerStats, healHealth);
			player.UpdateDamageTypeValues(damageTypeValues);
			UpdateStatUI();
		}
	}

	[Command("stats.close", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void CloseUI()
	{
		base.CloseUI();
		_statUI.CloseUI(Enums.Modal.OpenDirection.Horizontal);
		LeanTween.moveX(_statUIToggleButton.gameObject, _statUIToggleButtonClosedLocation.position.x, 0.1f);
	}

	private List<WeaponInstance> GetWeaponsFromBackpack()
	{
		return SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack();
	}

	private List<ItemInstance> GetItemsFromBackpack()
	{
		return SingletonController<BackpackController>.Instance.GetItemsFromBackpack();
	}

	private List<WeaponInstance> GetWeaponsFromStorage()
	{
		return SingletonController<BackpackController>.Instance.GetWeaponsFromStorage();
	}

	private List<ItemInstance> GetItemsFromStorage()
	{
		return SingletonController<BackpackController>.Instance.GetItemsFromStorage();
	}

	private List<WeaponInstance> GetWeaponsFromShop()
	{
		ShopController controllerByType = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		if (controllerByType == null)
		{
			return new List<WeaponInstance>();
		}
		return controllerByType.GetWeaponsFromShop();
	}

	private List<ItemInstance> GetItemsFromShop()
	{
		ShopController controllerByType = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		if (controllerByType == null)
		{
			return new List<ItemInstance>();
		}
		return controllerByType.GetItemsFromShop();
	}

	private List<RelicSO> GetRelicsFromBackpack()
	{
		return SingletonController<RelicsController>.Instance.ActiveRelics.Select((BackpackSurvivors.Game.Relic.Relic x) => x.RelicSO).ToList();
	}

	private List<TalentSO> GetTalents()
	{
		return SingletonController<NewTalentController>.Instance.GetActiveTalents();
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Stats;
	}

	public override void Clear()
	{
		_statUI.ClearBuffs();
	}

	public override void ClearAdventure()
	{
		_statUI.ClearBuffs();
	}

	public override bool AudioOnOpen()
	{
		return false;
	}

	public override bool AudioOnClose()
	{
		return false;
	}
}
