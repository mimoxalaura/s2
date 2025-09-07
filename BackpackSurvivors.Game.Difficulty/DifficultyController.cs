using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Difficulty;
using UnityEngine;

namespace BackpackSurvivors.Game.Difficulty;

public class DifficultyController : SingletonController<DifficultyController>
{
	public delegate void OnDifficultyChangedHandler(object sender, DifficultyChangedEventArgs e);

	private int _maxDifficulty = 9;

	public int ActiveDifficulty { get; private set; }

	public event OnDifficultyChangedHandler OnDifficultyChanged;

	private void Start()
	{
		base.IsInitialized = true;
		if (GameDatabase.IsDemo)
		{
			_maxDifficulty = SingletonController<GameDatabase>.Instance.GameDatabaseSO.MaxDifficulty;
		}
	}

	internal int GetMaxDifficulty()
	{
		return _maxDifficulty;
	}

	private LevelSO GetCurrentLevel()
	{
		return SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>().CurrentLevel;
	}

	public float GetCurrentLevelEnemyDamageMultiplierFromHellfire()
	{
		return GetEnemyDamageMultiplierFromHellfire(GetCurrentLevel());
	}

	public void ChangeDifficulty(int newDifficulty, bool save = false)
	{
		if (save)
		{
			ActiveDifficulty = newDifficulty;
		}
		this.OnDifficultyChanged?.Invoke(this, new DifficultyChangedEventArgs(newDifficulty));
	}

	internal Color GetDifficultyColor()
	{
		if (ActiveDifficulty == 0)
		{
			return Color.white;
		}
		return Color.red;
	}

	internal float GetEnemyHealthMultiplierFromHellfire(LevelSO level)
	{
		if (ActiveDifficulty == _maxDifficulty)
		{
			return (float)ActiveDifficulty * level.Adventure.MaxHellfireEnemyHealthMultiplier + 1f;
		}
		return (float)ActiveDifficulty * level.Adventure.HellfireEnemyHealthMultiplier + 1f;
	}

	internal float GetEnemyDamageMultiplierFromHellfire(LevelSO level)
	{
		if (ActiveDifficulty == _maxDifficulty)
		{
			return (float)ActiveDifficulty * level.Adventure.MaxHellfireEnemyDamageMultiplier + 1f;
		}
		return (float)ActiveDifficulty * level.Adventure.HellfireEnemyDamageMultiplier + 1f;
	}

	internal float GetEnemyCountMultiplierFromHellfire(LevelSO level)
	{
		if (ActiveDifficulty == _maxDifficulty)
		{
			return (float)ActiveDifficulty * level.Adventure.MaxHellfireEnemyCountMultiplier + 1f;
		}
		return (float)ActiveDifficulty * level.Adventure.HellfireEnemyCountMultiplier + 1f;
	}

	internal float GetRewardMultiplierFromHellfire(LevelSO level)
	{
		return GetRewardMultiplierFromHellfire(level.Adventure);
	}

	internal float GetRewardMultiplierFromHellfire(AdventureSO adventure)
	{
		if (ActiveDifficulty == _maxDifficulty)
		{
			return (float)ActiveDifficulty * adventure.MaxHellfireCoinMultiplier + 1f;
		}
		return (float)ActiveDifficulty * adventure.HellfireCoinMultiplier + 1f;
	}

	internal float GetExperienceMultiplierFromHellfire(LevelSO level)
	{
		return GetExperienceMultiplierFromHellfire(level.Adventure);
	}

	internal float GetExperienceMultiplierFromHellfire(AdventureSO adventure)
	{
		if (ActiveDifficulty == _maxDifficulty)
		{
			return (float)ActiveDifficulty * adventure.MaxHellfireExperienceMultiplier + 1f;
		}
		return (float)ActiveDifficulty * adventure.HellfireExperienceMultiplier + 1f;
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
