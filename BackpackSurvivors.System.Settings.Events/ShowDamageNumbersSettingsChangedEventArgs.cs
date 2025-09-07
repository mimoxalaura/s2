using System;

namespace BackpackSurvivors.System.Settings.Events;

public class ShowDamageNumbersSettingsChangedEventArgs : EventArgs
{
	public Enums.ShowDamageNumbers ShowDamageNumbers { get; }

	public ShowDamageNumbersSettingsChangedEventArgs(Enums.ShowDamageNumbers showDamageNumbers)
	{
		ShowDamageNumbers = showDamageNumbers;
	}
}
