using BackpackSurvivors.Game.Effects;

namespace BackpackSurvivors.Game.Items;

public class WeaponAttackEffectModifier
{
	public WeaponAttackEffect WeaponAttackEffect;

	public ItemModifierSource Source;

	public ItemModifierFilter ItemModifierFilter;

	public static WeaponAttackEffectModifier Clone(WeaponAttackEffectModifier original)
	{
		return new WeaponAttackEffectModifier
		{
			WeaponAttackEffect = original.WeaponAttackEffect,
			Source = original.Source,
			ItemModifierFilter = original.ItemModifierFilter
		};
	}
}
