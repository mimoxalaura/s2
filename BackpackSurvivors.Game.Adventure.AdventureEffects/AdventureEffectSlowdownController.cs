using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure.AdventureEffects;

public class AdventureEffectSlowdownController : AdventureEffectController
{
	[SerializeField]
	private DebuffSO _speedReductionDebuff;

	internal override void InitializeEffect(TimeBasedWaveController timeBasedWaveController, LevelSO level)
	{
		base.InitializeEffect(timeBasedWaveController, level);
		DebuffHandler debuffHandler = new DebuffHandler();
		debuffHandler.Init(_speedReductionDebuff, SingletonController<GameController>.Instance.Player);
		SingletonController<GameController>.Instance.Player.AddDebuffAfterDelay(debuffHandler, SingletonController<GameController>.Instance.Player, 0f);
	}
}
