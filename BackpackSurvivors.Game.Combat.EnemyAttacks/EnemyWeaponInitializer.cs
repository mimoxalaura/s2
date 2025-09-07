using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.EnemyAttacks;

internal class EnemyWeaponInitializer : MonoBehaviour
{
	[SerializeField]
	private List<WeaponSO> _weaponSOs;

	[SerializeField]
	private SerializableDictionaryBase<int, float> _weaponAttackChances;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private string _attackAnimationParameter;

	[SerializeField]
	private Transform _attackStartPoint;

	[Header("Movement")]
	[SerializeField]
	private bool _stopMovementOnAttack;

	[SerializeField]
	private float _stopMovementTime;

	[SerializeField]
	private Enemy _character;

	private Enemy _enemy;

	private List<CombatWeapon> _combatWeapons = new List<CombatWeapon>();

	private bool _isActive = true;

	private void Awake()
	{
		RegisterComponents();
	}

	private void Start()
	{
		InitWeapon();
		RegisterEvents();
		StartCoroutine(UpdateTimeCoroutine());
	}

	internal void Pause()
	{
		_isActive = false;
	}

	private IEnumerator UpdateTimeCoroutine()
	{
		float interval = 0.1f;
		while (_isActive)
		{
			DecreaseCooldown(interval);
			AttackOnZeroCooldown();
			yield return new WaitForSeconds(interval);
		}
	}

	private void RegisterEvents()
	{
		SingletonController<GameController>.Instance.OnPauseUpdated += GameController_OnPauseUpdated;
		SingletonController<GameController>.Instance.Player.OnKilled += Player_OnCharacterKilled;
	}

	private void TimeBasedLevelController_OnLevelCompleted(object sender, EventArgs e)
	{
		SetAllWeaponsPauseStatus(isPaused: true);
	}

	private void Player_OnCharacterKilled(object sender, EventArgs e)
	{
		Pause();
		SetAllWeaponsPauseStatus(isPaused: true);
	}

	private void GameController_OnPauseUpdated(bool isPaused)
	{
		SetAllWeaponsPauseStatus(isPaused);
	}

	private void SetAllWeaponsPauseStatus(bool isPaused)
	{
		foreach (CombatWeapon combatWeapon in _combatWeapons)
		{
			combatWeapon.SetIsPaused(isPaused);
		}
	}

	private void DecreaseCooldown(float timePassed)
	{
		foreach (CombatWeapon combatWeapon in _combatWeapons)
		{
			combatWeapon.DecreaseCooldown(timePassed);
		}
	}

	private void AttackOnZeroCooldown()
	{
		foreach (CombatWeapon combatWeapon in _combatWeapons)
		{
			if (!combatWeapon.CanAttack() || _enemy.IsCurrentlyAttacking || (_weaponAttackChances.ContainsKey(combatWeapon.WeaponInstance.BaseWeaponSO.Id) && !RandomHelper.GetRollSuccess(_weaponAttackChances[combatWeapon.WeaponInstance.BaseWeaponSO.Id])))
			{
				continue;
			}
			float num = Vector2.Distance(base.transform.position, SingletonController<GameController>.Instance.PlayerPosition);
			if (combatWeapon.WeaponInstance.BaseWeaponSO.WeaponAttackPrefab.ProjectilePrefab.ProjectileMovement == Enums.ProjectileMovement.RotatingAroundStartPosition || !(combatWeapon.WeaponStats.GetValueOrDefault(Enums.WeaponStatType.WeaponRange) < num))
			{
				if (_animator != null && !string.IsNullOrEmpty(_attackAnimationParameter))
				{
					_animator.SetTrigger(_attackAnimationParameter);
				}
				_enemy.IsCurrentlyAttacking = true;
				StartCoroutine(combatWeapon.Attack());
				if (_stopMovementOnAttack)
				{
					_enemy.StopMovementForAmountOfTime(_stopMovementTime, fromAttack: true);
				}
			}
		}
	}

	private void RegisterComponents()
	{
		_enemy = GetComponentInParent<Enemy>();
	}

	private void InitWeapon()
	{
		float currentLevelEnemyDamageMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetCurrentLevelEnemyDamageMultiplierFromHellfire();
		foreach (WeaponSO weaponSO in _weaponSOs)
		{
			WeaponInstance weaponInstance = new WeaponInstance(weaponSO);
			CombatWeapon combatWeaponPrefab = SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab;
			CombatWeapon combatWeapon = ((!(_attackStartPoint != null)) ? UnityEngine.Object.Instantiate(combatWeaponPrefab, _enemy.transform, worldPositionStays: false) : UnityEngine.Object.Instantiate(combatWeaponPrefab, _attackStartPoint, worldPositionStays: false));
			combatWeapon.Init(weaponInstance, 0f, _enemy);
			combatWeapon.ScaleWeaponDamage(currentLevelEnemyDamageMultiplierFromHellfire);
			_combatWeapons.Add(combatWeapon);
		}
	}

	private void OnDestroy()
	{
		if (base.isActiveAndEnabled)
		{
			SingletonController<GameController>.Instance.OnPauseUpdated -= GameController_OnPauseUpdated;
			SingletonController<GameController>.Instance.Player.OnKilled -= Player_OnCharacterKilled;
		}
	}
}
