using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Effects.Core;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class PoisonPoolEffectHandler : WeaponEffectHandler
{
	[SerializeField]
	private PoisonPool _poisonPoolPrefab;

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
			PoisonPool poisonPool = Object.Instantiate(_poisonPoolPrefab, transform.position, Quaternion.identity);
			EffectsHelper.SetLingeringEffectBaseValues(poisonPool, _source, target, Enums.LingeringEffectRotation.Random);
			poisonPool.Init(_source, _combatWeapon);
			poisonPool.OnLingeringEffectTriggered += PoisonPool_OnLingeringEffectTriggered;
			poisonPool.Activate();
		}
	}

	public override bool ShouldPreventSpawning(Vector2 position)
	{
		return SingletonController<LingeringEffectsController>.Instance.ShouldPreventSpawning(_poisonPoolPrefab.LingeringEffectType, position, 1f);
	}

	private void PoisonPool_OnLingeringEffectTriggered(object sender, LingeringEffectTriggeredEventArgs e)
	{
		float damage = DamageEngine.CalculateIndirectDamage(_source.CalculatedStats, e.TriggeredOn.CalculatedStats, _source.CalculatedDamageTypeValues, 1, _damageInstance, e.CombatWeapon.WeaponInstance.DamageInstance, e.TriggeredOn.GetCharacterType());
		e.TriggeredOn.Damage(damage, wasCrit: false, null, 0f, null, _damageInstance.CalculatedDamageType, _source);
		foreach (DebuffHandler originalWeaponAttackDebuffHandler in _originalWeaponAttackDebuffHandlers)
		{
			DebuffHandler debuffHandler = EffectsHelper.CopyDebuffHandler(originalWeaponAttackDebuffHandler, e.CombatWeapon, e.TriggeredOn);
			e.TriggeredOn.AddDebuff(debuffHandler, _source);
		}
	}

	public override void Init(WeaponAttack originalWeaponAttack, List<DebuffHandler> debuffHandlers, WeaponInstance weaponInstance)
	{
		_originalWeaponAttackDebuffHandlers = debuffHandlers;
		_damageInstance = new DamageInstance(Damage);
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
