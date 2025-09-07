using System.Linq;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Combat.ScriptableObjects;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Game/Stats/WeaponStats", order = 1)]
public class WeaponStatsSO : ScriptableObject
{
	[Header("Base")]
	[SerializeField]
	public SerializableDictionaryBase<Enums.WeaponStatType, float> StatValues;

	[SerializeField]
	public SerializableDictionaryBase<Enums.DamageType, float> DamageTypeValues;

	[Header("Overrides")]
	[SerializeField]
	public WeaponStatTypeCalculationOverride[] WeaponStatTypeCalculationOverrides;

	[SerializeField]
	public WeaponDamageCalculationOverride[] WeaponDamageCalculationOverrides;

	[Header("Conditions")]
	[SerializeField]
	public ConditionSO[] Conditions;

	[Header("Formula")]
	[SerializeField]
	public SerializableDictionaryBase<Enums.WeaponStatType, FormulaSO> FormulaStats;

	public bool ContainsData()
	{
		if ((StatValues == null || !StatValues.Any()) && (FormulaStats == null || !FormulaStats.Any()) && (DamageTypeValues == null || !DamageTypeValues.Any()) && (WeaponStatTypeCalculationOverrides == null || !WeaponStatTypeCalculationOverrides.Any()))
		{
			if (WeaponDamageCalculationOverrides != null)
			{
				return WeaponDamageCalculationOverrides.Any();
			}
			return false;
		}
		return true;
	}
}
