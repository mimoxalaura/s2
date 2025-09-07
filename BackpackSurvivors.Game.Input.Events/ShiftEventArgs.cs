using System;

namespace BackpackSurvivors.Game.Input.Events;

public class ShiftEventArgs : EventArgs
{
	public bool Pressed { get; private set; }

	public ShiftEventArgs(bool pressed)
	{
		Pressed = pressed;
	}
}
