using System.Collections;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Story;

internal class MovieStep : MonoBehaviour
{
	[SerializeField]
	private float _stepDuration;

	[SerializeField]
	private float _showTextDelay;

	[SerializeField]
	private float _textCharacterWaitTime = 0.1f;

	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _textMeshProUGUI;

	[SerializeField]
	private Image _fadeToBlackOverlay;

	[SerializeField]
	private Enums.MovieStep _movieStep;

	[SerializeField]
	private AudioClip _voiceOver;

	internal float Duration => _stepDuration;

	internal Image Image => _image;

	internal float ShowTextDelay => _showTextDelay;

	internal float TextCharacterWaitTime => _textCharacterWaitTime;

	internal AudioClip Voiceover => _voiceOver;

	internal virtual void Play()
	{
		if (_textMeshProUGUI != null)
		{
			_textMeshProUGUI.text = string.Empty;
			StartCoroutine(SlowlyShowText(StringHelper.GetMovieStepText(_movieStep)));
			SingletonController<AudioController>.Instance.PlaySFXClip(_voiceOver, 1f);
		}
	}

	internal virtual void Finish()
	{
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator SlowlyShowText(string text)
	{
		yield return new WaitForSeconds(_showTextDelay);
		int index = 0;
		while (index < text.Length)
		{
			_textMeshProUGUI.text += text[index];
			index++;
			yield return new WaitForSeconds(_textCharacterWaitTime);
		}
	}

	internal void FadeToBlack(float duration = 1f)
	{
		LeanTween.value(base.gameObject, delegate(float val)
		{
			_fadeToBlackOverlay.color = new Color(0f, 0f, 0f, val);
		}, 0f, 1f, duration);
	}

	internal void FadeFromBlack(float duration = 1f)
	{
		LeanTween.value(base.gameObject, delegate(float val)
		{
			_fadeToBlackOverlay.color = new Color(0f, 0f, 0f, val);
		}, 1f, 0f, duration);
	}
}
