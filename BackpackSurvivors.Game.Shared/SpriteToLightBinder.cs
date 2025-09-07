using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.Game.Shared;

internal class SpriteToLightBinder : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Light2D _light2D;

	private void Update()
	{
		if (_light2D.lightCookieSprite != _spriteRenderer.sprite)
		{
			_light2D.lightCookieSprite = _spriteRenderer.sprite;
		}
	}
}
