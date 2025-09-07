using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Player;

public class PlayerAnimationStartedEventArgs : EventArgs
{
	public Enums.AnimatorStateMachineEnums.PlayerAnimationStates PlayerAnimationState { get; private set; }

	public PlayerAnimationStartedEventArgs(Enums.AnimatorStateMachineEnums.PlayerAnimationStates playerAnimationStatet)
	{
		PlayerAnimationState = playerAnimationStatet;
	}
}
