using System.Collections;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI;

public class UITextPopup : MonoBehaviour
{
	[SerializeField]
	private float _fadeOutTime;

	[SerializeField]
	private float _fontSize;

	[SerializeField]
	private float _movementY;

	private TextMeshProUGUI _text;

	public void Init(string text, Color color, float fadeOutTime = 0f)
	{
		_text.SetText(text);
		_text.color = color;
		_text.fontSize = _fontSize;
		if (fadeOutTime > 0f)
		{
			_fadeOutTime = fadeOutTime;
		}
		LeanTween.moveY(base.gameObject, base.transform.position.y + _movementY, 0.4f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(base.gameObject, UpdateFade, 1f, 0f, _fadeOutTime).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		StartCoroutine(DestroyAfterDelay(_fadeOutTime));
	}

	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		Object.Destroy(base.gameObject);
	}

	private void Awake()
	{
		_text = GetComponentInChildren<TextMeshProUGUI>();
	}

	private void UpdateFade(float val)
	{
		_text.color = new Color(_text.color.r, _text.color.g, _text.color.b, val);
	}
}
