using System.Collections.Generic;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipDamageTypeHelper
{
	public static string CreateLine(KeyValuePair<Enums.DamageType, float> damage, Enums.PlaceableRarity rarity, bool isStarred = false, bool conditionsAreSatisfied = true, bool isWeapon = false)
	{
		string text = ((damage.Value > 0f) ? "+" : "");
		string text2 = StringHelper.TooltipPoint;
		if (!isWeapon)
		{
			text2 = StringHelper.TooltipPointPositive;
		}
		if (damage.Value < 0f && !isWeapon)
		{
			text2 = StringHelper.TooltipPointNegative;
		}
		string text4;
		if (conditionsAreSatisfied)
		{
			string text3 = "<color=" + Constants.Colors.HexStrings.TooltipHexStringSameAsBase + ">" + text + StringHelper.GetValueAsPercentageString(damage.Value, isModifier: false) + "</color>";
			text4 = text3 + " <sprite name=\"" + StringHelper.GetSpriteValue(damage.Key) + "\"> " + damage.Key.ToString().ToUpper() + " DAMAGE";
		}
		else
		{
			string text3 = text + StringHelper.GetValueAsPercentageString(damage.Value, isModifier: false);
			text4 = "<color=" + ColorHelper.GetColorConditionUnsatisfied(rarity) + ">" + text3 + " <sprite name=\"" + StringHelper.GetSpriteValue(damage.Key) + "\"> " + damage.Key.ToString().ToUpper() + " DAMAGE</color>";
		}
		if (conditionsAreSatisfied)
		{
			text4 = TextMeshProStringHelper.HighlightElementalKeywords(text4);
			text4 = TextMeshProStringHelper.HighlightKeywords(text4, "#FFF");
		}
		return text2 + text4;
	}
}
