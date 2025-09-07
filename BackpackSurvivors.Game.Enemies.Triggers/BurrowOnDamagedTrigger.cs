using System;
using BackpackSurvivors.Game.Enemies.Movement;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Triggers;

[RequireComponent(typeof(EnemyEventTriggers))]
[RequireComponent(typeof(BurrowingMovement))]
internal class BurrowOnDamagedTrigger : MonoBehaviour, IEnemyEventTriggerable
{
	[SerializeField]
	private float _damageTresholdForTriggering;

	private BurrowingMovement _burrowingMovement;

	public Enums.Enemies.EnemyTriggerType EnemyTriggerType => Enums.Enemies.EnemyTriggerType.Damaged;

	public bool Retriggerable => true;

	public float TimeBasedDelay => 0f;

	public float DamageTresholdForTriggering => _damageTresholdForTriggering;

	public float DamagePercentageTresholdForTriggering
	{
		get
		{
			throw new NotImplementedException();
		}
	}

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
