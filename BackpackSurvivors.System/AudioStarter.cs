using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.System;

public class AudioStarter : MonoBehaviour
{
	[SerializeField]
	private AudioClip _audioClipToPlay;

	[SerializeField]
	private Enums.AudioType _audioType;

	[SerializeField]
	private float _baseVolume;

	[SerializeField]
	private bool _loop = true;

	private void Start()
	{
		StartCoroutine(PlayClip());
	}

	private IEnumerator PlayClip()
	{
		yield return new WaitForSecondsRealtime(0.2f);
		switch (_audioType)
		{
		case Enums.AudioType.Music:
			if (_audioClipToPlay != null)
			{
				SingletonController<AudioController>.Instance.PlayMusicClip(_audioClipToPlay, 1f, _loop);
			}
			break;
		case Enums.AudioType.Ambiance:
			SingletonController<AudioController>.Instance.PlayAmbianceClip(_audioClipToPlay, _baseVolume, _loop);
			break;
		}
	}
}
