using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Game.Events;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Game;

internal class RecalculateStatsTriggerController : SingletonController<RecalculateStatsTriggerController>
{
	private class TriggerInfo
	{
		internal float TriggerAmount { get; set; }

		internal int NumberOfTriggersRegistered { get; set; }

		internal TriggerInfo(float triggerAmount)
		{
			TriggerAmount = triggerAmount;
			NumberOfTriggersRegistered = 1;
		}

		internal void IncreaseRegistration()
		{
			NumberOfTriggersRegistered++;
		}

		internal void DecreaseRegistration()
		{
			NumberOfTriggersRegistered--;
		}
	}

	[SerializeField]
	private float _minimumTimeBetweenRecalculations;

	private Dictionary<Enums.CurrencyType, TriggerInfo> _currencyRecalculateTriggers = new Dictionary<Enums.CurrencyType, TriggerInfo>();

	private float _timeOfLastRecalculation;

	private void Start()
	{
		RegisterEvents();
		base.IsInitialized = true;
	}

	internal void RegisterConditions(ConditionSO[] conditions)
	{
		IEnumerable<ConditionSO> currencyRelatedConditions = conditions.Where((ConditionSO c) => c.TypeToCheckAgainst == Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount);
		RegisterCoinCurrencyConditions(currencyRelatedConditions);
	}

	internal void UnregisterConditions(ConditionSO[] conditions)
	{
		IEnumerable<ConditionSO> currencyRelatedConditions = conditions.Where((ConditionSO c) => c.TypeToCheckAgainst == Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount);
		UnregisterCoinCurrencyConditions(currencyRelatedConditions);
	}

	private void UnregisterCoinCurrencyConditions(IEnumerable<ConditionSO> currencyRelatedConditions)
	{
		foreach (ConditionSO currencyRelatedCondition in currencyRelatedConditions)
		{
			float checkAmount = currencyRelatedCondition.CheckAmount;
			UnregisterCurrencyTriggerAmount(Enums.CurrencyType.Coins, (int)checkAmount);
		}
	}

	private void RegisterCoinCurrencyConditions(IEnumerable<ConditionSO> currencyRelatedConditions)
	{
		foreach (ConditionSO currencyRelatedCondition in currencyRelatedConditions)
		{
			float checkAmount = currencyRelatedCondition.CheckAmount;
			RegisterCurrencyTriggerAmount(Enums.CurrencyType.Coins, (int)checkAmount);
		}
	}

	internal void RegisterCurrencyTriggerAmount(Enums.CurrencyType currencyType, int triggerAmount)
	{
		if (_currencyRecalculateTriggers.ContainsKey(currencyType))
		{
			_currencyRecalculateTriggers[currencyType].IncreaseRegistration();
			return;
		}
		TriggerInfo value = new TriggerInfo(triggerAmount);
		_currencyRecalculateTriggers.Add(currencyType, value);
	}

	internal void UnregisterCurrencyTriggerAmount(Enums.CurrencyType currencyType, int triggerAmount)
	{
		if (_currencyRecalculateTriggers.ContainsKey(currencyType))
		{
			_currencyRecalculateTriggers[currencyType].DecreaseRegistration();
			if (_currencyRecalculateTriggers[currencyType].NumberOfTriggersRegistered == 0)
			{
				_currencyRecalculateTriggers.Remove(currencyType);
			}
		}
	}

	private void RegisterCurrencyChangedEvent()
	{
		SingletonController<CurrencyController>.Instance.OnCurrencyChanged += CurrencyController_OnCurrencyChanged;
	}

	private void CurrencyController_OnCurrencyChanged(object sender, CurrencyChangedEventArgs e)
	{
		if (!SingletonController<InputController>.Instance.IsInPlayerInputMap || !_currencyRecalculateTriggers.ContainsKey(e.CurrencyType))
		{
			return;
		}
		foreach (KeyValuePair<Enums.CurrencyType, TriggerInfo> item in _currencyRecalculateTriggers.Where((KeyValuePair<Enums.CurrencyType, TriggerInfo> c) => c.Key == e.CurrencyType))
		{
			float triggerAmount = item.Value.TriggerAmount;
			if (TriggerAmountPassed(e.OldAmount, e.NewAmount, triggerAmount))
			{
				RecalculateStats();
				break;
			}
		}
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CurrencyController>.Instance, RegisterCurrencyChangedEvent);
	}

	private bool TriggerAmountPassed(float oldValue, float newValue, float triggerAmount)
	{
		bool num = oldValue < triggerAmount && newValue >= triggerAmount;
		bool flag = oldValue >= triggerAmount && newValue < triggerAmount;
		return num || flag;
	}

	private void RecalculateStats()
	{
		if (!StatsWereRecalculatedRecently())
		{
			SingletonCacheController.Instance.GetControllerByType<WeaponController>().RefreshWeapons();
			_timeOfLastRecalculation = Time.realtimeSinceStartup;
		}
	}

	private bool StatsWereRecalculatedRecently()
	{
		return Time.realtimeSinceStartup - _timeOfLastRecalculation < _minimumTimeBetweenRecalculations;
	}

	public override void Clear()
	{
		_currencyRecalculateTriggers.Clear();
	}

	public override void ClearAdventure()
	{
		_currencyRecalculateTriggers.Clear();
	}
}
