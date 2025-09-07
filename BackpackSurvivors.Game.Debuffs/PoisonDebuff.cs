using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Debuffs;

public class PoisonDebuff : DebuffEffect
{
	public override void FellOff(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
	}

	public override void Trigger(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		bool wasCrit = false;
		float damage = DamageEngine.CalculateIndirectDamage(debuffSourceCharacter.CalculatedStats, debuffedCharacter.CalculatedStats, debuffSourceCharacter.CalculatedDamageTypeValues, stacks, damageInstance, weaponDamageInstance, debuffedCharacter.GetCharacterType());
		debuffedCharacter.Damage(damage, wasCrit, null, 0f, null, combatWeaponDamageType, debuffSourceCharacter);
	}
}
