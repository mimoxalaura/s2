using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class StationaryAttackOnTrailEffectHandler : WeaponEffectHandler
{
	[SerializeField]
	private WeaponAttack _weaponAttackPrefab;

	private DamageInstance _damageInstance;

	private CombatWeapon _combatWeapon;

	private Character _source;

	private List<DebuffHandler> _originalWeaponAttackDebuffHandlers;

	public override void Activate(Transform transform, Character target, CombatWeapon combatWeapon, Character source)
	{
		if (!ShouldPreventSpawning(transform.position))
		{
			_source = source;
			_combatWeapon = combatWeapon;
			WeaponAttack weaponAttack = Object.Instantiate(_weaponAttackPrefab, transform.position, Quaternion.identity);
			weaponAttack.Init(_source, target, _combatWeapon, _damageInstance);
			weaponAttack.Activate(transform.position, canTriggerEffects: false, canTriggerDebuffs: true, _source, target);
		}
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
		_damageInstance.CalculatedDamageType = weaponInstance.DamageInstance.CalculatedDamageType;
	}

	public override string SetValuesForDescriptor(string placeholder)
	{
		string text = placeholder;
		if (_damageInstance != null)
		{
			text = text.Replace("{mindamage}", _damageInstance.CalculatedMinDamage.ToString());
			return text.Replace("{maxdamage}", _damageInstance.CalculatedMaxDamage.ToString());
		}
		text = text.Replace("{mindamage}", Damage.BaseMinDamage.ToString());
		return text.Replace("{maxdamage}", Damage.BaseMaxDamage.ToString());
	}
}
