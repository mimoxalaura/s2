using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.System;

public class AudioController : SingletonController<AudioController>
{
	private enum MusicSourceStatus
	{
		Stopped,
		Playing,
		FadingOut,
		FadingIn
	}

	[Header("Music")]
	[SerializeField]
	private AudioSource _musicSource1;

	[SerializeField]
	private AudioSource _musicSource2;

	[SerializeField]
	private float _fadeOutTime;

	[SerializeField]
	private float _fadeInTime;

	[SerializeField]
	private float _minimumTimeBetweenPlayingSameClip;

	private float _baseVolumeMusicSource1;

	private float _baseVolumeMusicSource2;

	[Header("Music")]
	[SerializeField]
	private AudioSource _ambianceSource;

	private float _baseVolumeAmbianceSource;

	[Header("SFX")]
	[SerializeField]
	private List<AudioSource> _sfxSources;

	[Header("Default")]
	[SerializeField]
	private SerializableDictionaryBase<Enums.DefaultAudioType, AudioClip> _defaultAudioClips;

	[SerializeField]
	private SerializableDictionaryBase<Enums.Enemies.EnemyOnHitType, AudioClip> _defaultOnHitAudioClips;

	[SerializeField]
	private SerializableDictionaryBase<Enums.PlaceableRarity, AudioClip> _defaultBackpackDropAudioOnVfx;

	[Header("DEBUG")]
	[SerializeField]
	private AudioClip _debugSound;

	[SerializeField]
	private Vector3 _leftLocation;

	[SerializeField]
	private Vector3 _rightLocation;

	private float _masterVolume = 1f;

	private float _musicVolume = 1f;

	private float _ambianceVolume = 1f;

	private float _sfxVolume = 1f;

	private float _maxDeltaTimeForFadingOut = 0.1f;

	private MusicSourceStatus _musicSource1Status;

	private MusicSourceStatus _musicSource2Status;

	private List<AudioClip> _audioClipsToPreventPlaying = new List<AudioClip>();

	public bool IsMusicPlaying
	{
		get
		{
			if (!_musicSource1.isPlaying)
			{
				return _musicSource2.isPlaying;
			}
			return true;
		}
	}

	private void Start()
	{
		base.IsInitialized = true;
	}

	public void SetFadeOutTime(float fadeOutTime)
	{
		_fadeOutTime = fadeOutTime;
	}

	public void SetFadeInTime(float fadeInTime)
	{
		_fadeInTime = fadeInTime;
	}

	public float GetVolume(Enums.AudioType audioType)
	{
		return audioType switch
		{
			Enums.AudioType.Music => _musicVolume, 
			Enums.AudioType.Ambiance => _ambianceVolume, 
			Enums.AudioType.SFX => _sfxVolume, 
			Enums.AudioType.Master => _masterVolume, 
			_ => 0f, 
		};
	}

	public void SetVolume(Enums.AudioType audioType, float volume)
	{
		switch (audioType)
		{
		case Enums.AudioType.Music:
			_musicVolume = volume;
			UpdateMusicVolume();
			break;
		case Enums.AudioType.Ambiance:
			_ambianceVolume = volume;
			UpdateAmbianceVolume();
			break;
		case Enums.AudioType.SFX:
			_sfxVolume = volume;
			UpdateSFXVolume();
			break;
		case Enums.AudioType.Master:
			_masterVolume = volume;
			UpdateMusicVolume();
			UpdateAmbianceVolume();
			UpdateSFXVolume();
			break;
		}
	}

	private void UpdateSFXVolume()
	{
		foreach (AudioSource sfxSource in _sfxSources)
		{
			sfxSource.volume = _sfxVolume * _masterVolume;
		}
	}

	private void UpdateAmbianceVolume()
	{
		_ambianceSource.volume = _baseVolumeAmbianceSource * _ambianceVolume * _masterVolume;
	}

	internal void SetMusicVolumeByModAndGetOriginal(float volumeMod)
	{
		if (_musicSource1.isPlaying)
		{
			float volume = _musicSource1.volume * volumeMod;
			_musicSource1.volume = volume;
		}
		if (_musicSource2.isPlaying)
		{
			float volume2 = _musicSource2.volume * volumeMod;
			_musicSource2.volume = volume2;
		}
	}

	internal void ResetMusicVolume()
	{
		UpdateMusicVolume();
	}

	private void UpdateMusicVolume()
	{
		if (_musicSource1.isPlaying)
		{
			_musicSource1.volume = _baseVolumeMusicSource1 * _musicVolume * _masterVolume;
		}
		if (_musicSource2.isPlaying)
		{
			_musicSource2.volume = _baseVolumeMusicSource2 * _musicVolume * _masterVolume;
		}
	}

	internal void PreloadAudioClip(AudioClip audioClip)
	{
		if (!(audioClip == null))
		{
			StartCoroutine(PreloadAudioClipAsync(audioClip));
		}
	}

	private IEnumerator PreloadAudioClipAsync(AudioClip audioClip)
	{
		audioClip.LoadAudioData();
		yield return null;
	}

	public void PlayMusicClip(AudioClip audioClip, float baseVolume = 1f, bool loop = true, float time = 0f)
	{
		bool flag = WillMusicBePlayedOnSourceOne();
		List<AudioSource> audioSourcesToFadeOut = GetAudioSourcesToFadeOut();
		AudioSource audioSource = (flag ? _musicSource1 : _musicSource2);
		FadeOutAudioSources(audioSourcesToFadeOut);
		bool loop2 = loop;
		StartCoroutine(FadeInAudioSource(audioSource, baseVolume, audioClip, _fadeInTime, loop2, time));
		SaveBaseVolume(baseVolume, flag);
	}

	private void FadeOutAudioSources(List<AudioSource> audioSourcesToFadeOut)
	{
		foreach (AudioSource item in audioSourcesToFadeOut)
		{
			StartCoroutine(FadeOutAudioSource(item));
		}
	}

	public void FadeOutAudioSources()
	{
		FadeOutAudioSources(GetAudioSourcesToFadeOut());
	}

	private void SetMusicSourceStatus(AudioSource audioSource, MusicSourceStatus status)
	{
		if (audioSource.Equals(_musicSource1))
		{
			_musicSource1Status = status;
		}
		if (audioSource.Equals(_musicSource2))
		{
			_musicSource2Status = status;
		}
	}

	private bool WillMusicBePlayedOnSourceOne()
	{
		if (_musicSource1Status == MusicSourceStatus.Stopped)
		{
			return true;
		}
		if (_musicSource2Status == MusicSourceStatus.Stopped)
		{
			return false;
		}
		if (_musicSource1Status == MusicSourceStatus.FadingOut)
		{
			return true;
		}
		if (_musicSource2Status == MusicSourceStatus.FadingOut)
		{
			return false;
		}
		Debug.LogWarning($"Both music sources are either playing or fading in. This should never happen. Music source 1: {_musicSource1Status} | Music source 2: {_musicSource2Status}");
		return true;
	}

	private List<AudioSource> GetAudioSourcesToFadeOut()
	{
		List<AudioSource> list = new List<AudioSource>();
		if (_musicSource1Status != MusicSourceStatus.Stopped)
		{
			list.Add(_musicSource1);
		}
		if (_musicSource2Status != MusicSourceStatus.Stopped)
		{
			list.Add(_musicSource2);
		}
		return list;
	}

	public void StopMusicClip()
	{
		_musicSource1.Stop();
		_musicSource2.Stop();
		SetMusicSourceStatus(_musicSource1, MusicSourceStatus.Stopped);
		SetMusicSourceStatus(_musicSource2, MusicSourceStatus.Stopped);
	}

	internal void StopAmbience(float fadeOutDuration = 0f)
	{
		float num = 1f * _ambianceVolume * _masterVolume;
		LeanTween.value(base.gameObject, delegate(float val)
		{
			_ambianceSource.volume = val;
		}, num, 0f, fadeOutDuration);
		StartCoroutine(StopAmbienceAsync(fadeOutDuration));
	}

	private IEnumerator StopAmbienceAsync(float fadeOutDuration)
	{
		yield return new WaitForSecondsRealtime(fadeOutDuration);
		_ambianceSource.Stop();
	}

	public void PlayAmbianceClip(AudioClip audioClip, float baseVolume = 1f, bool loop = true, float fadeInDuration = 0f)
	{
		_ambianceSource.Stop();
		if (!(audioClip == null))
		{
			_ambianceSource.loop = loop;
			_ambianceSource.clip = audioClip;
			float to = baseVolume * _ambianceVolume * _masterVolume;
			_baseVolumeAmbianceSource = baseVolume;
			_ambianceSource.volume = 0f;
			LeanTween.value(base.gameObject, delegate(float val)
			{
				_ambianceSource.volume = val;
			}, 0f, to, fadeInDuration);
			_ambianceSource.Play();
		}
	}

	public void PlayAudioSourceAsSfxClip(AudioSource audioSource)
	{
		if (!(audioSource == null) && !(audioSource.clip == null))
		{
			PlaySFXClip(audioSource.clip, audioSource.volume);
		}
	}

	[Command("audiocontroller.test.left", Platform.AllPlatforms, MonoTargetType.Single)]
	public void DEBUG_LocalSFX_LEFT()
	{
		PlaySFXClipAtPosition(_debugSound, 1f, new Vector3(SingletonController<GameController>.Instance.PlayerPosition.x + _leftLocation.x, SingletonController<GameController>.Instance.PlayerPosition.y, 0f));
	}

	[Command("audiocontroller.test.right", Platform.AllPlatforms, MonoTargetType.Single)]
	public void DEBUG_LocalSFX_RIGHT()
	{
		PlaySFXClipAtPosition(_debugSound, 1f, new Vector3(SingletonController<GameController>.Instance.PlayerPosition.x + _rightLocation.x, SingletonController<GameController>.Instance.PlayerPosition.y, 0f));
	}

	public void PlaySFXClipAtPosition(AudioClip audioClip, float volume, Vector3 position)
	{
		if (!(audioClip == null) && ShouldPlaySFXClip(audioClip))
		{
			StartCoroutine(StoreAudioClipToPreventPlaying(audioClip));
			float num = 2f;
			float volume2 = volume * num * _sfxVolume * _masterVolume;
			AudioSource.PlayClipAtPoint(audioClip, position, volume2);
		}
	}

	public AudioSource PlaySFXClip(AudioClip audioClip, float volume, float offset = 0f, float pitch = 1f)
	{
		if (audioClip == null)
		{
			return null;
		}
		if (!ShouldPlaySFXClip(audioClip))
		{
			return null;
		}
		StartCoroutine(StoreAudioClipToPreventPlaying(audioClip));
		AudioSource audioSource = null;
		foreach (AudioSource sfxSource in _sfxSources)
		{
			if (!sfxSource.isPlaying)
			{
				audioSource = sfxSource;
				break;
			}
		}
		if (audioSource == null)
		{
			return null;
		}
		audioSource.clip = audioClip;
		audioSource.volume = volume * _sfxVolume * _masterVolume;
		audioSource.time = offset;
		audioSource.pitch = pitch;
		audioSource.Play();
		return audioSource;
	}

	private bool ShouldPlaySFXClip(AudioClip audioClip)
	{
		return !_audioClipsToPreventPlaying.Contains(audioClip);
	}

	private IEnumerator StoreAudioClipToPreventPlaying(AudioClip audioClip)
	{
		_audioClipsToPreventPlaying.Add(audioClip);
		yield return new WaitForSecondsRealtime(_minimumTimeBetweenPlayingSameClip);
		_audioClipsToPreventPlaying.Remove(audioClip);
	}

	public void PlayDefaultAudio(Enums.DefaultAudioType defaultAudioType, float baseVolume = 1f)
	{
		if (_defaultAudioClips.ContainsKey(defaultAudioType) && _defaultAudioClips[defaultAudioType] != null)
		{
			AudioClip audioClip = _defaultAudioClips[defaultAudioType];
			PlaySFXClip(audioClip, baseVolume);
		}
	}

	public void PlayOnHitAudio(Enums.Enemies.EnemyOnHitType enemyOnHitType, float baseVolume = 1f)
	{
		if (_defaultOnHitAudioClips.ContainsKey(enemyOnHitType) && _defaultOnHitAudioClips[enemyOnHitType] != null)
		{
			AudioClip audioClip = _defaultOnHitAudioClips[enemyOnHitType];
			PlaySFXClip(audioClip, baseVolume);
		}
	}

	public void PlayOnHitAudioOnPosition(Enums.Enemies.EnemyOnHitType enemyOnHitType, Vector3 position, float baseVolume = 2f)
	{
		if (_defaultOnHitAudioClips.ContainsKey(enemyOnHitType) && _defaultOnHitAudioClips[enemyOnHitType] != null)
		{
			AudioClip audioClip = _defaultOnHitAudioClips[enemyOnHitType];
			PlaySFXClipAtPosition(audioClip, baseVolume, position);
		}
	}

	public void PlayOnDraggableDroppedAudio(Enums.PlaceableRarity placeableRarity, float baseVolume = 1f)
	{
		if (_defaultBackpackDropAudioOnVfx.ContainsKey(placeableRarity) && _defaultBackpackDropAudioOnVfx[placeableRarity] != null)
		{
			AudioClip audioClip = _defaultBackpackDropAudioOnVfx[placeableRarity];
			PlaySFXClip(audioClip, baseVolume, 0f, GetPitchVariation());
		}
	}

	private void SaveBaseVolume(float baseVolume, bool willBePlayedOnMusicSource1)
	{
		if (willBePlayedOnMusicSource1)
		{
			_baseVolumeMusicSource1 = baseVolume;
		}
		else
		{
			_baseVolumeMusicSource2 = baseVolume;
		}
	}

	private IEnumerator FadeOutAudioSource(AudioSource audioSource)
	{
		SetMusicSourceStatus(audioSource, MusicSourceStatus.FadingOut);
		float timeSpentFadingOut = 0f;
		while (timeSpentFadingOut < _fadeOutTime)
		{
			float num = Mathf.Min(_maxDeltaTimeForFadingOut, Time.unscaledDeltaTime);
			float num2 = num / _fadeOutTime;
			audioSource.volume -= num2 * audioSource.volume;
			timeSpentFadingOut += num;
			yield return null;
		}
		audioSource.Stop();
		SetMusicSourceStatus(audioSource, MusicSourceStatus.Stopped);
	}

	private IEnumerator FadeInAudioSource(AudioSource audioSource, float baseVolume, AudioClip audioClip, float fadeInDelay = 0f, bool loop = true, float time = 0f)
	{
		SetMusicSourceStatus(audioSource, MusicSourceStatus.FadingIn);
		yield return new WaitForSecondsRealtime(fadeInDelay);
		while (audioSource.isPlaying)
		{
			yield return null;
		}
		audioSource.clip = audioClip;
		audioSource.volume = 0f;
		audioSource.loop = loop;
		audioSource.pitch = 1f;
		audioSource.time = time;
		audioSource.Play();
		float timeSpentFadingIn = 0f;
		float totalMusicVolume = baseVolume * _musicVolume * _masterVolume;
		while (timeSpentFadingIn < _fadeInTime)
		{
			float num = Time.unscaledDeltaTime / _fadeInTime;
			audioSource.volume += num * totalMusicVolume;
			timeSpentFadingIn += Time.unscaledDeltaTime;
			yield return null;
		}
		audioSource.volume = totalMusicVolume;
		SetMusicSourceStatus(audioSource, MusicSourceStatus.Playing);
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}

	internal void StopSFX(AudioSource audioSourceToStop)
	{
		if (!(audioSourceToStop == null))
		{
			audioSourceToStop.Stop();
		}
	}

	internal void StopAllSFX()
	{
		foreach (AudioSource sfxSource in _sfxSources)
		{
			sfxSource.Stop();
		}
	}

	internal static float GetPitchVariation()
	{
		return Random.Range(0.8f, 1.2f);
	}

	internal float GetCurrentMusicProgress()
	{
		if (IsMusicPlaying)
		{
			if (_musicSource1Status == MusicSourceStatus.Playing)
			{
				return _musicSource1.time;
			}
			if (_musicSource2Status == MusicSourceStatus.Playing)
			{
				return _musicSource2.time;
			}
		}
		return 0f;
	}

	internal void ChangeMusicPitch(float pitch)
	{
		if (IsMusicPlaying)
		{
			if (_musicSource1Status == MusicSourceStatus.Playing)
			{
				_musicSource1.pitch = pitch;
			}
			if (_musicSource2Status == MusicSourceStatus.Playing)
			{
				_musicSource2.pitch = pitch;
			}
		}
	}

	internal void PlayEmoteAudio(Enums.Emotes emote)
	{
		AudioClip value = null;
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.EmoteAudio.TryGetValue(emote, out value))
		{
			PlaySFXClip(value, 1f);
		}
		if (emote != Enums.Emotes.Attention)
		{
			_ = 1;
		}
	}

	internal void PlayShowItemSFX()
	{
		PlaySFXClip(SingletonController<GameDatabase>.Instance.GameDatabaseSO.ShowItemAudio, 1f);
	}
}
