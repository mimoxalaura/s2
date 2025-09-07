using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

public class SpriteAnimation : MonoBehaviour
{
	[SerializeField]
	private bool _isLoop = true;

	[SerializeField]
	private float _secondPreFrame = 0.1f;

	[SerializeField]
	private Sprite[] _spriteFrames;

	private SpriteRenderer _spriteRenderer;

	private int _frameIndex;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
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

	public void ResetToBeginning()
	{
		UpdateSprite();
	}

	private IEnumerator PlayAsync()
	{
		if (_spriteFrames == null || _spriteFrames.Length == 0)
		{
			base.enabled = false;
			yield break;
		}
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
			yield return new WaitForSecondsRealtime(_secondPreFrame);
		}
	}

	private void UpdateSprite()
	{
		if (_spriteRenderer == null)
		{
			base.enabled = false;
		}
		else if (_spriteFrames == null || _spriteFrames.Length == 0)
		{
			base.enabled = false;
		}
		else if (_frameIndex >= 0 && _frameIndex < _spriteFrames.Length)
		{
			_spriteRenderer.sprite = _spriteFrames[_frameIndex];
		}
	}
}
