using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerSpawningAnimationState : PlayerAnimationState
{
	public PlayerSpawningAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference)
	{
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		PlayerAnimationReference.PlayerSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		PlayerAnimationReference.TopMask.GetComponent<SpriteRenderer>().enabled = false;
		PlayerAnimationReference.TopMask.GetComponent<SpriteMask>().enabled = false;
		PlayerAnimationReference.PlayerHelmetController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerShieldController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerShadow.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerSmallMeleeWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerSmallSpellWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerSmallBowWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerSmallThrownWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeMeleeWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeSpellWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeBowWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeThrownWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeFireArmWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PortalSpriteRenderer.material = PlayerAnimationReference.SpawningPortalMaterial;
	}

	public override void OnFinish(Player player)
	{
		base.OnFinish(player);
		if (player.PlayerVisualController.HasWeapon)
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
		PlayerAnimationReference.PlayerSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		PlayerAnimationReference.TopMask.GetComponent<SpriteRenderer>().enabled = true;
		PlayerAnimationReference.TopMask.GetComponent<SpriteMask>().enabled = true;
		PlayerAnimationReference.PlayerHelmetController.gameObject.SetActive(value: true);
		PlayerAnimationReference.PlayerShadow.gameObject.SetActive(value: true);
		if (player.PlayerVisualController.HasShield && (!player.PlayerVisualController.HasWeapon || player.PlayerVisualController.WeaponAnimationSize != Enums.WeaponAnimationSize.Large || player.PlayerVisualController.WeaponAnimationType != Enums.WeaponAnimationType.Bow))
		{
			PlayerAnimationReference.PlayerShieldController.gameObject.SetActive(value: true);
		}
	}
}
