using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class DecreaseCooldownOnDashRelicHandler : RelicHandler
{
	private WeaponController _weaponController;

	private WeaponController WeaponController
	{
		get
		{
			if (_weaponController == null)
			{
				_weaponController = SingletonCacheController.Instance.GetControllerByType<WeaponController>();
			}
			return _weaponController;
		}
	}

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnPlayerDashed -= EventController_OnPlayerDashed;
	}

	public override void Execute()
	{
		WeaponController.SetRandomWeaponToZeroCooldown();
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnPlayerDashed += EventController_OnPlayerDashed;
	}

	private void EventController_OnPlayerDashed(DashCooldownEventArgs args)
	{
		Execute();
	}

	public override void TearDown()
	{
	}
}
