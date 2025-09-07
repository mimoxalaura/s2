using System;

namespace BackpackSurvivors.Game.Level;

internal class InfinityDifficultyIncreasedHandlerEventArgs : EventArgs
{
	internal float CurrentDifficulty { get; private set; }

	public InfinityDifficultyIncreasedHandlerEventArgs(float currentDifficulty)
	{
		CurrentDifficulty = currentDifficulty;
	}
}
