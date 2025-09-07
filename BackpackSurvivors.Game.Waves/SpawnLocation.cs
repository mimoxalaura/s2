using BackpackSurvivors.Assets.Game.Waves;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

public class SpawnLocation : MonoBehaviour
{
	[SerializeField]
	private Enums.SpawnDirection _spawnDirection;

	private Vector2 _groupedSpawnPosition;

	private bool _isGroupedSpawnPositionSet;

	private OverlappingArea _overlappingArea;

	private bool _overlappingAreaIsCalculated;

	private WorldSpawnLocation[] _allWorldSpawnLocations;

	public Enums.SpawnDirection SpawnDirection => _spawnDirection;

	private float _minX => base.transform.position.x - base.transform.localScale.x / 2f;

	private float _maxX => base.transform.position.x + base.transform.localScale.x / 2f;

	private float _minY => base.transform.position.y - base.transform.localScale.y / 2f;

	private float _maxY => base.transform.position.y + base.transform.localScale.y / 2f;

	private void Start()
	{
		_allWorldSpawnLocations = Object.FindObjectsOfType<WorldSpawnLocation>();
	}

	public bool HasValidSpawnArea()
	{
		OverlappingArea validSpawnArea = GetValidSpawnArea();
		if (validSpawnArea.IsValid)
		{
			return validSpawnArea.GetAreaSize() > 0f;
		}
		return false;
	}

	public Vector2 GetRandomPositionWithinSpawnbounds()
	{
		OverlappingArea validSpawnArea = GetValidSpawnArea();
		float x = Random.Range(validSpawnArea.MinX, validSpawnArea.MaxX);
		float y = Random.Range(validSpawnArea.MinY, validSpawnArea.MaxY);
		return new Vector2(x, y);
	}

	private void LogSpawnArea(OverlappingArea overlappingArea)
	{
		Debug.Log($"Player location: {SingletonController<GameController>.Instance.PlayerPosition}");
		Debug.Log($"TopLeft: {overlappingArea.MinX}.{overlappingArea.MaxY}");
		Debug.Log($"TopRight: {overlappingArea.MaxX}.{overlappingArea.MaxY}");
		Debug.Log($"BottomLeft: {overlappingArea.MinX}.{overlappingArea.MinY}");
		Debug.Log($"BottomRight: {overlappingArea.MaxX}.{overlappingArea.MinY}");
	}

	private OverlappingArea GetValidSpawnArea()
	{
		if (_overlappingAreaIsCalculated)
		{
			return _overlappingArea;
		}
		float num = 0f;
		WorldSpawnLocation[] allWorldSpawnLocations = _allWorldSpawnLocations;
		foreach (WorldSpawnLocation worldSpawnLocation in allWorldSpawnLocations)
		{
			if (WorldSpawnLocationOverlapsThis(worldSpawnLocation))
			{
				OverlappingArea overlappingArea = GetOverlappingArea(worldSpawnLocation);
				if (!(overlappingArea.GetAreaSize() <= num))
				{
					num = overlappingArea.GetAreaSize();
					_overlappingArea = overlappingArea;
				}
			}
		}
		if (_overlappingArea == null)
		{
			_overlappingArea = new OverlappingArea(_minX, _maxX, _minY, _maxY, isValid: false);
		}
		_overlappingAreaIsCalculated = true;
		return _overlappingArea;
	}

	private bool WorldSpawnLocationOverlapsThis(WorldSpawnLocation worldSpawnLocation)
	{
		bool num = _maxX > worldSpawnLocation.MinX && _minX < worldSpawnLocation.MaxX;
		bool flag = _maxY > worldSpawnLocation.MinY && _minY < worldSpawnLocation.MaxY;
		return num && flag;
	}

	private OverlappingArea GetOverlappingArea(WorldSpawnLocation worldSpawnLocation)
	{
		float minX = Mathf.Max(_minX, worldSpawnLocation.MinX);
		float maxX = Mathf.Min(_maxX, worldSpawnLocation.MaxX);
		float minY = Mathf.Max(_minY, worldSpawnLocation.MinY);
		float maxY = Mathf.Min(_maxY, worldSpawnLocation.MaxY);
		return new OverlappingArea(minX, maxX, minY, maxY);
	}

	public void ResetState()
	{
		_isGroupedSpawnPositionSet = false;
		_overlappingAreaIsCalculated = false;
	}

	public Vector2 GetGroupedSpawnPosition()
	{
		if (!_isGroupedSpawnPositionSet)
		{
			SetGroupedSpawnPosition();
		}
		return GetNearbyPosition(_groupedSpawnPosition);
	}

	private Vector2 GetNearbyPosition(Vector2 randomPosition, float maxXOffset = 2f, float maxYOffset = 2f)
	{
		float num = Random.Range(0f - maxXOffset, maxXOffset);
		float num2 = Random.Range(0f - maxYOffset, maxYOffset);
		return new Vector2(randomPosition.x + num, randomPosition.y + num2);
	}

	private void SetGroupedSpawnPosition()
	{
		_groupedSpawnPosition = GetRandomPositionWithinSpawnbounds();
		_isGroupedSpawnPositionSet = true;
	}
}
