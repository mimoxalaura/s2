using System;
using UnityEngine;

namespace BackpackSurvivors.Game.Input.Events;

public class CursorPositionEventArgs : EventArgs
{
	public Vector2 CursorPosition { get; private set; }

	public CursorPositionEventArgs(Vector2 fakeMousePosition)
	{
		CursorPosition = fakeMousePosition;
	}
}
