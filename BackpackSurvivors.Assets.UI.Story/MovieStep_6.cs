using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_6 : MovieStep
{
	[SerializeField]
	private Image _heartOrb;

	[SerializeField]
	private Image _heartVfx;

	private void Awake()
	{
		base.Image.color = new Color(1f, 1f, 1f, 0f);
	}

	internal override void Play()
	{
		base.Play();
		_heartOrb.material.SetFloat("_Alpha", 0f);
		_heartVfx.material.SetFloat("_Alpha", 0.5f);
		StartCoroutine(PlayMovieStep());
	}

	private IEnumerator PlayMovieStep()
	{
		FadeFromBlack();
		yield return new WaitForSeconds(0f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 0f, 1f, 1f);
		LeanTween.value(_heartOrb.gameObject, delegate(float val)
		{
			_heartOrb.material.SetFloat("_Alpha", val);
		}, 0f, 1f, 5f);
		LeanTween.value(_heartOrb.gameObject, delegate(float val)
		{
			_heartOrb.color = new Color(1f, 1f, 1f, val);
		}, 0f, 1f, 6f);
		LeanTween.value(_heartVfx.gameObject, delegate(float val)
		{
			_heartVfx.material.SetFloat("_Alpha", val);
		}, 0.5f, 1f, 6f);
		LeanTween.rotateAroundLocal(_heartVfx.gameObject, new Vector3(0f, 0f, 1f), -720f, 10f);
		LeanTween.scale(_heartOrb.gameObject, new Vector3(1.6f, 1.6f, 1.6f), 5f);
		LeanTween.scale(_heartVfx.gameObject, new Vector3(3f, 3f, 3f), 10f);
		yield return new WaitForSeconds(5f);
		LeanTween.cancel(_heartOrb.gameObject);
		LeanTween.cancel(_heartVfx.gameObject);
		LeanTween.value(_heartOrb.gameObject, delegate(float val)
		{
			_heartOrb.material.SetFloat("_Alpha", val);
		}, 1f, 0f, 1f);
		LeanTween.rotateAroundLocal(_heartVfx.gameObject, new Vector3(0f, 0f, 1f), -720f, 10f);
		LeanTween.value(_heartVfx.gameObject, delegate(float val)
		{
			_heartVfx.material.SetFloat("_Alpha", val);
		}, 1f, 0f, 1f);
		yield return new WaitForSeconds(1f);
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
