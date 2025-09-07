using System;

namespace BackpackSurvivors.Game.Input.Events;

public class ControlSchemeChangedEventArgs : EventArgs
{
	public string CurrentControlSchemeName { get; private set; }

	public ControlSchemeChangedEventArgs(string controlSchemeName)
	{
		CurrentControlSchemeName = controlSchemeName;
	}
}
