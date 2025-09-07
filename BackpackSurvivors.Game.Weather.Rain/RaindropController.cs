using System.Collections;
using BackpackSurvivors.Game.Player;
using UnityEngine;

namespace BackpackSurvivors.Game.Weather.Rain;

internal class RaindropController : MonoBehaviour
{
	[SerializeField]
	private BackpackSurvivors.Game.Player.Player _player;

	[SerializeField]
	private Raindrop _prefab;

	[SerializeField]
	private GameObject _parent;

	[SerializeField]
	private bool _enabled;

	[SerializeField]
	private float _durationBetweenSpawns;

	[SerializeField]
	private int _maxSpawns;

	private int _currentSpawns;

	internal void RemovedDroplet()
	{
		_currentSpawns--;
	}

	private void Start()
	{
		StartCoroutine(StartSpawning());
	}

	public Vector3 RandomPointInBounds(Bounds bounds)
	{
		return new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
	}

	public Bounds CreateBoundsAroundPosition(Vector3 position, int horizontalOffset, int verticalOffset)
	{
		return new Bounds(position, new Vector3(horizontalOffset * 2, verticalOffset * 2));
	}

	private IEnumerator StartSpawning()
	{
		while (_enabled)
		{
			if (_currentSpawns < _maxSpawns)
			{
				Raindrop raindrop = Object.Instantiate(_prefab, _parent.transform);
				raindrop.transform.position = RandomPointInBounds(CreateBoundsAroundPosition(_player.transform.position, 8, 5));
				raindrop.Init(this);
				_currentSpawns++;
			}
			yield return new WaitForSeconds(_durationBetweenSpawns);
		}
	}
}
