using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

public class FollowPlayerUntilRange : EnemyMovement
{
	[SerializeField]
	private float _distanceToKeepFromPlayer;

	[SerializeField]
	private Animator _animator;

	private bool _isFollowingPlayer;

	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.FollowPlayerUntilRange;
	}

	internal override void CalculateNewPosition()
	{
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		Vector2 vector = Vector2.MoveTowards(base.transform.position, playerPosition, base.MoveSpeed * Time.deltaTime);
		float num = Vector2.Distance(vector, playerPosition);
		_isFollowingPlayer = true;
		if (num < _distanceToKeepFromPlayer)
		{
			PreventMovement();
			if (_animator != null)
			{
				_animator.SetBool("IsMoving", value: false);
			}
			_isFollowingPlayer = false;
		}
		else
		{
			if (_animator != null)
			{
				AllowMovement();
				_animator.SetBool("IsMoving", value: true);
			}
			_newPosition = vector;
		}
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return !_isFollowingPlayer;
	}
}
