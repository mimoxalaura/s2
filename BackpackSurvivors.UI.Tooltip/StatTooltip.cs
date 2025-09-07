using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class StatTooltip : BaseTooltip
{
	[Header("Visuals")]
	[SerializeField]
	private Image _backgroundImage;

	internal void RefreshUI()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)base.transform);
		Canvas.ForceUpdateCanvases();
	}

	internal void SetStat(Enums.ItemStatType statType, List<ItemStatModifier> itemStatModifiers, bool active)
	{
		string header = " <sprite name=\"" + StringHelper.GetSpriteValue(statType) + "\"> " + StringHelper.GetCleanString(statType);
		string text = StringHelper.GetDescription(statType);
		float num = SingletonController<GameController>.Instance.Player.BaseStats.TryGet(statType, 0f);
		if (num != 0f)
		{
			text += Environment.NewLine;
			text = text + CreateSingleStatSourceLine(num, statType, baseFromPlayer: true) + " (from Character)";
		}
		if (itemStatModifiers != null)
		{
			text += Environment.NewLine;
			foreach (Enums.ItemModifierSourceType itemModifierSourceType in Enum.GetValues(typeof(Enums.ItemModifierSourceType)))
			{
				List<ItemStatModifier> list = itemStatModifiers.Where((ItemStatModifier x) => x.Source != null && x.Source.WeaponModifierSourceType == itemModifierSourceType).ToList();
				float num2 = list.Sum((ItemStatModifier x) => x.CalculatedBonus);
				if (num2 != 0f)
				{
					text = text + CreateSingleStatSourceLine(num2, statType) + " (from " + StringHelper.GetCleanValue(itemModifierSourceType, list).ToString().ToUpper() + ")";
					text += Environment.NewLine;
				}
			}
		}
		SetText(text, header);
	}

	internal void SetStat(Enums.DamageType damageType, List<DamageTypeValueModifier> damageTypeValueModifiers, bool active)
	{
		string header = " <sprite name=\"" + StringHelper.GetSpriteValue(damageType) + "\"> " + StringHelper.GetCleanString(damageType);
		string text = StringHelper.GetDescription(damageType);
		if (damageTypeValueModifiers != null)
		{
			text += Environment.NewLine;
			foreach (Enums.ItemModifierSourceType itemModifierSourceType in Enum.GetValues(typeof(Enums.ItemModifierSourceType)))
			{
				List<DamageTypeValueModifier> list = damageTypeValueModifiers.Where((DamageTypeValueModifier x) => x.Source != null && x.Source.WeaponModifierSourceType == itemModifierSourceType).ToList();
				float num = list.Sum((DamageTypeValueModifier x) => x.CalculatedBonus);
				if (num != 0f)
				{
					text = text + CreateSingleStatSourceLine(num, damageType) + " (from " + StringHelper.GetCleanValue(itemModifierSourceType, list).ToString().ToUpper() + ")";
					text += Environment.NewLine;
				}
			}
		}
		SetText(text, header);
	}

	private string CreateSingleStatSourceLine(float calculatedValue, Enums.ItemStatType itemStatType, bool baseFromPlayer = false)
	{
		string cleanValue = StringHelper.GetCleanValue(calculatedValue, itemStatType);
		string text = string.Empty;
		string colorStringForTooltip;
		if (calculatedValue > 0f)
		{
			if (!baseFromPlayer)
			{
				text = "+";
			}
			colorStringForTooltip = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase);
		}
		else
		{
			text = "";
			colorStringForTooltip = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase);
		}
		return "<color=" + colorStringForTooltip + ">" + text + cleanValue + "</color>";
	}

	private string CreateSingleStatSourceLine(float calculatedValue, Enums.DamageType damageType, bool baseFromPlayer = false)
	{
		string cleanValue = StringHelper.GetCleanValue(calculatedValue, damageType);
		string text = string.Empty;
		string colorStringForTooltip;
		if (calculatedValue > 0f)
		{
			if (!baseFromPlayer)
			{
				text = "+";
			}
			colorStringForTooltip = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase);
		}
		else
		{
			text = "";
			colorStringForTooltip = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase);
		}
		return "<color=" + colorStringForTooltip + ">" + text + cleanValue + "</color>";
	}
}
