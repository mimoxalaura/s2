using System;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.System.Settings.Events;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Health;

public class HealthBarUI : MonoBehaviour
{
	[Tooltip("Optional; Either assign a reference in the Editor (that implements IGetHealthSystem) or manually call SetHealthSystem()")]
	[SerializeField]
	private GameObject getHealthSystemGameObject;

	[Tooltip("Image to show the Health Bar, should be set as Fill, the script modifies fillAmount")]
	[SerializeField]
	private Image _image;

	[SerializeField]
	private GameObject _healthbar;

	[SerializeField]
	private GameObject _debuffs;

	private Enums.Enemies.EnemyType _enemyType;

	private HealthSystem _healthSystem;

	private void Start()
	{
		ToggleHealthbarVisibility(SingletonController<SettingsController>.Instance.GameplaySettingsController.ShowHealthBars == Enums.ShowHealthBars.Visible || (SingletonController<SettingsController>.Instance.GameplaySettingsController.ShowHealthBars == Enums.ShowHealthBars.OnlyBosses && (_enemyType == Enums.Enemies.EnemyType.Miniboss || _enemyType == Enums.Enemies.EnemyType.Boss)));
		if (HealthSystem.TryGetHealthSystem(getHealthSystemGameObject, out var healthSystem))
		{
			SetHealthSystem(healthSystem, Enums.Enemies.EnemyType.Monster);
		}
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnShowHealthBarsSettingChanged += GameplaySettingsController_OnShowHealthBarsSettingChanged;
	}

	private void GameplaySettingsController_OnShowHealthBarsSettingChanged(object sender, ShowHealthBarsSettingsChangedEventArgs e)
	{
		ToggleHealthbarVisibility(e.ShowHealthBars == Enums.ShowHealthBars.Visible || (e.ShowHealthBars == Enums.ShowHealthBars.OnlyBosses && (_enemyType == Enums.Enemies.EnemyType.Miniboss || _enemyType == Enums.Enemies.EnemyType.Boss)));
	}

	private void ToggleHealthbarVisibility(bool visible)
	{
		_healthbar.SetActive(visible);
		_debuffs.SetActive(visible);
	}

	public void SetHealthSystem(HealthSystem healthSystem, Enums.Enemies.EnemyType enemyType)
	{
		_enemyType = enemyType;
		if (_healthSystem != null)
		{
			_healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
		}
		_healthSystem = healthSystem;
		UpdateHealthBar();
		healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
	}

	private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
	{
		UpdateHealthBar();
	}

	private void UpdateHealthBar()
	{
		_image.fillAmount = _healthSystem.GetHealthNormalized();
	}

	private void OnDestroy()
	{
		if (_healthSystem != null)
		{
			_healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
		}
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnShowHealthBarsSettingChanged -= GameplaySettingsController_OnShowHealthBarsSettingChanged;
	}
}
