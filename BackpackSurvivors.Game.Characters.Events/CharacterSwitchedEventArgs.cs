using System;
using BackpackSurvivors.ScriptableObjects.Classes;

namespace BackpackSurvivors.Game.Characters.Events;

public class CharacterSwitchedEventArgs : EventArgs
{
	public CharacterSO NewCharacter { get; }

	public CharacterSwitchedEventArgs(CharacterSO newCharacter)
	{
		NewCharacter = newCharacter;
	}
}
