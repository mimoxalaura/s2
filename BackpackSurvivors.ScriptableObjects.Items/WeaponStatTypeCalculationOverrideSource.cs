using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.ScriptableObjects.Items;

[Serializable]
public class WeaponStatTypeCalculationOverrideSource
{
	public Enums.ItemStatType WeaponStatType;

	public float Value;
}
