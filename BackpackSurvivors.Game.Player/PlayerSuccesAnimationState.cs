using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerSuccesAnimationState : PlayerAnimationState
{
	public PlayerSuccesAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference)
	{
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		PlayerAnimationReference.LevelCompletedSuccesLight.SetActive(value: true);
		ShowBackdropAfterDelay(3f);
	}

	private IEnumerator ShowBackdropAfterDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		PlayerAnimationReference.SuccessBackdrop.SetActive(value: true);
	}
}
