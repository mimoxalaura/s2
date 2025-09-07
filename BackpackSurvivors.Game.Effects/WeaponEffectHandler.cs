using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public abstract class WeaponEffectHandler : MonoBehaviour
{
	[SerializeField]
	public DamageSO Damage;

	protected const float PreventSpawningWithinDistanceOfExistingEffect = 1f;

	public abstract void Init(WeaponAttack originalWeaponAttack, List<DebuffHandler> debuffHandlers, WeaponInstance weaponInstance);

	public abstract void Activate(Transform position, Character target, CombatWeapon combatWeapon, Character source);

	public abstract bool ShouldPreventSpawning(Vector2 position);

	public virtual string SetValuesForDescriptor(string placeholder)
	{
		return placeholder;
	}
}
