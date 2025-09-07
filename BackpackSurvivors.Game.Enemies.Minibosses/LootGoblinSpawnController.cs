using System.Collections;
using System.Linq;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Minibosses;

internal class LootGoblinSpawnController : MonoBehaviour
{
	[SerializeField]
	private LootGoblin _lootGoblinPrefab;

	[SerializeField]
	private Transform _parent;

	[SerializeField]
	private float _spawnChance;

	[SerializeField]
	private float _spawnTickRate;

	[SerializeField]
	private int _maxNumberOfGoblinsAllowed = 1;

	private TimeBasedLevelController _timeBasedLevelController;

	private float _currentSpawnChance;

	private void Start()
	{
		_timeBasedLevelController = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
		StartCoroutine(PauseMovementRandomly());
	}

	private IEnumerator PauseMovementRandomly()
	{
		_currentSpawnChance = _spawnChance;
		while (!_timeBasedLevelController.IsLevelFinished)
		{
			yield return new WaitForSeconds(_spawnTickRate);
			if (RandomHelper.GetRollSuccess(_currentSpawnChance))
			{
				if (Object.FindObjectsByType<LootGoblin>(FindObjectsSortMode.None).Count() < _maxNumberOfGoblinsAllowed)
				{
					SingletonController<EnemyController>.Instance.SpawnLootGoblin(_lootGoblinPrefab, _parent, _timeBasedLevelController.CurrentLevel);
					_currentSpawnChance = _spawnChance;
				}
			}
			else
			{
				_currentSpawnChance += _spawnChance;
			}
		}
	}
}
