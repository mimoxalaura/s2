using System;
using System.Collections.Generic;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving.Backpack;

[Serializable]
internal class WeaponInBackpack
{
	internal int WeaponId { get; private set; }

	internal List<int> CellIds { get; private set; } = new List<int>();

	internal Enums.Backpack.ItemRotation ItemRotation { get; private set; }

	public WeaponInBackpack(int weaponId, List<int> cellIds, Enums.Backpack.ItemRotation itemRotation)
	{
		WeaponId = weaponId;
		CellIds = cellIds;
		ItemRotation = itemRotation;
	}
}
