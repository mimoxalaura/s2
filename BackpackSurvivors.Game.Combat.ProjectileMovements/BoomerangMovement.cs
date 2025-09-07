using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

internal class BoomerangMovement : MonoBehaviour, IProjectileMovement
{
	private bool _allowMovement = true;

	private Vector2 _startPosition;

	private float _range;

	private bool _isReturning;

	private AnimationCurve _speedCurve;

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		if (!_allowMovement)
		{
			return currentPosition;
		}
		TriggerReturn(currentPosition);
		Vector2 target = (_isReturning ? _startPosition : targetPosition);
		float maxMovement = GetMaxMovement(maxMovementPerFrame, currentPosition);
		return Vector2.MoveTowards(currentPosition, target, maxMovement);
	}

	private float GetMaxMovement(float maxMovement, Vector2 currentPosition)
	{
		float time = Vector2.Distance(_startPosition, currentPosition) / _range;
		float num = _speedCurve.Evaluate(time);
		return maxMovement * num;
	}

	private void TriggerReturn(Vector2 currentPosition)
	{
		if (!_isReturning && Vector2.Distance(currentPosition, _startPosition) > _range)
		{
			_isReturning = true;
		}
	}

	internal void Init(Vector2 startPosition, float range, AnimationCurve speedCurve)
	{
		_startPosition = startPosition;
		_range = range;
		_speedCurve = speedCurve;
	}

	public bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition)
	{
		float num = Vector2.Distance(_startPosition, currentPosition);
		if (_isReturning)
		{
			return num <= 0.01f;
		}
		return false;
	}

	public void ToggleMovement(bool allowMovement)
	{
		_allowMovement = allowMovement;
	}
}
