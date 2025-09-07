using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureValuePercentage : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _Text;

	[SerializeField]
	private Image _icon;

	private float _value;

	public float Value => _value;

	public void ChangeValue(float value, bool percentage = true)
	{
		if (_value != value)
		{
			_value = value;
			if (_icon != null)
			{
				LeanTween.cancel(_icon.gameObject);
				LeanTween.scale(_icon.gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.3f).setEasePunch().setIgnoreTimeScale(useUnScaledTime: true);
				LeanTween.scale(_icon.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
			}
			if (percentage)
			{
				_Text.SetText($"+{(int)(100f * value)}%");
			}
			else
			{
				_Text.SetText($"{(int)value}");
			}
		}
	}

	public void ChangeValue(string value)
	{
		_value = -1f;
		if (_icon != null)
		{
			LeanTween.cancel(_icon.gameObject);
			LeanTween.scale(_icon.gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.3f).setEasePunch().setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(_icon.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		}
		_Text.SetText(value ?? "");
	}
}
