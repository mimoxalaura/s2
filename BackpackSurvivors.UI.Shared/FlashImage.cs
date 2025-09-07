using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Shared;

[RequireComponent(typeof(Image))]
public class FlashImage : MonoBehaviour
{
	[SerializeField]
	private float _secondsForOneFlash = 0.5f;

	[SerializeField]
	private float _minAlpha = 0.15f;

	[SerializeField]
	private float _maxAlpha = 0.25f;

	private Coroutine _flashRoutine;

	private Image _flashImage;

	public event Action OnStop = delegate
	{
	};

	public event Action OnCycleStart = delegate
	{
	};

	public event Action OnCycleComplete = delegate
	{
	};

	private void Awake()
	{
		_flashImage = GetComponent<Image>();
		SetAlphaToDefault();
	}

	public void Flash(Color color)
	{
		if (base.isActiveAndEnabled && !(_secondsForOneFlash <= 0f))
		{
			if (_flashRoutine != null)
			{
				StopCoroutine(_flashRoutine);
			}
			_flashRoutine = StartCoroutine(FlashRoutine(_secondsForOneFlash, _minAlpha, _maxAlpha, color));
		}
	}

	public void StopFlash()
	{
		if (_flashRoutine != null)
		{
			StopCoroutine(_flashRoutine);
		}
		SetAlphaToDefault();
		this.OnStop?.Invoke();
	}

	private IEnumerator FlashRoutine(float secondsForOneFlash, float minAlpha, float maxAlpha, Color color)
	{
		SetColor(color);
		float flashInDuration = secondsForOneFlash / 2f;
		float flashOutDuration = secondsForOneFlash / 2f;
		this.OnCycleStart?.Invoke();
		for (float t = 0f; t <= flashInDuration; t += Time.deltaTime)
		{
			Color color2 = _flashImage.color;
			color2.a = Mathf.Lerp(minAlpha, maxAlpha, t / flashInDuration);
			_flashImage.color = color2;
			yield return null;
		}
		for (float t = 0f; t <= flashOutDuration; t += Time.deltaTime)
		{
			Color color3 = _flashImage.color;
			color3.a = Mathf.Lerp(maxAlpha, minAlpha, t / flashOutDuration);
			_flashImage.color = color3;
			yield return null;
		}
		SetAlphaToDefault();
		this.OnCycleComplete?.Invoke();
	}

	private void SetColor(Color newColor)
	{
		_flashImage.color = newColor;
	}

	private void SetAlphaToDefault()
	{
		Color color = _flashImage.color;
		color.a = 0f;
		_flashImage.color = color;
	}
}
