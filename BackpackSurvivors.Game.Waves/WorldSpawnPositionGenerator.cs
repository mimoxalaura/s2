using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Enemies.SpatialPartitioning;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.ScriptableObjects.Waves;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

public class WorldSpawnPositionGenerator : MonoBehaviour
{
	[SerializeField]
	public float LeftBoundX;

	[SerializeField]
	public float RightBoundX;

	[SerializeField]
	public float TopBoundY;

	[SerializeField]
	public float BottomBoundY;

	[SerializeField]
	public float XDistanceBetweenSpawnpositions;

	[SerializeField]
	public float YDistanceBetweenSpawnpositions;

	[SerializeField]
	public GameObject SpawnPositionPrefab;

	[SerializeField]
	public Transform SpawnPositionsParentTransform;

	private Dictionary<string, CircleSpawnPositionCalculator> _circleSpawnPositionCalculators = new Dictionary<string, CircleSpawnPositionCalculator>();

	private Dictionary<string, LineSpawnPositionCalculator> _lineSpawnPositionCalculators = new Dictionary<string, LineSpawnPositionCalculator>();

	private Dictionary<string, GroupedSpawnPositionCalculator> _groupedSpawnPositionCalculators = new Dictionary<string, GroupedSpawnPositionCalculator>();

	private float _minEnemyDistance;

	private float _maxEnemyDistance;

	private List<SpawnPosition> _spawnPositions;

	private List<SpawnPosition> SpawnPositions
	{
		get
		{
			if (_spawnPositions == null)
			{
				InitSpawnPositions();
			}
			return _spawnPositions;
		}
	}

	private void Start()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<EnemyController>.Instance, RegisterAtEnemyController);
	}

	private void InitSpawnPositions()
	{
		_spawnPositions = SpawnPositionsParentTransform.GetComponentsInChildren<SpawnPosition>().ToList();
	}

	private void RegisterAtEnemyController()
	{
		SingletonController<EnemyController>.Instance.SetWorldSpawnPositionGenerator(this);
		_minEnemyDistance = SingletonController<EnemyController>.Instance.MinEnemyDistance;
		_maxEnemyDistance = SingletonController<EnemyController>.Instance.MaxEnemyDistance;
	}

	internal Vector2 GetSpawnPositionBySpawnDirection(Enums.SpawnDirection initialSpawnDirection)
	{
		Enums.SpawnDirection spawnDirection = FixSpawnDirection(initialSpawnDirection);
		List<SpawnPosition> spawnPositionsBySpawnDirection = GetSpawnPositionsBySpawnDirection(spawnDirection);
		spawnPositionsBySpawnDirection = RemoveSpawnPositionsNearPlayer(spawnPositionsBySpawnDirection);
		return GetSpawnpositionClosestToPlayer(spawnPositionsBySpawnDirection);
	}

	private Enums.SpawnDirection FixSpawnDirection(Enums.SpawnDirection initialSpawnDirection)
	{
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		if (playerPosition.x <= LeftBoundX && FlagContainsSearchedEnum(initialSpawnDirection, Enums.SpawnDirection.WestOfPlayer))
		{
			initialSpawnDirection |= Enums.SpawnDirection.EastOfPlayer;
		}
		if (playerPosition.x >= RightBoundX && FlagContainsSearchedEnum(initialSpawnDirection, Enums.SpawnDirection.EastOfPlayer))
		{
			initialSpawnDirection |= Enums.SpawnDirection.WestOfPlayer;
		}
		if (playerPosition.y <= BottomBoundY && FlagContainsSearchedEnum(initialSpawnDirection, Enums.SpawnDirection.SouthOfPlayer))
		{
			initialSpawnDirection |= Enums.SpawnDirection.NorthOfPlayer;
		}
		if (playerPosition.x >= TopBoundY && FlagContainsSearchedEnum(initialSpawnDirection, Enums.SpawnDirection.NorthOfPlayer))
		{
			initialSpawnDirection |= Enums.SpawnDirection.SouthOfPlayer;
		}
		return initialSpawnDirection;
	}

	private Vector2 GetSpawnpositionClosestToPlayer(List<SpawnPosition> possibleSpawnPositions)
	{
		if (possibleSpawnPositions.Count == 0)
		{
			return GetRandomSpawnPositionNearPlayer();
		}
		float num = float.MaxValue;
		SpawnPosition spawnPosition = possibleSpawnPositions[0];
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		foreach (SpawnPosition possibleSpawnPosition in possibleSpawnPositions)
		{
			float num2 = Vector2.Distance(possibleSpawnPosition.transform.position, playerPosition);
			if (!(num2 > num))
			{
				spawnPosition = possibleSpawnPosition;
				num = num2;
			}
		}
		return spawnPosition.transform.position;
	}

	private List<SpawnPosition> RemoveSpawnPositionsNearPlayer(List<SpawnPosition> spawnPositions)
	{
		return spawnPositions.Where((SpawnPosition sp) => Vector2.Distance(sp.transform.position, SingletonController<GameController>.Instance.PlayerPosition) > _minEnemyDistance).ToList();
	}

	private List<SpawnPosition> RemoveSpawnPositionsTooFarFromPlayer(List<SpawnPosition> spawnPositions)
	{
		return spawnPositions.Where((SpawnPosition sp) => Vector2.Distance(sp.transform.position, SingletonController<GameController>.Instance.PlayerPosition) < _maxEnemyDistance).ToList();
	}

	private List<SpawnPosition> GetSpawnPositionsBySpawnDirection(Enums.SpawnDirection spawnDirection)
	{
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		List<SpawnPosition> list = new List<SpawnPosition>();
		if (FlagContainsSearchedEnum(spawnDirection, Enums.SpawnDirection.NorthOfPlayer))
		{
			list.AddRange(SpawnPositions.Where((SpawnPosition sp) => sp.transform.position.y > playerPosition.y));
		}
		if (FlagContainsSearchedEnum(spawnDirection, Enums.SpawnDirection.SouthOfPlayer))
		{
			list.AddRange(SpawnPositions.Where((SpawnPosition sp) => sp.transform.position.y < playerPosition.y));
		}
		if (FlagContainsSearchedEnum(spawnDirection, Enums.SpawnDirection.EastOfPlayer))
		{
			list.AddRange(SpawnPositions.Where((SpawnPosition sp) => sp.transform.position.x > playerPosition.x));
		}
		if (FlagContainsSearchedEnum(spawnDirection, Enums.SpawnDirection.WestOfPlayer))
		{
			list.AddRange(SpawnPositions.Where((SpawnPosition sp) => sp.transform.position.x < playerPosition.x));
		}
		return list.Distinct().ToList();
	}

	private bool FlagContainsSearchedEnum(Enums.SpawnDirection flagEnum, Enums.SpawnDirection searchedEnum)
	{
		return (flagEnum & searchedEnum) == searchedEnum;
	}

	internal SpawnPosition GetRandomActiveSpawnPosition()
	{
		List<SpawnPosition> list = SpawnPositions.Where((SpawnPosition sp) => sp.IsActive).ToList();
		return list[Random.Range(0, list.Count)];
	}

	internal Vector2 GetRandomSpawnPositionNearPlayer(bool selectOnlyBossSpawnLocations = false)
	{
		List<SpawnPosition> spawnPositionsNearPlayer = GetSpawnPositionsNearPlayer(selectOnlyBossSpawnLocations);
		int index = Random.Range(0, spawnPositionsNearPlayer.Count);
		return spawnPositionsNearPlayer[index].transform.position;
	}

	internal Vector2 GetTeleportLocation(Vector2 enemyLocation, Vector2 playerLocation, bool selectOnlyBossSpawnLocations)
	{
		List<SpawnPosition> spawnPositionsNearPlayer = GetSpawnPositionsNearPlayer(selectOnlyBossSpawnLocations);
		spawnPositionsNearPlayer = GetSpawnPositionsOnOtherSideOfPlayer(spawnPositionsNearPlayer, enemyLocation, playerLocation);
		if (!spawnPositionsNearPlayer.Any())
		{
			spawnPositionsNearPlayer = GetSpawnPositionsNearPlayer(selectOnlyBossSpawnLocations);
		}
		int index = Random.Range(0, spawnPositionsNearPlayer.Count);
		return spawnPositionsNearPlayer[index].transform.position;
	}

	private List<SpawnPosition> GetSpawnPositionsOnOtherSideOfPlayer(List<SpawnPosition> spawnPositions, Vector2 enemyLocation, Vector2 playerLocation)
	{
		spawnPositions = ((!(Mathf.Abs(enemyLocation.x - playerLocation.x) > enemyLocation.y - playerLocation.y)) ? ((enemyLocation.y > playerLocation.y) ? spawnPositions.Where((SpawnPosition sp) => sp.transform.position.y < playerLocation.y).ToList() : spawnPositions.Where((SpawnPosition sp) => sp.transform.position.y > playerLocation.y).ToList()) : ((enemyLocation.x > playerLocation.x) ? spawnPositions.Where((SpawnPosition sp) => sp.transform.position.x < playerLocation.x).ToList() : spawnPositions.Where((SpawnPosition sp) => sp.transform.position.x > playerLocation.x).ToList()));
		return spawnPositions;
	}

	private List<SpawnPosition> GetSpawnPositionsNearPlayer(bool selectOnlyBossSpawnLocations)
	{
		List<SpawnPosition> list = SpawnPositions.ToList();
		if (selectOnlyBossSpawnLocations)
		{
			list = list.Where((SpawnPosition x) => x.IsBossSpawnPoint).ToList();
		}
		list = RemoveSpawnPositionsNearPlayer(list);
		list = RemoveSpawnPositionsTooFarFromPlayer(list);
		if (!list.Any())
		{
			list = SpawnPositions;
		}
		return list;
	}

	internal Vector2 GetRandomSpawnPositionAwayFromPlayer(Vector2 ownPosition)
	{
		List<SpawnPosition> list = new List<SpawnPosition>();
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		float num = Vector2.Distance(ownPosition, playerPosition);
		foreach (SpawnPosition spawnPosition in SpawnPositions)
		{
			if (Vector2.Distance(Vector2.MoveTowards(ownPosition, spawnPosition.transform.position, 0.01f), playerPosition) > num)
			{
				list.Add(spawnPosition);
			}
		}
		if (!list.Any())
		{
			return GetRandomSpawnPositionNearPlayer();
		}
		int index = Random.Range(0, list.Count);
		return list[index].transform.position;
	}

	internal Vector2 GetRandomSpawnPositionInDirection(Enums.SpawnDirection initialDirection, bool selectOnlyBossSpawnLocations = false)
	{
		Enums.SpawnDirection spawnDirection = FixSpawnDirection(initialDirection);
		List<SpawnPosition> spawnPositionsBySpawnDirection = GetSpawnPositionsBySpawnDirection(spawnDirection);
		spawnPositionsBySpawnDirection = RemoveSpawnPositionsNearPlayer(spawnPositionsBySpawnDirection);
		spawnPositionsBySpawnDirection = RemoveSpawnPositionsTooFarFromPlayer(spawnPositionsBySpawnDirection);
		if (!spawnPositionsBySpawnDirection.Any())
		{
			spawnPositionsBySpawnDirection = SpawnPositions;
		}
		return spawnPositionsBySpawnDirection[Random.Range(0, spawnPositionsBySpawnDirection.Count)].transform.position;
	}

	internal Vector2 GetGroupedSpawnPosition(WaveChunkSO waveChunk)
	{
		if (_groupedSpawnPositionCalculators.ContainsKey(waveChunk.name))
		{
			return _groupedSpawnPositionCalculators[waveChunk.name].GetSpawnPosition();
		}
		Vector2 spawnPositionBySpawnDirection = GetSpawnPositionBySpawnDirection(waveChunk.SpawnLocation);
		GroupedSpawnPositionCalculator groupedSpawnPositionCalculator = new GroupedSpawnPositionCalculator(waveChunk.name, spawnPositionBySpawnDirection);
		_groupedSpawnPositionCalculators.Add(waveChunk.name, groupedSpawnPositionCalculator);
		return groupedSpawnPositionCalculator.GetSpawnPosition();
	}

	internal Vector2 GetCircleSpawnPosition(WaveChunkSO waveChunk)
	{
		if (_circleSpawnPositionCalculators.ContainsKey(waveChunk.name))
		{
			return _circleSpawnPositionCalculators[waveChunk.name].GetSpawnPosition();
		}
		CircleSpawnPositionCalculator circleSpawnPositionCalculator = new CircleSpawnPositionCalculator(SingletonController<GameController>.Instance.PlayerPosition, waveChunk);
		_circleSpawnPositionCalculators.Add(waveChunk.name, circleSpawnPositionCalculator);
		return circleSpawnPositionCalculator.GetSpawnPosition();
	}

	internal Vector2 GetLineSpawnPosition(WaveChunkSO waveChunk)
	{
		if (_lineSpawnPositionCalculators.ContainsKey(waveChunk.name))
		{
			return _lineSpawnPositionCalculators[waveChunk.name].GetSpawnPosition();
		}
		float x = base.transform.position.x;
		float y = base.transform.position.y;
		Enums.CardinalDirection directionTowardsPlayer;
		LineSpawnPositionCalculator lineSpawnPositionCalculator = new LineSpawnPositionCalculator(LineSpawnPositionCalculator.GetStartPosition(SingletonController<GameController>.Instance.PlayerPosition, LeftBoundX + x, RightBoundX + x, BottomBoundY + y, TopBoundY + y, waveChunk.DistanceFromPlayer, out directionTowardsPlayer), directionTowardsPlayer, waveChunk);
		_lineSpawnPositionCalculators.Add(waveChunk.name, lineSpawnPositionCalculator);
		return lineSpawnPositionCalculator.GetSpawnPosition();
	}

	internal LineSpawnPositionCalculator GetLineSpawnPositionCalculator(string wavechunkName)
	{
		if (_lineSpawnPositionCalculators.ContainsKey(wavechunkName))
		{
			return _lineSpawnPositionCalculators[wavechunkName];
		}
		return null;
	}

	internal CircleSpawnPositionCalculator GetCircleSpawnPositionCalculator(string wavechunkName)
	{
		if (_circleSpawnPositionCalculators.ContainsKey(wavechunkName))
		{
			return _circleSpawnPositionCalculators[wavechunkName];
		}
		return null;
	}

	internal void ResetStoredWavechunkSpawnLocations(string wavechunkName)
	{
		_circleSpawnPositionCalculators.Remove(wavechunkName);
		_lineSpawnPositionCalculators.Remove(wavechunkName);
	}

	internal void BlockSpawnPosition(SpawnPosition spawnPos)
	{
		spawnPos.SetActiveState(isActive: false);
	}

	internal void InitSpatialController(SpatialController spatialController)
	{
		spatialController.Init(base.transform.position, LeftBoundX, RightBoundX, TopBoundY, BottomBoundY);
	}
}
