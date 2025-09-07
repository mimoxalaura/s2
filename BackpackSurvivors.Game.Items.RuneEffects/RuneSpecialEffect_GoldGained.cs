using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.RuneEffects;

public class RuneSpecialEffect_GoldGained : RuneSpecialEffect
{
	[SerializeField]
	private int _goldToGain;

	public override bool Trigger()
	{
		base.Trigger();
		SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.Coins, _goldToGain, Enums.CurrencySource.Drop);
		return true;
	}

	public override string GetTriggerMessage()
	{
		return $"{_goldToGain} coins gained!";
	}
}
