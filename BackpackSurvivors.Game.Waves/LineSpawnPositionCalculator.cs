using BackpackSurvivors.ScriptableObjects.Waves;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

public class LineSpawnPositionCalculator
{
	private Vector2 _startPosition;

	private float _distanceBetweenEnemies;

	private int _numberOfEnemiesPlaced;

	private Enums.CardinalDirection _directionTowardsPlayer;

	private Vector2 _distanceBetweenEnemiesMultiplicator;

	private float _despawnDelay;

	internal Enums.CardinalDirection DirectionTowardsPlayer => _directionTowardsPlayer;

	internal float DespawnDelay => _despawnDelay;

	public LineSpawnPositionCalculator(Vector2 startPosition, Enums.CardinalDirection directionTowardsPlayer, WaveChunkSO waveChunkSO)
	{
		_startPosition = startPosition;
		_directionTowardsPlayer = directionTowardsPlayer;
		_distanceBetweenEnemies = waveChunkSO.DistanceBetweenEnemies;
		_despawnDelay = waveChunkSO.DespawnDelay;
		_distanceBetweenEnemiesMultiplicator = GetDistanceBetweenEnemiesMultiplicator(_directionTowardsPlayer);
	}

	internal Vector2 GetSpawnPosition()
	{
		int num = (_numberOfEnemiesPlaced + 1) / 2;
		int num2 = ((_numberOfEnemiesPlaced % 2 != 0) ? 1 : (-1));
		Vector2 vector = (float)(num * num2) * _distanceBetweenEnemies * _distanceBetweenEnemiesMultiplicator;
		_numberOfEnemiesPlaced++;
		return _startPosition + vector;
	}

	internal Vector2 GetMovementDirection()
	{
		return _directionTowardsPlayer switch
		{
			Enums.CardinalDirection.MovingSouth => Vector2.down, 
			Enums.CardinalDirection.MovingNorth => Vector2.up, 
			Enums.CardinalDirection.MovingWest => Vector2.left, 
			Enums.CardinalDirection.MovingEast => Vector2.right, 
			_ => Vector2.down, 
		};
	}

	private Vector2 GetDistanceBetweenEnemiesMultiplicator(Enums.CardinalDirection direction)
	{
		switch (direction)
		{
		case Enums.CardinalDirection.MovingNorth:
		case Enums.CardinalDirection.MovingSouth:
			return Vector2.right;
		case Enums.CardinalDirection.MovingEast:
		case Enums.CardinalDirection.MovingWest:
			return Vector2.up;
		default:
			return Vector2.up;
		}
	}

	public static Vector2 GetStartPosition(Vector2 playerPosition, float leftX, float rightX, float bottomY, float topY, float distanceFromPlayer, out Enums.CardinalDirection directionTowardsPlayer)
	{
		float num = Mathf.Abs(leftX - playerPosition.x);
		float num2 = Mathf.Abs(rightX - playerPosition.x);
		float num3 = Mathf.Abs(bottomY - playerPosition.y);
		float num4 = Mathf.Abs(topY - playerPosition.y);
		float num5 = Mathf.Min(num, num2, num3, num4);
		Vector2 vector = Vector2.right;
		directionTowardsPlayer = Enums.CardinalDirection.MovingWest;
		if (num5 == num)
		{
			vector = Vector2.right;
			directionTowardsPlayer = Enums.CardinalDirection.MovingWest;
		}
		else if (num5 == num2)
		{
			vector = Vector2.left;
			directionTowardsPlayer = Enums.CardinalDirection.MovingEast;
		}
		else if (num5 == num3)
		{
			vector = Vector2.up;
			directionTowardsPlayer = Enums.CardinalDirection.MovingSouth;
		}
		else if (num5 == num4)
		{
			vector = Vector2.down;
			directionTowardsPlayer = Enums.CardinalDirection.MovingNorth;
		}
		return playerPosition + vector * distanceFromPlayer;
	}
}
