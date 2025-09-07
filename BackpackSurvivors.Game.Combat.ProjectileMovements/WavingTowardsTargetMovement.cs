using System;
using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.ProjectileMovements;

internal class WavingTowardsTargetMovement : MonoBehaviour, IProjectileMovement
{
	private Vector2 _startPosition;

	private float _magnitude;

	private float _frequency;

	private float _straightMovementTraveled;

	private Vector2 _travelVector;

	private Vector2 _perpendicularVector;

	private bool _vectorsInitialized;

	private float _timeSpentMoving;

	private bool _allowMovement = true;

	internal void Init(Vector2 startPosition, float magnitude, float frequency)
	{
		_startPosition = startPosition;
		_magnitude = magnitude;
		_frequency = frequency;
	}

	public void ToggleMovement(bool allowMovement)
	{
		_allowMovement = allowMovement;
	}

	public Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		if (!_allowMovement)
		{
			return currentPosition;
		}
		InitVectors(currentPosition, targetPosition);
		_timeSpentMoving += Time.deltaTime;
		_straightMovementTraveled += maxMovementPerFrame;
		Vector2 vector = Vector2.MoveTowards(_startPosition, targetPosition, _straightMovementTraveled);
		float waveMovement = GetWaveMovement();
		Vector2 vector2 = _perpendicularVector * waveMovement;
		return vector + vector2;
	}

	private void InitVectors(Vector2 currentPosition, Vector2 targetPosition)
	{
		if (!_vectorsInitialized)
		{
			_startPosition = currentPosition;
			_travelVector = (targetPosition - currentPosition).normalized;
			_perpendicularVector = new Vector2(_travelVector.y * -1f, _travelVector.x);
			_vectorsInitialized = true;
		}
	}

	private float GetWaveMovement()
	{
		return MathF.Sin(_timeSpentMoving * _frequency) * _magnitude;
	}

	public bool TargetPositionReached(Vector2 currentPosition, Vector2 targetPosition)
	{
		return Vector2.Distance(Vector2.MoveTowards(_startPosition, targetPosition, _straightMovementTraveled), targetPosition) < float.Epsilon;
	}
}
