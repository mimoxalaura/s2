using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Waves;

[CreateAssetMenu(fileName = "Game", menuName = "Game/Waves/Chunk", order = 1)]
public class WaveChunkSO : ScriptableObject
{
	[SerializeField]
	public EnemySO Enemy;

	[SerializeField]
	public int NumberOfEnemiesToSpawn;

	[SerializeField]
	public float StartSpawningAfterDelay;

	[SerializeField]
	public float SpawnDelayBetweenEnemies;

	[SerializeField]
	public Enums.WavePositionSpawnType WavePositionSpawnType;

	[SerializeField]
	public Enums.SpawnDirection SpawnLocation;

	[SerializeField]
	public bool BlockSpawn;

	[Header("SpawnType Circle/Line")]
	[SerializeField]
	public float DistanceFromPlayer;

	[SerializeField]
	public float DespawnDelay;

	[Header("SpawnType Line")]
	[SerializeField]
	public float DistanceBetweenEnemies;

	[Header("SpawnType Circle")]
	[SerializeField]
	public float FixedMoveSpeed;

	[Header("SpawnLocation")]
	[SerializeField]
	public Vector2 ForcedSpawnLocation;

	private void RenameFile()
	{
	}
}
