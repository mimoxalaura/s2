using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_11 : MovieStep
{
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
		yield return new WaitForSeconds(0f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 0f, 1f, 1f);
		FadeFromBlack();
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
