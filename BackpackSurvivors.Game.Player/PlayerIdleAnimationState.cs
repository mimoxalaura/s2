using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerIdleAnimationState : PlayerAnimationState
{
	public PlayerIdleAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference)
	{
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		SetShieldActive(player);
		SetHelmetActive(player);
		SetWeaponActive(player);
	}

	private void SetWeaponActive(Player player)
	{
		PlayerAnimationReference.PlayerSmallMeleeWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerSmallSpellWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerSmallBowWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerSmallThrownWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerLargeMeleeWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerLargeSpellWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerLargeBowWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerLargeThrownWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerLargeFireArmWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerShieldController.gameObject.SetActive(value: false);
		if (player.PlayerVisualController.HasWeapon)
		{
			switch (player.PlayerVisualController.WeaponAnimationSize)
			{
			case Enums.WeaponAnimationSize.Small:
				switch (player.PlayerVisualController.WeaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					PlayerAnimationReference.PlayerSmallMeleeWeaponController.EnableSpriteRenderer();
					break;
				case Enums.WeaponAnimationType.Spell:
					PlayerAnimationReference.PlayerSmallSpellWeaponController.EnableSpriteRenderer();
					break;
				case Enums.WeaponAnimationType.Bow:
					PlayerAnimationReference.PlayerSmallBowWeaponController.EnableSpriteRenderer();
					break;
				case Enums.WeaponAnimationType.Thrown:
					PlayerAnimationReference.PlayerSmallThrownWeaponController.EnableSpriteRenderer();
					break;
				}
				break;
			case Enums.WeaponAnimationSize.Large:
				switch (player.PlayerVisualController.WeaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					PlayerAnimationReference.PlayerLargeMeleeWeaponController.EnableSpriteRenderer();
					break;
				case Enums.WeaponAnimationType.Spell:
					PlayerAnimationReference.PlayerLargeSpellWeaponController.EnableSpriteRenderer();
					break;
				case Enums.WeaponAnimationType.Bow:
					PlayerAnimationReference.PlayerLargeBowWeaponController.EnableSpriteRenderer();
					break;
				case Enums.WeaponAnimationType.Thrown:
					PlayerAnimationReference.PlayerLargeThrownWeaponController.EnableSpriteRenderer();
					break;
				case Enums.WeaponAnimationType.Firearm:
					PlayerAnimationReference.PlayerLargeFireArmWeaponController.EnableSpriteRenderer();
					break;
				}
				break;
			}
		}
		if (player.PlayerVisualController.WeaponAnimationType != Enums.WeaponAnimationType.Bow)
		{
			PlayerAnimationReference.PlayerShieldController.gameObject.SetActive(value: true);
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

	public override void OnFinish(Player player)
	{
		base.OnFinish(player);
		if (player.PlayerVisualController.HasWeapon && player.PlayerVisualController.WeaponAnimationType == Enums.WeaponAnimationType.Thrown)
		{
			PlayerAnimationReference.PlayerLargeThrownWeaponController.gameObject.SetActive(value: true);
		}
	}
}
