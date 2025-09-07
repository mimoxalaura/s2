using System;

namespace BackpackSurvivors.System.Saving.SavefileEditorComponents.Events;

public class ActiveCharacterChangedEventArgs : EventArgs
{
	public int ActivatedCharacterId { get; private set; }

	public ActiveCharacterChangedEventArgs(int activatedCharacterId)
	{
		ActivatedCharacterId = activatedCharacterId;
	}
}
