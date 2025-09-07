using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.System;

public class RandomAudioPicker : MonoBehaviour
{
	[SerializeField]
	private AudioClip[] _clipsToChooseFrom;

	[SerializeField]
	private int _randomMax;

	[SerializeField]
	private bool _enabled;

	private void Start()
	{
		StartCoroutine(StartAttemptingPlays());
	}

	private IEnumerator StartAttemptingPlays()
	{
		while (_enabled)
		{
			if (Random.Range(0, _randomMax) == 0)
			{
				int num = Random.Range(0, _clipsToChooseFrom.Length);
				PlayAudio(_clipsToChooseFrom[num]);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void PlayAudio(AudioClip clip)
	{
		SingletonController<AudioController>.Instance.PlaySFXClip(clip, 1f);
	}
}
