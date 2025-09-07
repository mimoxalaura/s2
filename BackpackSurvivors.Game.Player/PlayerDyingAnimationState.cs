using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerDyingAnimationState : PlayerAnimationState
{
	public PlayerDyingAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference)
	{
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		PlayerAnimationReference.DeathWeaponInGroundMask.SetActive(value: true);
		if (player.PlayerVisualController.HasWeapon)
		{
			switch (player.PlayerVisualController.WeaponAnimationSize)
			{
			case Enums.WeaponAnimationSize.Small:
			{
				Enums.WeaponAnimationType weaponAnimationType = player.PlayerVisualController.WeaponAnimationType;
				if ((uint)weaponAnimationType <= 3u)
				{
					PlayerAnimationReference.DeathFlag.SetActive(value: true);
				}
				break;
			}
			case Enums.WeaponAnimationSize.Large:
				switch (player.PlayerVisualController.WeaponAnimationType)
				{
				case Enums.WeaponAnimationType.Melee:
					PlayerAnimationReference.PlayerLargeMeleeWeaponController.SetMask(SpriteMaskInteraction.VisibleOutsideMask);
					break;
				case Enums.WeaponAnimationType.Spell:
					PlayerAnimationReference.PlayerLargeSpellWeaponController.SetMask(SpriteMaskInteraction.VisibleOutsideMask);
					break;
				case Enums.WeaponAnimationType.Bow:
					PlayerAnimationReference.PlayerLargeBowWeaponController.SetMask(SpriteMaskInteraction.VisibleOutsideMask);
					break;
				case Enums.WeaponAnimationType.Thrown:
					PlayerAnimationReference.PlayerLargeThrownWeaponController.SetMask(SpriteMaskInteraction.VisibleOutsideMask);
					break;
				}
				break;
			}
		}
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
		PlayerAnimationReference.PlayerSpriteRenderer.color = Color.white;
		foreach (Transform item in PlayerAnimationReference.VisualEffectsContainer)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in PlayerAnimationReference.BuffVisualEffectsContainer)
		{
			Object.Destroy(item2.gameObject);
		}
	}
}
