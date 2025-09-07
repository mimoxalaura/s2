using System;

namespace BackpackSurvivors.Game.Backpack.Events;

public class DraggableHoveredEventArgs : EventArgs
{
	public bool IsHovered { get; private set; }

	public DraggableHoveredEventArgs(bool isHovered)
	{
		IsHovered = isHovered;
	}
}
