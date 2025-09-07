using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Backpack;

public class SlotCalculator
{
	private int _gridWidth;

	public SlotCalculator(int gridWith = 10)
	{
		_gridWidth = gridWith;
	}

	public List<int> GetSlotIdsToPlaceItemIn(PlaceableInfo placeable, HoveredSlotInfo hoveredSlotInfo)
	{
		List<int> slotIdsRectangleForItemSize = GetSlotIdsRectangleForItemSize(placeable, hoveredSlotInfo, useItemWidth: true);
		List<int> list = ApplyItemSizeArrayToFoundRectangle(slotIdsRectangleForItemSize, placeable);
		if (list.Any((int i) => i < 0))
		{
			list.Clear();
		}
		return list;
	}

	public List<int> GetSlotIdsToPlaceStarsIn(PlaceableInfo placeable, int topLeftSlotId)
	{
		int topLeftIndexOfCellWithPlaceable = GetTopLeftIndexOfCellWithPlaceable(placeable);
		List<int> starIndicesWithinPlaceable = GetStarIndicesWithinPlaceable(placeable);
		return CalculateStarSlotIds(topLeftIndexOfCellWithPlaceable, topLeftSlotId, starIndicesWithinPlaceable);
	}

	private List<int> CalculateStarSlotIds(int topLeftIndex, int topLeftSlotId, List<int> starIndicesWithinPlaceable)
	{
		List<int> list = new List<int>();
		int num = topLeftIndex / 10;
		int num2 = topLeftIndex % 10;
		foreach (int item in starIndicesWithinPlaceable)
		{
			int num3 = item / 10;
			int num4 = item % 10;
			int num5 = num3 - num;
			int num6 = num4 - num2;
			int num7 = num5 * _gridWidth + num6;
			int num8 = topLeftSlotId + num7;
			if (StarSlotStillOnCorrectRow(num8, topLeftSlotId, num5, num6))
			{
				list.Add(num8);
			}
		}
		return list;
	}

	private bool StarSlotStillOnCorrectRow(int starSlot, int topLeftSlotId, int starRowOffset, int starColumnOffset)
	{
		int num = topLeftSlotId / _gridWidth;
		int num2 = topLeftSlotId % _gridWidth;
		int num3 = starSlot / _gridWidth;
		int num4 = starSlot % _gridWidth;
		bool num5 = num3 - num == starRowOffset;
		bool flag = num4 - num2 == starColumnOffset;
		return num5 && flag;
	}

	public List<int> GetSlotIdsRectangleForItemSize(PlaceableInfo placeable, HoveredSlotInfo hoveredSlotInfo, bool useItemWidth = false)
	{
		List<int> list = new List<int>();
		if (hoveredSlotInfo.Slotid == -1)
		{
			return list;
		}
		list.Add(hoveredSlotInfo.Slotid);
		int width = (useItemWidth ? placeable.ItemWidth : placeable.TotalWidth);
		int height = (useItemWidth ? placeable.ItemHeight : placeable.TotalHeight);
		List<int> additionalSlotsHorizontally = GetAdditionalSlotsHorizontally(hoveredSlotInfo, width);
		List<int> additionalSlotsVertically = GetAdditionalSlotsVertically(hoveredSlotInfo, height);
		list.AddRange(additionalSlotsHorizontally);
		list.AddRange(additionalSlotsVertically);
		AddMissingSlotsIdsToFormRectangle(list);
		return list;
	}

	public List<int> ApplyItemSizeArrayToFoundRectangle(List<int> rectangleSlotIds, PlaceableInfo placeable)
	{
		if (rectangleSlotIds.Count != placeable.ItemWidth * placeable.ItemHeight)
		{
			return new List<int>();
		}
		List<int> list = new List<int>();
		List<int> placeableIndices = GetPlaceableIndices(placeable);
		rectangleSlotIds.Sort();
		foreach (int item in placeableIndices)
		{
			list.Add(rectangleSlotIds[item]);
		}
		return list;
	}

	private List<int> GetPlaceableIndices(PlaceableInfo placeable)
	{
		List<int> list = new List<int>();
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		for (int i = 0; i < placeable.ItemSizeInfo.Count; i++)
		{
			if (placeable.ItemSizeInfo[i] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable)
			{
				int item = i / 10;
				int item2 = i % 10;
				hashSet.Add(item);
				hashSet2.Add(item2);
			}
		}
		int num = 0;
		List<int> list2 = hashSet.ToList();
		list2.Sort();
		List<int> list3 = hashSet2.ToList();
		list3.Sort();
		foreach (int item3 in list2)
		{
			foreach (int item4 in list3)
			{
				int index = item3 * 10 + item4;
				if (placeable.ItemSizeInfo[index] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable)
				{
					list.Add(num);
				}
				num++;
			}
		}
		return list;
	}

	private int GetTopLeftIndexOfCellWithPlaceable(PlaceableInfo placeable)
	{
		for (int i = 0; i < placeable.ItemSizeInfo.Count; i++)
		{
			if (placeable.ItemSizeInfo[i] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable)
			{
				return i;
			}
		}
		return -1;
	}

	private List<int> GetStarIndicesWithinPlaceable(PlaceableInfo placeable)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < placeable.ItemSizeInfo.Count; i++)
		{
			if (placeable.ItemSizeInfo[i] == Enums.Backpack.ItemSizeCellType.CellContainsStar)
			{
				list.Add(i);
			}
		}
		return list;
	}

	private void AddMissingSlotsIdsToFormRectangle(List<int> currentSlotIds)
	{
		int num = currentSlotIds.Min((int x) => x / _gridWidth);
		int num2 = currentSlotIds.Max((int x) => x / _gridWidth);
		int num3 = currentSlotIds.Min((int x) => x % _gridWidth);
		int num4 = currentSlotIds.Max((int x) => x % _gridWidth);
		for (int num5 = num; num5 <= num2; num5++)
		{
			for (int num6 = num3; num6 <= num4; num6++)
			{
				int item = _gridWidth * num5 + num6;
				if (!currentSlotIds.Contains(item))
				{
					currentSlotIds.Add(item);
				}
			}
		}
	}

	private List<int> GetAdditionalSlotsHorizontally(HoveredSlotInfo hoveredSlotInfo, int width)
	{
		List<int> list = new List<int>();
		int slotid = hoveredSlotInfo.Slotid;
		if (width % 2 == 0)
		{
			int num = (width - 1) / 2;
			AddSlotsHorizontally(slotid, list, num);
			int num2 = (hoveredSlotInfo.IsHoveredSlotOnRight ? 1 : (-1));
			int slotIdToAdd = slotid + num2 * num + num2;
			AddSlotIfOnSameRow(slotid, slotIdToAdd, list);
		}
		else
		{
			int slotsToAddOnEitherSide = (width - 1) / 2;
			AddSlotsHorizontally(slotid, list, slotsToAddOnEitherSide);
		}
		return list;
	}

	private List<int> GetAdditionalSlotsVertically(HoveredSlotInfo hoveredSlotInfo, int height)
	{
		List<int> list = new List<int>();
		int slotid = hoveredSlotInfo.Slotid;
		if (height % 2 == 0)
		{
			int num = (height - 1) / 2;
			AddSlotsVertically(slotid, list, num);
			int num2 = (hoveredSlotInfo.IsHoveredSlotOnBottom ? 1 : (-1));
			int slotIdToAdd = slotid + num2 * num * _gridWidth + num2 * _gridWidth;
			AddSlotIfOnSameColumn(slotid, slotIdToAdd, list);
		}
		else
		{
			int slotsToAddOnEitherSide = (height - 1) / 2;
			AddSlotsVertically(slotid, list, slotsToAddOnEitherSide);
		}
		return list;
	}

	private void AddSlotsHorizontally(int hoveredSlotId, List<int> result, int slotsToAddOnEitherSide)
	{
		for (int i = 1; i <= slotsToAddOnEitherSide; i++)
		{
			int slotIdToAdd = hoveredSlotId - i;
			int slotIdToAdd2 = hoveredSlotId + i;
			AddSlotIfOnSameRow(hoveredSlotId, slotIdToAdd, result);
			AddSlotIfOnSameRow(hoveredSlotId, slotIdToAdd2, result);
		}
	}

	private void AddSlotIfOnSameRow(int hoveredSlotId, int slotIdToAdd, List<int> slotIdList)
	{
		int num = hoveredSlotId / _gridWidth;
		int num2 = slotIdToAdd / _gridWidth;
		if (num == num2)
		{
			slotIdList.Add(slotIdToAdd);
		}
	}

	private void AddSlotIfOnSameColumn(int hoveredSlotId, int slotIdToAdd, List<int> slotIdList)
	{
		int num = hoveredSlotId % _gridWidth;
		int num2 = slotIdToAdd % _gridWidth;
		if (num == num2)
		{
			slotIdList.Add(slotIdToAdd);
		}
	}

	private void AddSlotsVertically(int hoveredSlotId, List<int> result, int slotsToAddOnEitherSide)
	{
		for (int i = 1; i <= slotsToAddOnEitherSide; i++)
		{
			int slotIdToAdd = hoveredSlotId - i * _gridWidth;
			int slotIdToAdd2 = hoveredSlotId + i * _gridWidth;
			AddSlotIfOnSameColumn(hoveredSlotId, slotIdToAdd, result);
			AddSlotIfOnSameColumn(hoveredSlotId, slotIdToAdd2, result);
		}
	}
}
