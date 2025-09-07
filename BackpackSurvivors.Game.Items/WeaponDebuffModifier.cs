using BackpackSurvivors.ScriptableObjects.Debuffs;

namespace BackpackSurvivors.Game.Items;

public class WeaponDebuffModifier
{
	public DebuffSO DebuffSO;

	public ItemModifierSource Source;

	public ItemModifierFilter ItemModifierFilter;

	public static WeaponDebuffModifier Clone(WeaponDebuffModifier original)
	{
		return new WeaponDebuffModifier
		{
			DebuffSO = original.DebuffSO,
			Source = original.Source,
			ItemModifierFilter = original.ItemModifierFilter
		};
	}
}
