using System;
using BackpackSurvivors.Game.Unlockables;

namespace BackpackSurvivors.UI.Unlockables;

public class UnlockItemSelectedEventArgs : EventArgs
{
	public UnlockableItem UnlockableItem { get; }

	public UnlockItemSelectedEventArgs(UnlockableItem unlockableItem)
	{
		UnlockableItem = unlockableItem;
	}
}
