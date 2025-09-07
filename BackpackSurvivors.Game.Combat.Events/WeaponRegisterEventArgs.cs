using System;

namespace BackpackSurvivors.Game.Combat.Events;

public class WeaponRegisterEventArgs : EventArgs
{
	public CombatWeapon CombatWeapon { get; set; }

	public WeaponRegisterEventArgs(CombatWeapon combatWeapon)
	{
		CombatWeapon = combatWeapon;
	}
}
