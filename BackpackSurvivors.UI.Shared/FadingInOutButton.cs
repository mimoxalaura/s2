using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Shared;

internal class FadingInOutButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private Image[] _imagesToFadeInOut;

	[SerializeField]
	private float _fadeDuration = 0.3f;

	[SerializeField]
	private float _fadeInAlpha = 1f;

	[SerializeField]
	private float _fadeOutAlpha = 0.3f;

	private bool _fadedIn = true;

	private void Start()
	{
		FadeInformationElements(fadeIn: false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		FadeInformationElements(fadeIn: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		FadeInformationElements(fadeIn: false);
	}

	public void FadeInformationElements(bool fadeIn)
	{
		if (fadeIn && !_fadedIn)
		{
			_fadedIn = true;
			Image[] imagesToFadeInOut = _imagesToFadeInOut;
			foreach (Image imageToFade in imagesToFadeInOut)
			{
				LeanTween.cancel(imageToFade.gameObject);
				LeanTween.value(imageToFade.gameObject, delegate(float val)
				{
					imageToFade.color = new Color(imageToFade.color.r, imageToFade.color.g, imageToFade.color.b, val);
				}, _fadeOutAlpha, _fadeInAlpha, _fadeDuration).setIgnoreTimeScale(useUnScaledTime: true);
			}
		}
		else
		{
			if (fadeIn || !_fadedIn)
			{
				return;
			}
			_fadedIn = false;
			Image[] imagesToFadeInOut = _imagesToFadeInOut;
			foreach (Image imageToFade2 in imagesToFadeInOut)
			{
				LeanTween.cancel(imageToFade2.gameObject);
				LeanTween.value(imageToFade2.gameObject, delegate(float val)
				{
					imageToFade2.color = new Color(imageToFade2.color.r, imageToFade2.color.g, imageToFade2.color.b, val);
				}, _fadeInAlpha, _fadeOutAlpha, _fadeDuration).setIgnoreTimeScale(useUnScaledTime: true);
			}
		}
	}
}
