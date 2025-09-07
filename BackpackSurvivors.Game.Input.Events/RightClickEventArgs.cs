using System;

namespace BackpackSurvivors.Game.Input.Events;

public class RightClickEventArgs : EventArgs
{
	public bool Pressed { get; private set; }

	public RightClickEventArgs(bool pressed)
	{
		Pressed = pressed;
	}
}
