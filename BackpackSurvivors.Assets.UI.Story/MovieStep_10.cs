using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_10 : MovieStep
{
	[SerializeField]
	private MovieStepVFX _mist1;

	[SerializeField]
	private MovieStepVFX _mist2;

	[SerializeField]
	private Transform _mist1End;

	[SerializeField]
	private Transform _mist2End;

	[SerializeField]
	private AudioClip _whispers;

	private void Awake()
	{
		base.Image.color = new Color(1f, 1f, 1f, 0f);
		base.Image.material.SetFloat("_GreyscaleBlend", 0f);
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
		LeanTween.value(base.gameObject, delegate(float val)
		{
			base.Image.material.SetFloat("_GreyscaleBlend", val);
		}, 0f, 1f, 5f);
		LeanTween.moveLocalX(_mist1.gameObject, _mist1End.localPosition.x, 8f);
		LeanTween.moveLocalX(_mist2.gameObject, _mist2End.localPosition.x, 8f);
		_mist1.FadeIn(6f);
		_mist2.FadeIn(6f);
		yield return new WaitForSeconds(2f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_whispers, 1f, 0f, 0.5f);
		yield return new WaitForSeconds(4f);
		FadeToBlack(2f);
	}

	internal void FadeToValue(float val)
	{
		base.Image.color = new Color(1f, 1f, 1f, val);
	}

	internal override void Finish()
	{
		_mist1.FadeOut(0f);
		_mist2.FadeOut(0f);
		base.Finish();
	}
}
