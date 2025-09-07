using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_0 : MovieStep
{
	internal override void Play()
	{
		base.Play();
		StartCoroutine(PlayMovieStep());
	}

	private IEnumerator PlayMovieStep()
	{
		FadeFromBlack(3f);
		yield return new WaitForSeconds(0f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 0f, 1f, 1f);
		yield return new WaitForSeconds(base.Duration);
		LeanTween.value(base.Image.gameObject, FadeToValue, 1f, 0f, 1f);
		FadeToBlack();
	}

	internal void FadeToValue(float val)
	{
		base.Image.color = new Color(0f, 0f, 0f, val);
	}

	internal override void Finish()
	{
		base.Finish();
	}
}
