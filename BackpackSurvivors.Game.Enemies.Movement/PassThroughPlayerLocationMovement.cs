using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

internal class PassThroughPlayerLocationMovement : EnemyMovement
{
	[SerializeField]
	private float _targetLocationDistancePastPlayer;

	[SerializeField]
	private float _timeBeforeTargetCalculation;

	private Vector2 _targetPosition;

	private bool _targetCalculationCooldownFinished;

	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.PassThroughPlayerLocation;
	}

	internal override void CalculateNewPosition()
	{
		if (SingletonController<GameController>.Instance.Player.IsDead)
		{
			PreventMovement();
			return;
		}
		if (ShouldCalculateTargetPosition())
		{
			CalculateTargetPosition();
		}
		Vector3 vector = Vector3.MoveTowards(base.transform.position, _targetPosition, base.MoveSpeed * Time.fixedDeltaTime * _movementScale);
		SetNewPosition(vector);
	}

	private void CalculateTargetPosition()
	{
		Vector3 vector = base.transform.InverseTransformPoint(SingletonController<GameController>.Instance.PlayerPosition).normalized * _targetLocationDistancePastPlayer;
		_targetPosition = SingletonController<GameController>.Instance.PlayerPosition + (Vector2)vector;
		_targetCalculationCooldownFinished = false;
		StartCoroutine(SetTargetCalculationCooldownFinished());
	}

	private bool ShouldCalculateTargetPosition()
	{
		if (_targetCalculationCooldownFinished)
		{
			return true;
		}
		if (_targetPosition == Vector2.zero)
		{
			return true;
		}
		if (Vector2.Distance(base.transform.position, _targetPosition) <= float.Epsilon)
		{
			return true;
		}
		return false;
	}

	private IEnumerator SetTargetCalculationCooldownFinished()
	{
		yield return new WaitForSeconds(_timeBeforeTargetCalculation);
		_targetCalculationCooldownFinished = true;
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return true;
	}
}
