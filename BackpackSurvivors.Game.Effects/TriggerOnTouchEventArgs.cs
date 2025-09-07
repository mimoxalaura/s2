using System;
using BackpackSurvivors.Game.Combat;

namespace BackpackSurvivors.Game.Effects;

public class TriggerOnTouchEventArgs : EventArgs
{
	public Character TriggeredOn { get; }

	public TriggerOnTouchEventArgs(Character triggeredOn)
	{
		TriggeredOn = triggeredOn;
	}
}
