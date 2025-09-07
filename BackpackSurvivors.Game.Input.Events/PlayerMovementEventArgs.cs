using System;
using UnityEngine;

namespace BackpackSurvivors.Game.Input.Events;

public class PlayerMovementEventArgs : EventArgs
{
	public Vector2 PlayerMovement { get; private set; }

	public PlayerMovementEventArgs(Vector2 playerMovement)
	{
		PlayerMovement = playerMovement;
	}
}
