using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Enemies.Movement;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Debuffs;

public class SlowingDebuff : DebuffEffect
{
	private float speedReductionOnEnemy = 0.6f;

	private float speedReductionOnpPlayer = 0.3f;

	public override void FellOff(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		if (debuffedCharacter.IsDead)
		{
			return;
		}
		if (debuffedCharacter.IsEnemy)
		{
			EnemyMovement component = debuffedCharacter.GetComponent<EnemyMovement>();
			if (!(component == null))
			{
				component.ChangeMovementSpeed(1f);
			}
		}
		else
		{
			SingletonController<GameController>.Instance.Player.RemoveBuffedStat(Enums.ItemStatType.SpeedPercentage, 0f - speedReductionOnpPlayer);
			SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: false);
		}
	}

	public override void Trigger(Character debuffedCharacter, Character debuffSourceCharacter, int stacks, DamageInstance damageInstance, DamageInstance weaponDamageInstance, Enums.DamageType combatWeaponDamageType)
	{
		if (debuffedCharacter.IsEnemy)
		{
			debuffedCharacter.GetComponent<EnemyMovement>().ChangeMovementSpeed(speedReductionOnEnemy);
			return;
		}
		SingletonController<GameController>.Instance.Player.AddBuffedStat(Enums.ItemStatType.SpeedPercentage, 0f - speedReductionOnpPlayer);
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: false);
	}
}
