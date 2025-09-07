using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Player;

public class PlayerAnimationCompletedEventArgs : EventArgs
{
	public Enums.AnimatorStateMachineEnums.PlayerAnimationStates PlayerAnimationState { get; private set; }

	public PlayerAnimationCompletedEventArgs(Enums.AnimatorStateMachineEnums.PlayerAnimationStates playerAnimationState)
	{
		PlayerAnimationState = playerAnimationState;
	}
}
