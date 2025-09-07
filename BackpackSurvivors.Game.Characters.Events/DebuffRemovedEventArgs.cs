using System;
using BackpackSurvivors.ScriptableObjects.Debuffs;

namespace BackpackSurvivors.Game.Characters.Events;

public class DebuffRemovedEventArgs : EventArgs
{
	public DebuffSO Debuff { get; }

	public DebuffRemovedEventArgs(DebuffSO debuff)
	{
		Debuff = debuff;
	}
}
