using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Items;

[CreateAssetMenu(fileName = "Damage", menuName = "Game/Stats/Damage", order = 3)]
public class DamageSO : ScriptableObject
{
	[SerializeField]
	public float BaseMinDamage;

	[SerializeField]
	public float BaseMaxDamage;

	[SerializeField]
	public Enums.DamageType BaseDamageType;

	[SerializeField]
	public Enums.DamageCalculationType DamageCalculationType;

	[SerializeField]
	public float WeaponPercentageUsed;
}
