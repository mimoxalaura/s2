using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Enemies;

public class EnemyKilledEventArgs : EventArgs
{
	public int EnemyId { get; }

	public Enemy Enemy { get; }

	public Enums.Enemies.EnemyType EnemyType { get; }

	public int TotalEnemiesKilled { get; }

	public EnemyKilledEventArgs(Enemy enemy, Enums.Enemies.EnemyType enemyType, int totalEnemiesKilled)
	{
		Enemy = enemy;
		EnemyType = enemyType;
		TotalEnemiesKilled = totalEnemiesKilled;
	}
}
