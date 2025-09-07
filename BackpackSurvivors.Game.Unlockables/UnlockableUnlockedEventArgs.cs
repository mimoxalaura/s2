using System;

namespace BackpackSurvivors.Game.Unlockables;

public class UnlockableUnlockedEventArgs : EventArgs
{
	public UnlockableItem UnlockedItem { get; }

	public bool FromLoad { get; }

	public bool FromUI { get; }

	public UnlockableUnlockedEventArgs(UnlockableItem unlockedItem, bool fromLoad, bool fromUI)
	{
		UnlockedItem = unlockedItem;
		FromLoad = fromLoad;
		FromUI = fromUI;
	}
}
