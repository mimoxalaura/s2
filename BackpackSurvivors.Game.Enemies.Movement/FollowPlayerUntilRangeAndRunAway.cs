using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

internal class FollowPlayerUntilRangeAndRunAway : EnemyMovement
{
	[SerializeField]
	private float _followPlayerUntilRange;

	[SerializeField]
	private float _runAwayFromPlayerWithinRange;

	[SerializeField]
	private float _runAwayMovementspeedPercentageOfMoveSpeed = 1f;

	[SerializeField]
	private Animator _animator;

	private bool _isFollowingPlayer;

	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.FollowPlayerUntilRangeAndRunAway;
	}

	internal override void CalculateNewPosition()
	{
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		float num = Vector2.Distance(base.transform.position, playerPosition);
		bool num2 = num < _runAwayFromPlayerWithinRange || num > _followPlayerUntilRange;
		_isFollowingPlayer = false;
		if (!num2)
		{
			PreventMovement();
			if (_animator != null)
			{
				_animator.SetBool("IsMoving", value: false);
			}
			return;
		}
		AllowMovement();
		if (_animator != null)
		{
			_animator.SetBool("IsMoving", value: true);
		}
		if (num > _followPlayerUntilRange)
		{
			Vector2 newPosition = Vector2.MoveTowards(base.transform.position, playerPosition, base.MoveSpeed * Time.deltaTime);
			_newPosition = newPosition;
			_isFollowingPlayer = true;
		}
		else if (num < _runAwayFromPlayerWithinRange)
		{
			Vector2 vector = (Vector2)base.transform.position - playerPosition;
			Vector2 target = (Vector2)base.transform.position + 100f * vector;
			Vector2 newPosition2 = Vector2.MoveTowards(base.transform.position, target, base.MoveSpeed * _runAwayMovementspeedPercentageOfMoveSpeed * Time.deltaTime);
			_newPosition = newPosition2;
		}
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return !_isFollowingPlayer;
	}
}
