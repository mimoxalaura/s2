using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Bosses.VoidCorruption;

public class LeavingDarknessEffect : MonoBehaviour
{
	[SerializeField]
	private Darkness _darknessPrefab;

	[SerializeField]
	private Transform _darknessSpawnParent;

	[SerializeField]
	private Enemy _sourceEnemy;

	[SerializeField]
	private bool _dropDarkness = true;

	[SerializeField]
	private float _dropEvery = 0.5f;

	[SerializeField]
	private DamageSO _damageSO;

	private void Start()
	{
		StartCoroutine(SpawnDarkness());
	}

	private IEnumerator SpawnDarkness()
	{
		while (true)
		{
			if (_dropDarkness)
			{
				Darkness darkness = Object.Instantiate(_darknessPrefab, _darknessSpawnParent);
				darkness.gameObject.transform.SetParent(null);
				darkness.Init(_sourceEnemy, null);
				darkness.DamagePlayerOnTouch.Init(new Dictionary<Enums.ItemStatType, float>(), _damageSO, new Dictionary<Enums.WeaponStatType, float>());
				darkness.SetDuration(60f);
				darkness.Activate();
			}
			yield return new WaitForSeconds(_dropEvery);
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
