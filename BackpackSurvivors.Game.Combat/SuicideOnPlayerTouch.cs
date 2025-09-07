using System;
using System.Collections;
using BackpackSurvivors.Game.Combat.Droppables;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat;

public class SuicideOnPlayerTouch : DamagePlayerOnTouch
{
	[SerializeField]
	private bool _damagePlayerOnTouch;

	[SerializeField]
	private Character _character;

	[SerializeField]
	private SpawnExplosionAfterWarning _spawnExplosionAfterWarning;

	[SerializeField]
	private float _explosionDelay = 2f;

	[SerializeField]
	private bool _stopMovementOnTrigger;

	private bool _triggered;

	private void Start()
	{
		_character.OnKilled += _character_OnKilled;
	}

	private void _character_OnKilled(object sender, EventArgs e)
	{
		if (!_triggered)
		{
			SpawnExplosion();
		}
	}

	private void OnDestroy()
	{
		_character.OnKilled -= _character_OnKilled;
	}

	internal override void DealDamage()
	{
		if (!_triggered)
		{
			_triggered = true;
			_character.AnimateSuicide();
			base.DealDamage();
			if (_stopMovementOnTrigger)
			{
				base.Enemy.EnemyMovement.StopMovement();
				base.Enemy.EnemyMovement.PreventMovement();
				base.Enemy.EnemyMovement.ToggleLockPosition(locked: true);
				base.Enemy.MoveToPhasingLayer();
			}
			SpawnExplosion();
			StartCoroutine(KillCharacterAfterDamagingAttempt());
		}
	}

	private void SpawnExplosion()
	{
		if (_spawnExplosionAfterWarning != null)
		{
			_spawnExplosionAfterWarning.SetWarningDelay(_explosionDelay);
			_spawnExplosionAfterWarning.Activate();
			_spawnExplosionAfterWarning.transform.SetParent(null);
		}
	}

	private IEnumerator KillCharacterAfterDamagingAttempt()
	{
		yield return new WaitForSeconds(_explosionDelay + 0.1f);
		_character.Kill();
	}

	internal override void TryDamagePlayer()
	{
		if (CanDamagePlayer())
		{
			DealDamage();
		}
	}
}
