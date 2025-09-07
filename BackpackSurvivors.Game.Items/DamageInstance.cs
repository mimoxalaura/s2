using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Items;

public class DamageInstance
{
	private DamageSO _baseDamage;

	public float CalculatedMinDamage;

	public float CalculatedMaxDamage;

	public Enums.DamageType CalculatedDamageType;

	public DamageSO BaseDamage => _baseDamage;

	public float BaseMinDamage => _baseDamage.BaseMinDamage;

	public float BaseMaxDamage => _baseDamage.BaseMaxDamage;

	public Enums.DamageType DamageType => _baseDamage.BaseDamageType;

	public Enums.DamageCalculationType DamageCalculationType => _baseDamage.DamageCalculationType;

	public float WeaponPercentageUsed => _baseDamage.WeaponPercentageUsed;

	public DamageInstance(DamageSO damageSO)
	{
		_baseDamage = damageSO;
		CalculatedMinDamage = BaseMinDamage;
		CalculatedMaxDamage = BaseMaxDamage;
		CalculatedDamageType = DamageType;
	}

	public void ScaleDamage(float damageScale)
	{
		CalculatedMinDamage *= damageScale;
		CalculatedMaxDamage *= damageScale;
	}

	public void SetMinMaxDamage(float calculatedMinDamage, float calculatedMaxDamage)
	{
		CalculatedMinDamage = calculatedMinDamage;
		CalculatedMaxDamage = calculatedMaxDamage;
	}
}
