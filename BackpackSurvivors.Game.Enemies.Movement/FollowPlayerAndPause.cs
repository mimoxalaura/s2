using System.Collections;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

public class FollowPlayerAndPause : FollowPlayerMovement
{
	[SerializeField]
	private float _pauseMovementCooldown;

	[SerializeField]
	private float _resumeMovementCooldown;

	private bool _movementIsPaused;

	private float _currentPauseMovementCooldown;

	private float _currentResumeMovementCooldown;

	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.FollowPlayerAndPause;
		_currentPauseMovementCooldown = _resumeMovementCooldown;
	}

	public new void Init(float moveSpeed)
	{
		base.Init(moveSpeed);
	}

	protected override void AfterStart()
	{
		base.AfterStart();
		StartCoroutine(UpdateTimeCoroutine());
	}

	private IEnumerator UpdateTimeCoroutine()
	{
		float interval = 0.1f;
		while (true)
		{
			DecreaseCooldown(interval);
			SwitchMovementBasedOnCooldown();
			yield return new WaitForSeconds(interval);
		}
	}

	private void DecreaseCooldown(float timePassed)
	{
		if (_movementIsPaused)
		{
			_currentResumeMovementCooldown -= timePassed;
		}
		else
		{
			_currentPauseMovementCooldown -= timePassed;
		}
	}

	private void SwitchMovementBasedOnCooldown()
	{
		if (_currentPauseMovementCooldown <= 0f)
		{
			_movementIsPaused = true;
			_currentPauseMovementCooldown = _pauseMovementCooldown;
			SetCanMove(canMove: false);
		}
		else if (_currentResumeMovementCooldown <= 0f)
		{
			_movementIsPaused = false;
			_currentResumeMovementCooldown = _resumeMovementCooldown;
			SetCanMove(canMove: true);
		}
	}
}
