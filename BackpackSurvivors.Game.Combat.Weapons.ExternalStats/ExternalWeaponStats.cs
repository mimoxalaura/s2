using System.Collections.Generic;
using System.IO;
using BackpackSurvivors.Combat.ScriptableObjects;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.Weapons.ExternalStats;

internal static class ExternalWeaponStats
{
	private static bool _externalStatsLoaded;

	private static Dictionary<int, ExternalWeaponStat> _weaponStats = new Dictionary<int, ExternalWeaponStat>();

	internal static void ReloadStats()
	{
		_externalStatsLoaded = false;
	}

	internal static bool ExternalWeaponStatsAvailable()
	{
		return File.Exists(ExternalWeaponStatLoader.GetFilepath());
	}

	internal static WeaponSO GetWeaponSO(int weaponId, WeaponSO originalWeaponSO)
	{
		if (!_externalStatsLoaded)
		{
			LoadExternalWeaponStats();
		}
		if (!_weaponStats.ContainsKey(weaponId))
		{
			return originalWeaponSO;
		}
		return GetExternalWeaponSO(weaponId, originalWeaponSO);
	}

	private static WeaponSO GetExternalWeaponSO(int weaponId, WeaponSO originalWeaponSO)
	{
		ExternalWeaponStat externalWeaponStat = _weaponStats[weaponId];
		WeaponSO weaponSO = Object.Instantiate(originalWeaponSO);
		weaponSO.Damage = GetWeaponSODamage(originalWeaponSO, externalWeaponStat);
		weaponSO.Stats = GetWeaponSOStats(originalWeaponSO, externalWeaponStat);
		weaponSO.ItemRarity = externalWeaponStat.Rarity;
		weaponSO.BuyingPrice = externalWeaponStat.Price;
		weaponSO.Name += "(ext)";
		return weaponSO;
	}

	private static DamageSO GetWeaponSODamage(WeaponSO originalWeaponSO, ExternalWeaponStat weapon)
	{
		DamageSO damageSO = Object.Instantiate(originalWeaponSO.Damage);
		damageSO.BaseMinDamage = weapon.MinDamage;
		damageSO.BaseMaxDamage = weapon.MaxDamage;
		return damageSO;
	}

	private static WeaponStatsSO GetWeaponSOStats(WeaponSO originalWeaponSO, ExternalWeaponStat weapon)
	{
		WeaponStatsSO weaponStatsSO = Object.Instantiate(originalWeaponSO.Stats);
		weaponStatsSO.StatValues[Enums.WeaponStatType.CritMultiplier] = weapon.CritMultiplier;
		weaponStatsSO.StatValues[Enums.WeaponStatType.CritChancePercentage] = weapon.CritChance;
		weaponStatsSO.StatValues[Enums.WeaponStatType.WeaponRange] = weapon.Range;
		weaponStatsSO.StatValues[Enums.WeaponStatType.CooldownTime] = weapon.Cooldown;
		weaponStatsSO.StatValues[Enums.WeaponStatType.Penetrating] = weapon.Piercing;
		return weaponStatsSO;
	}

	private static void LoadExternalWeaponStats()
	{
		_weaponStats = ExternalWeaponStatLoader.GetExternalWeaponStats();
	}
}
