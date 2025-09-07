using System;

namespace BackpackSurvivors.System.Settings.Events;

public class DashVisualSettingsChangedEventArgs : EventArgs
{
	public Enums.DashVisual DashVisual { get; }

	public DashVisualSettingsChangedEventArgs(Enums.DashVisual dashVisual)
	{
		DashVisual = dashVisual;
	}
}
