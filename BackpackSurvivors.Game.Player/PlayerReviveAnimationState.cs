using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerReviveAnimationState : PlayerAnimationState
{
	public PlayerReviveAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference)
	{
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		PlayerAnimationReference.DeathFlag.SetActive(value: false);
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
					PlayerAnimationReference.PlayerLargeMeleeWeaponController.SetMask(SpriteMaskInteraction.None);
					break;
				case Enums.WeaponAnimationType.Spell:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeSpellWeapon);
					PlayerAnimationReference.PlayerLargeSpellWeaponController.SetMask(SpriteMaskInteraction.None);
					break;
				case Enums.WeaponAnimationType.Bow:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeBowWeapon);
					PlayerAnimationReference.PlayerLargeBowWeaponController.SetMask(SpriteMaskInteraction.None);
					break;
				case Enums.WeaponAnimationType.Thrown:
					_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleLargeThrownWeapon);
					PlayerAnimationReference.PlayerLargeThrownWeaponController.SetMask(SpriteMaskInteraction.None);
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
}
