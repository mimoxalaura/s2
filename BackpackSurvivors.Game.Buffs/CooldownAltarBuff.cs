using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Buffs;

public class CooldownAltarBuff : BuffEffect
{
	private float cooldownReduction = 0.5f;

	public override void Trigger(Character buffedCharacter)
	{
		base.Trigger(buffedCharacter);
		buffedCharacter.AddBuffedStat(Enums.ItemStatType.CooldownReductionPercentage, cooldownReduction);
		SingletonCacheController.Instance.GetControllerByType<WeaponController>().RefreshWeapons();
	}

	public override void OnFallOff(Character buffedCharacter)
	{
		base.OnFallOff(buffedCharacter);
		buffedCharacter.AddBuffedStat(Enums.ItemStatType.CooldownReductionPercentage, 0f);
		SingletonCacheController.Instance.GetControllerByType<WeaponController>().RefreshWeapons();
	}
}
