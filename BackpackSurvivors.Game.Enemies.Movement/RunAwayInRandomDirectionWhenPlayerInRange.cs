using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

internal class RunAwayInRandomDirectionWhenPlayerInRange : EnemyMovement
{
	[SerializeField]
	private float _runAwayFromPlayerWithinRange;

	[SerializeField]
	private float _runAwayMovementspeedPercentageOfMoveSpeed = 1f;

	private bool _isRunning;

	private WorldSpawnPositionGenerator _spawnPositionGenerator;

	private Vector2 _targetPosition;

	private bool _targetPositionSet;

	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.RunAwayInRandomDirectionWhenPlayerInRange;
	}

	protected override void AfterStart()
	{
		_spawnPositionGenerator = Object.FindAnyObjectByType<WorldSpawnPositionGenerator>();
		UpdateTargetPosition();
	}

	internal override void CalculateNewPosition()
	{
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		float num = Vector2.Distance(base.transform.position, playerPosition);
		float num2 = Vector2.Distance(base.transform.position, _targetPosition);
		if (num > _runAwayFromPlayerWithinRange || num2 <= float.Epsilon)
		{
			StopRunning();
			return;
		}
		if (!_targetPositionSet)
		{
			UpdateTargetPosition();
		}
		Vector2 vector = Vector2.MoveTowards(base.transform.position, _targetPosition, base.MoveSpeed * _runAwayMovementspeedPercentageOfMoveSpeed * Time.deltaTime);
		if (Vector2.Distance(vector, playerPosition) < num)
		{
			UpdateTargetPosition();
			vector = Vector2.MoveTowards(base.transform.position, _targetPosition, base.MoveSpeed * _runAwayMovementspeedPercentageOfMoveSpeed * Time.deltaTime);
		}
		_newPosition = vector;
		StartRunning();
	}

	private void StopRunning()
	{
		if (_isRunning)
		{
			TriggerMovementStopped();
		}
		_isRunning = false;
		_targetPositionSet = false;
		_newPosition = base.transform.position;
		UpdateTargetPosition();
	}

	private void StartRunning()
	{
		if (!_isRunning)
		{
			TriggerMovementStarted();
		}
		_isRunning = true;
	}

	private void UpdateTargetPosition()
	{
		_targetPosition = _spawnPositionGenerator.GetRandomSpawnPositionAwayFromPlayer(base.transform.position);
		_targetPositionSet = true;
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return true;
	}
}
