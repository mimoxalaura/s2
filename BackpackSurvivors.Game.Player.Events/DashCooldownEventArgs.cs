using System;

namespace BackpackSurvivors.Game.Player.Events;

public class DashCooldownEventArgs : EventArgs
{
	public float CurrentCooldown { get; private set; }

	public float TotalCooldown { get; private set; }

	public int RemainingDashes { get; }

	public int TotalDashes { get; }

	public DashCooldownEventArgs(float currentCooldown, float totalCooldown, int remainingDashes, int totalDashes)
	{
		CurrentCooldown = currentCooldown;
		TotalCooldown = totalCooldown;
		RemainingDashes = remainingDashes;
		TotalDashes = totalDashes;
	}
}
