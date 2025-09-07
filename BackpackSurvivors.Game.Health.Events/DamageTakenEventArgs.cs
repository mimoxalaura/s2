using System;

namespace BackpackSurvivors.Game.Health.Events;

internal class DamageTakenEventArgs : EventArgs
{
	public float DamageDealt { get; private set; }

	public bool ShouldTriggerAudioOnDamage { get; }

	public float TotalHealth { get; }

	public float RemainingHealth { get; }

	public DamageTakenEventArgs(float damageDealt, bool shouldTriggerAudioOnDamage, float totalHealth, float remainingHealth)
	{
		DamageDealt = damageDealt;
		ShouldTriggerAudioOnDamage = shouldTriggerAudioOnDamage;
		TotalHealth = totalHealth;
		RemainingHealth = remainingHealth;
	}
}
