using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

internal class RunAwayFromPlayerUntillRange : EnemyMovement
{
	[SerializeField]
	private float _runAwayFromPlayerWithinRange;

	[SerializeField]
	private float _runAwayMovementspeedPercentageOfMoveSpeed = 1f;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private float _randomDirectionVariable;

	private bool _isRunning;

	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.RunAwayFromPlayerUntilRange;
	}

	internal override void CalculateNewPosition()
	{
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		if (Vector2.Distance(base.transform.position, playerPosition) < _runAwayFromPlayerWithinRange)
		{
			float num = Random.Range(0f, _randomDirectionVariable);
			Vector2 vector = (Vector2)base.transform.position - playerPosition * (1f + num);
			Vector2 target = (Vector2)base.transform.position + 100f * (1f + num) * vector;
			Vector2 newPosition = Vector2.MoveTowards(base.transform.position, target, base.MoveSpeed * _runAwayMovementspeedPercentageOfMoveSpeed * Time.deltaTime);
			_newPosition = newPosition;
			if (!_isRunning)
			{
				TriggerMovementStarted();
			}
			_isRunning = true;
		}
		else
		{
			if (_isRunning)
			{
				TriggerMovementStopped();
			}
			_isRunning = false;
		}
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return true;
	}
}
