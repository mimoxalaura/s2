using System;

namespace BackpackSurvivors.System.Settings.Events;

public class TooltipComplexitySettingsChangedEventArgs : EventArgs
{
	public Enums.TooltipComplexity TooltipComplexity { get; }

	public TooltipComplexitySettingsChangedEventArgs(Enums.TooltipComplexity tooltipComplexity)
	{
		TooltipComplexity = tooltipComplexity;
	}
}
