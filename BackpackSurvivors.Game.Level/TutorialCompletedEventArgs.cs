using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Level;

public class TutorialCompletedEventArgs : EventArgs
{
	public Enums.Tutorial Tutorial { get; }

	public TutorialCompletedEventArgs(Enums.Tutorial tutorial)
	{
		Tutorial = tutorial;
	}
}
