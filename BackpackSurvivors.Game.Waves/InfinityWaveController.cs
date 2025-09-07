using System.Collections;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.ScriptableObjects.Waves;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

internal class InfinityWaveController : TimeBasedWaveController
{
	[SerializeField]
	private InfiniteLevelController _infiniteLevelController;

	internal override IEnumerator SpawnWaveChunk(WaveChunkSO waveChunk, Vector2 forcedSpawnPosition)
	{
		if (waveChunk.BlockSpawn)
		{
			yield return null;
		}
		int scaledEnemiesToSpawn = waveChunk.NumberOfEnemiesToSpawn;
		float healthScale = (float)_infiniteLevelController.TimeSpentInLevel / _infiniteLevelController.HealthScaler + 1f;
		float damageScale = (float)_infiniteLevelController.TimeSpentInLevel / _infiniteLevelController.DamageScaler + 1f;
		float speedScale = (float)_infiniteLevelController.TimeSpentInLevel / _infiniteLevelController.SpeedScaler + 1f;
		SingletonController<EnemyController>.Instance.ResetStoredWavechunkSpawnLocations(waveChunk.name);
		for (int i = 0; i < scaledEnemiesToSpawn; i++)
		{
			while (SingletonController<EnemyController>.Instance.IsAboveMaxEnemyCount())
			{
				yield return new WaitForSeconds(1f);
			}
			Enemy enemy = Object.Instantiate(waveChunk.Enemy.EnemyPrefab, base.EnemiesParentGameObject.transform, worldPositionStays: true);
			enemy.CanDropLoot = !BossSpawned;
			enemy.ScaleHealth(base.CurrentWave.HealthScaleFactor, 1f, healthScale);
			enemy.ScaleDamage(base.CurrentWave.DamageScaleFactor * damageScale);
			Vector2 spawnPosition = SingletonController<EnemyController>.Instance.GetSpawnPosition(waveChunk, forcedSpawnPosition, enemy.EnemyType == Enums.Enemies.EnemyType.Boss);
			enemy.transform.position = spawnPosition;
			OverrideMovementBasedOnSpawnFormation(enemy, waveChunk);
			enemy.ScaleMovementSpeed(base.CurrentWave.MovementspeedScaleFactor * speedScale);
			enemy.ScaleLoot(base.CurrentWave.LootScaleFactor);
			if (RandomHelper.GetRollSuccess(base.EliteChance) && enemy.EnemyType != Enums.Enemies.EnemyType.Boss && enemy.EnemyType != Enums.Enemies.EnemyType.Miniboss)
			{
				enemy.UpgradeToElite();
			}
			SingletonController<EnemyController>.Instance.AddEnemy(enemy);
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
}
