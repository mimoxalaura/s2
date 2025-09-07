using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Game.Events;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Game;

public class CurrencyController : SingletonController<CurrencyController>
{
	internal delegate void CurrencyChangedHandler(object sender, CurrencyChangedEventArgs e);

	internal delegate void CraftingResourceChangedHandler(object sender, CraftingResourceChangedEventArgs e);

	internal delegate void CannotAffordCurrencyHandler(object sender, CannotAffordCurrencyEventArgs e);

	[SerializeField]
	private SerializableDictionaryBase<Enums.CurrencyType, AudioClip> _currenyGainedAudioClips;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CurrencyType, AudioClip> _currenyLostAudioClips;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CurrencyType, int> _maxCurrencyValue;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CraftingResource, int> _maxCraftingResourceValue;

	private Dictionary<Enums.CurrencyType, int> _currencies = new Dictionary<Enums.CurrencyType, int>();

	private Dictionary<Enums.CraftingResource, int> _craftingResources = new Dictionary<Enums.CraftingResource, int>();

	public float bundleInterval = 0.5f;

	private int pendingCurrency;

	private bool isProcessing;

	internal event CurrencyChangedHandler OnCurrencyChanged;

	internal event CraftingResourceChangedHandler OnCraftingResourceChanged;

	internal event CannotAffordCurrencyHandler OnCannotAffordCurrency;

	internal CurrencySaveState GetSaveState()
	{
		CurrencySaveState currencySaveState = new CurrencySaveState();
		currencySaveState.SetState(new Dictionary<Enums.CurrencyType, int> { 
		{
			Enums.CurrencyType.TitanSouls,
			_currencies[Enums.CurrencyType.TitanSouls]
		} }, _craftingResources);
		return currencySaveState;
	}

	private void Start()
	{
		RegisterEvents();
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameLoaded);
	}

	internal void RegisterSaveGameLoaded()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		SaveGame saveGame = e.SaveGame;
		saveGame.CurrencyState.ValidateAndFix();
		foreach (Enums.CurrencyType value in Enum.GetValues(typeof(Enums.CurrencyType)))
		{
			if (saveGame.CurrencyState.MetaProgressionCurrencies.ContainsKey(value))
			{
				SetCurrency(value, saveGame.CurrencyState.MetaProgressionCurrencies[value]);
			}
			else
			{
				SetCurrency(value, 0);
			}
		}
		foreach (Enums.CraftingResource value2 in Enum.GetValues(typeof(Enums.CraftingResource)))
		{
			if (saveGame.CurrencyState.CraftingResources.ContainsKey(value2))
			{
				SetCraftingResource(value2, saveGame.CurrencyState.CraftingResources[value2]);
			}
			else
			{
				SetCraftingResource(value2, 0);
			}
		}
		base.IsInitialized = true;
	}

	[Command("currency.give", Platform.AllPlatforms, MonoTargetType.Single)]
	public void CHEAT_GetRich()
	{
		GainCurrency(Enums.CurrencyType.Coins, 9999, Enums.CurrencySource.Drop);
		GainCurrency(Enums.CurrencyType.TitanSouls, 9999, Enums.CurrencySource.Drop);
	}

	[Command("currency.gain-currency", Platform.AllPlatforms, MonoTargetType.Single)]
	public void GainCurrency(Enums.CurrencyType currencyType, int numberOfCurrency, Enums.CurrencySource source)
	{
		if (currencyType == Enums.CurrencyType.Coins)
		{
			pendingCurrency += numberOfCurrency;
			if (!isProcessing)
			{
				StartCoroutine(ProcessCurrency(source));
			}
		}
		else
		{
			HandleCurrencyGain(numberOfCurrency, currencyType, source);
		}
	}

	private IEnumerator ProcessCurrency(Enums.CurrencySource source)
	{
		isProcessing = true;
		yield return new WaitForSecondsRealtime(bundleInterval);
		if (pendingCurrency > 0)
		{
			HandleCurrencyGain(pendingCurrency, Enums.CurrencyType.Coins, source);
			pendingCurrency = 0;
		}
		isProcessing = false;
	}

	private void HandleCurrencyGain(int amount, Enums.CurrencyType currencyType, Enums.CurrencySource source)
	{
		int oldAmount = _currencies[currencyType];
		SingletonController<AudioController>.Instance.PlaySFXClip(_currenyGainedAudioClips[currencyType], 1f, 0f, AudioController.GetPitchVariation());
		_currencies[currencyType] += amount;
		ResolveMaxCurrency(currencyType);
		this.OnCurrencyChanged?.Invoke(this, new CurrencyChangedEventArgs(currencyType, GetCurrency(currencyType), oldAmount, amount, source));
	}

	[Command("craftingResource.gain", Platform.AllPlatforms, MonoTargetType.Single)]
	public void GainCraftingResource(Enums.CraftingResource craftingResource, int numberOfCraftingResource, Enums.CurrencySource source)
	{
		int oldAmount = _craftingResources[craftingResource];
		_craftingResources[craftingResource] += numberOfCraftingResource;
		ResolveMaxCraftingResource(craftingResource);
		this.OnCraftingResourceChanged?.Invoke(this, new CraftingResourceChangedEventArgs(craftingResource, GetCraftingResource(craftingResource), oldAmount, numberOfCraftingResource, source));
	}

	private void ResolveMaxCurrency(Enums.CurrencyType currencyType)
	{
		if (_maxCurrencyValue.ContainsKey(currencyType) && _currencies[currencyType] > _maxCurrencyValue[currencyType])
		{
			_currencies[currencyType] = _maxCurrencyValue[currencyType];
		}
	}

	private void ResolveMaxCraftingResource(Enums.CraftingResource craftingResource)
	{
		if (_maxCraftingResourceValue.ContainsKey(craftingResource) && _craftingResources[craftingResource] > _maxCraftingResourceValue[craftingResource])
		{
			_craftingResources[craftingResource] = _maxCraftingResourceValue[craftingResource];
		}
	}

	[Command("currency.lose-currency", Platform.AllPlatforms, MonoTargetType.Single)]
	internal bool TrySpendCurrency(Enums.CurrencyType currencyType, int numberOfCurrency)
	{
		if (!CanAfford(currencyType, numberOfCurrency))
		{
			return false;
		}
		if (_currenyLostAudioClips.ContainsKey(currencyType))
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(_currenyLostAudioClips[currencyType], 1f);
		}
		int oldAmount = _currencies[currencyType];
		_currencies[currencyType] -= numberOfCurrency;
		this.OnCurrencyChanged?.Invoke(this, new CurrencyChangedEventArgs(currencyType, GetCurrency(currencyType), oldAmount, -numberOfCurrency, Enums.CurrencySource.Shop));
		return true;
	}

	[Command("craftingResource.lose", Platform.AllPlatforms, MonoTargetType.Single)]
	internal bool TrySpendCraftingResource(Enums.CraftingResource craftingResource, int numberOfCraftingResource)
	{
		if (!CanAfford(craftingResource, numberOfCraftingResource))
		{
			return false;
		}
		int oldAmount = _craftingResources[craftingResource];
		_craftingResources[craftingResource] -= numberOfCraftingResource;
		this.OnCraftingResourceChanged?.Invoke(this, new CraftingResourceChangedEventArgs(craftingResource, GetCraftingResource(craftingResource), oldAmount, -numberOfCraftingResource, Enums.CurrencySource.Shop));
		return true;
	}

	internal void SetCurrency(Enums.CurrencyType currencyType, int numberOfCurrency)
	{
		int oldAmount = _currencies[currencyType];
		_currencies[currencyType] = numberOfCurrency;
		ResolveMaxCurrency(currencyType);
		this.OnCurrencyChanged?.Invoke(this, new CurrencyChangedEventArgs(currencyType, GetCurrency(currencyType), oldAmount, numberOfCurrency, Enums.CurrencySource.PrerunSetup));
	}

	internal void SetCraftingResource(Enums.CraftingResource craftingResource, int numberOfCraftingResource)
	{
		int oldAmount = _craftingResources[craftingResource];
		_craftingResources[craftingResource] = numberOfCraftingResource;
		ResolveMaxCraftingResource(craftingResource);
		this.OnCraftingResourceChanged?.Invoke(this, new CraftingResourceChangedEventArgs(craftingResource, GetCraftingResource(craftingResource), oldAmount, numberOfCraftingResource, Enums.CurrencySource.PrerunSetup));
	}

	internal bool CanAfford(Enums.CurrencyType currencyType, int cost, bool showCannotAffordUI = false)
	{
		bool num = cost <= _currencies[currencyType];
		if (!num && showCannotAffordUI)
		{
			CannotAffordCurrencyHandler cannotAffordCurrencyHandler = this.OnCannotAffordCurrency;
			if (cannotAffordCurrencyHandler == null)
			{
				return num;
			}
			cannotAffordCurrencyHandler(this, new CannotAffordCurrencyEventArgs(currencyType, cost));
		}
		return num;
	}

	internal bool CanAfford(Enums.CraftingResource craftingResource, int cost, bool showCannotAffordUI = false)
	{
		return cost <= _craftingResources[craftingResource];
	}

	internal int GetCurrency(Enums.CurrencyType currencyType)
	{
		return _currencies[currencyType];
	}

	internal int GetCraftingResource(Enums.CraftingResource craftingResources)
	{
		return _craftingResources[craftingResources];
	}

	private void InitCurrencies()
	{
		_currencies.Add(Enums.CurrencyType.Coins, 0);
		_currencies.Add(Enums.CurrencyType.TitanSouls, 0);
	}

	private void InitCraftingResources()
	{
		_craftingResources = new Dictionary<Enums.CraftingResource, int>();
		foreach (Enums.CraftingResource value in Enum.GetValues(typeof(Enums.CraftingResource)))
		{
			_craftingResources.Add(value, 0);
		}
	}

	public override void AfterBaseAwake()
	{
		InitCurrencies();
		InitCraftingResources();
	}

	public override void Clear()
	{
		_currencies.Clear();
		InitCurrencies();
		InitCraftingResources();
	}

	public override void ClearAdventure()
	{
		SetCurrency(Enums.CurrencyType.Coins, 0);
	}
}
