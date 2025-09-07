using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.RuneEffects;

public class RuneSpecialEffect_GenerateSpecificItemToStorage : RuneSpecialEffect
{
	[SerializeField]
	private ItemSO _itemToGenerate;

	[SerializeField]
	private WeaponSO _weaponToGenerate;

	[SerializeField]
	private BagSO _bagToGenerate;

	[SerializeField]
	private Enums.PlaceableType _placeableItemTypeToAppear = Enums.PlaceableType.Item;

	public override bool Trigger()
	{
		base.Trigger();
		return _placeableItemTypeToAppear switch
		{
			Enums.PlaceableType.Weapon => SingletonController<BackpackController>.Instance.AddWeaponToStorage(_weaponToGenerate, showVfx: true), 
			Enums.PlaceableType.Bag => SingletonController<BackpackController>.Instance.AddBagToStorage(_bagToGenerate, showVfx: true), 
			Enums.PlaceableType.Item => SingletonController<BackpackController>.Instance.AddItemToStorage(_itemToGenerate, showVfx: true), 
			_ => false, 
		};
	}
}
