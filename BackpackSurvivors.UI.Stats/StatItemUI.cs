using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Stats;

public class StatItemUI : MonoBehaviour
{
	[SerializeField]
	private StatTooltipTrigger _statTooltipTrigger;

	[SerializeField]
	private Image _statIconImage;

	[SerializeField]
	private TextMeshProUGUI _statNameText;

	[SerializeField]
	protected TextMeshProUGUI _statValueText;

	protected Enums.ItemStatType _itemStatType;

	protected Enums.DamageType _itemDamageType;

	protected List<ItemStatModifier> _itemStatModifiers;

	protected List<DamageTypeValueModifier> _damageTypeValueModifiers;

	protected string _lastValueColor;

	public virtual void Init(Enums.ItemStatType itemStatType, List<ItemStatModifier> itemStatModifiers)
	{
		_itemStatType = itemStatType;
		_itemStatModifiers = itemStatModifiers;
		_statNameText.SetText(StringHelper.GetCleanString(itemStatType));
		_statValueText.SetText(StringHelper.GetCleanValue(_itemStatModifiers.Sum((ItemStatModifier x) => x.CalculatedBonus), itemStatType));
		_statIconImage.sprite = SpriteHelper.GetItemStatTypeSprite(itemStatType);
		_statTooltipTrigger.SetStat(itemStatType, _itemStatModifiers, active: true);
	}

	public virtual void Init(Enums.DamageType damageType, List<DamageTypeValueModifier> damageTypeValueModifiers)
	{
		_itemDamageType = damageType;
		_damageTypeValueModifiers = damageTypeValueModifiers;
		_statNameText.SetText(StringHelper.GetCleanString(damageType));
		_statValueText.SetText(_damageTypeValueModifiers.Sum((DamageTypeValueModifier x) => x.CalculatedBonus).ToString());
		_statIconImage.sprite = SpriteHelper.GetDamageTypeIconSprite(damageType);
		_statTooltipTrigger.SetStat(_itemDamageType, _damageTypeValueModifiers, active: true);
	}

	public virtual void UpdateStat(string newValue, float statChange, List<ItemStatModifier> itemStatModifiers)
	{
		string color = GetColor(statChange);
		SetValueText(newValue, color);
		UpdateTooltip(itemStatModifiers);
	}

	public virtual void UpdateStat(string newValue, float statChange, List<DamageTypeValueModifier> damageTypeValueModifiers)
	{
		string color = GetColor(statChange);
		SetValueText(newValue, color);
		UpdateTooltip(damageTypeValueModifiers);
	}

	public virtual void UpdateTooltip(List<ItemStatModifier> itemStatModifiers)
	{
		_itemStatModifiers = itemStatModifiers;
		_statTooltipTrigger.SetStat(_itemStatType, _itemStatModifiers, active: true);
	}

	public virtual void UpdateTooltip(List<DamageTypeValueModifier> damageTypeValueModifiers)
	{
		_damageTypeValueModifiers = damageTypeValueModifiers;
		_statTooltipTrigger.SetStat(_itemDamageType, _damageTypeValueModifiers, active: true);
	}

	public void AnimateStat(string value, string colorString = "#ff0000", int numberOfBlinks = 3, float timeBetweenBlinks = 0.1f)
	{
		StartCoroutine(AnimateStatCoroutine(value, colorString, numberOfBlinks, timeBetweenBlinks));
	}

	private IEnumerator AnimateStatCoroutine(string value, string colorString, int numberOfBlinks, float timeBetweenBlinks)
	{
		string statNameText = StringHelper.GetCleanString(_itemStatType);
		for (int i = 0; i < numberOfBlinks; i++)
		{
			SetValueText(value, colorString, saveValueColor: false);
			SetStatNameText(statNameText, colorString);
			yield return new WaitForSecondsRealtime(timeBetweenBlinks);
			SetValueText(value, Constants.Colors.HexStrings.White, saveValueColor: false);
			SetStatNameText(statNameText, Constants.Colors.HexStrings.White);
			yield return new WaitForSecondsRealtime(timeBetweenBlinks);
		}
		SetValueText(value, _lastValueColor, saveValueColor: false);
		_statNameText.SetText(statNameText ?? "");
	}

	internal virtual void SetValueText(string value, string colorString, bool saveValueColor = true)
	{
		if (saveValueColor)
		{
			_lastValueColor = colorString;
		}
		_statValueText.SetText("<color=" + colorString + ">" + value + "</color>");
	}

	private void SetStatNameText(string statName, string colorString)
	{
		_statNameText.SetText("<color=" + colorString + ">" + statName + "</color>");
	}

	private string GetColor(float statChange)
	{
		string result = string.Empty;
		if (statChange == 0f)
		{
			result = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase);
		}
		if (statChange > 0f)
		{
			result = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase);
		}
		if (statChange < 0f)
		{
			result = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase);
		}
		return result;
	}
}
