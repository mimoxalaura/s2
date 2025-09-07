using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors;

public class MainButton : MonoBehaviour
{
	[SerializeField]
	private Image _buttonImage;

	[SerializeField]
	private TextMeshProUGUI _buttonText;

	[SerializeField]
	private AudioClip _buttonHoverAudio;

	[SerializeField]
	private AudioClip _buttonClickAudio;

	[SerializeField]
	private float scaleSizeOnHover = 1.05f;

	[SerializeField]
	private bool _playAudioOnHover = true;

	[SerializeField]
	private float _audioOnHoverVolume = 0.5f;

	[SerializeField]
	private bool _playAudioOnClick = true;

	[SerializeField]
	private float _audioOnClickVolume = 0.5f;

	private Vector3 _originalSize;

	[SerializeField]
	private bool _hoverToggled;

	public void Start()
	{
		_originalSize = base.transform.localScale;
	}

	public void ToggleHoverEffects(bool toggled)
	{
		_hoverToggled = toggled;
	}

	public void SetButtonClickAudioClip(AudioClip audioClip)
	{
		_buttonClickAudio = audioClip;
	}

	public virtual void OnClick()
	{
		if (_playAudioOnClick)
		{
			if (_buttonClickAudio == null)
			{
				SingletonController<AudioController>.Instance.PlayDefaultAudio(Enums.DefaultAudioType.ButtonClick);
			}
			else
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_buttonClickAudio, _audioOnClickVolume);
			}
		}
	}

	public virtual void OnHover()
	{
		UIElementHighlighterHelper.Show(((RectTransform)base.transform).position, ((RectTransform)base.transform).sizeDelta, GetComponentInParent<Canvas>());
		if (_playAudioOnHover && GetComponent<Button>().interactable)
		{
			if (_buttonClickAudio == null)
			{
				SingletonController<AudioController>.Instance.PlayDefaultAudio(Enums.DefaultAudioType.ButtonHover);
			}
			else
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_buttonHoverAudio, _audioOnHoverVolume);
			}
		}
		if (_hoverToggled)
		{
			LeanTween.scale(base.gameObject, new Vector3(scaleSizeOnHover, scaleSizeOnHover, scaleSizeOnHover), 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		}
	}

	public virtual void OnExitHover()
	{
		UIElementHighlighterHelper.Hide();
		if (_hoverToggled)
		{
			LeanTween.scale(base.gameObject, new Vector3(_originalSize.x, _originalSize.y, _originalSize.z), 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		}
	}

	internal void SetIconAndText(string text, Sprite sprite)
	{
		if (sprite != null)
		{
			_buttonImage.sprite = sprite;
		}
		_buttonText.SetText(text);
	}

	internal void SetIconColor(Color color)
	{
		if (_buttonImage != null)
		{
			_buttonImage.color = color;
		}
		_buttonImage.SetAllDirty();
	}
}
