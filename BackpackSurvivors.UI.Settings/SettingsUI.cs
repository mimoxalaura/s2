using System;
using System.Collections;
using System.Diagnostics;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Scenes;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.UI.Shared;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Settings;

public class SettingsUI : ModalUI
{
	[Header("Audio")]
	[SerializeField]
	private Button _buttonAudio;

	[SerializeField]
	private GameObject _audioPanel;

	[SerializeField]
	private Slider _masterSlider;

	[SerializeField]
	private TextMeshProUGUI _masterText;

	[SerializeField]
	private Toggle _masterToggle;

	[SerializeField]
	private Slider _musicSlider;

	[SerializeField]
	private TextMeshProUGUI _musicText;

	[SerializeField]
	private Toggle _musicToggle;

	[SerializeField]
	private Slider _sfxSlider;

	[SerializeField]
	private TextMeshProUGUI _sfxText;

	[SerializeField]
	private Toggle _sfxToggle;

	[SerializeField]
	private Slider _ambienceSlider;

	[SerializeField]
	private TextMeshProUGUI _ambienceText;

	[SerializeField]
	private Toggle _ambienceToggle;

	[Header("Video")]
	[SerializeField]
	private Button _buttonVideo;

	[SerializeField]
	private GameObject _videoPanel;

	[SerializeField]
	private GameObject _videoPanelDisabledOutsideMainMenu;

	[SerializeField]
	private TMP_Dropdown _dropdownResolution;

	[SerializeField]
	private TMP_Dropdown _dropdownWindowMode;

	[SerializeField]
	private TMP_Dropdown _dropdownCameraShake;

	[SerializeField]
	private TMP_Dropdown _dropdownWorldDetail;

	[SerializeField]
	private TMP_Dropdown _dropdownBloom;

	[SerializeField]
	private TMP_InputField _textMaxFps;

	[SerializeField]
	private TMP_Dropdown _dropdownShaders;

	[Header("Gameplay")]
	[SerializeField]
	private Button _buttonGameplay;

	[SerializeField]
	private GameObject _gameplayPanel;

	[SerializeField]
	private TMP_Dropdown _dropdownShowDamageNumbers;

	[SerializeField]
	private TMP_Dropdown _dropdownShowHealthBars;

	[SerializeField]
	private TMP_Dropdown _dropdownCooldownVisuals;

	[SerializeField]
	private TMP_Dropdown _dropdownMinimapVisuals;

	[SerializeField]
	private TMP_Dropdown _dropdownFlashOnDamageTaken;

	[SerializeField]
	private TMP_Dropdown _dropdownTooltipComplexity;

	[SerializeField]
	private TMP_Dropdown _dropdownDashVisuals;

	[SerializeField]
	private TMP_Dropdown _dropdownTargeting;

	private const int _maxFPSLowerBound = 20;

	private const int _maxFPSUpperBound = 240;

	private AudioSettingsController _newAudioSettingsControllerValues;

	private VideoSettingsController _newVideoSettingsControllerValues;

	private GameplaySettingsController _newGameplaySettingsControllerValues;

	private AudioSettingsController _originalAudioSettingsControllerValues;

	private VideoSettingsController _originalVideoSettingsControllerValues;

	private GameplaySettingsController _originalGameplaySettingsControllerValues;

	private bool _preventNextUnmuteEvent;

	private bool _videoSettingsChanged;

	private bool _closeUIAfterFinishing;

	private void Start()
	{
		_buttonAudio.onClick.AddListener(ShowAudio);
		_buttonVideo.onClick.AddListener(ShowVideo);
		_buttonGameplay.onClick.AddListener(ShowGameplay);
		_dropdownResolution.onValueChanged.RemoveAllListeners();
		_dropdownWindowMode.onValueChanged.RemoveAllListeners();
		_textMaxFps.onValueChanged.RemoveAllListeners();
		SetupAudioSettings();
		SetupVideoSettings();
		SetupGameplaySettings();
		CloneOriginalControllerValues();
	}

	private void CloneOriginalControllerValues()
	{
		_newAudioSettingsControllerValues = new AudioSettingsController();
		_newAudioSettingsControllerValues.UpdateMasterVolume(SingletonController<SettingsController>.Instance.AudioSettingsController.MasterVolume);
		_newAudioSettingsControllerValues.UpdateAmbienceVolume(SingletonController<SettingsController>.Instance.AudioSettingsController.AmbienceVolume);
		_newAudioSettingsControllerValues.UpdateMusicVolume(SingletonController<SettingsController>.Instance.AudioSettingsController.MusicVolume);
		_newAudioSettingsControllerValues.UpdateSfxVolume(SingletonController<SettingsController>.Instance.AudioSettingsController.SfxVolume);
		_newVideoSettingsControllerValues = new VideoSettingsController();
		_newVideoSettingsControllerValues.UpdateMaxFPS(SingletonController<SettingsController>.Instance.VideoSettingsController.MaxFPS);
		_newVideoSettingsControllerValues.UpdateResolution(SingletonController<SettingsController>.Instance.VideoSettingsController.Resolution);
		_newVideoSettingsControllerValues.UpdateWindowMode(SingletonController<SettingsController>.Instance.VideoSettingsController.Windowmodes);
		_newVideoSettingsControllerValues.UpdateCameraShake(SingletonController<SettingsController>.Instance.VideoSettingsController.CameraShake);
		_newGameplaySettingsControllerValues = new GameplaySettingsController();
		_newGameplaySettingsControllerValues.UpdateShowHealthBars(SingletonController<SettingsController>.Instance.GameplaySettingsController.ShowHealthBars);
		_newGameplaySettingsControllerValues.UpdateCooldownVisuals(SingletonController<SettingsController>.Instance.GameplaySettingsController.CooldownVisuals);
		_newGameplaySettingsControllerValues.UpdateShowDamageNumbers(SingletonController<SettingsController>.Instance.GameplaySettingsController.ShowDamageNumbers);
		_newGameplaySettingsControllerValues.UpdateMinimap(SingletonController<SettingsController>.Instance.GameplaySettingsController.MinimapVisual);
		_newGameplaySettingsControllerValues.UpdateFlashOnDamageTaken(SingletonController<SettingsController>.Instance.GameplaySettingsController.FlashOnDamageTaken);
		_newGameplaySettingsControllerValues.UpdateDashVisual(SingletonController<SettingsController>.Instance.GameplaySettingsController.DashVisual);
		_newGameplaySettingsControllerValues.UpdateTargeting(SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting);
		_originalAudioSettingsControllerValues = new AudioSettingsController();
		_originalAudioSettingsControllerValues.UpdateMasterVolume(SingletonController<SettingsController>.Instance.AudioSettingsController.MasterVolume);
		_originalAudioSettingsControllerValues.UpdateAmbienceVolume(SingletonController<SettingsController>.Instance.AudioSettingsController.AmbienceVolume);
		_originalAudioSettingsControllerValues.UpdateMusicVolume(SingletonController<SettingsController>.Instance.AudioSettingsController.MusicVolume);
		_originalAudioSettingsControllerValues.UpdateSfxVolume(SingletonController<SettingsController>.Instance.AudioSettingsController.SfxVolume);
		_originalVideoSettingsControllerValues = new VideoSettingsController();
		_originalVideoSettingsControllerValues.UpdateMaxFPS(SingletonController<SettingsController>.Instance.VideoSettingsController.MaxFPS);
		_originalVideoSettingsControllerValues.UpdateResolution(SingletonController<SettingsController>.Instance.VideoSettingsController.Resolution);
		_originalVideoSettingsControllerValues.UpdateWindowMode(SingletonController<SettingsController>.Instance.VideoSettingsController.Windowmodes);
		_originalGameplaySettingsControllerValues = new GameplaySettingsController();
		_originalGameplaySettingsControllerValues.UpdateShowHealthBars(SingletonController<SettingsController>.Instance.GameplaySettingsController.ShowHealthBars);
		_originalGameplaySettingsControllerValues.UpdateCooldownVisuals(SingletonController<SettingsController>.Instance.GameplaySettingsController.CooldownVisuals);
		_originalGameplaySettingsControllerValues.UpdateShowDamageNumbers(SingletonController<SettingsController>.Instance.GameplaySettingsController.ShowDamageNumbers);
		_originalGameplaySettingsControllerValues.UpdateMinimap(SingletonController<SettingsController>.Instance.GameplaySettingsController.MinimapVisual);
		_originalGameplaySettingsControllerValues.UpdateFlashOnDamageTaken(SingletonController<SettingsController>.Instance.GameplaySettingsController.FlashOnDamageTaken);
		_originalGameplaySettingsControllerValues.UpdateTooltipComplexity(SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity);
		_originalGameplaySettingsControllerValues.UpdateDashVisual(SingletonController<SettingsController>.Instance.GameplaySettingsController.DashVisual);
		_originalGameplaySettingsControllerValues.UpdateTargeting(SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting);
	}

	private void SetupKeyBinds()
	{
		SingletonController<InputController>.Instance.OnCancelHandler += InputController_OnCancelHandler;
		SingletonController<InputController>.Instance.OnAcceptHandler += InputController_OnAcceptHandler;
		SingletonController<InputController>.Instance.OnNextHandler += InputController_OnNextHandler;
		SingletonController<InputController>.Instance.OnPreviousHandler += InputController_OnPreviousHandler;
	}

	private void InputController_OnPreviousHandler(object sender, EventArgs e)
	{
		RevertButton();
	}

	private void InputController_OnNextHandler(object sender, EventArgs e)
	{
		ApplyButton();
	}

	private void InputController_OnAcceptHandler(object sender, EventArgs e)
	{
		AcceptButton();
	}

	private void InputController_OnCancelHandler(object sender, EventArgs e)
	{
		Cancel(shouldExecuteCloseButton: false);
	}

	public void RevertButton()
	{
		RevertChanges();
	}

	public void ApplyButton()
	{
		ApplyChanges();
		if (_videoSettingsChanged)
		{
			ShowGameRestartPopup(closeUIAfterFinishing: false);
		}
	}

	public void CancelButton()
	{
		Cancel();
	}

	private void Cancel(bool shouldExecuteCloseButton = true)
	{
		SingletonController<SettingsController>.Instance.LoadSavedSettings();
		if (shouldExecuteCloseButton)
		{
			SingletonController<SettingsController>.Instance.ClickedCloseButton();
		}
	}

	public void AcceptButton()
	{
		ApplyChanges();
		if (_videoSettingsChanged)
		{
			ShowGameRestartPopup(closeUIAfterFinishing: true);
		}
		else
		{
			SingletonController<SettingsController>.Instance.ClickedCloseButton();
		}
	}

	private void ApplyChanges()
	{
		SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateMasterVolume(_newAudioSettingsControllerValues.MasterVolume);
		SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateAmbienceVolume(_newAudioSettingsControllerValues.AmbienceVolume);
		SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateMusicVolume(_newAudioSettingsControllerValues.MusicVolume);
		SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateSfxVolume(_newAudioSettingsControllerValues.SfxVolume);
		SingletonController<SettingsController>.Instance.VideoSettingsController.UpdateMaxFPS(_newVideoSettingsControllerValues.MaxFPS);
		SingletonController<SettingsController>.Instance.VideoSettingsController.UpdateResolution(_newVideoSettingsControllerValues.Resolution);
		SingletonController<SettingsController>.Instance.VideoSettingsController.UpdateWindowMode(_newVideoSettingsControllerValues.Windowmodes);
		SingletonController<SettingsController>.Instance.VideoSettingsController.UpdateCameraShake(_newVideoSettingsControllerValues.CameraShake);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateShowHealthBars(_newGameplaySettingsControllerValues.ShowHealthBars);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateCooldownVisuals(_newGameplaySettingsControllerValues.CooldownVisuals);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateShowDamageNumbers(_newGameplaySettingsControllerValues.ShowDamageNumbers);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateMinimap(_newGameplaySettingsControllerValues.MinimapVisual);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateFlashOnDamageTaken(_newGameplaySettingsControllerValues.FlashOnDamageTaken);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateTooltipComplexity(_newGameplaySettingsControllerValues.TooltipComplexity);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateDashVisual(_newGameplaySettingsControllerValues.DashVisual);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateTargeting(_newGameplaySettingsControllerValues.Targeting);
	}

	private void RevertChanges()
	{
		SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateMasterVolume(_originalAudioSettingsControllerValues.MasterVolume);
		SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateAmbienceVolume(_originalAudioSettingsControllerValues.AmbienceVolume);
		SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateMusicVolume(_originalAudioSettingsControllerValues.MusicVolume);
		SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateSfxVolume(_originalAudioSettingsControllerValues.SfxVolume);
		SingletonController<SettingsController>.Instance.VideoSettingsController.UpdateMaxFPS(_originalVideoSettingsControllerValues.MaxFPS);
		SingletonController<SettingsController>.Instance.VideoSettingsController.UpdateResolution(_originalVideoSettingsControllerValues.Resolution);
		SingletonController<SettingsController>.Instance.VideoSettingsController.UpdateWindowMode(_originalVideoSettingsControllerValues.Windowmodes);
		SingletonController<SettingsController>.Instance.VideoSettingsController.UpdateCameraShake(_originalVideoSettingsControllerValues.CameraShake);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateShowHealthBars(_originalGameplaySettingsControllerValues.ShowHealthBars);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateCooldownVisuals(_originalGameplaySettingsControllerValues.CooldownVisuals);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateShowDamageNumbers(_originalGameplaySettingsControllerValues.ShowDamageNumbers);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateMinimap(_originalGameplaySettingsControllerValues.MinimapVisual);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateFlashOnDamageTaken(_originalGameplaySettingsControllerValues.FlashOnDamageTaken);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateTooltipComplexity(_originalGameplaySettingsControllerValues.TooltipComplexity);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateDashVisual(_originalGameplaySettingsControllerValues.DashVisual);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.UpdateTargeting(_originalGameplaySettingsControllerValues.Targeting);
		SingletonController<SettingsController>.Instance.LoadSavedSettings();
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		TogglePanel(Enums.SettingsPanels.Audio);
		_buttonAudio.GetComponent<SettingsTabButton>().OnClick();
		SetupKeyBinds();
		SingletonController<SettingsController>.Instance.LoadSavedSettings();
		CloneOriginalControllerValues();
		base.OpenUI(openDirection);
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		RemoveKeyBindings();
		base.CloseUI(openDirection);
	}

	private void RemoveKeyBindings()
	{
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
		SingletonController<InputController>.Instance.OnAcceptHandler -= InputController_OnAcceptHandler;
		SingletonController<InputController>.Instance.OnNextHandler -= InputController_OnNextHandler;
		SingletonController<InputController>.Instance.OnPreviousHandler -= InputController_OnPreviousHandler;
	}

	public void TogglePanel(Enums.SettingsPanels settingsPanel)
	{
		SetAllPanelsInactive();
		switch (settingsPanel)
		{
		case Enums.SettingsPanels.Audio:
			_audioPanel.SetActive(value: true);
			break;
		case Enums.SettingsPanels.Video:
			if (CurrentlyInMainMenu())
			{
				_videoPanel.SetActive(value: true);
			}
			else
			{
				_videoPanelDisabledOutsideMainMenu.SetActive(value: true);
			}
			break;
		case Enums.SettingsPanels.Gameplay:
			_gameplayPanel.SetActive(value: true);
			break;
		}
	}

	private void SetAllPanelsInactive()
	{
		_audioPanel.SetActive(value: false);
		_videoPanelDisabledOutsideMainMenu.SetActive(value: false);
		_videoPanel.SetActive(value: false);
		_gameplayPanel.SetActive(value: false);
	}

	private bool CurrentlyInMainMenu()
	{
		SceneInfo sceneInfo = UnityEngine.Object.FindObjectOfType<SceneInfo>();
		if (sceneInfo == null)
		{
			return false;
		}
		return sceneInfo.SceneType == Enums.SceneType.MainMenu;
	}

	private void SetupAudioSettings()
	{
		_masterSlider.onValueChanged.AddListener(MasterAudioValueChanged);
		_musicSlider.onValueChanged.AddListener(MusicAudioValueChanged);
		_sfxSlider.onValueChanged.AddListener(SfxAudioValueChanged);
		_ambienceSlider.onValueChanged.AddListener(AmbienceAudioValueChanged);
		_masterToggle.onValueChanged.AddListener(MuteMasterAudioToggled);
		_musicToggle.onValueChanged.AddListener(MuteMusicAudioToggled);
		_sfxToggle.onValueChanged.AddListener(MuteSfxAudioToggled);
		_ambienceToggle.onValueChanged.AddListener(MuteAmbienceAudioToggled);
	}

	internal void MasterAudioValueChanged(float newValue)
	{
		_masterText.SetText($"{newValue * 100f:n0}%");
		_newAudioSettingsControllerValues.UpdateMasterVolume(newValue);
		SetMuteButtonCheckedState(_masterToggle, newValue);
	}

	internal void MusicAudioValueChanged(float newValue)
	{
		_musicText.SetText($"{newValue * 100f:n0}%");
		_newAudioSettingsControllerValues.UpdateMusicVolume(newValue);
		SetMuteButtonCheckedState(_musicToggle, newValue);
	}

	internal void SfxAudioValueChanged(float newValue)
	{
		_sfxText.SetText($"{newValue * 100f:n0}%");
		_newAudioSettingsControllerValues.UpdateSfxVolume(newValue);
		SetMuteButtonCheckedState(_sfxToggle, newValue);
	}

	internal void AmbienceAudioValueChanged(float newValue)
	{
		_ambienceText.SetText($"{newValue * 100f:n0}%");
		_newAudioSettingsControllerValues.UpdateAmbienceVolume(newValue);
		SetMuteButtonCheckedState(_ambienceToggle, newValue);
	}

	internal void SetMasterAudioFromSave(float value)
	{
		_masterText.SetText($"{value * 100f:n0}%");
		_masterSlider.value = value;
		SetMuteButtonCheckedState(_masterToggle, value);
	}

	internal void SetMusicAudioFromSave(float value)
	{
		_musicText.SetText($"{value * 100f:n0}%");
		_musicSlider.value = value;
		SetMuteButtonCheckedState(_musicToggle, value);
	}

	internal void SetSFXAudioFromSave(float value)
	{
		_sfxText.SetText($"{value * 100f:n0}%");
		_sfxSlider.value = value;
		SetMuteButtonCheckedState(_sfxToggle, value);
	}

	internal void SetAmbienceAudioFromSave(float value)
	{
		_ambienceText.SetText($"{value * 100f:n0}%");
		_ambienceSlider.value = value;
		SetMuteButtonCheckedState(_ambienceToggle, value);
	}

	private void MuteMasterAudioToggled(bool isMuted)
	{
		if (_preventNextUnmuteEvent)
		{
			_preventNextUnmuteEvent = false;
		}
		else
		{
			_masterSlider.value = (isMuted ? 0f : 0.5f);
		}
	}

	private void MuteMusicAudioToggled(bool isMuted)
	{
		if (_preventNextUnmuteEvent)
		{
			_preventNextUnmuteEvent = false;
		}
		else
		{
			_musicSlider.value = (isMuted ? 0f : 0.5f);
		}
	}

	private void MuteSfxAudioToggled(bool isMuted)
	{
		if (_preventNextUnmuteEvent)
		{
			_preventNextUnmuteEvent = false;
		}
		else
		{
			_sfxSlider.value = (isMuted ? 0f : 0.5f);
		}
	}

	private void MuteAmbienceAudioToggled(bool isMuted)
	{
		if (_preventNextUnmuteEvent)
		{
			_preventNextUnmuteEvent = false;
		}
		else
		{
			_ambienceSlider.value = (isMuted ? 0f : 0.5f);
		}
	}

	private void SetMuteButtonCheckedState(Toggle muteCheckbox, float volume)
	{
		bool flag = volume <= float.Epsilon;
		bool preventNextUnmuteEvent = !flag && muteCheckbox.isOn;
		_preventNextUnmuteEvent = preventNextUnmuteEvent;
		muteCheckbox.isOn = flag;
	}

	private void ShowAudio()
	{
		TogglePanel(Enums.SettingsPanels.Audio);
	}

	private void SetupVideoSettings()
	{
		foreach (Enums.Resolutions value in Enum.GetValues(typeof(Enums.Resolutions)))
		{
			_dropdownResolution.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value)
			});
		}
		_dropdownResolution.value = 0;
		_dropdownResolution.RefreshShownValue();
		_dropdownResolution.onValueChanged.AddListener(ResolutionsValueChanged);
		foreach (Enums.Windowmodes value2 in Enum.GetValues(typeof(Enums.Windowmodes)))
		{
			_dropdownWindowMode.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value2)
			});
		}
		_dropdownWindowMode.value = 0;
		_dropdownWindowMode.RefreshShownValue();
		_dropdownWindowMode.onValueChanged.AddListener(WindowModeValueChanged);
		foreach (Enums.CameraShake value3 in Enum.GetValues(typeof(Enums.CameraShake)))
		{
			_dropdownCameraShake.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value3)
			});
		}
		_dropdownCameraShake.value = 0;
		_dropdownCameraShake.RefreshShownValue();
		_dropdownCameraShake.onValueChanged.AddListener(CameraShakeValueChanged);
		_textMaxFps.onEndEdit.AddListener(WorldMaxFPSValueChanged);
	}

	internal void SetResolutionFromSave(int value)
	{
		_dropdownResolution.value = value;
		_dropdownResolution.RefreshShownValue();
	}

	internal void SetWindowedModeFromSave(int value)
	{
		_dropdownWindowMode.value = value;
		_dropdownWindowMode.RefreshShownValue();
	}

	internal void SetMaxFPSFromSave(int maxFps)
	{
		_textMaxFps.text = maxFps.ToString();
	}

	internal void SetCameraShakeFromSave(int cameraShake)
	{
		_dropdownCameraShake.value = cameraShake;
		_dropdownCameraShake.RefreshShownValue();
	}

	private void ResolutionsValueChanged(int newIndex)
	{
		if (SingletonController<SettingsController>.Instance.IsInitialized)
		{
			_newVideoSettingsControllerValues.UpdateResolution((Enums.Resolutions)newIndex);
			if (_originalVideoSettingsControllerValues.Resolution != _newVideoSettingsControllerValues.Resolution)
			{
				_videoSettingsChanged = true;
			}
		}
	}

	private void ShowGameRestartPopup(bool closeUIAfterFinishing)
	{
		_closeUIAfterFinishing = closeUIAfterFinishing;
		SingletonController<GenericPopupController>.Instance.CreatePopup(Enums.GenericPopupLocation.MiddleOfScreen, "Restart needed!", "Changing the video settings requires a restart. Do you want to restart the game now?", Enums.GenericPopupButtons.Cancel, Enums.GenericPopupButtons.Cancel, closeOnCancel: false, null);
		SingletonController<GenericPopupController>.Instance.ClearEvents();
		SingletonController<GenericPopupController>.Instance.OnPopupButtonYesClicked += ResolutionChangedPopup_OnPopupButtonYesClicked;
		SingletonController<GenericPopupController>.Instance.OnPopupButtonCancelClicked += ResolutionChangedPopup_OnPopupButtonCancelClicked;
		SingletonController<GenericPopupController>.Instance.ShowPopup();
	}

	private void ResolutionChangedPopup_OnPopupButtonCancelClicked(object sender, EventArgs e)
	{
		SaveSettings();
		if (_closeUIAfterFinishing)
		{
			SingletonController<SettingsController>.Instance.ClickedCloseButton();
		}
	}

	private void ResolutionChangedPopup_OnPopupButtonYesClicked(object sender, EventArgs e)
	{
		SaveSettings();
		RestartGame();
	}

	private void SaveSettings()
	{
		ApplyChanges();
		SingletonController<SettingsController>.Instance.SaveSettings();
	}

	private IEnumerator RestartGameAsync()
	{
		yield return new WaitForSecondsRealtime(0.1f);
		if (SteamManager.Initialized)
		{
			if (SingletonController<GameDatabase>.Instance._isDemo)
			{
				Process.Start("steam://rungameid/" + SingletonController<GameDatabase>.Instance.GetDemoAppId());
			}
			else
			{
				Process.Start("steam://rungameid/" + SteamUtils.GetAppID().ToString());
			}
			Application.Quit();
			yield break;
		}
		string text = Application.dataPath;
		if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			text += "/../";
		}
		Process.Start(text + "/" + Application.productName + ".exe");
		Application.Quit();
	}

	private void RestartGame()
	{
		SingletonController<GenericPopupController>.Instance.ClearEvents();
		SingletonController<GenericPopupController>.Instance.ClosePopup();
		StartCoroutine(RestartGameAsync());
	}

	private void WindowModeValueChanged(int newIndex)
	{
		if (SingletonController<SettingsController>.Instance.IsInitialized)
		{
			_newVideoSettingsControllerValues.UpdateWindowMode((Enums.Windowmodes)newIndex);
			if (_originalVideoSettingsControllerValues.Windowmodes != _newVideoSettingsControllerValues.Windowmodes)
			{
				_videoSettingsChanged = true;
			}
		}
	}

	private void CameraShakeValueChanged(int newIndex)
	{
		_newVideoSettingsControllerValues.UpdateCameraShake((Enums.CameraShake)newIndex);
	}

	private void WorldDetailValueChanged(int newIndex)
	{
		_newVideoSettingsControllerValues.UpdateWorldDetail((Enums.WorldDetail)newIndex);
	}

	private void BloomValueChanged(int newIndex)
	{
		_newVideoSettingsControllerValues.UpdateBloom((Enums.Bloom)newIndex);
	}

	private void WorldMaxFPSValueChanged(string newMaxFPS)
	{
		if (!string.IsNullOrEmpty(newMaxFPS))
		{
			int value = Convert.ToInt32(newMaxFPS);
			value = Mathf.Clamp(value, 20, 240);
			_textMaxFps.text = value.ToString();
			_newVideoSettingsControllerValues.UpdateMaxFPS(value);
		}
	}

	private void ShowVideo()
	{
		TogglePanel(Enums.SettingsPanels.Video);
	}

	private void SetupGameplaySettings()
	{
		foreach (Enums.ShowDamageNumbers value in Enum.GetValues(typeof(Enums.ShowDamageNumbers)))
		{
			_dropdownShowDamageNumbers.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value)
			});
		}
		_dropdownShowDamageNumbers.value = 0;
		_dropdownShowDamageNumbers.RefreshShownValue();
		_dropdownShowDamageNumbers.onValueChanged.AddListener(DamageNumbersValueChanged);
		foreach (Enums.ShowHealthBars value2 in Enum.GetValues(typeof(Enums.ShowHealthBars)))
		{
			_dropdownShowHealthBars.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value2)
			});
		}
		_dropdownShowHealthBars.value = 0;
		_dropdownShowHealthBars.RefreshShownValue();
		_dropdownShowHealthBars.onValueChanged.AddListener(ShowHealthBarsValueChanged);
		foreach (Enums.CooldownVisuals value3 in Enum.GetValues(typeof(Enums.CooldownVisuals)))
		{
			_dropdownCooldownVisuals.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value3)
			});
		}
		_dropdownCooldownVisuals.value = 0;
		_dropdownCooldownVisuals.RefreshShownValue();
		_dropdownCooldownVisuals.onValueChanged.AddListener(CooldownVisualsValueChanged);
		foreach (Enums.MinimapVisual value4 in Enum.GetValues(typeof(Enums.MinimapVisual)))
		{
			_dropdownMinimapVisuals.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value4)
			});
		}
		_dropdownMinimapVisuals.value = 0;
		_dropdownMinimapVisuals.RefreshShownValue();
		_dropdownMinimapVisuals.onValueChanged.AddListener(MinimapVisualsValueChanged);
		foreach (Enums.TooltipComplexity value5 in Enum.GetValues(typeof(Enums.TooltipComplexity)))
		{
			_dropdownTooltipComplexity.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value5)
			});
		}
		_dropdownTooltipComplexity.value = 0;
		_dropdownTooltipComplexity.RefreshShownValue();
		_dropdownTooltipComplexity.onValueChanged.AddListener(TooltipComplexityValueChanged);
		foreach (Enums.FlashOnDamageTaken value6 in Enum.GetValues(typeof(Enums.FlashOnDamageTaken)))
		{
			_dropdownFlashOnDamageTaken.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value6)
			});
		}
		_dropdownFlashOnDamageTaken.value = 0;
		_dropdownFlashOnDamageTaken.RefreshShownValue();
		_dropdownFlashOnDamageTaken.onValueChanged.AddListener(FlashOnDamageTakenValueChanged);
		foreach (Enums.DashVisual value7 in Enum.GetValues(typeof(Enums.DashVisual)))
		{
			_dropdownDashVisuals.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value7)
			});
		}
		_dropdownDashVisuals.value = 0;
		_dropdownDashVisuals.RefreshShownValue();
		_dropdownDashVisuals.onValueChanged.AddListener(DashVisualValueChanged);
		foreach (Enums.Targeting value8 in Enum.GetValues(typeof(Enums.Targeting)))
		{
			_dropdownTargeting.options.Add(new TMP_Dropdown.OptionData
			{
				text = StringHelper.GetCleanString(value8)
			});
		}
		_dropdownTargeting.value = 0;
		_dropdownTargeting.RefreshShownValue();
		_dropdownTargeting.onValueChanged.AddListener(TargetingValueChanged);
	}

	internal void SetShowDamageNumbersFromSave(int value)
	{
		_dropdownShowDamageNumbers.value = value;
		_dropdownShowDamageNumbers.RefreshShownValue();
	}

	internal void SetHealthBarsFromSave(int value)
	{
		_dropdownShowHealthBars.value = value;
		_dropdownShowHealthBars.RefreshShownValue();
	}

	internal void SetCooldownVisualsFromSave(int value)
	{
		_dropdownCooldownVisuals.value = value;
		_dropdownCooldownVisuals.RefreshShownValue();
	}

	internal void SetMinimapVisualsFromSave(int value)
	{
		_dropdownMinimapVisuals.value = value;
		_dropdownMinimapVisuals.RefreshShownValue();
	}

	internal void SetTooltipComplexityFromSave(int value)
	{
		_dropdownTooltipComplexity.value = value;
		_dropdownTooltipComplexity.RefreshShownValue();
	}

	internal void SetDashVisualFromSave(int value)
	{
		_dropdownDashVisuals.value = value;
		_dropdownDashVisuals.RefreshShownValue();
	}

	internal void SetTargetingFromSave(int value)
	{
		_dropdownTargeting.value = value;
		_dropdownTargeting.RefreshShownValue();
	}

	internal void SetFlashOnDamageTakenFromSave(int value)
	{
		_dropdownFlashOnDamageTaken.value = value;
		_dropdownFlashOnDamageTaken.RefreshShownValue();
	}

	private void DamageNumbersValueChanged(int newIndex)
	{
		_newGameplaySettingsControllerValues.UpdateShowDamageNumbers((Enums.ShowDamageNumbers)newIndex);
	}

	private void ShowHealthBarsValueChanged(int newIndex)
	{
		_newGameplaySettingsControllerValues.UpdateShowHealthBars((Enums.ShowHealthBars)newIndex);
	}

	private void CooldownVisualsValueChanged(int newIndex)
	{
		_newGameplaySettingsControllerValues.UpdateCooldownVisuals((Enums.CooldownVisuals)newIndex);
	}

	private void MinimapVisualsValueChanged(int newIndex)
	{
		_newGameplaySettingsControllerValues.UpdateMinimap((Enums.MinimapVisual)newIndex);
	}

	private void FlashOnDamageTakenValueChanged(int newIndex)
	{
		_newGameplaySettingsControllerValues.UpdateFlashOnDamageTaken((Enums.FlashOnDamageTaken)newIndex);
	}

	private void DashVisualValueChanged(int newIndex)
	{
		_newGameplaySettingsControllerValues.UpdateDashVisual((Enums.DashVisual)newIndex);
	}

	private void TargetingValueChanged(int newIndex)
	{
		_newGameplaySettingsControllerValues.UpdateTargeting((Enums.Targeting)newIndex);
	}

	private void TooltipComplexityValueChanged(int newIndex)
	{
		_newGameplaySettingsControllerValues.UpdateTooltipComplexity((Enums.TooltipComplexity)newIndex);
	}

	private void ShowGameplay()
	{
		TogglePanel(Enums.SettingsPanels.Gameplay);
	}
}
