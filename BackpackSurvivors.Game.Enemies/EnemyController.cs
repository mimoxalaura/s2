using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Enemies.Minibosses;
using BackpackSurvivors.Game.Enemies.SpatialPartitioning;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.ScriptableObjects.Waves;
using BackpackSurvivors.System;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

internal class EnemyController : SingletonController<EnemyController>
{
	internal delegate void EnemyKilledHandler(object sender, EnemyKilledEventArgs e);

	[SerializeField]
	private int _standardMaxEnemyCount;

	[SerializeField]
	private int _duringBossFightMaxEnemyCount;

	[SerializeField]
	private float _maxEnemyDistance;

	[SerializeField]
	private float _minEnemyDistance;

	[SerializeField]
	private Enemy[] DEBUG_Enemies;

	private readonly List<Enemy> _enemies = new List<Enemy>();

	private int _enemySortOrderCounter;

	private int _totalEnemiesKilled;

	private WorldSpawnPositionGenerator _spawnPositionGenerator;

	private int _currentMaxEnemyCount;

	private TimeBasedLevelController _timeBasedLevelController;

	internal float MaxEnemyDistance => _maxEnemyDistance;

	internal float MinEnemyDistance => _minEnemyDistance;

	internal int TotalEnemiesKilled => _totalEnemiesKilled;

	internal TimeBasedLevelController TimeBasedLevelController
	{
		get
		{
			if (_timeBasedLevelController == null)
			{
				_timeBasedLevelController = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
			}
			return _timeBasedLevelController;
		}
	}

	public int ActiveEnemyCount => _enemies.Count();

	internal bool CountEnemiesKilled { get; private set; }

	internal float TotalEnemyHealth => _enemies.Select((Enemy e) => e.HealthSystem.GetHealth()).Sum();

	internal event EnemyKilledHandler OnEnemyKilled;

	internal void SetWorldSpawnPositionGenerator(WorldSpawnPositionGenerator spawnPositionGenerator)
	{
		_spawnPositionGenerator = spawnPositionGenerator;
	}

	internal void ResetStoredWavechunkSpawnLocations(string wavechunkName)
	{
		if (!(_spawnPositionGenerator == null))
		{
			_spawnPositionGenerator.ResetStoredWavechunkSpawnLocations(wavechunkName);
		}
	}

	internal void SetCountEnemiesKilled(bool countEnemiesKilled)
	{
		CountEnemiesKilled = countEnemiesKilled;
	}

	internal void ResetEnemySortOrderCounter()
	{
		_enemySortOrderCounter = 0;
	}

	internal void SetMaxEnemiesToStandard()
	{
		_currentMaxEnemyCount = _standardMaxEnemyCount;
	}

	internal void SetMaxEnemiesToDuringBossFight()
	{
		_currentMaxEnemyCount = _duringBossFightMaxEnemyCount;
	}

	internal Enemy GetRandomEnemyWithinRange(Vector2 origin, float range)
	{
		List<Enemy> enemiesToIgnore = new List<Enemy>();
		List<Enemy> list = _enemies.Where((Enemy x) => !ShouldSkipEnemy(x, enemiesToIgnore) && Vector2.Distance(origin, x.transform.position) <= range).ToList();
		if (list.Any())
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
		return null;
	}

	public Vector2 GetSpawnPosition(WaveChunkSO waveChunk, Vector2 forcedSpawnPosition, bool selectOnlyBossSpawnLocations = false)
	{
		if (_spawnPositionGenerator == null)
		{
			return forcedSpawnPosition;
		}
		if (forcedSpawnPosition != Vector2.zero)
		{
			return forcedSpawnPosition;
		}
		switch (waveChunk.WavePositionSpawnType)
		{
		case Enums.WavePositionSpawnType.Random:
		{
			Vector2 randomSpawnPositionInDirection = _spawnPositionGenerator.GetRandomSpawnPositionInDirection(waveChunk.SpawnLocation);
			float num = UnityEngine.Random.Range(-4f, 4f);
			float num2 = UnityEngine.Random.Range(-4f, 4f);
			return new Vector2(randomSpawnPositionInDirection.x + num, randomSpawnPositionInDirection.y + num2);
		}
		case Enums.WavePositionSpawnType.Grouped:
			return _spawnPositionGenerator.GetGroupedSpawnPosition(waveChunk);
		case Enums.WavePositionSpawnType.Circle:
			return _spawnPositionGenerator.GetCircleSpawnPosition(waveChunk);
		case Enums.WavePositionSpawnType.Line:
			return _spawnPositionGenerator.GetLineSpawnPosition(waveChunk);
		default:
			Debug.LogWarning(string.Format("WavePositionSpawnType {0} not handled in {1}.{2}", waveChunk.WavePositionSpawnType, "EnemyController", "GetSpawnPosition"));
			return _spawnPositionGenerator.GetRandomSpawnPositionNearPlayer();
		}
	}

	public Vector2 GetFreePositionNearSpawnPosition(Enemy enemy, Vector2 wantedSpawnPosition)
	{
		Vector2 vector = wantedSpawnPosition;
		int num = 0;
		while (!SpatialController.Instance.IsTargetPositionFree(enemy, vector, isSpawnPositionCheck: true) && num < 10)
		{
			vector = OffsetInRandomDirection(vector, 0.5f + 0.5f * (float)num);
			num++;
		}
		return vector;
	}

	private Vector2 OffsetInRandomDirection(Vector2 position, float distance)
	{
		Vector2 vector = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * distance;
		return position + vector;
	}

	public Vector2 GetRandomSpawnPosition(bool selectOnlyBossSpawnLocations = false)
	{
		return _spawnPositionGenerator.GetRandomSpawnPositionNearPlayer(selectOnlyBossSpawnLocations);
	}

	public Vector2 GetTeleportLocation(Vector2 enemyLocation, Vector2 playerLocation, bool selectOnlyBossSpawnLocations = false)
	{
		return _spawnPositionGenerator.GetTeleportLocation(enemyLocation, playerLocation, selectOnlyBossSpawnLocations);
	}

	internal Enemy GetEnemyWithinRange(Vector2 origin, float range, bool applyEnemyPriority = false)
	{
		List<Enemy> enemiesToIgnore = new List<Enemy>();
		return GetEnemyWithinRange(origin, range, enemiesToIgnore, applyEnemyPriority);
	}

	internal Enemy GetEnemyWithinRange(Vector2 origin, float range, List<Enemy> enemiesToIgnore, bool applyEnemyPriority = false)
	{
		int num = 0;
		Enemy result = null;
		float num2 = float.MaxValue;
		foreach (Enemy enemy in _enemies)
		{
			if (ShouldSkipEnemy(enemy, enemiesToIgnore))
			{
				continue;
			}
			int enemyPriority = GetEnemyPriority(enemy);
			if (applyEnemyPriority && enemyPriority < num)
			{
				continue;
			}
			float num3 = Vector2.Distance(origin, enemy.transform.position);
			if (!(num3 > range))
			{
				if (applyEnemyPriority && enemyPriority > num)
				{
					result = enemy;
					num2 = num3;
					num = enemyPriority;
				}
				else if (!(num3 > num2))
				{
					result = enemy;
					num2 = num3;
					num = enemyPriority;
				}
			}
		}
		return result;
	}

	private int GetEnemyPriority(Enemy enemy)
	{
		switch (enemy.BaseEnemy.EnemyType)
		{
		case Enums.Enemies.EnemyType.Minion:
			return 1;
		case Enums.Enemies.EnemyType.Monster:
			return 2;
		case Enums.Enemies.EnemyType.Elite:
			return 3;
		case Enums.Enemies.EnemyType.Miniboss:
			return 4;
		case Enums.Enemies.EnemyType.Boss:
			return 5;
		default:
			Debug.LogWarning(string.Format("EnemyType {0} is not handled in {1}.{2}", enemy.BaseEnemy.EnemyType, "EnemyController", "GetEnemyPriority"));
			return 0;
		}
	}

	private void Start()
	{
		_enemies.Clear();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType(typeof(Enemy), FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			Enemy item = (Enemy)array[i];
			_enemies.Add(item);
		}
		_currentMaxEnemyCount = _standardMaxEnemyCount;
		CountEnemiesKilled = true;
		base.IsInitialized = true;
	}

	private bool ShouldSkipEnemy(Enemy enemy, List<Enemy> enemiesToIgnore)
	{
		if (enemy == null)
		{
			return true;
		}
		if (!enemy.isActiveAndEnabled)
		{
			return true;
		}
		if (!enemy.IsAlive)
		{
			return true;
		}
		if (enemiesToIgnore.Contains(enemy))
		{
			return true;
		}
		return false;
	}

	internal void AddEnemy(Enemy enemy)
	{
		SpatialController.Instance.RegisterEnemy(enemy);
		int num = 0;
		if (enemy.BaseEnemy.IsFlying)
		{
			num += 5000;
		}
		enemy.OnKilled += Enemy_OnCharacterKilled;
		switch (enemy.EnemyType)
		{
		}
		_enemySortOrderCounter++;
		_enemies.Add(enemy);
	}

	private void Enemy_OnCharacterKilled(object sender, EventArgs e)
	{
		if (CountEnemiesKilled)
		{
			_totalEnemiesKilled++;
		}
		Enemy enemy = (Enemy)sender;
		SingletonController<CollectionController>.Instance.UnlockEnemy(enemy.BaseEnemy);
		this.OnEnemyKilled?.Invoke(this, new EnemyKilledEventArgs(enemy, enemy.EnemyType, _totalEnemiesKilled));
		SingletonController<SaveGameController>.Instance.ActiveSaveGame.StatisticsState.AddEnemyKilled(enemy.BaseId);
		enemy.RemoveHealthEvents();
		RemoveEnemy(enemy);
	}

	internal void RemoveEnemy(Enemy enemy)
	{
		_enemies.Remove(enemy);
		SpatialController.Instance.UnregisterEnemy(enemy);
	}

	internal void StopAllEnemies()
	{
		Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>(includeInactive: true);
		foreach (Enemy obj in array)
		{
			obj.EnemyMovement.PreventMovement();
			obj.EnemyMovement.StopMovement();
		}
	}

	[Command("Enemies.kill", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void KillAllEnemies()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType(typeof(Enemy), FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			Enemy enemy = (Enemy)array[i];
			if (!(enemy == null) && enemy.BaseEnemy.EnemyType != Enums.Enemies.EnemyType.Miniboss && enemy.BaseEnemy.EnemyType != Enums.Enemies.EnemyType.Boss && enemy.KillOnLevelCompleted)
			{
				enemy.OnKilled -= Enemy_OnCharacterKilled;
				enemy.CanDropLoot = false;
				enemy.Kill();
			}
		}
	}

	internal void CHEAT_KillAllEnemies()
	{
		_enemies.Clear();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType(typeof(Enemy), FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			Enemy enemy = (Enemy)array[i];
			if (!ShouldSkipEnemy(enemy, new List<Enemy>()))
			{
				enemy.Kill();
			}
		}
	}

	internal void CHEAT_KillOneEnemy()
	{
		_enemies.FirstOrDefault((Enemy x) => !ShouldSkipEnemy(x, new List<Enemy>()))?.Kill();
	}

	public override void Clear()
	{
		_enemies.Clear();
	}

	public override void ClearAdventure()
	{
		_enemies.Clear();
		_totalEnemiesKilled = 0;
	}

	internal bool IsAboveMaxEnemyCount()
	{
		return ActiveEnemyCount > _currentMaxEnemyCount;
	}

	internal void ResetActiveEnemyCount()
	{
		_enemies.Clear();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType(typeof(Enemy), FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			Enemy item = (Enemy)array[i];
			_enemies.Add(item);
		}
	}

	internal bool ShouldTeleportToPlayer(float distance)
	{
		return distance > _maxEnemyDistance;
	}

	internal Character CreateAndGetDummyEnemy()
	{
		return UnityEngine.Object.Instantiate(position: new Vector2(0f, 0f), original: SingletonController<GameDatabase>.Instance.GameDatabaseSO.DummyEnemy, rotation: Quaternion.identity);
	}

	internal void SpawnLootGoblin(LootGoblin lootGoblinPrefab, Transform parent, LevelSO currentLevel)
	{
		Vector2 randomSpawnPosition = GetRandomSpawnPosition();
		if (!(randomSpawnPosition == Vector2.zero))
		{
			Enemy enemy = UnityEngine.Object.Instantiate(lootGoblinPrefab).Enemy;
			enemy.SetCanTeleport(canTeleport: false);
			float enemyHealthMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetEnemyHealthMultiplierFromHellfire(currentLevel);
			float rewardMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetRewardMultiplierFromHellfire(currentLevel);
			enemy.ScaleHealth(1f, enemyHealthMultiplierFromHellfire, 1f);
			enemy.ScaleLoot(rewardMultiplierFromHellfire);
			enemy.gameObject.transform.position = randomSpawnPosition;
			AddEnemy(enemy);
		}
	}

	internal void RegisterTimeBasedLevelController(TimeBasedLevelController timeBasedLevelController)
	{
		_timeBasedLevelController = timeBasedLevelController;
	}

	internal void UnregisterTimeBasedLevelController()
	{
		_timeBasedLevelController = null;
	}
}
