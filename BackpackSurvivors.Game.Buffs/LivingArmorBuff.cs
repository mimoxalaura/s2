using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs;

public class LivingArmorBuff : BuffEffect
{
	[SerializeField]
	private float armorBonus = 25f;

	public override void Trigger(Character buffedCharacter)
	{
		base.Trigger(buffedCharacter);
		float calculatedStat = buffedCharacter.GetCalculatedStat(Enums.ItemStatType.Armor);
		buffedCharacter.SetCalculatedStat(Enums.ItemStatType.Armor, calculatedStat + armorBonus);
	}

	public override void OnFallOff(Character buffedCharacter)
	{
		base.OnFallOff(buffedCharacter);
		float calculatedStat = buffedCharacter.GetCalculatedStat(Enums.ItemStatType.Armor);
		buffedCharacter.SetCalculatedStat(Enums.ItemStatType.Armor, calculatedStat - armorBonus);
	}
}
