using System.Collections;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep_4 : MovieStep
{
	[SerializeField]
	private TextMeshProUGUI _peaceTextComponent;

	[SerializeField]
	private GameObject _peaceBackdropComponent;

	[SerializeField]
	private TextMeshProUGUI _warTextComponent;

	[SerializeField]
	private GameObject _warBackdropComponent;

	[SerializeField]
	private Image[] _peaceRunes;

	[SerializeField]
	private Image[] _warRunes;

	[SerializeField]
	private AudioClip _runeActivationAudioClip;

	[SerializeField]
	private AudioClip _portalActivationAudioClip;

	[SerializeField]
	private AudioClip _badPortalActivationAudioClip;

	[SerializeField]
	private Image _portalGood;

	[SerializeField]
	private Image _portalBad;

	[SerializeField]
	private MovieStepVFX _goodVfx;

	[SerializeField]
	private MovieStepVFX _badVfx;

	[SerializeField]
	private AudioClip _goodVoiceover;

	[SerializeField]
	private AudioClip _badVoiceover;

	private string _peaceText;

	private string _warText;

	private void Awake()
	{
		_peaceText = _peaceTextComponent.text;
		_warText = _warTextComponent.text;
		_peaceTextComponent.text = string.Empty;
		_warTextComponent.text = string.Empty;
		_warBackdropComponent.SetActive(value: false);
		_portalGood.material.SetFloat("_Alpha", 0f);
		_portalBad.material.SetFloat("_FadeAmount", 1f);
	}

	internal override void Play()
	{
		base.Play();
		StartCoroutine(PlayMovieStep());
	}

	private IEnumerator PlayMovieStep()
	{
		FadeFromBlack();
		yield return new WaitForSeconds(1f);
		_peaceTextComponent.gameObject.SetActive(value: true);
		StartCoroutine(SlowlyShowText(_peaceTextComponent, _peaceText));
		yield return new WaitForSeconds(2f);
		_goodVfx.FadeIn(2f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_goodVoiceover, 1f);
		int i;
		for (i = 0; i < 9; i++)
		{
			LeanTween.value(base.gameObject, delegate(float val)
			{
				_peaceRunes[i].color = new Color(1f, 1f, 1f, val);
			}, 0f, 1f, 0.3f);
			SingletonController<AudioController>.Instance.PlaySFXClip(_runeActivationAudioClip, 1f);
			yield return new WaitForSeconds(0.3f);
		}
		SingletonController<AudioController>.Instance.PlaySFXClip(_portalActivationAudioClip, 1f);
		LeanTween.value(_portalGood.gameObject, delegate(float val)
		{
			_portalGood.material.SetFloat("_Alpha", val);
		}, 0f, 1f, 1f);
		yield return new WaitForSeconds(4f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_badPortalActivationAudioClip, 1f);
		_goodVfx.FadeOut(0.5f);
		_badVfx.FadeIn(1f);
		_peaceTextComponent.gameObject.SetActive(value: false);
		_warTextComponent.gameObject.SetActive(value: true);
		_warBackdropComponent.SetActive(value: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_badVoiceover, 1f);
		StartCoroutine(SlowlyShowText(_warTextComponent, _warText));
		int i2;
		for (i2 = 0; i2 < 9; i2++)
		{
			LeanTween.value(base.gameObject, delegate(float val)
			{
				_warRunes[i2].color = new Color(0.36f, 0.36f, 0.36f, val);
			}, 0f, 1f, 0.3f);
			SingletonController<AudioController>.Instance.PlaySFXClip(_runeActivationAudioClip, 1f);
			yield return new WaitForSeconds(0.3f);
		}
		LeanTween.value(_portalBad.gameObject, delegate(float val)
		{
			_portalBad.material.SetFloat("_FadeAmount", val);
		}, 1f, -0.1f, 4f);
		yield return new WaitForSeconds(1f);
		LeanTween.value(base.Image.gameObject, FadeToValue, 1f, 0f, 4f);
		yield return new WaitForSeconds(5f);
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

	private IEnumerator SlowlyShowText(TextMeshProUGUI _textElement, string textToShow)
	{
		yield return new WaitForSeconds(base.ShowTextDelay);
		int index = 0;
		while (index < textToShow.Length)
		{
			_textElement.text += textToShow[index];
			index++;
			yield return new WaitForSeconds(base.TextCharacterWaitTime);
		}
	}
}
