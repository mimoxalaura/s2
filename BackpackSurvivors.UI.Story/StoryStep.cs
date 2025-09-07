using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Story;

public abstract class StoryStep : MonoBehaviour
{
	[SerializeField]
	public float StartDuration;

	[SerializeField]
	public float RunDuration;

	[SerializeField]
	public float FinishDuration;

	[SerializeField]
	private TextMeshProUGUI _textMeshProUGUI;

	private Image _imageToFade;

	private TextMeshProUGUI _textToFade;

	private int _index;

	private string _actualText;

	public virtual void Run()
	{
	}

	internal virtual void BeforeFinish()
	{
	}

	internal virtual void AfterFinish()
	{
		if (_textMeshProUGUI != null)
		{
			_textMeshProUGUI.gameObject.SetActive(value: false);
		}
		base.gameObject.SetActive(value: false);
	}

	internal virtual void BeforeStart()
	{
		if (_textMeshProUGUI != null)
		{
			_textMeshProUGUI.gameObject.SetActive(value: true);
			ShowText(_textMeshProUGUI.text);
		}
	}

	internal virtual void AfterStart()
	{
	}

	public void MoveToPositionOverTime(Transform targetPosition, GameObject toMove)
	{
		Vector3 position = targetPosition.position;
		LeanTween.move(toMove, position, StartDuration + RunDuration + FinishDuration);
	}

	public void ZoomInOverTime(GameObject toZoom, float zoomFactor)
	{
		Vector3 to = toZoom.transform.localScale * zoomFactor;
		LeanTween.scale(toZoom, to, StartDuration + RunDuration + FinishDuration);
	}

	public void FadeAlpha(TextMeshProUGUI toFade, float targetAlpha, float duration)
	{
		_textToFade = toFade;
		Color color = toFade.color;
		Color to = color;
		to.a = targetAlpha;
		LeanTween.value(toFade.gameObject, updateValueTextCallback, color, to, duration);
	}

	private void updateValueTextCallback(Color val)
	{
		_textToFade.color = val;
	}

	public void FadeAlpha(Image toFade, float targetAlpha, float duration)
	{
		_imageToFade = toFade;
		Color color = toFade.color;
		Color to = color;
		to.a = targetAlpha;
		LeanTween.value(toFade.gameObject, updateValueImageCallback, color, to, duration);
	}

	public void FadeAlpha(Image toFade, float targetAlpha, float duration, float delay)
	{
		_imageToFade = toFade;
		Color color = toFade.color;
		Color to = color;
		to.a = targetAlpha;
		LeanTween.value(toFade.gameObject, updateValueImageCallback, color, to, duration).setDelay(delay);
	}

	private void updateValueImageCallback(Color val)
	{
		_imageToFade.color = val;
	}

	public void ShowText(string targetText)
	{
		_textMeshProUGUI.text = _actualText;
		_actualText = string.Empty;
		_index = 0;
		StartCoroutine(SlowlyShowText(targetText));
	}

	private IEnumerator SlowlyShowText(string targetText)
	{
		while (_index < targetText.Length)
		{
			_actualText += targetText[_index];
			_textMeshProUGUI.text = _actualText;
			_index++;
			yield return new WaitForSecondsRealtime(0.1f);
		}
	}
}
