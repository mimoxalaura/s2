using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Enemies.Triggers;

internal interface IEnemyEventTriggerable
{
	Enums.Enemies.EnemyTriggerType EnemyTriggerType { get; }

	bool Retriggerable { get; }

	float TimeBasedDelay { get; }

	float DamageTresholdForTriggering { get; }

	float DamagePercentageTresholdForTriggering { get; }

	void Execute();

	bool ShouldExecute();
}
