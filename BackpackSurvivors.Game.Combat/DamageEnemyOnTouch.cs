using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat;

internal class DamageEnemyOnTouch : MonoBehaviour
{
	[SerializeField]
	private WeaponSO _weaponSO;

	private Dictionary<Enums.ItemStatType, float> _sourceStats;

	private DamageInstance _damageInstance;

	private Dictionary<Enums.WeaponStatType, float> _critInfo;

	private bool _canAct;

	private float _damageScale;

	private List<Enemy> _enemiesWithinCollider = new List<Enemy>();

	private bool _canDamageRepeatedly;

	private void Update()
	{
		TryDamageEnemy();
	}

	public void Init(Dictionary<Enums.ItemStatType, float> sourceStats, DamageSO damageSO, Dictionary<Enums.WeaponStatType, float> critInfo, bool canDamageRepeatedly = true)
	{
		_sourceStats = sourceStats;
		_damageInstance = new DamageInstance(damageSO);
		_critInfo = critInfo;
		_canAct = true;
		_canDamageRepeatedly = canDamageRepeatedly;
	}

	public void ScaleDamage(float damageScale)
	{
		_damageScale = damageScale;
		_damageInstance.CalculatedMinDamage *= _damageScale;
		_damageInstance.CalculatedMaxDamage *= _damageScale;
	}

	public void SetCanAct(bool canAct)
	{
		_canAct = canAct;
	}

	private void DealDamage(Enemy enemy)
	{
		bool wasCrit;
		float damage = DamageEngine.CalculateDamage(_critInfo, _sourceStats, enemy.CalculatedStats, _damageInstance, enemy.GetCharacterType(), out wasCrit);
		enemy.Damage(damage, wasCrit, base.gameObject, 0f, _weaponSO, _damageInstance.CalculatedDamageType, SingletonController<GameController>.Instance.Player);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Enemy component = collision.gameObject.GetComponent<Enemy>();
		if (!(component == null) && !_enemiesWithinCollider.Contains(component))
		{
			_enemiesWithinCollider.Add(component);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Enemy component = collision.gameObject.GetComponent<Enemy>();
		if (!(component == null))
		{
			_enemiesWithinCollider.Remove(component);
		}
	}

	private void TryDamageEnemy()
	{
		if (!_enemiesWithinCollider.Any() || !_canAct)
		{
			return;
		}
		for (int i = 0; i < _enemiesWithinCollider.Count; i++)
		{
			Enemy enemy = _enemiesWithinCollider[i];
			if (!enemy.IsDead)
			{
				DealDamage(enemy);
			}
		}
		if (_canDamageRepeatedly)
		{
			StartCoroutine(TempDisableDamaging());
		}
		else
		{
			_canAct = false;
		}
	}

	private IEnumerator TempDisableDamaging()
	{
		_canAct = false;
		yield return new WaitForSeconds(0.1f);
		_canAct = true;
	}
}
