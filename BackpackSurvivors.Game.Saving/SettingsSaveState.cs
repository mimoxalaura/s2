using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class SettingsSaveState : BaseSaveState
{
	public int MaxNoticeNumberSeen;

	public float MasterVolume { get; private set; }

	public float MusicVolume { get; private set; }

	public float SfxVolume { get; private set; }

	public float AmbienceVolume { get; private set; }

	public Enums.Resolutions Resolution { get; private set; }

	public Enums.Windowmodes Windowmodes { get; private set; }

	public Enums.WorldDetail WorldDetail { get; private set; }

	public Enums.Bloom Bloom { get; private set; }

	public int MaxFPS { get; private set; }

	public Enums.CameraShake CameraShake { get; private set; }

	public Enums.QualityShaders QualityShaders { get; private set; }

	public Enums.ShowDamageNumbers ShowDamageNumbers { get; private set; }

	public Enums.ShowHealthBars ShowHealthBars { get; private set; }

	public Enums.CooldownVisuals CooldownVisuals { get; private set; }

	public Enums.MinimapVisual MinimapVisual { get; private set; }

	public Enums.FlashOnDamageTaken FlashOnDamageTaken { get; private set; }

	public Enums.TooltipComplexity TooltipComplexity { get; private set; }

	public Enums.DashVisual DashVisual { get; private set; }

	public Enums.Targeting Targeting { get; private set; }

	public SettingsSaveState()
	{
		Init();
	}

	public void SetAudioSettingsState(float masterVolume, float musicVolume, float sfxVolume, float ambianceVolume)
	{
		MasterVolume = masterVolume;
		MusicVolume = musicVolume;
		SfxVolume = sfxVolume;
		AmbienceVolume = ambianceVolume;
	}

	public void SetVideoSettingsState(Enums.Resolutions resolution, Enums.Windowmodes windowmode, Enums.WorldDetail worldDetail, Enums.Bloom bloom, int maxFps, Enums.QualityShaders qualityShaders, Enums.CameraShake cameraShake)
	{
		Resolution = resolution;
		Windowmodes = windowmode;
		WorldDetail = worldDetail;
		CameraShake = cameraShake;
		Bloom = bloom;
		MaxFPS = maxFps;
		QualityShaders = qualityShaders;
	}

	public void SetGameplaySettings(Enums.ShowDamageNumbers showDamageNumbers, Enums.ShowHealthBars showHealthBars, Enums.CooldownVisuals cooldownVisuals, Enums.MinimapVisual minimapVisual, Enums.FlashOnDamageTaken flashOnDamageTaken, Enums.TooltipComplexity tooltipComplexity, Enums.DashVisual dashVisual, Enums.Targeting targeting)
	{
		ShowDamageNumbers = showDamageNumbers;
		ShowHealthBars = showHealthBars;
		CooldownVisuals = cooldownVisuals;
		MinimapVisual = minimapVisual;
		FlashOnDamageTaken = flashOnDamageTaken;
		TooltipComplexity = tooltipComplexity;
		DashVisual = dashVisual;
		Targeting = targeting;
	}

	public void Init()
	{
		MasterVolume = 0.5f;
		MusicVolume = 0.5f;
		SfxVolume = 0.5f;
		AmbienceVolume = 0.5f;
		Resolution = Enums.Resolutions._1920_x_1080;
		Windowmodes = Enums.Windowmodes.Windowed;
		WorldDetail = Enums.WorldDetail.Full;
		Bloom = Enums.Bloom.Enabled;
		MaxFPS = 60;
		QualityShaders = Enums.QualityShaders.Enabled;
	}

	public override bool HasData()
	{
		return false;
	}
}
