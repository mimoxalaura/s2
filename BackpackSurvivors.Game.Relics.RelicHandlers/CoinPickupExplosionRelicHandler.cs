using BackpackSurvivors.Assets.Game.Combat.Droppables;
using BackpackSurvivors.Game.Adventure.Interfaces;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class CoinPickupExplosionRelicHandler : RelicHandler, IDPSLoggagable
{
	private int _coinsPickedUp;

	[SerializeField]
	private Explosion _explosionPrefab;

	[SerializeField]
	private WeaponSO _coinExplosionWeaponSO;

	WeaponSO IDPSLoggagable.WeaponSO => _coinExplosionWeaponSO;

	float IDPSLoggagable.ActiveSinceTime { get; set; }

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnCoinPickedUp -= EventController_OnCoinPickedUp;
	}

	public override void Execute()
	{
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnCoinPickedUp += EventController_OnCoinPickedUp;
	}

	private void EventController_OnCoinPickedUp(object sender, CoinPickedUpEventArgs e)
	{
		_coinsPickedUp++;
		if (_coinsPickedUp >= 66)
		{
			_coinsPickedUp = 0;
			CombatWeapon combatWeapon = Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab, base.transform);
			WeaponInstance weaponInstance = new WeaponInstance(_coinExplosionWeaponSO);
			combatWeapon.Init(weaponInstance, 0f, SingletonController<GameController>.Instance.Player);
			WeaponAttack weaponAttack = Object.Instantiate(_coinExplosionWeaponSO.WeaponAttackPrefab, SingletonController<GameController>.Instance.Player.transform.position, Quaternion.identity);
			weaponAttack.Init(damageInstance: new DamageInstance(_coinExplosionWeaponSO.Damage), source: SingletonController<GameController>.Instance.Player, target: SingletonController<GameController>.Instance.Player, combatWeapon: combatWeapon);
			weaponAttack.SetCustomStartPosition(SingletonController<GameController>.Instance.Player.transform.position);
			weaponAttack.Activate(SingletonController<GameController>.Instance.Player.transform.position, canTriggerEffects: false, canTriggerDebuffs: false, SingletonController<GameController>.Instance.Player, SingletonController<GameController>.Instance.Player);
			Object.Destroy(combatWeapon.gameObject);
		}
	}

	public override void TearDown()
	{
	}
}
