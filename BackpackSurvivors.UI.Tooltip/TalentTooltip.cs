using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class TalentTooltip : BaseTooltip
{
	[Header("Visuals")]
	[SerializeField]
	private Image _backgroundImage;

	[Header("Backdrop")]
	[SerializeField]
	private Sprite _regular;

	[SerializeField]
	private Sprite _keystone;

	internal void RefreshUI()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)base.transform);
		Canvas.ForceUpdateCanvases();
	}

	public void SetTalent(TalentSO talentSO, bool active, bool showActivatedPart)
	{
		SetText(CreateTooltipDescription(active, showActivatedPart, talentSO), CreateTooltipHeader(talentSO));
		SetBackground(talentSO);
	}

	private void SetBackground(TalentSO talentSO)
	{
		if (talentSO.IsKeystone)
		{
			_backgroundImage.sprite = _keystone;
		}
		else
		{
			_backgroundImage.sprite = _regular;
		}
	}

	internal static string CreateTooltipDescription(bool active, bool showActivatedPart, TalentSO talentSO)
	{
		string empty = string.Empty;
		empty += Environment.NewLine;
		if (talentSO.UseCustomDescription)
		{
			empty += talentSO.Description;
		}
		else
		{
			string text = string.Empty;
			if (talentSO.Stats == null)
			{
				return string.Empty;
			}
			if (talentSO.Stats.StatValues.Any())
			{
				foreach (KeyValuePair<Enums.ItemStatType, float> statValue in talentSO.Stats.StatValues)
				{
					string spriteValue = StringHelper.GetSpriteValue(statValue.Key);
					string text2 = ((statValue.Value > 0f) ? ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase) : ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase));
					string text3 = ((statValue.Value > 0f) ? "+" : "");
					text = text + "<color=" + text2 + ">" + text3 + StringHelper.GetCleanValue(statValue.Value, statValue.Key) + "</color> <sprite name=\"" + spriteValue + "\"> " + StringHelper.GetCleanString(statValue.Key);
					text += Environment.NewLine;
				}
			}
			if (talentSO.Stats.DamageTypeValues.Any())
			{
				foreach (KeyValuePair<Enums.DamageType, float> damageTypeValue in talentSO.Stats.DamageTypeValues)
				{
					string spriteValue2 = StringHelper.GetSpriteValue(damageTypeValue.Key);
					string colorStringForDamageType = ColorHelper.GetColorStringForDamageType(damageTypeValue.Key);
					string text4 = ((damageTypeValue.Value > 0f) ? ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase) : ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase));
					string text5 = ((damageTypeValue.Value > 0f) ? "+" : "");
					text = text + "<color=" + text4 + ">" + text5 + StringHelper.GetCleanValue(damageTypeValue.Value, damageTypeValue.Key) + "</color> <sprite name=\"" + spriteValue2 + "\"> <color=" + colorStringForDamageType + ">" + StringHelper.GetCleanString(damageTypeValue.Key) + "</color> damage";
					text += Environment.NewLine;
				}
			}
			if (talentSO.Stats != null && talentSO.Stats.DebuffSOs.Any())
			{
				DebuffSO[] debuffSOs = talentSO.Stats.DebuffSOs;
				foreach (DebuffSO debuffSO in debuffSOs)
				{
					string text6 = string.Empty;
					if (debuffSO.MaxStacks > 1)
					{
						text6 = "(stacks)";
					}
					string text7 = string.Empty;
					switch (debuffSO.DebuffFalloffTimeType)
					{
					case Enums.Debuff.DebuffFalloffTimeType.SetTime:
						text7 = $"for {debuffSO.TimeUntillFalloff}s";
						break;
					case Enums.Debuff.DebuffFalloffTimeType.AfterTrigger:
						text7 = "once";
						break;
					case Enums.Debuff.DebuffFalloffTimeType.Infinite:
						text7 = "forever";
						break;
					}
					string text8 = "Enemies hit are afflicted by <color=" + debuffSO.DebuffTextColor + ">" + debuffSO.DebuffType.ToString().ToUpper() + "</color> " + text7 + " " + text6;
					text += text8;
					text += Environment.NewLine;
				}
			}
			if (talentSO.Stats.Conditions.Any())
			{
				text += Environment.NewLine;
				Condition[] array = new Condition[talentSO.Stats.Conditions.Length];
				for (int j = 0; j < talentSO.Stats.Conditions.Length; j++)
				{
					array[j] = new Condition(talentSO.Stats.Conditions[j]);
				}
				string conditionsString = StringHelper.GetConditionsString(array, isStarCondition: false);
				text = text + "(" + conditionsString + ")";
			}
			empty += text;
		}
		empty += Environment.NewLine;
		if (showActivatedPart)
		{
			empty += Environment.NewLine;
			empty = ((!active) ? (empty + "<size='18'><color=#B97729>(Click to activate talentpoint)</color></size>") : (empty + "<size='18'><color=#B97729>(Click to remove talentpoint)</color></size>"));
		}
		return empty;
	}

	private string CreateTooltipHeader(TalentSO talentSO)
	{
		if (talentSO.IsKeystone)
		{
			return talentSO.Name + Environment.NewLine + "<color=#1696FF>KEYSTONE</color>" + Environment.NewLine;
		}
		return talentSO.Name + Environment.NewLine;
	}
}
