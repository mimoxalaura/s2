using System;

namespace BackpackSurvivors.Game.Relic;

public class RelicAddedEventArgs : EventArgs
{
	public Relic Relic { get; }

	public bool ByUser { get; }

	public RelicAddedEventArgs(Relic relic, bool byUser)
	{
		Relic = relic;
		ByUser = byUser;
	}
}
