using System;
using BackpackSurvivors.Game.Enemies;

namespace BackpackSurvivors.Game.Waves.Events;

internal class BossSpawnedEventArgs : EventArgs
{
	internal Enemy Boss { get; private set; }

	public BossSpawnedEventArgs(Enemy boss)
	{
		Boss = boss;
	}
}
