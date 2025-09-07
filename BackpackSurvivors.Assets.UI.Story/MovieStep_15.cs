using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_15 : MovieStep
{
	[SerializeField]
	private GameObject _textObject;

	[SerializeField]
	private AudioClip _demonLaugh;

	[SerializeField]
	private MovieStepVFX _voidVfx1;

	[SerializeField]
	private MovieStepVFX _voidVfx2;

	private void Awake()
	{
		base.Image.color = new Color(1f, 1f, 1f, 0f);
	}

	internal override void Play()
	{
		base.Play();
		base.Image.material.SetFloat("_GlowGlobal", 1f);
		base.Image.material.SetFloat("_OverlayBlend", 0.07f);
		base.Image.material.SetFloat("_OverlayGlow", 2f);
		base.Image.material.SetColor("_OverlayColor", new Color(0.99f, 0.94f, 0.78f, 0.08f));
		StartCoroutine(PlayMovieStep());
	}

	private IEnumerator PlayMovieStep()
	{
		FadeFromBlack();
		yield return new WaitForSeconds(0f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 0f, 1f, 1f);
		yield return new WaitForSeconds(3f);
		LeanTween.value(base.gameObject, delegate(float val)
		{
			base.Image.material.SetFloat("_GlowGlobal", val);
		}, 1f, 100f, 3f);
		LeanTween.value(base.gameObject, delegate(float val)
		{
			base.Image.material.SetFloat("_OverlayBlend", val);
		}, 0.07f, 1f, 3f);
		LeanTween.value(base.gameObject, delegate(float val)
		{
			base.Image.material.SetFloat("_OverlayGlow", val);
		}, 2f, 25f, 3f);
		LeanTween.value(base.gameObject, delegate(float val)
		{
			base.Image.material.SetColor("_OverlayColor", new Color(0.99f, 0.94f, 0.78f, val));
		}, 0.08f, 1f, 3f);
		_textObject.SetActive(value: false);
		_voidVfx1.FadeOut(3f);
		_voidVfx2.FadeOut(3f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_demonLaugh, 1f);
		yield return new WaitForSeconds(3f);
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
