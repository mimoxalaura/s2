using System;

namespace BackpackSurvivors.Game.Input.Events;

public class RotationEventArgs : EventArgs
{
	public bool Clockwise { get; private set; }

	public RotationEventArgs(bool clockwise)
	{
		Clockwise = clockwise;
	}
}
