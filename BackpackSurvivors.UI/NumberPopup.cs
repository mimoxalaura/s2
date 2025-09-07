using System.Collections;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI;

public class NumberPopup : MonoBehaviour
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
	private float _movementY;

	private TextMeshProUGUI _numberText;

	private float _fadeOutTimeDone;

	public void Init(int number, Color color, bool attentionEffect)
	{
		_numberText.SetText(number.ToString());
		_numberText.color = color;
		SetFontSizeBasedOnDamage(number);
		LeanTween.moveY(base.gameObject, base.transform.position.y + _movementY, 0.4f);
		if (attentionEffect)
		{
			LeanTween.scale(base.gameObject, new Vector3(1.8f, 1.8f, 1f), 0.2f).setEaseOutElastic();
			LeanTween.scale(base.gameObject, new Vector3(1f, 1f, 1f), 0.4f).setEaseOutElastic().setDelay(0.3f);
		}
		StartCoroutine(FadeOut());
	}

	private void SetFontSizeBasedOnDamage(int number)
	{
		float num = (float)number / _damageForMaxFontSize;
		num += _minFontSize;
		num = Mathf.Clamp(num, _minFontSize, _maxFontSize);
		_numberText.fontSize = num;
	}

	private void Awake()
	{
		_numberText = GetComponentInChildren<TextMeshProUGUI>();
	}

	private IEnumerator FadeOut()
	{
		while (_fadeOutTimeDone < _fadeOutTime)
		{
			_fadeOutTimeDone += Time.deltaTime;
			float num = _fadeOutTimeDone / _fadeOutTime;
			float a = 1f - num;
			Color color = new Color(_numberText.color.r, _numberText.color.g, _numberText.color.b, a);
			_numberText.color = color;
			yield return null;
		}
		base.gameObject.SetActive(value: false);
		Object.Destroy(base.gameObject);
	}
}
