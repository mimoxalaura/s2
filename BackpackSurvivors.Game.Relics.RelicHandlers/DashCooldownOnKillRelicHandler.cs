using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class DashCooldownOnKillRelicHandler : RelicHandler
{
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
		if (RandomHelper.GetRollSuccess(0.05f) && SingletonController<GameController>.Instance.Player.GetPlayerDash().TotalDashes > SingletonController<GameController>.Instance.Player.GetPlayerDash().CurrentDashes)
		{
			SingletonController<GameController>.Instance.Player.GetPlayerDash().ResetCooldown();
		}
	}

	public override void TearDown()
	{
	}
}
