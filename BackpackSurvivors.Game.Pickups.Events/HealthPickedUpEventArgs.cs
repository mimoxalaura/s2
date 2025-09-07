using System;
using UnityEngine;

namespace BackpackSurvivors.Game.Pickups.Events;

internal class HealthPickedUpEventArgs : EventArgs
{
	internal Vector2 PickupPosition { get; private set; }

	public HealthPickedUpEventArgs(Vector2 pickupPosition)
	{
		PickupPosition = pickupPosition;
	}
}
