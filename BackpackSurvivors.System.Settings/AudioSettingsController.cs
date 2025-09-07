using System;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.System.Settings.Events;

namespace BackpackSurvivors.System.Settings;

[Serializable]
public class AudioSettingsController
{
	public delegate void VolumeChangedHandler(object sender, VolumeChangedEventArgs e);

	private float _masterVolume;

	private float _musicVolume;

	private float _sfxVolume;

	private float _ambienceVolume;

	public float MasterVolume => _masterVolume;

	public float MusicVolume => _musicVolume;

	public float SfxVolume => _sfxVolume;

	public float AmbienceVolume => _ambienceVolume;

	public event VolumeChangedHandler OnVolumeChanged;

	public bool UpdateMasterVolume(float masterVolume)
	{
		_masterVolume = masterVolume;
		if (this.OnVolumeChanged != null)
		{
			this.OnVolumeChanged(this, new VolumeChangedEventArgs(Enums.AudioType.Master, masterVolume));
		}
		return true;
	}

	public bool UpdateMusicVolume(float musicVolume)
	{
		_musicVolume = musicVolume;
		if (this.OnVolumeChanged != null)
		{
			this.OnVolumeChanged(this, new VolumeChangedEventArgs(Enums.AudioType.Music, musicVolume));
		}
		return true;
	}

	public bool UpdateSfxVolume(float sfxVolume)
	{
		_sfxVolume = sfxVolume;
		if (this.OnVolumeChanged != null)
		{
			this.OnVolumeChanged(this, new VolumeChangedEventArgs(Enums.AudioType.SFX, sfxVolume));
		}
		return true;
	}

	public bool UpdateAmbienceVolume(float ambienceVolume)
	{
		_ambienceVolume = ambienceVolume;
		if (this.OnVolumeChanged != null)
		{
			this.OnVolumeChanged(this, new VolumeChangedEventArgs(Enums.AudioType.Ambiance, ambienceVolume));
		}
		return true;
	}

	public void LoadSettingsFromSavegame(SettingsSaveState settingsSaveState)
	{
		_masterVolume = settingsSaveState.MasterVolume;
		_musicVolume = settingsSaveState.MusicVolume;
		_sfxVolume = settingsSaveState.SfxVolume;
		_ambienceVolume = settingsSaveState.AmbienceVolume;
		ApplyLoadedSettings();
	}

	private void ApplyLoadedSettings()
	{
		UpdateAmbienceVolume(_ambienceVolume);
		UpdateMasterVolume(_masterVolume);
		UpdateMusicVolume(_musicVolume);
		UpdateSfxVolume(_sfxVolume);
	}

	public void FillSaveState(SettingsSaveState saveState)
	{
		saveState.SetAudioSettingsState(_masterVolume, _musicVolume, _sfxVolume, _ambienceVolume);
	}
}
