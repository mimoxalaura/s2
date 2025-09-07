using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_7 : MovieStep
{
	private string _peaceText;

	private string _warText;

	[SerializeField]
	private AudioClip _warSounds;

	private void Awake()
	{
		base.Image.color = new Color(1f, 1f, 1f, 0f);
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
		SingletonController<AudioController>.Instance.PlayAmbianceClip(_warSounds, 0.5f, loop: false, 2f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 0f, 1f, 1f);
		yield return new WaitForSeconds(5f);
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
