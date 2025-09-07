using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class PlayerAnimationState
{
	private AnimationClip _animationClip;

	internal PlayerAnimationState AnimationNameAfterCompletion;

	protected readonly PlayerAnimatorStateMachine _playerAnimatorStateMachine;

	internal readonly PlayerAnimationReference PlayerAnimationReference;

	internal readonly bool RunIfAlreadyActive;

	internal readonly Enums.AnimatorStateMachineEnums.PlayerAnimationStates AnimationStateEnum;

	internal bool Loops => _animationClip.isLooping;

	internal string AnimationName => _animationClip?.name;

	public PlayerAnimationState(Enums.AnimatorStateMachineEnums.PlayerAnimationStates currentAnimationStateEnum, AnimationClip animationClip, PlayerAnimationState animationNameAfterCompletion, PlayerAnimatorStateMachine playerAnimatorStateMachine, PlayerAnimationReference playerAnimationReference, bool runIfAlreadyActive = false)
	{
		AnimationStateEnum = currentAnimationStateEnum;
		_animationClip = animationClip;
		AnimationNameAfterCompletion = animationNameAfterCompletion;
		_playerAnimatorStateMachine = playerAnimatorStateMachine;
		PlayerAnimationReference = playerAnimationReference;
		RunIfAlreadyActive = runIfAlreadyActive;
	}

	public virtual void OnStart(Player player)
	{
	}

	public virtual void OnFinish(Player player)
	{
	}

	public virtual void OnLoopFinish(Player player)
	{
	}
}
