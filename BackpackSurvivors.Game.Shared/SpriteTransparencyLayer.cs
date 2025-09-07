using BackpackSurvivors.Game.Player;
using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class SpriteTransparencyLayer : MonoBehaviour
{
	[SerializeField]
	private bool _changeTransparencyOnAbove;

	[SerializeField]
	private float _transparencyOnAbove = 0.5f;

	private SpriteRenderer _renderer;

	private float _originalTransparency;

	private void Start()
	{
		_renderer = GetComponent<SpriteRenderer>();
		_originalTransparency = _renderer.color.a;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent<BackpackSurvivors.Game.Player.Player>(out var _) && _changeTransparencyOnAbove)
		{
			LeanTween.value(base.gameObject, delegate(float val)
			{
				_renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, val);
			}, 1f, _transparencyOnAbove, 0.2f);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent<BackpackSurvivors.Game.Player.Player>(out var _) && _changeTransparencyOnAbove)
		{
			LeanTween.value(base.gameObject, delegate(float val)
			{
				_renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, val);
			}, _transparencyOnAbove, _originalTransparency, 0.2f);
		}
	}
}
