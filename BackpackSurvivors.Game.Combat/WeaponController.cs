using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Combat.Events;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.Game.Talents;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat;

public class WeaponController : MonoBehaviour
{
	public delegate void WeaponRegisterHandler(object sender, WeaponRegisterEventArgs e);

	public delegate void WeaponReadyHandler(object sender, WeaponReadyEventArgs e);

	public delegate void WeaponCooldownUpdateHandler(object sender, WeaponCooldownUpdateEventArgs e);

	public delegate void WeaponsResetHandler(object sender, WeaponsResetEventArgs e);

	public delegate void WeaponAttackedHandler(object sender, WeaponAttackedEventArgs e);

	private StatController _statController;

	private bool _canAct;

	private List<CombatWeapon> _weapons = new List<CombatWeapon>();

	private CombatWeapon _combatWeaponPrefab;

	private BackpackSurvivors.Game.Player.Player _player;

	public event WeaponRegisterHandler OnWeaponRegister;

	public event WeaponReadyHandler OnWeaponReady;

	public event WeaponCooldownUpdateHandler OnWeaponCooldownUpdate;

	public event WeaponsResetHandler OnWeaponsReset;

	public event WeaponAttackedHandler OnWeaponAttacked;

	private void Start()
	{
		RegisterGameObjects();
		RegisterEvents();
		StartCoroutine(UpdateTimeCoroutine());
	}

	private IEnumerator UpdateTimeCoroutine()
	{
		float interval = 0.1f;
		while (true)
		{
			if (_canAct)
			{
				DecreaseCooldown(interval);
				ActivateWeaponsOnZeroCooldown();
			}
			yield return new WaitForSeconds(interval);
		}
	}

	internal void SetRandomWeaponToZeroCooldown()
	{
		if (_weapons.Count != 0)
		{
			int index = UnityEngine.Random.Range(0, _weapons.Count());
			CombatWeapon combatWeapon = _weapons[index];
			combatWeapon.DecreaseCooldown(combatWeapon.CurrentCooldown);
		}
	}

	private void RegisterEvents()
	{
		_player.OnKilled += Player_OnCharacterKilled;
		SingletonController<GameController>.Instance.OnPauseUpdated += GameController_OnPauseUpdated;
	}

	private void GameController_OnPauseUpdated(bool isPaused)
	{
		SetAllWeaponsPauseStatus(isPaused);
	}

	private void RegisterGameObjects()
	{
		_statController = GetStatController();
		_player = SingletonController<GameController>.Instance.Player;
		_combatWeaponPrefab = SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab;
	}

	private void Player_OnCharacterKilled(object sender, EventArgs e)
	{
		SetAllWeaponsPauseStatus(isPaused: true);
	}

	private void SetAllWeaponsPauseStatus(bool isPaused)
	{
		foreach (CombatWeapon weapon in _weapons)
		{
			weapon.SetIsPaused(isPaused);
		}
	}

	internal void ResetAllWeaponsAttackCount()
	{
		foreach (CombatWeapon weapon in _weapons)
		{
			weapon.ResetAttackCount();
		}
	}

	private void DecreaseCooldown(float timePassed)
	{
		foreach (CombatWeapon weapon in _weapons)
		{
			weapon.DecreaseCooldown(timePassed);
			this.OnWeaponCooldownUpdate?.Invoke(this, new WeaponCooldownUpdateEventArgs(weapon));
		}
	}

	private void ActivateWeaponsOnZeroCooldown()
	{
		foreach (CombatWeapon weapon in _weapons)
		{
			if (weapon.CanAttack())
			{
				StartCoroutine(weapon.Attack());
			}
		}
	}

	private StatController GetStatController()
	{
		if (_statController == null)
		{
			_statController = UnityEngine.Object.FindAnyObjectByType<StatController>();
		}
		return _statController;
	}

	private List<WeaponInstance> GetWeaponsFromBackpack()
	{
		return SingletonController<BackpackController>.Instance.GetWeaponsFromBackpack();
	}

	private List<ItemInstance> GetItemsFromBackpack()
	{
		return SingletonController<BackpackController>.Instance.GetItemsFromBackpack();
	}

	private List<RelicSO> GetRelics()
	{
		return SingletonController<RelicsController>.Instance.ActiveRelics.Select((BackpackSurvivors.Game.Relic.Relic x) => x.RelicSO).ToList();
	}

	private List<TalentSO> GetTalents()
	{
		return SingletonController<NewTalentController>.Instance.GetActiveTalents();
	}

	private void InitializeWeapons(List<WeaponInstance> weaponInstances)
	{
		this.OnWeaponsReset?.Invoke(this, new WeaponsResetEventArgs());
		foreach (CombatWeapon weapon in _weapons)
		{
			weapon.ResetAttackCount();
			weapon.OnWeaponAttack -= CombatWeapon_OnWeaponAttack;
			weapon.StopAllCoroutines();
			UnityEngine.Object.Destroy(weapon.gameObject);
		}
		_weapons = new List<CombatWeapon>();
		int num = -1;
		float num2 = 0f;
		float num3 = 0.5f;
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		foreach (WeaponInstance weaponInstance in weaponInstances.OrderBy((WeaponInstance x) => x.Id))
		{
			CombatWeapon combatWeapon = UnityEngine.Object.Instantiate(_combatWeaponPrefab, player.WeaponAttackTransform);
			num3 = Mathf.Clamp(weaponInstance.BaseWeaponSO.Stats.StatValues.TryGet(Enums.WeaponStatType.CooldownTime, 1f) / 8f, 0.5f, 100f);
			if (num == weaponInstance.Id)
			{
				int num4 = weaponInstances.Count((WeaponInstance x) => x.Id == weaponInstance.Id);
				float num5 = num3 / (float)num4;
				num2 += num5;
			}
			else
			{
				num2 = 0f;
				num = weaponInstance.Id;
			}
			combatWeapon.Init(weaponInstance, num2, player);
			combatWeapon.OnWeaponAttack += CombatWeapon_OnWeaponAttack;
			combatWeapon.OnBeforeWeaponAttack += CombatWeapon_OnBeforeWeaponAttack;
			combatWeapon.OnWeaponReady += CombatWeapon_OnWeaponReady;
			_weapons.Add(combatWeapon);
			this.OnWeaponRegister?.Invoke(this, new WeaponRegisterEventArgs(combatWeapon));
		}
	}

	private void CombatWeapon_OnWeaponReady(object sender, WeaponReadyEventArgs e)
	{
		this.OnWeaponReady?.Invoke(this, e);
	}

	private void RefreshStats(bool refreshDashes = true)
	{
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		GetStatController().RecalculateStats(player.GetComplexBaseStats(), GetWeaponsFromBackpack(), GetItemsFromBackpack(), out var playerStats, out var damageTypeValues, GetRelics(), GetTalents(), player.BuffStats, player.BaseDamageTypeValues, GetWeaponsFromStorage(), GetItemsFromStorage(), GetWeaponsFromShop(), GetItemsFromShop());
		player.UpdateStats(playerStats, healHealth: false, refreshDashes);
		player.UpdateDamageTypeValues(damageTypeValues);
	}

	private List<WeaponInstance> GetWeaponsFromStorage()
	{
		return SingletonController<BackpackController>.Instance.GetWeaponsFromStorage();
	}

	private List<ItemInstance> GetItemsFromStorage()
	{
		return SingletonController<BackpackController>.Instance.GetItemsFromStorage();
	}

	private List<WeaponInstance> GetWeaponsFromShop()
	{
		ShopController controllerByType = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		if (controllerByType == null)
		{
			return new List<WeaponInstance>();
		}
		return controllerByType.GetWeaponsFromShop();
	}

	private List<ItemInstance> GetItemsFromShop()
	{
		ShopController controllerByType = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		if (controllerByType == null)
		{
			return new List<ItemInstance>();
		}
		return controllerByType.GetItemsFromShop();
	}

	[Command("combat.start", Platform.AllPlatforms, MonoTargetType.Single)]
	public void ReloadAndStart()
	{
		RefreshStats();
		List<WeaponInstance> weaponsFromBackpack = GetWeaponsFromBackpack();
		ResetAllWeaponsAttackCount();
		InitializeWeapons(weaponsFromBackpack);
		ResetAllWeaponsAttackCount();
		_canAct = true;
	}

	public void RefreshWeapons(bool refreshDashes = true)
	{
		RefreshStats(refreshDashes);
		List<WeaponInstance> weaponsFromBackpack = GetWeaponsFromBackpack();
		foreach (CombatWeapon combatWeapon in _weapons)
		{
			combatWeapon.Refresh(weaponsFromBackpack.FirstOrDefault((WeaponInstance w) => w.Equals(combatWeapon.WeaponInstance)), SingletonController<GameController>.Instance.Player);
		}
	}

	private void CombatWeapon_OnWeaponAttack(object sender, WeaponAttackEventArgs e)
	{
		if (_canAct)
		{
			this.OnWeaponAttacked?.Invoke(this, new WeaponAttackedEventArgs(e.CombatWeapon));
		}
	}

	private void CombatWeapon_OnBeforeWeaponAttack(object sender, WeaponAttackEventArgs e)
	{
		_ = _canAct;
	}

	private void OnDestroy()
	{
		SingletonController<GameController>.Instance.OnPauseUpdated -= GameController_OnPauseUpdated;
		_player.OnKilled -= Player_OnCharacterKilled;
	}
}
