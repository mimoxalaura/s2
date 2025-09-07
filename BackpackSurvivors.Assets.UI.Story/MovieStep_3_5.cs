using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_3_5 : MovieStep
{
	[SerializeField]
	private AudioClip _orcishHorde;

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
		LeanTween.value(base.Image.gameObject, FadeToValue, 0f, 1f, 1f);
		yield return new WaitForSeconds(1f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_orcishHorde, 1f);
		yield return new WaitForSeconds(base.Duration - 3f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 1f, 0f, 1f);
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
