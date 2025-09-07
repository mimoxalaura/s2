using System.Collections;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Effects.Core;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class DropOnDashRelicHandler : RelicHandler
{
	[SerializeField]
	private LingeringEffect _droppableOnDashPrefab;

	[SerializeField]
	private Enums.LingeringEffectType _lingeringEffect;

	[SerializeField]
	private int _droppableOnDashAmount = 1;

	[SerializeField]
	private float _delayBetweenDroppable = 0.05f;

	[SerializeField]
	private float _droppableOnDashDuration = 3f;

	[SerializeField]
	private DamageSO _droppableDamageSO;

	[SerializeField]
	private WeaponSO _droppableWeaponSO;

	public override void Execute()
	{
		base.Execute();
		StartCoroutine(DropOverDashTime());
	}

	private IEnumerator DropOverDashTime()
	{
		for (int i = 0; i < _droppableOnDashAmount; i++)
		{
			if (!ShouldPreventSpawning(SingletonController<GameController>.Instance.PlayerPosition))
			{
				LingeringEffect lingeringEffect = Object.Instantiate(_droppableOnDashPrefab, SingletonController<GameController>.Instance.Player.Transform);
				lingeringEffect.OnLingeringEffectTriggered += Droppable_OnLingeringEffectTriggered;
				lingeringEffect.Init(SingletonController<GameController>.Instance.Player, null);
				lingeringEffect.SetDuration(_droppableOnDashDuration);
				lingeringEffect.Activate();
				lingeringEffect.transform.SetParent(null);
			}
			yield return new WaitForSeconds(_delayBetweenDroppable);
		}
	}

	public bool ShouldPreventSpawning(Vector2 position)
	{
		return SingletonController<LingeringEffectsController>.Instance.ShouldPreventSpawning(_lingeringEffect, position, 1f);
	}

	private void Droppable_OnLingeringEffectTriggered(object sender, LingeringEffectTriggeredEventArgs e)
	{
		DamageInstance damageInstance = new DamageInstance(_droppableDamageSO);
		float damage = DamageEngine.CalculateIndirectDamage(SingletonController<GameController>.Instance.Player.CalculatedStats, e.TriggeredOn.CalculatedStats, SingletonController<GameController>.Instance.Player.CalculatedDamageTypeValues, 1, damageInstance, damageInstance, e.TriggeredOn.GetCharacterType());
		e.TriggeredOn.Damage(damage, wasCrit: false, null, 0f, null, damageInstance.CalculatedDamageType, SingletonController<GameController>.Instance.Player);
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
		SingletonController<EventController>.Instance.OnPlayerDashed -= EventController_OnPlayerDashed;
	}
}
