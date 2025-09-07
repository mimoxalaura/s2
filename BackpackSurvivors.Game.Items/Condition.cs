using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Items;

public class Condition
{
	public Enums.ConditionalStats.ConditionTarget ConditionTarget;

	public Enums.ConditionalStats.ConditionCheckType ConditionCheckType;

	public float CheckAmount;

	public Enums.ConditionalStats.TypeToCheckAgainst TypeToCheckAgainst;

	public Enums.PlaceableTagType PlaceableTagType;

	public Enums.DamageType DamageTypeTag;

	public Enums.WeaponType WeaponTypeTag;

	public Enums.PlaceableWeaponSubtype WeaponSubtypeTag;

	public Enums.PlaceableItemSubtype ItemSubtypeTag;

	public Enums.ItemStatType ItemStatType;

	public Enums.PlaceableType PlaceableType;

	public Enums.PlaceableRarity PlaceableRarity;

	public Enums.DamageType DamageType;

	public Enums.WeaponType WeaponType;

	public Condition(ConditionSO conditionSO)
	{
		ConditionTarget = conditionSO.ConditionTarget;
		ConditionCheckType = conditionSO.ConditionCheckType;
		CheckAmount = conditionSO.CheckAmount;
		TypeToCheckAgainst = conditionSO.TypeToCheckAgainst;
		PlaceableTagType = conditionSO.PlaceableTagType;
		DamageTypeTag = conditionSO.DamageTypeTag;
		WeaponTypeTag = conditionSO.WeaponTypeTag;
		WeaponSubtypeTag = conditionSO.WeaponSubtypeTag;
		ItemSubtypeTag = conditionSO.ItemSubtypeTag;
		ItemStatType = conditionSO.ItemStatType;
		PlaceableType = conditionSO.PlaceableType;
		PlaceableRarity = conditionSO.PlaceableRarity;
		DamageType = conditionSO.DamageType;
		WeaponType = conditionSO.WeaponType;
	}

	public bool IsConditionSatisfied(WeaponInstance weaponInstance = null)
	{
		switch (ConditionTarget)
		{
		case Enums.ConditionalStats.ConditionTarget.Global:
			return CheckGlobalCondition();
		case Enums.ConditionalStats.ConditionTarget.Weapon:
			return CheckWeaponFitsCondition(weaponInstance);
		default:
			Debug.LogWarning(string.Format("ConditionTarget {0} is not handled in {1}.{2}", ConditionTarget, "Condition", "IsConditionSatisfied"));
			return false;
		}
	}

	private bool CheckWeaponFitsCondition(WeaponInstance weaponInstance)
	{
		if (weaponInstance == null)
		{
			return false;
		}
		bool flag = true;
		bool flag2 = true;
		if (WeaponType != Enums.WeaponType.None)
		{
			flag = (weaponInstance.BaseWeaponType & WeaponType) == weaponInstance.BaseWeaponType;
		}
		if (DamageType != Enums.DamageType.None)
		{
			flag2 = (weaponInstance.DamageInstance.CalculatedDamageType & DamageType) > Enums.DamageType.None;
		}
		return flag && flag2;
	}

	private bool CheckGlobalCondition()
	{
		switch (TypeToCheckAgainst)
		{
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
			return CheckTagCount();
		case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
			return CheckStatType();
		case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
			return CheckCoinAmount();
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
			return CheckPlaceableTypeCount();
		default:
			Debug.LogWarning(string.Format("TypeToCheckAgainst {0} is not handled in {1}.{2}", TypeToCheckAgainst, "Condition", "IsConditionSatisfied"));
			return false;
		}
	}

	internal float GetCurrentConditionAmount()
	{
		switch (TypeToCheckAgainst)
		{
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
			return GetTagCount();
		case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
			return GetStatTypeCount();
		case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
			return GetCoinCount();
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
			return GetPlaceableTypeCount();
		default:
			Debug.LogWarning(string.Format("TypeToCheckAgainst {0} is not handled in {1}.{2}", TypeToCheckAgainst, "Condition", "IsConditionSatisfied"));
			return 0f;
		}
	}

	internal string GetCheckAmountFormatted()
	{
		return GetAmountFormatted(CheckAmount);
	}

	internal string GetCurrentAmountFormatted()
	{
		float currentConditionAmount = GetCurrentConditionAmount();
		return GetAmountFormatted(currentConditionAmount);
	}

	private string GetAmountFormatted(float amount)
	{
		switch (TypeToCheckAgainst)
		{
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
		case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
			return amount.ToString();
		case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
			return StringHelper.GetCleanValue(amount, ItemStatType);
		default:
			Debug.LogWarning(string.Format("TypeToCheckAgainst {0} is not handled in {1}.{2}", TypeToCheckAgainst, "Condition", "GetAmountFormatted"));
			return amount.ToString();
		}
	}

	private bool CheckPlaceableRarityCount()
	{
		float placeableRarityCount = GetPlaceableRarityCount();
		return CheckConditionCheckType(placeableRarityCount);
	}

	private float GetPlaceableRarityCount()
	{
		if (SingletonController<BackpackController>.Instance == null || SingletonController<BackpackController>.Instance.CountController == null)
		{
			return 0f;
		}
		return SingletonController<BackpackController>.Instance.CountController.GetPlaceableRarityCount(PlaceableRarity);
	}

	private bool CheckPlaceableTypeCount()
	{
		float placeableTypeCount = GetPlaceableTypeCount();
		return CheckConditionCheckType(placeableTypeCount);
	}

	private float GetPlaceableTypeCount()
	{
		if (SingletonController<BackpackController>.Instance == null || SingletonController<BackpackController>.Instance.CountController == null)
		{
			return 0f;
		}
		return SingletonController<BackpackController>.Instance.CountController.GetPlaceableTypeCount(PlaceableType);
	}

	private bool CheckCoinAmount()
	{
		float coinCount = GetCoinCount();
		return CheckConditionCheckType(coinCount);
	}

	private float GetCoinCount()
	{
		if (SingletonController<CurrencyController>.Instance == null)
		{
			return 0f;
		}
		return SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.Coins);
	}

	private bool CheckStatType()
	{
		float statTypeCount = GetStatTypeCount();
		return CheckConditionCheckType(statTypeCount);
	}

	private float GetStatTypeCount()
	{
		if (SingletonController<GameController>.Instance == null || SingletonController<GameController>.Instance.Player == null)
		{
			return 0f;
		}
		return SingletonController<GameController>.Instance.Player.GetCalculatedStat(ItemStatType);
	}

	private bool CheckTagCount()
	{
		float tagCount = GetTagCount();
		return CheckConditionCheckType(tagCount);
	}

	private float GetTagCount()
	{
		Enums.PlaceableTag placeableTag = Enums.PlaceableTag.None;
		switch (PlaceableTagType)
		{
		case Enums.PlaceableTagType.DamageType:
			placeableTag = EnumHelper.DamageTypesToPlaceableTags(DamageTypeTag);
			break;
		case Enums.PlaceableTagType.WeaponType:
			placeableTag = EnumHelper.WeaponTypesToPlaceableTags(WeaponTypeTag);
			break;
		case Enums.PlaceableTagType.WeaponSubtype:
			placeableTag = EnumHelper.WeaponSubtypeToPlaceableTag(WeaponSubtypeTag);
			break;
		case Enums.PlaceableTagType.ItemSubtype:
			placeableTag = EnumHelper.ItemSubtypeToPlaceableTag(ItemSubtypeTag);
			break;
		case Enums.PlaceableTagType.Rarity:
			placeableTag = EnumHelper.RarityToPlaceableTag(PlaceableRarity);
			break;
		default:
			Debug.LogWarning(string.Format("PlaceableTagType {0} is not handled in {1}.{2}", PlaceableTagType, "Condition", "GetTagCount"));
			break;
		}
		if (SingletonController<BackpackController>.Instance == null || SingletonController<BackpackController>.Instance.CountController == null)
		{
			return 0f;
		}
		return SingletonController<BackpackController>.Instance.CountController.GetPlaceableTagCount(placeableTag);
	}

	private bool CheckConditionCheckType(float currentAmount)
	{
		switch (ConditionCheckType)
		{
		case Enums.ConditionalStats.ConditionCheckType.Minimum:
			return currentAmount >= CheckAmount;
		case Enums.ConditionalStats.ConditionCheckType.Maximum:
			return currentAmount <= CheckAmount;
		default:
			Debug.LogWarning(string.Format("ConditionCheckType {0} is not handled in {1}.{2}", ConditionCheckType, "Condition", "CheckConditionCheckType"));
			return false;
		}
	}

	internal string GetGeneratedDescription(bool isStarCondition)
	{
		string result = string.Empty;
		string text = "[NUMBER]";
		string text2 = ((ConditionCheckType == Enums.ConditionalStats.ConditionCheckType.Minimum) ? "Requires at least" : "Requires at most");
		switch (ConditionTarget)
		{
		case Enums.ConditionalStats.ConditionTarget.Global:
		{
			string placeableTagName = GetPlaceableTagName();
			switch (TypeToCheckAgainst)
			{
			case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
				result = text2 + " " + text + " " + placeableTagName;
				break;
			case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
				result = string.Format("{0} {1} {2}{3}", text2, text, PlaceableType, GetSingleOrMultipleString("", "s"));
				break;
			case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
				result = text2 + " " + text + " " + StringHelper.GetCleanString(ItemStatType);
				break;
			case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
				result = text2 + " " + text + " coins";
				break;
			default:
				Debug.LogWarning(string.Format("TypeToCheckAgainst {0} is not handled in {1}.{2}", TypeToCheckAgainst, "Condition", "GetGeneratedDescription"));
				break;
			}
			break;
		}
		case Enums.ConditionalStats.ConditionTarget.Weapon:
		{
			int num = WeaponType.GetUniqueFlags().Count();
			int num2 = Enum.GetNames(typeof(Enums.WeaponType)).Length - 2;
			int uniqueDamageTypeCount = DamageType.GetUniqueFlags().Count();
			int maximumDamageTypeCount = Enum.GetNames(typeof(Enums.DamageType)).Length - 2;
			string prefixString = (isStarCondition ? "" : "all");
			string suffixString = (isStarCondition ? "weapon" : "weapons");
			if (num == 0)
			{
				result = GenerateConditionDescriptionForAnyOrNoneWeaponType(result, uniqueDamageTypeCount, maximumDamageTypeCount, prefixString, suffixString);
			}
			else if (num == 1)
			{
				result = GenerateConditionDescriptionForSingleWeaponType(result, uniqueDamageTypeCount, maximumDamageTypeCount, prefixString, suffixString);
			}
			else if (num > 1 && num != num2)
			{
				result = GenerateConditionDescriptionForMultipleWeaponTypes(result, uniqueDamageTypeCount, maximumDamageTypeCount, prefixString, suffixString);
			}
			else if (num > 1 && num == num2)
			{
				result = GenerateConditionDescriptionForAllWeaponTypes(result, uniqueDamageTypeCount, maximumDamageTypeCount, prefixString, suffixString);
			}
			break;
		}
		}
		return result;
	}

	private string GenerateConditionDescriptionForAllWeaponTypes(string result, int uniqueDamageTypeCount, int maximumDamageTypeCount, string prefixString, string suffixString)
	{
		if (uniqueDamageTypeCount == 0)
		{
			result = "Affects " + prefixString + " weapons";
		}
		else if (uniqueDamageTypeCount == 1)
		{
			Enums.DamageType damageType = (Enums.DamageType)(object)DamageType.GetUniqueFlags().First();
			string colorStringForDamageType = ColorHelper.GetColorStringForDamageType(damageType);
			string fullSpriteValue = StringHelper.GetFullSpriteValue(damageType);
			string text = $"<color={colorStringForDamageType}>{fullSpriteValue} {damageType}</color>";
			result = "Affects " + prefixString + " " + text + " based weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount < maximumDamageTypeCount / 2 && uniqueDamageTypeCount != maximumDamageTypeCount)
		{
			string text2 = CreateCleanListOfDamageTypes(DamageType.ListFlags().ToList());
			result = "Affects " + prefixString + " " + text2 + " based weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount > maximumDamageTypeCount / 2 && uniqueDamageTypeCount != maximumDamageTypeCount)
		{
			List<Enums.DamageType> notIncludedEnums = GetNotIncludedEnums(DamageType.ListFlags().ToList());
			notIncludedEnums = RemoveUnusedDamageTypes(notIncludedEnums);
			string text3 = CreateCleanListOfDamageTypes(notIncludedEnums);
			result = "Affects " + prefixString + " except " + text3 + " based weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount == maximumDamageTypeCount)
		{
			result = "Affects " + prefixString + " " + suffixString;
		}
		return result;
	}

	private List<Enums.DamageType> RemoveUnusedDamageTypes(List<Enums.DamageType> notIncludedDamageTypes)
	{
		List<Enums.DamageType> second = new List<Enums.DamageType>
		{
			Enums.DamageType.All,
			Enums.DamageType.None,
			Enums.DamageType.PhysicalDONOTUSE
		};
		return notIncludedDamageTypes.Except(second).ToList();
	}

	private string GenerateConditionDescriptionForMultipleWeaponTypes(string result, int uniqueDamageTypeCount, int maximumDamageTypeCount, string prefixString, string suffixString)
	{
		Enums.WeaponType weaponType = Enums.WeaponType.None;
		weaponType = (from x in GetNotIncludedEnums(WeaponType.ListFlags().ToList())
			where x != Enums.WeaponType.None
			select x).FirstOrDefault();
		string text = string.Empty;
		if (weaponType != Enums.WeaponType.None)
		{
			string colorStringForTooltip = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase);
			text = $"<color={colorStringForTooltip}>{weaponType}</color>";
		}
		if (uniqueDamageTypeCount == 0)
		{
			result = "Affects " + prefixString + " weapons, except " + text + " weapons";
		}
		else if (uniqueDamageTypeCount == 1)
		{
			Enums.DamageType damageType = (Enums.DamageType)(object)DamageType.GetUniqueFlags().First();
			string colorStringForDamageType = ColorHelper.GetColorStringForDamageType(damageType);
			string fullSpriteValue = StringHelper.GetFullSpriteValue(damageType);
			string text2 = $"<color={colorStringForDamageType}>{fullSpriteValue} {damageType}</color>";
			result = "Affects " + prefixString + " " + text2 + " based weapons, except " + text + " weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount < maximumDamageTypeCount / 2 && uniqueDamageTypeCount != maximumDamageTypeCount)
		{
			string text3 = CreateCleanListOfDamageTypes(DamageType.ListFlags().ToList());
			result = "Affects " + prefixString + " " + text3 + " based weapons, except " + text + " weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount > maximumDamageTypeCount / 2 && uniqueDamageTypeCount != maximumDamageTypeCount)
		{
			List<Enums.DamageType> notIncludedEnums = GetNotIncludedEnums(DamageType.ListFlags().ToList());
			notIncludedEnums = RemoveUnusedDamageTypes(notIncludedEnums);
			string text4 = CreateCleanListOfDamageTypes(notIncludedEnums);
			result = "Affects " + prefixString + " except " + text4 + " based " + text + " weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount == maximumDamageTypeCount)
		{
			result = "Affects " + prefixString + " " + suffixString + " except " + text + " weapons";
		}
		return result;
	}

	private string GenerateConditionDescriptionForSingleWeaponType(string result, int uniqueDamageTypeCount, int maximumDamageTypeCount, string prefixString, string suffixString)
	{
		Enums.WeaponType weaponType = (Enums.WeaponType)(object)WeaponType.GetUniqueFlags().First();
		string colorForWeaponType = ColorHelper.GetColorForWeaponType(weaponType);
		string fullSpriteValue = StringHelper.GetFullSpriteValue(weaponType);
		string text = $"<color={colorForWeaponType}>{fullSpriteValue} {weaponType}</color>";
		if (uniqueDamageTypeCount == 0)
		{
			result = "Affects " + prefixString + " " + text + " weapons";
		}
		else if (uniqueDamageTypeCount == 1)
		{
			Enums.DamageType damageType = (Enums.DamageType)(object)DamageType.GetUniqueFlags().First();
			string colorStringForDamageType = ColorHelper.GetColorStringForDamageType(damageType);
			string fullSpriteValue2 = StringHelper.GetFullSpriteValue(damageType);
			string text2 = $"<color={colorStringForDamageType}>{fullSpriteValue2} {damageType}</color>";
			result = "Affects " + prefixString + " " + text2 + " based " + text + " weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount < maximumDamageTypeCount / 2 && uniqueDamageTypeCount != maximumDamageTypeCount)
		{
			string text3 = CreateCleanListOfDamageTypes(DamageType.ListFlags().ToList());
			result = "Affects " + prefixString + " " + text3 + " based " + text + " weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount > maximumDamageTypeCount / 2 && uniqueDamageTypeCount != maximumDamageTypeCount)
		{
			List<Enums.DamageType> notIncludedEnums = GetNotIncludedEnums(DamageType.ListFlags().ToList());
			notIncludedEnums = RemoveUnusedDamageTypes(notIncludedEnums);
			string text4 = CreateCleanListOfDamageTypes(notIncludedEnums);
			result = "Affects " + prefixString + " except " + text4 + " based " + text + " weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount == maximumDamageTypeCount)
		{
			result = "Affects " + prefixString + " " + text + " weapons";
		}
		return result;
	}

	private string GenerateConditionDescriptionForAnyOrNoneWeaponType(string result, int uniqueDamageTypeCount, int maximumDamageTypeCount, string prefixString, string suffixString)
	{
		if (uniqueDamageTypeCount == 0)
		{
			result = "Affects " + prefixString + " weapons";
		}
		else if (uniqueDamageTypeCount == 1)
		{
			Enums.DamageType damageType = (Enums.DamageType)(object)DamageType.GetUniqueFlags().First();
			string colorStringForDamageType = ColorHelper.GetColorStringForDamageType(damageType);
			string fullSpriteValue = StringHelper.GetFullSpriteValue(damageType);
			string text = $"<color={colorStringForDamageType}>{fullSpriteValue} {damageType}</color>";
			result = "Affects " + prefixString + " " + text + " based weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount < maximumDamageTypeCount / 2 && uniqueDamageTypeCount != maximumDamageTypeCount)
		{
			string text2 = CreateCleanListOfDamageTypes(DamageType.ListFlags().ToList());
			result = "Affects " + prefixString + " " + text2 + " based weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount > maximumDamageTypeCount / 2 && uniqueDamageTypeCount != maximumDamageTypeCount)
		{
			List<Enums.DamageType> notIncludedEnums = GetNotIncludedEnums(DamageType.ListFlags().ToList());
			notIncludedEnums = RemoveUnusedDamageTypes(notIncludedEnums);
			string text3 = CreateCleanListOfDamageTypes(notIncludedEnums);
			result = "Affects " + prefixString + " except " + text3 + " based weapons";
		}
		else if (uniqueDamageTypeCount > 1 && uniqueDamageTypeCount == maximumDamageTypeCount)
		{
			result = "Affects " + prefixString + " " + suffixString;
		}
		return result;
	}

	private List<T> GetNotIncludedEnums<T>(List<T> sourceList)
	{
		List<T> list = new List<T>();
		foreach (T value in Enum.GetValues(typeof(T)))
		{
			if (!sourceList.Contains(value))
			{
				list.Add(value);
			}
		}
		return list;
	}

	private string CreateCleanListOfDamageTypes(List<Enums.DamageType> damageTypes)
	{
		string text = string.Empty;
		if (damageTypes.Count() == 1)
		{
			text = $"<sprite name=\"{StringHelper.GetSpriteValue(damageTypes[0])}\"><color={ColorHelper.GetColorStringForDamageType(damageTypes[0])}>{damageTypes[0]}</color>";
		}
		else if (damageTypes.Count() == 2)
		{
			string text2 = $"<sprite name=\"{StringHelper.GetSpriteValue(damageTypes[0])}\"> <color={ColorHelper.GetColorStringForDamageType(damageTypes[0])}>{damageTypes[0]}</color>";
			string text3 = $"<sprite name=\"{StringHelper.GetSpriteValue(damageTypes[1])}\"> <color={ColorHelper.GetColorStringForDamageType(damageTypes[1])}>{damageTypes[1]}</color>";
			text = text2 + " and " + text3;
		}
		else
		{
			for (int i = 0; i < damageTypes.Count(); i++)
			{
				string text4 = $"<sprite name=\"{StringHelper.GetSpriteValue(damageTypes[i])}\"> <color={ColorHelper.GetColorStringForDamageType(damageTypes[i])}>{damageTypes[i]}</color>";
				text = ((i >= damageTypes.Count() - 2) ? ((i != damageTypes.Count() - 2) ? (text + text4) : (text + text4 + " and ")) : (text + text4 + ", "));
			}
		}
		return text;
	}

	private string GetPlaceableTagName()
	{
		string arg = ((CheckAmount > 1f) ? "tags" : "tag");
		return PlaceableTagType switch
		{
			Enums.PlaceableTagType.DamageType => $"{DamageTypeTag} {arg}", 
			Enums.PlaceableTagType.WeaponSubtype => $"{WeaponSubtypeTag} {arg}", 
			Enums.PlaceableTagType.ItemSubtype => $"{ItemSubtypeTag} {arg}", 
			Enums.PlaceableTagType.WeaponType => $"{WeaponTypeTag} {arg}", 
			Enums.PlaceableTagType.Rarity => $"{PlaceableRarity} {arg}", 
			_ => string.Empty, 
		};
	}

	private string GetSingleOrMultipleString(string single, string multiple)
	{
		if (CheckAmount != 1f)
		{
			return multiple;
		}
		return single;
	}
}
