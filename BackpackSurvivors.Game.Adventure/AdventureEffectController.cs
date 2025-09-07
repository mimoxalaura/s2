using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Waves;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure;

public class AdventureEffectController : MonoBehaviour
{
	[SerializeField]
	private bool _triggerInBossArena;

	private TimeBasedWaveController _timeBasedWaveController;

	private LevelSO _level;

	protected bool TriggerInBossArena => _triggerInBossArena;

	protected LevelSO LevelSO => _level;

	internal virtual void InitializeEffect(TimeBasedWaveController timeBasedWaveController, LevelSO level)
	{
		_timeBasedWaveController = timeBasedWaveController;
		_level = level;
	}

	internal TimeBasedWaveController GetTimeBasedWaveController()
	{
		return _timeBasedWaveController;
	}
}
