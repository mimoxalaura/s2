using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_16 : MovieStep
{
	[SerializeField]
	private Image _titleImage;

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
		yield return new WaitForSeconds(2f);
		LeanTween.value(_titleImage.gameObject, FadeTitleImageToValue, 0f, 1f, 3f);
		yield return new WaitForSeconds(4f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 1f, 0f, 1f);
		LeanTween.value(_titleImage.gameObject, FadeTitleImageToValue, 1f, 0f, 1f);
		FadeToBlack();
	}

	internal void FadeToValue(float val)
	{
		base.Image.color = new Color(0f, 0f, 0f, val);
	}

	internal void FadeTitleImageToValue(float val)
	{
		_titleImage.color = new Color(1f, 1f, 1f, val);
	}

	internal override void Finish()
	{
		base.Finish();
	}
}
