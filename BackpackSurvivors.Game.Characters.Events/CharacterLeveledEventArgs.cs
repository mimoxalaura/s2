using System;

namespace BackpackSurvivors.Game.Characters.Events;

public class CharacterLeveledEventArgs : EventArgs
{
	public int NewLevel { get; }

	public CharacterLeveledEventArgs(int newLevel)
	{
		NewLevel = newLevel;
	}
}
