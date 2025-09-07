using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerRunningAnimationState : PlayerAnimationState
{
	public PlayerRunningAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference)
		: base(currentAnimationStateEnum, animationClip, animationNameAfterCompletion, playerAnimatorStateMachine, playerAnimationReference)
	{
	}

	public override void OnStart(Player player)
	{
		base.OnStart(player);
		_ = AnimationStateEnum;
		_ = 410;
	}

	public override void OnLoopFinish(Player player)
	{
		base.OnLoopFinish(player);
		_ = AnimationStateEnum;
		_ = 410;
	}
}
