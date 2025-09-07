using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Waves;

namespace BackpackSurvivors.Game.Adventure.AdventureEffects;

public class AdventureEffectEliteController : AdventureEffectController
{
	internal override void InitializeEffect(TimeBasedWaveController timeBasedWaveController, LevelSO level)
	{
		base.InitializeEffect(timeBasedWaveController, level);
		timeBasedWaveController.SetEliteChance(0.01f);
	}
}
