using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Game.Events;

public class CurrencyChangedEventArgs : EventArgs
{
	public Enums.CurrencyType CurrencyType { get; }

	public int NewAmount { get; }

	public int OldAmount { get; }

	public int ChangedValue { get; }

	public Enums.CurrencySource Source { get; }

	public CurrencyChangedEventArgs(Enums.CurrencyType currencyType, int newAmount, int oldAmount, int changedValue, Enums.CurrencySource source)
	{
		CurrencyType = currencyType;
		NewAmount = newAmount;
		OldAmount = oldAmount;
		ChangedValue = changedValue;
		Source = source;
	}
}
