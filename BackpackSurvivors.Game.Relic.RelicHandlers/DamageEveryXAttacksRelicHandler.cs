using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relic.RelicHandlers;

public class DamageEveryXAttacksRelicHandler : RelicHandler
{
	[SerializeField]
	private float _attackCountForAttack;

	[SerializeField]
	private GameObject _whirlwindAttackPrefab;

	private int _attackCounter;

	private WeaponController _weaponController;

	public override void Setup(Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnWeaponAttacked += EventController_OnWeaponAttacked;
	}

	private void EventController_OnWeaponAttacked(WeaponAttackEventArgs e)
	{
		_attackCounter++;
		if ((float)_attackCounter % _attackCountForAttack == 0f)
		{
			Execute();
		}
	}

	public override void Execute()
	{
		Object.Instantiate(_whirlwindAttackPrefab, SingletonController<GameController>.Instance.PlayerPosition, Quaternion.identity);
	}

	public override void BeforeDestroy()
	{
		UnregisterEvents();
	}

	public override void TearDown()
	{
		UnregisterEvents();
	}

	private void UnregisterEvents()
	{
		SingletonController<EventController>.Instance.OnWeaponAttacked -= EventController_OnWeaponAttacked;
	}
}
