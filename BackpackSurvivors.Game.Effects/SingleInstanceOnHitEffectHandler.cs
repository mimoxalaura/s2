using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class SingleInstanceOnHitEffectHandler : WeaponEffectHandler
{
	[SerializeField]
	private WeaponAttack _explosionWeaponAttackPrefab;

	private DamageInstance _damageInstance;

	private List<DebuffHandler> _originalWeaponAttackDebuffHandlers;

	public override void Activate(Transform parentTransform, Character target, CombatWeapon combatWeapon, Character source)
	{
		if (!ShouldPreventSpawning(base.transform.position))
		{
			if (target == null)
			{
				target = SingletonController<EnemyController>.Instance.CreateAndGetDummyEnemy();
			}
			WeaponAttack weaponAttack = Object.Instantiate(_explosionWeaponAttackPrefab, parentTransform.position, default(Quaternion));
			weaponAttack.SetCustomStartPosition(parentTransform.position);
			GetDamageTypeModifiers(source);
			weaponAttack.Init(source, target, combatWeapon, _damageInstance);
			weaponAttack.Activate(parentTransform.position, canTriggerEffects: false, canTriggerDebuffs: true, source, target);
		}
	}

	private List<DamageTypeValueModifier> GetDamageTypeModifiers(Character source)
	{
		if (!(source is BackpackSurvivors.Game.Player.Player))
		{
			return new List<DamageTypeValueModifier>();
		}
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (KeyValuePair<Enums.DamageType, List<DamageTypeValueModifier>> item in ((BackpackSurvivors.Game.Player.Player)source).CalculatedDamageTypeValuesWithSource)
		{
			foreach (DamageTypeValueModifier item2 in item.Value)
			{
				list.Add(item2);
			}
		}
		return list;
	}

	public override bool ShouldPreventSpawning(Vector2 position)
	{
		return false;
	}

	public override void Init(WeaponAttack originalWeaponAttack, List<DebuffHandler> debuffHandlers, WeaponInstance weaponInstance)
	{
		_damageInstance = new DamageInstance(Damage);
		_originalWeaponAttackDebuffHandlers = debuffHandlers;
		_damageInstance.CalculatedMinDamage = weaponInstance.DamageInstance.CalculatedMinDamage * _damageInstance.WeaponPercentageUsed;
		_damageInstance.CalculatedMaxDamage = weaponInstance.DamageInstance.CalculatedMaxDamage * _damageInstance.WeaponPercentageUsed;
	}

	public override string SetValuesForDescriptor(string placeholder)
	{
		string text = placeholder;
		if (_damageInstance != null)
		{
			text = text.Replace("{mindamage}", _damageInstance.CalculatedMinDamage.ToString());
			text = text.Replace("{maxdamage}", _damageInstance.CalculatedMaxDamage.ToString());
			return text.Replace("{weaponType}", _damageInstance.CalculatedDamageType.ToString());
		}
		text = text.Replace("{mindamage}", Damage.BaseMinDamage.ToString());
		text = text.Replace("{maxdamage}", Damage.BaseMaxDamage.ToString());
		return text.Replace("{weaponType}", Damage.BaseDamageType.ToString());
	}
}
