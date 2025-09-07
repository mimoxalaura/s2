using System.Collections.Generic;
using System.IO;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.ExternalStats;

internal static class ExternalEnemyStats
{
	private static bool _externalStatsLoaded;

	private static Dictionary<int, ExternalEnemyStat> _enemyStats = new Dictionary<int, ExternalEnemyStat>();

	internal static void ReloadStats()
	{
		_externalStatsLoaded = false;
	}

	internal static bool ExternalEnemyStatsAvailable()
	{
		return File.Exists(ExternalEnemyStatLoader.GetFilepath());
	}

	internal static Dictionary<Enums.ItemStatType, float> GetExternalEnemyStats(int enemyId, Dictionary<Enums.ItemStatType, float> originalDictionary)
	{
		if (!_externalStatsLoaded)
		{
			LoadExternalEnemyStats();
		}
		if (_enemyStats.ContainsKey(enemyId))
		{
			return _enemyStats[enemyId].EnemyStats;
		}
		return originalDictionary;
	}

	internal static DamageSO GetEnemyDamageSO(int enemyId, DamageSO originalDamageSO)
	{
		DamageSO damageSO = ScriptableObject.CreateInstance<DamageSO>();
		damageSO.BaseDamageType = originalDamageSO.BaseDamageType;
		damageSO.DamageCalculationType = originalDamageSO.DamageCalculationType;
		damageSO.WeaponPercentageUsed = originalDamageSO.WeaponPercentageUsed;
		damageSO.BaseMinDamage = _enemyStats[enemyId].MinDamage;
		damageSO.BaseMaxDamage = _enemyStats[enemyId].MaxDamage;
		return damageSO;
	}

	private static void LoadExternalEnemyStats()
	{
		_enemyStats = ExternalEnemyStatLoader.GetExternalEnemyStats();
	}
}
