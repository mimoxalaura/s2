using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs;

internal class GenericStatMultiplierBuff : BuffEffect
{
	[SerializeField]
	private Enums.ItemStatType _buffedStatType;

	[SerializeField]
	private float _buffMultiplierValue;

	public override void Trigger(Character buffedCharacter)
	{
		base.Trigger(buffedCharacter);
		float calculatedStat = buffedCharacter.GetCalculatedStat(_buffedStatType);
		float value = _buffMultiplierValue * calculatedStat;
		buffedCharacter.AddBuffedStat(_buffedStatType, value);
		SingletonCacheController.Instance.GetControllerByType<WeaponController>().RefreshWeapons();
	}

	public override void OnFallOff(Character buffedCharacter)
	{
		base.OnFallOff(buffedCharacter);
		float calculatedStat = buffedCharacter.GetCalculatedStat(_buffedStatType);
		float value = _buffMultiplierValue * calculatedStat;
		buffedCharacter.RemoveBuffedStat(_buffedStatType, value);
		SingletonCacheController.Instance.GetControllerByType<WeaponController>().RefreshWeapons();
	}
}
