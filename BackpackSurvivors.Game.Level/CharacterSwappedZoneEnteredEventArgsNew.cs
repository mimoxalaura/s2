using System;
using BackpackSurvivors.ScriptableObjects.Classes;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwappedZoneEnteredEventArgsNew : EventArgs
{
	public CharacterSO CharacterSO { get; }

	public CharacterSwappedZoneEnteredEventArgsNew(CharacterSO characterSO)
	{
		CharacterSO = characterSO;
	}
}
