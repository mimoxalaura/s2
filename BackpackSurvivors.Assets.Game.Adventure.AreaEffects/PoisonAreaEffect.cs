using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.Game.Adventure.AreaEffects;

internal class PoisonAreaEffect : AdventureAreaEffect
{
	[SerializeField]
	private float _tickDelaySeconds;

	[SerializeField]
	private float _damagePerTick;

	private bool _damageZoneIsActive = true;

	internal override void Activate()
	{
		base.Activate();
		StartCoroutine(RunDamageTicks());
	}

	private IEnumerator RunDamageTicks()
	{
		while (_damageZoneIsActive)
		{
			yield return new WaitForSeconds(_tickDelaySeconds);
			SingletonController<GameController>.Instance.Player.Damage(_damagePerTick, wasCrit: false, null, 0f, null, Enums.DamageType.Poison);
		}
	}

	private void OnDestroy()
	{
		_damageZoneIsActive = false;
		StopAllCoroutines();
	}
}
