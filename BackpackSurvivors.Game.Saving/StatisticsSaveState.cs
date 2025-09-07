using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class StatisticsSaveState : BaseSaveState
{
	public Dictionary<int, int> CompletedAdventuresAndHellfireLevel;

	public Dictionary<int, int> EnemiesKilledPerEnemyId;

	public Dictionary<Enums.SingleValueStatisticMetrics, int> SingleValueStatisticMetrics;

	public int PlayedTime;

	public DateTime LastPlayed;

	public StatisticsSaveState()
	{
		Init();
	}

	public void Init()
	{
		EnemiesKilledPerEnemyId = new Dictionary<int, int>();
		SingleValueStatisticMetrics = new Dictionary<Enums.SingleValueStatisticMetrics, int>();
	}

	public void SetState(Dictionary<int, int> enemiesKilledPerEnemyId, Dictionary<Enums.SingleValueStatisticMetrics, int> singleValueStatisticMetrics, int playedTime, DateTime lastPlayed, Dictionary<int, int> _completedAdventuresAndHellfireLevel)
	{
		EnemiesKilledPerEnemyId = enemiesKilledPerEnemyId;
		SingleValueStatisticMetrics = singleValueStatisticMetrics;
		PlayedTime = playedTime;
		LastPlayed = DateTime.Now;
		CompletedAdventuresAndHellfireLevel = _completedAdventuresAndHellfireLevel;
	}

	public override bool HasData()
	{
		if (EnemiesKilledPerEnemyId == null || !EnemiesKilledPerEnemyId.Any())
		{
			if (SingleValueStatisticMetrics == null || !SingleValueStatisticMetrics.Any())
			{
				return PlayedTime > 0;
			}
			return true;
		}
		return true;
	}

	public void AddEnemyKilled(int enemyId)
	{
		if (!EnemiesKilledPerEnemyId.ContainsKey(enemyId))
		{
			EnemiesKilledPerEnemyId.Add(enemyId, 0);
		}
		EnemiesKilledPerEnemyId[enemyId]++;
	}

	public int GetEnemyKilledCount(int enemyId)
	{
		if (EnemiesKilledPerEnemyId.ContainsKey(enemyId))
		{
			return EnemiesKilledPerEnemyId[enemyId];
		}
		return 0;
	}
}
