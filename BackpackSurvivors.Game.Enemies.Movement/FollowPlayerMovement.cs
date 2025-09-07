using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

public class FollowPlayerMovement : EnemyMovement
{
	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.FollowPlayer;
	}

	internal override void CalculateNewPosition()
	{
		if (SingletonController<GameController>.Instance.Player.IsDead)
		{
			PreventMovement();
		}
		else if (_executeMovement)
		{
			Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
			Vector3 vector = Vector3.MoveTowards(base.transform.position, playerPosition, base.MoveSpeed * Time.fixedDeltaTime * _movementScale);
			SetNewPosition(vector);
		}
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return false;
	}
}
