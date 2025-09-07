using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Health.Events;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Triggers;

internal class EnemyEventTriggers : MonoBehaviour
{
	private List<IEnemyEventTriggerable> _damagedTriggers = new List<IEnemyEventTriggerable>();

	private List<IEnemyEventTriggerable> _percentageDamageTakenTriggers = new List<IEnemyEventTriggerable>();

	private List<IEnemyEventTriggerable> _percentageHealthReachedTriggers = new List<IEnemyEventTriggerable>();

	private List<IEnemyEventTriggerable> _deadTriggers = new List<IEnemyEventTriggerable>();

	private List<IEnemyEventTriggerable> _timeBasedTriggers = new List<IEnemyEventTriggerable>();

	private void Start()
	{
		InitTriggers();
		RegisterEvents();
		StartTimeBasedTriggersCountdown();
	}

	private void StartTimeBasedTriggersCountdown()
	{
		foreach (IEnemyEventTriggerable timeBasedTrigger in _timeBasedTriggers)
		{
			if (timeBasedTrigger.ShouldExecute())
			{
				StartCoroutine(ExecuteTriggerAfterTimeBasedDelay(timeBasedTrigger));
			}
		}
	}

	private IEnumerator ExecuteTriggerAfterTimeBasedDelay(IEnemyEventTriggerable trigger)
	{
		yield return new WaitForSeconds(trigger.TimeBasedDelay);
		if (trigger.ShouldExecute())
		{
			trigger.Execute();
		}
		if (trigger.Retriggerable)
		{
			StartCoroutine(ExecuteTriggerAfterTimeBasedDelay(trigger));
		}
	}

	private void RegisterEvents()
	{
		Enemy component = GetComponent<Enemy>();
		component.OnKilled += Enemy_HealthSystem_OnDead;
		component.OnCharacterDamaged += Enemy_HealthSystem_OnDamaged;
	}

	private void InitTriggers()
	{
		IEnemyEventTriggerable[] components = GetComponents<IEnemyEventTriggerable>();
		foreach (IEnemyEventTriggerable trigger in components)
		{
			AddTriggerToList(trigger, _deadTriggers, Enums.Enemies.EnemyTriggerType.Dead);
			AddTriggerToList(trigger, _damagedTriggers, Enums.Enemies.EnemyTriggerType.Damaged);
			AddTriggerToList(trigger, _percentageDamageTakenTriggers, Enums.Enemies.EnemyTriggerType.PercentageDamaged);
			AddTriggerToList(trigger, _percentageHealthReachedTriggers, Enums.Enemies.EnemyTriggerType.PercentageHealthReached);
			AddTriggerToList(trigger, _timeBasedTriggers, Enums.Enemies.EnemyTriggerType.TimeBased);
		}
	}

	private void AddTriggerToList(IEnemyEventTriggerable trigger, List<IEnemyEventTriggerable> list, Enums.Enemies.EnemyTriggerType triggerType)
	{
		if ((trigger.EnemyTriggerType & triggerType) == triggerType)
		{
			list.Add(trigger);
		}
	}

	private void Enemy_HealthSystem_OnDamaged(object sender, DamageTakenEventArgs e)
	{
		foreach (IEnemyEventTriggerable damagedTrigger in _damagedTriggers)
		{
			if (!(e.DamageDealt < damagedTrigger.DamageTresholdForTriggering))
			{
				damagedTrigger.Execute();
			}
		}
		foreach (IEnemyEventTriggerable percentageDamageTakenTrigger in _percentageDamageTakenTriggers)
		{
			if (!(e.DamageDealt / e.TotalHealth > percentageDamageTakenTrigger.DamagePercentageTresholdForTriggering))
			{
				percentageDamageTakenTrigger.Execute();
			}
		}
		foreach (IEnemyEventTriggerable percentageHealthReachedTrigger in _percentageHealthReachedTriggers)
		{
			if (!(e.RemainingHealth / e.TotalHealth > percentageHealthReachedTrigger.DamagePercentageTresholdForTriggering))
			{
				percentageHealthReachedTrigger.Execute();
			}
		}
	}

	private void Enemy_HealthSystem_OnDead(object sender, EventArgs e)
	{
		ExecuteTriggers(_deadTriggers);
	}

	private void ExecuteTriggers(List<IEnemyEventTriggerable> triggers)
	{
		foreach (IEnemyEventTriggerable trigger in triggers)
		{
			if (trigger.ShouldExecute())
			{
				trigger.Execute();
			}
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
