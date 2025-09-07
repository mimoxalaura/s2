using System;

namespace BackpackSurvivors.Game.Buffs.Base;

internal class BuffAddedEventArgs : EventArgs
{
	public BuffHandler BuffHandler { get; private set; }

	public BuffAddedEventArgs(BuffHandler buffHandler)
	{
		BuffHandler = buffHandler;
	}
}
