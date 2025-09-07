using System.Collections.Generic;

namespace BackpackSurvivors.Game.Items;

public class ItemModifierFilter
{
	public WeaponInstance HardTargetWeaponInstance;

	public List<Condition> Conditions = new List<Condition>();
}
