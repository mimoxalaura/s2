using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.ScriptableObjects.Items;

[Serializable]
public class WeaponDamageCalculationOverrideSource
{
	public Enums.ItemStatType WeaponStatType;

	public float MinValue;

	public float MaxValue;
}
