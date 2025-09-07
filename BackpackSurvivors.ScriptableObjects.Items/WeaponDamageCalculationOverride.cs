using System;

namespace BackpackSurvivors.ScriptableObjects.Items;

[Serializable]
public class WeaponDamageCalculationOverride
{
	public WeaponDamageCalculationOverrideSource[] WeaponDamageCalculationOverrideSources;

	public string Description;
}
