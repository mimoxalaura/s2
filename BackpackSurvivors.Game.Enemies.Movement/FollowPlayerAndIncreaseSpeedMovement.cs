using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

public class FollowPlayerAndIncreaseSpeedMovement : EnemyMovement
{
	[SerializeField]
	private float _speedMultiplierPerSecond = 0.01f;

	private float _period = 1f;

	private bool _isIncreasingSpeed = true;

	public float _actualSpeedMultiplier = 1f;

	protected override void AfterStart()
	{
		_actualSpeedMultiplier = 1f;
		StartCoroutine(IncreaseMovementspeedOverTimeAsync());
	}

	private IEnumerator IncreaseMovementspeedOverTimeAsync()
	{
		while (_isIncreasingSpeed)
		{
			yield return new WaitForSeconds(_period);
			_actualSpeedMultiplier += _speedMultiplierPerSecond;
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		_isIncreasingSpeed = false;
	}

	public void ResetSpeed()
	{
		_actualSpeedMultiplier = 1f;
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
			Vector3 vector = Vector3.MoveTowards(base.transform.position, playerPosition, base.MoveSpeed * Time.deltaTime * _movementScale * _actualSpeedMultiplier);
			SetNewPosition(vector);
		}
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return false;
	}
}
