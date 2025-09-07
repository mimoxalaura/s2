using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Items;

public class WeaponStatModifier : BaseStatModifier
{
	public Enums.WeaponStatType WeaponStatType;

	public static WeaponStatModifier Clone(WeaponStatModifier original)
	{
		return new WeaponStatModifier
		{
			WeaponStatType = original.WeaponStatType,
			CalculatedBonus = original.CalculatedBonus,
			Source = original.Source,
			ItemModifierFilter = original.ItemModifierFilter
		};
	}

	public override string ToString()
	{
		return $"{WeaponStatType} {CalculatedBonus}";
	}
}
