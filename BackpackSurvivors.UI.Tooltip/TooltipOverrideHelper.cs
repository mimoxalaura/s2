using System.Linq;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipOverrideHelper
{
	public static string CreateLine(string description, bool conditionsAreSatisfied, Enums.PlaceableRarity rarity)
	{
		if (conditionsAreSatisfied)
		{
			string originalString = TextMeshProStringHelper.HighlightKeywords(description, "#FFF");
			originalString = TextMeshProStringHelper.HighlightElementalKeywords(originalString);
			return TextMeshProStringHelper.HighlightWeaponStatKeywords(originalString, "#FFF");
		}
		return "<color=" + ColorHelper.GetColorConditionUnsatisfied(rarity) + ">" + description + "</color>";
	}

	public static string CreateLine(Enums.DamageType sourceWeaponDamageType, Enums.DamageType targetWeaponDamageType, bool conditionsAreSatisfied, Enums.PlaceableRarity rarity)
	{
		_ = string.Empty;
		string empty = string.Empty;
		string empty2 = string.Empty;
		return CreateLine(string.Concat(str1: (sourceWeaponDamageType != Enums.DamageType.All) ? (empty + $"<sprite name=\"{StringHelper.GetSpriteValue(sourceWeaponDamageType)}\"> {sourceWeaponDamageType.GetUniqueFlags().Last()}") : "All", str3: (targetWeaponDamageType != Enums.DamageType.All) ? (empty2 + $"<sprite name=\"{StringHelper.GetSpriteValue(targetWeaponDamageType)}\"> {targetWeaponDamageType.GetUniqueFlags().Last()}") : "All", str0: StringHelper.TooltipPoint, str2: " damage is converted to "), conditionsAreSatisfied, rarity);
	}

	public static string CreateLine(string description)
	{
		string originalString = TextMeshProStringHelper.HighlightKeywords(description, "#FFF");
		originalString = TextMeshProStringHelper.HighlightElementalKeywords(originalString);
		originalString = TextMeshProStringHelper.HighlightWeaponStatKeywords(originalString, "#FFF");
		return StringHelper.TooltipPoint + " " + originalString;
	}

	public static string CreateLine(Enums.DamageType sourceWeaponDamageType, Enums.DamageType targetWeaponDamageType)
	{
		_ = string.Empty;
		string empty = string.Empty;
		string empty2 = string.Empty;
		return CreateLine(string.Concat(str1: (sourceWeaponDamageType != Enums.DamageType.All) ? (empty + $"<sprite name=\"{StringHelper.GetSpriteValue(sourceWeaponDamageType)}\"> {sourceWeaponDamageType.GetUniqueFlags().Last()}") : "All", str3: (targetWeaponDamageType != Enums.DamageType.All) ? (empty2 + $"<sprite name=\"{StringHelper.GetSpriteValue(targetWeaponDamageType)}\"> {targetWeaponDamageType.GetUniqueFlags().Last()}") : "All", str0: StringHelper.TooltipPoint, str2: " damage is converted to "));
	}
}
