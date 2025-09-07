using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.ScriptableObjects.Waves;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Bosses.VoidCorruption;

internal class PortalSpawningWaveChunk : MonoBehaviour
{
	[SerializeField]
	private WaveChunkSO _waveChunkToSpawn;

	[SerializeField]
	private float _delayBeforeSpawning;

	[SerializeField]
	private Vector2 _spawnOffset;

	[SerializeField]
	private Animator _animator;

	private void Start()
	{
		MoveToOffset();
		_animator.SetBool("Closed", value: false);
		StartCoroutine(SpawnWavechunkAfterDelay());
	}

	private void MoveToOffset()
	{
		Vector2 vector = (Vector2)base.transform.position + _spawnOffset;
		base.transform.position = vector;
	}

	private IEnumerator SpawnWavechunkAfterDelay()
	{
		yield return new WaitForSeconds(_delayBeforeSpawning);
		TimeBasedWaveController controllerByType = SingletonCacheController.Instance.GetControllerByType<TimeBasedWaveController>();
		List<WaveChunkSO> waveChunks = new List<WaveChunkSO> { _waveChunkToSpawn };
		if (controllerByType != null)
		{
			controllerByType.SpawnWaveChunkExternal(waveChunks, base.transform.position);
		}
		_animator.SetBool("Closed", value: true);
		yield return new WaitForSeconds(2f);
		Object.Destroy(base.gameObject);
	}
}
