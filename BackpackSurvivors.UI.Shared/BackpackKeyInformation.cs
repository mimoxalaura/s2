using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Shared;

public class BackpackKeyInformation : KeyInformation
{
	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private Image[] _images;

	[SerializeField]
	private Color _textColor;

	[SerializeField]
	private Color _imageColor;

	[SerializeField]
	private float _fadeDuration;

	[SerializeField]
	private float _fadeInAlpha = 1f;

	[SerializeField]
	private float _fadeOutAlpha = 0.3f;

	private bool _fadedIn = true;

	private void Start()
	{
		SetupButtonIcons();
		SingletonController<InputController>.Instance.OnControlSchemeChanged += Instance_OnControlSchemeChanged;
		if (SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard)
		{
			FadeInformationElements(fadeIn: false);
		}
	}

	private void Instance_OnControlSchemeChanged(object sender, ControlSchemeChangedEventArgs e)
	{
		SetupButtonIcons();
		if (SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard)
		{
			FadeInformationElements(fadeIn: false);
		}
		else
		{
			FadeInformationElements(fadeIn: true);
		}
	}

	public void FadeInformationElements(bool fadeIn)
	{
		if (fadeIn && !_fadedIn)
		{
			_fadedIn = true;
			LeanTween.value(_text.gameObject, delegate(float val)
			{
				_text.color = new Color(_textColor.r, _textColor.g, _textColor.b, val);
			}, 1f, _fadeInAlpha, _fadeDuration).setIgnoreTimeScale(useUnScaledTime: true);
			Image[] images = _images;
			foreach (Image image in images)
			{
				LeanTween.value(image.gameObject, delegate(float val)
				{
					image.color = new Color(_textColor.r, _textColor.g, _textColor.b, val);
				}, 1f, _fadeInAlpha, _fadeDuration).setIgnoreTimeScale(useUnScaledTime: true);
			}
		}
		else
		{
			if (fadeIn || !_fadedIn)
			{
				return;
			}
			_fadedIn = false;
			LeanTween.cancel(_text.gameObject);
			LeanTween.value(_text.gameObject, delegate(float val)
			{
				_text.color = new Color(_textColor.r, _textColor.g, _textColor.b, val);
			}, 1f, _fadeOutAlpha, _fadeDuration).setIgnoreTimeScale(useUnScaledTime: true);
			Image[] images = _images;
			foreach (Image image2 in images)
			{
				LeanTween.cancel(image2.gameObject);
				LeanTween.cancel(image2.gameObject);
				LeanTween.value(image2.gameObject, delegate(float val)
				{
					image2.color = new Color(_textColor.r, _textColor.g, _textColor.b, val);
				}, 1f, _fadeOutAlpha, _fadeDuration).setIgnoreTimeScale(useUnScaledTime: true);
			}
		}
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnControlSchemeChanged -= Instance_OnControlSchemeChanged;
	}
}
