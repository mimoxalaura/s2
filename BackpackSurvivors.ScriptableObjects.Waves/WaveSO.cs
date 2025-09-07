using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Waves;

[CreateAssetMenu(fileName = "Game", menuName = "Game/Waves/Wave", order = 1)]
public class WaveSO : ScriptableObject
{
	[SerializeField]
	public int WaveNumber;

	[SerializeField]
	public List<WaveChunkSO> WaveChunks;

	[SerializeField]
	public float HealthScaleFactor = 1f;

	[SerializeField]
	public float DamageScaleFactor = 1f;

	[SerializeField]
	public float MovementspeedScaleFactor = 1f;

	[SerializeField]
	public float LootScaleFactor = 1f;
}
