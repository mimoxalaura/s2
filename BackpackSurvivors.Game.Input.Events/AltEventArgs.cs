using System;

namespace BackpackSurvivors.Game.Input.Events;

public class AltEventArgs : EventArgs
{
	public bool Pressed { get; private set; }

	public AltEventArgs(bool pressed)
	{
		Pressed = pressed;
	}
}
