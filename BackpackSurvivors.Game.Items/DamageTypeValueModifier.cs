using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Items;

public class DamageTypeValueModifier
{
	public Enums.DamageType DamageType;

	public float CalculatedBonus;

	public ItemModifierSource Source;

	public ItemModifierFilter ItemModifierFilter;
}
