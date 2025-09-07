using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

internal class NoMovement : MonoBehaviour, IProjectileMovement
{
	private bool _allowMovement = true;

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		return currentPosition;
	}

	public bool ShouldRotateToFaceTarget()
	{
		return false;
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
