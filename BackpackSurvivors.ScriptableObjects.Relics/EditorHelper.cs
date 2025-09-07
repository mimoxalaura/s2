using BackpackSurvivors.Game.Core;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Relics;

public static class EditorHelper
{
	private static GameDatabaseSO _gameDatabaseSO;

	public static Color GetRarityColor(Enums.PlaceableRarity rarity)
	{
		return Color.white;
	}
}
