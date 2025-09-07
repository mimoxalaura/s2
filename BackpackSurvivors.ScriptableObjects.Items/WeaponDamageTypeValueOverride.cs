using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.ScriptableObjects.Items;

[Serializable]
public class WeaponDamageTypeValueOverride
{
	public Enums.DamageType SourceWeaponDamageType;

	public Enums.DamageType TargetWeaponDamageType;
}
