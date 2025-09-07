using System;

namespace BackpackSurvivors.System.Settings.Events;

public class ShowHealthBarsSettingsChangedEventArgs : EventArgs
{
	public Enums.ShowHealthBars ShowHealthBars { get; }

	public ShowHealthBarsSettingsChangedEventArgs(Enums.ShowHealthBars showHealthBars)
	{
		ShowHealthBars = showHealthBars;
	}
}
