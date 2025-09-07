using System;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves.Events;

public class WaveOnEnemyKilledEventArgs : EventArgs
{
	public GameObject EnemyKilled { get; }

	public WaveOnEnemyKilledEventArgs(GameObject enemyKilled)
	{
		EnemyKilled = enemyKilled;
	}
}
