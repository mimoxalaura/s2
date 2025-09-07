using System;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relic.RelicHandlers;

public class DamageWithHealthBelowPercentageRelicHandler : RelicHandler
{
	[SerializeField]
	private float _additionalDamagePercentage;

	[SerializeField]
	private float _miniumHealthPercentage;

	[SerializeField]
	private BuffSO _buffSO;

	private bool _inRageMode;

	public override void Execute()
	{
		if (SingletonController<GameController>.Instance.Player.HealthSystem.GetHealth() < SingletonController<GameController>.Instance.Player.HealthSystem.GetHealthMax() * _miniumHealthPercentage)
		{
			if (!_inRageMode)
			{
				SingletonController<GameController>.Instance.Player.AddBuff(_buffSO);
				_inRageMode = true;
			}
		}
		else
		{
			SingletonController<GameController>.Instance.Player.RemoveBuff(_buffSO);
			_inRageMode = false;
		}
	}

	public override void Setup(Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnPlayerHealthChanged += EventController_OnPlayerHealthChanged;
	}

	private void EventController_OnPlayerHealthChanged(object sender, EventArgs e)
	{
		Execute();
	}

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnPlayerHealthChanged -= EventController_OnPlayerHealthChanged;
	}

	public override void TearDown()
	{
		SingletonController<EventController>.Instance.OnPlayerHealthChanged -= EventController_OnPlayerHealthChanged;
	}
}
