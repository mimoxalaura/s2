using System;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.ScriptableObjects.Items;
using UnityEngine;

namespace BackpackSurvivors.System.Helper;

public class EnumHelper
{
	public static Enums.ItemStatTypeGroup GetItemStatType(Enums.ItemStatType itemStatType)
	{
		return itemStatType switch
		{
			Enums.ItemStatType.CritChancePercentage => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.CritMultiplier => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.Health => Enums.ItemStatTypeGroup.Defensive, 
			Enums.ItemStatType.HealthRegeneration => Enums.ItemStatTypeGroup.Defensive, 
			Enums.ItemStatType.DamagePercentage => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.SpeedPercentage => Enums.ItemStatTypeGroup.Utility, 
			Enums.ItemStatType.LuckPercentage => Enums.ItemStatTypeGroup.Utility, 
			Enums.ItemStatType.CooldownTime => Enums.ItemStatTypeGroup.Unused, 
			Enums.ItemStatType.DamageReductionPercentageDONOTUSE => Enums.ItemStatTypeGroup.Unused, 
			Enums.ItemStatType.Armor => Enums.ItemStatTypeGroup.Defensive, 
			Enums.ItemStatType.Spiked => Enums.ItemStatTypeGroup.Defensive, 
			Enums.ItemStatType.FlatDamage => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.PickupRadiusPercentage => Enums.ItemStatTypeGroup.Utility, 
			Enums.ItemStatType.EnemyCount => Enums.ItemStatTypeGroup.Utility, 
			Enums.ItemStatType.WeaponRange => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.Penetrating => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.ExplosionSizePercentage => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.LifeDrainPercentage => Enums.ItemStatTypeGroup.Defensive, 
			Enums.ItemStatType.ProjectileCount => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.ProjectileSpeed => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.ProjectileSizePercentage => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.StunChancePercentage => Enums.ItemStatTypeGroup.Defensive, 
			Enums.ItemStatType.ExtraCoinChancePercentage => Enums.ItemStatTypeGroup.Utility, 
			Enums.ItemStatType.CooldownReductionPercentage => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.ExtraDash => Enums.ItemStatTypeGroup.Utility, 
			Enums.ItemStatType.DodgePercentage => Enums.ItemStatTypeGroup.Defensive, 
			Enums.ItemStatType.ProjectileDuration => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.MaximumCompanionCount => Enums.ItemStatTypeGroup.Hidden, 
			Enums.ItemStatType.WeaponCapacity => Enums.ItemStatTypeGroup.Offensive, 
			Enums.ItemStatType.DamageAgainstNormalEnemies => Enums.ItemStatTypeGroup.Hidden, 
			Enums.ItemStatType.DamageAgainstEliteAndBossEnemies => Enums.ItemStatTypeGroup.Hidden, 
			Enums.ItemStatType.ExperienceGainedPercentage => Enums.ItemStatTypeGroup.Hidden, 
			Enums.ItemStatType.BuffDuration => Enums.ItemStatTypeGroup.Hidden, 
			Enums.ItemStatType.DebuffDuration => Enums.ItemStatTypeGroup.Hidden, 
			Enums.ItemStatType.Knockback => Enums.ItemStatTypeGroup.Hidden, 
			_ => Enums.ItemStatTypeGroup.Unused, 
		};
	}

	public static Enums.PlaceableTag DamageTypesToPlaceableTags(Enums.DamageType damageTypeTag)
	{
		Enums.PlaceableTag placeableTag = Enums.PlaceableTag.None;
		foreach (Enums.DamageType uniqueFlag in damageTypeTag.GetUniqueFlags())
		{
			Enums.PlaceableTag placeableTag2 = DamageTypeToPlaceableTag(uniqueFlag);
			placeableTag |= placeableTag2;
		}
		return placeableTag;
	}

	private static Enums.PlaceableTag DamageTypeToPlaceableTag(Enums.DamageType damageTypeTag)
	{
		Enums.PlaceableTag placeableTag = Enums.PlaceableTag.None;
		foreach (Enums.DamageType value in Enum.GetValues(typeof(Enums.DamageType)))
		{
			if ((value & damageTypeTag) == value)
			{
				switch (value)
				{
				case Enums.DamageType.PhysicalDONOTUSE:
					placeableTag |= Enums.PlaceableTag.Physical;
					break;
				case Enums.DamageType.Fire:
					placeableTag |= Enums.PlaceableTag.Fire;
					break;
				case Enums.DamageType.Cold:
					placeableTag |= Enums.PlaceableTag.Cold;
					break;
				case Enums.DamageType.Poison:
					placeableTag |= Enums.PlaceableTag.Poison;
					break;
				case Enums.DamageType.Energy:
					placeableTag |= Enums.PlaceableTag.Energy;
					break;
				case Enums.DamageType.Lightning:
					placeableTag |= Enums.PlaceableTag.Lightning;
					break;
				case Enums.DamageType.Void:
					placeableTag |= Enums.PlaceableTag.Void;
					break;
				case Enums.DamageType.Holy:
					placeableTag |= Enums.PlaceableTag.Holy;
					break;
				case Enums.DamageType.Blunt:
					placeableTag |= Enums.PlaceableTag.Blunt;
					break;
				case Enums.DamageType.Slashing:
					placeableTag |= Enums.PlaceableTag.Slashing;
					break;
				case Enums.DamageType.Piercing:
					placeableTag |= Enums.PlaceableTag.Piercing;
					break;
				default:
					Debug.LogWarning(string.Format("Damagetype {0} is not handled in {1}.{2}()", value, "EnumHelper", "DamageTypeToPlaceableTag"));
					break;
				case Enums.DamageType.None:
					break;
				}
			}
		}
		return placeableTag;
	}

	internal static Enums.PlaceableTag ItemSubtypeToPlaceableTag(Enums.PlaceableItemSubtype itemSubtypeTag)
	{
		switch (itemSubtypeTag)
		{
		case Enums.PlaceableItemSubtype.None:
			return Enums.PlaceableTag.None;
		case Enums.PlaceableItemSubtype.Trinket:
			return Enums.PlaceableTag.Trinket;
		case Enums.PlaceableItemSubtype.Shield:
			return Enums.PlaceableTag.Shield;
		case Enums.PlaceableItemSubtype.BodyArmor:
			return Enums.PlaceableTag.BodyArmor;
		case Enums.PlaceableItemSubtype.LegArmor:
			return Enums.PlaceableTag.LegArmor;
		case Enums.PlaceableItemSubtype.Boots:
			return Enums.PlaceableTag.Boots;
		case Enums.PlaceableItemSubtype.Gloves:
			return Enums.PlaceableTag.Gloves;
		case Enums.PlaceableItemSubtype.Amulet:
			return Enums.PlaceableTag.Amulet;
		case Enums.PlaceableItemSubtype.Ring:
			return Enums.PlaceableTag.Ring;
		case Enums.PlaceableItemSubtype.Headwear:
			return Enums.PlaceableTag.Headwear;
		case Enums.PlaceableItemSubtype.Special:
			return Enums.PlaceableTag.Special;
		default:
			Debug.LogWarning(string.Format("ItemSubtype {0} is not handled in {1}.{2}()", itemSubtypeTag, "EnumHelper", "ItemSubtypeToPlaceableTag"));
			return Enums.PlaceableTag.None;
		}
	}

	internal static Enums.PlaceableTag WeaponSubtypeToPlaceableTag(Enums.PlaceableWeaponSubtype weaponSubtypeTag)
	{
		switch (weaponSubtypeTag)
		{
		case Enums.PlaceableWeaponSubtype.None:
			return Enums.PlaceableTag.None;
		case Enums.PlaceableWeaponSubtype.Sword:
			return Enums.PlaceableTag.Sword;
		case Enums.PlaceableWeaponSubtype.Hammer:
			return Enums.PlaceableTag.Hammer;
		case Enums.PlaceableWeaponSubtype.Axe:
			return Enums.PlaceableTag.Axe;
		case Enums.PlaceableWeaponSubtype.FistWeapon:
			return Enums.PlaceableTag.FistWeapon;
		case Enums.PlaceableWeaponSubtype.Halberd:
			return Enums.PlaceableTag.Halberd;
		case Enums.PlaceableWeaponSubtype.Bow:
			return Enums.PlaceableTag.Bow;
		case Enums.PlaceableWeaponSubtype.Crossbow:
			return Enums.PlaceableTag.Crossbow;
		case Enums.PlaceableWeaponSubtype.Javelin:
			return Enums.PlaceableTag.Javelin;
		case Enums.PlaceableWeaponSubtype.Throwing:
			return Enums.PlaceableTag.Throwing;
		case Enums.PlaceableWeaponSubtype.Wand:
			return Enums.PlaceableTag.Wand;
		case Enums.PlaceableWeaponSubtype.Staff:
			return Enums.PlaceableTag.Staff;
		case Enums.PlaceableWeaponSubtype.Spellbook:
			return Enums.PlaceableTag.Spellbook;
		case Enums.PlaceableWeaponSubtype.Whip:
			return Enums.PlaceableTag.Whip;
		case Enums.PlaceableWeaponSubtype.Exotic:
			return Enums.PlaceableTag.Exotic;
		case Enums.PlaceableWeaponSubtype.Firearm:
			return Enums.PlaceableTag.FireArm;
		default:
			Debug.LogWarning(string.Format("ItemSubtype {0} is not handled in {1}.{2}()", weaponSubtypeTag, "EnumHelper", "WeaponSubtypeToPlaceableTag"));
			return Enums.PlaceableTag.None;
		}
	}

	internal static Enums.PlaceableTag WeaponTypesToPlaceableTags(Enums.WeaponType weaponTypeTag)
	{
		Enums.PlaceableTag placeableTag = Enums.PlaceableTag.None;
		foreach (Enums.WeaponType uniqueFlag in weaponTypeTag.GetUniqueFlags())
		{
			Enums.PlaceableTag placeableTag2 = WeaponTypeToPlaceableTag(uniqueFlag);
			placeableTag |= placeableTag2;
		}
		return placeableTag;
	}

	private static Enums.PlaceableTag WeaponTypeToPlaceableTag(Enums.WeaponType weaponTypeTag)
	{
		Enums.PlaceableTag placeableTag = Enums.PlaceableTag.None;
		foreach (Enums.WeaponType value in Enum.GetValues(typeof(Enums.WeaponType)))
		{
			if ((value & weaponTypeTag) == value)
			{
				switch (weaponTypeTag)
				{
				case Enums.WeaponType.Melee:
					placeableTag |= Enums.PlaceableTag.Melee;
					break;
				case Enums.WeaponType.Ranged:
					placeableTag |= Enums.PlaceableTag.Ranged;
					break;
				default:
					Debug.LogWarning(string.Format("WeaponType {0} is not handled in {1}.{2}()", weaponTypeTag, "EnumHelper", "WeaponTypeToPlaceableTag"));
					return Enums.PlaceableTag.None;
				case Enums.WeaponType.None:
					break;
				}
			}
		}
		return placeableTag;
	}

	internal static Enums.PlaceableTag RarityToPlaceableTag(Enums.PlaceableRarity rarityTag)
	{
		Enums.PlaceableTag placeableTag = Enums.PlaceableTag.None;
		foreach (Enums.PlaceableRarity value in Enum.GetValues(typeof(Enums.PlaceableRarity)))
		{
			if ((value & rarityTag) == value)
			{
				switch (rarityTag)
				{
				case Enums.PlaceableRarity.Common:
					placeableTag |= Enums.PlaceableTag.Common;
					break;
				case Enums.PlaceableRarity.Uncommon:
					placeableTag |= Enums.PlaceableTag.Uncommon;
					break;
				case Enums.PlaceableRarity.Rare:
					placeableTag |= Enums.PlaceableTag.Rare;
					break;
				case Enums.PlaceableRarity.Epic:
					placeableTag |= Enums.PlaceableTag.Epic;
					break;
				case Enums.PlaceableRarity.Legendary:
					placeableTag |= Enums.PlaceableTag.Legendary;
					break;
				case Enums.PlaceableRarity.Mythic:
					placeableTag |= Enums.PlaceableTag.Mythic;
					break;
				case Enums.PlaceableRarity.Unique:
					placeableTag |= Enums.PlaceableTag.Unique;
					break;
				default:
					Debug.LogWarning(string.Format("Rarity {0} is not handled in {1}.{2}()", rarityTag, "EnumHelper", "RarityToPlaceableTag"));
					return Enums.PlaceableTag.None;
				}
			}
		}
		return placeableTag;
	}

	internal static Enums.PlaceableTag GetCombinedPlaceableTags(Enums.PlaceableWeaponSubtype? placeableWeaponSubtype, Enums.PlaceableItemSubtype? placeableItemSubtype, Enums.DamageType? damageType, Enums.PlaceableRarity? rarity, Enums.WeaponType? weaponType, bool isUsedInFormula = false)
	{
		Enums.PlaceableTag placeableTag = Enums.PlaceableTag.None;
		if (placeableWeaponSubtype.HasValue)
		{
			Enums.PlaceableTag placeableTag2 = WeaponSubtypeToPlaceableTag(placeableWeaponSubtype.Value);
			placeableTag |= placeableTag2;
		}
		if (placeableItemSubtype.HasValue)
		{
			Enums.PlaceableTag placeableTag3 = ItemSubtypeToPlaceableTag(placeableItemSubtype.Value);
			placeableTag |= placeableTag3;
		}
		if (damageType.HasValue)
		{
			Enums.PlaceableTag placeableTag4 = DamageTypesToPlaceableTags(damageType.Value);
			placeableTag |= placeableTag4;
		}
		if (rarity.HasValue)
		{
			Enums.PlaceableTag placeableTag5 = RarityToPlaceableTag(rarity.Value);
			placeableTag |= placeableTag5;
		}
		if (weaponType.HasValue)
		{
			Enums.PlaceableTag placeableTag6 = WeaponTypesToPlaceableTags(weaponType.Value);
			placeableTag |= placeableTag6;
		}
		if (isUsedInFormula)
		{
			placeableTag |= Enums.PlaceableTag.MergeIngredient;
		}
		return placeableTag;
	}

	internal static Enums.PlaceableTag GetCombinedPlaceableTags(Enums.PlaceableItemSubtype placeableItemSubtype)
	{
		return ItemSubtypeToPlaceableTag(placeableItemSubtype);
	}

	internal static Enums.PlaceableTag GetCombinedPlaceableTags(WeaponSO weaponSO)
	{
		return GetCombinedPlaceableTags(weaponSO.WeaponSubtype, null, weaponSO.Damage.BaseDamageType, weaponSO.ItemRarity, weaponSO.WeaponType, SingletonController<MergeController>.Instance.BaseItemSOIsUsedInAnyMerge(weaponSO));
	}

	internal static Enums.PlaceableTag GetCombinedPlaceableTags(ItemSO itemSO)
	{
		return GetCombinedPlaceableTags(null, itemSO.ItemSubtype, itemSO.DamageType, itemSO.ItemRarity, itemSO.WeaponType, SingletonController<MergeController>.Instance.BaseItemSOIsUsedInAnyMerge(itemSO));
	}

	internal static Enums.PlaceableTag GetCombinedPlaceableTags(WeaponInstance weaponInstance)
	{
		return GetCombinedPlaceableTags(weaponInstance.BaseWeaponSO.WeaponSubtype, null, weaponInstance.DamageInstance.CalculatedDamageType, weaponInstance.ItemRarity, weaponInstance.BaseWeaponSO.WeaponType, SingletonController<MergeController>.Instance.BaseItemSOIsUsedInAnyMerge(weaponInstance.BaseItemSO));
	}

	internal static Enums.PlaceableTag GetCombinedPlaceableTags(ItemInstance itemInstance)
	{
		return GetCombinedPlaceableTags(null, itemInstance.ItemSO.ItemSubtype, itemInstance.ItemSO.DamageType, itemInstance.ItemSO.ItemRarity, itemInstance.ItemSO.WeaponType, SingletonController<MergeController>.Instance.BaseItemSOIsUsedInAnyMerge(itemInstance.BaseItemSO));
	}

	internal static Enums.PlaceableType DraggableTypeToPlaceableType(Enums.Backpack.DraggableType draggableType)
	{
		return draggableType switch
		{
			Enums.Backpack.DraggableType.Bag => Enums.PlaceableType.Bag, 
			Enums.Backpack.DraggableType.Item => Enums.PlaceableType.Item, 
			Enums.Backpack.DraggableType.Weapon => Enums.PlaceableType.Weapon, 
			_ => Enums.PlaceableType.Weapon, 
		};
	}

	internal static Enums.Backpack.DraggableType PlaceableTypeTypeToDraggableType(Enums.PlaceableType placeableType)
	{
		return placeableType switch
		{
			Enums.PlaceableType.Bag => Enums.Backpack.DraggableType.Bag, 
			Enums.PlaceableType.Item => Enums.Backpack.DraggableType.Item, 
			Enums.PlaceableType.Weapon => Enums.Backpack.DraggableType.Weapon, 
			_ => Enums.Backpack.DraggableType.Weapon, 
		};
	}
}
