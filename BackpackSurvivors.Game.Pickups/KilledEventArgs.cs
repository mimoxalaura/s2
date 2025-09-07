using System;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Pickups;

internal class KilledEventArgs : EventArgs
{
	public Character Character { get; }

	public float Damage { get; }

	public bool WasCrit { get; }

	public WeaponSO WeaponSource { get; }

	public Enums.DamageType DamageType { get; }

	public Character DamageSource { get; }

	public KilledEventArgs(Character character, float damage, bool wasCrit, WeaponSO weaponSource, Enums.DamageType damageType, Character damageSource)
	{
		Character = character;
		Damage = damage;
		WasCrit = wasCrit;
		WeaponSource = weaponSource;
		DamageType = damageType;
		DamageSource = damageSource;
	}
}
