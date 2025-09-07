using System;
using BackpackSurvivors.ScriptableObjects.Adventures;

namespace BackpackSurvivors.Game.Adventure;

public class AdventureStartedEventArgs : EventArgs
{
	public AdventureSO Adventure;

	public AdventureStartedEventArgs(AdventureSO adventure)
	{
		Adventure = adventure;
	}
}
