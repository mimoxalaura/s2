using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipHelperBase
{
	internal static string GetColorForTooltipValues(float baseValue, float newValue)
	{
		if (baseValue != newValue)
		{
			if (baseValue > newValue)
			{
				return ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase);
			}
			return ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase);
		}
		return ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase);
	}

	internal static string GetDamageTypeString(Enums.DamageType damageType)
	{
		string original = damageType.ToString();
		original = TextMeshProStringHelper.ToBold(original);
		original = TextMeshProStringHelper.ToCaps(original);
		return "<color=" + ColorHelper.GetColorStringForDamageType(damageType) + ">" + original + "</color>";
	}
}
