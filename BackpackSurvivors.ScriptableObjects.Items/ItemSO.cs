using System;
using BackpackSurvivors.Combat.ScriptableObjects;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Items.RuneEffects;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BackpackSurvivors.ScriptableObjects.Items;

[CreateAssetMenu(fileName = "Item", menuName = "Game/Items/Item", order = 2)]
public class ItemSO : BaseItemSO
{
	public string _ItemSubtype;

	[SerializeField]
	public ItemStatsSO GlobalStats;

	[SerializeField]
	public ItemStatsSO ConditionalStats;

	[SerializeField]
	public ItemStatsSO StarredWeaponStats;

	[SerializeField]
	public Enums.DamageType DamageType;

	[SerializeField]
	public Enums.WeaponType WeaponType;

	[SerializeField]
	public Enums.PlaceableWeaponSubtype PlaceableWeaponSubtype;

	[SerializeField]
	public Enums.PlaceableItemSubtype ItemSubtype;

	[SerializeField]
	public bool KeepHeadOnDisplay;

	[SerializeField]
	public SerializableDictionaryBase<Enums.CharacterClass, Sprite> IngameImagesPerCharacter;

	[SerializeField]
	public bool HasSpecialEffect;

	[SerializeField]
	public RuneSpecialEffect RuneSpecialEffect;

	[SerializeField]
	[FormerlySerializedAs("Material")]
	public SerializableDictionaryBase<Enums.CharacterClass, Material> BodyMaterials;

	internal bool HasGlobalStats => GlobalStats != null;

	internal SerializableDictionaryBase<Enums.ItemStatType, float> GlobalStatValues
	{
		get
		{
			if (!HasGlobalStats)
			{
				return new SerializableDictionaryBase<Enums.ItemStatType, float>();
			}
			return GlobalStats.StatValues;
		}
	}

	internal SerializableDictionaryBase<Enums.DamageType, float> GlobalDamageTypeValues
	{
		get
		{
			if (!HasGlobalStats)
			{
				return new SerializableDictionaryBase<Enums.DamageType, float>();
			}
			return GlobalStats.DamageTypeValues;
		}
	}

	internal WeaponDamageTypeValueOverride[] GlobalDamageTypeValueOverrides
	{
		get
		{
			if (!HasGlobalStats)
			{
				return new WeaponDamageTypeValueOverride[0];
			}
			return GlobalStats.WeaponDamageTypeValueOverrides;
		}
	}

	internal WeaponDamageCalculationOverride[] GlobalDamageCalculationOverrides
	{
		get
		{
			if (!HasGlobalStats)
			{
				return new WeaponDamageCalculationOverride[0];
			}
			return GlobalStats.WeaponDamageCalculationOverrides;
		}
	}

	internal WeaponAttackEffect[] GlobalAttackEffects
	{
		get
		{
			if (!HasGlobalStats)
			{
				return new WeaponAttackEffect[0];
			}
			return GlobalStats.WeaponAttackEffects;
		}
	}

	internal DebuffSO[] GlobalDebuffSOs
	{
		get
		{
			if (!HasGlobalStats)
			{
				return new DebuffSO[0];
			}
			return GlobalStats.DebuffSOs;
		}
	}

	internal bool HasConditionalStats => ConditionalStats != null;

	internal SerializableDictionaryBase<Enums.ItemStatType, float> ConditionalStatValues
	{
		get
		{
			if (!HasConditionalStats)
			{
				return new SerializableDictionaryBase<Enums.ItemStatType, float>();
			}
			return ConditionalStats.StatValues;
		}
	}

	internal SerializableDictionaryBase<Enums.DamageType, float> ConditionalDamageTypeValues
	{
		get
		{
			if (!HasConditionalStats)
			{
				return new SerializableDictionaryBase<Enums.DamageType, float>();
			}
			return ConditionalStats.DamageTypeValues;
		}
	}

	internal WeaponDamageTypeValueOverride[] ConditionalWeaponDamageTypeValueOverrides
	{
		get
		{
			if (!HasConditionalStats)
			{
				return new WeaponDamageTypeValueOverride[0];
			}
			return ConditionalStats.WeaponDamageTypeValueOverrides;
		}
	}

	internal WeaponDamageCalculationOverride[] ConditionalWeaponDamageCalculationOverrides
	{
		get
		{
			if (!HasConditionalStats)
			{
				return new WeaponDamageCalculationOverride[0];
			}
			return ConditionalStats.WeaponDamageCalculationOverrides;
		}
	}

	internal WeaponAttackEffect[] ConditionalWeaponFilterAttackEffects
	{
		get
		{
			if (!HasConditionalStats)
			{
				return new WeaponAttackEffect[0];
			}
			return ConditionalStats.WeaponAttackEffects;
		}
	}

	internal DebuffSO[] ConditionalDebuffSOs
	{
		get
		{
			if (!HasConditionalStats)
			{
				return new DebuffSO[0];
			}
			return ConditionalStats.DebuffSOs;
		}
	}

	internal ConditionSO[] ConditionalStatConditions
	{
		get
		{
			if (!HasConditionalStats)
			{
				return new ConditionSO[0];
			}
			return ConditionalStats.Conditions;
		}
	}

	internal bool HasStarredWeaponStats => StarredWeaponStats != null;

	internal SerializableDictionaryBase<Enums.ItemStatType, float> StarredWeaponStatValues
	{
		get
		{
			if (!HasStarredWeaponStats)
			{
				return new SerializableDictionaryBase<Enums.ItemStatType, float>();
			}
			return StarredWeaponStats.StatValues;
		}
	}

	internal SerializableDictionaryBase<Enums.DamageType, float> StarredWeaponDamageTypeValues
	{
		get
		{
			if (!HasStarredWeaponStats)
			{
				return new SerializableDictionaryBase<Enums.DamageType, float>();
			}
			return StarredWeaponStats.DamageTypeValues;
		}
	}

	internal WeaponDamageTypeValueOverride[] StarredWeaponDamageTypeValueOverrides
	{
		get
		{
			if (!HasStarredWeaponStats)
			{
				return new WeaponDamageTypeValueOverride[0];
			}
			return StarredWeaponStats.WeaponDamageTypeValueOverrides;
		}
	}

	internal WeaponDamageCalculationOverride[] StarredDamageCalculationOverrides
	{
		get
		{
			if (!HasStarredWeaponStats)
			{
				return new WeaponDamageCalculationOverride[0];
			}
			return StarredWeaponStats.WeaponDamageCalculationOverrides;
		}
	}

	internal WeaponAttackEffect[] StarredWeaponAttackEffects
	{
		get
		{
			if (!HasStarredWeaponStats)
			{
				return new WeaponAttackEffect[0];
			}
			return StarredWeaponStats.WeaponAttackEffects;
		}
	}

	internal DebuffSO[] StarredWeaponDebuffSOs
	{
		get
		{
			if (!HasStarredWeaponStats)
			{
				return new DebuffSO[0];
			}
			return StarredWeaponStats.DebuffSOs;
		}
	}

	internal ConditionSO[] StarredWeaponConditions
	{
		get
		{
			if (!HasStarredWeaponStats)
			{
				return new ConditionSO[0];
			}
			return StarredWeaponStats.Conditions;
		}
	}

	internal bool HasStarStats()
	{
		return StarredWeaponStats != null;
	}

	public override bool Equals(object other)
	{
		if (other is ItemSO itemSO)
		{
			return Name.Equals(itemSO.Name);
		}
		return base.Equals(other);
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add(base.GetHashCode());
		hashCode.Add(Id);
		hashCode.Add(Name);
		hashCode.Add(Description);
		hashCode.Add(BuyingPrice);
		hashCode.Add(SellingPrice);
		hashCode.Add(ItemRarity);
		hashCode.Add(ItemType);
		hashCode.Add(ItemSize);
		hashCode.Add(StarringEffectIsPositive);
		hashCode.Add(AvailableInShop);
		hashCode.Add(Icon);
		hashCode.Add(IngameImage);
		hashCode.Add(BackpackImage);
		hashCode.Add(BodyMaterials);
		hashCode.Add(BeginDragAudio);
		hashCode.Add(EndDropSuccesAudio);
		hashCode.Add(EndDropFailedAudio);
		hashCode.Add(GlobalStats);
		hashCode.Add(ConditionalStats);
		hashCode.Add(StarredWeaponStats);
		hashCode.Add(DamageType);
		hashCode.Add(WeaponType);
		hashCode.Add(PlaceableWeaponSubtype);
		hashCode.Add(ItemSubtype);
		hashCode.Add(KeepHeadOnDisplay);
		hashCode.Add(IngameImagesPerCharacter);
		hashCode.Add(HasGlobalStats);
		hashCode.Add(GlobalStatValues);
		hashCode.Add(GlobalDamageTypeValues);
		hashCode.Add(GlobalDamageTypeValueOverrides);
		hashCode.Add(GlobalDamageCalculationOverrides);
		hashCode.Add(GlobalAttackEffects);
		hashCode.Add(GlobalDebuffSOs);
		hashCode.Add(HasConditionalStats);
		hashCode.Add(ConditionalStatValues);
		hashCode.Add(ConditionalDamageTypeValues);
		hashCode.Add(ConditionalWeaponDamageTypeValueOverrides);
		hashCode.Add(ConditionalWeaponDamageCalculationOverrides);
		hashCode.Add(ConditionalWeaponFilterAttackEffects);
		hashCode.Add(ConditionalDebuffSOs);
		hashCode.Add(ConditionalStatConditions);
		hashCode.Add(HasStarredWeaponStats);
		hashCode.Add(StarredWeaponStatValues);
		hashCode.Add(StarredWeaponDamageTypeValues);
		hashCode.Add(StarredWeaponDamageTypeValueOverrides);
		hashCode.Add(StarredDamageCalculationOverrides);
		hashCode.Add(StarredWeaponAttackEffects);
		hashCode.Add(StarredWeaponDebuffSOs);
		hashCode.Add(StarredWeaponConditions);
		return hashCode.ToHashCode();
	}

	private void CreateData()
	{
		if (Icon != null)
		{
			_PreviewIcon = Icon.texture;
		}
		_Id = Id.ToString();
		_Title = Name;
		_Rarity = ItemRarity.ToString();
		_ItemSubtype = ItemSubtype.ToString();
	}
}
