using System;

namespace BackpackSurvivors.Game.Combat.Events;

public class WeaponReadyEventArgs : EventArgs
{
	public CombatWeapon CombatWeapon { get; set; }

	public WeaponReadyEventArgs(CombatWeapon combatWeapon)
	{
		CombatWeapon = combatWeapon;
	}
}
