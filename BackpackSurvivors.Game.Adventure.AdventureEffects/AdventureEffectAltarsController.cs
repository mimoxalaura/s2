using System.Collections.Generic;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure.AdventureEffects;

public class AdventureEffectAltarsController : AdventureEffectController
{
	[SerializeField]
	private SerializableDictionaryBase<GameObject, float> _availableAltars;

	[SerializeField]
	private int _minAmountSpawned;

	[SerializeField]
	private int _maxAmountSpawned;

	[SerializeField]
	private int _maxTries;

	private WorldSpawnPositionGenerator _worldSpawnPositionGenerator;

	private int _spawnedAltars;

	internal override void InitializeEffect(TimeBasedWaveController timeBasedWaveController, LevelSO level)
	{
		_spawnedAltars = 0;
		base.InitializeEffect(timeBasedWaveController, level);
		if (base.LevelSO.BossLevel && !base.TriggerInBossArena)
		{
			return;
		}
		for (int i = 0; i < _maxTries; i++)
		{
			if (_spawnedAltars >= _maxAmountSpawned)
			{
				break;
			}
			foreach (KeyValuePair<GameObject, float> availableAltar in _availableAltars)
			{
				if (RandomHelper.GetRollSuccess(availableAltar.Value))
				{
					SpawnAltarInRandomPosition(availableAltar.Key);
					_spawnedAltars++;
				}
			}
			if (_spawnedAltars >= _minAmountSpawned)
			{
				break;
			}
		}
	}

	private void SpawnAltarInRandomPosition(GameObject key)
	{
		if (_worldSpawnPositionGenerator == null)
		{
			_worldSpawnPositionGenerator = SingletonCacheController.Instance.GetControllerByType<WorldSpawnPositionGenerator>();
		}
		Debug.Log("Spawning Altar: " + key.name);
		SpawnPosition randomActiveSpawnPosition = _worldSpawnPositionGenerator.GetRandomActiveSpawnPosition();
		Object.Instantiate(key).transform.position = randomActiveSpawnPosition.transform.position;
		_worldSpawnPositionGenerator.BlockSpawnPosition(randomActiveSpawnPosition);
	}
}
