using System.Collections.Generic;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Game;

internal class GameState
{
	private AdventureSO _currentAdventure;

	private int _currentLevelIndex;

	internal List<Enums.PlaceableRarity> UnlockedItemRarities { get; private set; }

	internal string AdventureName => _currentAdventure.AdventureName;

	internal bool IsFinished { get; private set; }

	internal float StoredPlayerHealth { get; private set; }

	internal GameState(AdventureSO adventure)
	{
		_currentAdventure = adventure;
		UnlockedItemRarities = new List<Enums.PlaceableRarity> { Enums.PlaceableRarity.Common };
	}

	internal void FinishGame()
	{
		IsFinished = true;
		StoredPlayerHealth = 0f;
	}

	internal void ClearState()
	{
		UnlockedItemRarities = new List<Enums.PlaceableRarity> { Enums.PlaceableRarity.Common };
	}

	internal void UnlockItemRarity(Enums.PlaceableRarity itemRarity)
	{
		if (!UnlockedItemRarities.Contains(itemRarity))
		{
			UnlockedItemRarities.Add(itemRarity);
		}
	}

	internal LevelSO GetCurrentLevel()
	{
		return _currentAdventure.Levels[_currentLevelIndex];
	}

	internal void AdvanceLevel()
	{
		_currentLevelIndex++;
		StorePlayerHealth();
	}

	internal void StorePlayerHealth()
	{
		StoredPlayerHealth = SingletonController<GameController>.Instance.Player.HealthSystem.GetHealth();
	}

	internal bool HasLevelRemaining()
	{
		return _currentAdventure.Levels.Count > _currentLevelIndex + 1;
	}
}
