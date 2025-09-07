using BackpackSurvivors.Assets.Game.Adventure.AreaEffects;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Waves;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure.AdventureEffects;

public class AdventureEffecSlowdownController : AdventureEffectController
{
	[SerializeField]
	private AdventureAreaEffect _adventureAreaEffect;

	internal override void InitializeEffect(TimeBasedWaveController timeBasedWaveController, LevelSO level)
	{
		base.InitializeEffect(timeBasedWaveController, level);
		Object.Instantiate(_adventureAreaEffect).Activate();
	}
}
