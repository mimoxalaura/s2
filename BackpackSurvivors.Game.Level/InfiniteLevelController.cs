using System;
using System.Linq;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Level.Events;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.GameplayFeedback;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

internal class InfiniteLevelController : TimeBasedLevelController
{
	[SerializeReference]
	private InfiniteLevelProgressFeedback _infiniteLevelProgressFeedback;

	[SerializeReference]
	internal float HealthScaler = 60f;

	[SerializeReference]
	internal float DamageScaler = 120f;

	[SerializeReference]
	internal float SpeedScaler = 360f;

	[SerializeReference]
	internal float SpawnBossEvery = 60f;

	[SerializeReference]
	internal float SpawnMajorBossEvery = 300f;

	[SerializeReference]
	internal EnemySO[] _bossPoolToSpawn;

	[SerializeReference]
	internal EnemySO[] _majorBossPoolToSpawn;

	private float _difficultyScaler;

	internal float DifficultyScaler => _difficultyScaler;

	protected override void AfterStart()
	{
		_difficultyScaler = (HealthScaler + DamageScaler + SpeedScaler) / 3f;
		_infiniteLevelProgressFeedback.Init();
		base.OnTimeRemainingInLevelUpdated += InfiniteLevelController_OnTimeRemainingInLevelUpdated;
	}

	private void InfiniteLevelController_OnTimeRemainingInLevelUpdated(object sender, TimeRemainingEventArgs e)
	{
		if (Math.Abs((float)base.TimeSpentInLevel - SpawnMajorBossEvery) == 0f)
		{
			if (_majorBossPoolToSpawn.Any())
			{
				int randomRoll = RandomHelper.GetRandomRoll(_majorBossPoolToSpawn.Count());
				EnemySO boss = _majorBossPoolToSpawn[randomRoll];
				SpawnBoss(boss);
			}
		}
		else if (Math.Abs((float)base.TimeSpentInLevel - SpawnBossEvery) == 0f && _bossPoolToSpawn.Any())
		{
			int randomRoll2 = RandomHelper.GetRandomRoll(_bossPoolToSpawn.Count());
			EnemySO boss2 = _bossPoolToSpawn[randomRoll2];
			SpawnBoss(boss2);
		}
	}

	private void SpawnBoss(EnemySO boss)
	{
		PlayBossAudio();
		Enemy enemy = UnityEngine.Object.Instantiate(boss.EnemyPrefab, base.transform, worldPositionStays: true);
		float hellfireScaleFactor = (float)base.TimeSpentInLevel / HealthScaler + 1f;
		float damageScaleFactor = (float)base.TimeSpentInLevel / DamageScaler + 1f;
		float movementSpeedScaleFactor = (float)base.TimeSpentInLevel / SpeedScaler + 1f;
		enemy.HealthSystem.SetHealthMax(1000f, setHealthToMax: true);
		enemy.ScaleHealth(1f, hellfireScaleFactor, 1f);
		enemy.ScaleDamage(damageScaleFactor);
		enemy.ScaleMovementSpeed(movementSpeedScaleFactor);
		enemy.ScaleLoot(1f);
		Vector2 randomSpawnPosition = SingletonController<EnemyController>.Instance.GetRandomSpawnPosition(selectOnlyBossSpawnLocations: true);
		enemy.transform.position = randomSpawnPosition;
		SingletonController<EnemyController>.Instance.AddEnemy(enemy);
	}
}
