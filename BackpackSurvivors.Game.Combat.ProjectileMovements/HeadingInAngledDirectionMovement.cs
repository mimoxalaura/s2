using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

internal class HeadingInAngledDirectionMovement : MonoBehaviour, IProjectileMovement
{
	private Vector2 _targetPosition;

	private bool _allowMovement = true;

	internal void SetTargetPosition(Vector2 targetPositionOffset)
	{
		_targetPosition = (Vector2)base.transform.position + targetPositionOffset;
	}

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		if (!_allowMovement)
		{
			return currentPosition;
		}
		return Vector2.MoveTowards(currentPosition, _targetPosition, maxMovementPerFrame);
	}

	public bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition)
	{
		return false;
	}

	public void ToggleMovement(bool allowMovement)
	{
		_allowMovement = allowMovement;
	}
}
