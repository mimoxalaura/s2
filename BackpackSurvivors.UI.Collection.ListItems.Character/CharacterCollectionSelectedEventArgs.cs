using System;
using BackpackSurvivors.ScriptableObjects.Classes;

namespace BackpackSurvivors.UI.Collection.ListItems.Character;

public class CharacterCollectionSelectedEventArgs : EventArgs
{
	public CharacterSO Character { get; set; }

	public CharacterCollectionSelectedEventArgs(CharacterSO character)
	{
		Character = character;
	}
}
