using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs;

internal class GenericDamageTypeBuff : BuffEffect
{
	[SerializeField]
	private Enums.DamageType[] _buffedDamageType;

	[SerializeField]
	private float _buffValue;

	private WeaponController _weaponController;

	public override void Init(BuffSO buffSO)
	{
		base.Init(buffSO);
		_weaponController = SingletonCacheController.Instance.GetControllerByType<WeaponController>();
	}

	public override void Trigger(Character buffedCharacter)
	{
		base.Trigger(buffedCharacter);
		Enums.DamageType[] buffedDamageType = _buffedDamageType;
		foreach (Enums.DamageType type in buffedDamageType)
		{
			buffedCharacter.AddBuffedDamageType(type, _buffValue);
		}
		_weaponController.RefreshWeapons();
	}

	public override void OnFallOff(Character buffedCharacter)
	{
		base.OnFallOff(buffedCharacter);
		Enums.DamageType[] buffedDamageType = _buffedDamageType;
		foreach (Enums.DamageType type in buffedDamageType)
		{
			buffedCharacter.AddBuffedDamageType(type, 0f - _buffValue);
		}
		_weaponController.RefreshWeapons();
	}
}
