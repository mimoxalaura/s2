using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipSimpleStatHelper
{
	public static string CreateLine(Enums.ItemStatType itemStatType, float baseValue, float calculatedValue, Enums.PlaceableRarity rarity, bool conditionsAreSatisfied = true, bool isWeapon = false)
	{
		string colorConditionSatisfied = ColorHelper.ColorConditionSatisfied;
		string colorConditionUnsatisfied = ColorHelper.GetColorConditionUnsatisfied(rarity);
		string text = (conditionsAreSatisfied ? colorConditionSatisfied : colorConditionUnsatisfied);
		string text2 = StringHelper.TooltipPoint;
		float num;
		string text3;
		if (isWeapon)
		{
			if (calculatedValue >= 0f)
			{
				num = calculatedValue - baseValue;
				text3 = ColorHelper.GetColorStringForTooltip((num != 0f) ? ((num > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
			}
			else
			{
				num = calculatedValue;
				text3 = ColorHelper.GetColorStringForTooltip((num != 0f) ? ((num > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
				text2 = StringHelper.TooltipPointNegative;
			}
		}
		else
		{
			num = calculatedValue;
			if (calculatedValue < 0f)
			{
				text3 = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase);
				text2 = StringHelper.TooltipPointNegative;
			}
			else
			{
				text3 = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase);
				text2 = StringHelper.TooltipPointPositive;
			}
		}
		if (!conditionsAreSatisfied)
		{
			text3 = colorConditionUnsatisfied;
		}
		string cleanValue = StringHelper.GetCleanValue(calculatedValue, itemStatType, isModifier: true);
		string cleanValue2 = StringHelper.GetCleanValue(num, itemStatType, isModifier: true);
		string cleanString = StringHelper.GetCleanString(itemStatType);
		string text4 = string.Empty;
		if (baseValue >= 0f && isWeapon)
		{
			if (num != 0f)
			{
				string text5 = ((num >= 0f) ? "+" : "-");
				if (num < 0f)
				{
					text2 = StringHelper.TooltipPointNegative;
				}
				text4 = "(<color=" + text3 + ">" + text5 + cleanValue2 + "</color>)";
			}
		}
		else if (num != 0f)
		{
			text4 = string.Empty;
		}
		string text6 = "<sprite name=\"" + StringHelper.GetSpriteValue(itemStatType) + "\">";
		string text7 = string.Empty;
		if (calculatedValue > 0f && !cleanValue.Contains("+"))
		{
			text7 = "+";
		}
		return text2 + "<color=" + text + ">" + text7 + "<color=" + text3 + ">" + cleanValue + "</color> " + text6 + " " + cleanString + text4 + "</color>";
	}

	public static string CreateLine(Enums.WeaponStatType weaponStatType, float baseValue, float calculatedValue, bool isStarred = false)
	{
		float num = calculatedValue - baseValue;
		string colorForTooltipValues = TooltipHelperBase.GetColorForTooltipValues(baseValue, calculatedValue);
		string cleanValue = StringHelper.GetCleanValue(calculatedValue, weaponStatType);
		string cleanValue2 = StringHelper.GetCleanValue(num, weaponStatType);
		string cleanString = StringHelper.GetCleanString(weaponStatType);
		string text = string.Empty;
		string text2 = StringHelper.TooltipPoint;
		if (!isStarred)
		{
			if (num != 0f)
			{
				string text3 = string.Empty;
				if (baseValue >= 0f)
				{
					if (num > 0f)
					{
						text3 = "+";
					}
					else
					{
						text2 = StringHelper.TooltipPointNegative;
					}
				}
				text = "(<color=" + colorForTooltipValues + ">" + text3 + cleanValue2 + "</color>)";
			}
		}
		else
		{
			text = string.Empty;
		}
		string text4 = "<sprite name=\"" + StringHelper.GetSpriteValue(weaponStatType) + "\">";
		if (!isStarred)
		{
			return text2 + "<color=" + colorForTooltipValues + ">" + cleanValue + "</color> " + text4 + " " + cleanString + text;
		}
		return text2 + "+<color=" + colorForTooltipValues + ">" + cleanValue + "</color> " + text4 + " " + cleanString + text;
	}
}
