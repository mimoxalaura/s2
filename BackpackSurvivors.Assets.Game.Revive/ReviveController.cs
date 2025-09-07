using System;
using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.Assets.Game.Revive;

internal class ReviveController : BaseSingletonModalUIController<ReviveController>
{
	[SerializeField]
	private ReviveUI _reviveUI;

	[SerializeField]
	private GameObject _reviveBlackBackdrop;

	[SerializeField]
	private GameObject _reviveCountContainer;

	[SerializeField]
	private TextMeshProUGUI _reviveCount;

	private int _availableRevives;

	private int _spentRevives;

	private float _currentVolume;

	public int AvailableRevives => _availableRevives - _spentRevives;

	public int SpentRevives => _spentRevives;

	private void Start()
	{
		_availableRevives = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ExtraRevives);
		RegisterEvents();
		UpdateReviveCountUI();
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SceneChangeController>.Instance, RegisterSceneChangeControllerInitialized);
	}

	public void RegisterSceneChangeControllerInitialized()
	{
		SingletonController<SceneChangeController>.Instance.OnLevelSceneLoaded += SceneChangeController_OnLevelSceneLoaded;
	}

	private void SceneChangeController_OnLevelSceneLoaded(object sender, LevelSceneLoadedEventArgs e)
	{
		UpdateReviveCountUI();
	}

	public override void ClearAdventure()
	{
		_availableRevives = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ExtraRevives);
		_spentRevives = 0;
		UpdateReviveCountUI();
	}

	internal void UpdateReviveCountUI(bool forceHide = false, bool showUI = false)
	{
		if (forceHide)
		{
			_reviveCountContainer.SetActive(showUI);
		}
		else
		{
			_reviveCountContainer.SetActive(_availableRevives > 0);
		}
		_reviveCount.SetText($"{AvailableRevives}");
		if (_availableRevives > 0)
		{
			if (AvailableRevives > 0)
			{
				_reviveCount.color = Color.white;
			}
			else
			{
				_reviveCount.color = Color.red;
			}
		}
	}

	private void ReviveUI_OnGiveUpButtonPressed(object sender, EventArgs e)
	{
		_reviveBlackBackdrop.gameObject.SetActive(value: false);
		SingletonController<InputController>.Instance.SwitchToUIActionMap();
		SingletonController<GameController>.Instance.PlayerDied(delayUI: false);
		CloseUI();
	}

	private void ReviveUI_OnContinueButtonPressed(object sender, EventArgs e)
	{
		_spentRevives++;
		_reviveBlackBackdrop.gameObject.SetActive(value: false);
		SingletonController<InputController>.Instance.SwitchToIngameActionMap();
		SingletonController<GameController>.Instance.Player.Revive();
		UpdateReviveCountUI();
		CloseUI();
	}

	public void ShowReviveUI()
	{
		_currentVolume = SingletonController<AudioController>.Instance.GetVolume(Enums.AudioType.Music);
		SingletonController<AudioController>.Instance.SetVolume(Enums.AudioType.Music, _currentVolume / 3f);
		SetCamerasEnabled(enabled: true);
		_reviveUI.gameObject.SetActive(value: true);
		_reviveBlackBackdrop.gameObject.SetActive(value: true);
		_reviveUI.OnContinueButtonPressed += ReviveUI_OnContinueButtonPressed;
		_reviveUI.OnGiveUpButtonPressed += ReviveUI_OnGiveUpButtonPressed;
		_reviveUI.SetInformationValues(SingletonController<GameController>.Instance.Player.LastDamageSource);
		StartCoroutine(ShowReviveUIAsync());
	}

	private IEnumerator ShowReviveUIAsync()
	{
		yield return new WaitForSeconds(2f);
		_reviveUI.gameObject.SetActive(value: true);
		_reviveUI.OpenUI();
		SingletonController<InputController>.Instance.SwitchToUIActionMap();
	}

	public override void CloseUI()
	{
		base.CloseUI();
		_reviveUI.OnContinueButtonPressed -= ReviveUI_OnContinueButtonPressed;
		_reviveUI.OnGiveUpButtonPressed -= ReviveUI_OnGiveUpButtonPressed;
		SingletonController<AudioController>.Instance.SetVolume(Enums.AudioType.Music, _currentVolume);
		_reviveUI.CloseUI();
		_reviveUI.gameObject.SetActive(value: false);
		_reviveBlackBackdrop.gameObject.SetActive(value: false);
		UpdateReviveCountUI();
	}

	public override bool CloseOnCancelInput()
	{
		return false;
	}

	public override bool AudioOnOpen()
	{
		return false;
	}

	public override bool AudioOnClose()
	{
		return false;
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Revive;
	}

	public override void OpenUI()
	{
		base.OpenUI();
	}

	internal void SetReviveCountUIVisibility(bool visible)
	{
		UpdateReviveCountUI(forceHide: true, visible);
	}
}
