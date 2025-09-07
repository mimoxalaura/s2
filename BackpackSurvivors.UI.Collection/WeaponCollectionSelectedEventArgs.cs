using System;
using BackpackSurvivors.ScriptableObjects.Items;

namespace BackpackSurvivors.UI.Collection;

public class WeaponCollectionSelectedEventArgs : EventArgs
{
	public WeaponSO Weapon { get; set; }

	public CollectionWeaponUI CollectionWeaponUI { get; set; }

	public WeaponCollectionSelectedEventArgs(WeaponSO weapon, CollectionWeaponUI collectionWeaponUI)
	{
		Weapon = weapon;
		CollectionWeaponUI = collectionWeaponUI;
	}
}
