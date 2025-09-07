using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Waves;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.SpatialPartitioning;

internal class SpatialController : MonoBehaviour
{
	internal class BatchScore : IComparable<BatchScore>
	{
		internal int BatchId { get; }

		internal int Score { get; private set; }

		internal BatchScore(int batchId, int score)
		{
			BatchId = batchId;
			Score = score;
		}

		internal void UpdateScore(int delta)
		{
			Score += delta;
		}

		public int CompareTo(BatchScore other)
		{
			int num = Score.CompareTo(other.Score);
			if (num == 0)
			{
				return BatchId.CompareTo(other.BatchId);
			}
			return num;
		}
	}

	internal static SpatialController Instance;

	private int _numberOfBatches = 1;

	private Dictionary<int, List<Enemy>> _enemyBatches = new Dictionary<int, List<Enemy>>();

	private float _runLogicTimer;

	private float _runLogicTimerCooldown = 1f;

	private bool _canAct;

	private int _phasingEnemiesLayerId;

	private int _numberOfPartitions = 1000;

	[HideInInspector]
	internal Dictionary<int, HashSet<Enemy>> EnemySpatialGroups = new Dictionary<int, HashSet<Enemy>>();

	private float _mapWidth;

	private float _mapHeight;

	private Vector2 _mapCenter;

	private SortedSet<BatchScore> _batchQueue_Enemy = new SortedSet<BatchScore>();

	private Dictionary<int, BatchScore> _batchScoreMap_Enemy = new Dictionary<int, BatchScore>();

	private void Start()
	{
		_phasingEnemiesLayerId = LayerMask.NameToLayer("PHASING_ENEMIES");
		Instance = this;
		SingletonCacheController.Instance.GetControllerByType<WorldSpawnPositionGenerator>().InitSpatialController(this);
	}

	private IEnumerator LogLargestSpatialGroup()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			int num = 0;
			foreach (KeyValuePair<int, HashSet<Enemy>> enemySpatialGroup in EnemySpatialGroups)
			{
				if (enemySpatialGroup.Value.Count() > num)
				{
					num = enemySpatialGroup.Value.Count();
				}
			}
			Debug.Log($"Largest Spatial Group: {num}");
			Debug.Log($"FPS: {(int)(1f / Time.deltaTime)}");
		}
	}

	private void FixedUpdate()
	{
		if (_canAct)
		{
			_runLogicTimer += Time.deltaTime;
			if (_runLogicTimer >= _runLogicTimerCooldown)
			{
				_runLogicTimer = 0f;
			}
			RunBatchLogic((int)(_runLogicTimer * (float)_numberOfBatches));
		}
	}

	internal void Init(Vector2 worldCenter, float leftBound, float rightBound, float topBound, float bottomBound)
	{
		InitializeBatches();
		CreateSpatialGroups();
		SetMapInfo(worldCenter, leftBound, rightBound, topBound, bottomBound);
		_canAct = true;
	}

	private void SetMapInfo(Vector2 worldCenter, float leftBound, float rightBound, float topBound, float bottomBound)
	{
		_mapCenter = worldCenter;
		_mapWidth = rightBound - leftBound;
		_mapHeight = topBound - bottomBound;
	}

	private void CreateSpatialGroups()
	{
		for (int i = 0; i < _numberOfPartitions; i++)
		{
			EnemySpatialGroups.Add(i, new HashSet<Enemy>());
		}
	}

	internal void AddToEnemyBatch(int batchId, Enemy enemy)
	{
		enemy.BatchID = batchId;
		_enemyBatches[batchId].Add(enemy);
	}

	internal void UpdateBatchOnUnitDeath(string option, int batchId)
	{
		if (option == "enemy")
		{
			UpdateBatchOnEnemyDeathRaw(_batchQueue_Enemy, _batchScoreMap_Enemy, batchId);
		}
	}

	private void UpdateBatchOnEnemyDeathRaw(SortedSet<BatchScore> batchQueue, Dictionary<int, BatchScore> batchScoreMap, int batchId)
	{
		if (!batchScoreMap.ContainsKey(batchId))
		{
			Debug.Log($"Batch not found for BatchId {batchId}");
		}
		BatchScore batchScore = batchScoreMap[batchId];
		batchQueue.Remove(batchScore);
		batchScore.UpdateScore(-1);
		batchQueue.Add(batchScore);
	}

	internal int GetBestBatch(string option)
	{
		if (option == "enemy")
		{
			return GetBestBatchRaw(_batchQueue_Enemy);
		}
		return -1;
	}

	private int GetBestBatchRaw(SortedSet<BatchScore> batchQueue)
	{
		BatchScore min = batchQueue.Min;
		if (min == null)
		{
			Debug.Log("No batch found");
			return 0;
		}
		batchQueue.Remove(min);
		min.UpdateScore(1);
		batchQueue.Add(min);
		return min.BatchId;
	}

	private void InitializeBatches()
	{
		for (int i = 0; i < _numberOfBatches; i++)
		{
			BatchScore batchScore = new BatchScore(i, 0);
			_enemyBatches.Add(i, new List<Enemy>());
			_batchScoreMap_Enemy.Add(i, batchScore);
			_batchQueue_Enemy.Add(batchScore);
		}
	}

	private void RunBatchLogic(int batchID)
	{
		foreach (Enemy item in _enemyBatches[batchID])
		{
			if (item != null)
			{
				item.RunLogic();
			}
		}
	}

	internal void RegisterEnemy(Enemy enemy)
	{
		int bestBatch = GetBestBatch("enemy");
		int spatialGroupID = GetSpatialGroupID(enemy.transform.position);
		AddToSpatialGroup(spatialGroupID, enemy);
		AddToEnemyBatch(bestBatch, enemy);
	}

	internal void UnregisterEnemy(Enemy enemy)
	{
		RemoveFromSpatialGroup(enemy.SpatialGroupID, enemy);
		UpdateBatchOnUnitDeath("enemy", enemy.BatchID);
	}

	internal bool IsTargetPositionFree(Enemy enemy, Vector2 targetPosition, bool isSpawnPositionCheck = false)
	{
		if (enemy == null || !EnemySpatialGroups.ContainsKey(enemy.SpatialGroupID))
		{
			return false;
		}
		if (enemy.gameObject.layer == _phasingEnemiesLayerId)
		{
			return true;
		}
		if (!isSpawnPositionCheck && enemy.EnemyMovement.MovementShouldIgnoreCollisions())
		{
			return true;
		}
		return !GetEnemies(enemy, targetPosition).Any((Enemy e) => e != null && IsOnSamePhysicsLayer(enemy, e) && IsWithinRadius(enemy, targetPosition, e) && IsOtherEnemy(enemy, e) && !WouldMoveAway(enemy, e, targetPosition));
	}

	private HashSet<Enemy> GetEnemies(Enemy enemy, Vector2 targetPosition)
	{
		HashSet<Enemy> hashSet = EnemySpatialGroups[enemy.SpatialGroupID].ToHashSet();
		int spatialGroupID = GetSpatialGroupID(targetPosition);
		if (spatialGroupID != enemy.SpatialGroupID)
		{
			foreach (Enemy item in EnemySpatialGroups[spatialGroupID])
			{
				hashSet.Add(item);
			}
		}
		return hashSet;
	}

	private bool WouldMoveAway(Enemy self, Enemy otherEnemy, Vector2 targetPosition)
	{
		return Vector2.Distance(otherEnemy.transform.position, self.transform.position) < Vector2.Distance(otherEnemy.transform.position, targetPosition);
	}

	private bool IsWithinRadius(Enemy self, Vector2 targetPosition, Enemy otherEnemy)
	{
		return Vector2.Distance(targetPosition, otherEnemy.transform.position) < self.Radius + otherEnemy.Radius;
	}

	private bool IsOnSamePhysicsLayer(Enemy self, Enemy otherEnemy)
	{
		return otherEnemy.gameObject.layer == self.gameObject.layer;
	}

	private bool IsOtherEnemy(Enemy self, Enemy otherEnemy)
	{
		return otherEnemy != self;
	}

	internal int GetSpatialGroupID(Vector2 position)
	{
		int num = (int)Mathf.Sqrt(_numberOfPartitions);
		int num2 = num;
		float num3 = _mapWidth / (float)num;
		float num4 = _mapHeight / (float)num2;
		float num5 = position.x - _mapCenter.x + _mapWidth / 2f;
		float num6 = position.y - _mapCenter.y + _mapHeight / 2f;
		int value = (int)(num5 / num3);
		int value2 = (int)(num6 / num4);
		value = Mathf.Clamp(value, 0, num - 1);
		value2 = Mathf.Clamp(value2, 0, num2 - 1);
		return value + value2 * num;
	}

	internal void AddToSpatialGroup(int spatialGroupID, Enemy enemy)
	{
		enemy.SpatialGroupID = spatialGroupID;
		EnemySpatialGroups[spatialGroupID].Add(enemy);
	}

	internal void RemoveFromSpatialGroup(int spatialGroupID, Enemy enemy)
	{
		EnemySpatialGroups[spatialGroupID].Remove(enemy);
	}

	private void OnDestroy()
	{
		Clear();
	}

	public void Clear()
	{
		Instance = null;
		_enemyBatches.Clear();
		EnemySpatialGroups.Clear();
	}
}
