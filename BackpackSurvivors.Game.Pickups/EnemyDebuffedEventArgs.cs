using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Pickups;

internal class EnemyDebuffedEventArgs : EventArgs
{
	public Enemy Enemy { get; }

	public List<Enums.Debuff.DebuffType> DebuffsCaused { get; }

	public EnemyDebuffedEventArgs(Enemy enemy, List<Enums.Debuff.DebuffType> debuffsCaused)
	{
		Enemy = enemy;
		DebuffsCaused = debuffsCaused;
	}
}
