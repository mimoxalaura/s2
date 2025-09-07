using System;

namespace BackpackSurvivors.Game.Level.Events;

internal class TimeRemainingEventArgs : EventArgs
{
	internal int TimeRemaining { get; private set; }

	internal int TotalLevelDuration { get; private set; }

	public TimeRemainingEventArgs(int timeRemaining, int totalLevelDuration)
	{
		TimeRemaining = timeRemaining;
		TotalLevelDuration = totalLevelDuration;
	}
}
