using BackpackSurvivors.Game.Core;
using UnityEngine;

namespace BackpackSurvivors.System.Helper;

public class ColorHelper
{
	public static string ColorConditionSatisfied => Constants.Colors.HexStrings.TooltipHexColorConditionSatisfied;

	public static string GetColorConditionUnsatisfied(Enums.PlaceableRarity rarity)
	{
		return rarity switch
		{
			Enums.PlaceableRarity.Common => Constants.Colors.HexStrings.TooltipHexColorConditionUnsatisfiedRarityCommon, 
			Enums.PlaceableRarity.Uncommon => Constants.Colors.HexStrings.TooltipHexColorConditionUnsatisfiedRarityUncommon, 
			Enums.PlaceableRarity.Rare => Constants.Colors.HexStrings.TooltipHexColorConditionUnsatisfiedRarityRare, 
			Enums.PlaceableRarity.Epic => Constants.Colors.HexStrings.TooltipHexColorConditionUnsatisfiedRarityEpic, 
			Enums.PlaceableRarity.Legendary => Constants.Colors.HexStrings.TooltipHexColorConditionUnsatisfiedRarityLegendary, 
			Enums.PlaceableRarity.Mythic => Constants.Colors.HexStrings.TooltipHexColorConditionUnsatisfiedRarityMythic, 
			Enums.PlaceableRarity.Unique => Constants.Colors.HexStrings.TooltipHexColorConditionUnsatisfiedRarityUnique, 
			_ => string.Empty, 
		};
	}

	public static string GetColorStringForTooltip(Enums.TooltipValueDifference valueDifference)
	{
		return valueDifference switch
		{
			Enums.TooltipValueDifference.SameAsBase => Constants.Colors.HexStrings.TooltipHexStringSameAsBase, 
			Enums.TooltipValueDifference.HigherThenBase => Constants.Colors.HexStrings.TooltipHexStringHigherThenBase, 
			Enums.TooltipValueDifference.LowerThenBase => Constants.Colors.HexStrings.TooltipHexStringLowerThenBase, 
			_ => Constants.Colors.HexStrings.TooltipHexStringSameAsBase, 
		};
	}

	public static string GetColorStringForDamageType(Enums.DamageType damageType)
	{
		return damageType switch
		{
			Enums.DamageType.None => string.Empty, 
			Enums.DamageType.PhysicalDONOTUSE => Constants.Colors.HexStrings.DamageTypePhysical, 
			Enums.DamageType.Fire => Constants.Colors.HexStrings.DamageTypeFire, 
			Enums.DamageType.Cold => Constants.Colors.HexStrings.DamageTypeCold, 
			Enums.DamageType.Lightning => Constants.Colors.HexStrings.DamageTypeLightning, 
			Enums.DamageType.Void => Constants.Colors.HexStrings.DamageTypeVoid, 
			Enums.DamageType.Poison => Constants.Colors.HexStrings.DamageTypePoison, 
			Enums.DamageType.Energy => Constants.Colors.HexStrings.DamageTypeEnergy, 
			Enums.DamageType.Holy => Constants.Colors.HexStrings.DamageTypeHoly, 
			Enums.DamageType.Blunt => Constants.Colors.HexStrings.DamageTypeBlunt, 
			Enums.DamageType.Slashing => Constants.Colors.HexStrings.DamageTypeSlashing, 
			Enums.DamageType.Piercing => Constants.Colors.HexStrings.DamageTypePiercing, 
			Enums.DamageType.All => Constants.Colors.HexStrings.DamageTypePhysical, 
			_ => null, 
		};
	}

	public static string GetColorStringForDebuffType(Enums.Debuff.DebuffType debuffType)
	{
		return debuffType switch
		{
			Enums.Debuff.DebuffType.ArmorReduction => Constants.Colors.HexStrings.DamageTypeVoid, 
			Enums.Debuff.DebuffType.Stun => Constants.Colors.HexStrings.DamageTypeEnergy, 
			Enums.Debuff.DebuffType.Burn => Constants.Colors.HexStrings.DamageTypeFire, 
			Enums.Debuff.DebuffType.Poison => Constants.Colors.HexStrings.DamageTypePoison, 
			Enums.Debuff.DebuffType.Bleed => Constants.Colors.HexStrings.DamageTypePhysical, 
			Enums.Debuff.DebuffType.Slow => Constants.Colors.HexStrings.DamageTypeCold, 
			Enums.Debuff.DebuffType.Arcing => Constants.Colors.HexStrings.DamageTypeLightning, 
			_ => null, 
		};
	}

	public static Color GetColorDamageType(Enums.DamageType damageType)
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.DamageTypeColor.ContainsKey(damageType))
		{
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.DamageTypeColor[damageType];
		}
		return new Color(255f, 105f, 180f);
	}

	public static string GetColorHexcodeForRarity(Enums.PlaceableRarity rarity)
	{
		return rarity switch
		{
			Enums.PlaceableRarity.Common => "#FFFFFF", 
			Enums.PlaceableRarity.Uncommon => "#00FF02", 
			Enums.PlaceableRarity.Rare => "#4C70FF", 
			Enums.PlaceableRarity.Epic => "#E200FB", 
			Enums.PlaceableRarity.Legendary => "#BC3600", 
			Enums.PlaceableRarity.Mythic => "#FFCC00", 
			Enums.PlaceableRarity.Unique => "#00FFBF", 
			_ => "#000000", 
		};
	}

	public static string GetColorForWeaponType(Enums.WeaponType weaponType)
	{
		return weaponType switch
		{
			Enums.WeaponType.None => Constants.Colors.HexStrings.TooltipHexStringSameAsBase, 
			Enums.WeaponType.Melee => Constants.Colors.HexStrings.TooltipHexStringSameAsBase, 
			Enums.WeaponType.Ranged => Constants.Colors.HexStrings.TooltipHexStringSameAsBase, 
			_ => string.Empty, 
		};
	}

	public static Color GetColorForRarity(Enums.PlaceableRarity rarity)
	{
		return GameDatabaseHelper.GetRarityColor(rarity);
	}

	public static string GetColorHexcodeForRarity(Enums.PlaceableTag rarity)
	{
		return rarity switch
		{
			Enums.PlaceableTag.Common => "#FFFFFF", 
			Enums.PlaceableTag.Uncommon => "#00FF02", 
			Enums.PlaceableTag.Rare => "#4C70FF", 
			Enums.PlaceableTag.Epic => "#E200FB", 
			Enums.PlaceableTag.Legendary => "#BC3600", 
			Enums.PlaceableTag.Mythic => "#FFCC00", 
			_ => "000000", 
		};
	}

	internal static string GetColor(Enums.PlaceableTag foundTag)
	{
		if (foundTag <= Enums.PlaceableTag.Javelin)
		{
			if (foundTag <= Enums.PlaceableTag.Gloves)
			{
				if (foundTag <= Enums.PlaceableTag.Holy)
				{
					if (foundTag <= Enums.PlaceableTag.Void)
					{
						if ((ulong)foundTag <= 8uL)
						{
							switch (foundTag)
							{
							case Enums.PlaceableTag.None:
								return Constants.Colors.HexStrings.DefaultTextColor;
							case Enums.PlaceableTag.Physical:
								return Constants.Colors.HexStrings.DamageTypePhysical;
							case Enums.PlaceableTag.Fire:
								return Constants.Colors.HexStrings.DamageTypeFire;
							case Enums.PlaceableTag.Cold:
								return Constants.Colors.HexStrings.DamageTypeCold;
							case Enums.PlaceableTag.Lightning:
								return Constants.Colors.HexStrings.DamageTypeLightning;
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Fire:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Fire | Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Fire | Enums.PlaceableTag.Cold:
								goto IL_0467;
							}
						}
						if (foundTag == Enums.PlaceableTag.Void)
						{
							return Constants.Colors.HexStrings.DamageTypeVoid;
						}
					}
					else
					{
						switch (foundTag)
						{
						case Enums.PlaceableTag.Poison:
							return Constants.Colors.HexStrings.DamageTypePoison;
						case Enums.PlaceableTag.Energy:
							return Constants.Colors.HexStrings.DamageTypeEnergy;
						case Enums.PlaceableTag.Holy:
							return Constants.Colors.HexStrings.DamageTypeHoly;
						}
					}
				}
				else
				{
					switch (foundTag)
					{
					case Enums.PlaceableTag.Trinket:
						return Constants.Colors.HexStrings.DefaultItemType;
					case Enums.PlaceableTag.Shield:
						return Constants.Colors.HexStrings.DefaultItemType;
					case Enums.PlaceableTag.BodyArmor:
						return Constants.Colors.HexStrings.DefaultItemType;
					case Enums.PlaceableTag.LegArmor:
						return Constants.Colors.HexStrings.DefaultItemType;
					case Enums.PlaceableTag.Boots:
						return Constants.Colors.HexStrings.DefaultItemType;
					case Enums.PlaceableTag.Gloves:
						return Constants.Colors.HexStrings.DefaultItemType;
					}
				}
			}
			else
			{
				switch (foundTag)
				{
				case Enums.PlaceableTag.Amulet:
					return Constants.Colors.HexStrings.DefaultItemType;
				case Enums.PlaceableTag.Ring:
					return Constants.Colors.HexStrings.DefaultItemType;
				case Enums.PlaceableTag.Headwear:
					return Constants.Colors.HexStrings.DefaultItemType;
				case Enums.PlaceableTag.Sword:
					return Constants.Colors.HexStrings.DefaultWeaponType;
				case Enums.PlaceableTag.Hammer:
					return Constants.Colors.HexStrings.DefaultWeaponType;
				case Enums.PlaceableTag.Axe:
					return Constants.Colors.HexStrings.DefaultWeaponType;
				case Enums.PlaceableTag.FistWeapon:
					return Constants.Colors.HexStrings.DefaultWeaponType;
				case Enums.PlaceableTag.Halberd:
					return Constants.Colors.HexStrings.DefaultWeaponType;
				case Enums.PlaceableTag.Bow:
					return Constants.Colors.HexStrings.DefaultWeaponType;
				case Enums.PlaceableTag.Crossbow:
					return Constants.Colors.HexStrings.DefaultWeaponType;
				case Enums.PlaceableTag.Javelin:
					return Constants.Colors.HexStrings.DefaultWeaponType;
				}
			}
		}
		else
		{
			switch (foundTag)
			{
			case Enums.PlaceableTag.Throwing:
				return Constants.Colors.HexStrings.DefaultWeaponType;
			case Enums.PlaceableTag.Wand:
				return Constants.Colors.HexStrings.DefaultWeaponType;
			case Enums.PlaceableTag.Staff:
				return Constants.Colors.HexStrings.DefaultWeaponType;
			case Enums.PlaceableTag.Spellbook:
				return Constants.Colors.HexStrings.DefaultWeaponType;
			case Enums.PlaceableTag.Whip:
				return Constants.Colors.HexStrings.DefaultWeaponType;
			case Enums.PlaceableTag.Exotic:
				return Constants.Colors.HexStrings.DefaultWeaponType;
			case Enums.PlaceableTag.FireArm:
				return Constants.Colors.HexStrings.DefaultWeaponType;
			case Enums.PlaceableTag.Melee:
				return Constants.Colors.HexStrings.WeaponTypeMelee;
			case Enums.PlaceableTag.Ranged:
				return Constants.Colors.HexStrings.WeaponTypeRanged;
			case Enums.PlaceableTag.SpellDONOTUSE:
				return Constants.Colors.HexStrings.DefaultKeywordColor;
			case Enums.PlaceableTag.Common:
				return "#FFFFFF";
			case Enums.PlaceableTag.Uncommon:
				return "#00FF02";
			case Enums.PlaceableTag.Rare:
				return "#4C70FF";
			case Enums.PlaceableTag.Epic:
				return "#E200FB";
			case Enums.PlaceableTag.Legendary:
				return "#BC3600";
			case Enums.PlaceableTag.Mythic:
				return "#FFCC00";
			case Enums.PlaceableTag.Special:
				return "#00FFBF";
			case Enums.PlaceableTag.Unique:
				return Constants.Colors.HexStrings.DefaultTextColor;
			case Enums.PlaceableTag.Blunt:
				return Constants.Colors.HexStrings.DefaultTextColor;
			case Enums.PlaceableTag.Slashing:
				return Constants.Colors.HexStrings.DefaultTextColor;
			case Enums.PlaceableTag.Piercing:
				return Constants.Colors.HexStrings.DefaultTextColor;
			case Enums.PlaceableTag.MergeIngredient:
				return Constants.Colors.HexStrings.MergeIngredientColor;
			}
		}
		goto IL_0467;
		IL_0467:
		return Constants.Colors.HexStrings.DefaultTextColor;
	}
}
