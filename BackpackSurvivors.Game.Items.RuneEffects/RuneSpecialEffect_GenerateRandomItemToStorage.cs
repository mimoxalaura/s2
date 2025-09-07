using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.RuneEffects;

public class RuneSpecialEffect_GenerateRandomItemToStorage : RuneSpecialEffect
{
	[SerializeField]
	private Enums.PlaceableRarity _placeableRarityToAppear;

	[SerializeField]
	private Enums.PlaceableType _placeableItemTypeToAppear = Enums.PlaceableType.Item;

	public override bool Trigger()
	{
		base.Trigger();
		ShopController controllerByType = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		switch (_placeableItemTypeToAppear)
		{
		case Enums.PlaceableType.Weapon:
		{
			WeaponSO randomWeaponByRarity = controllerByType.ShopGenerator.GetRandomWeaponByRarity(_placeableRarityToAppear);
			return SingletonController<BackpackController>.Instance.AddWeaponToStorage(randomWeaponByRarity, showVfx: true);
		}
		case Enums.PlaceableType.Bag:
		{
			BagSO randomBagByRarity = controllerByType.ShopGenerator.GetRandomBagByRarity(_placeableRarityToAppear);
			return SingletonController<BackpackController>.Instance.AddBagToStorage(randomBagByRarity, showVfx: true);
		}
		case Enums.PlaceableType.Item:
		{
			ItemSO randomItemByRarity = controllerByType.ShopGenerator.GetRandomItemByRarity(_placeableRarityToAppear);
			return SingletonController<BackpackController>.Instance.AddItemToStorage(randomItemByRarity, showVfx: true);
		}
		default:
			return false;
		}
	}

	public override string GetDescription()
	{
		return $"You get a random <color={ColorHelper.GetColorHexcodeForRarity(_placeableRarityToAppear)}>{_placeableRarityToAppear}</color> {_placeableItemTypeToAppear} when entering the next shop";
	}
}
