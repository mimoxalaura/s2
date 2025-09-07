using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Bosses.Infernal_Destroyer;

internal class EyeTowerGroundEffect : MonoBehaviour
{
	[SerializeField]
	private DamageSO damageToPlayerOnStay;

	private bool _playerInsideCollider;

	private void Start()
	{
		StartCoroutine(DestroyAfterDelay());
	}

	private void Update()
	{
		if (!SingletonController<GameController>.Instance.GamePaused && _playerInsideCollider)
		{
			DealDamage();
		}
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

	private IEnumerator DestroyAfterDelay()
	{
		yield return new WaitForSeconds(3.5f);
		Object.Destroy(base.gameObject);
	}

	internal virtual void DealDamage()
	{
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		if (player.CanTakeDamage())
		{
			bool num = player.CheckForDodge();
			DamageInstance damageInstance = new DamageInstance(damageToPlayerOnStay);
			if (!num)
			{
				bool wasCrit;
				float damage = DamageEngine.CalculateDamage(new Dictionary<Enums.WeaponStatType, float>(), new Dictionary<Enums.ItemStatType, float>(), SingletonController<GameController>.Instance.Player.CalculatedStats, damageInstance, Enums.Enemies.EnemyType.Player, out wasCrit);
				player.Damage(damage, wasCrit, base.gameObject, 0f, null, damageInstance.CalculatedDamageType);
			}
			else
			{
				player.Dodged();
				player.SetImmunityForDuration(Enums.ImmunitySource.DamageTaken, 0.5f);
			}
		}
	}
}
