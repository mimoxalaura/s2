using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class AdditionalLootOnKillRelicHandler : RelicHandler
{
	[SerializeField]
	private float _additionalLootChance = 0.1f;

	[SerializeField]
	private int _additionalLootCount = 2;

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnEnemyKilled -= Instance_OnEnemyKilled;
	}

	public override void Execute()
	{
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnEnemyKilled += Instance_OnEnemyKilled;
	}

	private void Instance_OnEnemyKilled(object sender, KilledEventArgs e)
	{
		if (RandomHelper.GetRollSuccess(_additionalLootChance) && e.Character.IsEnemy)
		{
			for (int i = 0; i < _additionalLootCount; i++)
			{
				((Enemy)e.Character).TryDrops();
				((Enemy)e.Character).TryDrops();
			}
		}
	}

	public override void TearDown()
	{
	}
}
