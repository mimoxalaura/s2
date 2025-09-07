using System.Collections;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Game.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback.Currency;

internal class CurrencyChangedFeedback : MonoBehaviour
{
	[SerializeField]
	private Currency[] _currencies;

	[SerializeField]
	private AudioClip _notEnoughCurrencyAudioClip;

	private const float MinimumTimeBetweenNotEnoughCurrencyEvents = 0.5f;

	private bool _canFireNotEnoughCurrencyEvent = true;

	private void Start()
	{
		RegisterEvents();
		UpdateUI();
	}

	private void UpdateUI()
	{
		Currency[] currencies = _currencies;
		foreach (Currency currency in currencies)
		{
			int currency2 = SingletonController<CurrencyController>.Instance.GetCurrency(currency.CurrencyType);
			currency.UpdateCurrency(currency2, 0, Enums.CurrencySource.PrerunSetup);
		}
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CurrencyController>.Instance, RegisterCurrencyEvents);
	}

	public void RegisterCurrencyEvents()
	{
		SingletonController<CurrencyController>.Instance.OnCurrencyChanged += CurrencyController_OnCurrencyChanged;
		SingletonController<CurrencyController>.Instance.OnCannotAffordCurrency += CurrencyController_OnCannotAffordCurrency;
		UpdateUI();
	}

	private void CurrencyController_OnCannotAffordCurrency(object sender, CannotAffordCurrencyEventArgs e)
	{
		if (!_canFireNotEnoughCurrencyEvent || !base.isActiveAndEnabled)
		{
			return;
		}
		Currency[] currencies = _currencies;
		foreach (Currency currency in currencies)
		{
			if (e.CurrencyType == currency.CurrencyType)
			{
				currency.AnimateCannotAfford();
			}
		}
		SingletonController<AudioController>.Instance.PlaySFXClip(_notEnoughCurrencyAudioClip, 1f);
		StartCoroutine(ActivateNotEnoughCurrencyEventCooldown());
	}

	private IEnumerator ActivateNotEnoughCurrencyEventCooldown()
	{
		_canFireNotEnoughCurrencyEvent = false;
		yield return new WaitForSecondsRealtime(0.5f);
		_canFireNotEnoughCurrencyEvent = true;
	}

	private void CurrencyController_OnCurrencyChanged(object sender, CurrencyChangedEventArgs e)
	{
		Currency currency = _currencies.FirstOrDefault((Currency x) => x.CurrencyType == e.CurrencyType);
		if (!(currency == null))
		{
			currency.UpdateCurrency(e.NewAmount, e.ChangedValue, e.Source);
		}
	}

	public void ToggleVisibility(Enums.CurrencyType currencyType, bool visible)
	{
		Currency currency = _currencies.FirstOrDefault((Currency x) => x.CurrencyType == currencyType);
		if (!(currency == null))
		{
			currency.SetVisibility(visible);
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		SingletonController<CurrencyController>.Instance.OnCurrencyChanged -= CurrencyController_OnCurrencyChanged;
		SingletonController<CurrencyController>.Instance.OnCannotAffordCurrency -= CurrencyController_OnCannotAffordCurrency;
	}
}
