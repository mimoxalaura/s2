using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class PoisoningIncreasesPhysicalDamageRelicHandler : RelicHandler
{
	[SerializeField]
	private BuffSO _buffAddedOnDebuff;

	[SerializeField]
	private Enums.Debuff.DebuffType _debuffToTriggerOn;

	public override void BeforeDestroy()
	{
		SingletonController<GameController>.Instance.Player.RemoveBuff(_buffAddedOnDebuff);
		SingletonController<EventController>.Instance.OnEnemyDebuffed -= Instance_OnEnemyDebuffed;
	}

	public override void Execute()
	{
		SingletonController<GameController>.Instance.Player.AddBuff(_buffAddedOnDebuff);
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnEnemyDebuffed += Instance_OnEnemyDebuffed;
	}

	private void Instance_OnEnemyDebuffed(object sender, EnemyDebuffedEventArgs e)
	{
		if (e.DebuffsCaused != null && e.DebuffsCaused.Any() && e.DebuffsCaused.Contains(_debuffToTriggerOn))
		{
			Execute();
		}
	}

	public override void TearDown()
	{
	}
}
