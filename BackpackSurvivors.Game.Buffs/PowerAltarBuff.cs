using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs;

public class PowerAltarBuff : BuffEffect
{
	[SerializeField]
	private float DamagePercentageBonus = 1f;

	public override void Trigger(Character buffedCharacter)
	{
		base.Trigger(buffedCharacter);
		buffedCharacter.GetCalculatedStat(Enums.ItemStatType.DamagePercentage);
		buffedCharacter.AddBuffedStat(Enums.ItemStatType.DamagePercentage, DamagePercentageBonus);
		SingletonCacheController.Instance.GetControllerByType<WeaponController>().RefreshWeapons();
	}

	public override void OnFallOff(Character buffedCharacter)
	{
		base.OnFallOff(buffedCharacter);
		buffedCharacter.AddBuffedStat(Enums.ItemStatType.DamagePercentage, 0f);
		SingletonCacheController.Instance.GetControllerByType<WeaponController>().RefreshWeapons();
	}
}
