using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat;

public class DamagePlayerOnTouch : MonoBehaviour
{
	[SerializeField]
	private Enemy _enemy;

	private Dictionary<Enums.ItemStatType, float> _sourceStats;

	private DamageInstance _damageInstance;

	private Dictionary<Enums.WeaponStatType, float> _critInfo;

	private bool _canAct;

	private float _damageScale;

	protected bool _playerInsideCollider;

	private bool _canDamageRepeatedly;

	private bool _damagingOnCooldown;

	public Enemy Enemy => _enemy;

	private void Update()
	{
		if (!SingletonController<GameController>.Instance.GamePaused)
		{
			TryDamagePlayer();
		}
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

	internal virtual void DealDamage()
	{
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		if (!player.CheckForDodge())
		{
			bool wasCrit;
			float damage = DamageEngine.CalculateDamage(_critInfo, _sourceStats, player.CalculatedStats, _damageInstance, player.GetCharacterType(), out wasCrit);
			player.Damage(damage, wasCrit, base.gameObject, 0f, null, _damageInstance.CalculatedDamageType, _enemy);
		}
		else
		{
			player.Dodged();
			player.SetImmunityForDuration(Enums.ImmunitySource.DamageTaken, 0.5f);
		}
	}

	private void ReceiveSpikeDamage(BackpackSurvivors.Game.Player.Player player)
	{
		Enemy componentInParent = GetComponentInParent<Enemy>();
		if (!(componentInParent == null))
		{
			float calculatedStat = player.GetCalculatedStat(Enums.ItemStatType.Spiked);
			if (!(calculatedStat <= 0f) && componentInParent.CanTakeSpikeDamage)
			{
				componentInParent.Damage(calculatedStat, wasCrit: false, null, 0f, null, Enums.DamageType.Piercing, componentInParent);
				StartCoroutine(ActivateCanTakeSpikeDamageCooldown(componentInParent, 0.5f));
			}
		}
	}

	private IEnumerator ActivateCanTakeSpikeDamageCooldown(Enemy enemy, float cooldownTime)
	{
		enemy.SetCanTakeSpikeDamage(canTakeSpikeDamage: false);
		yield return new WaitForSeconds(cooldownTime);
		enemy.SetCanTakeSpikeDamage(canTakeSpikeDamage: true);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(collision.gameObject.GetComponent<BackpackSurvivors.Game.Player.Player>() == null))
		{
			_playerInsideCollider = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!(collision.gameObject.GetComponent<BackpackSurvivors.Game.Player.Player>() == null))
		{
			_playerInsideCollider = false;
		}
	}

	internal virtual void TryDamagePlayer()
	{
		if (CanDamagePlayerForSpike())
		{
			ReceiveSpikeDamage(SingletonController<GameController>.Instance.Player);
		}
		if (CanDamagePlayer())
		{
			DealDamage();
			if (_canDamageRepeatedly)
			{
				StartCoroutine(TempDisableDamaging());
			}
			else
			{
				_canAct = false;
			}
		}
	}

	private IEnumerator TempDisableDamaging()
	{
		_damagingOnCooldown = true;
		yield return new WaitForSeconds(0.1f);
		_damagingOnCooldown = false;
	}

	internal bool CanDamagePlayer()
	{
		if (!_playerInsideCollider)
		{
			return false;
		}
		if (!_canAct)
		{
			return false;
		}
		if (!SingletonController<GameController>.Instance.Player.CanTakeDamage())
		{
			return false;
		}
		if (SingletonController<GameController>.Instance.Player.IsDead)
		{
			return false;
		}
		if (_damagingOnCooldown)
		{
			return false;
		}
		return true;
	}

	private bool CanDamagePlayerForSpike()
	{
		if (!_playerInsideCollider)
		{
			return false;
		}
		if (!_canAct)
		{
			return false;
		}
		if (SingletonController<GameController>.Instance.Player.IsDead)
		{
			return false;
		}
		if (_damagingOnCooldown)
		{
			return false;
		}
		return true;
	}
}
