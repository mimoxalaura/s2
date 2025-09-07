using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs;

internal class GenericStatBuff : BuffEffect
{
	[SerializeField]
	private Enums.ItemStatType _buffedStatType;

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
		buffedCharacter.AddBuffedStat(_buffedStatType, _buffValue);
		_weaponController.RefreshWeapons(refreshDashes: false);
	}

	public override void OnFallOff(Character buffedCharacter)
	{
		base.OnFallOff(buffedCharacter);
		buffedCharacter.RemoveBuffedStat(_buffedStatType, _buffValue);
		_weaponController.RefreshWeapons(refreshDashes: false);
	}
}
