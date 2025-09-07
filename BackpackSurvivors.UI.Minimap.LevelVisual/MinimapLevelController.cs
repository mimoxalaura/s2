using System.Linq;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Level.Events;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Minimap.LevelVisual;

internal class MinimapLevelController : MonoBehaviour
{
	[SerializeField]
	private MinimapLevelLineVisual _minimapLevelLineVisualPrefab;

	[SerializeField]
	private MinimapLevelVisual _minimapLevelVisualPrefab;

	[SerializeField]
	private Transform _minimapLevelContainerTransform;

	[SerializeField]
	private TimeBasedLevelController _timeBasedLevelController;

	[SerializeField]
	private LevelSO TownLevel;

	private AdventureSO _currentAdventure;

	private LevelSO _currentLevel;

	[SerializeField]
	private AdventureSO DEBUG_Adventure;

	[SerializeField]
	private LevelSO DEBUG_Level;

	private void Start()
	{
		foreach (Transform item in _minimapLevelContainerTransform)
		{
			Object.Destroy(item.gameObject);
		}
		RegisterEvents();
	}

	private void RegisterEvents()
	{
		_timeBasedLevelController = Object.FindObjectOfType<TimeBasedLevelController>();
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(_timeBasedLevelController, RegisterTimeBasedLevelControllerStarted);
	}

	private void RegisterTimeBasedLevelControllerStarted()
	{
		Init(_timeBasedLevelController.CurrentLevel.Adventure, _timeBasedLevelController.CurrentLevel);
	}

	public void Init(AdventureSO currentAdventure, LevelSO currentLevel)
	{
		_currentAdventure = currentAdventure;
		_currentLevel = currentLevel;
		_timeBasedLevelController.OnTimeRemainingInLevelUpdated += _timeBasedLevelController_OnTimeRemainingInLevelUpdated;
		foreach (Transform item in _minimapLevelContainerTransform)
		{
			Object.Destroy(item.gameObject);
		}
		Object.Instantiate(_minimapLevelVisualPrefab, _minimapLevelContainerTransform).Init(TownLevel, completed: true);
		foreach (LevelSO item2 in _currentAdventure.Levels.OrderBy((LevelSO x) => x.LevelId))
		{
			MinimapLevelLineVisual minimapLevelLineVisual = Object.Instantiate(_minimapLevelLineVisualPrefab, _minimapLevelContainerTransform);
			MinimapLevelVisual minimapLevelVisual = Object.Instantiate(_minimapLevelVisualPrefab, _minimapLevelContainerTransform);
			minimapLevelVisual.Init(item2, completed: true);
			if (item2.LevelId == _currentLevel.LevelId)
			{
				minimapLevelVisual.SetActiveLevel();
			}
			if (_currentLevel.LevelId >= item2.LevelId)
			{
				minimapLevelLineVisual.SetFillState(1f);
			}
		}
	}

	private void _timeBasedLevelController_OnTimeRemainingInLevelUpdated(object sender, TimeRemainingEventArgs e)
	{
	}
}
