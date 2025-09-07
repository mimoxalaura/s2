using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipDebuffHelper
{
	public static string CreateLine(DebuffSO debuff, bool conditionsAreSatisfied, Enums.PlaceableRarity rarity)
	{
		string text = string.Empty;
		switch (debuff.DebuffFalloffTimeType)
		{
		case Enums.Debuff.DebuffFalloffTimeType.SetTime:
			text = string.Format("{0} {1}s", "for", debuff.TimeUntillFalloff);
			break;
		case Enums.Debuff.DebuffFalloffTimeType.AfterTrigger:
			text = "once";
			break;
		case Enums.Debuff.DebuffFalloffTimeType.Infinite:
			text = "forever";
			break;
		}
		if (conditionsAreSatisfied)
		{
			return StringHelper.TooltipPoint + "Enemies hit are afflicted by <color=" + debuff.DebuffTextColor + ">" + StringHelper.AddLink(StringHelper.GetColoredName(debuff.DebuffType), StringHelper.GetCleanString(debuff.DebuffType)) + "</color> " + text;
		}
		return StringHelper.TooltipPoint + "<color=" + ColorHelper.GetColorConditionUnsatisfied(rarity) + ">Enemies hit are afflicted by <sprite name=\"" + StringHelper.GetSpriteValue(debuff.DamageSO.BaseDamageType) + "\"/> " + StringHelper.AddLink(StringHelper.GetCleanString(debuff.DebuffType), StringHelper.GetCleanString(debuff.DebuffType)) + " " + text + "</color>";
	}
}
