using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Relics.RelicHandlers;

internal class KillWithTypedDamageGainBuff : RelicHandler
{
	[SerializeField]
	private BuffSO _buffAddedOnKill;

	[SerializeField]
	private Enums.DamageType _damageTypeToHitWith;

	public override void BeforeDestroy()
	{
		if (!(SingletonController<GameController>.Instance == null) && !(SingletonController<GameController>.Instance.Player == null) && !(SingletonController<EventController>.Instance == null) && !(_buffAddedOnKill == null))
		{
			SingletonController<GameController>.Instance.Player.RemoveBuff(_buffAddedOnKill);
			SingletonController<EventController>.Instance.OnEnemyDamaged -= Instance_OnEnemyDamaged;
		}
	}

	public override void Execute()
	{
		SingletonController<GameController>.Instance.Player.AddBuff(_buffAddedOnKill);
	}

	public override void Setup(BackpackSurvivors.Game.Relic.Relic relic)
	{
		base.Setup(relic);
		SingletonController<EventController>.Instance.OnEnemyDamaged += Instance_OnEnemyDamaged;
	}

	private void Instance_OnEnemyDamaged(object sender, EnemyDamagedEventArgs e)
	{
		if (e.DamageType == _damageTypeToHitWith)
		{
			Execute();
		}
	}

	public override void TearDown()
	{
	}
}
