using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.ScriptableObjects.Items;

[Serializable]
public class WeaponStatTypeCalculationOverride
{
	public Enums.WeaponStatType TargetWeaponStatType;

	public WeaponStatTypeCalculationOverrideSource[] WeaponStatTypeCalculationOverrideSources;

	public Enums.OverrideOrAdd OverrideOrAdd;

	public string Description;
}
