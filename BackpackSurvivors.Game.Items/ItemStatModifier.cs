using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Items;

public class ItemStatModifier : BaseStatModifier
{
	public Enums.ItemStatType ItemStatType;

	public static ItemStatModifier Clone(ItemStatModifier original)
	{
		return new ItemStatModifier
		{
			ItemStatType = original.ItemStatType,
			CalculatedBonus = original.CalculatedBonus,
			Source = original.Source,
			ItemModifierFilter = original.ItemModifierFilter
		};
	}

	public override string ToString()
	{
		return $"{ItemStatType} {CalculatedBonus}";
	}
}
