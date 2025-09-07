using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class IceSpikeEffectHandler : WeaponEffectHandler
{
	[SerializeField]
	private IceSpikes _iceSpikePrefab;

	[SerializeField]
	private Vector2 _spawningPointOffset;

	private DamageInstance _damageInstance;

	private CombatWeapon _combatWeapon;

	private Character _source;

	private List<DebuffHandler> _originalWeaponAttackDebuffHandlers;

	public override void Activate(Transform position, Character target, CombatWeapon combatWeapon, Character source)
	{
		_source = source;
		_combatWeapon = combatWeapon;
		IceSpikes obj = Object.Instantiate(position: new Vector2(position.position.x + _spawningPointOffset.x, position.position.y + _spawningPointOffset.y), original: _iceSpikePrefab, rotation: Quaternion.identity);
		EffectsHelper.SetLingeringEffectBaseValues(obj, _source, target, Enums.LingeringEffectRotation.Default);
		obj.Init(_source, _combatWeapon);
		obj.OnLingeringEffectTriggered += IceSpike_OnLingeringEffectTriggered;
		obj.Activate();
	}

	private void IceSpike_OnLingeringEffectTriggered(object sender, LingeringEffectTriggeredEventArgs e)
	{
		float damage = DamageEngine.CalculateIndirectDamage(_source.CalculatedStats, e.TriggeredOn.CalculatedStats, _source.CalculatedDamageTypeValues, 1, _damageInstance, e.CombatWeapon.WeaponInstance.DamageInstance, e.TriggeredOn.GetCharacterType());
		e.TriggeredOn.Damage(damage, wasCrit: false, null, 0f, null, _damageInstance.CalculatedDamageType, _source);
		if (e.TriggeredOn.GetType() == typeof(Enemy))
		{
			((Enemy)e.TriggeredOn).Knockback(base.transform, 1f);
		}
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

	public override bool ShouldPreventSpawning(Vector2 position)
	{
		return false;
	}
}
