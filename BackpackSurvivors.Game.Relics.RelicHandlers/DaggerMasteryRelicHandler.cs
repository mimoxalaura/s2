using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class DaggerMasteryRelicHandler : RelicHandler
{
	[SerializeField]
	private int _numberOfKillsForStatIncrease;

	[SerializeField]
	private Enums.PlaceableWeaponSubtype _weaponSubtypeFilter;

	[SerializeField]
	private BuffSO _buffSO;

	private int _currentKills;

	public override void BeforeDestroy()
	{
	}

	public override void Execute()
	{
		SingletonController<GameController>.Instance.Player.AddBuff(_buffSO);
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnWeaponKilledEnemy += EventController_OnWeaponKilledEnemy;
	}

	private void EventController_OnWeaponKilledEnemy(CombatWeapon combatWeapon)
	{
		if (combatWeapon.WeaponInstance.PlaceableWeaponSubtype == _weaponSubtypeFilter)
		{
			_currentKills++;
			if (_currentKills >= _numberOfKillsForStatIncrease)
			{
				Execute();
				_currentKills = 0;
			}
		}
	}

	public override void TearDown()
	{
		SingletonController<EventController>.Instance.OnWeaponKilledEnemy -= EventController_OnWeaponKilledEnemy;
	}
}
