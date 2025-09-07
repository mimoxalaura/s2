using System;
using BackpackSurvivors.ScriptableObjects.Classes;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwappedZoneEnteredEventArgs : EventArgs
{
	public CharacterSO CharacterSO { get; }

	public CharacterSwappedZoneEnteredEventArgs(CharacterSO characterSO)
	{
		CharacterSO = characterSO;
	}
}
