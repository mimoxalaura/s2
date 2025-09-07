using System;

namespace BackpackSurvivors.UI.Difficulty;

public class DifficultyChangedEventArgs : EventArgs
{
	public int NewDifficulty { get; }

	public DifficultyChangedEventArgs(int newDifficulty)
	{
		NewDifficulty = newDifficulty;
	}
}
