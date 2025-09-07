using System;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.System.Settings.Events;

namespace BackpackSurvivors.System.Settings;

[Serializable]
public class GameplaySettingsController
{
	public delegate void ShowDamageNumbersSettingChangedHandler(object sender, ShowDamageNumbersSettingsChangedEventArgs e);

	public delegate void ShowHealthBarsSettingChangedHandler(object sender, ShowHealthBarsSettingsChangedEventArgs e);

	public delegate void CooldownVisualsSettingChangedHandler(object sender, CooldownVisualSettingsChangedEventArgs e);

	public delegate void MinimapSettingChangedHandler(object sender, MinimapSettingsChangedEventArgs e);

	public delegate void FlashOnDamageTakenSettingChangedHandler(object sender, FlashOnDamageTakenSettingsChangedEventArgs e);

	public delegate void DashVisualSettingChangedHandler(object sender, DashVisualSettingsChangedEventArgs e);

	public delegate void TargetingSettingChangedHandler(object sender, TargetingSettingsChangedEventArgs e);

	public delegate void TooltipComplexitySettingChangedSettingChangedHandler(object sender, TooltipComplexitySettingsChangedEventArgs e);

	private Enums.ShowDamageNumbers _showDamageNumbers;

	private Enums.ShowHealthBars _showHealthBars;

	private Enums.CooldownVisuals _cooldownVisuals;

	private Enums.MinimapVisual _minimapVisual;

	private Enums.FlashOnDamageTaken _flashOnDamageTaken;

	private Enums.DashVisual _dashVisual;

	private Enums.Targeting _targeting;

	private Enums.TooltipComplexity _tooltipComplexity;

	public Enums.ShowDamageNumbers ShowDamageNumbers => _showDamageNumbers;

	public Enums.ShowHealthBars ShowHealthBars => _showHealthBars;

	public Enums.CooldownVisuals CooldownVisuals => _cooldownVisuals;

	public Enums.MinimapVisual MinimapVisual => _minimapVisual;

	public Enums.FlashOnDamageTaken FlashOnDamageTaken => _flashOnDamageTaken;

	public Enums.DashVisual DashVisual => _dashVisual;

	public Enums.Targeting Targeting => _targeting;

	public Enums.TooltipComplexity TooltipComplexity => _tooltipComplexity;

	public event CooldownVisualsSettingChangedHandler OnCooldownVisualsSettingChanged;

	public event MinimapSettingChangedHandler OnMinimapSettingChanged;

	public event ShowHealthBarsSettingChangedHandler OnShowHealthBarsSettingChanged;

	public event ShowDamageNumbersSettingChangedHandler OnShowDamageNumbersSettingChanged;

	public event FlashOnDamageTakenSettingChangedHandler OnFlashOnDamageTakenSettingChanged;

	public event DashVisualSettingChangedHandler OnDashVisualSettingChanged;

	public event TargetingSettingChangedHandler OnTargetingSettingChanged;

	public event TooltipComplexitySettingChangedSettingChangedHandler OnTooltipComplexitySettingChanged;

	public bool UpdateShowDamageNumbers(Enums.ShowDamageNumbers showDamageNumbers)
	{
		_showDamageNumbers = showDamageNumbers;
		if (this.OnShowDamageNumbersSettingChanged != null)
		{
			this.OnShowDamageNumbersSettingChanged(this, new ShowDamageNumbersSettingsChangedEventArgs(showDamageNumbers));
		}
		return true;
	}

	public bool UpdateShowHealthBars(Enums.ShowHealthBars showHealthBars)
	{
		_showHealthBars = showHealthBars;
		if (this.OnShowHealthBarsSettingChanged != null)
		{
			this.OnShowHealthBarsSettingChanged(this, new ShowHealthBarsSettingsChangedEventArgs(showHealthBars));
		}
		return true;
	}

	public bool UpdateCooldownVisuals(Enums.CooldownVisuals cooldownVisuals)
	{
		_cooldownVisuals = cooldownVisuals;
		if (this.OnCooldownVisualsSettingChanged != null)
		{
			this.OnCooldownVisualsSettingChanged(this, new CooldownVisualSettingsChangedEventArgs(cooldownVisuals));
		}
		return true;
	}

	public bool UpdateMinimap(Enums.MinimapVisual minimapVisual)
	{
		_minimapVisual = minimapVisual;
		if (this.OnMinimapSettingChanged != null)
		{
			this.OnMinimapSettingChanged(this, new MinimapSettingsChangedEventArgs(minimapVisual));
		}
		return true;
	}

	public bool UpdateFlashOnDamageTaken(Enums.FlashOnDamageTaken flashOnDamageTaken)
	{
		_flashOnDamageTaken = flashOnDamageTaken;
		if (this.OnFlashOnDamageTakenSettingChanged != null)
		{
			this.OnFlashOnDamageTakenSettingChanged(this, new FlashOnDamageTakenSettingsChangedEventArgs(flashOnDamageTaken));
		}
		return true;
	}

	public bool UpdateDashVisual(Enums.DashVisual dashVisual)
	{
		_dashVisual = dashVisual;
		if (this.OnDashVisualSettingChanged != null)
		{
			this.OnDashVisualSettingChanged(this, new DashVisualSettingsChangedEventArgs(_dashVisual));
		}
		return true;
	}

	public bool UpdateTargeting(Enums.Targeting targeting)
	{
		_targeting = targeting;
		if (this.OnTargetingSettingChanged != null)
		{
			this.OnTargetingSettingChanged(this, new TargetingSettingsChangedEventArgs(_targeting));
		}
		return true;
	}

	public bool UpdateTooltipComplexity(Enums.TooltipComplexity tooltipComplexity)
	{
		_tooltipComplexity = tooltipComplexity;
		if (this.OnTooltipComplexitySettingChanged != null)
		{
			this.OnTooltipComplexitySettingChanged(this, new TooltipComplexitySettingsChangedEventArgs(_tooltipComplexity));
		}
		return true;
	}

	public void LoadSettingsFromSavegame(SettingsSaveState settingsSaveState)
	{
		_showDamageNumbers = settingsSaveState.ShowDamageNumbers;
		_showHealthBars = settingsSaveState.ShowHealthBars;
		_cooldownVisuals = settingsSaveState.CooldownVisuals;
		_minimapVisual = settingsSaveState.MinimapVisual;
		_flashOnDamageTaken = settingsSaveState.FlashOnDamageTaken;
		_tooltipComplexity = settingsSaveState.TooltipComplexity;
		_dashVisual = settingsSaveState.DashVisual;
		_targeting = settingsSaveState.Targeting;
		ApplyLoadedSettings();
	}

	private void ApplyLoadedSettings()
	{
		UpdateCooldownVisuals(_cooldownVisuals);
		UpdateShowDamageNumbers(_showDamageNumbers);
		UpdateShowHealthBars(_showHealthBars);
		UpdateMinimap(_minimapVisual);
		UpdateFlashOnDamageTaken(_flashOnDamageTaken);
		UpdateTooltipComplexity(_tooltipComplexity);
		UpdateDashVisual(_dashVisual);
		UpdateTargeting(_targeting);
	}

	public void FillSaveState(SettingsSaveState saveState)
	{
		saveState.SetGameplaySettings(_showDamageNumbers, _showHealthBars, _cooldownVisuals, _minimapVisual, _flashOnDamageTaken, _tooltipComplexity, _dashVisual, _targeting);
	}
}
