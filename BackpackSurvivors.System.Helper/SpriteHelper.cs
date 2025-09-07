using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Shared.Extensions;
using UnityEngine;

namespace BackpackSurvivors.System.Helper;

public class SpriteHelper
{
	public static Sprite GetSkullSprite()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.SkullSprite;
	}

	public static Sprite GetMergeIcon()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.MergeSprite;
	}

	public static Sprite GetCurrencySprite(Enums.CurrencyType currencyType)
	{
		return currencyType switch
		{
			Enums.CurrencyType.Coins => SingletonController<GameDatabase>.Instance.GameDatabaseSO.CoinValue1, 
			Enums.CurrencyType.TitanSouls => SingletonController<GameDatabase>.Instance.GameDatabaseSO.TitanicSouls, 
			_ => null, 
		};
	}

	public static Sprite GetMenuIconSprite(Enums.GameMenuButtonType gameMenuButtonType)
	{
		return gameMenuButtonType switch
		{
			Enums.GameMenuButtonType.Resume => SingletonController<GameDatabase>.Instance.GameDatabaseSO.Resume, 
			Enums.GameMenuButtonType.Character => SingletonController<GameDatabase>.Instance.GameDatabaseSO.Character, 
			Enums.GameMenuButtonType.Settings => SingletonController<GameDatabase>.Instance.GameDatabaseSO.Settings, 
			Enums.GameMenuButtonType.Collection => SingletonController<GameDatabase>.Instance.GameDatabaseSO.Collection, 
			Enums.GameMenuButtonType.BackToTown => SingletonController<GameDatabase>.Instance.GameDatabaseSO.BackToTown, 
			Enums.GameMenuButtonType.ExitGame => SingletonController<GameDatabase>.Instance.GameDatabaseSO.ExitGame, 
			Enums.GameMenuButtonType.QuitGame => SingletonController<GameDatabase>.Instance.GameDatabaseSO.QuitGame, 
			Enums.GameMenuButtonType.Feedback => SingletonController<GameDatabase>.Instance.GameDatabaseSO.Feedback, 
			_ => null, 
		};
	}

	public static Sprite GetRarityTypeSprite(Enums.PlaceableRarity itemQuality)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.ItemRarity.TryGet(itemQuality, null);
	}

	public static Sprite GetItemStatTypeSprite(Enums.ItemStatType itemStatType)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.ItemStatTypeIcons.TryGet(itemStatType, null);
	}

	public static Sprite GetRarityTypeBackgroundSprite(Enums.PlaceableRarity itemQuality)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.ItemRarityBackground.TryGet(itemQuality, null);
	}

	internal static Sprite GetDamageTypeIconSprite(Enums.DamageType damageType)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.DamageTypes.TryGet(damageType, null);
	}

	internal static Sprite GetWeaponTypeIconSprite(Enums.WeaponType weaponType)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.WeaponTypes.TryGet(weaponType, null);
	}

	internal static Sprite GetPlaceableWeaponSubtypeIconSprite(Enums.PlaceableWeaponSubtype placeableWeaponSubtype)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.PlaceableWeaponSubtypes.TryGet(placeableWeaponSubtype, null);
	}

	internal static Sprite GetCoinSprite(int coinValue)
	{
		if (coinValue > 1000)
		{
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.CoinValue1000;
		}
		if (coinValue > 500)
		{
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.CoinValue500;
		}
		if (coinValue > 100)
		{
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.CoinValue100;
		}
		if (coinValue > 25)
		{
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.CoinValue25;
		}
		if (coinValue > 10)
		{
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.CoinValue10;
		}
		if (coinValue > 1)
		{
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.CoinValue1;
		}
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.CoinValueDefault;
	}

	internal static Sprite GetDebuffIcon(Enums.Debuff.DebuffType debuffType)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.DebuffIcons.TryGet(debuffType, null);
	}

	internal static Sprite GetSoftPopupTypeSprite(Enums.SoftPopupType softPopupType)
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.SoftPopupIcons.ContainsKey(softPopupType))
		{
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.SoftPopupIcons[softPopupType];
		}
		return null;
	}
}
