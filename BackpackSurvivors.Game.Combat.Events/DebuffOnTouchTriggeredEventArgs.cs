using System;

namespace BackpackSurvivors.Game.Combat.Events;

public class DebuffOnTouchTriggeredEventArgs : EventArgs
{
	public CombatWeapon CombatWeapon { get; }

	public Character TriggeredOn { get; }

	public Character TriggeredFrom { get; }

	public DebuffOnTouchTriggeredEventArgs(Character triggeredOn, Character triggeredFrom)
	{
		TriggeredOn = triggeredOn;
		TriggeredFrom = triggeredFrom;
	}
}
