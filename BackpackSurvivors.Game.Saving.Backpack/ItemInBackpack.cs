using System;
using System.Collections.Generic;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving.Backpack;

[Serializable]
internal class ItemInBackpack
{
	internal int ItemId { get; private set; }

	internal List<int> CellIds { get; private set; } = new List<int>();

	internal Enums.Backpack.ItemRotation ItemRotation { get; private set; }

	public ItemInBackpack(int itemId, List<int> cellIds, Enums.Backpack.ItemRotation itemRotation)
	{
		ItemId = itemId;
		CellIds = cellIds;
		ItemRotation = itemRotation;
	}
}
