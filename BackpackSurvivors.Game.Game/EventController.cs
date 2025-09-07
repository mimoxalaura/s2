using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Health;
using BackpackSurvivors.Game.Health.Events;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Pickups.Events;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Game;

internal class EventController : SingletonController<EventController>
{
	private List<WeaponAttack> _weaponAttacks = new List<WeaponAttack>();

	private List<CombatWeapon> _combatWeapons = new List<CombatWeapon>();

	private List<HealthPickup> _healthPickups = new List<HealthPickup>();

	private List<CoinPickup> _coinPickups = new List<CoinPickup>();

	internal event Action<CombatWeapon> OnWeaponKilledEnemy;

	internal event Action<WeaponAttackEventArgs> OnWeaponAttacked;

	internal event HealthSystem.HealthChangedEventHandler OnPlayerHealthChanged;

	internal event HealthSystem.HealthMaxChangedEventHandler OnPlayerHealthMaxChanged;

	internal event HealthPickup.HealthPickedUpHandler OnHealthPickedUp;

	internal event CoinPickup.CoinPickedUpHandler OnCoinPickedUp;

	internal event Enemy.EnemyDamagedHandler OnEnemyDamaged;

	internal event Enemy.EnemyDebuffedHandler OnEnemyDebuffed;

	internal event Character.KilledHandler OnEnemyKilled;

	internal event Action<DashCooldownEventArgs> OnPlayerDashed;

	internal event EventHandler OnPlayerLoaded;

	private void Start()
	{
		RegisterEvents();
		base.IsInitialized = true;
	}

	internal void RegisterEnemy(Enemy enemy)
	{
		enemy.OnEnemyDamaged += Enemy_OnEnemyDamaged;
		enemy.OnEnemyDebuffed += Enemy_OnEnemyDebuffed;
		enemy.OnKilled += Enemy_OnKilled;
	}

	private void Enemy_OnKilled(object sender, KilledEventArgs e)
	{
		this.OnEnemyKilled?.Invoke(this, e);
	}

	internal void RegisterWeaponAttackEvents(WeaponAttack weaponAttack)
	{
		_weaponAttacks.Add(weaponAttack);
		weaponAttack.OnEnemyKilled += WeaponAttack_OnEnemyKilled;
	}

	internal void RegisterCombatWeaponEvents(CombatWeapon combatWeapon)
	{
		_combatWeapons.Add(combatWeapon);
		combatWeapon.OnWeaponAttack += CombatWeapon_OnWeaponAttack;
	}

	internal void RegisterHealthPickupSpawned(HealthPickup healthPickup)
	{
		_healthPickups.Add(healthPickup);
		healthPickup.OnHealthPickedUp += HealthPickup_OnHealthPickedUp;
	}

	internal void RegisterCoinPickupSpawned(CoinPickup coinPickup)
	{
		_coinPickups.Add(coinPickup);
		coinPickup.OnCoinPickedUp += CoinPickup_OnCoinPickedUp;
	}

	private void CoinPickup_OnCoinPickedUp(object sender, CoinPickedUpEventArgs e)
	{
		this.OnCoinPickedUp?.Invoke(this, e);
	}

	private void Player_OnDashed(object sender, DashCooldownEventArgs e)
	{
		this.OnPlayerDashed?.Invoke(e);
	}

	private void HealthPickup_OnHealthPickedUp(object sender, HealthPickedUpEventArgs e)
	{
		this.OnHealthPickedUp?.Invoke(this, e);
	}

	private void CombatWeapon_OnWeaponAttack(object sender, WeaponAttackEventArgs e)
	{
		this.OnWeaponAttacked?.Invoke(e);
	}

	private void Enemy_OnEnemyDamaged(object sender, EnemyDamagedEventArgs e)
	{
		this.OnEnemyDamaged?.Invoke(this, e);
	}

	private void Enemy_OnEnemyDebuffed(object sender, EnemyDebuffedEventArgs e)
	{
		this.OnEnemyDebuffed?.Invoke(this, e);
	}

	internal void RegisterPlayer(BackpackSurvivors.Game.Player.Player player)
	{
		player.OnHealthChanged += Player_OnHealthChanged;
		player.OnHealthMaxChanged += Player_OnHealthMaxChanged;
		player.OnDashed += Player_OnDashed;
		player.OnLoaded += Player_OnLoaded;
	}

	internal void UnregisterPlayer(BackpackSurvivors.Game.Player.Player player)
	{
		player.OnHealthChanged -= Player_OnHealthChanged;
		player.OnHealthMaxChanged -= Player_OnHealthMaxChanged;
		player.OnDashed -= Player_OnDashed;
		player.OnLoaded -= Player_OnLoaded;
	}

	private void WeaponAttack_OnEnemyKilled(CombatWeapon combatWeapon, Enemy killedEnemy)
	{
		this.OnWeaponKilledEnemy?.Invoke(combatWeapon);
	}

	private void RegisterEvents()
	{
		SingletonController<SceneChangeController>.Instance.OnNewSceneLoading += SceneChangeController_OnNewSceneLoading;
	}

	private void SceneChangeController_OnNewSceneLoading()
	{
		UnregisterPlayerHealthChanged();
		UnregisterPlayerDashed();
		UnregisterWeaponAttackEvents();
		UnregisterCombatWeaponEvents();
		UnregisterHealthPickupEvents();
		UnregisterCoinPickupEvents();
	}

	private void UnregisterWeaponAttackEvents()
	{
		foreach (WeaponAttack weaponAttack in _weaponAttacks)
		{
			weaponAttack.OnEnemyKilled -= WeaponAttack_OnEnemyKilled;
		}
		_weaponAttacks.Clear();
	}

	private void UnregisterCombatWeaponEvents()
	{
		foreach (CombatWeapon combatWeapon in _combatWeapons)
		{
			combatWeapon.OnWeaponAttack -= CombatWeapon_OnWeaponAttack;
		}
		_combatWeapons.Clear();
	}

	private void UnregisterHealthPickupEvents()
	{
		foreach (HealthPickup healthPickup in _healthPickups)
		{
			healthPickup.OnHealthPickedUp -= HealthPickup_OnHealthPickedUp;
		}
		_healthPickups.Clear();
	}

	private void UnregisterCoinPickupEvents()
	{
		foreach (CoinPickup coinPickup in _coinPickups)
		{
			coinPickup.OnCoinPickedUp -= CoinPickup_OnCoinPickedUp;
		}
		_coinPickups.Clear();
	}

	private void UnregisterPlayerHealthChanged()
	{
		if (!(SingletonController<GameController>.Instance.Player == null))
		{
			SingletonController<GameController>.Instance.Player.OnHealthChanged -= Player_OnHealthChanged;
			SingletonController<GameController>.Instance.Player.OnHealthMaxChanged -= Player_OnHealthMaxChanged;
		}
	}

	private void UnregisterPlayerDashed()
	{
		if (!(SingletonController<GameController>.Instance.Player == null))
		{
			SingletonController<GameController>.Instance.Player.OnDashed -= Player_OnDashed;
		}
	}

	private void Player_OnHealthChanged(object sender, HealthChangedEventArgs e)
	{
		this.OnPlayerHealthChanged?.Invoke(this, e);
	}

	private void Player_OnHealthMaxChanged(object sender, HealthMaxChangedEventArgs e)
	{
		this.OnPlayerHealthMaxChanged?.Invoke(this, e);
	}

	private void Player_OnLoaded(object sender, EventArgs e)
	{
		this.OnPlayerLoaded?.Invoke(sender, e);
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
