using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Shared;

public class ImageAnimationTimeScaleZero : MonoBehaviour
{
	[SerializeField]
	private bool _isLoop = true;

	[SerializeField]
	private float _fps = 30f;

	[SerializeField]
	private Sprite[] _spriteFrames;

	private int _frameIndex;

	private Image _image;

	private float _secondPerFrame = 1f / 30f;

	private void Awake()
	{
		_image = GetComponent<Image>();
	}

	public void SetFrames(Sprite[] sprites)
	{
		_spriteFrames = sprites;
		Play();
	}

	private void Start()
	{
		Play();
	}

	public void Play()
	{
		StopAllCoroutines();
		StartCoroutine(PlayAsync());
	}

	private IEnumerator PlayAsync()
	{
		if (_spriteFrames == null || _spriteFrames.Length == 0)
		{
			yield break;
		}
		ResetToBeginning();
		while (base.isActiveAndEnabled)
		{
			if (_spriteFrames == null || _spriteFrames.Length == 0)
			{
				base.enabled = false;
				break;
			}
			_frameIndex++;
			if (_frameIndex >= _spriteFrames.Length)
			{
				_frameIndex = ((!_isLoop) ? _spriteFrames.Length : 0);
			}
			UpdateSprite();
			yield return new WaitForSecondsRealtime(_secondPerFrame);
		}
	}

	public void ResetToBeginning()
	{
		_secondPerFrame = 1f / _fps;
		_frameIndex = 0;
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		if (_image == null)
		{
			base.enabled = false;
		}
		else if (_spriteFrames == null || _spriteFrames.Length == 0)
		{
			base.enabled = false;
		}
		else if (_frameIndex >= 0 && _frameIndex < _spriteFrames.Length)
		{
			_image.sprite = _spriteFrames[_frameIndex];
		}
	}
}
