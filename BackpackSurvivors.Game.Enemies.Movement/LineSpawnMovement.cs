using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

internal class LineSpawnMovement : EnemyMovement
{
	private Vector2 _moveDirection;

	private float _despawnDelay = 1f;

	private Enemy _enemy;

	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.MoveInFixedDirection;
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

	public override void Init(float moveSpeed)
	{
		base.Init(moveSpeed);
		_enemy = GetComponent<Enemy>();
		_enemy.MoveToSpecialMovementLayer();
		InitLineMovementProperties();
		StartCoroutine(DespawnAfterDelay());
	}

	private void InitLineMovementProperties()
	{
		LineSpawnPositionCalculator lineSpawnPositionCalculator = SingletonCacheController.Instance.GetControllerByType<WorldSpawnPositionGenerator>().GetLineSpawnPositionCalculator(WaveChunkName);
		if (lineSpawnPositionCalculator == null)
		{
			Debug.LogWarning("Line Spawn Position Calculator not found for wavechunk '" + WaveChunkName + "'");
			return;
		}
		_despawnDelay = lineSpawnPositionCalculator.DespawnDelay;
		_moveDirection = lineSpawnPositionCalculator.GetMovementDirection();
	}

	internal override void CalculateNewPosition()
	{
		if (SingletonController<GameController>.Instance.Player.IsDead)
		{
			PreventMovement();
		}
		else if (_executeMovement)
		{
			Vector3 target = base.transform.position + 100f * (Vector3)_moveDirection;
			Vector3 vector = Vector3.MoveTowards(base.transform.position, target, base.MoveSpeed * Time.fixedDeltaTime * _movementScale);
			SetNewPosition(vector);
		}
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return true;
	}
}
