using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class ProjectileVisualizationSingle : ProjectileVisualization
{
	[SerializeField]
	private TriggerOnTouch TriggerOnTouch;

	internal override void InitTriggers(Enums.CharacterType characterTypeForTriggerOnTouch, Character source, Vector2 targetPosition)
	{
		if (TriggerOnTouch != null)
		{
			TriggerOnTouch.Init(characterTypeForTriggerOnTouch);
			TriggerOnTouch.OnTriggerOnTouch += TriggerOnTouch_OnTriggerOnTouch;
		}
	}

	private void TriggerOnTouch_OnTriggerOnTouch(object sender, TriggerOnTouchEventArgs e)
	{
		RaiseOnTriggerOnTouch(e.TriggeredOn);
	}
}
