using System;

namespace BackpackSurvivors.Game.Backpack.Events;

public class DraggableSizeCalculatedEventArgs : EventArgs
{
	public int Width { get; private set; }

	public int Height { get; private set; }

	public DraggableSizeCalculatedEventArgs(int width, int height)
	{
		Width = width;
		Height = height;
	}
}
