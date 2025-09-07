using System;
using BackpackSurvivors.ScriptableObjects.Adventures;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureSelectedEventArgs : EventArgs
{
	public AdventureSO SelectedAdventure { get; }

	public AdventureSelectedEventArgs(AdventureSO selectedAdventure)
	{
		SelectedAdventure = selectedAdventure;
	}
}
