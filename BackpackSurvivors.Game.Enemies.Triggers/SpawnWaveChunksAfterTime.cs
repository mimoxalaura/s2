using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Enemies.Triggers.TriggerConditions;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.ScriptableObjects.Waves;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Triggers;

[RequireComponent(typeof(EnemyEventTriggers))]
internal class SpawnWaveChunksAfterTime : MonoBehaviour, IEnemyEventTriggerable
{
	[SerializeField]
	private bool _retriggerable;

	[SerializeField]
	private float _triggerDelay;

	[SerializeField]
	private List<WaveChunkSO> _wavechunksToSpawn;

	[SerializeField]
	private BaseTriggerCondition[] _triggerConditions;

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
		SingletonCacheController.Instance.GetControllerByType<TimeBasedWaveController>().SpawnWaveChunkExternal(_wavechunksToSpawn, base.transform.position);
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
