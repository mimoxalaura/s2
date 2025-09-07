using System;
using BackpackSurvivors.ScriptableObjects.Adventures;

namespace BackpackSurvivors.Game.Adventure;

public class AdventureCompletedEventArgs : EventArgs
{
	public AdventureSO Adventure;

	public bool Success;

	public AdventureCompletedEventArgs(AdventureSO adventure, bool success)
	{
		Adventure = adventure;
		Success = success;
	}
}
