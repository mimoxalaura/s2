using System;
using BackpackSurvivors.ScriptableObjects.Adventures;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureOptionSelectedeventArgs : EventArgs
{
	public AdventureSO SelectedAdventure { get; }

	public AdventureOptionSelectedeventArgs(AdventureSO selectedAdventure)
	{
		SelectedAdventure = selectedAdventure;
	}
}
