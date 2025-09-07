using System;

namespace BackpackSurvivors.Game.Backpack.Events;

public class BackpackCellQuadrantHoveredEventArgs : EventArgs
{
	internal BackpackCellQuadrant BackpackCellQuadrant { get; private set; }

	public BackpackCellQuadrantHoveredEventArgs(BackpackCellQuadrant backpackCellQuadrant)
	{
		BackpackCellQuadrant = backpackCellQuadrant;
	}
}
