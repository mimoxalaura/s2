using System;

namespace BackpackSurvivors.Game.Health.Events;

internal class HealthChangedEventArgs : EventArgs
{
	public float OldHealth { get; set; }

	public float NewHealth { get; set; }

	public bool HealthDidChange => OldHealth != NewHealth;

	public HealthChangedEventArgs()
	{
	}

	public HealthChangedEventArgs(float oldHealth, float newHealth)
	{
		OldHealth = oldHealth;
		NewHealth = newHealth;
	}
}
