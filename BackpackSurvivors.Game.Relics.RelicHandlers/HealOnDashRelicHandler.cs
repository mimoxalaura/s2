using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class HealOnDashRelicHandler : RelicHandler
{
	[SerializeField]
	private float _healPercentageOnDash;

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnPlayerDashed -= EventController_OnPlayerDashed;
	}

	public override void Execute()
	{
		float healthMax = SingletonController<GameController>.Instance.Player.HealthSystem.GetHealthMax();
		if (!(healthMax <= SingletonController<GameController>.Instance.Player.HealthSystem.GetHealth()))
		{
			int num = SingletonController<RelicsController>.Instance.ActiveRelics.Count((BackpackSurvivors.Game.Relic.Relic x) => x.RelicSO.Id == base.Relic.RelicSO.Id);
			float num2 = healthMax * _healPercentageOnDash;
			SingletonController<GameController>.Instance.Player.DamageVisualizer.ShowTextPopup($"healed {(int)(num2 * (float)num)}", Constants.Colors.PositiveEffectColor, 2f);
			SingletonController<GameController>.Instance.Player.HealthSystem.Heal(num2);
		}
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
