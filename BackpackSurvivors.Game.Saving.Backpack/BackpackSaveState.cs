using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving.Backpack;

[Serializable]
internal class BackpackSaveState : BaseSaveState
{
	internal string SaveStateName { get; private set; }

	internal List<BagInBackpack> BagsInBackpack { get; private set; } = new List<BagInBackpack>();

	internal List<WeaponInBackpack> WeaponsInBackpack { get; private set; } = new List<WeaponInBackpack>();

	internal List<ItemInBackpack> ItemsInBackpack { get; private set; } = new List<ItemInBackpack>();

	public BackpackSaveState(string saveStateName = "default")
	{
		SaveStateName = saveStateName;
	}

	internal void AddBag(int bagId, List<int> cellIds, Enums.Backpack.ItemRotation itemRotation)
	{
		BagInBackpack item = new BagInBackpack(bagId, cellIds, itemRotation);
		BagsInBackpack.Add(item);
	}

	internal void AddWeapon(int weaponId, List<int> cellIds, Enums.Backpack.ItemRotation itemRotation)
	{
		WeaponInBackpack item = new WeaponInBackpack(weaponId, cellIds, itemRotation);
		WeaponsInBackpack.Add(item);
	}

	internal void AddItem(int itemId, List<int> cellIds, Enums.Backpack.ItemRotation itemRotation)
	{
		ItemInBackpack item = new ItemInBackpack(itemId, cellIds, itemRotation);
		ItemsInBackpack.Add(item);
	}

	public override bool HasData()
	{
		if (!BagsInBackpack.Any() && !WeaponsInBackpack.Any())
		{
			return ItemsInBackpack.Any();
		}
		return true;
	}
}
