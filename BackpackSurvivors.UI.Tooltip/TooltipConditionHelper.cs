using System.Collections.Generic;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipConditionHelper
{
	public static string CreateLine(ConditionSO conditionSO, List<WeaponInstance> starredWeapons, bool isStarCondition)
	{
		Condition condition = new Condition(conditionSO);
		string text = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase);
		string generatedDescription = condition.GetGeneratedDescription(isStarCondition);
		string text2 = string.Empty;
		string empty = string.Empty;
		float currentConditionAmount = condition.GetCurrentConditionAmount();
		string text3 = currentConditionAmount.ToString();
		string text4 = conditionSO.CheckAmount.ToString();
		if (conditionSO.ConditionTarget == Enums.ConditionalStats.ConditionTarget.Global)
		{
			if (condition.IsConditionSatisfied())
			{
				empty = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase);
				text2 = StringHelper.YesSprite;
				text = empty;
			}
			else
			{
				empty = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase);
				text2 = StringHelper.NoSprite;
			}
		}
		else
		{
			bool flag = false;
			foreach (WeaponInstance starredWeapon in starredWeapons)
			{
				flag = condition.IsConditionSatisfied(starredWeapon);
			}
			if (flag)
			{
				empty = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase);
				text = empty;
			}
			else
			{
				empty = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase);
			}
		}
		string text5 = string.Empty;
		if (conditionSO.ConditionTarget == Enums.ConditionalStats.ConditionTarget.Global)
		{
			switch (conditionSO.ConditionCheckType)
			{
			case Enums.ConditionalStats.ConditionCheckType.Minimum:
				switch (condition.TypeToCheckAgainst)
				{
				case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
					text4 = StringHelper.GetCleanValue(conditionSO.CheckAmount, condition.ItemStatType);
					text3 = StringHelper.GetCleanValue(currentConditionAmount, condition.ItemStatType);
					break;
				}
				text5 = "(<color=" + empty + ">" + text3 + "/" + text4 + "</color>)";
				break;
			case Enums.ConditionalStats.ConditionCheckType.Maximum:
				text5 = $"(<color={empty}>{currentConditionAmount}</color>)";
				break;
			}
		}
		return (text2 + " " + generatedDescription + " " + text5).Replace("[NUMBER]", "<color=" + text + ">" + text4 + "</color>");
	}
}
