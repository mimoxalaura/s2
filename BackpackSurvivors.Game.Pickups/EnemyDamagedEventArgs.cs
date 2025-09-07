using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Pickups;

internal class EnemyDamagedEventArgs : EventArgs
{
	public Enemy Enemy { get; }

	public bool WasCrit { get; }

	public WeaponSO WeaponSource { get; }

	public Enums.DamageType DamageType { get; }

	public Character DamageSource { get; }

	public List<Enums.Debuff.DebuffType> DebuffsCaused { get; }

	public EnemyDamagedEventArgs(Enemy enemy, bool wasCrit, WeaponSO weaponSource, Enums.DamageType damageType, Character damageSource)
	{
		Enemy = enemy;
		WasCrit = wasCrit;
		WeaponSource = weaponSource;
		DamageType = damageType;
		DamageSource = damageSource;
	}
}
