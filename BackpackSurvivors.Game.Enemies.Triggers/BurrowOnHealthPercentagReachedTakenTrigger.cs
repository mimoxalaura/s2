using BackpackSurvivors.Game.Enemies.Movement;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Triggers;

[RequireComponent(typeof(EnemyEventTriggers))]
[RequireComponent(typeof(BurrowingMovement))]
internal class BurrowOnHealthPercentagReachedTakenTrigger : MonoBehaviour, IEnemyEventTriggerable
{
	[SerializeField]
	private float _damagePercentageTresholdForTriggering;

	private BurrowingMovement _burrowingMovement;

	public Enums.Enemies.EnemyTriggerType EnemyTriggerType => Enums.Enemies.EnemyTriggerType.PercentageHealthReached;

	public bool Retriggerable => true;

	public float TimeBasedDelay => 0f;

	public float DamageTresholdForTriggering => 0f;

	public float DamagePercentageTresholdForTriggering => _damagePercentageTresholdForTriggering;

	private void Awake()
	{
		_burrowingMovement = GetComponent<BurrowingMovement>();
	}

	public void Execute()
	{
		_burrowingMovement.Burrow();
	}

	public bool ShouldExecute()
	{
		return true;
	}
}
