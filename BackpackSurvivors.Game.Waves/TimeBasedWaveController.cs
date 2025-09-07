using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Waves.Events;
using BackpackSurvivors.ScriptableObjects.Waves;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

internal class TimeBasedWaveController : MonoBehaviour
{
	public delegate void WaveStartedHandler(object sender, WaveStartedEventArgs e);

	internal delegate void BossSpawnedHandler(object sender, BossSpawnedEventArgs e);

	[SerializeField]
	private GameObject _enemiesParentGameObject;

	internal bool BossSpawned;

	private WaveSO _wave;

	private float _timePassedInWave;

	private List<WaveChunkSO> _waveChunks = new List<WaveChunkSO>();

	private bool _canAct;

	private LevelSO _level;

	private WaveSO _currentWave;

	private EnemyController _enemyController;

	private TimeBasedLevelController _timeBasedLevelController;

	private BackpackSurvivors.Game.Player.Player _player;

	private bool _isBossLevel;

	private bool _finalBossSetupCompleted;

	private Enemy _adventureBoss;

	private List<GameObject> _gameObjectsToDisableDuringBossIntroduction;

	private float _eliteChance;

	protected GameObject EnemiesParentGameObject => _enemiesParentGameObject;

	protected LevelSO Level => _level;

	protected WaveSO CurrentWave => _currentWave;

	protected float EliteChance => _eliteChance;

	internal event WaveStartedHandler OnWaveStarted;

	private EnemyController GetEnemyController()
	{
		if (_enemyController == null)
		{
			_enemyController = SingletonCacheController.Instance.GetControllerByType<EnemyController>();
		}
		return _enemyController;
	}

	private TimeBasedLevelController GetTimeBasedLevelController()
	{
		if (_timeBasedLevelController == null)
		{
			_timeBasedLevelController = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
		}
		return _timeBasedLevelController;
	}

	public void SpawnWaveChunkExternal(List<WaveChunkSO> waveChunks, Vector2 spawnPosition)
	{
		if (!_canAct)
		{
			return;
		}
		foreach (WaveChunkSO waveChunk in waveChunks)
		{
			if (!(waveChunk == null))
			{
				StartCoroutine(SpawnWaveChunk(waveChunk, spawnPosition));
			}
		}
	}

	public void InitLevel(LevelSO level)
	{
		_level = level;
		_wave = level.Waves[0];
		_isBossLevel = level.BossLevel;
		if (_isBossLevel)
		{
			SingletonController<EnemyController>.Instance.SetMaxEnemiesToDuringBossFight();
		}
		else
		{
			SingletonController<EnemyController>.Instance.SetMaxEnemiesToStandard();
		}
	}

	public void StartSpawning()
	{
		GetEnemyController().ResetEnemySortOrderCounter();
		_currentWave = _wave;
		_waveChunks.AddRange(_currentWave.WaveChunks);
		StartCoroutine(StartWaveCountdown());
	}

	internal void StopSpawning()
	{
		_canAct = false;
		StopAllCoroutines();
	}

	private IEnumerator StartWaveCountdown()
	{
		yield return new WaitForSeconds(0.1f);
		_canAct = true;
		this.OnWaveStarted?.Invoke(this, new WaveStartedEventArgs(_currentWave));
		StartCoroutine(UpdateTimeCoroutine());
	}

	private void Start()
	{
		RegisterObjects();
	}

	private IEnumerator UpdateTimeCoroutine()
	{
		float interval = 1f;
		while (true)
		{
			if (_canAct)
			{
				_timePassedInWave += interval;
				SpawnEnemies();
			}
			yield return new WaitForSeconds(interval);
		}
	}

	private void RegisterObjects()
	{
		_enemyController = SingletonCacheController.Instance.GetControllerByType<EnemyController>();
		_player = UnityEngine.Object.FindObjectOfType<BackpackSurvivors.Game.Player.Player>();
		_player.OnKilled += Player_OnCharacterKilled;
	}

	private void Player_OnCharacterKilled(object sender, EventArgs e)
	{
		_canAct = false;
	}

	private bool AnyWaveChunksRemaining()
	{
		return _waveChunks.Any();
	}

	private void SpawnEnemies()
	{
		if (!_waveChunks.Any())
		{
			ResetWavechunks();
		}
		WaveChunkSO waveChunkToSpawn = GetWaveChunkToSpawn();
		if (!(waveChunkToSpawn == null))
		{
			StartCoroutine(SpawnWaveChunk(waveChunkToSpawn, waveChunkToSpawn.ForcedSpawnLocation));
			_waveChunks.Remove(waveChunkToSpawn);
		}
	}

	private void ResetWavechunks()
	{
		if (!_isBossLevel)
		{
			_waveChunks.AddRange(_currentWave.WaveChunks);
			_timePassedInWave = 0f;
		}
	}

	private WaveChunkSO GetWaveChunkToSpawn()
	{
		WaveChunkSO waveChunkSO = (from wc in _waveChunks
			orderby wc.StartSpawningAfterDelay, wc.name
			select wc).FirstOrDefault((WaveChunkSO wc) => wc.StartSpawningAfterDelay < _timePassedInWave);
		if (waveChunkSO != null)
		{
			return waveChunkSO;
		}
		if (AnyEnemiesRemaining())
		{
			return null;
		}
		return _waveChunks.OrderBy((WaveChunkSO wc) => wc.StartSpawningAfterDelay).FirstOrDefault();
	}

	private bool AnyEnemiesRemaining()
	{
		return GetEnemyController().ActiveEnemyCount > 0;
	}

	internal virtual IEnumerator SpawnWaveChunk(WaveChunkSO waveChunk, Vector2 forcedSpawnPosition)
	{
		if (waveChunk == null)
		{
			yield return null;
		}
		if (waveChunk.BlockSpawn)
		{
			yield return null;
		}
		float enemyHealthMultiplier = SingletonController<DifficultyController>.Instance.GetEnemyHealthMultiplierFromHellfire(_level);
		float enemyDamageMultiplier = SingletonController<DifficultyController>.Instance.GetEnemyDamageMultiplierFromHellfire(_level);
		float enemyCountMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetEnemyCountMultiplierFromHellfire(_level);
		float enemyRewardMultiplier = SingletonController<DifficultyController>.Instance.GetRewardMultiplierFromHellfire(_level);
		int scaledEnemiesToSpawn = (int)((float)waveChunk.NumberOfEnemiesToSpawn * enemyCountMultiplierFromHellfire);
		if (waveChunk.Enemy.EnemyType == Enums.Enemies.EnemyType.Miniboss || waveChunk.Enemy.EnemyType == Enums.Enemies.EnemyType.Boss)
		{
			scaledEnemiesToSpawn = 1;
		}
		SingletonController<EnemyController>.Instance.ResetStoredWavechunkSpawnLocations(waveChunk.name);
		for (int i = 0; i < scaledEnemiesToSpawn; i++)
		{
			while (SingletonController<EnemyController>.Instance.IsAboveMaxEnemyCount())
			{
				yield return new WaitForSeconds(1f);
			}
			Enemy enemy = UnityEngine.Object.Instantiate(waveChunk.Enemy.EnemyPrefab, _enemiesParentGameObject.transform, worldPositionStays: true);
			float levelDurationScaleFactor = (float)GetTimeBasedLevelController().TimeSpentInLevel / 120f + 1f;
			enemy.CanDropLoot = !BossSpawned;
			enemy.ScaleHealth(_currentWave.HealthScaleFactor, enemyHealthMultiplier, levelDurationScaleFactor);
			enemy.ScaleDamage(_currentWave.DamageScaleFactor * enemyDamageMultiplier);
			Vector2 spawnPosition = SingletonController<EnemyController>.Instance.GetSpawnPosition(waveChunk, forcedSpawnPosition, enemy.EnemyType == Enums.Enemies.EnemyType.Boss);
			Vector2 freePositionNearSpawnPosition = SingletonController<EnemyController>.Instance.GetFreePositionNearSpawnPosition(enemy, spawnPosition);
			enemy.transform.position = freePositionNearSpawnPosition;
			OverrideMovementBasedOnSpawnFormation(enemy, waveChunk);
			enemy.ScaleMovementSpeed(_currentWave.MovementspeedScaleFactor);
			enemy.ScaleLoot(_currentWave.LootScaleFactor * enemyRewardMultiplier);
			if (RandomHelper.GetRollSuccess(_eliteChance) && enemy.EnemyType != Enums.Enemies.EnemyType.Boss && enemy.EnemyType != Enums.Enemies.EnemyType.Miniboss)
			{
				enemy.UpgradeToElite();
			}
			SingletonController<EnemyController>.Instance.AddEnemy(enemy);
			if (enemy.EnemyType == Enums.Enemies.EnemyType.Boss)
			{
				BossSpawned = true;
				AnimateFinalBossIntroduction(enemy);
				SingletonController<EnemyController>.Instance.SetMaxEnemiesToDuringBossFight();
			}
			if (waveChunk.SpawnDelayBetweenEnemies > 0f)
			{
				yield return new WaitForSeconds(waveChunk.SpawnDelayBetweenEnemies);
			}
			else
			{
				yield return null;
			}
		}
	}

	private void AnimateFinalBossIntroduction(Enemy enemy)
	{
		_adventureBoss = enemy;
		enemy.EnemyMovement.PreventMovement();
		enemy.EnemyMovement.SetCanMove(canMove: false);
		enemy.SetCanAct(canAct: false);
		enemy.SetHealthBarVisibility(show: false);
		enemy.GetSpriteRenderer().color = new Color(255f, 255f, 255f, 0f);
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: false);
		_gameObjectsToDisableDuringBossIntroduction = new List<GameObject>();
		_gameObjectsToDisableDuringBossIntroduction.Add(GameObject.Find("WorkingGameplayCanvas"));
		_gameObjectsToDisableDuringBossIntroduction.Add(GameObject.Find("MinimapCanvas"));
		foreach (GameObject item in _gameObjectsToDisableDuringBossIntroduction)
		{
			item.SetActive(value: false);
		}
		StartCoroutine(AnimateFinalBossIntroductionAsync(enemy));
	}

	private IEnumerator AnimateFinalBossIntroductionAsync(Enemy enemy)
	{
		AdventureBossAnimationController controllerByType = SingletonCacheController.Instance.GetControllerByType<AdventureBossAnimationController>();
		float animationDuration = controllerByType.GetAnimationDuration();
		controllerByType.Animate(SetupFinalBoss, enemy);
		yield return new WaitForSeconds(animationDuration);
		SetupFinalBoss();
	}

	public void SetupFinalBoss()
	{
		if (_finalBossSetupCompleted)
		{
			return;
		}
		_finalBossSetupCompleted = true;
		_adventureBoss.SetCanAct(canAct: true);
		_adventureBoss.EnemyMovement.SetCanMove(canMove: true);
		_adventureBoss.EnemyMovement.AllowMovement();
		_adventureBoss.SetHealthBarVisibility(show: true);
		_adventureBoss.GetSpriteRenderer().color = new Color(255f, 255f, 255f, 1f);
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: true);
		foreach (GameObject item in _gameObjectsToDisableDuringBossIntroduction)
		{
			item.SetActive(value: true);
		}
		SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>().SetupBoss(_adventureBoss, showHealthBar: true);
	}

	protected void OverrideMovementBasedOnSpawnFormation(Enemy enemy, WaveChunkSO waveChunk)
	{
		switch (waveChunk.WavePositionSpawnType)
		{
		case Enums.WavePositionSpawnType.Circle:
		{
			bool fixedSpriteFacingDirection = enemy.transform.position.x < SingletonController<GameController>.Instance.PlayerPosition.x;
			enemy.SetFixedSpriteFacingDirection(fixedSpriteFacingDirection);
			enemy.ReplaceEnemyMovement(Enums.EnemyMovementType.MoveToFixedPoint, storeCurrentMovement: true, waveChunk.name);
			enemy.SetCanTeleport(canTeleport: false);
			break;
		}
		case Enums.WavePositionSpawnType.Line:
		{
			bool fixedSpriteFacingDirection = SingletonCacheController.Instance.GetControllerByType<WorldSpawnPositionGenerator>().GetLineSpawnPositionCalculator(waveChunk.name).DirectionTowardsPlayer == Enums.CardinalDirection.MovingEast;
			enemy.SetFixedSpriteFacingDirection(fixedSpriteFacingDirection);
			enemy.ReplaceEnemyMovement(Enums.EnemyMovementType.MoveInFixedDirection, storeCurrentMovement: true, waveChunk.name);
			enemy.SetCanTeleport(canTeleport: false);
			break;
		}
		}
	}

	private void OnDestroy()
	{
		if (_player != null)
		{
			_player.OnKilled -= Player_OnCharacterKilled;
		}
	}

	internal void SetEliteChance(float eliteChance)
	{
		_eliteChance = eliteChance;
	}
}
