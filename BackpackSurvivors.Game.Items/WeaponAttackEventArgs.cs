using System;
using BackpackSurvivors.Game.Combat;

namespace BackpackSurvivors.Game.Items;

public class WeaponAttackEventArgs : EventArgs
{
	public CombatWeapon CombatWeapon { get; }

	public WeaponAttackEventArgs(CombatWeapon combatWeapon)
	{
		CombatWeapon = combatWeapon;
	}
}
