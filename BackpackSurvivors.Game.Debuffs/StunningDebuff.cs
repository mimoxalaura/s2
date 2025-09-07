using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Debuffs;

public class StunningDebuff : DebuffEffect
{
	public override void FellOff(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		if (debuffedCharacter.IsEnemy)
		{
			Enemy enemy = (Enemy)debuffedCharacter;
			if (enemy != null)
			{
				enemy.EnemyMovement.EnableAfterDelay(0f);
			}
		}
	}

	public override void Trigger(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		if (debuffedCharacter.IsEnemy)
		{
			Enemy enemy = (Enemy)debuffedCharacter;
			if (enemy != null)
			{
				enemy.EnemyMovement.SetCanMove(canMove: false);
				enemy.EnemyMovement.SetMass(1000f);
				enemy.EnemyMovement.EnableAfterDelay(1f);
			}
		}
	}
}
