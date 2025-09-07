using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Items.RuneEffects;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Items;

public class ItemInstance : BaseItemInstance
{
	private ItemSO _itemSO;

	public RuneSpecialEffect RuneSpecialEffect;

	private List<Condition> _starredWeaponConditions = new List<Condition>();

	public List<ItemStatModifier> ItemModifiers { get; private set; }

	public List<WeaponStatModifier> WeaponStatModifiersForStarredSlot { get; set; }

	public Dictionary<Enums.ItemStatType, float> CalculatedStats { get; private set; }

	public Dictionary<Enums.DamageType, float> CalculatedDamageTypeValues { get; private set; }

	public Dictionary<Enums.ItemStatType, float> WeaponFilterStats { get; private set; }

	public ItemSO ItemSO => _itemSO;

	public string Name => _itemSO.Name;

	public Enums.PlaceableRarity ItemRarity => _itemSO.ItemRarity;

	public string Description => _itemSO.Description;

	public Sprite Icon => _itemSO.Icon;

	public Enums.PlaceableType ItemType => _itemSO.ItemType;

	public Enums.PlaceableItemSubtype ItemSubtype => _itemSO.ItemSubtype;

	public Enums.PlaceableTag CombinedPlaceableTags => EnumHelper.GetCombinedPlaceableTags(this);

	public Dictionary<Enums.ItemStatType, float> GlobalStatValues { get; private set; }

	public Dictionary<Enums.DamageType, float> GlobalDamageTypeValues { get; private set; }

	public Dictionary<Enums.ItemStatType, FormulaSO> GlobalFormulaStatValues { get; private set; }

	public Dictionary<Enums.DamageType, FormulaSO> GlobalFormulaDamageTypeValues { get; private set; }

	public WeaponDamageTypeValueOverride[] GlobalDamageTypeValueOverrides => _itemSO.GlobalDamageTypeValueOverrides;

	public WeaponDamageCalculationOverride[] GlobalDamageCalculationOverrides => _itemSO.GlobalDamageCalculationOverrides;

	public WeaponAttackEffect[] GlobalAttackEffects => _itemSO.GlobalAttackEffects;

	public DebuffSO[] GlobalDebuffSOs => _itemSO.GlobalDebuffSOs;

	public Dictionary<Enums.ItemStatType, float> ConditionalStatValues { get; private set; }

	public Dictionary<Enums.DamageType, float> ConditionalDamageTypeValues { get; private set; }

	public Dictionary<Enums.ItemStatType, FormulaSO> ConditionalFormulaStatValues { get; private set; }

	public Dictionary<Enums.DamageType, FormulaSO> ConditionalFormulaDamageTypeValues { get; private set; }

	public WeaponDamageTypeValueOverride[] ConditionalWeaponDamageTypeValueOverrides => _itemSO.ConditionalWeaponDamageTypeValueOverrides;

	public WeaponDamageCalculationOverride[] ConditionalWeaponDamageCalculationOverrides => _itemSO.ConditionalWeaponDamageCalculationOverrides;

	public WeaponAttackEffect[] ConditionalWeaponAttackEffects => _itemSO.ConditionalWeaponFilterAttackEffects;

	public DebuffSO[] ConditionalDebuffSOs => _itemSO.ConditionalDebuffSOs;

	public ConditionSO[] ConditionalStatConditions => _itemSO.ConditionalStatConditions;

	public Dictionary<Enums.ItemStatType, float> StarStatValues { get; private set; }

	public Dictionary<Enums.DamageType, float> StarDamageTypeValues { get; private set; }

	public Dictionary<Enums.ItemStatType, FormulaSO> StarredFormulaStatValues { get; private set; }

	public Dictionary<Enums.DamageType, FormulaSO> StarredFormulaDamageTypeValues { get; private set; }

	public WeaponDamageTypeValueOverride[] StarWeaponDamageTypeValueOverrides => _itemSO.StarredWeaponDamageTypeValueOverrides;

	public WeaponDamageCalculationOverride[] StarWeaponDamageCalculationOverrides => _itemSO.StarredDamageCalculationOverrides;

	public WeaponAttackEffect[] StarWeaponAttackEffects => _itemSO.StarredWeaponAttackEffects;

	public DebuffSO[] StarDebuffSOs => _itemSO.StarredWeaponDebuffSOs;

	public ConditionSO[] StarWeaponConditions => _itemSO.StarredWeaponConditions;

	public ItemInstance(ItemSO itemSO)
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging)
		{
			ValidateItem(itemSO);
		}
		_itemSO = itemSO;
		SetBaseItemInstance(itemSO);
		CalculatedStats = StatCalculator.CreateItemStatDictionary();
		CalculatedDamageTypeValues = StatCalculator.CreateDamageTypeDictionary();
		ItemModifiers = new List<ItemStatModifier>();
		GlobalStatValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.GlobalStatValues);
		GlobalDamageTypeValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.GlobalDamageTypeValues);
		GlobalFormulaStatValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.GlobalStats.FormulaStats);
		GlobalFormulaDamageTypeValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.GlobalStats.FormulaDamageTypeValues);
		InitStarredWeaponStats();
		InitWeaponFilterStats();
		CalculateStats();
		CalculateDamageTypeValues();
	}

	private void InitWeaponFilterStats()
	{
		if (_itemSO.HasConditionalStats)
		{
			ConditionalStatValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.ConditionalStatValues);
			ConditionalDamageTypeValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.ConditionalDamageTypeValues);
			ConditionalFormulaStatValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.ConditionalStats.FormulaStats);
			ConditionalFormulaDamageTypeValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.ConditionalStats.FormulaDamageTypeValues);
		}
		else
		{
			ConditionalStatValues = new Dictionary<Enums.ItemStatType, float>();
			ConditionalDamageTypeValues = new Dictionary<Enums.DamageType, float>();
		}
	}

	private void InitStarredWeaponStats()
	{
		if (_itemSO.HasStarredWeaponStats)
		{
			StarStatValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.StarredWeaponStatValues);
			StarDamageTypeValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.StarredWeaponDamageTypeValues);
			StarredFormulaStatValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.StarredWeaponStats.FormulaStats);
			StarredFormulaDamageTypeValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_itemSO.StarredWeaponStats.FormulaDamageTypeValues);
			FillStarredWeaponConditions();
		}
		else
		{
			StarStatValues = new Dictionary<Enums.ItemStatType, float>();
			StarDamageTypeValues = new Dictionary<Enums.DamageType, float>();
		}
	}

	private void FillStarredWeaponConditions()
	{
		_starredWeaponConditions.Clear();
		ConditionSO[] starredWeaponConditions = _itemSO.StarredWeaponConditions;
		for (int i = 0; i < starredWeaponConditions.Length; i++)
		{
			Condition item = new Condition(starredWeaponConditions[i]);
			_starredWeaponConditions.Add(item);
		}
	}

	public void CalculateStats()
	{
		foreach (KeyValuePair<Enums.ItemStatType, float> globalStatValue in GlobalStatValues)
		{
			CalculatedStats[globalStatValue.Key] = globalStatValue.Value;
		}
	}

	public void SetCalculatedStat(Enums.ItemStatType stat, float statValue)
	{
		CalculatedStats[stat] = statValue;
	}

	public void CalculateDamageTypeValues()
	{
		foreach (KeyValuePair<Enums.DamageType, float> globalDamageTypeValue in GlobalDamageTypeValues)
		{
			CalculatedDamageTypeValues[globalDamageTypeValue.Key] = globalDamageTypeValue.Value;
		}
	}

	public void SetCalculatedDamageTypeValue(Enums.DamageType damageType, float statValue)
	{
		CalculatedDamageTypeValues[damageType] = statValue;
	}

	internal bool CanAffectWeaponInstance(WeaponInstance weaponInstance)
	{
		foreach (Condition starredWeaponCondition in _starredWeaponConditions)
		{
			Enums.WeaponType weaponType = starredWeaponCondition.WeaponType & weaponInstance.BaseWeaponType;
			if (starredWeaponCondition.WeaponType != Enums.WeaponType.None && weaponType == Enums.WeaponType.None)
			{
				return false;
			}
			Enums.DamageType damageType = starredWeaponCondition.DamageType & weaponInstance.DamageInstance.CalculatedDamageType;
			if (starredWeaponCondition.DamageType != Enums.DamageType.None && damageType == Enums.DamageType.None)
			{
				return false;
			}
			if (!starredWeaponCondition.IsConditionSatisfied(weaponInstance))
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object other)
	{
		if (other is ItemInstance)
		{
			return ((ItemInstance)other).Guid == base.Guid;
		}
		return base.Equals(other);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	private void ValidateItem(ItemSO itemSO)
	{
		ValidateGlobalStats(itemSO);
		ValidateWeaponFilterStats(itemSO);
	}

	private void ValidateWeaponFilterStats(ItemSO itemSO)
	{
		if (!(itemSO == null) && itemSO.HasConditionalStats && !itemSO.ConditionalStatConditions.Any())
		{
			Debug.LogWarning("Item " + itemSO.Name + " has NO conditions in the Conditional Stats - This should never happen (makes it a global stat)");
		}
	}

	private void ValidateGlobalStats(ItemSO itemSO)
	{
		if (!(itemSO == null))
		{
			if (itemSO.GlobalDamageTypeValueOverrides.Any())
			{
				Debug.LogWarning("Item " + itemSO.Name + " has damage type overrides in the Global Stats - This is most likely unwanted");
			}
			if (itemSO.GlobalAttackEffects.Any())
			{
				Debug.LogWarning("Item " + itemSO.Name + " has attack effects in the Global Stats - This is most likely unwanted");
			}
		}
	}
}
