using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipFormulaHelper
{
	public static string CreateLine(Enums.ItemStatType itemStatType, float baseValue, float calculatedValue, FormulaSO formula, Enums.PlaceableRarity rarity, bool conditionsAreSatisfied = true)
	{
		string text;
		if (calculatedValue >= 0f)
		{
			float num = calculatedValue - baseValue;
			Enums.TooltipValueDifference valueDifference = ((num != 0f) ? ((num > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
			text = (conditionsAreSatisfied ? ColorHelper.GetColorStringForTooltip(valueDifference) : ColorHelper.GetColorConditionUnsatisfied(rarity));
		}
		else
		{
			float num = calculatedValue;
			Enums.TooltipValueDifference valueDifference2 = ((num != 0f) ? ((num > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
			text = (conditionsAreSatisfied ? ColorHelper.GetColorStringForTooltip(valueDifference2) : ColorHelper.GetColorConditionUnsatisfied(rarity));
		}
		string cleanValue = StringHelper.GetCleanValue(calculatedValue, itemStatType, isModifier: true);
		string cleanString = StringHelper.GetCleanString(itemStatType);
		string text2 = "<sprite name=\"" + StringHelper.GetSpriteValue(itemStatType) + "\">";
		string text3 = (conditionsAreSatisfied ? ColorHelper.ColorConditionSatisfied : ColorHelper.GetColorConditionUnsatisfied(rarity));
		string text4 = "<color=" + text3 + "><sprite name=\"" + StringHelper.GetFormulaTypeToCheckAgainstIcon(formula) + "\"></color>";
		string text5 = "<color=" + text3 + "><sprite name=\"" + StringHelper.GetSpriteValue(itemStatType) + "\"></color>";
		string text6 = ((!formula.EffectIsPositive) ? "FormulaTransformNegative" : "FormulaTransform");
		string text7 = text4 + " <sprite name=\"" + text6 + "\"> " + text5;
		string text8 = "";
		string text9 = "<color=" + text3 + "></color>";
		string text10 = "<color=" + text + ">" + cleanValue + "</color>";
		string text11 = " " + text2 + " <color=" + text3 + ">" + cleanString + "</color>  ";
		string text12 = "[ " + text7 + " ]";
		string text13 = text8 + text9 + text10 + text11 + text12;
		return StringHelper.TooltipPoint + text13;
	}

	public static string CreateLine(Enums.DamageType damageType, float baseValue, float calculatedValue, FormulaSO formula, Enums.PlaceableRarity rarity, bool conditionsAreSatisfied = true)
	{
		string text;
		if (calculatedValue >= 0f)
		{
			float num = calculatedValue - baseValue;
			Enums.TooltipValueDifference valueDifference = ((num != 0f) ? ((num > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
			text = (conditionsAreSatisfied ? ColorHelper.GetColorStringForTooltip(valueDifference) : ColorHelper.GetColorConditionUnsatisfied(rarity));
		}
		else
		{
			float num = calculatedValue;
			Enums.TooltipValueDifference valueDifference2 = ((num != 0f) ? ((num > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
			text = (conditionsAreSatisfied ? ColorHelper.GetColorStringForTooltip(valueDifference2) : ColorHelper.GetColorConditionUnsatisfied(rarity));
		}
		string cleanValue = StringHelper.GetCleanValue(calculatedValue, damageType);
		string cleanString = StringHelper.GetCleanString(damageType);
		string text2 = "<sprite name=\"" + StringHelper.GetSpriteValue(damageType) + "\">";
		string text3 = ((calculatedValue > 0f) ? "+" : "");
		string text4 = (conditionsAreSatisfied ? ColorHelper.ColorConditionSatisfied : ColorHelper.GetColorConditionUnsatisfied(rarity));
		string text5 = "<color=" + text4 + "><sprite name=\"" + StringHelper.GetFormulaTypeToCheckAgainstIcon(formula) + "\"></color>";
		string text6 = "<color=" + text4 + "><sprite name=\"" + StringHelper.GetSpriteValue(damageType) + "\"></color>";
		string text7 = text5 + " > " + text6;
		string text8 = "";
		string text9 = "<color=" + text4 + ">" + text3 + "</color>";
		string text10 = "<color=" + text + ">" + cleanValue + "</color>";
		string text11 = " " + text2 + " <color=" + text4 + ">" + cleanString + "</color>  ";
		string text12 = "[ " + text7 + " ]";
		string text13 = text8 + text9 + text10 + text11 + text12;
		return StringHelper.TooltipPoint + text13;
	}

	public static string CreateLine(Enums.WeaponStatType weaponStatType, float baseValue, float calculatedValue, Enums.PlaceableRarity rarity, FormulaSO formula, bool conditionsAreSatisfied = true)
	{
		string text;
		if (calculatedValue >= 0f)
		{
			float num = calculatedValue - baseValue;
			Enums.TooltipValueDifference valueDifference = ((num != 0f) ? ((num > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
			text = (conditionsAreSatisfied ? ColorHelper.GetColorStringForTooltip(valueDifference) : ColorHelper.GetColorConditionUnsatisfied(rarity));
		}
		else
		{
			float num = calculatedValue;
			Enums.TooltipValueDifference valueDifference2 = ((num != 0f) ? ((num > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
			text = (conditionsAreSatisfied ? ColorHelper.GetColorStringForTooltip(valueDifference2) : ColorHelper.GetColorConditionUnsatisfied(rarity));
		}
		string cleanValue = StringHelper.GetCleanValue(calculatedValue, weaponStatType);
		string cleanString = StringHelper.GetCleanString(weaponStatType);
		string text2 = "<sprite name=\"" + StringHelper.GetSpriteValue(weaponStatType) + "\">";
		string text3 = ((calculatedValue > 0f) ? "+" : "");
		string text4 = (conditionsAreSatisfied ? ColorHelper.ColorConditionSatisfied : ColorHelper.GetColorConditionUnsatisfied(rarity));
		string text5 = "<color=" + text4 + "><sprite name=\"" + StringHelper.GetFormulaTypeToCheckAgainstIcon(formula) + "\"></color>";
		string text6 = "<color=" + text4 + "><sprite name=\"" + StringHelper.GetSpriteValue(weaponStatType) + "\"></color>";
		string text7 = text5 + " > " + text6;
		string text8 = "";
		string text9 = "<color=" + text4 + ">" + text3 + "</color>";
		string text10 = "<color=" + text + ">" + cleanValue + "</color>";
		string text11 = " " + text2 + " <color=" + text4 + ">" + cleanString + "</color>  ";
		string text12 = "[ " + text7 + " ]";
		string text13 = text8 + text9 + text10 + text11 + text12;
		return StringHelper.TooltipPoint + text13;
	}
}
