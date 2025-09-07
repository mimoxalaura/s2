using System;
using UnityEngine;

namespace BackpackSurvivors.Game.Input.Events;

public class PlayerAimEventArgs : EventArgs
{
	public Vector2 PlayerAim { get; private set; }

	public PlayerAimEventArgs(Vector2 playerAim)
	{
		PlayerAim = playerAim;
	}
}
