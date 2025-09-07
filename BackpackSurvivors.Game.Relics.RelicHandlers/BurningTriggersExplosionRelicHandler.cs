using System.Linq;
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

internal class BurningTriggersExplosionRelicHandler : RelicHandler, IDPSLoggagable
{
	[SerializeField]
	private WeaponSO _weaponSOExplosion;

	[SerializeField]
	private bool _useHealthPercentageAsDamage;

	[SerializeField]
	private float _healthPercentage;

	WeaponSO IDPSLoggagable.WeaponSO => _weaponSOExplosion;

	float IDPSLoggagable.ActiveSinceTime { get; set; }

	public override void BeforeDestroy()
	{
		SingletonController<EventController>.Instance.OnEnemyKilled -= Instance_OnEnemyKilled;
	}

	public override void Execute()
	{
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnEnemyKilled += Instance_OnEnemyKilled;
	}

	private void Instance_OnEnemyKilled(object sender, KilledEventArgs e)
	{
		if (!(e.Character == null) && e.Character.GetDebuffContainer().DebuffsOnDeath.Any((Enums.Debuff.DebuffType x) => x == Enums.Debuff.DebuffType.Burn))
		{
			float num = 1f;
			float num2 = 1f;
			if (_useHealthPercentageAsDamage)
			{
				float healthMax = e.Character.HealthSystem.GetHealthMax();
				num = healthMax * _healthPercentage;
				num2 = healthMax * _healthPercentage;
			}
			else
			{
				num = _weaponSOExplosion.Damage.BaseMinDamage;
				num2 = _weaponSOExplosion.Damage.BaseMinDamage;
			}
			CombatWeapon combatWeapon = Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab, base.transform);
			WeaponInstance weaponInstance = new WeaponInstance(_weaponSOExplosion);
			combatWeapon.Init(weaponInstance, 0f, e.DamageSource);
			WeaponAttack weaponAttack = Object.Instantiate(_weaponSOExplosion.WeaponAttackPrefab, e.Character.transform.position, Quaternion.identity);
			DamageInstance damageInstance = new DamageInstance(_weaponSOExplosion.Damage);
			damageInstance.SetMinMaxDamage(num, num2);
			weaponAttack.Init(e.DamageSource, e.Character, combatWeapon, damageInstance);
			weaponAttack.SetCustomStartPosition(e.Character.transform.position);
			weaponAttack.Activate(e.Character.transform.position, canTriggerEffects: false, canTriggerDebuffs: false, e.DamageSource, e.Character);
			Object.Destroy(combatWeapon.gameObject);
		}
	}

	public override void TearDown()
	{
	}
}
