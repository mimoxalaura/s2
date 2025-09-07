using System;
using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

internal class LightningMovement : MonoBehaviour, IProjectileMovement
{
	private float _minMovementToTriggerZigZagChange;

	private float _maxMovementToTriggerZigZagChange;

	private float _straightToTargetWithinYDistance;

	private float _minXZigZagMovement;

	private float _maxXZigZagMovement;

	private Vector2 _zigZagTarget;

	private bool _allowMovement = true;

	public void Init(float minMovementToTriggerZigZagChange, float maxMovementToTriggerZigZagChange, float straightToTargetWithinYDistance, float minXZigZagMovement, float maxXZigZagMovement)
	{
		_minMovementToTriggerZigZagChange = minMovementToTriggerZigZagChange;
		_maxMovementToTriggerZigZagChange = maxMovementToTriggerZigZagChange;
		_straightToTargetWithinYDistance = straightToTargetWithinYDistance;
		_minXZigZagMovement = minXZigZagMovement;
		_maxXZigZagMovement = maxXZigZagMovement;
	}

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		if (!_allowMovement)
		{
			return currentPosition;
		}
		if (ShouldMoveStraightTowardsTarget(currentPosition, targetPosition))
		{
			return Vector2.MoveTowards(currentPosition, targetPosition, maxMovementPerFrame);
		}
		if (ShouldUpdateZigZagTarget(currentPosition, targetPosition))
		{
			UpdateZigZagTarget(currentPosition, targetPosition);
		}
		return Vector2.MoveTowards(currentPosition, _zigZagTarget, maxMovementPerFrame);
	}

	private bool ShouldUpdateZigZagTarget(Vector2 currentPosition, Vector2 targetPosition)
	{
		if (_zigZagTarget == Vector2.zero)
		{
			return true;
		}
		return Vector2.Distance(currentPosition, _zigZagTarget) < float.Epsilon;
	}

	public void ToggleMovement(bool allowMovement)
	{
		_allowMovement = allowMovement;
	}

	private void UpdateZigZagTarget(Vector2 currentPosition, Vector2 targetPosition)
	{
		float x = UnityEngine.Random.Range(_minMovementToTriggerZigZagChange, _maxMovementToTriggerZigZagChange);
		float num = targetPosition.x - currentPosition.x;
		float num2 = UnityEngine.Random.Range(_minXZigZagMovement, _maxXZigZagMovement);
		int num3 = ((num > 0f) ? 1 : (-1));
		num2 *= (float)num3;
		float num4 = currentPosition.x + num2;
		float x2 = num4 - currentPosition.x;
		float num5 = Mathf.Sqrt(MathF.Pow(x, 2f) - MathF.Pow(x2, 2f));
		float y = currentPosition.y - num5;
		_zigZagTarget = new Vector2(num4, y);
	}

	private bool ShouldMoveStraightTowardsTarget(Vector2 currentPosition, Vector2 targetPosition)
	{
		return currentPosition.y - targetPosition.y <= _straightToTargetWithinYDistance;
	}

	public bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition)
	{
		return false;
	}
}
