using System.Collections.Generic;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat;

public static class DamageEngine
{
	public static float CalculateIndirectDamage(Dictionary<Enums.ItemStatType, float> attackSourceCharacterCalculatedStats, Dictionary<Enums.ItemStatType, float> attackTargetCharacterCalculatedStats, Dictionary<Enums.DamageType, float> attackSourceCharacterDamageTypeValues, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.Enemies.EnemyType enemyType)
	{
		float baseValue = 0f;
		if (damageInstance.DamageCalculationType == Enums.DamageCalculationType.Default)
		{
			baseValue = Random.Range(damageInstance.CalculatedMinDamage, damageInstance.CalculatedMaxDamage);
		}
		else if (damageInstance.DamageCalculationType == Enums.DamageCalculationType.BasedOnWeaponDamagePercentage)
		{
			if (weaponDamageInstance == null)
			{
				return 0f;
			}
			float minInclusive = weaponDamageInstance.CalculatedMinDamage * damageInstance.WeaponPercentageUsed;
			float calculatedMaxDamage = weaponDamageInstance.CalculatedMaxDamage;
			calculatedMaxDamage *= damageInstance.WeaponPercentageUsed;
			baseValue = Random.Range(minInclusive, calculatedMaxDamage);
			baseValue = Mathf.Clamp(baseValue, 1f, 9999f);
		}
		baseValue = AddElementalDamageFromPlayer(baseValue, damageInstance, attackSourceCharacterDamageTypeValues);
		baseValue = GetReduceBasedOnEnemyType(enemyType, attackTargetCharacterCalculatedStats, baseValue);
		return baseValue * (float)stacks;
	}

	public static float CalculateDamage(Dictionary<Enums.WeaponStatType, float> damageStats, Dictionary<Enums.ItemStatType, float> attackSourceCharacterCalculatedStats, Dictionary<Enums.ItemStatType, float> attackTargetCharacterCalculatedStats, DamageInstance damageInstance, Enums.Enemies.EnemyType enemyType, out bool wasCrit)
	{
		float calculatedMinDamage = damageInstance.CalculatedMinDamage;
		float calculatedMaxDamage = damageInstance.CalculatedMaxDamage;
		AddOrReduceBasedOnTargetArmor(attackTargetCharacterCalculatedStats, ref calculatedMinDamage, ref calculatedMaxDamage);
		AddOrReduceBasedOnEnemyType(enemyType, attackSourceCharacterCalculatedStats, ref calculatedMinDamage, ref calculatedMaxDamage);
		float calculatedDamage = Random.Range(calculatedMinDamage, calculatedMaxDamage);
		wasCrit = CalculateIfCrit(damageStats);
		return AddCritDamage(damageStats, wasCrit, calculatedDamage);
	}

	private static void AddOrReduceBasedOnEnemyType(Enums.Enemies.EnemyType enemyType, Dictionary<Enums.ItemStatType, float> attackSourceCharacterCalculatedStats, ref float calculatedMinDamage, ref float calculatedMaxDamage)
	{
		switch (enemyType)
		{
		case Enums.Enemies.EnemyType.Player:
			return;
		case Enums.Enemies.EnemyType.Monster:
		case Enums.Enemies.EnemyType.Minion:
		{
			float num = attackSourceCharacterCalculatedStats.TryGet(Enums.ItemStatType.DamageAgainstNormalEnemies, 1f);
			calculatedMinDamage *= num;
			calculatedMaxDamage *= num;
			break;
		}
		}
		if (enemyType == Enums.Enemies.EnemyType.Elite || enemyType == Enums.Enemies.EnemyType.Miniboss || enemyType == Enums.Enemies.EnemyType.Miniboss)
		{
			float num2 = attackSourceCharacterCalculatedStats.TryGet(Enums.ItemStatType.DamageAgainstEliteAndBossEnemies, 1f);
			calculatedMinDamage *= num2;
			calculatedMaxDamage *= num2;
		}
	}

	private static float GetReduceBasedOnEnemyType(Enums.Enemies.EnemyType enemyType, Dictionary<Enums.ItemStatType, float> attackSourceCharacterCalculatedStats, float calculatedDamage)
	{
		switch (enemyType)
		{
		case Enums.Enemies.EnemyType.Monster:
		case Enums.Enemies.EnemyType.Minion:
		{
			float num2 = attackSourceCharacterCalculatedStats.TryGet(Enums.ItemStatType.DamageAgainstNormalEnemies, 1f);
			return calculatedDamage * num2;
		}
		default:
			if (enemyType != Enums.Enemies.EnemyType.Miniboss)
			{
				return calculatedDamage;
			}
			goto case Enums.Enemies.EnemyType.Elite;
		case Enums.Enemies.EnemyType.Elite:
		case Enums.Enemies.EnemyType.Miniboss:
		{
			float num = attackSourceCharacterCalculatedStats.TryGet(Enums.ItemStatType.DamageAgainstEliteAndBossEnemies, 1f);
			return calculatedDamage * num;
		}
		}
	}

	private static void AddOrReduceBasedOnTargetArmor(Dictionary<Enums.ItemStatType, float> attackTargetCharacterCalculatedStats, ref float calculatedMinDamage, ref float calculatedMaxDamage)
	{
		if (attackTargetCharacterCalculatedStats.ContainsKey(Enums.ItemStatType.Armor))
		{
			float totalPercentageOfDamageDoneVsArmor = GetTotalPercentageOfDamageDoneVsArmor(attackTargetCharacterCalculatedStats[Enums.ItemStatType.Armor]);
			calculatedMinDamage *= totalPercentageOfDamageDoneVsArmor;
			calculatedMaxDamage *= totalPercentageOfDamageDoneVsArmor;
		}
	}

	public static float GetTotalPercentageOfDamageDoneVsArmor(float armor)
	{
		if (!(armor >= 0f))
		{
			return 2f - GetPercentageOfDamageDoneVsArmor(Mathf.Abs(armor));
		}
		return GetPercentageOfDamageDoneVsArmor(armor);
	}

	private static float GetPercentageOfDamageDoneVsArmor(float armor)
	{
		return 1f / (1f + armor / 50f);
	}

	private static float AddCritDamage(Dictionary<Enums.WeaponStatType, float> damageStats, bool wasCrit, float calculatedDamage)
	{
		float num = damageStats.TryGet(Enums.WeaponStatType.CritMultiplier, 1f);
		if (wasCrit)
		{
			calculatedDamage *= num;
		}
		return calculatedDamage;
	}

	public static bool CalculateIfCrit(Dictionary<Enums.WeaponStatType, float> damageStats)
	{
		return damageStats.TryGet(Enums.WeaponStatType.CritChancePercentage, 0f) > Random.Range(0f, 1f);
	}

	public static float AddElementalDamageFromPlayer(float baseValue, DamageInstance damageInstance, Dictionary<Enums.DamageType, float> attackSourceCharacterDamageTypeValues)
	{
		float result = baseValue;
		Enums.DamageType calculatedDamageType = damageInstance.CalculatedDamageType;
		if (attackSourceCharacterDamageTypeValues.ContainsKey(calculatedDamageType))
		{
			result = baseValue * attackSourceCharacterDamageTypeValues[calculatedDamageType];
		}
		return result;
	}

	public static float ReduceDamageFromTargetArmor(float baseDamage, Dictionary<Enums.ItemStatType, float> attackTargetCharacterCalculatedStats)
	{
		float num = attackTargetCharacterCalculatedStats.TryGet(Enums.ItemStatType.Armor, 0f);
		return Mathf.Clamp(baseDamage - num, 0f, float.MaxValue);
	}
}
