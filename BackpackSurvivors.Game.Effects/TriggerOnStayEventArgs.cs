using System;
using BackpackSurvivors.Game.Combat;

namespace BackpackSurvivors.Game.Effects;

public class TriggerOnStayEventArgs : EventArgs
{
	public Character TriggeredOn { get; }

	public TriggerOnStayEventArgs(Character triggeredOn)
	{
		TriggeredOn = triggeredOn;
	}
}
