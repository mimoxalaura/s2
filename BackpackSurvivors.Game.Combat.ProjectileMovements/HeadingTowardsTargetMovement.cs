using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

internal class HeadingTowardsTargetMovement : MonoBehaviour, IProjectileMovement
{
	private bool _allowMovement = true;

	public void ToggleMovement(bool allowMovement)
	{
		_allowMovement = allowMovement;
	}

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		if (!_allowMovement)
		{
			return currentPosition;
		}
		return Vector2.MoveTowards(currentPosition, targetPosition, maxMovementPerFrame);
	}

	public bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition)
	{
		return Vector2.Distance(currentPosition, targetPosition) < 0.01f;
	}
}
