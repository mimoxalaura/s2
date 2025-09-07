using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs;

public class SpeedAltarBuff : BuffEffect
{
	[SerializeField]
	private float speedBonusMultiplier = 0.5f;

	public override void Trigger(Character buffedCharacter)
	{
		base.Trigger(buffedCharacter);
		buffedCharacter.AddBuffedStat(Enums.ItemStatType.SpeedPercentage, speedBonusMultiplier);
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: false);
	}

	public override void OnFallOff(Character buffedCharacter)
	{
		base.OnFallOff(buffedCharacter);
		buffedCharacter.RemoveBuffedStat(Enums.ItemStatType.SpeedPercentage, speedBonusMultiplier);
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: false);
	}
}
