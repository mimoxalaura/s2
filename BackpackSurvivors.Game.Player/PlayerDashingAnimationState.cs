using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerDashingAnimationState : PlayerAnimationState
{
	public PlayerDashingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference)
	{
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		Object.Instantiate(PlayerAnimationReference.DustPrefab, PlayerAnimationReference.DustPrefabSpawnPoint).transform.SetParent(null, worldPositionStays: true);
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
		else if (player.IsMoving)
		{
			_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.RunNoWeapon);
		}
		else
		{
			_playerAnimatorStateMachine.ChangeAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates.IdleNoWeapon);
		}
	}
}
