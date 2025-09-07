using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_1 : MovieStep
{
	[SerializeField]
	private MovieStepVFX _voidVfx1;

	[SerializeField]
	private MovieStepVFX _voidVfx2;

	[SerializeField]
	private AudioClip _monsterRoar;

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
		_voidVfx1.FadeIn(5f);
		_voidVfx2.FadeIn(5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_monsterRoar, 1f, 0f, 0.7f);
		yield return new WaitForSeconds(4f);
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
