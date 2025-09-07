using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_13_5 : MovieStep
{
	[SerializeField]
	private MovieStepVFX _voidVfx1;

	[SerializeField]
	private MovieStepVFX _voidVfx2;

	internal override void Play()
	{
		base.Play();
		StartCoroutine(PlayMovieStep());
		_voidVfx1.FadeOut(0f);
		_voidVfx2.FadeOut(0f);
	}

	private IEnumerator PlayMovieStep()
	{
		_voidVfx1.FadeIn(4f);
		_voidVfx2.FadeIn(4f);
		yield return new WaitForSeconds(4f);
		FadeToBlack();
	}

	internal override void Finish()
	{
		base.Finish();
	}
}
