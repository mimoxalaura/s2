using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_13 : MovieStep
{
	[SerializeField]
	private GameObject _glimpse;

	[SerializeField]
	private AudioClip _glimpseAudio;

	private void Awake()
	{
		base.Image.color = new Color(1f, 1f, 1f, 0f);
		_glimpse.SetActive(value: false);
	}

	internal override void Play()
	{
		base.Play();
		StartCoroutine(PlayMovieStep());
	}

	private IEnumerator PlayMovieStep()
	{
		FadeFromBlack();
		yield return new WaitForSeconds(0f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 0f, 1f, 1f);
		yield return new WaitForSeconds(2f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_glimpseAudio, 0.8f);
		_glimpse.SetActive(value: true);
		yield return new WaitForSeconds(0.3f);
		_glimpse.SetActive(value: false);
		yield return new WaitForSeconds(base.Duration - 4f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 1f, 0f, 1f);
		FadeToBlack();
	}

	internal void FadeToValue(float val)
	{
		base.Image.color = new Color(1f, 1f, 1f, val);
	}

	internal override void Finish()
	{
		base.Finish();
	}
}
