using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_3_8 : MovieStep
{
	[SerializeField]
	private AudioClip _impactAudioClip;

	[SerializeField]
	private MovieStepVFX _voidVfx1;

	[SerializeField]
	private MovieStepVFX _voidVfx2;

	internal override void Play()
	{
		base.Play();
		StartCoroutine(PlayMovieStep());
	}

	private IEnumerator PlayMovieStep()
	{
		SingletonController<AudioController>.Instance.PlaySFXClip(_impactAudioClip, 0.5f);
		_voidVfx1.FadeOut(2f);
		_voidVfx2.FadeOut(2f);
		yield return new WaitForSeconds(base.Duration - 1f);
		FadeToBlack();
	}

	internal override void Finish()
	{
		base.Finish();
	}
}
