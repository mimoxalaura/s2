using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStepVFX : MonoBehaviour
{
	[SerializeField]
	private Image _vfxImage;

	[SerializeField]
	private float _startValue;

	[SerializeField]
	private bool _autoStart;

	[SerializeField]
	private float _delayBetweenFadeInAndFadeOut;

	[SerializeField]
	private float _fadeDuration;

	[SerializeField]
	private float _delayBeforeActivation;

	private void Awake()
	{
		_vfxImage.material.SetFloat("_Alpha", _startValue);
	}

	private void Start()
	{
		if (_autoStart)
		{
			StartCoroutine(FadeInAndOutAsync());
		}
	}

	private IEnumerator FadeInAndOutAsync()
	{
		yield return new WaitForSeconds(_delayBeforeActivation);
		FadeIn(_fadeDuration);
		yield return new WaitForSeconds(_delayBetweenFadeInAndFadeOut);
		FadeOut(_fadeDuration);
	}

	internal void FadeIn(float duration)
	{
		LeanTween.value(_vfxImage.gameObject, FadeVfxToValue, 0f, 1f, duration);
	}

	internal void FadeOut(float duration)
	{
		LeanTween.value(_vfxImage.gameObject, FadeVfxToValue, 1f, 0f, duration);
	}

	internal void FadeVfxToValue(float val)
	{
		_vfxImage.material.SetFloat("_Alpha", val);
	}
}
