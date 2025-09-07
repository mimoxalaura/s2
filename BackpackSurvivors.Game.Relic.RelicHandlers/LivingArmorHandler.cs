using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Health.Events;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relic.RelicHandlers;

public class LivingArmorHandler : RelicHandler
{
	[SerializeField]
	private int _cooldownForRelicToTrigger;

	[SerializeField]
	private int _armorToIncreaseOnTrigger;

	[SerializeField]
	private float _timeArmorIsIncreased;

	[SerializeField]
	private BuffSO _buffSO;

	private float _lastTriggerTime;

	private float _lastHealthOnTrigger;

	public override void Setup(Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnPlayerHealthChanged += EventController_OnPlayerHealthChanged;
	}

	private void EventController_OnPlayerHealthChanged(object sender, HealthChangedEventArgs e)
	{
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		if (CanTrigger(player))
		{
			Execute();
		}
	}

	private bool CanTrigger(BackpackSurvivors.Game.Player.Player player)
	{
		bool num = Time.time > _lastTriggerTime + (float)_cooldownForRelicToTrigger;
		bool flag = _lastHealthOnTrigger != player.HealthSystem.GetHealth();
		bool flag2 = player.HealthSystem.GetHealth() < player.HealthSystem.GetHealthMax();
		return num && flag && flag2;
	}

	public override void BeforeDestroy()
	{
		StopAllCoroutines();
		SingletonController<EventController>.Instance.OnPlayerHealthChanged -= EventController_OnPlayerHealthChanged;
	}

	public override void Execute()
	{
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		_lastTriggerTime = Time.time;
		_lastHealthOnTrigger = player.HealthSystem.GetHealth();
		player.AddBuff(_buffSO);
	}

	public override void TearDown()
	{
		SingletonController<EventController>.Instance.OnPlayerHealthChanged -= EventController_OnPlayerHealthChanged;
	}
}
