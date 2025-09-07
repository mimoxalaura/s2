using System;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.ScriptableObjects.Debuffs;

namespace BackpackSurvivors.Game.Characters.Events;

public class DebuffAppliedEventArgs : EventArgs
{
	public DebuffSO Debuff { get; }

	public Character Character { get; }

	public DebuffAppliedEventArgs(DebuffSO debuff, Character character)
	{
		Debuff = debuff;
		Character = character;
	}
}
