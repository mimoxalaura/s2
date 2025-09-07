using System;
using UnityEngine;

namespace BackpackSurvivors.Game.Pickups;

internal class CoinPickedUpEventArgs : EventArgs
{
	internal Vector2 PickupPosition { get; private set; }

	public CoinPickedUpEventArgs(Vector2 pickupPosition)
	{
		PickupPosition = pickupPosition;
	}
}
