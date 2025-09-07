using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class BackpackStorageSaveState
{
	public Enums.Backpack.GridCellStatus[] GridCells;

	public Dictionary<int, BagInstance> BagsInGridCells;

	public Dictionary<int, WeaponInstance> WeaponsInGridCells;

	public Dictionary<int, ItemInstance> ItemsInGridCells;

	public List<KeyValuePair<int, ItemInstance>> StarredCellsByItems;

	public List<KeyValuePair<int, WeaponInstance>> StarredCellsByWeapons;

	public BackpackStorageSaveState()
	{
		Init();
	}

	private void Init()
	{
		BagsInGridCells = new Dictionary<int, BagInstance>();
		WeaponsInGridCells = new Dictionary<int, WeaponInstance>();
		ItemsInGridCells = new Dictionary<int, ItemInstance>();
		StarredCellsByItems = new List<KeyValuePair<int, ItemInstance>>();
		StarredCellsByWeapons = new List<KeyValuePair<int, WeaponInstance>>();
	}

	public void SetState(Enums.Backpack.GridCellStatus[] gridCells, Dictionary<int, BagInstance> bagsInGridCells, Dictionary<int, WeaponInstance> weaponsInGridCells, Dictionary<int, ItemInstance> itemsInGridCells, List<KeyValuePair<int, ItemInstance>> starredCellsByItems, List<KeyValuePair<int, WeaponInstance>> starredCellsByWeapons)
	{
		GridCells = gridCells;
		BagsInGridCells = bagsInGridCells;
		WeaponsInGridCells = weaponsInGridCells;
		ItemsInGridCells = itemsInGridCells;
		StarredCellsByItems = starredCellsByItems;
		StarredCellsByWeapons = starredCellsByWeapons;
	}
}
