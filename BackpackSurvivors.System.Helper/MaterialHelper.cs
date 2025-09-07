using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Shared.Extensions;
using UnityEngine;

namespace BackpackSurvivors.System.Helper;

public class MaterialHelper
{
	public static Material GetVfxRarityMaterial(Enums.PlaceableRarity placeableRarity)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.RarityVFXMaterials.TryGet(placeableRarity, null);
	}

	public static Material GetWeaponElementTypeMaterial(Enums.DamageType elementType)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.ElementWeaponTypeMaterials.TryGet(elementType, null);
	}

	public static Material GetAttackElementTypeMaterial(Enums.DamageType elementType)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.ElementAttackTypeMaterials.TryGet(elementType, null);
	}

	public static Material GetShopOfferBorderRarityMaterial(Enums.PlaceableRarity placeableRarity)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.RarityBorderMaterials.TryGet(placeableRarity, null);
	}

	public static Material GetItemBorderRarityMaterial(Enums.PlaceableRarity placeableRarity)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.RarityItemBorderMaterials.TryGet(placeableRarity, null);
	}

	public static Material GetTooltipBorderRarityMaterial(Enums.PlaceableRarity placeableRarity)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.TooltipBorderMaterials.TryGet(placeableRarity, null);
	}

	public static Material GetInternalTooltipBorderRarityMaterial(Enums.PlaceableRarity placeableRarity)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.TooltipBorderMaterialsInternal.TryGet(placeableRarity, null);
	}
}
