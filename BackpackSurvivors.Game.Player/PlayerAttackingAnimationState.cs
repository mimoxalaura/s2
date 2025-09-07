using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerAttackingAnimationState : PlayerAnimationState
{
	public PlayerAttackingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference, bool runIfAlreadyActive)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference, runIfAlreadyActive)
	{
	}

	private void SetWeaponActive(Player player, bool active)
	{
		if (!player.PlayerVisualController.HasWeapon)
		{
			return;
		}
		if (player.PlayerVisualController.WeaponAnimationSize == Enums.WeaponAnimationSize.Large)
		{
			switch (player.PlayerVisualController.WeaponAnimationType)
			{
			case Enums.WeaponAnimationType.Melee:
				PlayerAnimationReference.PlayerLargeMeleeWeaponController.AttackWithIngameWeapon();
				break;
			case Enums.WeaponAnimationType.Spell:
				PlayerAnimationReference.PlayerLargeSpellWeaponController.AttackWithIngameWeapon();
				break;
			case Enums.WeaponAnimationType.Bow:
				PlayerAnimationReference.PlayerLargeBowWeaponController.AttackWithIngameWeapon();
				break;
			case Enums.WeaponAnimationType.Thrown:
				PlayerAnimationReference.PlayerLargeThrownWeaponController.gameObject.SetActive(active);
				PlayerAnimationReference.PlayerLargeThrownWeaponController.AttackWithIngameWeapon();
				break;
			case Enums.WeaponAnimationType.Firearm:
				PlayerAnimationReference.PlayerLargeFireArmWeaponController.AttackWithIngameWeapon();
				break;
			}
		}
		else
		{
			switch (player.PlayerVisualController.WeaponAnimationType)
			{
			case Enums.WeaponAnimationType.Melee:
				PlayerAnimationReference.PlayerSmallMeleeWeaponController.AttackWithIngameWeapon();
				break;
			case Enums.WeaponAnimationType.Spell:
				PlayerAnimationReference.PlayerSmallSpellWeaponController.AttackWithIngameWeapon();
				break;
			case Enums.WeaponAnimationType.Bow:
				PlayerAnimationReference.PlayerSmallBowWeaponController.AttackWithIngameWeapon();
				break;
			case Enums.WeaponAnimationType.Thrown:
				PlayerAnimationReference.PlayerSmallThrownWeaponController.gameObject.SetActive(active);
				PlayerAnimationReference.PlayerSmallThrownWeaponController.AttackWithIngameWeapon();
				break;
			}
		}
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		PlayerAnimationReference.PlayerSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		SetShieldActive(player);
		SetWeaponActive(player, active: false);
		SetHelmetActive(player);
	}

	public override void OnFinish(Player player)
	{
		base.OnFinish(player);
		SetWeaponActive(player, active: true);
		if (player.IsMoving)
		{
			if (player.PlayerVisualController.HasWeapon)
			{
				if (player.PlayerVisualController.WeaponAnimationSize == Enums.WeaponAnimationSize.Large)
				{
					switch (player.PlayerVisualController.WeaponAnimationType)
					{
					case Enums.WeaponAnimationType.Melee:
						_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunLargeMeleeWeapon);
						PlayerAnimationReference.LargeMeleeWeaponMask.SetActive(value: false);
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
					switch (player.PlayerVisualController.WeaponAnimationType)
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
		}
		else if (player.PlayerVisualController.HasWeapon)
		{
			if (player.PlayerVisualController.WeaponAnimationSize == Enums.WeaponAnimationSize.Large)
			{
				switch (player.PlayerVisualController.WeaponAnimationType)
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
				switch (player.PlayerVisualController.WeaponAnimationType)
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
	}

	private void SetShieldActive(Player player)
	{
		if (player.PlayerVisualController.HasShield)
		{
			if (player.PlayerVisualController.WeaponAnimationType == Enums.WeaponAnimationType.Bow)
			{
				PlayerAnimationReference.PlayerShieldController.gameObject.SetActive(value: false);
			}
			else
			{
				PlayerAnimationReference.PlayerShieldController.gameObject.SetActive(value: true);
			}
		}
	}

	private void SetHelmetActive(Player player)
	{
		PlayerAnimationReference.PlayerHelmetController.gameObject.SetActive(player.PlayerVisualController.HasHelmet);
		PlayerAnimationReference.HelmetMask.SetActive(player.PlayerVisualController.HasHelmet);
	}
}
