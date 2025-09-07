using System.Collections.Generic;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Enemies.ExternalStats;

internal class ExternalEnemyStat
{
	internal Dictionary<Enums.ItemStatType, float> EnemyStats = new Dictionary<Enums.ItemStatType, float>();

	internal float MinDamage;

	internal float MaxDamage;

	internal int EnemyId { get; set; }
}
