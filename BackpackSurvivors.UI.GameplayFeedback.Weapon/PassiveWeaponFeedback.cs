using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Combat.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.System.Settings.Events;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback.Weapon;

public class PassiveWeaponFeedback : MonoBehaviour
{
	[SerializeField]
	private PassiveWeaponIcon _passiveWeaponIconPrefab;

	[SerializeField]
	private PassiveWeaponBar _passiveWeaponBarPrefab;

	[SerializeField]
	private Transform _passiveWeaponBarContainer;

	[SerializeField]
	private Transform _passiveWeaponIconContainer;

	private Dictionary<Guid, PassiveWeaponIcon> _activeIcons;

	private Dictionary<Guid, PassiveWeaponBar> _activeBars;

	private Dictionary<Guid, CombatWeapon> _passiveWeapons;

	private Enums.CooldownVisuals _cooldownVisuals;

	private WeaponController _weaponController;

	private void Start()
	{
		InitDictionaries();
		RegisterGameObjects();
		RegisterEvents();
		ToggleVisualization(SingletonController<SettingsController>.Instance.GameplaySettingsController.CooldownVisuals);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SettingsController>.Instance, RegisterSettingsControllerInitialized);
	}

	private void RegisterSettingsControllerInitialized()
	{
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnCooldownVisualsSettingChanged += GameplaySettingsController_OnCooldownVisualsSettingChanged;
	}

	private void GameplaySettingsController_OnCooldownVisualsSettingChanged(object sender, CooldownVisualSettingsChangedEventArgs e)
	{
		ToggleVisualization(e.CooldownVisuals);
	}

	private void RegisterEvents()
	{
		_weaponController.OnWeaponCooldownUpdate += CombatController_OnWeaponCooldownUpdate;
		_weaponController.OnWeaponReady += CombatController_OnWeaponReady;
		_weaponController.OnWeaponRegister += CombatController_OnWeaponRegister;
		_weaponController.OnWeaponsReset += CombatController_OnWeaponsReset;
	}

	private void UnregisterEvents()
	{
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnCooldownVisualsSettingChanged -= GameplaySettingsController_OnCooldownVisualsSettingChanged;
		_weaponController.OnWeaponCooldownUpdate -= CombatController_OnWeaponCooldownUpdate;
		_weaponController.OnWeaponReady -= CombatController_OnWeaponReady;
		_weaponController.OnWeaponRegister -= CombatController_OnWeaponRegister;
		_weaponController.OnWeaponsReset -= CombatController_OnWeaponsReset;
	}

	private void RegisterGameObjects()
	{
		_weaponController = UnityEngine.Object.FindAnyObjectByType<WeaponController>();
	}

	private void InitDictionaries()
	{
		_passiveWeapons = new Dictionary<Guid, CombatWeapon>();
		_activeBars = new Dictionary<Guid, PassiveWeaponBar>();
		_activeIcons = new Dictionary<Guid, PassiveWeaponIcon>();
	}

	[Command("weaponbar.change", Platform.AllPlatforms, MonoTargetType.Single)]
	public void ToggleVisualization(Enums.CooldownVisuals cooldownVisuals)
	{
		_cooldownVisuals = cooldownVisuals;
		switch (cooldownVisuals)
		{
		case Enums.CooldownVisuals.Bar:
			ChangeToBarMode();
			break;
		case Enums.CooldownVisuals.Icon:
			ChangeToIconMode();
			break;
		}
	}

	public void AddPassiveWeapon(CombatWeapon weapon)
	{
		_passiveWeapons.Add(weapon.UniqueWeaponKey, weapon);
		switch (_cooldownVisuals)
		{
		case Enums.CooldownVisuals.Bar:
			CreateSingleWeaponBar(weapon);
			break;
		case Enums.CooldownVisuals.Icon:
			CreateSingleWeaponIcon(weapon);
			break;
		}
	}

	public void ChangeToBarMode()
	{
		ClearBars();
		ClearIcons();
		foreach (KeyValuePair<Guid, CombatWeapon> passiveWeapon in _passiveWeapons)
		{
			CreateSingleWeaponBar(passiveWeapon.Value);
		}
	}

	private PassiveWeaponBar CreateSingleWeaponBar(CombatWeapon weapon)
	{
		PassiveWeaponBar passiveWeaponBar = UnityEngine.Object.Instantiate(_passiveWeaponBarPrefab, _passiveWeaponBarContainer);
		passiveWeaponBar.Init(weapon);
		_activeBars.Add(weapon.UniqueWeaponKey, passiveWeaponBar);
		return passiveWeaponBar;
	}

	public void ChangeToIconMode()
	{
		ClearBars();
		ClearIcons();
		foreach (KeyValuePair<Guid, CombatWeapon> passiveWeapon in _passiveWeapons)
		{
			CreateSingleWeaponIcon(passiveWeapon.Value);
		}
	}

	private PassiveWeaponIcon CreateSingleWeaponIcon(CombatWeapon weapon)
	{
		PassiveWeaponIcon passiveWeaponIcon = UnityEngine.Object.Instantiate(_passiveWeaponIconPrefab, _passiveWeaponIconContainer);
		passiveWeaponIcon.Init(weapon);
		_activeIcons.Add(weapon.UniqueWeaponKey, passiveWeaponIcon);
		return passiveWeaponIcon;
	}

	private void ClearIcons()
	{
		foreach (KeyValuePair<Guid, PassiveWeaponIcon> activeIcon in _activeIcons)
		{
			UnityEngine.Object.Destroy(activeIcon.Value.gameObject);
		}
		_activeIcons.Clear();
	}

	private void ClearBars()
	{
		foreach (KeyValuePair<Guid, PassiveWeaponBar> activeBar in _activeBars)
		{
			UnityEngine.Object.Destroy(activeBar.Value.gameObject);
		}
		_activeBars.Clear();
	}

	private void ClearWeapons()
	{
		foreach (KeyValuePair<Guid, CombatWeapon> passiveWeapon in _passiveWeapons)
		{
			UnityEngine.Object.Destroy(passiveWeapon.Value.gameObject);
		}
		_passiveWeapons.Clear();
	}

	private void CombatController_OnWeaponsReset(object sender, WeaponsResetEventArgs e)
	{
		ClearIcons();
		ClearBars();
		ClearWeapons();
	}

	private void CombatController_OnWeaponRegister(object sender, WeaponRegisterEventArgs e)
	{
		AddPassiveWeapon(e.CombatWeapon);
	}

	private void CombatController_OnWeaponReady(object sender, WeaponReadyEventArgs e)
	{
		switch (_cooldownVisuals)
		{
		case Enums.CooldownVisuals.Bar:
			if (_activeBars.ContainsKey(e.CombatWeapon.UniqueWeaponKey))
			{
				_activeBars[e.CombatWeapon.UniqueWeaponKey].SetReady(isReady: true);
			}
			break;
		case Enums.CooldownVisuals.Icon:
			if (_activeIcons.ContainsKey(e.CombatWeapon.UniqueWeaponKey))
			{
				_activeIcons[e.CombatWeapon.UniqueWeaponKey].SetReady(isReady: true);
			}
			break;
		}
	}

	private void CombatController_OnWeaponCooldownUpdate(object sender, WeaponCooldownUpdateEventArgs e)
	{
		switch (_cooldownVisuals)
		{
		case Enums.CooldownVisuals.Bar:
			if (_activeBars.ContainsKey(e.CombatWeapon.UniqueWeaponKey))
			{
				_activeBars[e.CombatWeapon.UniqueWeaponKey].UpdateValues(e.CombatWeapon);
			}
			break;
		case Enums.CooldownVisuals.Icon:
			if (_activeIcons.ContainsKey(e.CombatWeapon.UniqueWeaponKey))
			{
				_activeIcons[e.CombatWeapon.UniqueWeaponKey].UpdateValues(e.CombatWeapon);
			}
			break;
		}
	}

	private void OnDestroy()
	{
		UnregisterEvents();
		_activeIcons.Clear();
		_activeBars.Clear();
	}
}
