using UnityEngine;

public class VisualWarningController : MonoBehaviour
{
	[SerializeField]
	private WarningAreaOfEffect _warningAreaOfEffectPrefab;

	[SerializeField]
	private WarningLineAreaOfEffect _warningLineAreaOfEffectPrefab;

	[SerializeField]
	private Transform _warningAreaOfEffectContainer;

	public void SpawnWarning(Vector3 position, float radius, float delay)
	{
		Object.Instantiate(_warningAreaOfEffectPrefab, _warningAreaOfEffectContainer).Init(position, radius, delay);
	}

	public void SpawnWarning(Vector3 startPosition, float width, Vector3 targetPosition, float delay)
	{
		Object.Instantiate(_warningLineAreaOfEffectPrefab, _warningAreaOfEffectContainer).Init(startPosition, width, targetPosition, delay);
	}

	public void DEBUG_SPAWNRANDOMAREA()
	{
		float x = Random.Range(-100f, -109f);
		float y = Random.Range(-97f, -104f);
		float radius = Random.Range(1f, 4f);
		float delay = Random.Range(1.5f, 3.5f);
		SpawnWarning(new Vector3(x, y, 0f), radius, delay);
	}

	public void DEBUG_SPAWNRANDOMLINE()
	{
		float x = Random.Range(-5f, 5f);
		float y = Random.Range(-5f, 5f);
		float x2 = Random.Range(-5f, 5f);
		float y2 = Random.Range(-5f, 5f);
		float width = Random.Range(1.5f, 4.5f);
		float delay = Random.Range(1.5f, 3.5f);
		SpawnWarning(new Vector3(x, y, 0f), width, new Vector3(x2, y2, 0f), delay);
	}
}
