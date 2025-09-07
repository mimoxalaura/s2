using System;
using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

public class CirclingTransformMovement : MonoBehaviour, IProjectileMovement
{
	private float _rotationsPerSecond;

	private float _distanceToTransform;

	private float _distanceIncreasePerSecond;

	private Enums.ProjectileMovementFreedom _projectileMovementFreedom;

	private Transform _transformToCircle;

	private float _angleToTransform;

	private bool _isinitialized;

	private WeaponAttack _weaponAttack;

	private Transform _dummyTransform;

	private bool _allowMovement = true;

	private void Awake()
	{
		_weaponAttack = GetComponent<WeaponAttack>();
	}

	private void Update()
	{
		if (_isinitialized)
		{
			UpdateAngle();
			UpdateDistanceToTransform();
		}
	}

	public void ToggleMovement(bool allowMovement)
	{
		_allowMovement = allowMovement;
	}

	public void Init(Enums.ProjectileMovementFreedom projectileMovementFreedom, Transform circleCentre, float rotationsPerSecond = 1f, float distanceToTransform = 2f, float distanceIncreasePerSecond = 0f)
	{
		_rotationsPerSecond = rotationsPerSecond;
		_distanceToTransform = distanceToTransform;
		_distanceIncreasePerSecond = distanceIncreasePerSecond;
		_projectileMovementFreedom = projectileMovementFreedom;
		SetTransformToCircle(circleCentre);
		_isinitialized = true;
	}

	private void SetTransformToCircle(Transform source)
	{
		_transformToCircle = source;
	}

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFramee)
	{
		if (!_allowMovement)
		{
			return currentPosition;
		}
		if (_transformToCircle == null)
		{
			Debug.Log("TransformToCircle not set");
			return currentPosition;
		}
		float x = _distanceToTransform * Mathf.Cos(_angleToTransform);
		float y = _distanceToTransform * Mathf.Sin(_angleToTransform);
		Vector2 vector = new Vector2(x, y);
		return (Vector2)_transformToCircle.position + vector;
	}

	private void UpdateDistanceToTransform()
	{
		_distanceToTransform += _distanceIncreasePerSecond * Time.deltaTime;
	}

	private void UpdateAngle()
	{
		_angleToTransform += _rotationsPerSecond * Time.deltaTime * 360f * (MathF.PI / 180f);
	}

	public bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition)
	{
		return false;
	}

	private void OnDestroy()
	{
		if (_dummyTransform != null)
		{
			UnityEngine.Object.Destroy(_dummyTransform.gameObject);
		}
	}
}
