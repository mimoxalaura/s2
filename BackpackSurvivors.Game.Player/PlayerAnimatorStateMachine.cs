using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerAnimatorStateMachine : MonoBehaviour
{
	internal delegate void PlayerAnimationStartedHandler(object sender, PlayerAnimationStartedEventArgs e);

	internal delegate void PlayerAnimationCompletedHandler(object sender, PlayerAnimationCompletedEventArgs e);

	[SerializeField]
	private Player _player;

	[Header("Animator")]
	[SerializeField]
	private AnimationClip _spawning;

	[SerializeField]
	private AnimationClip _despawning;

	[SerializeField]
	private AnimationClip _idleNoWeapon;

	[SerializeField]
	private AnimationClip _idleLargeMeleeWeapon;

	[SerializeField]
	private AnimationClip _idleSmallMeleeWeapon;

	[SerializeField]
	private AnimationClip _idleLargeSpellWeapon;

	[SerializeField]
	private AnimationClip _idleSmallSpellWeapon;

	[SerializeField]
	private AnimationClip _idleLargeBowWeapon;

	[SerializeField]
	private AnimationClip _idleSmallBowWeapon;

	[SerializeField]
	private AnimationClip _idleLargeThrownWeapon;

	[SerializeField]
	private AnimationClip _idleSmallThrownWeapon;

	[SerializeField]
	private AnimationClip _idleLargeFireArmWeapon;

	[SerializeField]
	private AnimationClip _attackNoWeapon;

	[SerializeField]
	private AnimationClip _attackLargeMeleeWeapon;

	[SerializeField]
	private AnimationClip _attackSmallMeleeWeapon;

	[SerializeField]
	private AnimationClip _attackLargeSpellWeapon;

	[SerializeField]
	private AnimationClip _attackSmallSpellWeapon;

	[SerializeField]
	private AnimationClip _attackLargeBowWeapon;

	[SerializeField]
	private AnimationClip _attackSmallBowWeapon;

	[SerializeField]
	private AnimationClip _attackLargeThrownWeapon;

	[SerializeField]
	private AnimationClip _attackSmallThrownWeapon;

	[SerializeField]
	private AnimationClip _attackLargeFireArmWeapon;

	[SerializeField]
	private AnimationClip _runNoWeapon;

	[SerializeField]
	private AnimationClip _runLargeMeleeWeapon;

	[SerializeField]
	private AnimationClip _runSmallMeleeWeapon;

	[SerializeField]
	private AnimationClip _runLargeSpellWeapon;

	[SerializeField]
	private AnimationClip _runSmallSpellWeapon;

	[SerializeField]
	private AnimationClip _runLargeBowWeapon;

	[SerializeField]
	private AnimationClip _runSmallBowWeapon;

	[SerializeField]
	private AnimationClip _runLargeThrownWeapon;

	[SerializeField]
	private AnimationClip _runSmallThrownWeapon;

	[SerializeField]
	private AnimationClip _runLargeFireArmWeapon;

	[SerializeField]
	private AnimationClip _runAttackNoWeapon;

	[SerializeField]
	private AnimationClip _runAttackLargeMeleeWeapon;

	[SerializeField]
	private AnimationClip _runAttackSmallMeleeWeapon;

	[SerializeField]
	private AnimationClip _runAttackLargeSpellWeapon;

	[SerializeField]
	private AnimationClip _runAttackSmallSpellWeapon;

	[SerializeField]
	private AnimationClip _runAttackLargeBowWeapon;

	[SerializeField]
	private AnimationClip _runAttackSmallBowWeapon;

	[SerializeField]
	private AnimationClip _runAttackLargeThrownWeapon;

	[SerializeField]
	private AnimationClip _runAttackSmallThrownWeapon;

	[SerializeField]
	private AnimationClip _runAttackLargeFireArmWeapon;

	[SerializeField]
	private AnimationClip _damageTaken;

	[SerializeField]
	private AnimationClip _dying;

	[SerializeField]
	private AnimationClip _dead;

	[SerializeField]
	private AnimationClip _succes;

	[SerializeField]
	private AnimationClip _showItem;

	[SerializeField]
	private AnimationClip _dash;

	[SerializeField]
	private AnimationClip _revive;

	private Dictionary<Enums.AnimatorStateMachineEnums.PlayerAnimationStates, PlayerAnimationState> _playerAnimationStates;

	private Animator _animator;

	private Enums.AnimatorStateMachineEnums.PlayerAnimationStates _currentAnimationState;

	internal event PlayerAnimationStartedHandler OnPlayerAnimationStarted;

	internal event PlayerAnimationCompletedHandler OnPlayerAnimationCompleted;

	private void Showspawning()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Spawning);
	}

	private void Showdespawning()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Despawning);
	}

	private void ShowidleNoWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleNoWeapon);
	}

	private void ShowidleLargeMeleeWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeMeleeWeapon);
	}

	private void ShowidleSmallMeleeWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallMeleeWeapon);
	}

	private void ShowidleLargeSpellWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeSpellWeapon);
	}

	private void ShowidleSmallSpellWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallSpellWeapon);
	}

	private void ShowidleLargeBowWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeBowWeapon);
	}

	private void ShowidleSmallBowWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallBowWeapon);
	}

	private void ShowidleSmallThrownWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallThrownWeapon);
	}

	private void ShowidleLargeThrownWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeThrownWeapon);
	}

	private void ShowattackNoWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackNoWeapon);
	}

	private void ShowattackLargeMeleeWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeMeleeWeapon);
	}

	private void ShowattackSmallMeleeWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallMeleeWeapon);
	}

	private void ShowattackLargeSpellWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeSpellWeapon);
	}

	private void ShowattackSmallSpellWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallSpellWeapon);
	}

	private void ShowattackLargeBowWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeBowWeapon);
	}

	private void ShowattackSmallBowWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallBowWeapon);
	}

	private void ShowattackLargeThrownWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeThrownWeapon);
	}

	private void ShowattackSmallThrownWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallThrownWeapon);
	}

	private void ShowrunNoWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunNoWeapon);
	}

	private void ShowrunLargeMeleeWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeMeleeWeapon);
	}

	private void ShowrunSmallMeleeWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallMeleeWeapon);
	}

	private void ShowrunLargeSpellWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeSpellWeapon);
	}

	private void ShowrunSmallSpellWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallSpellWeapon);
	}

	private void ShowrunLargeBowWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeBowWeapon);
	}

	private void ShowrunSmallBowWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallBowWeapon);
	}

	private void ShowrunLargeThrownWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeThrownWeapon);
	}

	private void ShowrunSmallThrownWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallThrownWeapon);
	}

	private void ShowrunAttackNoWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackNoWeapon);
	}

	private void ShowrunAttackLargeMeleeWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeMeleeWeapon);
	}

	private void ShowrunAttackSmallMeleeWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallMeleeWeapon);
	}

	private void ShowrunAttackLargeSpellWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeSpellWeapon);
	}

	private void ShowrunAttackSmallSpellWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallSpellWeapon);
	}

	private void ShowrunAttackLargeBowWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeBowWeapon);
	}

	private void ShowrunAttackSmallBowWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallBowWeapon);
	}

	private void ShowrunAttackLargeThrownWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeThrownWeapon);
	}

	private void ShowrunAttackSmallThrownWeapon()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallThrownWeapon);
	}

	private void ShowdamageTaken()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.DamageTaken);
	}

	private void Showdying()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dying);
	}

	private void Showdead()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dead);
	}

	private void Showsucces()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Succes);
	}

	private void ShowshowItem()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.ShowItem);
	}

	private void Showdash()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dash);
	}

	private void ShowRevive()
	{
		ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Revive);
	}

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	public void InitializeStates(PlayerAnimationReference playerAnimationReference)
	{
		_playerAnimationStates = new Dictionary<Enums.AnimatorStateMachineEnums.PlayerAnimationStates, PlayerAnimationState>();
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.None, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.None, null, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleNoWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleNoWeapon, _idleNoWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallMeleeWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallMeleeWeapon, _idleSmallMeleeWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeMeleeWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeMeleeWeapon, _idleLargeMeleeWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeSpellWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeSpellWeapon, _idleLargeSpellWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallSpellWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallSpellWeapon, _idleSmallSpellWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeBowWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeBowWeapon, _idleLargeBowWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallBowWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallBowWeapon, _idleSmallBowWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeThrownWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeThrownWeapon, _idleLargeThrownWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallThrownWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallThrownWeapon, _idleSmallThrownWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeFireArmWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeFireArmWeapon, _idleLargeFireArmWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunNoWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunNoWeapon, _runNoWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallMeleeWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallMeleeWeapon, _runSmallMeleeWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeMeleeWeapon, new PlayerRunningAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeMeleeWeapon, _runLargeMeleeWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeSpellWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeSpellWeapon, _runLargeSpellWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallSpellWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallSpellWeapon, _runSmallSpellWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeBowWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeBowWeapon, _runLargeBowWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallBowWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallBowWeapon, _runSmallBowWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeThrownWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeThrownWeapon, _runLargeThrownWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallThrownWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallThrownWeapon, _runSmallThrownWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeFireArmWeapon, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeFireArmWeapon, _runLargeFireArmWeapon, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackNoWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackNoWeapon, _attackNoWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.None], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallMeleeWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallMeleeWeapon, _attackSmallMeleeWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallMeleeWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeMeleeWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeMeleeWeapon, _attackLargeMeleeWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeMeleeWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeSpellWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeSpellWeapon, _attackLargeSpellWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeSpellWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallSpellWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallSpellWeapon, _attackSmallSpellWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallSpellWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeBowWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeBowWeapon, _attackLargeBowWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeBowWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallBowWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallBowWeapon, _attackSmallBowWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallBowWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeThrownWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeThrownWeapon, _attackLargeThrownWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeThrownWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallThrownWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackSmallThrownWeapon, _attackSmallThrownWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleSmallThrownWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeFireArmWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.AttackLargeFireArmWeapon, _attackLargeFireArmWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeFireArmWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackNoWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackNoWeapon, _runAttackNoWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunNoWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallMeleeWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallMeleeWeapon, _runAttackSmallMeleeWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallMeleeWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeMeleeWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeMeleeWeapon, _runAttackLargeMeleeWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeMeleeWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeSpellWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeSpellWeapon, _runAttackLargeSpellWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeSpellWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallSpellWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallSpellWeapon, _runAttackSmallSpellWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallSpellWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeBowWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeBowWeapon, _runAttackLargeBowWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeBowWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallBowWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallBowWeapon, _runAttackSmallBowWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallBowWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeThrownWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeThrownWeapon, _runAttackLargeThrownWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeThrownWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallThrownWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackSmallThrownWeapon, _runAttackSmallThrownWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunSmallThrownWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeFireArmWeapon, new PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunAttackLargeFireArmWeapon, _runAttackLargeFireArmWeapon, _playerAnimationStates[Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeFireArmWeapon], this, playerAnimationReference, runIfAlreadyActive: true));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Spawning, new PlayerSpawningAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Spawning, _spawning, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Despawning, new PlayerDespawningAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Despawning, _despawning, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.DamageTaken, new PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.DamageTaken, _damageTaken, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dead, new PlayerDeadAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dead, _dead, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dying, new PlayerDyingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dying, _dying, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Succes, new PlayerSuccesAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Succes, _succes, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.ShowItem, new PlayerShowItemAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.ShowItem, _showItem, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dash, new PlayerDashingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Dash, _dash, null, this, playerAnimationReference));
		_playerAnimationStates.Add(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Revive, new PlayerReviveAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.Revive, _revive, null, this, playerAnimationReference));
	}

	internal void ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates playerAnimationStateEnum)
	{
		if (playerAnimationStateEnum != _currentAnimationState)
		{
			_ = _currentAnimationState;
			_animator.Play(_playerAnimationStates[playerAnimationStateEnum].AnimationName);
			_currentAnimationState = playerAnimationStateEnum;
			this.OnPlayerAnimationStarted?.Invoke(this, new PlayerAnimationStartedEventArgs(_currentAnimationState));
			_playerAnimationStates[playerAnimationStateEnum].OnStart(_player);
			StopAllCoroutines();
			StartCoroutine(TriggerFinishAfterDuration(_player, _currentAnimationState));
		}
	}

	private IEnumerator TriggerFinishAfterDuration(Player player, Enums.AnimatorStateMachineEnums.PlayerAnimationStates completedAnimation)
	{
		yield return new WaitForEndOfFrame();
		bool flag = false;
		if (_playerAnimationStates[_currentAnimationState].Loops)
		{
			flag = true;
		}
		if (!flag)
		{
			float seconds = _animator.GetCurrentAnimatorStateInfo(0).length / _animator.GetCurrentAnimatorStateInfo(0).speed;
			yield return new WaitForSeconds(seconds);
			yield return new WaitForEndOfFrame();
			_playerAnimationStates[_currentAnimationState].OnFinish(player);
			this.OnPlayerAnimationCompleted?.Invoke(this, new PlayerAnimationCompletedEventArgs(completedAnimation));
		}
		else
		{
			float seconds2 = _animator.GetCurrentAnimatorStateInfo(0).length / _animator.GetCurrentAnimatorStateInfo(0).speed;
			yield return new WaitForSeconds(seconds2);
			_playerAnimationStates[_currentAnimationState].OnLoopFinish(player);
		}
	}

	internal bool IsAnimationPlaying(Enums.AnimatorStateMachineEnums.PlayerAnimationStates playerAnimationStateEnum)
	{
		if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_playerAnimationStates[playerAnimationStateEnum].AnimationName))
		{
			return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime * _animator.GetCurrentAnimatorStateInfo(0).speed < 1f;
		}
		return false;
	}

	internal void SetUpdateMode(AnimatorUpdateMode unscaledTime)
	{
		_animator.updateMode = unscaledTime;
	}
}
