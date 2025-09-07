using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

public class GroupedSpawnPositionCalculator
{
	private Vector2 _startPosition;

	private string _wavechunkName;

	private int _numberOfEnemiesSpawned;

	public GroupedSpawnPositionCalculator(string wavechunkName, Vector2 startPosition)
	{
		_wavechunkName = wavechunkName;
		_startPosition = startPosition;
	}

	internal Vector2 GetSpawnPosition()
	{
		Vector2 result = _startPosition + GetOffset();
		_numberOfEnemiesSpawned++;
		return result;
	}

	private Vector2 GetOffset()
	{
		int num = _numberOfEnemiesSpawned / 8;
		int offsetDirection = _numberOfEnemiesSpawned % 8;
		return num * GetOffsetDirection(offsetDirection);
	}

	private Vector2 GetOffsetDirection(int offsetDirection)
	{
		return offsetDirection switch
		{
			0 => Vector2.left, 
			1 => Vector2.right, 
			2 => Vector2.up, 
			3 => Vector2.down, 
			4 => Vector2.left + Vector2.up, 
			5 => Vector2.right + Vector2.up, 
			6 => Vector2.left + Vector2.down, 
			7 => Vector2.right + Vector2.down, 
			_ => Vector2.left, 
		};
	}
}
