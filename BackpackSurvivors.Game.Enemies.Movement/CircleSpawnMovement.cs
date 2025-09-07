using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

internal class CircleSpawnMovement : EnemyMovement
{
	private Vector3 _fixedTargetPosition;

	private float _fixedMoveSpeed = 0.25f;

	private float _despawnDelay = 1f;

	private Enemy _enemy;

	public override void Init(float moveSpeed)
	{
		base.Init(moveSpeed);
		_enemy = GetComponent<Enemy>();
		_enemy.MoveToSpecialMovementLayer();
		InitCircleMovementProperties();
		StartCoroutine(DespawnAfterDelay());
	}

	private void InitCircleMovementProperties()
	{
		CircleSpawnPositionCalculator circleSpawnPositionCalculator = SingletonCacheController.Instance.GetControllerByType<WorldSpawnPositionGenerator>().GetCircleSpawnPositionCalculator(WaveChunkName);
		if (circleSpawnPositionCalculator == null)
		{
			Debug.LogWarning("Circle Spawn Position Calculator not found for wavechunk '" + WaveChunkName + "'");
			return;
		}
		_fixedTargetPosition = circleSpawnPositionCalculator.CenterPosition;
		_fixedMoveSpeed = circleSpawnPositionCalculator.FixedMoveSpeed;
		_despawnDelay = circleSpawnPositionCalculator.DespawnDelay;
	}

	private IEnumerator DespawnAfterDelay()
	{
		yield return new WaitForSeconds(_despawnDelay);
		Enemy componentInParent = GetComponentInParent<Enemy>();
		if (!(componentInParent == null))
		{
			componentInParent.DestroyWithoutKilling();
		}
	}

	internal override void CalculateNewPosition()
	{
		if (SingletonController<GameController>.Instance.Player.IsDead)
		{
			PreventMovement();
		}
		else if (_executeMovement)
		{
			Vector3 vector = Vector3.MoveTowards(base.transform.position, _fixedTargetPosition, _fixedMoveSpeed * Time.fixedDeltaTime * _movementScale);
			SetNewPosition(vector);
		}
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return true;
	}
}
