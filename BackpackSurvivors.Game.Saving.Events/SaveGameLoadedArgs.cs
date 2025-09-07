using System;

namespace BackpackSurvivors.Game.Saving.Events;

public class SaveGameLoadedArgs : EventArgs
{
	public SaveGame SaveGame { get; private set; }

	public SaveGameLoadedArgs(SaveGame progressionState)
	{
		SaveGame = progressionState;
	}
}
