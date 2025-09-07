using System;

namespace BackpackSurvivors.Game.Relic;

public class RelicRewardPickedEventArgs : EventArgs
{
	public Relic Relic { get; }

	public RelicRewardPickedEventArgs(Relic relic)
	{
		Relic = relic;
	}
}
