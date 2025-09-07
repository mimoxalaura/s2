using System;

namespace BackpackSurvivors.Game.Shop;

public class ShopClosedEventArgs : EventArgs
{
	public bool ContinueToAdventure { get; }

	public ShopClosedEventArgs(bool continueToAdventure)
	{
		ContinueToAdventure = continueToAdventure;
	}
}
