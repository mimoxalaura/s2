using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Buffs;

public class RelicRageBuff : BuffEffect
{
	private float powerBonus = 1f;

	public override void Trigger(Character buffedCharacter)
	{
		base.Trigger(buffedCharacter);
		float calculatedStat = buffedCharacter.GetCalculatedStat(Enums.ItemStatType.DamagePercentage);
		buffedCharacter.SetCalculatedStat(Enums.ItemStatType.DamagePercentage, calculatedStat + powerBonus);
	}

	public override void OnFallOff(Character buffedCharacter)
	{
		base.OnFallOff(buffedCharacter);
		float calculatedStat = buffedCharacter.GetCalculatedStat(Enums.ItemStatType.DamagePercentage);
		buffedCharacter.SetCalculatedStat(Enums.ItemStatType.DamagePercentage, calculatedStat - powerBonus);
	}
}
