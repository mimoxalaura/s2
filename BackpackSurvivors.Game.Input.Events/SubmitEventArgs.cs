using System;

namespace BackpackSurvivors.Game.Input.Events;

public class SubmitEventArgs : EventArgs
{
	public bool Pressed { get; private set; }

	public SubmitEventArgs(bool pressed)
	{
		Pressed = pressed;
	}
}
