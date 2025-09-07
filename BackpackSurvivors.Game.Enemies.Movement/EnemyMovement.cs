using System;
using System.Collections;
using BackpackSurvivors.Game.Enemies.SpatialPartitioning;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

public abstract class EnemyMovement : MonoBehaviour
{
	public delegate void StoppedRunningHandler(object sender, EventArgs e);

	public delegate void StartedRunningHandler(object sender, EventArgs e);

	[SerializeField]
	private Rigidbody2D _rigidBody;

	[SerializeField]
	private Collider2D _collider;

	[SerializeField]
	private float _knockbackDuration = 0.15f;

	internal Enums.EnemyMovementType EnemyMovementType;

	internal EventHandler OnCanMoveChanged;

	protected float _movementScale = 1f;

	protected Vector2 _newPosition;

	protected bool _executeMovement = true;

	internal string WaveChunkName;

	private Enemy _parentEnemy;

	private Vector2 _knockbackMovement;

	private float _currentKnockbackDuration;

	internal float BaseMoveSpeed { get; private set; }

	internal float MoveSpeed { get; private set; }

	internal bool CanMove { get; private set; }

	internal Rigidbody2D RigidBody => _rigidBody;

	internal Collider2D Collider2D => _collider;

	internal event StoppedRunningHandler OnStoppedRunning;

	internal event StoppedRunningHandler OnStartedRunning;

	private void Start()
	{
		_parentEnemy = GetComponentInParent<Enemy>();
		AfterStart();
	}

	protected virtual void AfterStart()
	{
	}

	internal virtual void Move()
	{
		if (!SingletonController<GameController>.Instance.GamePaused && _executeMovement)
		{
			CalculateNewPosition();
			ApplyKnockback();
			TryMoveRigidbody();
		}
	}

	internal void Teleport(Vector2 newPosition)
	{
		_newPosition = newPosition;
		TryMoveRigidbody();
	}

	internal void SetKnockbackMovement(Vector2 knockbackMovement)
	{
		_knockbackMovement = knockbackMovement;
		_currentKnockbackDuration = _knockbackDuration;
	}

	internal abstract bool MovementShouldIgnoreCollisions();

	private bool TryMoveRigidbody()
	{
		bool num = SpatialController.Instance.IsTargetPositionFree(_parentEnemy, _newPosition);
		if (num)
		{
			MoveRigidBody(_newPosition);
		}
		return num;
	}

	private void ApplyKnockback()
	{
		if (!(_currentKnockbackDuration <= 0f))
		{
			_newPosition += _knockbackMovement;
			DecreaseKnockbackDuration();
		}
	}

	private void DecreaseKnockbackDuration()
	{
		_currentKnockbackDuration -= Time.deltaTime;
		_currentKnockbackDuration = Mathf.Clamp(_currentKnockbackDuration, 0f, _currentKnockbackDuration);
		_knockbackMovement *= _currentKnockbackDuration / _knockbackDuration;
	}

	public void SetCanMove(bool canMove)
	{
		CanMove = canMove;
		OnCanMoveChanged?.Invoke(this, null);
	}

	public void ToggleLockPosition(bool locked)
	{
		if (locked)
		{
			_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
		}
		else
		{
			_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
		}
	}

	public void ScaleMovementSpeed(float scale)
	{
		_movementScale = scale;
	}

	public virtual void Init(float moveSpeed)
	{
		MoveSpeed = moveSpeed;
		BaseMoveSpeed = moveSpeed;
	}

	public void ChangeMovementSpeed(float speedModifier)
	{
		MoveSpeed = BaseMoveSpeed * speedModifier;
	}

	internal abstract void CalculateNewPosition();

	protected void SetNewPosition(Vector2 newPosition)
	{
		_newPosition = newPosition;
		_executeMovement = true;
	}

	internal void PreventMovement()
	{
		_executeMovement = false;
	}

	internal void AllowMovement()
	{
		_executeMovement = true;
	}

	private void UpdateSpatialGroup()
	{
		int spatialGroupID = SpatialController.Instance.GetSpatialGroupID(base.transform.position);
		if (spatialGroupID != _parentEnemy.SpatialGroupID)
		{
			SpatialController.Instance.RemoveFromSpatialGroup(_parentEnemy.SpatialGroupID, _parentEnemy);
			SpatialController.Instance.AddToSpatialGroup(spatialGroupID, _parentEnemy);
		}
	}

	private void MoveRigidBody(Vector2 newPosition)
	{
		if (CanMove && _executeMovement)
		{
			_rigidBody.MovePosition(newPosition);
			UpdateSpatialGroup();
		}
	}

	internal void StopMovement()
	{
		_rigidBody.velocity = Vector3.zero;
	}

	internal void EnableAfterDelay(float delay)
	{
		StartCoroutine(EnableMovementAfterStun(delay));
	}

	private IEnumerator EnableMovementAfterStun(float delay)
	{
		yield return new WaitForSeconds(delay);
		SetMass(1f);
		SetCanMove(canMove: true);
	}

	internal void SetMass(float mass)
	{
		_rigidBody.mass = mass;
	}

	internal void TriggerMovementStopped()
	{
		this.OnStoppedRunning?.Invoke(this, new EventArgs());
	}

	internal void TriggerMovementStarted()
	{
		this.OnStartedRunning?.Invoke(this, new EventArgs());
	}

	internal void SetRigidBody(Rigidbody2D rigidbody)
	{
		_rigidBody = rigidbody;
	}

	protected virtual void InitMovementInfo()
	{
	}
}
