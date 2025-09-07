using System;

namespace BackpackSurvivors.Game.MainMenu;

public class SaveSlotUIItemSelectedEventArgs : EventArgs
{
	public Guid Key { get; }

	public SaveSlotUIItemSelectedEventArgs(Guid key)
	{
		Key = key;
	}
}
