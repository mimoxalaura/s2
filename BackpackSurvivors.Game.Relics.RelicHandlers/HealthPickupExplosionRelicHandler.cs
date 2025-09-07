using BackpackSurvivors.Game.Adventure.Interfaces;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Pickups.Events;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class HealthPickupExplosionRelicHandler : RelicHandler, IDPSLoggagable
{
	private float _defaultExplosionChance = 0.5f;

	[SerializeField]
	private WeaponSO _weaponSOExplosion;

	WeaponSO IDPSLoggagable.WeaponSO => _weaponSOExplosion;

	float IDPSLoggagable.ActiveSinceTime { get; set; }

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnHealthPickedUp -= EventController_OnHealthPickedUp;
	}

	public override void Execute()
	{
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnHealthPickedUp += EventController_OnHealthPickedUp;
	}

	private void EventController_OnHealthPickedUp(object sender, HealthPickedUpEventArgs e)
	{
		float calculatedStat = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.LuckPercentage);
		if (RandomHelper.GetRollSuccess(_defaultExplosionChance * calculatedStat))
		{
			CombatWeapon combatWeapon = Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab, base.transform);
			WeaponInstance weaponInstance = new WeaponInstance(_weaponSOExplosion);
			combatWeapon.Init(weaponInstance, 0f, SingletonController<GameController>.Instance.Player);
			WeaponAttack weaponAttack = Object.Instantiate(_weaponSOExplosion.WeaponAttackPrefab, SingletonController<GameController>.Instance.Player.transform.position, Quaternion.identity);
			weaponAttack.Init(damageInstance: new DamageInstance(_weaponSOExplosion.Damage), source: SingletonController<GameController>.Instance.Player, target: SingletonController<GameController>.Instance.Player, combatWeapon: combatWeapon);
			weaponAttack.SetCustomStartPosition(SingletonController<GameController>.Instance.Player.transform.position);
			weaponAttack.Activate(SingletonController<GameController>.Instance.Player.transform.position, canTriggerEffects: false, canTriggerDebuffs: false, SingletonController<GameController>.Instance.Player, SingletonController<GameController>.Instance.Player);
			Object.Destroy(combatWeapon.gameObject);
		}
	}

	public override void TearDown()
	{
	}
}
