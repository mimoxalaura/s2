using System;
using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

internal class BurrowingMovement : EnemyMovement
{
	private enum BurrowingMovementType
	{
		TowardsPlayer,
		Burrowing
	}

	[SerializeField]
	private float _burrowingSpeed;

	[SerializeField]
	private Animator _burrowAbleAnimator;

	[SerializeField]
	private float _burrowDuration;

	private Vector2 _targetPosition;

	private BurrowingMovementType _movementType;

	private bool _burrowTargetSet;

	private Enemy _enemy;

	private WorldBounds _worldBounds;

	private bool _burrowing;

	private void Awake()
	{
		RegisterComponents();
		EnemyMovementType = Enums.EnemyMovementType.MoveInFixedDirection;
	}

	protected override void AfterStart()
	{
		RegisterObjects();
		SetInitialTargetPosition();
	}

	private void RegisterObjects()
	{
		_worldBounds = SingletonCacheController.Instance.GetControllerByType<WorldBounds>();
	}

	private void SetInitialTargetPosition()
	{
		_targetPosition = SingletonController<GameController>.Instance.PlayerPosition;
	}

	private void RegisterComponents()
	{
		_enemy = GetComponentInParent<Enemy>();
	}

	private IEnumerator EmergeIfBurrowTargetReached()
	{
		yield return new WaitForSeconds(_burrowDuration);
		_enemy.SetCanAct(canAct: true);
		_burrowAbleAnimator.SetBool("Underground", value: false);
		_burrowAbleAnimator.SetTrigger("Appear");
		_enemy.SetColliderEnabled(enabled: true);
		_enemy.SetDamageOnTouchCanAct(canAct: true);
		_movementType = BurrowingMovementType.TowardsPlayer;
		SetTargetPosition();
	}

	internal override void CalculateNewPosition()
	{
		if (_burrowing)
		{
			float maxMovementThisFrame = GetMaxMovementThisFrame();
			Vector3 vector = Vector3.MoveTowards(base.transform.position, _targetPosition, maxMovementThisFrame);
			SetNewPosition(vector);
		}
	}

	internal void Burrow()
	{
		if (_movementType != BurrowingMovementType.Burrowing)
		{
			_burrowAbleAnimator.SetBool("Underground", value: true);
			_burrowAbleAnimator.SetTrigger("Disappear");
			_enemy.SetColliderEnabled(enabled: false);
			_enemy.SetImmunityForDuration(Enums.ImmunitySource.Burrowed, _burrowDuration);
			_enemy.SetDamageOnTouchCanAct(canAct: false);
			_enemy.SetCanAct(canAct: false);
			_movementType = BurrowingMovementType.Burrowing;
			_burrowTargetSet = false;
			_burrowing = true;
			LeanTween.delayedCall(1f, (Action)delegate
			{
				SetTargetPosition();
				StartCoroutine(EmergeIfBurrowTargetReached());
				_burrowing = false;
			});
		}
	}

	private void SetTargetPosition()
	{
		switch (_movementType)
		{
		case BurrowingMovementType.TowardsPlayer:
			SetPlayerAsTarget();
			break;
		case BurrowingMovementType.Burrowing:
			SetBurrowingTarget();
			break;
		}
	}

	private void SetBurrowingTarget()
	{
		if (!_burrowTargetSet)
		{
			Vector2 vector = SingletonController<GameController>.Instance.PlayerPosition - (Vector2)base.transform.position;
			_targetPosition = (Vector2)base.transform.position + 2f * vector;
			_targetPosition = _worldBounds.MovePositionWithinWorldBounds(_targetPosition);
			_burrowTargetSet = true;
			float burrowDuration = Vector2.Distance(base.transform.position, _targetPosition) / _burrowingSpeed;
			_burrowDuration = burrowDuration;
		}
	}

	private void SetPlayerAsTarget()
	{
		_targetPosition = SingletonController<GameController>.Instance.PlayerPosition;
	}

	private float GetMaxMovementThisFrame()
	{
		switch (_movementType)
		{
		case BurrowingMovementType.TowardsPlayer:
			return base.MoveSpeed * Time.fixedDeltaTime * _movementScale;
		case BurrowingMovementType.Burrowing:
			return _burrowingSpeed * Time.fixedDeltaTime * _movementScale;
		default:
			Debug.LogWarning(string.Format("Movement Type {0} is not handled in {1}.{2}", _movementType, "BurrowingMovement", "GetMaxMovementThisFrame"));
			return 0f;
		}
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return true;
	}
}
