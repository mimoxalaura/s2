using System;
using BackpackSurvivors.ScriptableObjects.Buffs;

namespace BackpackSurvivors.Game.Characters.Events;

public class BuffAppliedEventArgs : EventArgs
{
	public BuffSO Buff { get; }

	public BuffAppliedEventArgs(BuffSO buff)
	{
		Buff = buff;
	}
}
