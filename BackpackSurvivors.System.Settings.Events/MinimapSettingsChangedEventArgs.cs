using System;

namespace BackpackSurvivors.System.Settings.Events;

public class MinimapSettingsChangedEventArgs : EventArgs
{
	public Enums.MinimapVisual VisibleMinimap { get; }

	public MinimapSettingsChangedEventArgs(Enums.MinimapVisual visibleMinimap)
	{
		VisibleMinimap = visibleMinimap;
	}
}
