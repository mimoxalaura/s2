using System;

namespace BackpackSurvivors.Game.Health.Events;

internal class HealthMaxChangedEventArgs : EventArgs
{
	public float OldHealthMax { get; private set; }

	public float NewHealthMax { get; private set; }

	public bool HealthMaxChanged => OldHealthMax != NewHealthMax;

	public HealthMaxChangedEventArgs(float oldHealthMax, float newHealthMax)
	{
		OldHealthMax = oldHealthMax;
		NewHealthMax = newHealthMax;
	}
}
