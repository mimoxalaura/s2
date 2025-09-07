using System;

namespace BackpackSurvivors.Game.Combat.Events;

public class DamageOnTouchTriggeredEventArgs : EventArgs
{
	public CombatWeapon CombatWeapon { get; }

	public Character TriggeredOn { get; }

	public Character TriggeredFrom { get; }

	public DamageOnTouchTriggeredEventArgs(Character triggeredOn, Character triggeredFrom)
	{
		TriggeredOn = triggeredOn;
		TriggeredFrom = triggeredFrom;
	}
}
