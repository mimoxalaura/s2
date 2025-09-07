using System;
using System.Collections.Generic;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving.Backpack;

[Serializable]
internal class BagInBackpack
{
	internal int BagId { get; private set; }

	internal List<int> CellIds { get; private set; } = new List<int>();

	internal Enums.Backpack.ItemRotation ItemRotation { get; private set; }

	public BagInBackpack(int bagId, List<int> cellIds, Enums.Backpack.ItemRotation itemRotation)
	{
		BagId = bagId;
		CellIds = cellIds;
		ItemRotation = itemRotation;
	}
}
