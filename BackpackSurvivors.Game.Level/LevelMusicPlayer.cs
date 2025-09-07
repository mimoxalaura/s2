using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

internal class LevelMusicPlayer : MonoBehaviour
{
	[SerializeField]
	private AudioClip[] _shopMusicClips;

	[SerializeField]
	private AudioClip[] _levelMusicClips;

	[SerializeField]
	private AudioClip[] _levelAmbianceClips;

	internal void PlayShopMusic()
	{
		if (_shopMusicClips != null && _shopMusicClips.Length != 0)
		{
			AudioClip audioClip = _shopMusicClips[Random.Range(0, _shopMusicClips.Length)];
			SingletonController<AudioController>.Instance.PlayMusicClip(audioClip);
		}
	}

	internal void PlayLevelMusic(float time = 0f)
	{
		if (_levelMusicClips != null && _levelMusicClips.Length != 0)
		{
			AudioClip audioClip = _levelMusicClips[Random.Range(0, _levelMusicClips.Length)];
			SingletonController<AudioController>.Instance.PlayMusicClip(audioClip, 1f, loop: true, time);
		}
	}

	internal void PlayLevelAmbience()
	{
		if (_levelAmbianceClips != null && _levelAmbianceClips.Length != 0)
		{
			AudioClip audioClip = _levelAmbianceClips[Random.Range(0, _levelAmbianceClips.Length)];
			SingletonController<AudioController>.Instance.PlayAmbianceClip(audioClip, 0.3f);
		}
	}
}
