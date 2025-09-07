using System;

namespace BackpackSurvivors.System.Settings.Events;

public class CooldownVisualSettingsChangedEventArgs : EventArgs
{
	public Enums.CooldownVisuals CooldownVisuals { get; }

	public CooldownVisualSettingsChangedEventArgs(Enums.CooldownVisuals cooldownVisuals)
	{
		CooldownVisuals = cooldownVisuals;
	}
}
