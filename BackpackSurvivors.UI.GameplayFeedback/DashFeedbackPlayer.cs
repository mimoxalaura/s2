using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.System.Settings.Events;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class DashFeedbackPlayer : MonoBehaviour
{
	[SerializeField]
	private DashElementPlayer _dashElementUIPrefabRight;

	[SerializeField]
	private GameObject _dashElementUIContainer;

	private List<DashElementPlayer> _activeDashElements;

	private void Start()
	{
		SingletonController<GameController>.Instance.Player.OnDashed += Player_OnDashed;
		SingletonController<GameController>.Instance.Player.OnDashCountChanged += Player_OnDashCountChanged;
		SingletonController<GameController>.Instance.Player.OnDashReady += Player_OnDashReady;
		_dashElementUIContainer.SetActive(SingletonController<SettingsController>.Instance.GameplaySettingsController.DashVisual == Enums.DashVisual.UnderPlayer || SingletonController<SettingsController>.Instance.GameplaySettingsController.DashVisual == Enums.DashVisual.Both);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnDashVisualSettingChanged += GameplaySettingsController_OnDashVisualSettingChanged;
		RefreshUI();
	}

	private void GameplaySettingsController_OnDashVisualSettingChanged(object sender, DashVisualSettingsChangedEventArgs e)
	{
		_dashElementUIContainer.SetActive(e.DashVisual == Enums.DashVisual.UnderPlayer || e.DashVisual == Enums.DashVisual.Both);
	}

	private void RefreshUI()
	{
		_activeDashElements = new List<DashElementPlayer>();
		int totalDashCount = SingletonController<GameController>.Instance.Player.TotalDashCount;
		for (int num = _dashElementUIContainer.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(_dashElementUIContainer.transform.GetChild(num).gameObject);
		}
		for (int i = 0; i < totalDashCount; i++)
		{
			DashElementPlayer dashElementPlayer = UnityEngine.Object.Instantiate(_dashElementUIPrefabRight, _dashElementUIContainer.transform);
			dashElementPlayer.Index = i;
			dashElementPlayer.IsReady = true;
			dashElementPlayer.ToggleReadyState(isReady: true);
			_activeDashElements.Add(dashElementPlayer);
		}
	}

	private void Player_OnDashCountChanged(object sender, DashCooldownEventArgs e)
	{
		RefreshUI();
	}

	private void Player_OnDashReady(object sender, EventArgs e)
	{
		DashElementPlayer dashElementOnReady = GetDashElementOnReady();
		if (!(dashElementOnReady == null))
		{
			dashElementOnReady.gameObject.SetActive(value: true);
			dashElementOnReady.ToggleReadyState(isReady: true);
		}
	}

	private void Player_OnDashed(object sender, DashCooldownEventArgs e)
	{
		DashElementPlayer dashElementToUseOnDash = GetDashElementToUseOnDash();
		if (!(dashElementToUseOnDash == null))
		{
			dashElementToUseOnDash.ToggleReadyState(isReady: false);
			dashElementToUseOnDash.gameObject.SetActive(value: false);
		}
	}

	private DashElementPlayer GetDashElementOnReady()
	{
		return (from x in _activeDashElements
			where !x.IsReady
			orderby x.Index
			select x).FirstOrDefault();
	}

	private DashElementPlayer GetDashElementToUseOnDash()
	{
		return (from x in _activeDashElements
			where x.IsReady
			orderby x.Index descending
			select x).FirstOrDefault();
	}

	private void OnDestroy()
	{
		SingletonController<GameController>.Instance.Player.OnDashed -= Player_OnDashed;
		SingletonController<GameController>.Instance.Player.OnDashCountChanged -= Player_OnDashCountChanged;
		SingletonController<GameController>.Instance.Player.OnDashReady -= Player_OnDashReady;
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnDashVisualSettingChanged -= GameplaySettingsController_OnDashVisualSettingChanged;
	}
}
