using System;
using BackpackSurvivors.Game.Enemies.Triggers.TriggerConditions;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Triggers;

[RequireComponent(typeof(EnemyEventTriggers))]
internal class SpawnPrefabAfterTime : MonoBehaviour, IEnemyEventTriggerable
{
	[SerializeField]
	private bool _retriggerable;

	[SerializeField]
	private float _triggerDelay;

	[SerializeField]
	private GameObject _prefabToSpawn;

	[SerializeField]
	private BaseTriggerCondition[] _triggerConditions;

	public bool Enabled = true;

	public Enums.Enemies.EnemyTriggerType EnemyTriggerType => Enums.Enemies.EnemyTriggerType.TimeBased;

	public bool Retriggerable => _retriggerable;

	public float TimeBasedDelay => _triggerDelay;

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
		if (Enabled)
		{
			UnityEngine.Object.Instantiate(_prefabToSpawn, base.transform.position, Quaternion.identity);
		}
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
