using System;

namespace BackpackSurvivors.Game.Combat.Events;

public class WeaponCooldownUpdateEventArgs : EventArgs
{
	public CombatWeapon CombatWeapon { get; set; }

	public WeaponCooldownUpdateEventArgs(CombatWeapon combatWeapon)
	{
		CombatWeapon = combatWeapon;
	}
}
