using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Debuffs;

public class ArcingDebuff : DebuffEffect
{
	[SerializeField]
	private WeaponAttack _weaponAttack;

	[SerializeField]
	private WeaponSO _weaponSO;

	public override void FellOff(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
	}

	public override void Trigger(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		if (!debuffedCharacter.IsEnemy)
		{
			return;
		}
		Enemy enemy = (Enemy)debuffedCharacter;
		if (enemy != null && enemy.isActiveAndEnabled)
		{
			CombatWeapon combatWeapon = Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab);
			combatWeapon.Init(new WeaponInstance(_weaponSO), 0f, debuffSourceCharacter);
			WeaponAttack weaponAttack = Object.Instantiate(_weaponAttack, enemy.transform.position, Quaternion.identity);
			if (damageInstance.DamageCalculationType == Enums.DamageCalculationType.BasedOnWeaponDamagePercentage)
			{
				float weaponPercentageUsed = damageInstance.WeaponPercentageUsed;
				damageInstance.SetMinMaxDamage(weaponDamageInstance.CalculatedMinDamage, weaponDamageInstance.CalculatedMaxDamage);
				damageInstance.ScaleDamage(weaponPercentageUsed);
			}
			weaponAttack.Init(debuffSourceCharacter, enemy, combatWeapon, damageInstance);
			weaponAttack.Activate(enemy.transform.position, canTriggerEffects: false, canTriggerDebuffs: false, debuffSourceCharacter, enemy);
		}
	}
}
