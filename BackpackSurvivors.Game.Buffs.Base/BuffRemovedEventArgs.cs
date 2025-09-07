using System;

namespace BackpackSurvivors.Game.Buffs.Base;

internal class BuffRemovedEventArgs : EventArgs
{
	public BuffHandler BuffHandler { get; private set; }

	public BuffRemovedEventArgs(BuffHandler buffHandler)
	{
		BuffHandler = buffHandler;
	}
}
