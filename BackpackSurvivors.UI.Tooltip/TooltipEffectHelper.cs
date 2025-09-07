using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipEffectHelper
{
	public static string CreateLine(WeaponAttackEffect weaponAttackEffect, Enums.PlaceableRarity rarity, bool conditionsAreSatisfied)
	{
		string valuesForDescriptor;
		if (conditionsAreSatisfied)
		{
			valuesForDescriptor = TextMeshProStringHelper.HighlightKeywords(weaponAttackEffect.SimpleEffectDescription, "#FFF");
			valuesForDescriptor = weaponAttackEffect.HandlerOnTrigger.SetValuesForDescriptor(valuesForDescriptor);
			valuesForDescriptor = TextMeshProStringHelper.HighlightElementalKeywords(valuesForDescriptor);
			valuesForDescriptor = valuesForDescriptor ?? "";
		}
		else
		{
			valuesForDescriptor = "<color=" + ColorHelper.GetColorConditionUnsatisfied(rarity) + ">" + weaponAttackEffect.SimpleEffectDescription + "</color>";
			valuesForDescriptor = weaponAttackEffect.HandlerOnTrigger.SetValuesForDescriptor(valuesForDescriptor);
		}
		return StringHelper.TooltipPoint + valuesForDescriptor;
	}
}
