using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Game.Events;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Saving;
using UnityEngine;

namespace BackpackSurvivors.Game.Stastistics;

public class StatisticsController : SingletonController<StatisticsController>
{
	private Dictionary<int, int> _enemiesKilledPerEnemyId = new Dictionary<int, int>();

	private Dictionary<int, int> _completedAdventuresAndHellfireLevel = new Dictionary<int, int>();

	private Dictionary<Enums.SingleValueStatisticMetrics, int> _singleValueStatisticMetrics = new Dictionary<Enums.SingleValueStatisticMetrics, int>();

	private int _playedTimeInSeconds;

	private DateTime _lastPlayed;

	private int nextUpdate = 1;

	private float _startTick;

	private float _endTick;

	private Dictionary<int, int> _enemiesKilledPerEnemyIdInAdventure = new Dictionary<int, int>();

	private Dictionary<Enums.SingleValueStatisticMetrics, int> _singleValueStatisticMetricsInAdventure = new Dictionary<Enums.SingleValueStatisticMetrics, int>();

	public void ClearAdventureStatistics()
	{
		CopyRunStatisticsToGlobal();
		_startTick = 0f;
		_endTick = 0f;
		_enemiesKilledPerEnemyIdInAdventure.Clear();
		_singleValueStatisticMetricsInAdventure.Clear();
	}

	internal void ClearGlobalStatistics()
	{
		_completedAdventuresAndHellfireLevel.Clear();
		_enemiesKilledPerEnemyId.Clear();
		_singleValueStatisticMetrics.Clear();
		_playedTimeInSeconds = 0;
		_lastPlayed = DateTime.Now;
	}

	public StatisticsSaveState GetSaveState()
	{
		StatisticsSaveState statisticsSaveState = new StatisticsSaveState();
		statisticsSaveState.SetState(_enemiesKilledPerEnemyId, _singleValueStatisticMetrics, _playedTimeInSeconds, _lastPlayed, _completedAdventuresAndHellfireLevel);
		return statisticsSaveState;
	}

	private void Start()
	{
		RegisterEvents();
	}

	public void AddValue(Dictionary<Enums.SingleValueStatisticMetrics, int> targetDictionary, Enums.SingleValueStatisticMetrics singleValueStatisticMetrics, int value)
	{
		if (!targetDictionary.ContainsKey(singleValueStatisticMetrics))
		{
			targetDictionary.Add(singleValueStatisticMetrics, 0);
		}
		targetDictionary[singleValueStatisticMetrics] += value;
	}

	public void AddEnemyKilled(Dictionary<int, int> targetDictionary, int enemyId, int amount)
	{
		if (!targetDictionary.ContainsKey(enemyId))
		{
			targetDictionary.Add(enemyId, 0);
		}
		targetDictionary[enemyId] += amount;
	}

	public void SetAdventureCompleted(int adventureId, int hellfireLevel)
	{
		if (!_completedAdventuresAndHellfireLevel.ContainsKey(adventureId))
		{
			_completedAdventuresAndHellfireLevel.Add(adventureId, hellfireLevel);
		}
		else if (_completedAdventuresAndHellfireLevel[adventureId] < hellfireLevel)
		{
			_completedAdventuresAndHellfireLevel[adventureId] = hellfireLevel;
		}
	}

	public int HighestCompletedHellfireLevelForAdventure(int adventureId)
	{
		if (_completedAdventuresAndHellfireLevel.ContainsKey(adventureId))
		{
			return _completedAdventuresAndHellfireLevel[adventureId];
		}
		return -1;
	}

	public int GetAdventureMetricValue(Enums.SingleValueStatisticMetrics singleValueStatisticMetrics)
	{
		if (_singleValueStatisticMetricsInAdventure.ContainsKey(singleValueStatisticMetrics))
		{
			return _singleValueStatisticMetricsInAdventure[singleValueStatisticMetrics];
		}
		return 0;
	}

	public int GetGlobalMetricValue(Enums.SingleValueStatisticMetrics singleValueStatisticMetrics)
	{
		if (_singleValueStatisticMetrics.ContainsKey(singleValueStatisticMetrics))
		{
			return _singleValueStatisticMetrics[singleValueStatisticMetrics];
		}
		return 0;
	}

	private void EnemyController_OnEnemyKilled(object sender, EnemyKilledEventArgs e)
	{
		if (e.EnemyType == Enums.Enemies.EnemyType.Miniboss)
		{
			AddValue(_singleValueStatisticMetricsInAdventure, Enums.SingleValueStatisticMetrics.MiniBossesKilled, 1);
		}
		if (e.EnemyType == Enums.Enemies.EnemyType.Boss)
		{
			AddValue(_singleValueStatisticMetricsInAdventure, Enums.SingleValueStatisticMetrics.BossesKilled, 1);
		}
		AddEnemyKilled(_enemiesKilledPerEnemyIdInAdventure, e.EnemyId, 1);
	}

	private void AdventureCompletedController_OnAdventureCompleted(object sender, AdventureCompletedEventArgs e)
	{
		AddValue(_singleValueStatisticMetricsInAdventure, Enums.SingleValueStatisticMetrics.CompletedRuns, e.Success ? 1 : 0);
	}

	private void CurrencyController_OnCurrencyChanged(object sender, CurrencyChangedEventArgs e)
	{
		if (e.Source == Enums.CurrencySource.Drop && e.ChangedValue > 0 && e.CurrencyType == Enums.CurrencyType.Coins)
		{
			AddValue(_singleValueStatisticMetricsInAdventure, Enums.SingleValueStatisticMetrics.TotalCoinsLooted, e.ChangedValue);
		}
		if ((e.Source == Enums.CurrencySource.Drop || e.Source == Enums.CurrencySource.Reward) && e.ChangedValue > 0 && e.CurrencyType == Enums.CurrencyType.TitanSouls)
		{
			AddValue(_singleValueStatisticMetricsInAdventure, Enums.SingleValueStatisticMetrics.TotalTitanicSoulsLooted, e.ChangedValue);
		}
	}

	public override void Clear()
	{
		ClearAdventureStatistics();
		ClearGlobalStatistics();
	}

	public override void ClearAdventure()
	{
		ClearAdventureStatistics();
	}

	private void CopyRunStatisticsToGlobal()
	{
		foreach (KeyValuePair<int, int> item in _enemiesKilledPerEnemyIdInAdventure)
		{
			AddEnemyKilled(_enemiesKilledPerEnemyId, item.Key, item.Value);
		}
		foreach (KeyValuePair<Enums.SingleValueStatisticMetrics, int> item2 in _singleValueStatisticMetricsInAdventure)
		{
			AddValue(_singleValueStatisticMetrics, item2.Key, item2.Value);
		}
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<EnemyController>.Instance, RegisterEnemyControllerLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CurrencyController>.Instance, RegisterCurrencyControllerLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<AdventureCompletedController>.Instance, RegisterAdventureCompletedControllerLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, RegisterCharactersControllerLoaded);
	}

	public void RegisterSaveGameLoaded()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	public void RegisterCharactersControllerLoaded()
	{
		SingletonController<CharactersController>.Instance.OnExperienceGained += Instance_OnExperienceGained;
	}

	private void Instance_OnExperienceGained(object sender, ExperienceGainedEventArgs e)
	{
		AddValue(_singleValueStatisticMetrics, Enums.SingleValueStatisticMetrics.TotalExperienceGained, (int)e.ExperienceGained);
		AddValue(_singleValueStatisticMetricsInAdventure, Enums.SingleValueStatisticMetrics.TotalExperienceGained, (int)e.ExperienceGained);
	}

	public void RegisterEnemyControllerLoaded()
	{
		SingletonController<EnemyController>.Instance.OnEnemyKilled += EnemyController_OnEnemyKilled;
	}

	public void RegisterCurrencyControllerLoaded()
	{
		SingletonController<CurrencyController>.Instance.OnCurrencyChanged += CurrencyController_OnCurrencyChanged;
	}

	public void RegisterAdventureCompletedControllerLoaded()
	{
		SingletonController<AdventureCompletedController>.Instance.OnAdventureCompleted += AdventureCompletedController_OnAdventureCompleted;
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		LoadFromSave(e.SaveGame.StatisticsState);
		base.IsInitialized = true;
	}

	private void LoadFromSave(StatisticsSaveState statisticsState)
	{
		SaveFileController.LoadDictionary(_enemiesKilledPerEnemyId, statisticsState.EnemiesKilledPerEnemyId);
		SaveFileController.LoadDictionary(_singleValueStatisticMetrics, statisticsState.SingleValueStatisticMetrics);
		_playedTimeInSeconds = statisticsState.PlayedTime;
		_lastPlayed = statisticsState.LastPlayed;
		SaveFileController.LoadDictionary(_completedAdventuresAndHellfireLevel, statisticsState.CompletedAdventuresAndHellfireLevel);
	}

	private void Update()
	{
		if (Time.time >= (float)nextUpdate)
		{
			nextUpdate = Mathf.FloorToInt(Time.time) + 1;
			UpdatePlayedTime();
		}
	}

	private void UpdatePlayedTime()
	{
		_playedTimeInSeconds++;
	}

	internal int GetEnemiesKilledInAdventure()
	{
		int num = 0;
		foreach (KeyValuePair<int, int> item in _enemiesKilledPerEnemyIdInAdventure)
		{
			num += item.Value;
		}
		return num;
	}

	internal int GetEnemyKillCount(int enemyId)
	{
		if (_enemiesKilledPerEnemyId.ContainsKey(enemyId))
		{
			return _enemiesKilledPerEnemyId[enemyId];
		}
		return 0;
	}

	internal void SetStartTime()
	{
		_startTick = Time.time;
	}

	internal void SetEndTime()
	{
		_endTick = Time.time;
	}

	internal float GetAdventureDuration(bool useCurrentTime = false)
	{
		return (useCurrentTime ? Time.time : _endTick) - _startTick;
	}
}
