using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.System.Settings.Events;
using UnityEngine;

namespace BackpackSurvivors.UI.DamageNumbers;

public class DamageVisualizer : MonoBehaviour
{
	private DamageNumbersController _uiController;

	private bool _showNumbers;

	private void Start()
	{
		_uiController = Object.FindObjectOfType<DamageNumbersController>();
		_showNumbers = SingletonController<SettingsController>.Instance.GameplaySettingsController.ShowDamageNumbers == Enums.ShowDamageNumbers.Visible;
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SettingsController>.Instance, RegisterSettingsControllerInitialized);
	}

	private void RegisterSettingsControllerInitialized()
	{
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnShowDamageNumbersSettingChanged += GameplaySettingsController_OnShowDamageNumbersSettingChanged;
	}

	private void GameplaySettingsController_OnShowDamageNumbersSettingChanged(object sender, ShowDamageNumbersSettingsChangedEventArgs e)
	{
		_showNumbers = e.ShowDamageNumbers == Enums.ShowDamageNumbers.Visible;
	}

	internal void ShowDamagePopup(float damage, bool wasCrit, Enums.CharacterType characterType, Enums.DamageType damageType)
	{
		if (!(damage <= float.Epsilon) && _showNumbers)
		{
			int number = Mathf.CeilToInt(damage);
			Color color = GetColor(wasCrit, characterType, damageType);
			_uiController.InstantiateNumberPopup(base.transform.position, number, color, wasCrit);
		}
	}

	internal void ShowTextPopup(string text, Color color, float fadeOutTime = 1f, float movementY = 0.5f)
	{
		_uiController.InstantiateTextPopup(base.transform.position, text, color, fadeOutTime, movementY);
	}

	private Color GetColor(bool wasCrit, Enums.CharacterType characterType, Enums.DamageType damageType)
	{
		Color result = Color.white;
		switch (characterType)
		{
		case Enums.CharacterType.Player:
			result = (wasCrit ? Constants.Colors.PlayerDamagedByCritColor : Constants.Colors.PlayerDamagedColor);
			break;
		case Enums.CharacterType.Enemy:
			result = ColorHelper.GetColorDamageType(damageType);
			break;
		}
		return result;
	}

	private void OnDestroy()
	{
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnShowDamageNumbersSettingChanged -= GameplaySettingsController_OnShowDamageNumbersSettingChanged;
	}
}
