using System;

namespace BackpackSurvivors.Game.MainMenu;

public class SaveSlotUIItemHoveredEventArgs : EventArgs
{
	public Guid Key { get; }

	public SaveSlotUIItemHoveredEventArgs(Guid key)
	{
		Key = key;
	}
}
