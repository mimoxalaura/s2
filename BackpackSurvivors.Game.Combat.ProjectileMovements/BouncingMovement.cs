using System.Collections.Generic;
using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Enemies;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

internal class BouncingMovement : MonoBehaviour, IProjectileMovement
{
	private LineRenderer _lineRenderer;

	private Vector2 _targetPosition;

	private WeaponAttack _weaponAttack;

	private List<Enemy> _enemiesHit = new List<Enemy>();

	private bool _shouldGetNewTarget;

	private List<Vector3> _pointsForLineRenderer = new List<Vector3>();

	private bool _allowMovement = true;

	private void Awake()
	{
		_weaponAttack = GetComponent<WeaponAttack>();
		_weaponAttack.OnEnemyHit += WeaponAttack_OnEnemyHit;
	}

	internal void SetLineRenderer(LineRenderer lineRenderer)
	{
		if (!(lineRenderer == null))
		{
			_lineRenderer = Object.Instantiate(lineRenderer);
		}
	}

	public void ToggleMovement(bool allowMovement)
	{
		_allowMovement = allowMovement;
	}

	private void WeaponAttack_OnEnemyHit(List<Enemy> enemiesHit)
	{
		_enemiesHit = enemiesHit;
		_shouldGetNewTarget = true;
		UpdateLineRenderer(_targetPosition);
	}

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		if (!_allowMovement)
		{
			return currentPosition;
		}
		if (_targetPosition == Vector2.zero)
		{
			GetNewTarget(targetPosition);
			UpdateLineRenderer(currentPosition);
		}
		if (_shouldGetNewTarget)
		{
			GetNewTarget(currentPosition);
			_shouldGetNewTarget = false;
		}
		Vector2 vector = Vector2.MoveTowards(currentPosition, _targetPosition, maxMovementPerFrame);
		DrawLineToNewPosition(vector);
		return vector;
	}

	private void DrawLineToNewPosition(Vector2 newPosition)
	{
		if (!(_lineRenderer == null))
		{
			List<Vector3> list = new List<Vector3>();
			list.AddRange(_pointsForLineRenderer);
			list.Add(newPosition);
			_lineRenderer.positionCount = list.Count;
			_lineRenderer.SetPositions(list.ToArray());
		}
	}

	private void UpdateLineRenderer(Vector2 pointToAdd)
	{
		if (!(_lineRenderer == null))
		{
			_pointsForLineRenderer.Add(pointToAdd);
			_lineRenderer.positionCount = _pointsForLineRenderer.Count;
			_lineRenderer.SetPositions(_pointsForLineRenderer.ToArray());
		}
	}

	private void GetNewTarget(Vector2 currentPosition)
	{
	}

	private void OnDestroy()
	{
		if (_lineRenderer != null)
		{
			Object.Destroy(_lineRenderer, 0.3f);
		}
	}

	public bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition)
	{
		return false;
	}
}
