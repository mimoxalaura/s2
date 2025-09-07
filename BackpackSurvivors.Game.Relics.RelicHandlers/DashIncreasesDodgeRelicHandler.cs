using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class DashIncreasesDodgeRelicHandler : RelicHandler
{
	[SerializeField]
	private BuffSO _buffAddedOnDash;

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnPlayerDashed -= EventController_OnPlayerDashed;
	}

	public override void Execute()
	{
		SingletonController<GameController>.Instance.Player.AddBuff(_buffAddedOnDash);
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnPlayerDashed += EventController_OnPlayerDashed;
	}

	private void EventController_OnPlayerDashed(DashCooldownEventArgs obj)
	{
		Execute();
	}

	public override void TearDown()
	{
	}
}
