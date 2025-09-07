using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;

internal interface IProjectileMovement
{
	Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame);

	bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition);

	void ToggleMovement(bool allowMovement);
}
