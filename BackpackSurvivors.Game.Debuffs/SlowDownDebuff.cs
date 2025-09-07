using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Debuffs;

public class SlowDownDebuff : DebuffEffect
{
	public override void FellOff(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		if (!debuffedCharacter.IsDead)
		{
			(debuffedCharacter as BackpackSurvivors.Game.Player.Player).PlayerBaseSpeedModifier = 1f;
			SingletonController<GameController>.Instance.Player.RefreshMovementSpeed();
		}
	}

	public override void Trigger(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		(debuffedCharacter as BackpackSurvivors.Game.Player.Player).PlayerBaseSpeedModifier = 0.6f;
		SingletonController<GameController>.Instance.Player.RefreshMovementSpeed();
	}
}
