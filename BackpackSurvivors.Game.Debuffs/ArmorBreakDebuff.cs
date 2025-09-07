using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Debuffs;

public class ArmorBreakDebuff : DebuffEffect
{
	private float armorReduction = 20f;

	public override void FellOff(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		debuffedCharacter.AddBuffedStat(Enums.ItemStatType.Armor, armorReduction);
	}

	public override void Trigger(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		debuffedCharacter.AddBuffedStat(Enums.ItemStatType.Armor, 0f - armorReduction);
	}
}
