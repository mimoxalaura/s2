using System;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.System.Settings.Events;
using UnityEngine;

namespace BackpackSurvivors.System.Settings;

[Serializable]
public class VideoSettingsController
{
	public delegate void VideoSettingChangedHandler(object sender, VideoSettingsChangedEventArgs e);

	private Enums.Resolutions _resolution;

	private Enums.Windowmodes _windowmode;

	private Enums.WorldDetail _worldDetail;

	private Enums.Bloom _bloom;

	private int _maxFPS;

	private Enums.QualityShaders _qualityShader;

	private Enums.CameraShake _cameraShake;

	public Enums.Resolutions Resolution => _resolution;

	public Enums.Windowmodes Windowmodes => _windowmode;

	public Enums.WorldDetail WorldDetail => _worldDetail;

	public Enums.Bloom Bloom => _bloom;

	public int MaxFPS => _maxFPS;

	public Enums.QualityShaders QualityShader => _qualityShader;

	public Enums.CameraShake CameraShake => _cameraShake;

	public bool UpdateResolution(Enums.Resolutions resolution)
	{
		_resolution = resolution;
		return true;
	}

	public void ApplyVideoSettings()
	{
		GetResolution(out var horizontalResolution, out var verticalResolution);
		Screen.SetResolution(horizontalResolution, verticalResolution, GetCurrentFullScreenMode());
		Application.targetFrameRate = _maxFPS;
		QualitySettings.vSyncCount = 0;
	}

	private void GetResolution(out int horizontalResolution, out int verticalResolution)
	{
		horizontalResolution = 1920;
		verticalResolution = 1080;
		switch (_resolution)
		{
		case Enums.Resolutions._1920_x_1080:
			horizontalResolution = 1920;
			verticalResolution = 1080;
			break;
		case Enums.Resolutions._2560_x_1440:
			horizontalResolution = 2560;
			verticalResolution = 1440;
			break;
		case Enums.Resolutions._3840_x_2160:
			horizontalResolution = 3840;
			verticalResolution = 2160;
			break;
		case Enums.Resolutions._1128_x_634:
			horizontalResolution = 1128;
			verticalResolution = 634;
			break;
		case Enums.Resolutions._1280_x_720:
			horizontalResolution = 1280;
			verticalResolution = 720;
			break;
		case Enums.Resolutions._1366_x_766:
			horizontalResolution = 1366;
			verticalResolution = 766;
			break;
		case Enums.Resolutions._1600_x_900:
			horizontalResolution = 1600;
			verticalResolution = 900;
			break;
		case Enums.Resolutions._1760_x_990:
			horizontalResolution = 1760;
			verticalResolution = 990;
			break;
		}
	}

	internal FullScreenMode GetCurrentFullScreenMode()
	{
		return _windowmode switch
		{
			Enums.Windowmodes.Windowed => FullScreenMode.Windowed, 
			Enums.Windowmodes.WindowedBorderless => FullScreenMode.FullScreenWindow, 
			_ => FullScreenMode.FullScreenWindow, 
		};
	}

	public bool UpdateWindowMode(Enums.Windowmodes windowmode)
	{
		_windowmode = windowmode;
		return true;
	}

	public bool UpdateWorldDetail(Enums.WorldDetail worldDetail)
	{
		_worldDetail = worldDetail;
		return true;
	}

	public bool UpdateBloom(Enums.Bloom bloom)
	{
		_bloom = bloom;
		return true;
	}

	public bool UpdateMaxFPS(int maxFPS)
	{
		_maxFPS = maxFPS;
		return true;
	}

	public bool UpdateCameraShake(Enums.CameraShake cameraShake)
	{
		_cameraShake = cameraShake;
		return true;
	}

	public bool UpdateQualityShaders(Enums.QualityShaders qualityShader)
	{
		_qualityShader = qualityShader;
		return true;
	}

	public void LoadSettingsFromSavegame(SettingsSaveState settingsSaveState, bool applyVideoSettings)
	{
		_resolution = settingsSaveState.Resolution;
		_windowmode = settingsSaveState.Windowmodes;
		_worldDetail = settingsSaveState.WorldDetail;
		_bloom = settingsSaveState.Bloom;
		_maxFPS = settingsSaveState.MaxFPS;
		_qualityShader = settingsSaveState.QualityShaders;
		if (applyVideoSettings)
		{
			ApplyLoadedSettings();
		}
	}

	private void ApplyLoadedSettings()
	{
		UpdateBloom(_bloom);
		UpdateMaxFPS(_maxFPS);
		UpdateResolution(_resolution);
		UpdateQualityShaders(_qualityShader);
		UpdateWindowMode(_windowmode);
		UpdateWorldDetail(_worldDetail);
		ApplyVideoSettings();
	}

	public void FillSaveState(SettingsSaveState saveState)
	{
		saveState.SetVideoSettingsState(_resolution, _windowmode, _worldDetail, _bloom, _maxFPS, _qualityShader, _cameraShake);
	}
}
