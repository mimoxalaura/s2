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
internal class SpawnWavechunksOnDead : MonoBehaviour, IEnemyEventTriggerable
{
	[SerializeField]
	private List<WaveChunkSO> _wavechunksToSpawn;

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
		TimeBasedWaveController controllerByType = SingletonCacheController.Instance.GetControllerByType<TimeBasedWaveController>();
		if (_wavechunksToSpawn != null)
		{
			controllerByType.SpawnWaveChunkExternal(_wavechunksToSpawn, base.transform.position);
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
