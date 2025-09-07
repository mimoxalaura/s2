using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.Droppables;

internal class SpawnWeaponAttack : MonoBehaviour
{
	[SerializeField]
	protected WeaponSO _weaponSO;

	[SerializeField]
	protected bool _usePlayerAsSource;

	[SerializeField]
	private bool _canTriggerEffects;

	[SerializeField]
	private bool _canTriggerDebuffs;

	private CombatWeapon _combatWeapon;

	protected float _hellfireDamageMultiplier = 1f;

	private void Start()
	{
		InstantiateWeaponAttack(attackOriginatedFromPlayer: true);
	}

	protected void InstantiateWeaponAttack(bool attackOriginatedFromPlayer)
	{
		WeaponInstance weaponInstance = new WeaponInstance(_weaponSO);
		_combatWeapon = Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.CombatWeaponPrefab);
		Character source = GetSource(attackOriginatedFromPlayer, _usePlayerAsSource);
		Character character = SingletonController<CharactersController>.Instance.CreateAndGetDummyCharacter();
		character.transform.position = base.transform.position;
		_combatWeapon.Init(weaponInstance, 0f, source);
		_combatWeapon.ScaleWeaponDamage(_hellfireDamageMultiplier);
		WeaponAttack weaponAttack = Object.Instantiate(_weaponSO.WeaponAttackPrefab, base.transform.position, Quaternion.identity);
		weaponAttack.Init(source, character, _combatWeapon, weaponInstance.DamageInstance);
		weaponAttack.Activate(base.transform.position, _canTriggerEffects, _canTriggerDebuffs, source, character);
	}

	private Character GetSource(bool attackOriginatedFromPlayer, bool usePlayerAsSource)
	{
		Character character = null;
		if (attackOriginatedFromPlayer)
		{
			return usePlayerAsSource ? SingletonController<GameController>.Instance.Player : SingletonController<CharactersController>.Instance.CreateAndGetDummyCharacter();
		}
		return SingletonController<CharactersController>.Instance.CreateAndGetDummyCharacter();
	}

	private void OnDestroy()
	{
		if (_combatWeapon != null && _combatWeapon.isActiveAndEnabled)
		{
			Object.Destroy(_combatWeapon.gameObject);
		}
	}
}
