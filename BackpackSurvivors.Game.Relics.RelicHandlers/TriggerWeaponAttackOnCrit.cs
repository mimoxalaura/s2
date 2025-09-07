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

internal class TriggerWeaponAttackOnCrit : RelicHandler
{
	[SerializeField]
	private WeaponSO _weaponAttackOnCrit;

	public override void BeforeDestroy()
	{
		if (!(SingletonController<GameController>.Instance == null) && !(SingletonController<GameController>.Instance.Player == null) && !(SingletonController<EventController>.Instance == null))
		{
			SingletonController<EventController>.Instance.OnEnemyDamaged -= Instance_OnEnemyDamaged;
		}
	}

	public override void Execute()
	{
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnEnemyDamaged += Instance_OnEnemyDamaged;
	}

	private void Instance_OnEnemyDamaged(object sender, EnemyDamagedEventArgs e)
	{
		if (e.WasCrit)
		{
			CombatWeapon combatWeapon = Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab, base.transform);
			WeaponInstance weaponInstance = new WeaponInstance(_weaponAttackOnCrit);
			combatWeapon.Init(weaponInstance, 0f, e.DamageSource);
			WeaponAttack weaponAttack = Object.Instantiate(_weaponAttackOnCrit.WeaponAttackPrefab, e.Enemy.transform.position, Quaternion.identity);
			DamageInstance damageInstance = new DamageInstance(_weaponAttackOnCrit.Damage);
			damageInstance.SetMinMaxDamage(1f, 1f);
			weaponAttack.Init(e.DamageSource, e.Enemy, combatWeapon, damageInstance);
			weaponAttack.SetCustomStartPosition(e.Enemy.transform.position);
			weaponAttack.Activate(e.Enemy.transform.position, canTriggerEffects: false, canTriggerDebuffs: false, e.DamageSource, e.Enemy);
			Object.Destroy(combatWeapon.gameObject);
		}
	}

	public override void TearDown()
	{
	}
}
