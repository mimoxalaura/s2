using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Game.Events;

internal class CannotAffordCurrencyEventArgs : EventArgs
{
	internal Enums.CurrencyType CurrencyType { get; private set; }

	internal int AmountAttemptedToSpend { get; private set; }

	internal CannotAffordCurrencyEventArgs(Enums.CurrencyType currencyType, int amountAttemptedToSpend)
	{
		CurrencyType = currencyType;
		AmountAttemptedToSpend = amountAttemptedToSpend;
	}
}
