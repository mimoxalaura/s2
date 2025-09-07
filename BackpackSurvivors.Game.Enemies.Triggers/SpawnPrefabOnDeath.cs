using System;
using BackpackSurvivors.Game.Enemies.Triggers.TriggerConditions;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Triggers;

[RequireComponent(typeof(EnemyEventTriggers))]
internal class SpawnPrefabOnDeath : MonoBehaviour, IEnemyEventTriggerable
{
	[SerializeField]
	private GameObject _prefabToSpawn;

	[SerializeField]
	private BaseTriggerCondition[] _triggerConditions;

	public Enums.Enemies.EnemyTriggerType EnemyTriggerType => Enums.Enemies.EnemyTriggerType.Dead;

	public float TimeBasedDelay
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool Retriggerable => false;

	public float DamageTresholdForTriggering
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public float DamagePercentageTresholdForTriggering
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public void Execute()
	{
		UnityEngine.Object.Instantiate(_prefabToSpawn, base.transform.position, Quaternion.identity);
	}

	public bool ShouldExecute()
	{
		BaseTriggerCondition[] triggerConditions = _triggerConditions;
		for (int i = 0; i < triggerConditions.Length; i++)
		{
			if (!triggerConditions[i].ShouldExecute())
			{
				return false;
			}
		}
		return true;
	}
}
