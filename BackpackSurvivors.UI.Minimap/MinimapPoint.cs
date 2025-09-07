using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Minimap;

public class MinimapPoint : MonoBehaviour
{
	[SerializeField]
	private Transform _playerMinimapTransform;

	[SerializeField]
	private Enums.MinimapType _minimapType;

	[SerializeField]
	private bool _stickInsideMinimapBounds;

	[SerializeField]
	private StayInsideSquare _stayInsideSquare;

	[SerializeField]
	private Sprite _minimapSpriteOverride;

	private SpriteRenderer _spriteRenderer;

	internal void Disable()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		switch (_minimapType)
		{
		case Enums.MinimapType.Player:
			_spriteRenderer.color = Color.blue;
			TryAndBindPlayerCamera();
			break;
		case Enums.MinimapType.Enemy:
			_spriteRenderer.color = Color.red;
			TryBindIfNeeded();
			break;
		case Enums.MinimapType.Friendly:
			_spriteRenderer.color = Color.green;
			TryBindIfNeeded();
			break;
		case Enums.MinimapType.Loot:
			_spriteRenderer.color = Color.white;
			TryBindIfNeeded();
			break;
		case Enums.MinimapType.Interactable:
			_spriteRenderer.color = Color.yellow;
			TryBindIfNeeded();
			break;
		case Enums.MinimapType.OverrideWithIcon:
			_spriteRenderer.sprite = _minimapSpriteOverride;
			TryBindIfNeeded();
			return;
		case Enums.MinimapType.Projectile:
			_spriteRenderer.color = Color.cyan;
			break;
		}
		_spriteRenderer.enabled = true;
	}

	private void TryAndBindPlayerCamera()
	{
		if (_playerMinimapTransform != null)
		{
			Minimap minimap = Object.FindFirstObjectByType<Minimap>();
			if (minimap != null)
			{
				minimap.SetCameraToFollow(_playerMinimapTransform);
			}
		}
	}

	private void TryBindIfNeeded()
	{
		if (_stickInsideMinimapBounds && _stayInsideSquare != null)
		{
			BindCamera();
		}
		else
		{
			_stayInsideSquare.EnableClamping(enabled: false);
		}
	}

	public void ActivateMinimapClamp()
	{
		BindCamera();
		_stayInsideSquare.EnableClamping(enabled: true);
	}

	private void BindCamera()
	{
		GameObject gameObject = GameObject.Find("MinimapCamera");
		if (gameObject != null)
		{
			_stayInsideSquare.Init(gameObject.transform, 10f);
			_stayInsideSquare.EnableClamping(_stickInsideMinimapBounds);
		}
	}
}
