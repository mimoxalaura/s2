using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class MultiShotEffectHandler : WeaponEffectHandler
{
	[SerializeField]
	private int _projectileCopyCount;

	[SerializeField]
	private float _projectileDamageModifier;

	private WeaponAttack _originalWeaponAttack;

	private CombatWeapon _combatWeapon;

	private Character _source;

	public override void Activate(Transform position, Character target, CombatWeapon combatWeapon, Character source)
	{
		if (combatWeapon.WeaponInstance.BaseWeaponType == Enums.WeaponType.Melee || ShouldPreventSpawning(base.transform.position))
		{
			return;
		}
		_source = source;
		_combatWeapon = combatWeapon;
		DamageInstance damageInstance = new DamageInstance(combatWeapon.WeaponInstance.DamageInstance.BaseDamage);
		damageInstance.CalculatedMinDamage *= _projectileDamageModifier;
		damageInstance.CalculatedMaxDamage *= _projectileDamageModifier;
		for (int i = 0; i < _projectileCopyCount; i++)
		{
			if (_originalWeaponAttack == null)
			{
				continue;
			}
			Enemy randomEnemyWithinRange = SingletonController<EnemyController>.Instance.GetRandomEnemyWithinRange(_originalWeaponAttack.transform.position, _combatWeapon.WeaponInstance.GetCalculatedStat(Enums.WeaponStatType.WeaponRange));
			if (!(randomEnemyWithinRange == null))
			{
				WeaponAttack weaponAttack = Object.Instantiate(combatWeapon.WeaponInstance.WeaponAttackPrefab, source.transform.position, Quaternion.identity);
				if (!(weaponAttack == null))
				{
					weaponAttack.transform.SetParent(null);
					weaponAttack.Init(source, randomEnemyWithinRange, _combatWeapon, damageInstance);
					weaponAttack.SetCustomStartPosition(position.position);
					weaponAttack.AddEnemiesToIgnoreOnHit(new List<Character> { target });
					weaponAttack.Activate(randomEnemyWithinRange.transform.position, canTriggerEffects: false, canTriggerDebuffs: true, _source, randomEnemyWithinRange);
				}
			}
		}
	}

	public override bool ShouldPreventSpawning(Vector2 position)
	{
		return false;
	}

	public override void Init(WeaponAttack originalWeaponAttack, List<DebuffHandler> debuffHandlers, WeaponInstance weaponInstance)
	{
		_originalWeaponAttack = originalWeaponAttack;
	}
}
