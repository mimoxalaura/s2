using BackpackSurvivors.Game.Adventure.Interfaces;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class TwilightRelicHandler : RelicHandler, IDPSLoggagable
{
	[SerializeField]
	private DebuffSO _twilightDebuff;

	[SerializeField]
	private DebuffSO _gloomDebuff;

	[SerializeField]
	private WeaponSO _twilightExplosionSO;

	WeaponSO IDPSLoggagable.WeaponSO => _twilightExplosionSO;

	float IDPSLoggagable.ActiveSinceTime { get; set; }

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnEnemyDamaged -= EventController_OnEnemyDamaged;
	}

	public override void Execute()
	{
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnEnemyDamaged += EventController_OnEnemyDamaged;
	}

	private void EventController_OnEnemyDamaged(object sender, EnemyDamagedEventArgs e)
	{
		if (e.DamageType == Enums.DamageType.Void)
		{
			DebuffHandler debuffHandler = new DebuffHandler();
			debuffHandler.Init(_gloomDebuff, e.Enemy);
			e.Enemy.AddDebuff(debuffHandler, SingletonController<GameController>.Instance.Player);
		}
		if (e.DamageType == Enums.DamageType.Holy)
		{
			DebuffHandler debuffHandler2 = new DebuffHandler();
			debuffHandler2.Init(_twilightDebuff, e.Enemy);
			e.Enemy.AddDebuff(debuffHandler2, SingletonController<GameController>.Instance.Player);
		}
		if (e.Enemy.GetDebuffStacks(_twilightDebuff) > 0 && e.Enemy.GetDebuffStacks(_gloomDebuff) > 0)
		{
			e.Enemy.RemoveDebuff(Enums.Debuff.DebuffType.Twilight);
			e.Enemy.RemoveDebuff(Enums.Debuff.DebuffType.Gloom);
			CombatWeapon combatWeapon = Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab, base.transform);
			WeaponInstance weaponInstance = new WeaponInstance(_twilightExplosionSO);
			combatWeapon.Init(weaponInstance, 0f, e.DamageSource);
			WeaponAttack weaponAttack = Object.Instantiate(_twilightExplosionSO.WeaponAttackPrefab, e.Enemy.transform.position, Quaternion.identity);
			weaponAttack.Init(damageInstance: new DamageInstance(_twilightExplosionSO.Damage), source: e.DamageSource, target: e.Enemy, combatWeapon: combatWeapon);
			weaponAttack.SetCustomStartPosition(e.Enemy.transform.position);
			weaponAttack.Activate(e.Enemy.transform.position, canTriggerEffects: false, canTriggerDebuffs: false, e.DamageSource, e.Enemy);
			Object.Destroy(combatWeapon.gameObject);
		}
	}

	public override void TearDown()
	{
	}
}
