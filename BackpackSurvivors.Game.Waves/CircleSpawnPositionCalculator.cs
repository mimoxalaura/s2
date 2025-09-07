using BackpackSurvivors.ScriptableObjects.Waves;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

public class CircleSpawnPositionCalculator
{
	private Vector2 _centerPosition;

	private float _radius;

	private int _totalNumberOfEnemies;

	private int _numberOfEnemiesPlaced;

	private float _despawnDelay;

	private float _fixedMoveSpeed;

	internal float DespawnDelay => _despawnDelay;

	internal float FixedMoveSpeed => _fixedMoveSpeed;

	internal Vector2 CenterPosition => _centerPosition;

	public CircleSpawnPositionCalculator(Vector2 centerPosition, WaveChunkSO waveChunkSO)
	{
		_centerPosition = centerPosition;
		_radius = waveChunkSO.DistanceFromPlayer;
		_totalNumberOfEnemies = waveChunkSO.NumberOfEnemiesToSpawn;
		_despawnDelay = waveChunkSO.DespawnDelay;
		_fixedMoveSpeed = waveChunkSO.FixedMoveSpeed;
	}

	internal Vector2 GetSpawnPosition()
	{
		float f = 360f / (float)(_totalNumberOfEnemies + 1) * (float)_numberOfEnemiesPlaced;
		float num = _radius * Mathf.Cos(f);
		float num2 = _radius * Mathf.Sin(f);
		_numberOfEnemiesPlaced++;
		return new Vector2(_centerPosition.x + num, _centerPosition.y + num2);
	}
}
