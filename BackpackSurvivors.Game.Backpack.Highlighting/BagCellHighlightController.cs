using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Items;
using UnityEngine;

namespace BackpackSurvivors.Game.Backpack.Highlighting;

public class BagCellHighlightController
{
	private BackpackStorage _backpackStorage;

	private DraggableBag[] _draggableBags;

	public BagCellHighlightController(BackpackStorage backpackStorage)
	{
		_backpackStorage = backpackStorage;
	}

	public void HighlightBagSlots(List<int> placeableSlotIds)
	{
		ResetBagSlotHighlights();
		List<BagInstance> list = new List<BagInstance>();
		List<int> slotStatuses = GetSlotStatuses(placeableSlotIds);
		foreach (int placeableSlotId in placeableSlotIds)
		{
			BagInstance bagInSlot = _backpackStorage.GetBagFromSlot(placeableSlotId);
			if (bagInSlot != null && !list.Contains(bagInSlot))
			{
				DraggableBag draggableBag = _draggableBags.FirstOrDefault((DraggableBag b) => b.BagInstance.Equals(bagInSlot));
				if (!(draggableBag == null))
				{
					draggableBag.HighlightBagCellsWherePlaceableWouldBePlaced(placeableSlotIds, slotStatuses);
					list.Add(bagInSlot);
				}
			}
		}
	}

	internal void ResetBagSlotHighlights()
	{
		UppdateDraggableBagsCollection();
		DraggableBag[] draggableBags = _draggableBags;
		for (int i = 0; i < draggableBags.Length; i++)
		{
			draggableBags[i].ResetBagSlotHighlights();
		}
	}

	private List<int> GetSlotStatuses(List<int> placeableSlotIds)
	{
		List<int> list = new List<int>();
		foreach (int placeableSlotId in placeableSlotIds)
		{
			if (_backpackStorage.SlotContainsOnlyBag(placeableSlotId))
			{
				list.Add(placeableSlotId);
			}
		}
		return list;
	}

	private void UppdateDraggableBagsCollection()
	{
		_draggableBags = Object.FindObjectsOfType<DraggableBag>();
	}
}
