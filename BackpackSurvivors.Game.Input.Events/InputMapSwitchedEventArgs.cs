using System;

namespace BackpackSurvivors.Game.Input.Events;

internal class InputMapSwitchedEventArgs : EventArgs
{
	internal bool IsInPlayerInputMap { get; private set; }

	public InputMapSwitchedEventArgs(bool isInPlayerInputMap)
	{
		IsInPlayerInputMap = isInPlayerInputMap;
	}
}
