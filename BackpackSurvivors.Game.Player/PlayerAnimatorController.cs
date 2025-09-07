using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

[RequireComponent(typeof(PlayerAnimatorStateMachine))]
public class PlayerAnimatorController : MonoBehaviour
{
	internal delegate void PlayerAnimationStartedHandler(object sender, PlayerAnimationStartedEventArgs e);

	internal delegate void PlayerAnimationCompletedHandler(object sender, PlayerAnimationCompletedEventArgs e);

	[SerializeField]
	private Player _player;

	[SerializeField]
	private SpriteRenderer _playerSpriteRenderer;

	[SerializeField]
	private GameObject _playerTopMask;

	[SerializeField]
	private GameObject _helmetMask;

	[SerializeField]
	private GameObject _deathWeaponInGroundMask;

	[SerializeField]
	private GameObject _deathFlag;

	[SerializeField]
	private GameObject _largeMeleeWeaponMask;

	[SerializeField]
	private GameObject _largeMeleeWeaponFeetMask;

	[SerializeField]
	private GameObject _playerShadow;

	[SerializeField]
	private GameObject _successBackdrop;

	[SerializeField]
	private GameObject _levelCompletedSuccesLight;

	[SerializeField]
	private Transform _visualEffectsContainer;

	[SerializeField]
	private Transform _buffVisualEffectsContainer;

	[SerializeField]
	private GameObject _dustPrefab;

	[SerializeField]
	private Transform _dustPrefabSpawnPoint;

	[Header("Spawning")]
	[SerializeField]
	private SpriteRenderer _portalSpriteRenderer;

	[SerializeField]
	private Material _spawningPortalMaterial;

	[SerializeField]
	private Material _despawningPortalMaterial;

	[SerializeField]
	private PlayerAnimatorStateMachine _playerAnimatorStateMachine;

	private bool _hasWeapon;

	private Enums.WeaponAnimationSize _weaponAnimationSize;

	private Enums.WeaponAnimationType _weaponAnimationType;

	private bool _animatingTriggeredAnimation;

	private bool _allowDamageTakenAnimation;

	internal event PlayerAnimationStartedHandler OnPlayerAnimationStarted;

	internal event PlayerAnimationCompletedHandler OnPlayerAnimationCompleted;

	private void Awake()
	{
		_playerAnimatorStateMachine.OnPlayerAnimationStarted += _playerAnimatorStateMachine_OnPlayerAnimationStarted;
		_playerAnimatorStateMachine.OnPlayerAnimationCompleted += _playerAnimatorStateMachine_OnPlayerAnimationCompleted;
	}

	public void InitializeStates(PlayerAnimationReference playerAnimationReference)
	{
		playerAnimationReference.TopMask = _playerTopMask;
		playerAnimationReference.HelmetMask = _helmetMask;
		playerAnimationReference.DeathFlag = _deathFlag;
		playerAnimationReference.DeathWeaponInGroundMask = _deathWeaponInGroundMask;
		playerAnimationReference.LargeMeleeWeaponMask = _largeMeleeWeaponMask;
		playerAnimationReference.LevelCompletedSuccesLight = _levelCompletedSuccesLight;
		playerAnimationReference.LargeMeleeWeaponFeetMask = _largeMeleeWeaponFeetMask;
		playerAnimationReference.PlayerShadow = _playerShadow;
		playerAnimationReference.SuccessBackdrop = _successBackdrop;
		playerAnimationReference.PlayerSpriteRenderer = _playerSpriteRenderer;
		playerAnimationReference.VisualEffectsContainer = _visualEffectsContainer;
		playerAnimationReference.BuffVisualEffectsContainer = _buffVisualEffectsContainer;
		playerAnimationReference.DustPrefab = _dustPrefab;
		playerAnimationReference.DustPrefabSpawnPoint = _dustPrefabSpawnPoint;
		playerAnimationReference.PortalSpriteRenderer = _portalSpriteRenderer;
		playerAnimationReference.SpawningPortalMaterial = _spawningPortalMaterial;
		playerAnimationReference.DespawningPortalMaterial = _despawningPortalMaterial;
		_playerAnimatorStateMachine.InitializeStates(playerAnimationReference);
	}

	private void _playerAnimatorStateMachine_OnPlayerAnimationStarted(object sender, PlayerAnimationStartedEventArgs e)
	{
		this.OnPlayerAnimationStarted?.Invoke(this, new PlayerAnimationStartedEventArgs(e.PlayerAnimationState));
	}

	private void _playerAnimatorStateMachine_OnPlayerAnimationCompleted(object sender, PlayerAnimationCompletedEventArgs e)
	{
		SetAnimatingTriggeredAnimation(e.PlayerAnimationState);
		SetAllowDamageTakenAnimation(e.PlayerAnimationState);
		this.OnPlayerAnimationCompleted?.Invoke(this, new PlayerAnimationCompletedEventArgs(e.PlayerAnimationState));
	}

	private void SetAllowDamageTakenAnimation(Enums.AnimatorStateMachineEnums.PlayerAnimationStates playerAnimationState)
	{
		_allowDamageTakenAnimation = true;
		switch (playerAnimationState)
		{
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.None:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.DamageTaken:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleNoWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeFireArmWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackNoWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeFireArmWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunNoWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeFireArmWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackNoWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeFireArmWeapon:
			_allowDamageTakenAnimation = true;
			break;
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Spawning:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Despawning:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dash:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dying:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dead:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Succes:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.ShowItem:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Revive:
			_allowDamageTakenAnimation = false;
			break;
		}
		_animatingTriggeredAnimation = false;
	}

	private void SetAnimatingTriggeredAnimation(Enums.AnimatorStateMachineEnums.PlayerAnimationStates playerAnimationState)
	{
		switch (playerAnimationState)
		{
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.None:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Spawning:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Despawning:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dash:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.ShowItem:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleNoWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeFireArmWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackNoWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeFireArmWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunNoWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeFireArmWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackNoWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallMeleeWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallSpellWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallBowWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallThrownWeapon:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeFireArmWeapon:
			_animatingTriggeredAnimation = false;
			break;
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.DamageTaken:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dying:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dead:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Succes:
		case Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Revive:
			_animatingTriggeredAnimation = true;
			break;
		}
		this.OnPlayerAnimationCompleted?.Invoke(this, new PlayerAnimationCompletedEventArgs(playerAnimationState));
		_animatingTriggeredAnimation = false;
	}

	public void AllowAnimationActiveDuringZeroTimescale(bool allow)
	{
		if (allow)
		{
			_playerAnimatorStateMachine.SetUpdateMode(AnimatorUpdateMode.UnscaledTime);
		}
		else
		{
			_playerAnimatorStateMachine.SetUpdateMode(AnimatorUpdateMode.Normal);
		}
	}

	public void ChangeWeapon(WeaponInstance weapon)
	{
		_hasWeapon = weapon != null;
		if (_hasWeapon)
		{
			_weaponAnimationSize = weapon.BaseWeaponSO.WeaponAnimationSize;
			_weaponAnimationType = weapon.BaseWeaponSO.WeaponAnimationType;
		}
		else
		{
			_weaponAnimationSize = Enums.WeaponAnimationSize.None;
			_weaponAnimationType = Enums.WeaponAnimationType.None;
		}
	}

	private void Update()
	{
		if (_animatingTriggeredAnimation)
		{
			return;
		}
		if (_player.IsDead)
		{
			ShowDyingAnimation();
		}
		if (!_player.CanAct)
		{
			return;
		}
		if (_player.IsMoving)
		{
			if (_player.IsAttackingWithVisualWeapon)
			{
				ShowRunAttackAnimation();
			}
			else
			{
				ShowRunAnimation();
			}
		}
		else if (_player.IsAttackingWithVisualWeapon)
		{
			ShowAttackAnimation();
		}
		else
		{
			ShowIdleAnimation();
		}
	}

	private void ShowAttackAnimation()
	{
		if (_hasWeapon)
		{
			if (_weaponAnimationSize == Enums.WeaponAnimationSize.Large)
			{
				switch (_weaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeMeleeWeapon);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeSpellWeapon);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeBowWeapon);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeThrownWeapon);
					break;
				case Enums.WeaponAnimationType.Firearm:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeFireArmWeapon);
					break;
				}
			}
			else
			{
				switch (_weaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallMeleeWeapon);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallSpellWeapon);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallBowWeapon);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallThrownWeapon);
					break;
				}
			}
		}
		else
		{
			_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackNoWeapon);
		}
		_allowDamageTakenAnimation = true;
	}

	private void ShowRunAttackAnimation()
	{
		if (_hasWeapon)
		{
			if (_weaponAnimationSize == Enums.WeaponAnimationSize.Large)
			{
				switch (_weaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeMeleeWeapon);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeSpellWeapon);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeBowWeapon);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeThrownWeapon);
					break;
				case Enums.WeaponAnimationType.Firearm:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeFireArmWeapon);
					break;
				}
			}
			else
			{
				switch (_weaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallMeleeWeapon);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallSpellWeapon);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallBowWeapon);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallThrownWeapon);
					break;
				}
			}
		}
		else
		{
			_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackNoWeapon);
		}
		_allowDamageTakenAnimation = true;
	}

	private void ShowRunAnimation()
	{
		if (_hasWeapon)
		{
			if (_weaponAnimationSize == Enums.WeaponAnimationSize.Large)
			{
				switch (_weaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeMeleeWeapon);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeSpellWeapon);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeBowWeapon);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeThrownWeapon);
					break;
				case Enums.WeaponAnimationType.Firearm:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeFireArmWeapon);
					break;
				}
			}
			else
			{
				switch (_weaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallMeleeWeapon);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallSpellWeapon);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallBowWeapon);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallThrownWeapon);
					break;
				}
			}
		}
		else
		{
			_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunNoWeapon);
		}
		_allowDamageTakenAnimation = true;
	}

	internal void ShowIdleAnimation()
	{
		if (_hasWeapon)
		{
			if (_weaponAnimationSize == Enums.WeaponAnimationSize.Large)
			{
				switch (_weaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeMeleeWeapon);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeSpellWeapon);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeBowWeapon);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeThrownWeapon);
					break;
				case Enums.WeaponAnimationType.Firearm:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeFireArmWeapon);
					break;
				}
			}
			else
			{
				switch (_weaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallMeleeWeapon);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallSpellWeapon);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallBowWeapon);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallThrownWeapon);
					break;
				}
			}
		}
		else
		{
			_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleNoWeapon);
		}
		_allowDamageTakenAnimation = true;
	}

	public void ShowSpawnAnimation()
	{
		_animatingTriggeredAnimation = true;
		_allowDamageTakenAnimation = false;
		_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Spawning);
	}

	public void ShowDespawnAnimation()
	{
		_animatingTriggeredAnimation = true;
		_hasWeapon = false;
		_allowDamageTakenAnimation = false;
		_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Despawning);
	}

	public void ShowShowItemAnimation()
	{
		_animatingTriggeredAnimation = true;
		_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.ShowItem);
	}

	public void ShowSuccesAnimation()
	{
		_animatingTriggeredAnimation = true;
		_allowDamageTakenAnimation = false;
		_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Succes);
	}

	public void ShowDyingAnimation()
	{
		_animatingTriggeredAnimation = true;
		_allowDamageTakenAnimation = false;
		_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dying);
	}

	public void ShowHitAnimation()
	{
		if (_allowDamageTakenAnimation)
		{
			_animatingTriggeredAnimation = true;
			_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.DamageTaken);
		}
	}

	public void ShowDashAnimation()
	{
		_animatingTriggeredAnimation = true;
		_allowDamageTakenAnimation = false;
		_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dash);
	}

	public void ShowReviveAnimation()
	{
		_animatingTriggeredAnimation = true;
		_allowDamageTakenAnimation = false;
		_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Revive);
	}

	internal void ForceAnimationReset()
	{
		_animatingTriggeredAnimation = false;
		_allowDamageTakenAnimation = true;
	}
}
