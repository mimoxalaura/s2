using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.Droppables;

internal class SpawnExplosionAfterWarning : SpawnWeaponAttack
{
	[SerializeField]
	private float _warningDelay;

	[SerializeField]
	private Transform _warningSpawnPosition;

	private void Start()
	{
	}

	internal void Activate()
	{
		StartCoroutine(SpawnExplosionAfterDelay());
	}

	internal void SetWarningDelay(float delay)
	{
		_warningDelay = delay;
	}

	private IEnumerator SpawnExplosionAfterDelay()
	{
		float explosionRadius = GetExplosionRadius();
		VisualWarningController controllerByType = SingletonCacheController.Instance.GetControllerByType<VisualWarningController>();
		if (_warningSpawnPosition != null)
		{
			controllerByType.SpawnWarning(_warningSpawnPosition.position, explosionRadius, _warningDelay);
		}
		else
		{
			controllerByType.SpawnWarning(base.transform.position, explosionRadius, _warningDelay);
		}
		yield return new WaitForSeconds(_warningDelay);
		InstantiateWeaponAttack(attackOriginatedFromPlayer: false);
		yield return new WaitForSeconds(0.5f);
		Object.Destroy(base.gameObject);
	}

	private float GetExplosionRadius()
	{
		float num = _weaponSO.Stats.StatValues.TryGet(Enums.WeaponStatType.ExplosionSizePercentage, 1f);
		float num2 = (_usePlayerAsSource ? SingletonController<GameController>.Instance.Player.CalculatedStats.TryGet(Enums.ItemStatType.ExplosionSizePercentage, 1f) : 1f);
		return num * num2;
	}

	internal void SetHellfireMultiplier(float hellfireDamageMultiplier)
	{
		_hellfireDamageMultiplier = hellfireDamageMultiplier;
	}
}
