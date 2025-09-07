using System;

namespace BackpackSurvivors.System.Settings.Events;

public class TargetingSettingsChangedEventArgs : EventArgs
{
	public Enums.Targeting Targeting { get; }

	public TargetingSettingsChangedEventArgs(Enums.Targeting targeting)
	{
		Targeting = targeting;
	}
}
