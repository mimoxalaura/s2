using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

public class SpriteAnimationTimescaleZero : MonoBehaviour
{
	[SerializeField]
	private bool _isLoop = true;

	[SerializeField]
	private float _fps = 30f;

	[SerializeField]
	private Sprite[] _spriteFrames;

	private int _curFrameIndex;

	private SpriteRenderer _spriteRenderer;

	private float _secondPreFrame = 1f / 30f;

	private float _curFrameLeftTime;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		Play();
	}

	public void Play()
	{
		if (_spriteFrames != null && _spriteFrames.Length != 0)
		{
			ResetToBeginning();
		}
	}

	public void ResetToBeginning()
	{
		_secondPreFrame = 1f / _fps;
		_curFrameIndex = 0;
		UpdateSprite();
	}

	private void Update()
	{
		if (_spriteFrames == null || _spriteFrames.Length == 0)
		{
			base.enabled = false;
			return;
		}
		float deltaTime = Time.deltaTime;
		_curFrameLeftTime -= deltaTime;
		if (_curFrameLeftTime <= 0f)
		{
			_curFrameLeftTime = _secondPreFrame;
			_curFrameIndex++;
			if (_curFrameIndex >= _spriteFrames.Length)
			{
				_curFrameIndex = ((!_isLoop) ? _spriteFrames.Length : 0);
			}
			UpdateSprite();
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
		else if (_curFrameIndex >= 0 && _curFrameIndex < _spriteFrames.Length)
		{
			_spriteRenderer.sprite = _spriteFrames[_curFrameIndex];
		}
	}
}
