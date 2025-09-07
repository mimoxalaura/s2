using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.RuneEffects;

public class RuneSpecialEffect_GoldPercentageGained : RuneSpecialEffect
{
	[SerializeField]
	private float _goldPercentageToGain;

	private int _coinsToGain;

	private void Start()
	{
		int currency = SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.Coins);
		_coinsToGain = (int)((float)currency * _goldPercentageToGain);
	}

	public override bool Trigger()
	{
		base.Trigger();
		SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.Coins, _coinsToGain, Enums.CurrencySource.Drop);
		return true;
	}

	public override string GetTriggerMessage()
	{
		return $"{_coinsToGain} Coins gained!";
	}
}
