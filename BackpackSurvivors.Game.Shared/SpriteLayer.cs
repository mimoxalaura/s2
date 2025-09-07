using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteLayer : MonoBehaviour
{
	[SerializeField]
	private string _sortingLayerAbove;

	[SerializeField]
	private string _sortingLayerBelow;

	[SerializeField]
	private float _offsetY;

	[SerializeField]
	private Transform _parentTransform;

	[Header("DEBUG")]
	[SerializeField]
	private float _playerY;

	[SerializeField]
	private float _parentY;

	private SpriteRenderer _renderer;

	private int _sortingLayerAboveId;

	private int _sortingLayerBelowId;

	private void Start()
	{
		_renderer = GetComponent<SpriteRenderer>();
		_sortingLayerAboveId = SortingLayer.NameToID(_sortingLayerAbove);
		_sortingLayerBelowId = SortingLayer.NameToID(_sortingLayerBelow);
	}

	private void Update()
	{
		_playerY = SingletonController<GameController>.Instance.PlayerPosition.y;
		_parentY = _parentTransform.position.y;
		if (SingletonController<GameController>.Instance.PlayerPosition.y + _offsetY > _parentTransform.position.y)
		{
			if (_renderer.sortingLayerID != _sortingLayerAboveId)
			{
				_renderer.sortingLayerID = _sortingLayerAboveId;
			}
		}
		else if (_renderer.sortingLayerID != _sortingLayerBelowId)
		{
			_renderer.sortingLayerID = _sortingLayerBelowId;
		}
	}
}
