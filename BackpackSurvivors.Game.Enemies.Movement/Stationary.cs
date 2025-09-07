using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Enemies.Movement;

public class Stationary : EnemyMovement
{
	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.Stationary;
	}

	internal override void CalculateNewPosition()
	{
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return true;
	}
}
