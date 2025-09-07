using System.Linq;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Combat.ScriptableObjects;

[CreateAssetMenu(fileName = "ItemStats", menuName = "Game/Stats/ItemStats", order = 1)]
public class ItemStatsSO : ScriptableObject
{
	[Header("Base")]
	[SerializeField]
	public SerializableDictionaryBase<Enums.ItemStatType, float> StatValues;

	[SerializeField]
	public SerializableDictionaryBase<Enums.DamageType, float> DamageTypeValues;

	[Header("Overrides")]
	[SerializeField]
	public WeaponDamageTypeValueOverride[] WeaponDamageTypeValueOverrides;

	[SerializeField]
	public WeaponDamageCalculationOverride[] WeaponDamageCalculationOverrides;

	[Header("Effects")]
	[SerializeField]
	public WeaponAttackEffect[] WeaponAttackEffects;

	[Header("Debuff")]
	[SerializeField]
	public DebuffSO[] DebuffSOs;

	[Header("Conditions")]
	[SerializeField]
	public ConditionSO[] Conditions;

	[Header("Formula")]
	[SerializeField]
	public SerializableDictionaryBase<Enums.ItemStatType, FormulaSO> FormulaStats;

	[SerializeField]
	public SerializableDictionaryBase<Enums.DamageType, FormulaSO> FormulaDamageTypeValues;

	public bool ContainsData()
	{
		if ((StatValues == null || !StatValues.Any()) && (FormulaStats == null || !FormulaStats.Any()) && (DamageTypeValues == null || !DamageTypeValues.Any()) && (FormulaDamageTypeValues == null || !FormulaDamageTypeValues.Any()) && (WeaponDamageCalculationOverrides == null || !WeaponDamageCalculationOverrides.Any()) && (WeaponDamageTypeValueOverrides == null || !WeaponDamageTypeValueOverrides.Any()) && (WeaponAttackEffects == null || !WeaponAttackEffects.Any()))
		{
			if (DebuffSOs != null)
			{
				return DebuffSOs.Any();
			}
			return false;
		}
		return true;
	}
}
