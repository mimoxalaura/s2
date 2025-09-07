using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerDespawningAnimationState : PlayerAnimationState
{
	public PlayerDespawningAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference)
	{
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		PlayerAnimationReference.PlayerSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		PlayerAnimationReference.TopMask.GetComponent<SpriteRenderer>().enabled = false;
		PlayerAnimationReference.TopMask.GetComponent<SpriteMask>().enabled = false;
		PlayerAnimationReference.PlayerLargeMeleeWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeBowWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeSpellWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeThrownWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerLargeFireArmWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerSmallMeleeWeaponController.DisableSpriteRenderer();
		PlayerAnimationReference.PlayerSmallBowWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerSmallSpellWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerSmallThrownWeaponController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerShieldController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerHelmetController.gameObject.SetActive(value: false);
		PlayerAnimationReference.PlayerShadow.gameObject.SetActive(value: false);
		PlayerAnimationReference.PortalSpriteRenderer.material = PlayerAnimationReference.DespawningPortalMaterial;
	}

	public override void OnFinish(Player player)
	{
		base.OnFinish(player);
		PlayerAnimationReference.PlayerSpriteRenderer.enabled = false;
		PlayerAnimationReference.TopMask.GetComponent<SpriteRenderer>().enabled = true;
		PlayerAnimationReference.TopMask.GetComponent<SpriteMask>().enabled = true;
	}
}
