using System;

namespace BackpackSurvivors.Game.Combat.Events;

public class WeaponAttackedEventArgs : EventArgs
{
	public CombatWeapon CombatWeapon { get; }

	public WeaponAttackedEventArgs(CombatWeapon combatWeapon)
	{
		CombatWeapon = combatWeapon;
	}
}
