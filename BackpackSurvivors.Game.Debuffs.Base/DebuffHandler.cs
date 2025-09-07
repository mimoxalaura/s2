using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Debuffs.Base;

public class DebuffHandler
{
	private float _lastTrigger;

	private int _debuffStacks = 1;

	private float _totalDeltaTime;

	private Character _debuffedCharacter;

	private Character _sourceCharacter;

	private Enums.DamageType _combatWeaponDamageType;

	public Enums.Debuff.DebuffType DebuffType => DebuffSO.DebuffType;

	public Enums.Debuff.DebuffFalloffTimeType DebuffFalloffTimeType => DebuffSO.DebuffFalloffTimeType;

	public float TimeUntillFalloff => DebuffSO.TimeUntillFalloff;

	public int DebuffStacks => _debuffStacks;

	public float TimeAlive { get; private set; }

	public bool CanBeDestroyed { get; private set; }

	public int TriggerCount { get; private set; }

	public DebuffSO DebuffSO { get; private set; }

	public DamageInstance DamageInstance { get; private set; }

	public CombatWeapon CombatWeapon { get; private set; }

	public DebuffEffect DebuffEffect { get; private set; }

	public void Init(DebuffSO debuffSO, CombatWeapon combatWeapon, Character enemyHit)
	{
		Init(debuffSO, enemyHit);
		CombatWeapon = combatWeapon;
		_combatWeaponDamageType = CombatWeapon.WeaponInstance.DamageInstance.CalculatedDamageType;
		if (DamageInstance != null)
		{
			DamageInstance.CalculatedMinDamage = debuffSO.DamageSO.BaseMinDamage;
			DamageInstance.CalculatedMaxDamage = debuffSO.DamageSO.BaseMaxDamage;
		}
	}

	public void Init(DebuffSO debuffSO, Character enemyHit)
	{
		Init(debuffSO);
		DebuffEffect = DebuffSO.DebuffEffect;
	}

	public void Init(DebuffSO debuffSO)
	{
		DebuffSO = debuffSO;
		TimeAlive = 0f;
		if (debuffSO.DamageSO != null)
		{
			DamageInstance = new DamageInstance(debuffSO.DamageSO);
		}
	}

	public void Deactivate()
	{
		if (DebuffEffect != null)
		{
			DebuffEffect.FellOff(_debuffedCharacter, _sourceCharacter, _debuffStacks, DamageInstance, CombatWeapon.WeaponInstance.DamageInstance, _combatWeaponDamageType);
		}
		CanBeDestroyed = true;
	}

	public void ResetTime()
	{
		TimeAlive = 0f;
	}

	public void AddStack()
	{
		if (DebuffSO.MaxStacks > _debuffStacks)
		{
			_debuffStacks++;
		}
		if (DebuffSO.ResetFalloffTimeOnApplication)
		{
			TimeAlive = 0f;
		}
	}

	public void Trigger(float tickTime)
	{
		TimeAlive += tickTime;
		if (CanTrigger())
		{
			DamageInstance weaponDamageInstance = null;
			if (CombatWeapon != null && CombatWeapon.WeaponInstance != null && CombatWeapon.WeaponInstance.DamageInstance != null)
			{
				weaponDamageInstance = CombatWeapon.WeaponInstance.DamageInstance;
			}
			DebuffEffect.Trigger(_debuffedCharacter, _sourceCharacter, _debuffStacks, DamageInstance, weaponDamageInstance, _combatWeaponDamageType);
		}
	}

	private bool CanTrigger()
	{
		if (!_debuffedCharacter.isActiveAndEnabled)
		{
			return false;
		}
		if (_debuffedCharacter.IsDead)
		{
			return false;
		}
		if (_debuffedCharacter.GetDebuffImmunities().Contains(DebuffType))
		{
			return false;
		}
		if (DebuffSO.DebuffTriggerType == Enums.Debuff.DebuffTriggerType.Never)
		{
			return false;
		}
		if (DebuffSO.DebuffTriggerType == Enums.Debuff.DebuffTriggerType.Once)
		{
			return TriggerCount < 1;
		}
		_totalDeltaTime = Time.time;
		if (_totalDeltaTime - _lastTrigger > DebuffSO.DelayBetweenTriggers)
		{
			_lastTrigger = _totalDeltaTime;
			return true;
		}
		return false;
	}

	internal void SetCharacters(Character debuffedCharacter, Character sourceCharacter)
	{
		_debuffedCharacter = debuffedCharacter;
		_sourceCharacter = sourceCharacter;
	}
}
