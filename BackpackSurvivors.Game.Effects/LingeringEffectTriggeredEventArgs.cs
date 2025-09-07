using System;
using BackpackSurvivors.Game.Combat;

namespace BackpackSurvivors.Game.Effects;

public class LingeringEffectTriggeredEventArgs : EventArgs
{
	public CombatWeapon CombatWeapon { get; }

	public Character TriggeredOn { get; }

	public LingeringEffectTriggeredEventArgs(Character triggeredOn, CombatWeapon combatWeapon)
	{
		TriggeredOn = triggeredOn;
		CombatWeapon = combatWeapon;
	}
}
