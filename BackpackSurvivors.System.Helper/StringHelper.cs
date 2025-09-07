using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using UnityEngine;

namespace BackpackSurvivors.System.Helper;

public class StringHelper
{
	public static CultureInfo CultureInfo = new CultureInfo("en-us");

	public static string TooltipPointPositive => "<sprite name=\"TooltipPointPositive\"> ";

	public static string TooltipPointNegative => "<sprite name=\"TooltipPointNegative\"> ";

	public static string TooltipPoint => "<sprite name=\"TooltipPointNormal\"> ";

	public static string PositiveStarSprite => "<sprite name=\"PositiveStar\">";

	public static string NegativeStarSprite => "<sprite name=\"NegativeStar\">";

	public static string CoinSprite => "<sprite name=\"Coin\">";

	public static string SpecialItemActivationSprite => "<sprite name=\"SpecialItemActivation\">";

	public static string ItemSprite => "<sprite name=\"Item\">";

	public static string WeaponSprite => "<sprite name=\"Weapon\">";

	public static string BagSprite => "<sprite name=\"Bag\">";

	public static string YesSprite => "<sprite name=\"spritesheet_YES\">";

	public static string NoSprite => "<sprite name=\"spritesheet_NO\">";

	public static string GetValueAsPercentageString(float value, bool isModifier)
	{
		string text = value.ToString("P0", CultureInfo);
		text = text.Replace(" ", "");
		return GetModifierPrefix(value, isModifier) + text;
	}

	public static string GetValueAsIntegerString(float value, bool isModifier)
	{
		int num = Mathf.FloorToInt(value);
		string modifierPrefix = GetModifierPrefix(num, isModifier);
		return $"{modifierPrefix}{num}";
	}

	private static string GetFormattedString(float value, bool isModifier, string format)
	{
		string modifierPrefix = GetModifierPrefix(value, isModifier);
		string text = value.ToString(format, CultureInfo);
		return modifierPrefix + text;
	}

	internal static string GetConditionsString(Condition[] weaponFilters, bool isStarCondition)
	{
		string text = string.Empty;
		foreach (string item in weaponFilters.Select((Condition x) => x.GetGeneratedDescription(isStarCondition)))
		{
			text += item;
		}
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		return text ?? "";
	}

	public static string GetCleanValue(float value, Enums.DamageType damageType, bool isModifier = false)
	{
		string empty = string.Empty;
		switch (damageType)
		{
		case Enums.DamageType.None:
		case Enums.DamageType.PhysicalDONOTUSE:
		case Enums.DamageType.Fire:
		case Enums.DamageType.Cold:
		case Enums.DamageType.Lightning:
		case Enums.DamageType.Void:
		case Enums.DamageType.Poison:
		case Enums.DamageType.Energy:
		case Enums.DamageType.Holy:
		case Enums.DamageType.Blunt:
		case Enums.DamageType.Slashing:
		case Enums.DamageType.Piercing:
			return GetValueAsPercentageString(value, isModifier);
		default:
			return GetValueAsPercentageString(value, isModifier);
		}
	}

	public static string GetCleanValue(float value, Enums.WeaponStatType statType, bool isModifier = false)
	{
		string empty = string.Empty;
		switch (statType)
		{
		case Enums.WeaponStatType.CritChancePercentage:
		case Enums.WeaponStatType.CritMultiplier:
		case Enums.WeaponStatType.DamagePercentage:
		case Enums.WeaponStatType.CooldownTime:
		case Enums.WeaponStatType.WeaponRange:
		case Enums.WeaponStatType.ExplosionSizePercentage:
		case Enums.WeaponStatType.ProjectileSizePercentage:
		case Enums.WeaponStatType.CooldownReductionPercentage:
			return GetValueAsPercentageString(value, isModifier);
		case Enums.WeaponStatType.ProjectileSpeed:
			return GetFormattedString(value, isModifier, "0.#");
		case Enums.WeaponStatType.FlatDamage:
			return GetValueAsIntegerString(value, isModifier);
		case Enums.WeaponStatType.Penetrating:
			return (value > 100f) ? "Infinite" : GetValueAsIntegerString(value, isModifier);
		case Enums.WeaponStatType.LifeDrainPercentage:
			return (value < 0f) ? "0" : GetValueAsPercentageString(value, isModifier);
		case Enums.WeaponStatType.ProjectileCount:
			value = Mathf.Max(1f, value);
			return GetValueAsIntegerString(value, isModifier);
		case Enums.WeaponStatType.StunChancePercentage:
			value = Mathf.Max(1f, value);
			return GetValueAsPercentageString(value, isModifier);
		case Enums.WeaponStatType.ProjectileDuration:
			return GetFormattedString(value, isModifier, "F1");
		default:
			Debug.LogWarning($"{statType} has no cleaned value!");
			return string.Empty;
		}
	}

	private static string GetModifierPrefix(float value, bool isModifier)
	{
		if (!isModifier)
		{
			return string.Empty;
		}
		if (value == 0f)
		{
			return string.Empty;
		}
		if (!(value > 0f))
		{
			return "";
		}
		return "+";
	}

	public static string GetCleanValue(float value, Enums.ItemStatType statType, bool isModifier = false)
	{
		string empty = string.Empty;
		switch (statType)
		{
		case Enums.ItemStatType.CritChancePercentage:
		case Enums.ItemStatType.CritMultiplier:
		case Enums.ItemStatType.DamagePercentage:
		case Enums.ItemStatType.SpeedPercentage:
		case Enums.ItemStatType.LuckPercentage:
		case Enums.ItemStatType.CooldownTime:
		case Enums.ItemStatType.DamageReductionPercentageDONOTUSE:
		case Enums.ItemStatType.PickupRadiusPercentage:
		case Enums.ItemStatType.WeaponRange:
		case Enums.ItemStatType.ExplosionSizePercentage:
		case Enums.ItemStatType.LifeDrainPercentage:
		case Enums.ItemStatType.ProjectileSpeed:
		case Enums.ItemStatType.ProjectileSizePercentage:
		case Enums.ItemStatType.StunChancePercentage:
		case Enums.ItemStatType.ExtraCoinChancePercentage:
		case Enums.ItemStatType.CooldownReductionPercentage:
		case Enums.ItemStatType.DodgePercentage:
		case Enums.ItemStatType.ProjectileDuration:
		case Enums.ItemStatType.DamageAgainstNormalEnemies:
		case Enums.ItemStatType.DamageAgainstEliteAndBossEnemies:
		case Enums.ItemStatType.ExperienceGainedPercentage:
			return GetValueAsPercentageString(value, isModifier);
		case Enums.ItemStatType.HealthRegeneration:
			if (isModifier)
			{
				return GetValueAsIntegerString(value, isModifier);
			}
			return (value < 0f) ? "0" : GetValueAsIntegerString(value, isModifier);
		case Enums.ItemStatType.Armor:
		case Enums.ItemStatType.Spiked:
		case Enums.ItemStatType.FlatDamage:
		case Enums.ItemStatType.EnemyCount:
		case Enums.ItemStatType.ExtraDash:
		case Enums.ItemStatType.MaximumCompanionCount:
			return GetValueAsIntegerString(value, isModifier);
		case Enums.ItemStatType.Penetrating:
			return (value > 100f) ? "Infinite" : GetValueAsIntegerString(value, isModifier);
		case Enums.ItemStatType.Health:
			if (isModifier)
			{
				return GetValueAsIntegerString(value, isModifier);
			}
			return (value <= 0f) ? "1" : GetValueAsIntegerString(value, isModifier);
		case Enums.ItemStatType.ProjectileCount:
			return (value > 0f) ? GetValueAsIntegerString(value, isModifier) : (isModifier ? GetValueAsIntegerString(value, isModifier) : "1");
		case Enums.ItemStatType.WeaponCapacity:
			return (value > 100f) ? "Maximum" : GetValueAsIntegerString(value, isModifier);
		default:
			Debug.LogWarning($"{statType} has no cleaned value!");
			return string.Empty;
		}
	}

	public static string GetCleanString(Enums.PlaceableRarity itemQuality)
	{
		return itemQuality switch
		{
			Enums.PlaceableRarity.Common => "Common", 
			Enums.PlaceableRarity.Uncommon => "Uncommon", 
			Enums.PlaceableRarity.Rare => "Rare", 
			Enums.PlaceableRarity.Epic => "Epic", 
			Enums.PlaceableRarity.Legendary => "Legendary", 
			Enums.PlaceableRarity.Mythic => "Mythic", 
			Enums.PlaceableRarity.Unique => "Unique", 
			_ => itemQuality.ToString(), 
		};
	}

	public static string GetCleanString(Enums.DashVisual dashVisual)
	{
		return dashVisual switch
		{
			Enums.DashVisual.InUI => "UI", 
			Enums.DashVisual.UnderPlayer => "Player", 
			Enums.DashVisual.Both => "Both", 
			Enums.DashVisual.None => "None", 
			_ => dashVisual.ToString(), 
		};
	}

	public static string GetCleanString(Enums.Targeting targeting)
	{
		return targeting switch
		{
			Enums.Targeting.Automatic => "Automatic", 
			Enums.Targeting.Manual => "Manual", 
			Enums.Targeting.AutomaticButManualOnHotkey => "Manual on [SHIFT/LT]", 
			_ => targeting.ToString(), 
		};
	}

	public static string GetCleanString(Enums.DamageType damageType)
	{
		return damageType.ToString();
	}

	public static string GetCleanString(Enums.PlaceableItemSubtype itemSubtype, bool getPlural)
	{
		switch (itemSubtype)
		{
		case Enums.PlaceableItemSubtype.Trinket:
			if (!getPlural)
			{
				return "Trinket";
			}
			return "Trinkets";
		case Enums.PlaceableItemSubtype.Boots:
			if (!getPlural)
			{
				return "Boots";
			}
			return "Boots";
		case Enums.PlaceableItemSubtype.Ring:
			if (!getPlural)
			{
				return "Ring";
			}
			return "Rings";
		case Enums.PlaceableItemSubtype.Headwear:
			if (!getPlural)
			{
				return "Headwear";
			}
			return "Headwear";
		case Enums.PlaceableItemSubtype.Amulet:
			if (!getPlural)
			{
				return "Amulet";
			}
			return "Amulets";
		case Enums.PlaceableItemSubtype.BodyArmor:
			if (!getPlural)
			{
				return "Body Armor";
			}
			return "Body Armors";
		case Enums.PlaceableItemSubtype.Gloves:
			if (!getPlural)
			{
				return "Gloves";
			}
			return "Gloves";
		case Enums.PlaceableItemSubtype.LegArmor:
			if (!getPlural)
			{
				return "Leg Armor";
			}
			return "Leg Armors";
		case Enums.PlaceableItemSubtype.Shield:
			if (!getPlural)
			{
				return "Shield";
			}
			return "Shields";
		default:
			Debug.LogWarning(string.Format("PlaceableItemSubtype {0} is not handled in {1}.{2}()", itemSubtype, "StringHelper", "GetCleanString"));
			return itemSubtype.ToString();
		}
	}

	public static string GetCleanString(Enums.PlaceableWeaponSubtype weaponSubtype, bool getPlural)
	{
		switch (weaponSubtype)
		{
		case Enums.PlaceableWeaponSubtype.Sword:
			if (!getPlural)
			{
				return "Sword";
			}
			return "Swords";
		case Enums.PlaceableWeaponSubtype.Bow:
			if (!getPlural)
			{
				return "Bow";
			}
			return "Bows";
		case Enums.PlaceableWeaponSubtype.Crossbow:
			if (!getPlural)
			{
				return "Crossbow";
			}
			return "Crossbows";
		case Enums.PlaceableWeaponSubtype.Axe:
			if (!getPlural)
			{
				return "Axe";
			}
			return "Axes";
		case Enums.PlaceableWeaponSubtype.Exotic:
			if (!getPlural)
			{
				return "Exotic";
			}
			return "Exotics";
		case Enums.PlaceableWeaponSubtype.Firearm:
			if (!getPlural)
			{
				return "Firearm";
			}
			return "Firearms";
		case Enums.PlaceableWeaponSubtype.FistWeapon:
			if (!getPlural)
			{
				return "Fist Weapon";
			}
			return "Fist Weapons";
		case Enums.PlaceableWeaponSubtype.Halberd:
			if (!getPlural)
			{
				return "Halberd";
			}
			return "Halberds";
		case Enums.PlaceableWeaponSubtype.Hammer:
			if (!getPlural)
			{
				return "Hammer";
			}
			return "Hammers";
		case Enums.PlaceableWeaponSubtype.Javelin:
			if (!getPlural)
			{
				return "Javelin";
			}
			return "Javelins";
		case Enums.PlaceableWeaponSubtype.Spellbook:
			if (!getPlural)
			{
				return "Spellbook";
			}
			return "Spellbooks";
		case Enums.PlaceableWeaponSubtype.Staff:
			if (!getPlural)
			{
				return "Staff";
			}
			return "Staves";
		case Enums.PlaceableWeaponSubtype.Throwing:
			if (!getPlural)
			{
				return "Throwable";
			}
			return "Throwables";
		case Enums.PlaceableWeaponSubtype.Wand:
			if (!getPlural)
			{
				return "Wand";
			}
			return "Wands";
		case Enums.PlaceableWeaponSubtype.Whip:
			if (!getPlural)
			{
				return "Whip";
			}
			return "Whips";
		default:
			Debug.LogWarning(string.Format("PlaceableWeaponSubtype {0} is not handled in {1}.{2}()", weaponSubtype, "StringHelper", "GetCleanString"));
			return weaponSubtype.ToString();
		}
	}

	public static string GetCleanString(Enums.Resolutions resolution)
	{
		return resolution switch
		{
			Enums.Resolutions._1920_x_1080 => "1920 x 1080", 
			Enums.Resolutions._2560_x_1440 => "2560 x 1440", 
			Enums.Resolutions._3840_x_2160 => "3840 x 2160", 
			Enums.Resolutions._1128_x_634 => "1128 x 634", 
			Enums.Resolutions._1280_x_720 => "1280 x 720", 
			Enums.Resolutions._1366_x_766 => "1366 x 766", 
			Enums.Resolutions._1600_x_900 => "1600 x 900", 
			Enums.Resolutions._1760_x_990 => "1760 x 990", 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.Windowmodes windowmode)
	{
		return windowmode switch
		{
			Enums.Windowmodes.Windowed => SingletonController<LocalizationController>.Instance.Translate("Windowed"), 
			Enums.Windowmodes.WindowedBorderless => SingletonController<LocalizationController>.Instance.Translate("Fullscreen"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.CameraShake bloom)
	{
		return bloom switch
		{
			Enums.CameraShake.Enabled => SingletonController<LocalizationController>.Instance.Translate("Enabled"), 
			Enums.CameraShake.Disabled => SingletonController<LocalizationController>.Instance.Translate("Disabled"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.Bloom bloom)
	{
		return bloom switch
		{
			Enums.Bloom.Enabled => SingletonController<LocalizationController>.Instance.Translate("Enabled"), 
			Enums.Bloom.Disabled => SingletonController<LocalizationController>.Instance.Translate("Disabled"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.FlashOnDamageTaken flashOnDamageTaken)
	{
		return flashOnDamageTaken switch
		{
			Enums.FlashOnDamageTaken.Enabled => SingletonController<LocalizationController>.Instance.Translate("Enabled"), 
			Enums.FlashOnDamageTaken.Disabled => SingletonController<LocalizationController>.Instance.Translate("Disabled"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.WeatherEffects weather)
	{
		return weather switch
		{
			Enums.WeatherEffects.Enabled => SingletonController<LocalizationController>.Instance.Translate("Enabled"), 
			Enums.WeatherEffects.Disabled => SingletonController<LocalizationController>.Instance.Translate("Disabled"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.QualityShaders qualityShaders)
	{
		return qualityShaders switch
		{
			Enums.QualityShaders.Enabled => SingletonController<LocalizationController>.Instance.Translate("Enabled"), 
			Enums.QualityShaders.Disabled => SingletonController<LocalizationController>.Instance.Translate("Disabled"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.TooltipComplexity tooltipComplexity)
	{
		return tooltipComplexity switch
		{
			Enums.TooltipComplexity.AlwaysVisible => "Always visible", 
			Enums.TooltipComplexity.VisibleOnAlt => "Only on Hotkey", 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.ShowDamageNumbers showDamageNumbers)
	{
		return showDamageNumbers switch
		{
			Enums.ShowDamageNumbers.Visible => SingletonController<LocalizationController>.Instance.Translate("Enabled"), 
			Enums.ShowDamageNumbers.Hidden => SingletonController<LocalizationController>.Instance.Translate("Disabled"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.ShowHealthBars showHealthBars)
	{
		return showHealthBars switch
		{
			Enums.ShowHealthBars.Visible => SingletonController<LocalizationController>.Instance.Translate("Enabled"), 
			Enums.ShowHealthBars.Hidden => SingletonController<LocalizationController>.Instance.Translate("Disabled"), 
			Enums.ShowHealthBars.OnlyBosses => "Only Bosses", 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.CooldownVisuals cooldownVisuals)
	{
		return cooldownVisuals switch
		{
			Enums.CooldownVisuals.Bar => SingletonController<LocalizationController>.Instance.Translate("Bar"), 
			Enums.CooldownVisuals.Icon => SingletonController<LocalizationController>.Instance.Translate("Icon"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.MinimapVisual showHealthBars)
	{
		return showHealthBars switch
		{
			Enums.MinimapVisual.Visible => SingletonController<LocalizationController>.Instance.Translate("Enabled"), 
			Enums.MinimapVisual.Hidden => SingletonController<LocalizationController>.Instance.Translate("Disabled"), 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.WorldDetail worldDetail)
	{
		return worldDetail switch
		{
			Enums.WorldDetail.Full => "Full", 
			Enums.WorldDetail.Medium => "Medium", 
			Enums.WorldDetail.Low => "Low", 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.PlaceableType placeableType)
	{
		return placeableType switch
		{
			Enums.PlaceableType.Weapon => "Weapon", 
			Enums.PlaceableType.Bag => "Bag", 
			Enums.PlaceableType.Item => "Item", 
			_ => placeableType.ToString(), 
		};
	}

	public static string GetCleanString(Enums.PlaceableType placeableType, bool getSingular)
	{
		if (getSingular)
		{
			return GetCleanString(placeableType);
		}
		return placeableType switch
		{
			Enums.PlaceableType.Weapon => "Weapons", 
			Enums.PlaceableType.Bag => "Bags", 
			Enums.PlaceableType.Item => "Items", 
			_ => placeableType.ToString(), 
		};
	}

	public static string GetCleanString(Enums.CurrencyType currencyType)
	{
		return currencyType switch
		{
			Enums.CurrencyType.TitanSouls => "Titan Souls", 
			Enums.CurrencyType.Coins => "Gold", 
			_ => currencyType.ToString(), 
		};
	}

	public static string GetCleanString(Enums.PlaceableItemSubtype itemSubtype)
	{
		return itemSubtype switch
		{
			Enums.PlaceableItemSubtype.Trinket => "Trinket", 
			Enums.PlaceableItemSubtype.Shield => "Shield", 
			Enums.PlaceableItemSubtype.BodyArmor => "BodyArmor", 
			Enums.PlaceableItemSubtype.LegArmor => "LegArmor", 
			Enums.PlaceableItemSubtype.Boots => "Boots", 
			Enums.PlaceableItemSubtype.Gloves => "Gloves", 
			Enums.PlaceableItemSubtype.Amulet => "Amulet", 
			Enums.PlaceableItemSubtype.Ring => "Ring", 
			Enums.PlaceableItemSubtype.Headwear => "Headwear", 
			_ => itemSubtype.ToString(), 
		};
	}

	public static string GetCleanString(Enums.WeaponStatType statType)
	{
		switch (statType)
		{
		case Enums.WeaponStatType.CritChancePercentage:
			return "Crit Chance";
		case Enums.WeaponStatType.CritMultiplier:
			return "Crit Multiplier";
		case Enums.WeaponStatType.DamagePercentage:
			return "Damage";
		case Enums.WeaponStatType.CooldownTime:
			return "Cooldown Reduction";
		case Enums.WeaponStatType.WeaponRange:
			return "Weapon Range";
		case Enums.WeaponStatType.Penetrating:
			return "Penetrating";
		case Enums.WeaponStatType.ExplosionSizePercentage:
			return "Area of Effect Size";
		case Enums.WeaponStatType.LifeDrainPercentage:
			return "Life Drain";
		case Enums.WeaponStatType.ProjectileCount:
			return "Projectile Count";
		case Enums.WeaponStatType.ProjectileSpeed:
			return "Projectile Speed";
		case Enums.WeaponStatType.ProjectileSizePercentage:
			return "Projectile Size";
		case Enums.WeaponStatType.StunChancePercentage:
			return "Stun Chance";
		case Enums.WeaponStatType.CooldownReductionPercentage:
			return "Cooldown Reduction";
		case Enums.WeaponStatType.FlatDamage:
			return "Base Damage";
		case Enums.WeaponStatType.ProjectileDuration:
			return "Projectile Duration";
		default:
			Debug.LogWarning($"{statType} has no cleaned string!");
			return string.Empty;
		}
	}

	public static string GetCleanString(Enums.ItemModifierSourceType statType)
	{
		return statType switch
		{
			Enums.ItemModifierSourceType.Talent => "Talent", 
			Enums.ItemModifierSourceType.ConnectedItem => "Item", 
			Enums.ItemModifierSourceType.GlobalItem => "Item", 
			Enums.ItemModifierSourceType.ModifierOverride => string.Empty, 
			Enums.ItemModifierSourceType.Relic => "Item", 
			_ => string.Empty, 
		};
	}

	public static string GetCleanString(Enums.ItemStatType statType)
	{
		switch (statType)
		{
		case Enums.ItemStatType.CritChancePercentage:
			return "Crit Chance";
		case Enums.ItemStatType.CritMultiplier:
			return "Crit Multiplier";
		case Enums.ItemStatType.DamagePercentage:
			return "Damage";
		case Enums.ItemStatType.CooldownTime:
			return "Cooldown Reduction";
		case Enums.ItemStatType.WeaponRange:
			return "Weapon Range";
		case Enums.ItemStatType.Penetrating:
			return "Penetrating";
		case Enums.ItemStatType.ExplosionSizePercentage:
			return "Area of Effect Size";
		case Enums.ItemStatType.LifeDrainPercentage:
			return "Life Drain";
		case Enums.ItemStatType.ProjectileCount:
			return "Projectile Count";
		case Enums.ItemStatType.ProjectileSpeed:
			return "Projectile Speed";
		case Enums.ItemStatType.ProjectileSizePercentage:
			return "Projectile Size";
		case Enums.ItemStatType.Health:
			return "Health";
		case Enums.ItemStatType.HealthRegeneration:
			return "Health Regeneration";
		case Enums.ItemStatType.SpeedPercentage:
			return "Speed";
		case Enums.ItemStatType.LuckPercentage:
			return "Luck";
		case Enums.ItemStatType.DamageReductionPercentageDONOTUSE:
			return "Damage Reduction DO NOT USE";
		case Enums.ItemStatType.Armor:
			return "Armor";
		case Enums.ItemStatType.FlatDamage:
			return "Base Damage";
		case Enums.ItemStatType.PickupRadiusPercentage:
			return "Pickup Radius";
		case Enums.ItemStatType.EnemyCount:
			return "Enemy Count";
		case Enums.ItemStatType.Spiked:
			return "Spiked";
		case Enums.ItemStatType.StunChancePercentage:
			return "Stun Chance";
		case Enums.ItemStatType.ExtraCoinChancePercentage:
			return "Extra Coin Chance";
		case Enums.ItemStatType.CooldownReductionPercentage:
			return "Cooldown Reduction";
		case Enums.ItemStatType.ExtraDash:
			return "Dashes";
		case Enums.ItemStatType.DodgePercentage:
			return "Dodge Chance";
		case Enums.ItemStatType.ProjectileDuration:
			return "Projectile Duration";
		case Enums.ItemStatType.MaximumCompanionCount:
			return "Maximum Companion Count";
		case Enums.ItemStatType.WeaponCapacity:
			return "Weapon Capacity";
		case Enums.ItemStatType.DamageAgainstNormalEnemies:
			return "Damage Against weak enemies";
		case Enums.ItemStatType.DamageAgainstEliteAndBossEnemies:
			return "Damage Against strong enemies";
		case Enums.ItemStatType.ExperienceGainedPercentage:
			return "Experience Gained";
		case Enums.ItemStatType.DebuffDuration:
			return "Debuff Duration";
		case Enums.ItemStatType.BuffDuration:
			return "Buff Duration";
		case Enums.ItemStatType.Knockback:
			return "Knockback";
		default:
			Debug.LogWarning($"{statType} has no cleaned string!");
			return string.Empty;
		}
	}

	internal static string FromSecondsToCleanTimespan(int seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		return $"{timeSpan.Hours:D2}h:{timeSpan.Minutes:D2}m:{timeSpan.Seconds:D2}s";
	}

	internal static string GetDescription(Enums.CurrencyType currencyType)
	{
		return currencyType switch
		{
			Enums.CurrencyType.Coins => "Use these to spend on <sprite name=\"Item\"> <color=#FFFFFF>items</color>, <sprite name=\"Weapon\"> <color=#FFFFFF>weapons</color> and <sprite name=\"Bag\"> <color=#FFFFFF>Bags</color> in the <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">Shop</color>.", 
			Enums.CurrencyType.TitanSouls => "Use these for <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">upgrades</color> at the <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">Altar of Changes</color>.", 
			_ => string.Empty, 
		};
	}

	internal static string GetDescription(Enums.DamageType damageType)
	{
		switch (damageType)
		{
		case Enums.DamageType.None:
			return "Additional damage caused by SLASHING Weapons and Attacks.";
		case Enums.DamageType.PhysicalDONOTUSE:
			return "Additional damage caused by PHYSICAL Weapons and Attacks";
		case Enums.DamageType.Fire:
			return "Additional damage caused by FIRE Weapons and Attacks";
		case Enums.DamageType.Cold:
			return "Additional damage caused by COLD Weapons and Attacks";
		case Enums.DamageType.Lightning:
			return "Additional damage caused by LIGHTNING Weapons and Attacks";
		case Enums.DamageType.Void:
			return "Additional damage caused by VOID Weapons and Attacks";
		case Enums.DamageType.Poison:
			return "Additional damage caused by POISON Weapons and Attacks";
		case Enums.DamageType.Energy:
			return "Additional damage caused by ENERGY Weapons and Attacks";
		case Enums.DamageType.Holy:
			return "Additional damage caused by HOLY Weapons and Attacks";
		case Enums.DamageType.Blunt:
			return "Additional damage caused by BLUNT Weapons and Attacks";
		case Enums.DamageType.Slashing:
			return "Additional damage caused by SLASHING Weapons and Attacks";
		case Enums.DamageType.Piercing:
			return "Additional damage caused by PIERCING Weapons and Attacks";
		default:
			Debug.LogWarning($"{damageType} has no cleaned DESCRIPTION string!");
			return string.Empty;
		}
	}

	internal static string GetDescription(Enums.ItemStatType statType)
	{
		switch (statType)
		{
		case Enums.ItemStatType.CritChancePercentage:
			return "Your chance to do critical damage on an attack.";
		case Enums.ItemStatType.CritMultiplier:
			return "The amount of extra damage when the attack critted.";
		case Enums.ItemStatType.DamagePercentage:
			return "Global damage increase - affects all attacks.";
		case Enums.ItemStatType.CooldownTime:
			return "Reduces the cooldown your items have after an attack. The cooldown cannot be reduced below 25% of the original cooldown.";
		case Enums.ItemStatType.WeaponRange:
			return "The range your weapons attack targets at.";
		case Enums.ItemStatType.Penetrating:
			return "The amount of enemies your attacks can penetrate.";
		case Enums.ItemStatType.ExplosionSizePercentage:
			return "This increases the size of your area of affect effects.";
		case Enums.ItemStatType.LifeDrainPercentage:
			return "The chance per attack to heal 1 life. (0.1s cooldown)";
		case Enums.ItemStatType.ProjectileCount:
			return "The amount of projectiles fired with each weapon attack.";
		case Enums.ItemStatType.ProjectileSpeed:
			return "The speed weapon projectiles move at.";
		case Enums.ItemStatType.ProjectileSizePercentage:
			return "The size of your weapon attacks that use projectiles.";
		case Enums.ItemStatType.Health:
			return "Your health - if you lose it all you die.";
		case Enums.ItemStatType.HealthRegeneration:
			return "Health regained every 5 seconds.";
		case Enums.ItemStatType.SpeedPercentage:
			return "The speed your character moves at.";
		case Enums.ItemStatType.LuckPercentage:
			return "Your luck, affecting the rarity of items appearing in the shop. Also affects the chance of finding Altars and usable drops from enemies.";
		case Enums.ItemStatType.DamageReductionPercentageDONOTUSE:
			return "A percentual damage reduction. Affects all damage taken and reduces by this % amount.";
		case Enums.ItemStatType.Armor:
		{
			float calculatedStat = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.Armor);
			float f = 100f * (1f - DamageEngine.GetTotalPercentageOfDamageDoneVsArmor(calculatedStat));
			return string.Format((calculatedStat >= 0f) ? "Modifies incoming damage. Currently preventing <color={1}>{0}%</color> of incoming damage." : "Modifies incoming damage. Currently taking <color={1}>{0}%</color> extra incoming damage.", arg1: (calculatedStat >= 0f) ? ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase) : ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase), arg0: (int)Mathf.Abs(f));
		}
		case Enums.ItemStatType.FlatDamage:
			return "Increases damage done by all weapon attacks by a flat amount.";
		case Enums.ItemStatType.PickupRadiusPercentage:
			return "The radius at which your character can pickup items.";
		case Enums.ItemStatType.EnemyCount:
			return "The amount of enemies occasionally spawning additionally during an adventure.";
		case Enums.ItemStatType.Spiked:
			return "Damage returned to the attacker when your character is damaged.";
		case Enums.ItemStatType.StunChancePercentage:
			return "The chance your weapon attacks can stun the target enemy";
		case Enums.ItemStatType.ExtraCoinChancePercentage:
			return "The chance of an enemy dropping additional coins on death";
		case Enums.ItemStatType.CooldownReductionPercentage:
			return "Reduces the cooldown your items have after an attack. The cooldown cannot be reduced below 25% of the original cooldown.";
		case Enums.ItemStatType.ExtraDash:
			return "The amount of dashes your character has.";
		case Enums.ItemStatType.DodgePercentage:
			return "The chance of avoiding an enemy attack fully.";
		case Enums.ItemStatType.ProjectileDuration:
			return "The duration your weapon attack projectiles are alive before destroyed.";
		case Enums.ItemStatType.MaximumCompanionCount:
			return "The maximum amount of companions allowed.";
		case Enums.ItemStatType.WeaponCapacity:
			return "The maximum amount of weapons you can have active at the same time";
		default:
			Debug.LogWarning($"{statType} has no cleaned DESCRIPTION string!");
			return string.Empty;
		}
	}

	internal static string GetButtonText(Enums.GameMenuButtonType gameMenuButtonType)
	{
		return gameMenuButtonType switch
		{
			Enums.GameMenuButtonType.Resume => "Resume", 
			Enums.GameMenuButtonType.Character => "Character", 
			Enums.GameMenuButtonType.Settings => "Settings", 
			Enums.GameMenuButtonType.BackToTown => "Back to Town", 
			Enums.GameMenuButtonType.ExitGame => "Main Menu", 
			Enums.GameMenuButtonType.QuitGame => "Quit Game", 
			Enums.GameMenuButtonType.Collection => "Collection", 
			Enums.GameMenuButtonType.Feedback => "Feedback", 
			_ => string.Empty, 
		};
	}

	internal static object GetCleanValue(Enums.ItemModifierSourceType itemModifierSourceType, List<ItemStatModifier> itemStatModifiers)
	{
		return itemModifierSourceType switch
		{
			Enums.ItemModifierSourceType.Talent => "Talents", 
			Enums.ItemModifierSourceType.Relic => "Relics", 
			Enums.ItemModifierSourceType.Bag => "Bags", 
			Enums.ItemModifierSourceType.GlobalItem => "Items", 
			Enums.ItemModifierSourceType.ConnectedItem => "ConnectedItem", 
			Enums.ItemModifierSourceType.ConnectedWeapon => "ConnectedWeapon", 
			Enums.ItemModifierSourceType.ModifierOverride => "ModifierOverride", 
			Enums.ItemModifierSourceType.ConditionalStatsItem => CreateConditionStatsItem(itemStatModifiers) ?? "", 
			Enums.ItemModifierSourceType.BaseCharacter => "Character", 
			Enums.ItemModifierSourceType.Buff => "Buffs", 
			_ => string.Empty, 
		};
	}

	internal static object GetCleanValue(Enums.ItemModifierSourceType itemModifierSourceType, List<DamageTypeValueModifier> itemStatModifiers)
	{
		return itemModifierSourceType switch
		{
			Enums.ItemModifierSourceType.Talent => "Talents", 
			Enums.ItemModifierSourceType.Relic => "Relics", 
			Enums.ItemModifierSourceType.Bag => "Bags", 
			Enums.ItemModifierSourceType.GlobalItem => "Items", 
			Enums.ItemModifierSourceType.ConnectedItem => "ConnectedItem", 
			Enums.ItemModifierSourceType.ConnectedWeapon => "ConnectedWeapon", 
			Enums.ItemModifierSourceType.ModifierOverride => "ModifierOverride", 
			Enums.ItemModifierSourceType.ConditionalStatsItem => CreateConditionStatsItem(itemStatModifiers) ?? "", 
			Enums.ItemModifierSourceType.BaseCharacter => "Character", 
			_ => string.Empty, 
		};
	}

	private static string CreateConditionStatsItem(List<ItemStatModifier> itemStatModifiers)
	{
		string text = string.Empty;
		foreach (ItemStatModifier itemStatModifier in itemStatModifiers)
		{
			if (itemStatModifier.Source != null && itemStatModifier.Source.ConditionalStatsItem != null)
			{
				text = text + itemStatModifier.Source.ConditionalStatsItem.Name + ", ";
			}
		}
		if (text.Contains(", "))
		{
			text = text.Remove(text.Length - 2, 2);
		}
		return text;
	}

	private static string CreateConditionStatsItem(List<DamageTypeValueModifier> itemStatModifiers)
	{
		string text = string.Empty;
		foreach (DamageTypeValueModifier itemStatModifier in itemStatModifiers)
		{
			if (itemStatModifier.Source != null && itemStatModifier.Source.ConditionalStatsItem != null)
			{
				text = text + itemStatModifier.Source.ConditionalStatsItem.Name + ", ";
			}
		}
		if (text.Contains(", "))
		{
			text = text.Remove(text.Length - 2, 2);
		}
		return text;
	}

	internal static string GetDescription(Enums.Debuff.DebuffType debuffType)
	{
		return debuffType switch
		{
			Enums.Debuff.DebuffType.Burn => "Damages an enemy every 0.5 second for " + GetFullSpriteValue(Enums.DamageType.Fire) + " <color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Fire) + ">FIRE</color> damage. Damage per tick is <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">25%</color> of the original damage. Burn lasts <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">6</color> seconds. Does not stack.", 
			Enums.Debuff.DebuffType.Poison => "Damages an enemy every 0.5 second for " + GetFullSpriteValue(Enums.DamageType.Poison) + " <color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Poison) + ">POISON</color> damage. Damage per tick is <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">5%</color> of the original damage. Poison lasts for <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">10</color> seconds, but stacks infinitly if reapplied.", 
			Enums.Debuff.DebuffType.Bleed => "Damages an enemy every 0.5 second for " + GetFullSpriteValue(Enums.DamageType.Slashing) + " <color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Slashing) + ">SLASHING</color> damage. Damage per tick is 15% of the original damage. Bleed lasts for <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">20</color> seconds. Stacks up to <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">20</color>.", 
			Enums.Debuff.DebuffType.ArmorReduction => "Marks an enemy as vulnerable. Vulnerable enemies take <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">5%</color> more damage per stack from " + GetFullSpriteValue(Enums.DamageType.Blunt) + " <color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Blunt) + ">Blunt</color>, " + GetFullSpriteValue(Enums.DamageType.Piercing) + " <color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Piercing) + ">Piercing</color> or " + GetFullSpriteValue(Enums.DamageType.Slashing) + " <color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Slashing) + ">Slashing</color></color> attacks. Stacks up to <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">20</color>.", 
			Enums.Debuff.DebuffType.Slow => "Slows an enemy by <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">20%</color> for <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">3</color> seconds. Does not stack, but can be constantly reapplied. Does not work on bosses and minibosses.", 
			Enums.Debuff.DebuffType.Stun => "Stuns an enemy for <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">1</color> second. Enemy cannot be stunned again for <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">3</color> seconds. Does not work on bosses and minibosses.", 
			Enums.Debuff.DebuffType.Arcing => "Marks an enemy for <color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase) + ">6</color> seconds. The enemy chains " + GetFullSpriteValue(Enums.DamageType.Lightning) + " <color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Lightning) + ">Lightning</color> around itself, damaging other enemies for 50% of the original weapon damage.", 
			_ => string.Empty, 
		};
	}

	internal static string GetColoredName(Enums.Debuff.DebuffType debuffType)
	{
		return debuffType switch
		{
			Enums.Debuff.DebuffType.Burn => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Fire) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.Poison => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Poison) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.Bleed => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Slashing) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.ArmorReduction => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Void) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.Slow => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Cold) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.Stun => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Blunt) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.Arcing => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Lightning) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.Slowdown => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Cold) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.Twilight => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Holy) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.Gloom => "<color=" + ColorHelper.GetColorStringForDamageType(Enums.DamageType.Void) + ">" + GetCleanString(debuffType) + "</color>", 
			Enums.Debuff.DebuffType.None => string.Empty, 
			_ => debuffType.ToString().ToUpper(), 
		};
	}

	internal static string GetCleanString(Enums.AttackTargetingType baseAttackType)
	{
		return baseAttackType switch
		{
			Enums.AttackTargetingType.AttackClosestEnemy => "Targets closest enemy", 
			Enums.AttackTargetingType.AttackRandomEnemy => "Targets a random enemy in range", 
			Enums.AttackTargetingType.TargetCursorDONOTUSE => "DO NOT USE", 
			Enums.AttackTargetingType.AttackPlayer => "On player", 
			Enums.AttackTargetingType.AttackPlayerDirection => "Targets player direction", 
			Enums.AttackTargetingType.AttackPlayerAim => "Targets player aim", 
			Enums.AttackTargetingType.AttackCardinalDirection => "Targets a specific direction", 
			Enums.AttackTargetingType.None => "None", 
			_ => string.Empty, 
		};
	}

	internal static string GetSpriteValue(Enums.CurrencyType currencyType)
	{
		return currencyType switch
		{
			Enums.CurrencyType.Coins => "Coin", 
			Enums.CurrencyType.TitanSouls => "TitanSoul", 
			_ => string.Empty, 
		};
	}

	internal static string GetSpriteValue(Enums.RuneSpecialEffectDestructionType runeSpecialEffectDestructionType)
	{
		return runeSpecialEffectDestructionType switch
		{
			Enums.RuneSpecialEffectDestructionType.DestroyAfterTrigger => "<sprite name=\"SpecialItemDestroyAfterTrigger\">", 
			Enums.RuneSpecialEffectDestructionType.DestroyAfterShopEntering => "<sprite name=\"SpecialItemDestruction\">", 
			Enums.RuneSpecialEffectDestructionType.NeverDestroy => "<sprite name=\"SpecialItemNeverDestroy\">", 
			Enums.RuneSpecialEffectDestructionType.DestroyAfterX => "<sprite name=\"SpecialItemDestroyAfterX\">", 
			_ => string.Empty, 
		};
	}

	internal static string GetSpriteValue(Enums.WeaponStatType weaponStatType)
	{
		return weaponStatType switch
		{
			Enums.WeaponStatType.CritChancePercentage => "CritChancePercentage", 
			Enums.WeaponStatType.CritMultiplier => "CritMultiplier", 
			Enums.WeaponStatType.DamagePercentage => "DamagePercentage", 
			Enums.WeaponStatType.CooldownTime => "CooldownTime", 
			Enums.WeaponStatType.WeaponRange => "WeaponRange", 
			Enums.WeaponStatType.Penetrating => "Penetrating", 
			Enums.WeaponStatType.ExplosionSizePercentage => "ExplosionSizePercentage", 
			Enums.WeaponStatType.LifeDrainPercentage => "LifeDrainPercentage", 
			Enums.WeaponStatType.ProjectileCount => "ProjectileCount", 
			Enums.WeaponStatType.ProjectileSpeed => "ProjectileSpeed", 
			Enums.WeaponStatType.ProjectileSizePercentage => "ProjectileSizePercentage", 
			Enums.WeaponStatType.StunChancePercentage => "StunChancePercentage", 
			Enums.WeaponStatType.CooldownReductionPercentage => "CooldownReductionPercentage", 
			Enums.WeaponStatType.FlatDamage => "FlatDamage", 
			Enums.WeaponStatType.ProjectileDuration => "ProjectileDuration", 
			_ => "[NO ICON]", 
		};
	}

	internal static string GetSpriteValue(Enums.ItemStatType itemStatType)
	{
		return itemStatType switch
		{
			Enums.ItemStatType.CritChancePercentage => "CritChancePercentage", 
			Enums.ItemStatType.CritMultiplier => "CritMultiplier", 
			Enums.ItemStatType.Health => "Health", 
			Enums.ItemStatType.HealthRegeneration => "HealthRegeneration", 
			Enums.ItemStatType.DamagePercentage => "DamagePercentage", 
			Enums.ItemStatType.SpeedPercentage => "SpeedPercentage", 
			Enums.ItemStatType.LuckPercentage => "LuckPercentage", 
			Enums.ItemStatType.CooldownTime => "CooldownTime", 
			Enums.ItemStatType.DamageReductionPercentageDONOTUSE => "DamageReductionPercentage", 
			Enums.ItemStatType.Armor => "Armor", 
			Enums.ItemStatType.Spiked => "Spiked", 
			Enums.ItemStatType.FlatDamage => "FlatDamage", 
			Enums.ItemStatType.PickupRadiusPercentage => "PickupRadiusPercentage", 
			Enums.ItemStatType.EnemyCount => "EnemyCount", 
			Enums.ItemStatType.WeaponRange => "WeaponRange", 
			Enums.ItemStatType.Penetrating => "Penetrating", 
			Enums.ItemStatType.ExplosionSizePercentage => "ExplosionSizePercentage", 
			Enums.ItemStatType.LifeDrainPercentage => "LifeDrainPercentage", 
			Enums.ItemStatType.ProjectileCount => "ProjectileCount", 
			Enums.ItemStatType.ProjectileSpeed => "ProjectileSpeed", 
			Enums.ItemStatType.ProjectileSizePercentage => "ProjectileSizePercentage", 
			Enums.ItemStatType.StunChancePercentage => "StunChancePercentage", 
			Enums.ItemStatType.ExtraCoinChancePercentage => "ExtraCoinChancePercentage", 
			Enums.ItemStatType.CooldownReductionPercentage => "CooldownTime", 
			Enums.ItemStatType.ExtraDash => "DashCount", 
			Enums.ItemStatType.DodgePercentage => "DodgePercentage", 
			Enums.ItemStatType.ProjectileDuration => "ProjectileDuration", 
			Enums.ItemStatType.MaximumCompanionCount => "MaximumCompanionCount", 
			Enums.ItemStatType.WeaponCapacity => "WeaponCapacity", 
			_ => "[NO ICON]", 
		};
	}

	internal static string GetSpriteValue(Enums.DamageType damageType)
	{
		return damageType switch
		{
			Enums.DamageType.None => "DamageTypeNone", 
			Enums.DamageType.PhysicalDONOTUSE => "DamageTypePhysical", 
			Enums.DamageType.Fire => "DamageTypeFire", 
			Enums.DamageType.Cold => "DamageTypeCold", 
			Enums.DamageType.Lightning => "DamageTypeLightning", 
			Enums.DamageType.Void => "DamageTypeVoid", 
			Enums.DamageType.Poison => "DamageTypePoison", 
			Enums.DamageType.Energy => "DamageTypeEnergy", 
			Enums.DamageType.Holy => "DamageTypeHoly", 
			Enums.DamageType.Blunt => "Bludgeon", 
			Enums.DamageType.Slashing => "Slashing", 
			Enums.DamageType.Piercing => "Piercing", 
			_ => "", 
		};
	}

	internal static string GetFullSpriteValue(Enums.DamageType damageType)
	{
		return "<sprite name=\"" + GetSpriteValue(damageType) + "\">";
	}

	internal static string GetFullSpriteValue(Enums.WeaponType weaponType)
	{
		return "<sprite name=\"" + GetSpriteValue(weaponType) + "\">";
	}

	internal static string AddLink(string core, string linkTag)
	{
		return "<link=" + linkTag + "><u>" + core + "</u></link>";
	}

	internal static string GetSpriteValueByPlaceableTag(Enums.PlaceableTag placeableTag)
	{
		if (placeableTag <= Enums.PlaceableTag.Javelin)
		{
			if (placeableTag <= Enums.PlaceableTag.Gloves)
			{
				if (placeableTag <= Enums.PlaceableTag.Holy)
				{
					if (placeableTag <= Enums.PlaceableTag.Void)
					{
						if ((ulong)placeableTag <= 8uL)
						{
							switch (placeableTag)
							{
							case Enums.PlaceableTag.Physical:
								return GetSpriteValue(Enums.DamageType.PhysicalDONOTUSE);
							case Enums.PlaceableTag.Fire:
								return GetSpriteValue(Enums.DamageType.Fire);
							case Enums.PlaceableTag.Cold:
								return GetSpriteValue(Enums.DamageType.Cold);
							case Enums.PlaceableTag.Lightning:
								return GetSpriteValue(Enums.DamageType.Lightning);
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Fire:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Fire | Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Fire | Enums.PlaceableTag.Cold:
								goto IL_049c;
							case Enums.PlaceableTag.None:
								goto IL_04bb;
							}
						}
						if (placeableTag == Enums.PlaceableTag.Void)
						{
							return GetSpriteValue(Enums.DamageType.Void);
						}
					}
					else
					{
						switch (placeableTag)
						{
						case Enums.PlaceableTag.Poison:
							return GetSpriteValue(Enums.DamageType.Poison);
						case Enums.PlaceableTag.Energy:
							return GetSpriteValue(Enums.DamageType.Energy);
						case Enums.PlaceableTag.Holy:
							return GetSpriteValue(Enums.DamageType.Holy);
						}
					}
				}
				else
				{
					switch (placeableTag)
					{
					case Enums.PlaceableTag.Trinket:
						return GetSpriteValue(Enums.PlaceableItemSubtype.Trinket);
					case Enums.PlaceableTag.Shield:
						return GetSpriteValue(Enums.PlaceableItemSubtype.Shield);
					case Enums.PlaceableTag.BodyArmor:
						return GetSpriteValue(Enums.PlaceableItemSubtype.BodyArmor);
					case Enums.PlaceableTag.LegArmor:
						return GetSpriteValue(Enums.PlaceableItemSubtype.LegArmor);
					case Enums.PlaceableTag.Boots:
						return GetSpriteValue(Enums.PlaceableItemSubtype.Boots);
					case Enums.PlaceableTag.Gloves:
						return GetSpriteValue(Enums.PlaceableItemSubtype.Gloves);
					}
				}
			}
			else
			{
				switch (placeableTag)
				{
				case Enums.PlaceableTag.Amulet:
					return GetSpriteValue(Enums.PlaceableItemSubtype.Amulet);
				case Enums.PlaceableTag.Headwear:
					return GetSpriteValue(Enums.PlaceableItemSubtype.Headwear);
				case Enums.PlaceableTag.Sword:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Sword);
				case Enums.PlaceableTag.Hammer:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Hammer);
				case Enums.PlaceableTag.Axe:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Axe);
				case Enums.PlaceableTag.FistWeapon:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.FistWeapon);
				case Enums.PlaceableTag.Halberd:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Halberd);
				case Enums.PlaceableTag.Bow:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Bow);
				case Enums.PlaceableTag.Crossbow:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Crossbow);
				case Enums.PlaceableTag.Javelin:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Javelin);
				case Enums.PlaceableTag.Ring:
					return GetSpriteValue(Enums.PlaceableItemSubtype.Ring);
				}
			}
		}
		else if (placeableTag <= Enums.PlaceableTag.Uncommon)
		{
			if (placeableTag <= Enums.PlaceableTag.Whip)
			{
				switch (placeableTag)
				{
				case Enums.PlaceableTag.Throwing:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Throwing);
				case Enums.PlaceableTag.Wand:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Wand);
				case Enums.PlaceableTag.Staff:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Staff);
				case Enums.PlaceableTag.Spellbook:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Spellbook);
				case Enums.PlaceableTag.Whip:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Whip);
				}
			}
			else if (placeableTag <= Enums.PlaceableTag.Ranged)
			{
				switch (placeableTag)
				{
				case Enums.PlaceableTag.Exotic:
					return GetSpriteValue(Enums.PlaceableWeaponSubtype.Exotic);
				case Enums.PlaceableTag.Melee:
					return GetSpriteValue(Enums.WeaponType.Melee);
				case Enums.PlaceableTag.Ranged:
					return GetSpriteValue(Enums.WeaponType.Ranged);
				}
			}
			else
			{
				if (placeableTag == Enums.PlaceableTag.SpellDONOTUSE)
				{
					goto IL_04bb;
				}
				switch (placeableTag)
				{
				case Enums.PlaceableTag.Common:
					return GetSpriteValue(Enums.PlaceableRarity.Common);
				case Enums.PlaceableTag.Uncommon:
					return GetSpriteValue(Enums.PlaceableRarity.Uncommon);
				}
			}
		}
		else
		{
			switch (placeableTag)
			{
			case Enums.PlaceableTag.Blunt:
				return GetSpriteValue(Enums.DamageType.Blunt);
			case Enums.PlaceableTag.Slashing:
				return GetSpriteValue(Enums.DamageType.Slashing);
			case Enums.PlaceableTag.Piercing:
				return GetSpriteValue(Enums.DamageType.Piercing);
			case Enums.PlaceableTag.Rare:
				return GetSpriteValue(Enums.PlaceableRarity.Rare);
			case Enums.PlaceableTag.Epic:
				return GetSpriteValue(Enums.PlaceableRarity.Epic);
			case Enums.PlaceableTag.Legendary:
				return GetSpriteValue(Enums.PlaceableRarity.Legendary);
			case Enums.PlaceableTag.Mythic:
				return GetSpriteValue(Enums.PlaceableRarity.Mythic);
			case Enums.PlaceableTag.FireArm:
				return GetSpriteValue(Enums.PlaceableWeaponSubtype.Firearm);
			case Enums.PlaceableTag.Special:
				return GetSpriteValue(Enums.PlaceableItemSubtype.Special);
			case Enums.PlaceableTag.Unique:
				return GetSpriteValue(Enums.PlaceableRarity.Unique);
			case Enums.PlaceableTag.MergeIngredient:
				return "Recipes";
			}
		}
		goto IL_049c;
		IL_049c:
		Debug.LogWarning(string.Format("PlaceableTag {0} is not handled in {1}.{2}", placeableTag, "StringHelper", "GetSpriteValueByPlaceableTag"));
		goto IL_04bb;
		IL_04bb:
		return string.Empty;
	}

	private static string GetSpriteValueByPlaceableType(Enums.PlaceableType placeableType)
	{
		switch (placeableType)
		{
		case Enums.PlaceableType.Weapon:
			return "Weapon";
		case Enums.PlaceableType.Item:
			return "Item";
		case Enums.PlaceableType.Bag:
			return "Bag";
		default:
			Debug.LogWarning(string.Format("PlaceableType {0} is not handled in {1}.{2}", placeableType, "StringHelper", "GetSpriteValueByPlaceableType"));
			return string.Empty;
		}
	}

	internal static string GetFormulaTypeToCheckAgainstIcon(FormulaSO formula)
	{
		switch (formula.TypeToCheckAgainst)
		{
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
			return GetSpriteValueByPlaceableTag((Enums.PlaceableTag)formula.PlaceableTagLongValue);
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
			return GetSpriteValueByPlaceableType(formula.PlaceableType);
		case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
			return GetSpriteValue(formula.ItemStatType);
		case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
			return "Coin";
		default:
			Debug.LogWarning(string.Format("formula.TypeToCheckAgainst {0} is not handled in {1}.{2}", formula.TypeToCheckAgainst, "StringHelper", "GetFormulaTypeToCheckAgainstIcon"));
			return string.Empty;
		}
	}

	internal static string GetCleanString(Enums.PlaceableTag placeableTag)
	{
		if (placeableTag <= Enums.PlaceableTag.Javelin)
		{
			if (placeableTag <= Enums.PlaceableTag.Gloves)
			{
				if (placeableTag <= Enums.PlaceableTag.Holy)
				{
					if (placeableTag <= Enums.PlaceableTag.Void)
					{
						Enums.PlaceableTag num = placeableTag;
						if ((ulong)num <= 8uL)
						{
							switch (num)
							{
							case Enums.PlaceableTag.None:
								return placeableTag.ToString();
							case Enums.PlaceableTag.Physical:
								return placeableTag.ToString();
							case Enums.PlaceableTag.Fire:
								return placeableTag.ToString();
							case Enums.PlaceableTag.Cold:
								return placeableTag.ToString();
							case Enums.PlaceableTag.Lightning:
								return placeableTag.ToString();
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Fire:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Fire | Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Fire | Enums.PlaceableTag.Cold:
								goto IL_05df;
							}
						}
						if (placeableTag == Enums.PlaceableTag.Void)
						{
							return placeableTag.ToString();
						}
					}
					else
					{
						switch (placeableTag)
						{
						case Enums.PlaceableTag.Poison:
							return placeableTag.ToString();
						case Enums.PlaceableTag.Energy:
							return placeableTag.ToString();
						case Enums.PlaceableTag.Holy:
							return placeableTag.ToString();
						}
					}
				}
				else
				{
					switch (placeableTag)
					{
					case Enums.PlaceableTag.Trinket:
						return placeableTag.ToString();
					case Enums.PlaceableTag.Shield:
						return placeableTag.ToString();
					case Enums.PlaceableTag.BodyArmor:
						return placeableTag.ToString();
					case Enums.PlaceableTag.LegArmor:
						return placeableTag.ToString();
					case Enums.PlaceableTag.Boots:
						return placeableTag.ToString();
					case Enums.PlaceableTag.Gloves:
						return placeableTag.ToString();
					}
				}
			}
			else
			{
				switch (placeableTag)
				{
				case Enums.PlaceableTag.Amulet:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Ring:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Headwear:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Sword:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Hammer:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Axe:
					return placeableTag.ToString();
				case Enums.PlaceableTag.FistWeapon:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Halberd:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Bow:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Crossbow:
					return placeableTag.ToString();
				case Enums.PlaceableTag.Javelin:
					return placeableTag.ToString();
				}
			}
		}
		else
		{
			switch (placeableTag)
			{
			case Enums.PlaceableTag.Throwing:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Wand:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Staff:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Spellbook:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Whip:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Exotic:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Melee:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Ranged:
				return placeableTag.ToString();
			case Enums.PlaceableTag.SpellDONOTUSE:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Common:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Uncommon:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Rare:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Epic:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Legendary:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Mythic:
				return placeableTag.ToString();
			case Enums.PlaceableTag.FireArm:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Special:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Unique:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Blunt:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Slashing:
				return placeableTag.ToString();
			case Enums.PlaceableTag.Piercing:
				return placeableTag.ToString();
			case Enums.PlaceableTag.MergeIngredient:
				return "Ingredient";
			}
		}
		goto IL_05df;
		IL_05df:
		return placeableTag.ToString();
	}

	internal static string GetMovieStepText(Enums.MovieStep step)
	{
		return step switch
		{
			Enums.MovieStep.Step0_Black => string.Empty, 
			Enums.MovieStep.Step1_FromTheVoid => "It came from the Void...", 
			Enums.MovieStep.Step2_NoReason => "There was no communication, only destruction...", 
			Enums.MovieStep.Step3_VoidCorrupted => "Everything it touched was corrupted beyond recognition...", 
			Enums.MovieStep.Step3_5_OrcsCorrupted_ => "Some easier then others...\r\n\r\nEven nature was corrupted", 
			Enums.MovieStep.Step3_8_Black => string.Empty, 
			Enums.MovieStep.Step3_9_HumanCiv => "Humanity flurished for hundreds of years", 
			Enums.MovieStep.Step4_Gateways => "", 
			Enums.MovieStep.Step5_Fortresses => "Fortresses were erected to protect the Gateways and humanity", 
			Enums.MovieStep.Step6_EliteSoldiers => "Old magics forged powerfull warriors - The Aetherian Wardens", 
			Enums.MovieStep.Step7_HardBattle => "In their final hours, they stood strong...", 
			Enums.MovieStep.Step8_FellToTheHordes => "...but the Wardens were not enough. Humanity fell to the Void.", 
			Enums.MovieStep.Step8_5_Black => string.Empty, 
			Enums.MovieStep.Step9_BastionsDestroyed => "The bastions of humanity were destroyed.", 
			Enums.MovieStep.Step10_OneRemained => "However, one fortress remained. Hidden away during the cataclysm.", 
			Enums.MovieStep.Step11_ChildOfHumanity => "centuries later, the Warden bloodline continues...", 
			Enums.MovieStep.Step12_KeyIsGiven => "A chance for victory - a fortress key is crafted", 
			Enums.MovieStep.Step13_FinalFortress => "The final fortress has been found.", 
			Enums.MovieStep.Step13_5_Black => string.Empty, 
			Enums.MovieStep.Step14_DemonHordesAwait => "The currupted hordes await...\r\nThe void awaits...", 
			Enums.MovieStep.Step15_Ready => "Are you ready?", 
			Enums.MovieStep.Step15_5_Black => string.Empty, 
			_ => string.Empty, 
		};
	}

	internal static string GetCleanString(Enums.CraftingResource craftingResource)
	{
		return craftingResource switch
		{
			Enums.CraftingResource.DemonicDiamond => "Demonic Diamond", 
			Enums.CraftingResource.SpiderSilk => "Spider Silk", 
			Enums.CraftingResource.DamnationAcid => "Damnation Acid", 
			Enums.CraftingResource.CorruptedOre => "Corrupted Ore", 
			Enums.CraftingResource.HolyCrystal => "Holy Crystal", 
			_ => string.Empty, 
		};
	}

	internal static string GetCleanString(Enums.RuneSpecialEffectDestructionType runeSpecialEffectDestructionType)
	{
		return runeSpecialEffectDestructionType switch
		{
			Enums.RuneSpecialEffectDestructionType.DestroyAfterTrigger => "<color=#FF0000>Destroyed</color> after triggering", 
			Enums.RuneSpecialEffectDestructionType.DestroyAfterShopEntering => "<color=#FF0000>Destroyed</color> after entering the next shop", 
			Enums.RuneSpecialEffectDestructionType.NeverDestroy => "<color=#00A6FF>Triggers</color> <color=#FFFFFF>every</color> shop entry", 
			Enums.RuneSpecialEffectDestructionType.DestroyAfterX => "<color=#FF0000>Destroyed</color> after <color=#FFFFFF>[MaxTriggerCount]</color> triggers <color=#FFFFFF>([CurrentTriggerCount]/[MaxTriggerCount])</color>", 
			_ => string.Empty, 
		};
	}

	internal static string GetSpriteValue(Enums.PlaceableWeaponSubtype enumValue)
	{
		return enumValue switch
		{
			Enums.PlaceableWeaponSubtype.Sword => "PlaceableWeaponSubtypeSword", 
			Enums.PlaceableWeaponSubtype.Hammer => "PlaceableWeaponSubtypeHammer", 
			Enums.PlaceableWeaponSubtype.Axe => "PlaceableWeaponSubtypeAxe", 
			Enums.PlaceableWeaponSubtype.FistWeapon => "PlaceableWeaponSubtypeFistWeapon", 
			Enums.PlaceableWeaponSubtype.Halberd => "PlaceableWeaponSubtypeHalberd", 
			Enums.PlaceableWeaponSubtype.Bow => "PlaceableWeaponSubtypeBow", 
			Enums.PlaceableWeaponSubtype.Crossbow => "PlaceableWeaponSubtypeCrossbow", 
			Enums.PlaceableWeaponSubtype.Javelin => "PlaceableWeaponSubtypeJavelin", 
			Enums.PlaceableWeaponSubtype.Throwing => "PlaceableWeaponSubtypeThrowing", 
			Enums.PlaceableWeaponSubtype.Wand => "PlaceableWeaponSubtypeWand", 
			Enums.PlaceableWeaponSubtype.Staff => "PlaceableWeaponSubtypeStaff", 
			Enums.PlaceableWeaponSubtype.Spellbook => "PlaceableWeaponSubtypeSpellbook", 
			Enums.PlaceableWeaponSubtype.Whip => "PlaceableWeaponSubtypeWhip", 
			Enums.PlaceableWeaponSubtype.Exotic => "PlaceableWeaponSubtypeExotic", 
			Enums.PlaceableWeaponSubtype.Firearm => "PlaceableWeaponSubtypeFirearm", 
			Enums.PlaceableWeaponSubtype.None => string.Empty, 
			_ => string.Empty, 
		};
	}

	internal static string GetSpriteValue(Enums.PlaceableItemSubtype enumValue)
	{
		return enumValue switch
		{
			Enums.PlaceableItemSubtype.Trinket => "PlaceableItemSubtypeTrinket", 
			Enums.PlaceableItemSubtype.Shield => "PlaceableItemSubtypeShield", 
			Enums.PlaceableItemSubtype.BodyArmor => "PlaceableItemSubtypeBodyArmor", 
			Enums.PlaceableItemSubtype.LegArmor => "PlaceableItemSubtypeLegArmor", 
			Enums.PlaceableItemSubtype.Boots => "PlaceableItemSubtypeBoots", 
			Enums.PlaceableItemSubtype.Gloves => "PlaceableItemSubtypeGloves", 
			Enums.PlaceableItemSubtype.Amulet => "PlaceableItemSubtypeAmulet", 
			Enums.PlaceableItemSubtype.Ring => "PlaceableItemSubtypeRing", 
			Enums.PlaceableItemSubtype.Headwear => "PlaceableItemSubtypeHeadwear", 
			Enums.PlaceableItemSubtype.Special => "PlaceableItemSubtypeSpecial", 
			Enums.PlaceableItemSubtype.None => string.Empty, 
			_ => string.Empty, 
		};
	}

	internal static string GetSpriteValue(Enums.WeaponType enumValue)
	{
		return enumValue switch
		{
			Enums.WeaponType.None => string.Empty, 
			Enums.WeaponType.Melee => "WeaponTypeMelee", 
			Enums.WeaponType.Ranged => "WeaponTypeRanged", 
			Enums.WeaponType.All => string.Empty, 
			_ => string.Empty, 
		};
	}

	internal static string GetSpriteValue(Enums.PlaceableRarity enumValue)
	{
		return enumValue switch
		{
			Enums.PlaceableRarity.Common => "Common", 
			Enums.PlaceableRarity.Uncommon => "Uncommon", 
			Enums.PlaceableRarity.Rare => "Rare", 
			Enums.PlaceableRarity.Epic => "Epic", 
			Enums.PlaceableRarity.Legendary => "Legendary", 
			Enums.PlaceableRarity.Mythic => "Mythic", 
			Enums.PlaceableRarity.Unique => "Unique", 
			_ => string.Empty, 
		};
	}

	internal static string GetCleanString(Enums.Debuff.DebuffType debuffType)
	{
		return debuffType switch
		{
			Enums.Debuff.DebuffType.Burn => "BURN", 
			Enums.Debuff.DebuffType.Poison => "POISON", 
			Enums.Debuff.DebuffType.Bleed => "BLEED", 
			Enums.Debuff.DebuffType.ArmorReduction => "VULNERABLE", 
			Enums.Debuff.DebuffType.Slow => "SLOW", 
			Enums.Debuff.DebuffType.Stun => "STUN", 
			Enums.Debuff.DebuffType.Arcing => "ARCING", 
			Enums.Debuff.DebuffType.Slowdown => "SLOW DOWN", 
			Enums.Debuff.DebuffType.Twilight => "TWILIGHT", 
			Enums.Debuff.DebuffType.Gloom => "GLOOM", 
			Enums.Debuff.DebuffType.None => string.Empty, 
			_ => debuffType.ToString().ToUpper(), 
		};
	}

	internal static bool IsShownAsPercentage(Enums.ItemStatType itemStatType)
	{
		if (itemStatType != Enums.ItemStatType.LuckPercentage && itemStatType != Enums.ItemStatType.CooldownReductionPercentage && itemStatType != Enums.ItemStatType.CritChancePercentage && itemStatType != Enums.ItemStatType.DamagePercentage && itemStatType != Enums.ItemStatType.ExplosionSizePercentage && itemStatType != Enums.ItemStatType.ExtraCoinChancePercentage && itemStatType != Enums.ItemStatType.LifeDrainPercentage && itemStatType != Enums.ItemStatType.PickupRadiusPercentage && itemStatType != Enums.ItemStatType.ProjectileSizePercentage && itemStatType != Enums.ItemStatType.StunChancePercentage)
		{
			return itemStatType == Enums.ItemStatType.SpeedPercentage;
		}
		return true;
	}

	internal static bool IsShownAsPercentage(Enums.WeaponStatType weaponStatType)
	{
		if (weaponStatType != Enums.WeaponStatType.StunChancePercentage && weaponStatType != Enums.WeaponStatType.CritChancePercentage && weaponStatType != Enums.WeaponStatType.DamagePercentage && weaponStatType != Enums.WeaponStatType.ExplosionSizePercentage && weaponStatType != Enums.WeaponStatType.LifeDrainPercentage && weaponStatType != Enums.WeaponStatType.CooldownReductionPercentage)
		{
			return weaponStatType == Enums.WeaponStatType.ProjectileSizePercentage;
		}
		return true;
	}
}
