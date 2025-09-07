using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_15_5 : MovieStep
{
	internal override void Play()
	{
		base.Play();
		StartCoroutine(PlayMovieStep());
	}

	private IEnumerator PlayMovieStep()
	{
		yield return new WaitForSeconds(base.Duration - 1f);
		FadeToBlack();
	}

	internal override void Finish()
	{
		base.Finish();
	}
}
