using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteMask))]
internal class SpriteToMaskBinder : MonoBehaviour
{
	private SpriteRenderer _spriteRenderer;

	private SpriteMask _spriteMask;

	private void Start()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteMask = GetComponent<SpriteMask>();
	}

	private void Update()
	{
		if (_spriteMask.sprite != _spriteRenderer.sprite)
		{
			_spriteMask.sprite = _spriteRenderer.sprite;
		}
	}
}
