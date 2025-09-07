using System.Collections;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI;

public class TextPopup : MonoBehaviour
{
	[SerializeField]
	private float _fadeOutTime;

	[SerializeField]
	private float _minFontSize;

	[SerializeField]
	private float _maxFontSize;

	[SerializeField]
	private float _damageForMaxFontSize;

	[SerializeField]
	private TextMeshProUGUI _text;

	private float _fadeOutTimeDone;

	public void Init(string text, Color color, float fadeOutTime = 1f, float movementY = 0.5f)
	{
		if (!LeanTweenHelper.IsAtMaxCapacity())
		{
			_text.SetText(text);
			_text.color = color;
			_text.fontSize = _minFontSize;
			_fadeOutTime = fadeOutTime;
			LeanTween.moveY(base.gameObject, base.transform.position.y + movementY, 0.4f).setIgnoreTimeScale(useUnScaledTime: true);
			StartCoroutine(FadeOut());
		}
		else
		{
			base.gameObject.SetActive(value: false);
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator FadeOut()
	{
		while (_fadeOutTimeDone < _fadeOutTime)
		{
			_fadeOutTimeDone += Time.deltaTime;
			float num = _fadeOutTimeDone / _fadeOutTime;
			float a = 1f - num;
			Color color = new Color(_text.color.r, _text.color.g, _text.color.b, a);
			_text.color = color;
			yield return null;
		}
		base.gameObject.SetActive(value: false);
		Object.Destroy(base.gameObject);
	}
}
