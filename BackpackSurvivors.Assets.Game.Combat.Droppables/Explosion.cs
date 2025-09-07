using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.Game.Combat.Droppables;

[RequireComponent(typeof(DamagePlayerOnTouch))]
[RequireComponent(typeof(DamageEnemyOnTouch))]
internal class Explosion : MonoBehaviour
{
	[SerializeField]
	private ProjectileVisualization _projectileVisualizationPrefab;

	[SerializeField]
	private SerializableDictionaryBase<Enums.ItemStatType, float> _explosionStats;

	[SerializeField]
	private DamageSO _damageStats;

	[SerializeField]
	private SerializableDictionaryBase<Enums.WeaponStatType, float> _critInfo;

	[SerializeField]
	private float _explosionDelay;

	[SerializeField]
	private float _destroyDelayAfterExploding;

	private DamagePlayerOnTouch _damagePlayerOnTouch;

	private DamageEnemyOnTouch _damageEnemyOnTouch;

	private float _explosionSize;

	private bool _canAct = true;

	private void Awake()
	{
		InitExplosionSize();
		InitDamagePlayerOnTouch();
		InitDamageEnemyOnTouch();
	}

	private void Start()
	{
		RegisterEvents();
		ShowWarning();
		ExecuteExplosion();
	}

	private void RegisterEvents()
	{
		SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>().OnLevelCompleted += TimeBasedLevelController_OnLevelCompleted;
	}

	private void TimeBasedLevelController_OnLevelCompleted(object sender, EventArgs e)
	{
		_canAct = false;
	}

	private void InitExplosionSize()
	{
		_explosionSize = _explosionStats[Enums.ItemStatType.ExplosionSizePercentage];
		float calculatedStat = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.ExplosionSizePercentage);
		_explosionSize *= calculatedStat;
		base.transform.localScale = Vector3.one * _explosionSize;
	}

	internal void SetDamage(float minDamage, float maxDamage)
	{
		_damageStats.BaseMinDamage = minDamage;
		_damageStats.BaseMaxDamage = maxDamage;
	}

	private void ShowWarning()
	{
		if (!(_explosionDelay <= 0f))
		{
			SingletonCacheController.Instance.GetControllerByType<VisualWarningController>().SpawnWarning(base.transform.position, _explosionSize, _explosionDelay);
		}
	}

	private void ExecuteExplosion()
	{
		StartCoroutine(ExplodeAfterDelay());
	}

	private IEnumerator ExplodeAfterDelay()
	{
		yield return new WaitForSeconds(_explosionDelay);
		if (_canAct)
		{
			UnityEngine.Object.Instantiate(_projectileVisualizationPrefab, base.transform);
			_damagePlayerOnTouch.SetCanAct(canAct: true);
			_damageEnemyOnTouch.SetCanAct(canAct: true);
		}
		StartCoroutine(DestroyAfterExploding());
	}

	private IEnumerator DestroyAfterExploding()
	{
		yield return new WaitForSeconds(_destroyDelayAfterExploding);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void InitDamagePlayerOnTouch()
	{
		Dictionary<Enums.ItemStatType, float> sourceStats = _explosionStats.ToDictionary();
		Dictionary<Enums.WeaponStatType, float> critInfo = _critInfo.ToDictionary();
		_damagePlayerOnTouch = GetComponent<DamagePlayerOnTouch>();
		_damagePlayerOnTouch.Init(sourceStats, _damageStats, critInfo, canDamageRepeatedly: false);
		_damagePlayerOnTouch.SetCanAct(_canAct);
	}

	private void InitDamageEnemyOnTouch()
	{
		Dictionary<Enums.ItemStatType, float> sourceStats = _explosionStats.ToDictionary();
		Dictionary<Enums.WeaponStatType, float> critInfo = _critInfo.ToDictionary();
		_damageEnemyOnTouch = GetComponent<DamageEnemyOnTouch>();
		_damageEnemyOnTouch.Init(sourceStats, _damageStats, critInfo);
		_damageEnemyOnTouch.SetCanAct(_canAct);
	}

	private void OnDestroy()
	{
		SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>().OnLevelCompleted -= TimeBasedLevelController_OnLevelCompleted;
	}
}
