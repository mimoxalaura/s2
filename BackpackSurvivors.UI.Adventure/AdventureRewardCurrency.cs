using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureRewardCurrency : AdventureReward
{
	[SerializeField]
	private DefaultTooltipTrigger _tooltip;

	private int _value;

	public void Init(Enums.CurrencyType currencyType, float value, bool unlocked, bool interactable)
	{
		Init(interactable);
		SetImage(SpriteHelper.GetCurrencySprite(currencyType));
		SetText($"x{(int)value}");
		_value = (int)value;
		_tooltip.SetDefaultContent(StringHelper.GetCleanString(currencyType), _value.ToString(), active: true);
		_tooltip.ToggleEnabled(unlocked);
		LeanTween.scale(base.Image.gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.3f).setEaseInOutBounce().setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(base.Image.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
	}
}
