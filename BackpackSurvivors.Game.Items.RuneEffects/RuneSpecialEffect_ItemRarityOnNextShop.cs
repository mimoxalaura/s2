using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.RuneEffects;

public class RuneSpecialEffect_ItemRarityOnNextShop : RuneSpecialEffect
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
			if (randomWeaponByRarity != null)
			{
				controllerByType.GuaranteeWeaponInShop(randomWeaponByRarity.Id);
			}
			return true;
		}
		case Enums.PlaceableType.Bag:
		{
			BagSO randomBagByRarity = controllerByType.ShopGenerator.GetRandomBagByRarity(_placeableRarityToAppear);
			if (randomBagByRarity != null)
			{
				controllerByType.GuaranteeBagInShop(randomBagByRarity.Id);
			}
			return true;
		}
		case Enums.PlaceableType.Item:
		{
			ItemSO randomItemByRarity = controllerByType.ShopGenerator.GetRandomItemByRarity(_placeableRarityToAppear);
			if (randomItemByRarity != null)
			{
				controllerByType.GuaranteeItemInShop(randomItemByRarity.Id);
			}
			return true;
		}
		default:
			return false;
		}
	}

	public override string GetDescription()
	{
		string aorAnByRarity = GetAorAnByRarity(_placeableRarityToAppear);
		return $"Guarantees {aorAnByRarity} <color={ColorHelper.GetColorHexcodeForRarity(_placeableRarityToAppear)}>{_placeableRarityToAppear}</color> {_placeableItemTypeToAppear} in the next shop";
	}

	private string GetAorAnByRarity(Enums.PlaceableRarity rarity)
	{
		switch (rarity)
		{
		case Enums.PlaceableRarity.Common:
		case Enums.PlaceableRarity.Rare:
		case Enums.PlaceableRarity.Legendary:
		case Enums.PlaceableRarity.Mythic:
			return "a";
		case Enums.PlaceableRarity.Uncommon:
		case Enums.PlaceableRarity.Epic:
		case Enums.PlaceableRarity.Unique:
			return "an";
		default:
			Debug.LogWarning(string.Format("PlaceableRarity {0} is not handled in {1}.{2}", rarity, "RuneSpecialEffect_ItemRarityOnNextShop", "GetAorAnByRarity"));
			return "a";
		}
	}
}
