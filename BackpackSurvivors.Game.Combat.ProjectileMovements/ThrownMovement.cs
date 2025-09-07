using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

internal class ThrownMovement : MonoBehaviour, IProjectileMovement
{
	private bool _allowMovement = true;

	private Vector2 _startPosition;

	private float _arcHeight;

	private float _timeSpentMoving;

	private float _straightMovementTraveled;

	internal void Init(Vector2 startPosition, float arcHeight)
	{
		_startPosition = startPosition;
		_arcHeight = arcHeight;
	}

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		if (!_allowMovement)
		{
			return currentPosition;
		}
		_timeSpentMoving += Time.deltaTime;
		_straightMovementTraveled += maxMovementPerFrame;
		Vector2.MoveTowards(_startPosition, targetPosition, _straightMovementTraveled);
		float num = Vector2.Distance(_startPosition, targetPosition);
		GetCircleRadius(num, _arcHeight);
		Vector2 vector = (_startPosition + targetPosition) / 2f;
		Vector2 normalized = (targetPosition - _startPosition).normalized;
		Vector2 vector2 = new Vector2(normalized.y, normalized.x);
		Vector2 topOfArc = vector + vector2 * _arcHeight;
		float progressPercentage = _straightMovementTraveled / num;
		return GetArcedPosition(_startPosition, targetPosition, topOfArc, progressPercentage);
	}

	private Vector2 GetArcedPosition(Vector2 startPosition, Vector2 targetPosition, Vector2 topOfArc, float progressPercentage)
	{
		Vector2 a = Vector2.Lerp(startPosition, topOfArc, progressPercentage);
		Vector2 b = Vector2.Lerp(topOfArc, targetPosition, progressPercentage);
		return Vector2.Lerp(a, b, progressPercentage);
	}

	public bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition)
	{
		return Vector2.Distance(targetPosition, currentPosition) <= 0.01f;
	}

	public void ToggleMovement(bool allowMovement)
	{
		_allowMovement = allowMovement;
	}

	private float GetCircleRadius(float width, float arcHeight)
	{
		return arcHeight * 0.5f + width * width / (8f * arcHeight);
	}
}
