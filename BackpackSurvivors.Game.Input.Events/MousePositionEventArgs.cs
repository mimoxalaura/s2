using System;
using UnityEngine;

namespace BackpackSurvivors.Game.Input.Events;

public class MousePositionEventArgs : EventArgs
{
	public Vector2 MousePosition { get; private set; }

	public MousePositionEventArgs(Vector2 mousePosition)
	{
		MousePosition = mousePosition;
	}
}
