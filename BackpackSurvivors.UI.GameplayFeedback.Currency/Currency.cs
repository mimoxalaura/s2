using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback.Currency;

public class Currency : MonoBehaviour
{
	[SerializeField]
	private bool _visible;

	[SerializeField]
	private Enums.CurrencyType _currencyType;

	[SerializeField]
	private TextMeshProUGUI _currencyText;

	[SerializeField]
	private LostOrReceivedCurrencyAnimation _lostOrReceivedCurrencyAnimationPrefab;

	[SerializeField]
	private Transform _lostOrReceivedCurreencyControllerContainer;

	[SerializeField]
	private DefaultTooltipTrigger _defaultTooltipTrigger;

	public Enums.CurrencyType CurrencyType => _currencyType;

	private void Start()
	{
		SetVisibility(_visible);
	}

	public void SetVisibility(bool visible)
	{
		_visible = visible;
		base.gameObject.SetActive(_visible);
		RefreshUI();
	}

	public void UpdateCurrency(int newValue, int changedAmount, Enums.CurrencySource source)
	{
		_currencyText.SetText(newValue.ToString());
		if (_defaultTooltipTrigger != null)
		{
			_defaultTooltipTrigger.SetDefaultContent(StringHelper.GetCleanString(_currencyType), $"You have <sprite name=\"{StringHelper.GetSpriteValue(_currencyType)}\"> <color={ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase)}>{newValue}</color> {StringHelper.GetCleanString(_currencyType)}. {StringHelper.GetDescription(_currencyType)}", active: true);
		}
		if (changedAmount != 0)
		{
			if (base.isActiveAndEnabled && !LeanTweenHelper.IsAtMaxCapacity())
			{
				Object.Instantiate(_lostOrReceivedCurrencyAnimationPrefab, _lostOrReceivedCurreencyControllerContainer).Init(changedAmount, _currencyText.gameObject);
			}
			RefreshUI();
		}
	}

	internal void RefreshUI()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)base.transform);
		Canvas.ForceUpdateCanvases();
	}

	private bool ShouldPlayCurrencyGainedAudioClip(Enums.CurrencySource source)
	{
		return source == Enums.CurrencySource.Drop;
	}

	private bool ShouldPlayCurrencyLostAudioClip(Enums.CurrencySource source)
	{
		return source == Enums.CurrencySource.Shop;
	}

	internal void AnimateCannotAfford()
	{
		float num = 0.2f;
		float num2 = -10f;
		_ = _currencyText.transform.position;
		_ = _currencyText.transform.position;
		float num3 = 22f;
		float num4 = 32f;
		Color white = Color.white;
		Color red = Color.red;
		LeanTween.value(_currencyText.gameObject, UpdateCurrencyTextColor, white, red, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_currencyText.gameObject, UpdateCurrencyTextSize, num3, num4, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_currencyText.gameObject, UpdateCurrencyTextColor, red, white, num).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_currencyText.gameObject, UpdateCurrencyTextSize, num4, num3, num).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void UpdateCurrencyTextColor(Color val)
	{
		_currencyText.color = val;
	}

	private void UpdateCurrencyTextSize(float val)
	{
		_currencyText.fontSize = val;
	}
}
